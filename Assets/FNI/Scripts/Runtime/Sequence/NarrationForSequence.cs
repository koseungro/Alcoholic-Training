/// 작성자: 고승로
/// 작성일: 2020-08-31
/// 수정일: 2020-09-04
/// 저작권: Copyright(C) FNI Co., LTD. 
/// 수정이력

using UnityEngine;

namespace FNI
{
    /// <summary>
    /// 나레이션 관련 씬 데이터를 처리하는 클래스
    /// </summary>
    public class NarrationForSequence : MonoBehaviour, IVisualObject
    {
        VisualType IVisualObject.Type => VisualType.Narration;

        [SerializeField]
        private AudioSource audioSource;

        bool IVisualObject.IsFinish => isFinish;

        private bool isFinish = false;
        private float time = 0.0f;
        private float waitTime = 0.0f;

        /// <summary>
        /// 나레이션 재생 및 해당 컷씬에 필요한 데이터 초기화
        /// </summary>
        /// <param name="option"></param>
        void IVisualObject.Active(CutData option)
        {
            if (audioSource.isPlaying)
                audioSource.Stop();

            if (option.narrationOption.clip == null)
            {
                isFinish = true;
                audioSource.clip = null;
                return;
            }

            if (option.narrationOption.isGender)
            {
                if (GlobalStorage.userGenderType == GenderType.Man)
                {
                    audioSource.clip = option.narrationOption.clipM;
                }
                else
                {
                    audioSource.clip = option.narrationOption.clip;
                }
            }
            else
                audioSource.clip = option.narrationOption.clip;


            audioSource.Play();

            isFinish = false;
            time = 0.0f;
            waitTime = audioSource.clip.length;

        }

        void IVisualObject.Init()
        {
            FNIVR_HMDManager.hmdPlayAction += HMDPlay;
            FNIVR_HMDManager.hmdPauseAction += HMDPause;
        }


        /// <summary>
        /// 나레이션 시간동안 Update , 캐릭터 애니메이션 변경 데이터 있다면 변경
        /// </summary>
        void IVisualObject.MyUpdate()
        {
            time += Time.deltaTime;
            
            if(time>waitTime)
            {
                time = 0.0f;
                audioSource.clip = null;
                isFinish = true;
            }
        }

        private void HMDPlay()
        {
            if (audioSource != null && audioSource.clip !=null)
                audioSource.Play();
        }

        public void HMDPause()
        {
            if (audioSource != null)
                audioSource.Pause();
        }

        public void HMDStop()
        {
            audioSource.Stop();
            audioSource.clip = null;
        }

    }
}