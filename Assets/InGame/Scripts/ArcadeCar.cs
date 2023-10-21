using System;
using UnityEngine;
using System.Collections;
using Photon.Pun;
public class ArcadeCar : MonoBehaviour
{
    

    const int WHEEL_LEFT_INDEX = 0;
    const int WHEEL_RIGHT_INDEX = 1;

    const float wheelWidth = 0.085f;



    public Vector3 centerOfMass = Vector3.zero;

    [Header("PUN")]

    [SerializeField] PhotonView view;
    public TrailRenderer[] Tyremarks;


    [Header("Engine")]


    public AnimationCurve accelerationCurve = AnimationCurve.Linear(0.0f, 0.0f, 5.0f, 100.0f);

    public AnimationCurve accelerationCurveReverse = AnimationCurve.Linear(0.0f, 0.0f, 5.0f, 20.0f);

    public int reverseEvaluationAccuracy = 25;


 
    public AnimationCurve steerAngleLimit = AnimationCurve.Linear(0.0f, 35.0f, 100.0f, 5.0f);


    public AnimationCurve steeringResetSpeed = AnimationCurve.EaseInOut(0.0f, 30.0f, 100.0f, 10.0f);

 
    public AnimationCurve steeringSpeed = AnimationCurve.Linear(0.0f, 2.0f, 100.0f, 0.5f);



    public bool debugDraw = true;



    public float flightStabilizationForce = 8.0f;

 
    public float flightStabilizationDamping = 0.0f;

  
    public float handBrakeSlipperyTime = 2.2f;

    public bool controllable = true;

  
    public AnimationCurve downForceCurve = AnimationCurve.Linear(0.0f, 0.0f, 200.0f, 100.0f);


    public float downForce = 5.0f;



    public Axle[] axles = new Axle[2];




    float afterFlightSlipperyTiresTime = 0.0f;
    float brakeSlipperyTiresTime = 0.0f;
    float handBrakeSlipperyTiresTime = 0.0f;
    bool isBrake = false;
    bool isHandBrake = false;
    bool isAcceleration = false;
    bool isReverseAcceleration = false;
    float accelerationForceMagnitude = 0.0f;
    Rigidbody rb = null;

    // UI style for debug render
    static GUIStyle style = new GUIStyle();

    // For alloc-free raycasts
    Ray wheelRay = new Ray();
    RaycastHit[] wheelRayHits = new RaycastHit[16];




    void Start()
    {
       
        view = GetComponent<PhotonView>();
        if (view.IsMine)
        {
           
            style.normal.textColor = Color.red;

            rb = GetComponent<Rigidbody>();
            rb.centerOfMass = centerOfMass;


        }

    }

    float GetHandBrakeK()
    {
        float x = handBrakeSlipperyTiresTime / Math.Max(0.1f, handBrakeSlipperyTime);
        // smoother step
        x = x * x * x * (x * (x * 6 - 15) + 10);//Hermite interpolation
        return x;
    }

    float GetSteeringHandBrakeK() 
    {
        // 0.4 - pressed
        // 1.0 - not pressed
        float steeringK = Mathf.Clamp01(0.4f + (1.0f - GetHandBrakeK()) * 0.6f);
        return steeringK;
    }

