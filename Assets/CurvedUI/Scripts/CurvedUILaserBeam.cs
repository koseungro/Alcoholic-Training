﻿using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using FNI;

namespace CurvedUI
{
    /// <summary>
    /// This class contains code that controls the visuals (only!) of the laser pointer.
    /// </summary>
    public class CurvedUILaserBeam : MonoBehaviour
    {
        private static CurvedUILaserBeam _instance;
        public static CurvedUILaserBeam Instance
        {
            get
            {
                if(_instance == null)
                {
                    _instance = FindObjectOfType<CurvedUILaserBeam>();
                }
                return _instance;
            }
        }
#pragma warning disable 0649
        [SerializeField]
        Transform LaserBeamTransform;
        [SerializeField]
        Transform LaserBeamDot;
        [SerializeField]
        private bool hideWhenNotAimingAtCanvas = false;

#pragma warning restore 0649

        // Update is called once per frame
        void Update()
        {

            //get direction of the controller
            Ray myRay = new Ray(this.transform.position, this.transform.forward);


            //make laser beam hit stuff it points at.
            if(LaserBeamTransform && LaserBeamDot) {
                //change the laser's length depending on where it hits
                float length = 10000;
                
                RaycastHit hit;
                // FadeOut이 될 때는 Ray의 길이 0으로
                if (FadeInOutForSequence.Instance.canvasGroups[2].alpha != 0)
                {
                    length = 0;
                }

                if (Physics.Raycast(myRay, out hit, length, CurvedUIInputModule.Instance.RaycastLayerMask))
                {
                    length = Vector3.Distance(hit.point, this.transform.position);

                    //Find if we hit a canvas
                    CurvedUISettings cuiSettings = hit.collider.GetComponentInParent<CurvedUISettings>();
                    if (cuiSettings != null)
                    {
                        //find if there are any canvas objects we're pointing at. we only want transforms with graphics to block the pointer. (that are drawn by canvas => depth not -1)
                        int selectablesUnderPointer = cuiSettings.GetObjectsUnderPointer().FindAll(x => x != null && x.GetComponent<Graphic>() != null && x.GetComponent<Graphic>().depth != -1).Count;

                        length = selectablesUnderPointer == 0 ? 10000 : Vector3.Distance(hit.point, this.transform.position);
                    }
                    else if (hideWhenNotAimingAtCanvas)
                    {
                        length = 0;
                    }
                }
                else if (hideWhenNotAimingAtCanvas)
                {
                    length = 0;
                }


                //set the leangth of the beam
                LaserBeamTransform.localScale = LaserBeamTransform.localScale.ModifyZ(length);
            }
           

        }
    }
}
