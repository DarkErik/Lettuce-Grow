using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterdropsParticleHandler : MonoBehaviour {

    [SerializeField]
    private ParticleSystem particleSystem;

    /// <summary>
    /// Keep track of all possibly affected plants
    /// </summary>
    private List<GrowingPlant> plants = new List<GrowingPlant>();


    public void AddPlant(GrowingPlant plant) {
        particleSystem.trigger.AddCollider(plant.transform);
        plants.Add(plant);
    }



    public void OnParticleTrigger() {
        
        //Get all particles that entered the trigger
        List<ParticleSystem.Particle> enteredParticles = new List<ParticleSystem.Particle>();
        ParticleSystem.ColliderData collisionData;

        int enterCount = particleSystem.GetTriggerParticles(ParticleSystemTriggerEventType.Enter, enteredParticles, out collisionData);

        for (int i = 0; i < enteredParticles.Count; i++) {
            ParticleSystem.Particle particle = enteredParticles[i];

            // Iterate over all plants
            for (int i2 = 0; i2 < particleSystem.trigger.colliderCount; i2++) {
                
                Transform plant = particleSystem.trigger.GetCollider(i2).transform;


                //Debug.Log((collisionData.GetCollider(i, 0) == plant) + " " + collisionData.GetCollider(i, 0).gameObject);

                if (collisionData.GetCollider(i, 0) == plant) {
                    
                    plant.GetComponent<GrowingPlant>().AddWater();
                    SoundManager.Instance.PlayWaterHitPlantSound(this.transform.position);
                    break;
                }
            }
        }

    }

}