    float GetAccelerationForceMagnitude(AnimationCurve accCurve, float speedMetersPerSec, float dt)
    {
        float speedKmH = speedMetersPerSec * 3.6f;

        float mass = rb.mass;

        int numKeys = accCurve.length;
        if (numKeys == 0)
        {
            return 0.0f;
        }

        if (numKeys == 1)
        {
            float desiredSpeed = accCurve.keys[0].value;
            float acc = (desiredSpeed - speedKmH);

            //to meters per sec
            acc /= 3.6f;
            float forceMag = (acc * mass);
            forceMag = Mathf.Max(forceMag, 0.0f);
            return forceMag;
        }

        //binary search to reverse evaluate curve
        float minTime = accCurve.keys[0].time;
        float maxTime = accCurve.keys[numKeys - 1].time;

        float step = (maxTime - minTime);

        float timeNow = minTime;
        bool isResultFound = false;

        // Only actually do time for speed search if we're below our max speed
        if (speedKmH < accCurve.keys[numKeys - 1].value)
        {
            for (int i = 0; i < reverseEvaluationAccuracy; i++)
            {
                float cur_speed = accCurve.Evaluate(timeNow);
                float cur_speed_diff = Math.Abs(speedKmH - cur_speed);

                float stepTime = timeNow + step;
                float step_speed = accCurve.Evaluate(stepTime);
                float step_speed_diff = Math.Abs(speedKmH - step_speed);

                if (step_speed_diff < cur_speed_diff)
                {
                    timeNow = stepTime;
                    cur_speed = step_speed;
                }

                step = Math.Abs(step / 2) * Mathf.Sign(speedKmH - cur_speed);
            }
            isResultFound = true;
        }

        if (isResultFound)
        {
          

            float speed_desired = accCurve.Evaluate(timeNow + dt);

            float acc = (speed_desired - speedKmH);
            //to meters per sec
            acc /= 3.6f;
            float forceMag = (acc * mass);
            forceMag = Mathf.Max(forceMag, 0.0f);
            return forceMag;

        }

        if (debugDraw)
        {
            Debug.Log("Max speed reached!");
        }

        float _desiredSpeed = accCurve.keys[numKeys - 1].value;
        float _acc = (_desiredSpeed - speedKmH);
        //to meters per sec
        _acc /= 3.6f;
        float _forceMag = (_acc * mass);
        _forceMag = Mathf.Max(_forceMag, 0.0f);
        return _forceMag;

    }

    public float GetSpeed()
    {
        Vector3 velocity = rb.velocity;

        Vector3 wsForward = rb.transform.rotation * Vector3.forward;
        float vProj = Vector3.Dot(velocity, wsForward);
        Vector3 projVelocity = vProj * wsForward;
        float speed = projVelocity.magnitude * Mathf.Sign(vProj);
        return speed;
    }

    //TODO: Refactor (remove this func, GetAccelerationForceMagnitude is enough)
    float CalcAccelerationForceMagnitude()
    {
        if (!isAcceleration && !isReverseAcceleration)
        {
            return 0.0f;
        }

        float speed = GetSpeed();
        float dt = Time.fixedDeltaTime;

        if (isAcceleration)
        {
            float forceMag = GetAccelerationForceMagnitude(accelerationCurve, speed, dt);
            return forceMag;
        }
        else
        {
            float forceMag = GetAccelerationForceMagnitude(accelerationCurveReverse, -speed, dt);
            return -forceMag;
        }

    }

    float GetSteerAngleLimitInDeg(float speedMetersPerSec)
    {
        float speedKmH = speedMetersPerSec * 3.6f;

        // maximum angle limit when hand brake is pressed
        speedKmH *= GetSteeringHandBrakeK();

        float limitDegrees = steerAngleLimit.Evaluate(speedKmH);

        return limitDegrees;
    }

