using UnityEngine;
using Random = UnityEngine.Random;

namespace Boids
{
    public class Fish : MonoBehaviour
    {
        public float MinSpeed = 0.2f, MaxSpeed = 3f, RotationSpeed = 3, NeighborDistance = 2, MinAge = 30, MaxAge = 120;
        public float MinBite = 2, MaxBite = 4, MinVision = 3, MaxVision = 10, MinCapacity = 10, MaxCapacity = 20;
        public float MinMetabolism = 0.01f, MaxMetabolism = 1, Inertia = 1, LevyChance = 5, Stop = 0.4f, Avoid = 2;
        private float _speed, _bite, _vision, _age, _expectedLife, _nextAge, _capacity, _metabolism, _energy;
        private Flock _globalFlock;
        private FishFood _lastTree;

        void Start()
        {
            _speed = Random.Range(MinSpeed + 2, MaxSpeed);
            _bite = Random.Range(MinBite, MaxBite);
            _vision = Random.Range(MinVision, MaxVision);
            _expectedLife = Random.Range(MinAge, MaxAge);
            _metabolism = Random.Range(MinMetabolism, MaxMetabolism);
            _capacity = Random.Range(MinCapacity, MaxCapacity);
            _energy = (_capacity + MinCapacity) / 2;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Tree"))
            {
                _energy = Mathf.Min(_energy + other.GetComponent<FishFood>().Eat(_bite), _capacity);
                _lastTree = other.gameObject.GetComponent<FishFood>();
            }
            else if (other.CompareTag("Predator"))
            {
                other.GetComponent<Predator>().Eat();
                Die();
            }
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
                if (_lastTree != null)
                    _lastTree.AddWaste(_metabolism);
                if (_energy <= 0)
                    Die();
            }

            Vector3 vavoid = Vector3.zero;
            bool run = false;
            foreach (GameObject predator in Flock.Predators)
                if (Vector3.Distance(predator.transform.position, transform.position) <= Avoid)
                {
                    vavoid += transform.position - predator.transform.position;
                    run = true;
                }

            if (run)
            {
                _speed = MaxSpeed;
                transform.rotation = Quaternion.Slerp(transform.rotation,
                    Quaternion.LookRotation(vavoid - transform.position),
                    RotationSpeed * Time.deltaTime);
            }
            else if (Vector3.Distance(transform.position, Vector3.zero) >= _globalFlock.EnvironmentSize)
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(-transform.position),
                    RotationSpeed * Time.deltaTime);
            else if (Random.Range(0, LevyChance) < 1)
                ApplyRules();
        }

        private void ApplyRules()
        {
            Vector3 vavoid = Vector3.zero;
            Vector3 vcenter = Vector3.zero, goalPos = Random.insideUnitSphere * 4;
            float min = float.MaxValue;
            foreach (GameObject food in Flock.FishFoods)
            {
                float tmp = Vector3.Distance(transform.position, food.transform.position);
                if (tmp > _vision)
                    continue;
                FishFood fishFood = food.GetComponent<FishFood>();
                tmp -= fishFood.GetEnergy() - fishFood.GetWaste() + Inertia;
                if (tmp < min)
                {
                    min = tmp;
                    goalPos = food.transform.position;
                }
            }

            if (Vector3.Distance(transform.position, goalPos) <= Stop)
                _speed = MinSpeed;
            else
                _speed = MaxSpeed - 1;

            float groupSpeed = _speed;
            int groupSize = 0;
            foreach (GameObject fish in Flock.Fishes)
            {
                float dist = Vector3.Distance(fish.transform.position, transform.position);
                if (dist <= NeighborDistance)
                {
                    vcenter += fish.transform.position;
                    groupSize++;
                    if (dist < 1.2f && dist > 0)
                        vavoid += transform.position - fish.transform.position;
                    groupSpeed += fish.GetComponent<Fish>()._speed;
                }
            }

            vcenter /= groupSize;
            vcenter += vavoid;
            vcenter += goalPos - transform.position;
            vcenter -= transform.position;
            if (vcenter == Vector3.zero)
                return;
            _speed = Mathf.Clamp(groupSpeed / groupSize, MinSpeed, MaxSpeed);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(vcenter),
                RotationSpeed * Time.deltaTime);
        }

        private void Awake()
        {
            _globalFlock = GameObject.FindGameObjectWithTag("GameController").GetComponent<Flock>();
        }

        private void Die()
        {
            Flock.Fishes.Remove(gameObject);
            Destroy(gameObject);
        }
    }
}