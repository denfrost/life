using UnityEngine;

namespace Boids
{
    public class Predator : MonoBehaviour
    {
        public float MinSpeed = 0.2f, MaxSpeed = 3f, RotationSpeed = 0.14f, MinAge = 30, MaxAge = 120;
        public float MinVision = 3, MaxVision = 10, MinCapacity = 10, MaxCapacity = 20, KillEnergy = 10;
        public float MinMetabolism = 0.01f, MaxMetabolism = 1, Inertia = 1, LevyChance = 5;
        private float _maxSpeed, _vision, _age, _expectedLife, _nextAge, _capacity, _metabolism, _energy, _speed;
        private Flock _globalFlock;

        private void Start()
        {
            _maxSpeed = Random.Range(MinSpeed + 2, MaxSpeed);
            _speed = _maxSpeed;
            _vision = Random.Range(MinVision, MaxVision);
            _expectedLife = Random.Range(MinAge, MaxAge);
            _metabolism = Random.Range(MinMetabolism, MaxMetabolism);
            _capacity = Random.Range(MinCapacity, MaxCapacity);
            _energy = MinCapacity;
            foreach (GameObject food in Flock.FishFoods)            
                Physics.IgnoreCollision(food.GetComponent<Collider>(), GetComponent<Collider>());
            
            foreach (GameObject food in Flock.Trash)
                Physics.IgnoreCollision(food.GetComponent<Collider>(), GetComponent<Collider>());
            
            foreach (GameObject food in Flock.Predators)
                Physics.IgnoreCollision(food.GetComponent<Collider>(), GetComponent<Collider>());
        }
        
        private void FixedUpdate()
        {
            GetComponent<Rigidbody>().velocity = transform.forward * _speed;
        }

        private void Update()
        {
            if (Time.time > _nextAge)
            {
                _nextAge = Time.time + 1;
                _age += 1;
                if (_age > _expectedLife)
                    Die();
                _energy -= _metabolism;
                if (_energy <= 0)
                    Die();
            }

            if (Vector3.Distance(transform.position, Vector3.zero) >= _globalFlock.EnvironmentSize)
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(-transform.position),
                    RotationSpeed);
            else if (_energy >= _capacity)
                _speed = MinSpeed;
            else if (Random.Range(0, LevyChance) < 1)
                ApplyRules();
        }

        private void ApplyRules()
        {
            Vector3 goalPos = Random.insideUnitSphere * 4;
            float min = float.MaxValue;
            foreach (GameObject fish in Flock.Fishes)
            {
                float tmp = Vector3.Distance(transform.position, fish.transform.position);
                if (tmp > _vision)
                    continue;
                tmp += Inertia;
                if (tmp < min)
                {
                    min = tmp;
                    goalPos = fish.transform.position;
                }
            }

            _speed = _maxSpeed;
            goalPos -= transform.position;
            if (goalPos == Vector3.zero)
                return;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(goalPos),
                RotationSpeed);            
        }

        public void Eat()
        {
            _energy += KillEnergy;            
        }

        private void Awake()
        {
            _globalFlock = GameObject.FindGameObjectWithTag("GameController").GetComponent<Flock>();
        }
        
        private void Die()
        {
            Flock.Predators.Remove(gameObject);
            Destroy(gameObject);
        }
    }
}