    void UpdateInput()
    {
        float v = Input.GetAxis("Vertical");
        float h = Input.GetAxis("Horizontal");
        //Debug.Log (string.Format ("H = {0}", h));

        if (!controllable)
        {
            v = 0.0f;
            h = 0.0f;
        }


        bool isBrakeNow = false;
        bool isHandBrakeNow = Input.GetKey(KeyCode.Space) && controllable;

        float speed = GetSpeed();
        isAcceleration = false;
        isReverseAcceleration = false;
        if (v > 0.4f)
        {
            if (speed < -0.5f)
            {
                isBrakeNow = true;
            }
            else
            {
                isAcceleration = true;
            }
        }
        else if (v < -0.4f)
        {
            if (speed > 0.5f)
            {
                isBrakeNow = true;
            }
            else
            {
                isReverseAcceleration = true;
            }
        }

        // Make tires more slippery (for 1 seconds) when player hit brakes
        if (isBrakeNow == true && isBrake == false)
        {
            brakeSlipperyTiresTime = 1.0f;
        }

        // slippery tires while handsbrakes are pressed
        if (isHandBrakeNow == true)
        {
            handBrakeSlipperyTiresTime = Math.Max(0.1f, handBrakeSlipperyTime);
        }

        isBrake = isBrakeNow;

        // hand brake + acceleration = power slide
        isHandBrake = isHandBrakeNow && !isAcceleration && !isReverseAcceleration;

        axles[0].brakeLeft = isBrake;
        axles[0].brakeRight = isBrake;
        axles[1].brakeLeft = isBrake;
        axles[1].brakeRight = isBrake;

        axles[0].handBrakeLeft = isHandBrake;
        axles[0].handBrakeRight = isHandBrake;
        axles[1].handBrakeLeft = isHandBrake;
        axles[1].handBrakeRight = isHandBrake;


        //TODO: axles[0] always used for steering

        if (Mathf.Abs(h) > 0.001f)
        {
            float speedKmH = Mathf.Abs(speed) * 3.6f;

            // maximum steer speed when hand-brake is pressed
            speedKmH *= GetSteeringHandBrakeK();

            float steerSpeed = steeringSpeed.Evaluate(speedKmH);

            float newSteerAngle = axles[0].steerAngle + (h * steerSpeed);
            float sgn = Mathf.Sign(newSteerAngle);

            float steerLimit = GetSteerAngleLimitInDeg(speed);

            newSteerAngle = Mathf.Min(Math.Abs(newSteerAngle), steerLimit) * sgn;

            axles[0].steerAngle = newSteerAngle;
        }
        else
        {
            float speedKmH = Mathf.Abs(speed) * 3.6f;

            float angleReturnSpeedDegPerSec = steeringResetSpeed.Evaluate(speedKmH);

            angleReturnSpeedDegPerSec = Mathf.Lerp(0.0f, angleReturnSpeedDegPerSec, Mathf.Clamp01(speedKmH / 2.0f));


            float ang = axles[0].steerAngle;
            float sgn = Mathf.Sign(ang);

            ang = Mathf.Abs(ang);

            ang -= angleReturnSpeedDegPerSec * Time.fixedDeltaTime;
            ang = Mathf.Max(ang, 0.0f) * sgn;

            axles[0].steerAngle = ang;
        }
    }

    void Update()
    {
        if (view.IsMine)
        {
           
            ApplyVisual();


            if (isHandBrake)
            {
                skidEmmiter(true);

                AudioManger.instance.Play("Drift");

            }
            else
            {
                AudioManger.instance.Stop("Drift");
                skidEmmiter(false);
            }
        }




     
    }

