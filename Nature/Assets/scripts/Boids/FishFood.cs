using UnityEngine;

namespace Boids
{
    public class FishFood : MonoBehaviour
    {
        public float MinEnergy = 1, MaxEnergy = 4, ChargeRate = 0.5f, ChargeDelta = 1;
        private float _energyCapacity, _energy, _nextTime;

        public float eat(float bite)
        {
            float tmp = _energy;
            _energy = Mathf.Max(0, _energy - bite);
            return tmp - _energy;
        } 
        
        void Start()
        {
            _energyCapacity = Random.Range(MinEnergy, MaxEnergy);
        }

        void Update()
        {
            if (Time.time >= _nextTime)
            {
                _nextTime = Time.time + ChargeRate;                
                _energy = Mathf.Max(_energy += ChargeDelta, _energyCapacity);
            }
        }
    }
}