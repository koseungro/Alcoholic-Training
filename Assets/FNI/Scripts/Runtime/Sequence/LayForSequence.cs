/// 작성자: 고승로
/// 작성일: 2020-09-28
/// 수정일: 2020-09-28
/// 저작권: Copyright(C) FNI Co., LTD. 
/// 수정이력

using FNI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FNI
{
    public class LayForSequence : MonoBehaviour, IVisualObject
    {
        public VisualType Type => VisualType.Lay;

        public bool IsFinish => true;

        [SerializeField] private GameObject layObject;
        private MeshCollider curvedMesh;

        private void Start()
        {
            StartCoroutine(FindCurvedMesh());
        }

        public void Active(CutData option)
        {
            if(option.layOption.isOn)
            {
                layObject.SetActive(true);
                curvedMesh.enabled = true;
            }
            else
            {
                layObject.SetActive(false);
                curvedMesh.enabled = false;
            }
        }

        public void Init()
        {

        }

        public void MyUpdate()
        {

        }

        private IEnumerator FindCurvedMesh()
        {
            yield return new WaitForSeconds(0.1f);
            curvedMesh = GameObject.Find("---------------UI/UICanvas").GetComponent<MeshCollider>();
        }
    }
}