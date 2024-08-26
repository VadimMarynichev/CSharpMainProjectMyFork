using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Model.Runtime.Projectiles;
using UnityEngine;
using UnityEngine.UIElements;
using Utilities;

namespace UnitBrains.Player
{
    public class SecondUnitBrain : DefaultPlayerUnitBrain
    {
        public override string TargetUnitName => "Cobra Commando";
        private const float OverheatTemperature = 3f;
        private const float OverheatCooldown = 2f;
        private float _temperature = 0f;
        private float _cooldownTime = 0f;
        private bool _overheated;
        private float min;
        List<Vector2Int> _nextStep = new List<Vector2Int>();

        protected override void GenerateProjectiles(Vector2Int forTarget, List<BaseProjectile> intoList)
        {
            float overheatTemperature = OverheatTemperature;
            ///////////////////////////////////////
            // Homework 1.3 (1st block, 3rd module)
            ///////////////////////////////////////           

            int temp = GetTemperature();

            if (temp >= overheatTemperature) return;

               for (int i = 0; i < temp; i++)
                {
                    var projectile = CreateProjectile(forTarget);
                    AddProjectileToList(projectile, intoList);
                }
                    
            IncreaseTemperature();
        }

        public override Vector2Int GetNextStep()
        {
            if (_nextStep.Count != 0)
            {
                Vector2Int position = Vector2Int.zero;
                Vector2Int target =  _nextStep.First();
                position.CalcNextStepTowards(target);
                return position;
            }
            else
            {
                throw new System.Exception();
            }                
                      
        }

     
        private bool CanAttack(Vector2Int target) 
        {
            IsTargetInRange(target);
            return true;
        }

        protected override List<Vector2Int> SelectTargets()
        {
            ///////////////////////////////////////
            // Homework 1.4 (1st block, 4rd module)
            ///////////////////////////////////////
            var results = GetAllTargets();
            
            float min = float.MaxValue;
            Vector2Int bestTarget = Vector2Int.zero;
           

            results = results.Where(result => !CanAttack (result)).ToList();

            if (results.Count() != 0)
            {

                foreach (Vector2Int result in results)
                {
                    float distance = DistanceToOwnBase(result);

                    if (distance < min)
                    {
                        min = distance;
                        bestTarget = result;
                    }
                }

                _nextStep.Add(bestTarget);
            }
            else            
            {
               _nextStep.Add(runtimeModel.RoMap.Bases[0]);
            }        
            
            return _nextStep;
                        
        }

        public override void Update(float deltaTime, float time)
        {
            if (_overheated)
            {              
                _cooldownTime += Time.deltaTime;
                float t = _cooldownTime / (OverheatCooldown/10);
                _temperature = Mathf.Lerp(OverheatTemperature, 0, t);
                if (t >= 1)
                {
                    _cooldownTime = 0;
                    _overheated = false;
                }
            }
        }

        private int GetTemperature()
        {
            if(_overheated) return (int) OverheatTemperature;
            else return (int)_temperature;
        }

        private void IncreaseTemperature()
        {
            _temperature += 1f;
            if (_temperature >= OverheatTemperature) _overheated = true;
        }
    }
}