    private void skidEmmiter(bool t)
    {
        foreach (TrailRenderer T in Tyremarks)
        {
            T.emitting = t;
        }
    }
    void FixedUpdate()
    {
        if (view.IsMine)
        {
            UpdateInput();

            accelerationForceMagnitude = CalcAccelerationForceMagnitude();

            // 0.8 - pressed
            // 1.0 - not pressed
            float accelerationK = Mathf.Clamp01(0.8f + (1.0f - GetHandBrakeK()) * 0.2f);
            accelerationForceMagnitude *= accelerationK;

            CalculateAckermannSteering();

            int numberOfPoweredWheels = 0;
            for (int axleIndex = 0; axleIndex < axles.Length; axleIndex++)
            {
                if (axles[axleIndex].isPowered)
                {
                    numberOfPoweredWheels += 2;
                }
            }


            int totalWheelsCount = axles.Length * 2;
            for (int axleIndex = 0; axleIndex < axles.Length; axleIndex++)
            {
                CalculateAxleForces(axles[axleIndex], totalWheelsCount, numberOfPoweredWheels);
            }

            bool allWheelIsOnAir = true;
            for (int axleIndex = 0; axleIndex < axles.Length; axleIndex++)
            {
                if (axles[axleIndex].wheelDataL.isOnGround || axles[axleIndex].wheelDataR.isOnGround)
                {
                    allWheelIsOnAir = false;
                    break;
                }
            }

            if (allWheelIsOnAir)
            {
                // set after flight tire slippery time (1 sec)
                afterFlightSlipperyTiresTime = 1.0f;

                // Try to keep vehicle parallel to the ground while jumping
                Vector3 carUp = transform.TransformDirection(new Vector3(0.0f, 1.0f, 0.0f));
                Vector3 worldUp = new Vector3(0.0f, 1.0f, 0.0f);



                Vector3 axis = Vector3.Cross(carUp, worldUp);
                //axis.Normalize ();

                float mass = rb.mass;

                // angular velocity damping
                Vector3 angVel = rb.angularVelocity;

                Vector3 angVelDamping = angVel;
                angVelDamping.y = 0.0f;
                angVelDamping = angVelDamping * Mathf.Clamp01(flightStabilizationDamping * Time.fixedDeltaTime);

                //Debug.Log(string.Format("Ang {0}, Damping {1}", angVel, angVelDamping));
                rb.angularVelocity = angVel - angVelDamping;

                // in flight roll stabilization
                rb.AddTorque(axis * flightStabilizationForce * mass);
            }
            else
            {
                // downforce
                Vector3 carDown = transform.TransformDirection(new Vector3(0.0f, -1.0f, 0.0f));

                float speed = GetSpeed();
                float speedKmH = Mathf.Abs(speed) * 3.6f;

                float downForceAmount = downForceCurve.Evaluate(speedKmH) / 100.0f;

                float mass = rb.mass;

                rb.AddForce(carDown * mass * downForceAmount * downForce);

                //Debug.Log(string.Format("{0} downforce", downForceAmount * downForce));
            }

            if (afterFlightSlipperyTiresTime > 0.0f)
            {
                afterFlightSlipperyTiresTime -= Time.fixedDeltaTime;
            }
            else
            {
                afterFlightSlipperyTiresTime = 0.0f;
            }

            if (brakeSlipperyTiresTime > 0.0f)
            {
                brakeSlipperyTiresTime -= Time.fixedDeltaTime;
            }
            else
            {
                brakeSlipperyTiresTime = 0.0f;
            }

            if (handBrakeSlipperyTiresTime > 0.0f)
            {
                handBrakeSlipperyTiresTime -= Time.fixedDeltaTime;
            }
            else
            {
                handBrakeSlipperyTiresTime = 0.0f;
            }
        }
        

    }





    void AddForceAtPosition(Vector3 force, Vector3 position)
    {
        rb.AddForceAtPosition(force, position);
        //Debug.DrawRay(position, force, Color.magenta);
    }



    bool RayCast(Ray ray, float maxDistance, ref RaycastHit nearestHit)
    {
        int numHits = Physics.RaycastNonAlloc(wheelRay, wheelRayHits, maxDistance);
        if (numHits == 0)
        {
            return false;
        }

        // Find the nearest hit point and filter invalid hits
        ////////////////////////////////////////////////////////////////////////////////////////

        nearestHit.distance = float.MaxValue;
        for (int j = 0; j < numHits; j++)
        {
            if (wheelRayHits[j].collider != null && wheelRayHits[j].collider.isTrigger)
            {
                // skip triggers
                continue;
            }

            // Skip contacts with car body
            if (wheelRayHits[j].rigidbody == rb)
            {
                continue;
            }

            // Skip contacts with strange normals (walls?)
            if (Vector3.Dot(wheelRayHits[j].normal, new Vector3(0.0f, 1.0f, 0.0f)) < 0.6f)
            {
                continue;
            }

            if (wheelRayHits[j].distance < nearestHit.distance)
            {
                nearestHit = wheelRayHits[j];
            }
        }
        ////////////////////////////////////////////////////////////////////////////////////////

        return (nearestHit.distance <= maxDistance);
    }


