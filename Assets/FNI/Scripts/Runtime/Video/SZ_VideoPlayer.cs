using FNI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using System.IO;
using System.Security.Permissions;

namespace FNI
{
	public class SZ_VideoPlayer : MonoBehaviour
	{
		#region Property
		public VideoPlayer MyVideoPlayer
		{
			get
			{
				if (videoPlayer == null)
					videoPlayer = transform.GetComponent<VideoPlayer>();

				return videoPlayer;
			}
		}

		private static SZ_VideoPlayer _instance;
		public static SZ_VideoPlayer Instance
        {
			get
            {
				if(_instance == null)
                {
					_instance = FindObjectOfType<SZ_VideoPlayer>();
                }
				return _instance;
            }
        }

		/// <summary>
		/// 영상이 무엇이든 플레이 중인지 확인합니다.
		/// </summary>
		public bool IsPlaying { get { return MyVideoPlayer.isPlaying; } }

		/// <summary>
		/// 반복할지 안할지 확인합니다.
		/// </summary>
		public bool IsLooping { get { return isLoop; } }

		/// <summary>
		/// 재생중인 영상의 현재 프레임입니다.
		/// </summary>
		public long Frame { get { return MyVideoPlayer.frame; } set { MyVideoPlayer.frame = value; } }

		/// <summary>
		/// 재생중인 영상의 현재 시간입니다.
		/// </summary>
		public double Time { get { return MyVideoPlayer.time; } set { MyVideoPlayer.time = value; } }

		/// <summary>
		/// 영상의 총 길이입니다.
		/// </summary>
		public ulong Duration { get { return (ulong)(MyVideoPlayer.frameCount / MyVideoPlayer.frameRate); } }

		/// <summary>
		/// 현재 시간의 노말라이즈 입니다.
		/// </summary>
		public double NTime { get { return Time / Duration; } set { MyVideoPlayer.time = value * Duration; } }

		/// <summary>
		/// 영상이 준비 되었는지 확인합니다.
		/// </summary>
		public bool IsPrepared { get { return MyVideoPlayer.isPrepared; } }

		/// <summary>
		/// Seek가 가능한지 확인합니다.
		/// </summary>
		public bool CanSeek { get { return MyVideoPlayer.canSetTime; } }

		public bool IsRepeating { get { return isRepeat; } set { isRepeat = value; } }
		#endregion

		private VideoPlayer videoPlayer;
		private bool pastPlaying;
		private double repeatSTime;
		private double repeatETime;
		private bool isLoop;
		private bool isRepeat;
		private bool isDraged = false;
		
		public LoadState loadState;
		public SeekState seekState = SeekState.Complete;


		private void Awake()
		{
            //이벤트 연결
            MyVideoPlayer.errorReceived += ErrorReceived_Event;
            MyVideoPlayer.frameReady += FrameReady_Event;
            MyVideoPlayer.loopPointReached += LoopPointReached_Event;
            MyVideoPlayer.prepareCompleted += PrepareCompleted_Event;
            MyVideoPlayer.seekCompleted += SeekCompleted_Event;
            MyVideoPlayer.started += Started_Event;

        }

        private void Update()
        {
            //Debug.Log(MyVideoPlayer.time + "<color=yellow> 1 </color>");
            if (isRepeat)
            {
				if (seekState == SeekState.Complete &&
					(Time < repeatSTime ||
					repeatETime < Time))
				{
					seekState = SeekState.Order;
					Seek(repeatSTime);
					Debug.Log("반복!");
				}
            }
        }

        public void SetVideoCtrl(VideoOption key)
		{
			switch (key.state)
			{
				case VideoState.Load:
					if(key.isGender==false)
						StartCoroutine(Load(Application.dataPath + "/../"+ key.urlPath));
					else
                    {
						if(GlobalStorage.userGenderType == GenderType.Woman)
							StartCoroutine(Load(Application.dataPath + "/../" + key.urlWpath));
						else
							StartCoroutine(Load(Application.dataPath + "/../" + key.urlMpath));
					}								

					break;
				case VideoState.Play:
					Play();
					break;
				case VideoState.Pause:
					Pause();
					break;
				case VideoState.Stop:
					Stop();
					break;
				case VideoState.Seek:
					Seek(key.sTime);
					break;
				case VideoState.Repeat:
					Repeat(key);
					break;
				case VideoState.UnRepeat:
					isRepeat = false;
					break;
				case VideoState.Jump:
					JumpTime(key.jumpLenth);
					break;
				case VideoState.Loop:
					MyVideoPlayer.isLooping = true;
					break;
				case VideoState.UnLoop:
					MyVideoPlayer.isLooping = false;
					break;
				case VideoState.SeekAndRepeat:
					SeekAndRepeat(key);
					break;
				default:
					break;
			}
		}

		/// <summary>
		/// Play Stop Stop 전용
		/// </summary>
		/// <param name="key"></param>
		public void SetVideoCtrl(VideoState key)
		{
			switch (key)
			{
				case VideoState.Play:
					Play();
					break;
				case VideoState.Pause:
					Pause();
					break;
				case VideoState.Stop:
					Stop();
					break;
			}
		}

