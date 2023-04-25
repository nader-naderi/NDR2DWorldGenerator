using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NDR2DWorldGenerator
{
    public class CameraController : MonoBehaviour
    {
        public float dampTime = 0.15f;
        private Vector3 velocity = Vector3.zero;
        public Transform target;

        // Update is called once per frame
        void Update()
        {
            if (target)
            {
                Vector3 movement = new Vector3(Input.GetAxis("Horizontal") * 50f, Input.GetAxis("Vertical") * 50f, 0);

                target.transform.Translate(movement * Time.deltaTime);


                Vector3 point = Camera.main.WorldToViewportPoint(target.position);
                Vector3 delta = target.position - Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, point.z)); //(new Vector3(0.5, 0.5, point.z));
                Vector3 destination = transform.position + delta;
                transform.position = Vector3.SmoothDamp(transform.position, destination, ref velocity, dampTime);
            }
        }
    }
}