    void CalculateWheelForces(Axle axle, Vector3 wsDownDirection, WheelData wheelData, Vector3 wsAttachPoint, int wheelIndex, int totalWheelsCount, int numberOfPoweredWheels)
    {
        float dt = Time.fixedDeltaTime;


        // Get wheel world space rotation and axes
        Quaternion localWheelRot = Quaternion.Euler(new Vector3(0.0f, wheelData.yawRad * Mathf.Rad2Deg, 0.0f));
        Quaternion wsWheelRot = transform.rotation * localWheelRot;

        // Wheel axle left direction
        Vector3 wsAxleLeft = wsWheelRot * Vector3.left;

        wheelData.isOnGround = false;
        wheelRay.direction = wsDownDirection;


        // Ray cast (better to use shape cast here, but Unity does not support shape casts)

        float traceLen = axle.lengthRelaxed + axle.radius;

        wheelRay.origin = wsAttachPoint + wsAxleLeft * wheelWidth;
        RaycastHit s1 = new RaycastHit();
        bool b1 = RayCast(wheelRay, traceLen, ref s1);

        wheelRay.origin = wsAttachPoint - wsAxleLeft * wheelWidth;
        RaycastHit s2 = new RaycastHit();
        bool b2 = RayCast(wheelRay, traceLen, ref s2);

        wheelRay.origin = wsAttachPoint;
        bool isCollided = RayCast(wheelRay, traceLen, ref wheelData.touchPoint);

        // No wheel contant found
        if (!isCollided || !b1 || !b2)
        {
            // wheel do not touch the ground (relaxing spring)
            float relaxSpeed = 1.0f;
            wheelData.compressionPrev = wheelData.compression;
            wheelData.compression = Mathf.Clamp01(wheelData.compression - dt * relaxSpeed);
            return;
        }

        // Consider wheel radius
        float suspLenNow = wheelData.touchPoint.distance - axle.radius;

        Debug.AssertFormat(suspLenNow <= traceLen, "Sanity check failed.");

        wheelData.isOnGround = true;



        ////////////////////////////////////////////////////////////////////////////////////////////////////

        float suspForceMag = 0.0f;

 
        wheelData.compression = 1.0f - Mathf.Clamp01(suspLenNow / axle.lengthRelaxed);

        wheelData.debugText = wheelData.compression.ToString("F2");

        // Hooke's law (springs)
        // F = -k x 

        // Spring force (try to reset compression from spring)
        float springForce = wheelData.compression * -axle.stiffness;
        suspForceMag += springForce;

        // Damping force (try to reset velocity to 0)
        float suspCompressionVelocity = (wheelData.compression - wheelData.compressionPrev) / dt;
        wheelData.compressionPrev = wheelData.compression;

        float damperForce = -suspCompressionVelocity * axle.damping;
        suspForceMag += damperForce;

        // Only consider component of force that is along the contact normal.
        float denom = Vector3.Dot(wheelData.touchPoint.normal, -wsDownDirection);
        suspForceMag *= denom;

        // Apply suspension force
        Vector3 suspForce = wsDownDirection * suspForceMag;
        AddForceAtPosition(suspForce, wheelData.touchPoint.point);


        Vector3 wheelVelocity = rb.GetPointVelocity(wheelData.touchPoint.point);

        // Contact basis (can be different from wheel basis)
        Vector3 c_up = wheelData.touchPoint.normal;
        Vector3 c_left = (s1.point - s2.point).normalized;
        Vector3 c_fwd = Vector3.Cross(c_up, c_left);

        // Calculate sliding velocity (velocity without normal force)
        Vector3 lvel = Vector3.Dot(wheelVelocity, c_left) * c_left;
        Vector3 fvel = Vector3.Dot(wheelVelocity, c_fwd) * c_fwd;
        Vector3 slideVelocity = (lvel + fvel) * 0.5f;

        // Calculate current sliding force
        Vector3 slidingForce = (slideVelocity * rb.mass / dt) / (float)totalWheelsCount;

        if (debugDraw)
        {
            Debug.DrawRay(wheelData.touchPoint.point, slideVelocity, Color.red);
        }

        float laterialFriction = Mathf.Clamp01(axle.laterialFriction);


        float slipperyK = 1.0f;

        //Simulate slippery tires
        if (afterFlightSlipperyTiresTime > 0.0f)
        {
            float slippery = Mathf.Lerp(1.0f, axle.afterFlightSlipperyK, Mathf.Clamp01(afterFlightSlipperyTiresTime));
            slipperyK = Mathf.Min(slipperyK, slippery);
        }

        if (brakeSlipperyTiresTime > 0.0f)
        {
            float slippery = Mathf.Lerp(1.0f, axle.brakeSlipperyK, Mathf.Clamp01(brakeSlipperyTiresTime));
            slipperyK = Mathf.Min(slipperyK, slippery);
        }

        float handBrakeK = GetHandBrakeK();
        if (handBrakeK > 0.0f)
        {
            float slippery = Mathf.Lerp(1.0f, axle.handBrakeSlipperyK, handBrakeK);
            slipperyK = Mathf.Min(slipperyK, slippery);
        }

        laterialFriction = laterialFriction * slipperyK;


        // Simulate perfect static friction
        Vector3 frictionForce = -slidingForce * laterialFriction;

        // Remove friction along roll-direction of wheel 
        Vector3 longitudinalForce = Vector3.Dot(frictionForce, c_fwd) * c_fwd;

        // Apply braking force or rolling resistance force or nothing
        bool isBrakeEnabled = (wheelIndex == WHEEL_LEFT_INDEX) ? axle.brakeLeft : axle.brakeRight;
        bool isHandBrakeEnabled = (wheelIndex == WHEEL_LEFT_INDEX) ? axle.handBrakeLeft : axle.handBrakeRight;
        if (isBrakeEnabled || isHandBrakeEnabled)
        {
            float clampedMag = Mathf.Clamp(axle.brakeForceMag * rb.mass, 0.0f, longitudinalForce.magnitude);
            Vector3 brakeForce = longitudinalForce.normalized * clampedMag;

            if (isHandBrakeEnabled)
            {
                // hand brake are not powerful enough ;)
                brakeForce = brakeForce * 0.8f;
            }

            longitudinalForce -= brakeForce;
        }
        else
        {

            // Apply rolling-friction (automatic slow-down) only if player don't press to the accelerator
            if (!isAcceleration && !isReverseAcceleration)
            {
                float rollingK = 1.0f - Mathf.Clamp01(axle.rollingFriction);
                longitudinalForce *= rollingK;
            }
        }

        frictionForce -= longitudinalForce;

        if (debugDraw)
        {
            Debug.DrawRay(wheelData.touchPoint.point, frictionForce, Color.red);
            Debug.DrawRay(wheelData.touchPoint.point, longitudinalForce, Color.white);
        }

        // Apply resulting force
        AddForceAtPosition(frictionForce, wheelData.touchPoint.point);


        // Engine force
        if (!isBrake && axle.isPowered && Mathf.Abs(accelerationForceMagnitude) > 0.01f)
        {
            Vector3 accForcePoint = wheelData.touchPoint.point - (wsDownDirection * 0.2f);
            Vector3 engineForce = c_fwd * accelerationForceMagnitude / (float)numberOfPoweredWheels / dt;
            AddForceAtPosition(engineForce, accForcePoint);

            if (debugDraw)
            {
                Debug.DrawRay(accForcePoint, engineForce, Color.green);
            }
        }
        //




    }


