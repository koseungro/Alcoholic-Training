/// 작성자: 백인성 
/// 작성일: 2018-05-01 
/// 수정일: 2018-07-25
/// 저작권: Copyright(C) FNI Co., LTD. 
/// 용  도: PointerEventData 
/// 사용법: 수정 금지
/// 수정이력 
/// 

using System;
using System.Text;
using UnityEngine;
using UnityEngine.Assertions;

namespace UnityEngine.EventSystems
{
    /// <summary>
    /// Extension of Unity's PointerEventData to support ray based pointing and also touchpad swiping
    /// </summary>
    public class FNIVR_PointerEventData : PointerEventData
    {
        public FNIVR_PointerEventData(EventSystem eventSystem) : base(eventSystem) { }

        public Ray worldSpaceRay;
        public Vector2 swipeStart;

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine("<b>Position</b>: " + position);
            sb.AppendLine("<b>delta</b>: " + delta);
            sb.AppendLine("<b>eligibleForClick</b>: " + eligibleForClick);
            sb.AppendLine("<b>pointerEnter</b>: " + pointerEnter);
            sb.AppendLine("<b>pointerPress</b>: " + pointerPress);
            sb.AppendLine("<b>lastPointerPress</b>: " + lastPress);
            sb.AppendLine("<b>pointerDrag</b>: " + pointerDrag);
            sb.AppendLine("<b>worldSpaceRay</b>: " + worldSpaceRay);
            sb.AppendLine("<b>swipeStart</b>: " + swipeStart);
            sb.AppendLine("<b>Use Drag Threshold</b>: " + useDragThreshold);
            return sb.ToString();
        }
    }
    
    /// <summary>
    /// Static helpers for OVRPointerEventData.
    /// </summary>
    public static class FNIVR_PointerEventDataExtension
    {
        public static bool IsVRPointer_FNI(this PointerEventData pointerEventData)
        {
            return (pointerEventData is FNIVR_PointerEventData);
        }
        public static Ray GetRay_FNI(this PointerEventData pointerEventData)
        {
            FNIVR_PointerEventData vrPointerEventData = pointerEventData as FNIVR_PointerEventData;
            Assert.IsNotNull(vrPointerEventData);

            return vrPointerEventData.worldSpaceRay;
        }
        public static Vector2 GetSwipeStart_FNI(this PointerEventData pointerEventData)
        {
            FNIVR_PointerEventData vrPointerEventData = pointerEventData as FNIVR_PointerEventData;
            Assert.IsNotNull(vrPointerEventData);

            return vrPointerEventData.swipeStart;
        }
        public static void SetSwipeStart_FNI(this PointerEventData pointerEventData, Vector2 start)
        {
            FNIVR_PointerEventData vrPointerEventData = pointerEventData as FNIVR_PointerEventData;
            Assert.IsNotNull(vrPointerEventData);

            vrPointerEventData.swipeStart = start;
        }
    }
}