		/// <summary>
		/// 플레이X -> 시작합니다.
		/// </summary>
		private void Play()
		{
			if (IsPlaying) return;
			MyVideoPlayer.Play();
		}

		/// <summary>
		/// 플레이O -> 일시정지
		/// </summary>
		private void Pause()
		{
			MyVideoPlayer.Pause();
		}

		/// <summary>
		/// 플레이O -> 정지
		/// </summary>
		private void Stop()
		{
			MyVideoPlayer.Stop();
		}

		/// <summary>
		/// 시간 점프 기능
		/// </summary>
		/// <param name="time">점프하고자 하는 시간</param>
		private void JumpTime(double time)
		{
			double temp = MyVideoPlayer.time;
			temp += time;
			temp = Mathf.Clamp((float)temp, 0, Duration);

			MyVideoPlayer.time = temp;
		}

		/// <summary>
		/// Load 기능
		/// </summary>
		/// <param name="path">영상경로</param>
		/// <returns></returns>
		private IEnumerator Load(string path)
        {
            Debug.Log("비디오경로: " + path);

            if (path != null)
            {
				if(CheckFileExists(path))
                {
					MyVideoPlayer.url = path;
					loadState = LoadState.Loading;

					MyVideoPlayer.Prepare();

					while (!IsPrepared)
					{
						yield return null;
					}
					loadState = LoadState.Succeed;
				}
				else
                {
					loadState = LoadState.Fail;		
				}
            }
			else
            {
				loadState = LoadState.Fail;
            }

		}

		/// <summary>
		/// Seek 기능
		/// </summary>
		/// <param name="key">SeekState: 일반적으로는 Move 사용하면된다.</param>
		private void Seek(VideoOption key)
        {
			key.sTime = Mathf.Clamp((float)key.sTime, 0, Duration);

			switch (key.seekState)
            {
                case SeekState.Down:
					pastPlaying = videoPlayer.isPlaying;
					isDraged = false;
					seekState = SeekState.Down;
					break;
                case SeekState.Drag:
					isDraged = true;
					Seek(key.sTime, SeekState.Drag);
					break;
                case SeekState.Up:
                    if (!isDraged)
					{
						Seek(key.sTime, SeekState.Up);
                    }
					else
						seekState = SeekState.Up;
					break;
            }
		}

		private void Seek(double time)
		{
   			double temp = Mathf.Clamp((float)time, 0, Duration);
            MyVideoPlayer.time = temp;
			Debug.Log("루프 시작");
		}

		private void Seek(double time, SeekState state = SeekState.Order)
        {
			if (!MyVideoPlayer.canSetTime) return;
			if (!IsPrepared) return;

			seekState = state;
			MyVideoPlayer.time = time;
		}

		private void Repeat(VideoOption key)
		{
			repeatSTime = key.sTime;
			repeatETime = key.eTime;
			if (Time < repeatSTime || repeatETime < Time)
				Seek(repeatSTime);

			if(VoiceRecorder.Instance.IsRepeatOK)
            {
				isRepeat = true;
			}

		}
		private void SeekAndRepeat(VideoOption key)
		{
			Seek(key.sTime);

			repeatSTime = key.sTime;
			repeatETime = key.eTime;
			if (Time < repeatSTime || repeatETime < Time)
				Seek(repeatSTime);

			if (VoiceRecorder.Instance.IsRepeatOK)
			{
				isRepeat = true;
			}
		}

		#region VideoPlayerEvent
		/// <summary>
		/// 영상 관련 디버그
		/// </summary>
		/// <param name="source"></param>
		/// <param name="message"></param>
		private void ErrorReceived_Event(VideoPlayer source, string message)
		{
			Debug.Log("[" + name + "] play Err : " + message);
		}
		private void FrameReady_Event(VideoPlayer source, long frameIdx)
		{
			Debug.Log("[" + name + "] FrameReady : " + frameIdx);
		}
		private void LoopPointReached_Event(VideoPlayer source)
		{
			Debug.Log("[" + name + "] LoopPointReached");
		}
		private void PrepareCompleted_Event(VideoPlayer source)
		{
			//Debug.Log("[" + name + "] Video PrepareCompleted");
		}
		private void SeekCompleted_Event(VideoPlayer source)
		{
			//Debug.Log("<color=red> SeekCompleted </color>");

			if (seekState == SeekState.Drag) return;

			if (m_lateSeek_Routine != null)
				StopCoroutine(m_lateSeek_Routine);
			m_lateSeek_Routine = LateSeek_Routine();

			StartCoroutine(m_lateSeek_Routine);
		}
		private void Started_Event(VideoPlayer source)
		{
			//Debug.Log("[" + name + "] Started");
		}

		private IEnumerator m_lateSeek_Routine;
		private IEnumerator LateSeek_Routine()
		{
			yield return new WaitForSeconds(0.1f);
			seekState = SeekState.Complete;

		}
		#endregion

		private bool CheckFileExists(string path)
		{
			FileInfo file = new FileInfo(path);

			if (file.Exists)
				return true;
			else
				return false;
		}
	}
}