    void CalculateAxleForces(Axle axle, int totalWheelsCount, int numberOfPoweredWheels)
    {
        Vector3 wsDownDirection = transform.TransformDirection(Vector3.down);
        wsDownDirection.Normalize();

        Vector3 localL = new Vector3(axle.width * -0.5f, axle.offset.y, axle.offset.x);
        Vector3 localR = new Vector3(axle.width * 0.5f, axle.offset.y, axle.offset.x);

        Vector3 wsL = transform.TransformPoint(localL);
        Vector3 wsR = transform.TransformPoint(localR);

        // For each wheel
        for (int wheelIndex = 0; wheelIndex < 2; wheelIndex++)
        {
            WheelData wheelData = (wheelIndex == WHEEL_LEFT_INDEX) ? axle.wheelDataL : axle.wheelDataR;
            Vector3 wsFrom = (wheelIndex == WHEEL_LEFT_INDEX) ? wsL : wsR;

            CalculateWheelForces(axle, wsDownDirection, wheelData, wsFrom, wheelIndex, totalWheelsCount, numberOfPoweredWheels);
        }

        float travelL = 1.0f - Mathf.Clamp01(axle.wheelDataL.compression);
        float travelR = 1.0f - Mathf.Clamp01(axle.wheelDataR.compression);

        float antiRollForce = (travelL - travelR) * axle.antiRollForce;
        if (axle.wheelDataL.isOnGround)
        {
            AddForceAtPosition(wsDownDirection * antiRollForce, axle.wheelDataL.touchPoint.point);
            if (debugDraw)
            {
                Debug.DrawRay(axle.wheelDataL.touchPoint.point, wsDownDirection * antiRollForce, Color.magenta);
            }
        }

        if (axle.wheelDataR.isOnGround)
        {
            AddForceAtPosition(wsDownDirection * -antiRollForce, axle.wheelDataR.touchPoint.point);
            if (debugDraw)
            {
                Debug.DrawRay(axle.wheelDataR.touchPoint.point, wsDownDirection * -antiRollForce, Color.magenta);
            }
        }

    }


