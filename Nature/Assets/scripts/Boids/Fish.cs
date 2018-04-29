using UnityEngine;

namespace Boids
{
    public class Fish : MonoBehaviour
    {
        public float MinSpeed = 0.5f, MaxSpeed = 2f, RotationSpeed = 4, NeighborDistance = 3;
        public float MinBite = 1, MaxBite = 4, MinVision = 1, MaxVision = 6, MinAge = 30, MaxAge = 90;
        private float _speed, _bite, _vision, _age, _expectedLife, _nextTime;
        private Flock _globalFlock;

        void Start()
        {
            _speed = Random.Range(MinSpeed, MaxSpeed);
            _bite = Random.Range(MinBite, MaxBite);
            _vision = Random.Range(MinVision, MaxVision);
            _expectedLife = Random.Range(MinAge, MaxAge);
        }

        void Update()
        {
            if (Time.time > _nextTime)
            {
                _nextTime = Time.time + 1;
                _age += 1;
                if (_age > _expectedLife)
                {
                    Flock.Fishes.Remove(gameObject);
                    Destroy(gameObject);
                }
            }
            if (Vector3.Distance(transform.position, Vector3.zero) >= _globalFlock.EnvironmentSize)
            {
                Vector3 direction = Vector3.zero - transform.position;
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation((direction)),
                    RotationSpeed * Time.deltaTime);
            }
            else if (Random.Range(0, 5) < 1)
                ApplyRules();

            transform.Translate(0, 0, Time.deltaTime * _speed);
        }

        void ApplyRules()
        {            
            Vector3 vcenter = Vector3.zero, vavoid = Vector3.zero, goalPos = Vector3.zero;
            float min = float.MaxValue, tmp;
            foreach (GameObject food in Flock.FishFoods)
            {
                tmp = Vector3.Distance(transform.position, food.transform.position);
                if (tmp < min)
                {
                    min = tmp;
                    goalPos = food.transform.position;
                }
            }

            float groupSpeed = 0.1f;
            int groupSize = 0;
            foreach (GameObject fish in Flock.Fishes)
            {
                if (fish != gameObject)
                {
                    float dist = Vector3.Distance(fish.transform.position, transform.position);
                    if (dist <= NeighborDistance)
                    {
                        vcenter += fish.transform.position;
                        groupSize++;
                        if (dist < 1)
                            vavoid += transform.position - fish.transform.position;
                        groupSpeed += fish.GetComponent<Fish>()._speed;
                    }
                }
            }

            if (groupSize <= 0) return;
            vcenter = vcenter / groupSize + (goalPos - transform.position);
            _speed = Mathf.Clamp(groupSpeed / groupSize, MinSpeed, MaxSpeed);
            Vector3 direction = (vcenter + vavoid) - transform.position;
            if (direction != Vector3.zero)
                transform.rotation = Quaternion.Slerp(transform.rotation,
                    Quaternion.LookRotation(direction), RotationSpeed * Time.deltaTime);
        }
        
        void OnTriggerEnter(Collider other)
        {            
            if (other.CompareTag("Tree"))
            {
                       
            }
        }

        void Awake()
        {
            _globalFlock = GameObject.FindGameObjectWithTag("GameController").GetComponent<Flock>();
        }

        public float GetBite()
        {
            return _bite;
        }
    }
}