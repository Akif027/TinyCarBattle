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

        private void OnEnable()
        {
            StartCoroutine(isLaunched());
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

            int targetLayer = LayerMask.NameToLayer("Enemy"); // Replace "YourLayerName" with the target layer name
            if (collision.gameObject.layer == targetLayer)
            {
                // Instantiate an explosion if the explosion prefab is set
                if (_explosionPrefab) Instantiate(_explosionPrefab, transform.position, Quaternion.identity); // make it photonInstantiate

                // Call Explode if the colliding object implements IExplode
                if (collision.transform.TryGetComponent<IExplode>(out var ex)) ex.Explode();

                // Disable the GameObject
                gameObject.SetActive(false);
                _missileLaunched = false;
            }
            gameObject.SetActive(false);
            _missileLaunched = false;
        }

        private void OnDrawGizmos() {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, _standardPrediction);
            Gizmos.color = Color.green;
            Gizmos.DrawLine(_standardPrediction, _deviatedPrediction);
        }
    }
}