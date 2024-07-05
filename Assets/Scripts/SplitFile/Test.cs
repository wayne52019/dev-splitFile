using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SplitFile
{
    public class Test : MonoBehaviour
    {
        public Transform rotationTransform;

        // Start is called before the first frame update
        void Start()
        {

        }

        private void Update()
        {
            var e = rotationTransform.localEulerAngles;

            rotationTransform.localEulerAngles = new Vector3(e.x, e.y + (Time.deltaTime * 50), e.z);
        }

    }
}