    void CalculateAckermannSteering()
    {
        // Copy desired steering
        for (int axleIndex = 0; axleIndex < axles.Length; axleIndex++)
        {
            float steerAngleRad = axles[axleIndex].steerAngle * Mathf.Deg2Rad;

            axles[axleIndex].wheelDataL.yawRad = steerAngleRad;
            axles[axleIndex].wheelDataR.yawRad = steerAngleRad;
        }

        if (axles.Length != 2)
        {
            Debug.LogWarning("Ackermann work only for 2 axle vehicles.");
            return;
        }

        Axle frontAxle = axles[0];
        Axle rearAxle = axles[1];

        if (Mathf.Abs(rearAxle.steerAngle) > 0.0001f)
        {
            Debug.LogWarning("Ackermann work only for vehicles with forward steering axle.");
            return;
        }

        // Calculate our chassis (remove scale)
        Vector3 axleDiff = transform.TransformPoint(new Vector3(0.0f, frontAxle.offset.y, frontAxle.offset.x)) - transform.TransformPoint(new Vector3(0.0f, rearAxle.offset.y, rearAxle.offset.x));
        float axleSeparation = axleDiff.magnitude;

        Vector3 wheelDiff = transform.TransformPoint(new Vector3(frontAxle.width * -0.5f, frontAxle.offset.y, frontAxle.offset.x)) - transform.TransformPoint(new Vector3(frontAxle.width * 0.5f, frontAxle.offset.y, frontAxle.offset.x));
        float wheelsSeparation = wheelDiff.magnitude;

        // Get turning circle radius for steering angle input
        float turningCircleRadius = axleSeparation / Mathf.Tan(frontAxle.steerAngle * Mathf.Deg2Rad);

        // Make front inside tire turn sharper and outside tire less sharp based on turning circle radius
        float steerAngleLeft = Mathf.Atan(axleSeparation / (turningCircleRadius + (wheelsSeparation / 2)));
        float steerAngleRight = Mathf.Atan(axleSeparation / (turningCircleRadius - (wheelsSeparation / 2)));

        frontAxle.wheelDataL.yawRad = steerAngleLeft;
        frontAxle.wheelDataR.yawRad = steerAngleRight;
    }


