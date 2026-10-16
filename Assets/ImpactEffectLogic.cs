using UnityEngine;
using System.Collections;

namespace Assets
{
    public class ImpactEffectLogic : MonoBehaviour
    {
        public void OnAnimationComplete()
        {
            Destroy(gameObject);
        }
    }
}