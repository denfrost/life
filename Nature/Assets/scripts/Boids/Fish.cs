using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Boids
{
    public class Fish : MonoBehaviour
    {
        public float MinSpeed = 0.5f, MaxSpeed = 2f, RotationSpeed = 4, NeighborDistance = 3, MinAge = 60, MaxAge = 120;
        public float MinBite = 1, MaxBite = 4, MinVision = 1, MaxVision = 8, MinCapacity = 4, MaxCapacity = 10;
        public float MinMetabolism = 0.5f, MaxMetabolism = 4, Inertia = 2, LevyChance = 10, Stop = 0.3f;
        private float _speed, _bite, _vision, _age, _expectedLife, _nextAge, _capacity, _metabolism, _energy;
        private Flock _globalFlock;
        private FishFood _lastTree;
        private Quaternion _rotation = Quaternion.identity;

        void Start()
        {
            _speed = Random.Range(MinSpeed+2, MaxSpeed);
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
                //Debug.Log("Hola");
            }
        }

        private void FixedUpdate()
        {
            //transform.Translate(0, 0, Time.deltaTime * _speed);             
            GetComponent<Rigidbody>().velocity = transform.forward * _speed;
            //float e = _globalFlock.EnvironmentSize;
            /*r.position = new Vector3 
            (
                Mathf.Clamp (r.position.x, -e, e), 
                Mathf.Clamp (r.position.y, -e, e),
                Mathf.Clamp (r.position.z, -e, e)
            );*/
            GetComponent<Rigidbody>().rotation = _rotation;
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
            if (Vector3.Distance(transform.position, Vector3.zero) >= _globalFlock.EnvironmentSize)
            {
                Vector3 direction = -transform.position;
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation((direction)),
                    RotationSpeed * Time.deltaTime);
            }
            else if (Random.Range(0, LevyChance) < 1)
                ApplyRules();
        }

        private void ApplyRules()
        {
            Vector3 vcenter = Vector3.zero, vavoid = Vector3.zero, goalPos = Random.insideUnitSphere*4;
            float min = float.MaxValue;
            foreach (GameObject food in Flock.FishFoods)
            {
                float tmp = Vector3.Distance(transform.position, food.transform.position);
                if (tmp > _vision)
                    continue;
                FishFood fishFood = food.GetComponent<FishFood>();
                tmp -= fishFood.GetEnergy() - fishFood.GetWaste()+ Inertia;
                if (tmp < min)
                {
                    min = tmp;
                    goalPos = food.transform.position;
                }
            }
            if (Vector3.Distance(transform.position, goalPos) <= Stop)
                _speed = MinSpeed;

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
            _speed = Mathf.Clamp(groupSpeed / groupSize, MinSpeed, MaxSpeed);            
            //transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(vcenter - transform.position), RotationSpeed * Time.deltaTime);
            _rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(vcenter - transform.position), RotationSpeed * Time.deltaTime);
            //_rotation = Quaternion.Slerp(GetComponent<Rigidbody>().rotation, Quaternion.LookRotation(vcenter - transform.position), RotationSpeed * Time.deltaTime);
        }

        private void Awake()
        {
            _globalFlock = GameObject.FindGameObjectWithTag("GameController").GetComponent<Flock>();
        }

        private void Die()
        {
            Flock.Fishes.Remove(gameObject);
            Destroy(gameObject);
            Debug.Log("Dead");
        }
    }
}