using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityStandardAssets._2D
{
    public class CameraScript : MonoBehaviour
    {
        public Transform target;
        public float damping = 1;
        public float lookAheadFactor = 3;
        public float lookAheadReturnSpeed = 0.5f;
        public float lookAheadMoveThreshold = 0.1f;
        public List<Material> skyboxes;
        public Skybox skybox;

        private float m_OffsetZ;
        private Vector3 m_LastTargetPosition;
        private Vector3 m_CurrentVelocity;
        private Vector3 m_LookAheadPos;
        private float minX;
        private float maxX;
        
        // Use this for initialization
        private void Start()
        {
            m_LastTargetPosition = target.position;
            m_OffsetZ = (transform.position - target.position).z;
            transform.parent = null;

            float cameraSize = Camera.main.orthographicSize;
            float cameraAspect = Camera.main.aspect;
            float cameraHeight = 2f * cameraSize;
            float cameraWidth = (cameraHeight * cameraAspect) / 2.0f;
            float sideLength = PersistentTerrainSettings.settings.sideLength;
            minX = sideLength / 2.0f;
            maxX = sideLength / 2.0f;
            
            minX = -(minX - cameraWidth) + 1.0f;
            maxX = maxX - cameraWidth - 1.0f;

            setRandomSky();
        }


        // Update is called once per frame
        private void Update()
        {
            // only update lookahead pos if accelerating or changed direction
            float xMoveDelta = (target.position - m_LastTargetPosition).x;
            
            bool updateLookAheadTarget = Mathf.Abs(xMoveDelta) > lookAheadMoveThreshold;
            
            if (updateLookAheadTarget)
            {
                m_LookAheadPos = lookAheadFactor*Vector3.right*Mathf.Sign(xMoveDelta);
            }
            else
            {
                m_LookAheadPos = Vector3.MoveTowards(m_LookAheadPos, Vector3.zero, Time.deltaTime*lookAheadReturnSpeed);
            }
            
            Vector3 aheadTargetPos = target.position + m_LookAheadPos + Vector3.forward*m_OffsetZ;
            Vector3 newPos = Vector3.SmoothDamp(transform.position, aheadTargetPos, ref m_CurrentVelocity, damping);
            
            transform.position = newPos;
            
            m_LastTargetPosition = target.position;

            // Limit x position 
            Vector3 currentPosition = transform.position;

            currentPosition.x = Mathf.Clamp(transform.position.x, minX, maxX);
            transform.position = currentPosition;
        }
        private void setRandomSky() {
            float rand = UnityEngine.Random.Range(0f, (float)skyboxes.Count);
            skybox.material = skyboxes[(int)rand];

        }
    }
}
