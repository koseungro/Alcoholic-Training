/// 작성자: 백인성 
/// 작성일: 2018-05-01 
/// 수정일: 2018-07-25
/// 저작권: Copyright(C) FNI Co., LTD. 
/// 용  도: EventSystem의 물리이벤트 시스템 적용 용도
/// 사용법: Prefabs(FNIVR_EventSystem)를 이용해서 사용할 것
/// 수정이력 
/// 

using System.Collections.Generic;

namespace UnityEngine.EventSystems
{
    /// <summary>
    /// Simple event system using physics raycasts. Very closely based on UnityEngine.EventSystems.PhysicsRaycaster
    /// </summary>
    public class FNIVR_PhysicsRaycaster : BaseRaycaster
    {
        /// <summary>
        /// Const to use for clarity when no event mask is set
        /// </summary>
        protected const int kNoEventMaskSet = -1;


        /// <summary>
        /// Layer mask used to filter events. Always combined with the camera's culling mask if a camera is used.
        /// </summary>
        [SerializeField]
        protected LayerMask m_EventMask = kNoEventMaskSet;

        protected FNIVR_PhysicsRaycaster()
        { }

        public override Camera eventCamera
        {
            get
            {
                return FNIVR_Device.Instance.UICamera;
            }
        }

        /// <summary>
        /// Depth used to determine the order of event processing.
        /// </summary>
        public virtual int depth
        {
            get { return (eventCamera != null) ? (int)eventCamera.depth : 0xFFFFFF; }
        }

        public int sortOrder = 20;
        public override int sortOrderPriority
        {
            get
            {
                return sortOrder;
            }
        }

        /// <summary>
        /// Event mask used to determine which objects will receive events.
        /// </summary>
        public int finalEventMask
        {
            get { return (eventCamera != null) ? (eventCamera.cullingMask & m_EventMask) : kNoEventMaskSet; }
        }

        /// <summary>
        /// Layer mask used to filter events. Always combined with the camera's culling mask if a camera is used.
        /// </summary>
        public LayerMask eventMask
        {
            get { return m_EventMask; }
            set { m_EventMask = value; }
        }


        /// <summary>
        /// Perform a raycast using the worldSpaceRay in eventData.
        /// </summary>
        /// <param name="eventData"></param>
        /// <param name="resultAppendList"></param>
        public override void Raycast(PointerEventData eventData, List<RaycastResult> resultAppendList)
        {
            // This function is closely based on PhysicsRaycaster.Raycast

            if (eventCamera == null)
                return;

            if (!eventData.IsVRPointer_FNI())
                return;

            Ray ray = eventData.GetRay_FNI();

            float dist = eventCamera.farClipPlane - eventCamera.nearClipPlane;

            RaycastHit[] hits = Physics.RaycastAll(ray, dist, finalEventMask);

            if (hits.Length > 1)
                System.Array.Sort(hits, (r1, r2) => r1.distance.CompareTo(r2.distance));

            if (hits.Length != 0)
            {
                for (int b = 0, bmax = hits.Length; b < bmax; ++b)
                {
                    var result = new RaycastResult
                    {
                        gameObject = hits[b].collider.gameObject,
                        module = this,
                        distance = hits[b].distance,
                        index = resultAppendList.Count,
                        worldPosition = hits[0].point,
                        worldNormal = hits[0].normal,
                    };
                    resultAppendList.Add(result);
                }
            }
        }

        /// <summary>
        ///  Perform a Spherecast using the worldSpaceRay in eventData.
        /// </summary>
        /// <param name="eventData"></param>
        /// <param name="resultAppendList"></param>
        /// <param name="radius">Radius of the sphere</param>
        public void Spherecast(PointerEventData eventData, List<RaycastResult> resultAppendList, float radius)
        {
            if (eventCamera == null)
                return;

            if (!eventData.IsVRPointer_FNI())
                return;

            var ray = eventData.GetRay_FNI();


            float dist = eventCamera.farClipPlane - eventCamera.nearClipPlane;

            var hits = Physics.SphereCastAll(ray, radius, dist, finalEventMask);

            if (hits.Length > 1)
                System.Array.Sort(hits, (r1, r2) => r1.distance.CompareTo(r2.distance));

            if (hits.Length != 0)
            {
                for (int b = 0, bmax = hits.Length; b < bmax; ++b)
                {
                    var result = new RaycastResult
                    {
                        gameObject = hits[b].collider.gameObject,
                        module = this,
                        distance = hits[b].distance,
                        index = resultAppendList.Count,
                        worldPosition = hits[0].point,
                        worldNormal = hits[0].normal,
                    };
                    resultAppendList.Add(result);
                }
            }
        }
        /// <summary>
        /// Get screen position of this world position as seen by the event camera of this OVRPhysicsRaycaster
        /// </summary>
        /// <param name="worldPosition"></param>
        /// <returns></returns>
        public Vector2 GetScreenPos(Vector3 worldPosition)
        {
            // In future versions of Uinty RaycastResult will contain screenPosition so this will not be necessary
            return eventCamera.WorldToScreenPoint(worldPosition);
        }
    }
}