    void CalculateWheelVisualTransform(Vector3 wsAttachPoint, Vector3 wsDownDirection, Axle axle, WheelData data, int wheelIndex, float visualRotationRad, out Vector3 pos, out Quaternion rot)
    {
        float suspCurrentLen = Mathf.Clamp01(1.0f - data.compression) * axle.lengthRelaxed;

        pos = wsAttachPoint + wsDownDirection * suspCurrentLen;
    
        float additionalYaw = 0.0f;
        float additionalMul = Mathf.Rad2Deg;
        if (wheelIndex == WHEEL_LEFT_INDEX)
        {
            additionalYaw = 180.0f;
            additionalMul = -Mathf.Rad2Deg;
        }

        Quaternion localWheelRot = Quaternion.Euler(new Vector3(data.visualRotationRad * additionalMul, additionalYaw + data.yawRad * Mathf.Rad2Deg, 0.0f));
        rot = transform.rotation * localWheelRot;
        float a = additionalYaw + data.yawRad * Mathf.Rad2Deg;
     
    }

    void CalculateWheelRotationFromSpeed(Axle axle, WheelData data, Vector3 wsPos)
    {
        if (rb == null)
        {
            data.visualRotationRad = 0.0f;
            return;
        }

        Quaternion localWheelRot = Quaternion.Euler(new Vector3(0.0f, data.yawRad * Mathf.Rad2Deg, 0.0f));
        Quaternion wsWheelRot = transform.rotation * localWheelRot;

        Vector3 wsWheelForward = wsWheelRot * Vector3.forward;
        Vector3 velocityQueryPos = data.isOnGround ? data.touchPoint.point : wsPos;
        Vector3 wheelVelocity = rb.GetPointVelocity(velocityQueryPos);
        // Longitudinal speed (meters/sec)
        float tireLongSpeed = Vector3.Dot(wheelVelocity, wsWheelForward);

        // Circle length = 2 * PI * R
        float wheelLengthMeters = 2 * Mathf.PI * axle.radius;

        // Wheel "Revolutions per second";
        float rps = tireLongSpeed / wheelLengthMeters;

        float deltaRot = Mathf.PI * 2.0f * rps * Time.deltaTime;

        data.visualRotationRad += deltaRot;
     
            // If this is the local player, send the updated rotation to others
         
      
    }

 

    void ApplyVisual()
    {
        Vector3 wsDownDirection = transform.TransformDirection(Vector3.down);
        wsDownDirection.Normalize();

        for (int axleIndex = 0; axleIndex < axles.Length; axleIndex++)
        {
            Axle axle = axles[axleIndex];

            Vector3 localL = new Vector3(axle.width * -0.5f, axle.offset.y, axle.offset.x);
            Vector3 localR = new Vector3(axle.width * 0.5f, axle.offset.y, axle.offset.x);

            Vector3 wsL = transform.TransformPoint(localL);
            Vector3 wsR = transform.TransformPoint(localR);

            Vector3 wsPos;
            Quaternion wsRot;
            Vector3 wsScale;

            if (axle.wheelVisualLeft != null)
            {
                CalculateWheelVisualTransform(wsL, wsDownDirection, axle, axle.wheelDataL, WHEEL_LEFT_INDEX, axle.wheelDataL.visualRotationRad, out wsPos, out wsRot);
             
                axle.wheelVisualLeft.transform.position = wsPos;
                axle.wheelVisualLeft.transform.rotation = wsRot;

                wsScale = new Vector3(axle.radius, axle.radius, axle.radius) * axle.visualScale;
                axle.wheelVisualLeft.transform.localScale = wsScale;

                if (!isBrake)
                {
                    CalculateWheelRotationFromSpeed(axle, axle.wheelDataL, wsPos);
                }

                // If this is the local player, send the updated rotation to others
           
            }

            if (axle.wheelVisualRight != null)
            {
                CalculateWheelVisualTransform(wsR, wsDownDirection, axle, axle.wheelDataR, WHEEL_RIGHT_INDEX, axle.wheelDataR.visualRotationRad, out wsPos, out wsRot);

                axle.wheelVisualRight.transform.position = wsPos;
                axle.wheelVisualRight.transform.rotation = wsRot;

                wsScale = new Vector3(axle.radius, axle.radius, axle.radius) * axle.visualScale;
                axle.wheelVisualRight.transform.localScale = wsScale;

                if (!isBrake)
                {
                    CalculateWheelRotationFromSpeed(axle, axle.wheelDataR, wsPos);
                }

             
            }
        }

    }



}

