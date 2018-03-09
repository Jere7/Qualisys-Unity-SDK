﻿// Unity SDK for Qualisys Track Manager. Copyright 2015-2018 Qualisys AB
//
using UnityEngine;
using System.Collections;
using QTMRealTimeSDK;

namespace QualisysRealTime.Unity
{
    class RTObject : MonoBehaviour
    {
        public string ObjectName = "Put QTM 6DOF object name here";
        public Vector3 PositionOffset = new Vector3(0, 0, 0);
        public Vector3 RotationOffset = new Vector3(0, 0, 0);

        protected RTClient rtClient;
        protected SixDOFBody body;

        public bool hasBody()
        {
            return body != null;
        }

        public virtual void applyBody()
        {
            if (body.Position.magnitude > 0) //just to avoid error when position is NaN
            {
                transform.position = body.Position + PositionOffset;
                if (transform.parent)
                {
                    transform.position += transform.parent.position;
                    transform.rotation *= transform.parent.rotation;
                }
            }
        }

        // Use this for initialization
        void Start()
        {
            rtClient = RTClient.GetInstance();
        }

        // Update is called once per frame
        void Update()
        {
            if (rtClient == null) rtClient = RTClient.GetInstance();

            body = rtClient.GetBody(ObjectName);
            if (body != null)
            {
                this.applyBody();
            }
        }
    }
}