using System;
using System.Collections;
using UnityEngine;

namespace Tarodev {
    
    public class Missile : MonoBehaviour {
        [Header("REFERENCES")] 
        [SerializeField] private Rigidbody _rb;
        public Target _target;
        [SerializeField] private GameObject _explosionPrefab;

        [Header("MOVEMENT")] 
        [SerializeField] private float _speed = 15;
        [SerializeField] private float _rotateSpeed = 95;

        [Header("PREDICTION")] 
        [SerializeField] private float _maxDistancePredict = 100;
        [SerializeField] private float _minDistancePredict = 5;
        [SerializeField] private float _maxTimePrediction = 5;
        private Vector3 _standardPrediction, _deviatedPrediction;

        [Header("DEVIATION")] 
        [SerializeField] private float _deviationAmount = 50;
        [SerializeField] private float _deviationSpeed = 2;

       public bool _missileLaunched = false;

        private PlayerHealth PH;
        private void OnEnable()
        {
            StartCoroutine(isLaunched());
            StartCoroutine(DisableAfterDelay());
        }
        private void FixedUpdate() {
          
           

            if (_missileLaunched)
            {
                _rb.velocity = transform.forward * _speed;
                var leadTimePercentage = Mathf.InverseLerp(_minDistancePredict, _maxDistancePredict, Vector3.Distance(transform.position, _target.transform.position));

                PredictMovement(leadTimePercentage);

                AddDeviation(leadTimePercentage);

                RotateRocket();
               
            }
               
         
          
        }

        private IEnumerator DisableAfterDelay()
        {
            // Wait for the specified delay
            yield return new WaitForSeconds(3);

            gameObject.SetActive(false);
        }
        public void setPView(PlayerHealth p)
        {
            this.PH = p;

        }
        IEnumerator isLaunched()
        {
            yield return new WaitForSeconds(0.2f);
            _missileLaunched = true;    
        }
        private void PredictMovement(float leadTimePercentage) {
            var predictionTime = Mathf.Lerp(0, _maxTimePrediction, leadTimePercentage);

            _standardPrediction = _target.Rb.position + _target.Rb.velocity * predictionTime;
        }

        private void AddDeviation(float leadTimePercentage) {
            var deviation = new Vector3(Mathf.Cos(Time.time * _deviationSpeed), 0, 0);
            
            var predictionOffset = transform.TransformDirection(deviation) * _deviationAmount * leadTimePercentage;

            _deviatedPrediction = _standardPrediction + predictionOffset;
        }

        private void RotateRocket() {
            var heading = _deviatedPrediction - transform.position;

            var rotation = Quaternion.LookRotation(heading);
            _rb.MoveRotation(Quaternion.RotateTowards(transform.rotation, rotation, _rotateSpeed * Time.deltaTime));
        }

        private void OnCollisionEnter(Collision collision)
        {

            if (collision.collider.tag == "Enemy")
            {


                PlayerHealth otherPlayerH = collision.gameObject.GetComponent<PlayerHealth>();

                PH.OtherPlayerHealth = otherPlayerH;
                if (otherPlayerH.view.IsMine)
                {
                    otherPlayerH.TakeDamage(100);
                }
              
                //gameObject.SetActive(false);
            }
            //GameObject impact = Instantiate(bulletImact, transform.position, Quaternion.identity);
            //Destroy(impact, 1);
          //  gameObject.SetActive(false);
        }

        private void OnDrawGizmos() {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, _standardPrediction);
            Gizmos.color = Color.green;
            Gizmos.DrawLine(_standardPrediction, _deviatedPrediction);
        }
    }
}