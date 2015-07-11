
using UnityEngine;

using System;
using System.Collections;
using StaticDefine;
// using PixelCrushers.DialogueSystem;

public class CameraManager : GameSingleton<CameraManager>
{

    private Transform m_transMainCamera;
    private Camera m_mainCamera;
    public Camera MainCamera
    {
        get { return m_mainCamera; }
    }

    //移动速度
    short m_nMovespeed;

    private bool m_bRun = false;
//     public bool IsRun
//     {
//         get { return m_bRun; }
//         set { m_bRun = value; }
//     }

#region 目标

    //目标
    Transform m_transTarget;
    //距离
    public Vector3 m_vec3Distancetarget;

    //固定摄像机
    bool m_bLockCamera = false;
    public bool LockCamera
    {
        get { return m_bLockCamera; }
        set { m_bLockCamera = value; }
    }
    //镜头看向目标
    bool m_bLookingAtTarget = true;
    public bool LookAtTarget
    {
        get { return m_bLookingAtTarget; }
        set { m_bLookingAtTarget = value; }
    }
	
#endregion

#region 旋转
    //重置用的旋转数据
    private float m_fCurPreRotateX;
    private float m_fCurRotateX;
    private float m_fCurRotateY;
    private float m_fCurRotateDiff;
    //当前旋转数据
    private float m_fPreRotateX;
    private float m_fRotateX;
    private float m_fRotateY;
    private float m_fRotateDiff;

    //速度
    private float m_fRotateSpeedX;
	public float RotateSpeedX
	{
		set { m_fRotateSpeedX = value; }
	}

    private float m_fRotateSpeedY;
	
    private float m_fRotateDeltaX;
    private float m_fRotateDeltaY;

    private bool m_bRotateX;
    private bool m_bRotateY;

	public Action onRotXComplete;
	public Action onRotYComplete;

//     private bool m_b
#endregion

#region 缩放
    float m_fFov;
    float m_fCurFov;

    //缩放时间
    float m_fZoomDelta;
    //缩放速度
    float m_fZoomSpeed;

    float m_fZoomDelayTime;

#endregion
   
#region 震屏
    //震动时间与间隔
    private float m_fShakeTime;
    private float m_fShakeDelta;
    //单次震动计数
    private float m_fFrameTime;
    //震动幅度
    private float m_fShakeRange;
    private bool m_bShakeCamera = false;

    //记录本帧的位置
    Vector3 m_vec3LastPosition;
#endregion

#region BossCamera
    private bool m_bIsBossCamera =false;
    private Transform m_transBoss;
#endregion
	
    //TODO 是否和剧情有关?
    bool m_bIsStartConvertion = false;

	void Awake()
	{
        Init();
	}

	void Start () 
	{
//        Init();
	}

    public void RunMainCamera()
    {
        m_bRun = true;
        Init();
    }

    public void StopMainCamera()
    {
        m_bRun = false;
    }

    public void Init()
    {
        GetMainCamera();
        ResetCam(true);
        if (null != m_transMainCamera && null != m_transTarget)
        {
            m_transMainCamera.LookAt(m_transTarget);
        }

		onRotXComplete = null;
		onRotYComplete = null;
    }

    private void GetMainCamera()
    {
        m_mainCamera = Camera.main;
        if (null != m_mainCamera)
        {
            m_transMainCamera = m_mainCamera.transform;
        }
//         m_transMainCamera = transform;
    }

	public Vector3 GetDistanceFromCamera(Vector3 point)
	{
		Vector3 cameraPos = Vector3.zero;
		if (m_mainCamera != null)
		{
			cameraPos = m_transMainCamera.position;
		}

		return (cameraPos - point);
	}

    public void LookTarget(Transform traget)
    {
        m_transTarget = traget;
    }

// 	public void LookTargetWithZoom(Transform _target, int _zoomspeed, int _fov, float _delay, bool isStatic = false)
// 	{
// 		m_transTarget = _target;
// 		
// 		if (_delay > 0)
// 		{
// 			resetstart = true;
// 			resetcam_delay = _delay;
// // 			character.GetComponent<Character>().StopControl();
// 		}
// 
// 		if (_fov != 0)
// 		{
// 			ZoomIn(_zoomspeed, _fov, _delay);
// 		}
// 	}
	
    //重置摄像机
    //@bInit  是否为初始化重置，初始化重置会重置保存的摄像机状态 
	public void ResetCam(bool bInit = false, bool bLookAtTarget = true, bool bLockCamera = false)
	{
        m_bLookingAtTarget = bLookAtTarget;
        m_bLockCamera = bLockCamera;

        if (bInit)
        {
            //目标
            m_transTarget = null;
            //旋转
            m_fCurPreRotateX = 0;
            m_fCurRotateX = CameraSetting.ROTATEX;
            m_fCurRotateDiff = 0;
            m_fCurRotateY = CameraSetting.ROTATEY;
            m_fRotateDeltaX = 0.0f;
            m_fRotateDeltaY = 0.0f;
			ResetRotateSpeed();
//            m_fRotateSpeedX = CameraSetting.ROTATEX_SPEED;
//            m_fRotateSpeedY = CameraSetting.ROTATEY_SPEED;

            //缩放
            m_fCurFov = CameraSetting.FOV;
            m_fZoomSpeed = CameraSetting.FOV_SPEED;
            m_fZoomDelta = 0;
            m_fZoomDelayTime = 0.0f;
            //摄像机本身
            m_nMovespeed = CameraSetting.MOVE_SPEED;
//             m_vec3Distancetarget = new Vector3(0.0f, 19.5f, -15.6f);
            m_vec3Distancetarget = new Vector3(0.0f, 0.0f, -25.0f);   
        }

        m_fPreRotateX = m_fCurPreRotateX;
        m_fRotateX = m_fCurRotateX;
        m_fRotateY = m_fCurRotateY;
        m_fRotateDiff = m_fCurRotateDiff;

        m_bRotateX = true;
        m_bRotateY = true;

        m_fFov = m_fCurFov;

        //震动
        ResetShake();

//         m_bfovchange = true;
	}

    public void ResetShake()
    {
        m_mainCamera.rect = new Rect(0.0f, 0.0f, 1.0f, 1.0f);
        m_fShakeTime = CameraSetting.SHAKE_TIME;
        m_fShakeDelta = CameraSetting.SHAKE_DELTA;
        m_fShakeRange = CameraSetting.SHAKE_RANGE;
        m_fFrameTime = 0.0f;

        m_bShakeCamera = false;
    }

	public void ResetRotateSpeed()
	{
		m_fRotateSpeedX = CameraSetting.ROTATEX_SPEED;
		m_fRotateSpeedY = CameraSetting.ROTATEY_SPEED;
	}

	public Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Quaternion angle)
	{
		return angle * (point - pivot) + pivot;
	}

    void Update() 
	{
        if (!m_bRun)
        {
            return;
        }

        if (null == m_mainCamera || null == m_transMainCamera)
        {
            GetMainCamera();
        }

        if (null == m_mainCamera || null == m_transMainCamera)
        {
            return;
        }

//        if (m_transTarget == null)
//        {
//            m_transTarget = UnitManager.Singleton.Hero.transform;
//            m_transMainCamera.LookAt(m_transTarget);
//        }

        if (null == m_transTarget)
        {
            return;
        }

        Vector3 targetPosition;
        if(m_bIsBossCamera)
        {
            targetPosition = new Vector3(m_transTarget.position.x + (m_transBoss.position.x - m_transTarget.position.x) * 0.3f,
                                                        0.0f,
                                                        m_transTarget.position.z + (m_transBoss.position.z - m_transTarget.position.z) * 0.3f);
        }
        else
        {
            targetPosition = m_transTarget.position;
        }

        //旋转
        if (!m_bLockCamera)
        {
//             bool bRotate = (m_fRotateX != m_transMainCamera.rotation.ToEuler().y) || (m_fRotateY != m_transMainCamera.rotation.ToEuler().x);

            //增加判断条件，修复摄相机抖动
//             if ((int)m_fRotateX == (int)m_transMainCamera.rotation.eulerAngles.y)
//             {
//                 m_bRotateX = false;
//             }
// 
//             if ((int)m_fRotateY == (int)m_transMainCamera.rotation.eulerAngles.x)
//             {
//                 m_bRotateY = false;
//             }

            if(null != m_vec3LastPosition && m_vec3LastPosition.magnitude > 0.0f)
            {
                m_transMainCamera.position = m_vec3LastPosition;
                m_vec3LastPosition = Vector3.zero;
            }


		    m_fRotateDeltaX += (Time.deltaTime * m_fRotateSpeedX);
		    float angleX = Mathf.Lerp(m_fPreRotateX, m_fRotateX, m_fRotateDeltaX);
			if (angleX == m_fRotateX && onRotXComplete != null)
			{
				m_fRotateX %= 360.0f;
				onRotXComplete();
			}

            m_fRotateDeltaY += (Time.deltaTime * m_fRotateSpeedY);
		    float angleY = Mathf.Lerp(m_fRotateDiff, m_fRotateY, m_fRotateDeltaY);
			if (angleY == m_fRotateY && onRotYComplete != null)
			{
				angleY %= 360.0f;
				onRotYComplete();
			}

            //TODO GAMEDEBUG?
            Vector3 chaposition = targetPosition + m_vec3Distancetarget;
            chaposition = RotatePointAroundPivot(chaposition, targetPosition, Quaternion.Euler(angleY, angleX, 0.0f));
            m_transMainCamera.position = Vector3.Lerp(m_transMainCamera.position, chaposition, Time.deltaTime * m_nMovespeed);
            
            if (m_bRotateX || m_bRotateY)
            {
//                 m_transMainCamera.position = Vector3.Lerp(m_transMainCamera.position, chaposition, Mathf.Max(m_fRotateDeltaX, m_fRotateDeltaY));
                m_transMainCamera.LookAt(targetPosition, Vector3.up);
            }

            if ((int)m_fRotateX == (int)m_transMainCamera.rotation.eulerAngles.y && m_transMainCamera.position.x == chaposition.x && m_transMainCamera.position.z == chaposition.z)
            {
                m_bRotateX = false;
            }

            if ((int)m_fRotateY == (int)m_transMainCamera.rotation.eulerAngles.x && m_transMainCamera.position.y == chaposition.x && m_transMainCamera.position.z == chaposition.z)
            {
                m_bRotateY = false;
            }

		}

        //目标
        if (m_bLookingAtTarget)
        {
//             m_transMainCamera.LookAt(m_transTarget,Vector3.up);
        }

#region 鼠标滑轮缩放 移动版不会有这个
        if (Application.platform == RuntimePlatform.WindowsPlayer ||
           Application.platform == RuntimePlatform.WindowsEditor ||
           Application.platform == RuntimePlatform.OSXPlayer ||
           Application.platform == RuntimePlatform.OSXEditor)
        {
            //Zoom out
            if (Input.GetAxis("Mouse ScrollWheel") < 0)
            {
                ZoomIn(m_mainCamera.fieldOfView - 3,10,true);
            }
            //Zoom in
            if (Input.GetAxis("Mouse ScrollWheel") > 0)
            {
                ZoomIn(m_mainCamera.fieldOfView + 3,10,true);
            }
        }
#endregion

        //缩放
        m_fZoomDelta += Time.deltaTime * m_fZoomSpeed;
		if (m_mainCamera.orthographic)
		{
			m_mainCamera.orthographicSize = Mathf.Lerp(m_mainCamera.orthographicSize, m_fFov, m_fZoomDelta);
		}
		else
		{
        	m_mainCamera.fieldOfView = Mathf.Lerp(m_mainCamera.fieldOfView, m_fFov, m_fZoomDelta);
		}

        if (m_fZoomDelayTime > 0.0f)
        {
            m_fZoomDelayTime -= Time.deltaTime;
            if (m_fZoomDelayTime <= 0.0f)
            {
                ResetCam();
            }
        }
//		else if (!m_bIsStartConvertion)
//		{
//			m_bIsStartConvertion = true;
//		}

        if (m_bFPSCamera)
        {
            m_transMainCamera.position = m_transTarget.position + new Vector3(0.0f, 1.5f, 0.0f);
            m_transMainCamera.rotation = m_transTarget.rotation;
            return;
        }

        //震动
		if (m_bShakeCamera)
		{
            m_vec3LastPosition = m_transMainCamera.position;
			if(m_fShakeTime > 0)
			{
				m_fShakeTime -= Time.deltaTime;
				if(m_fShakeTime <= 0)
				{
                    ResetShake();
				}
				else
				{
					m_fFrameTime += Time.deltaTime;
					
					if(m_fFrameTime > m_fShakeDelta)
					{
						m_fFrameTime = 0;
//                         m_mainCamera.rect = new Rect((-m_fShakeRange + m_fShakeRange * 2 * Random.value), (-m_fShakeRange + m_fShakeRange * 2 * Random.value), 1.0f, 1.0f);
                        m_transMainCamera.position = new Vector3(m_transMainCamera.position.x,
                            m_transMainCamera.position.y,
                            m_transMainCamera.position.z + (-m_fShakeRange + m_fShakeRange * 2 * UnityEngine.Random.value));
                    }

				}
			}
		}
	}

	public void ShakeCam(float fShakeTime = CameraSetting.SHAKE_TIME, 
                            float fShakeDelta = CameraSetting.SHAKE_DELTA,
                            float fShakeRange = CameraSetting.SHAKE_RANGE)
	{
        m_fShakeTime = fShakeTime;
        m_fShakeDelta = fShakeDelta;
        m_fShakeRange = fShakeRange;
		m_bShakeCamera = true;
	}

    //@bSetCurrent 如果为true，则保存摄像机的状态，保存的状态不会被reset重置
	public void RotateCameraX(float rotate, float speed = CameraSetting.ROTATEX_SPEED, bool bSetCurrent = false)
	{
// 		if (rotate == m_fRotateX)
// 		{
// 			return;
// 		}

		m_fPreRotateX = m_fRotateX;
		m_fRotateX = m_fPreRotateX + rotate;
		m_fRotateDeltaX = 0.0f;
		m_fRotateSpeedX = speed;

        if (bSetCurrent)
        {
            m_fCurPreRotateX = m_fPreRotateX;
            m_fCurRotateX = m_fRotateX;
        }

        m_bRotateX = true;
	}

    //@bSetCurrent 如果为true，则保存摄像机的状态，保存的状态不会被reset重置
	public void RotateCameraToX(float rotate, float speed = CameraSetting.ROTATEX_SPEED, bool bSetCurrent = false, bool clockwise = true)
    {
        if (rotate == m_fRotateX)
        {
            return;
        }

		m_fPreRotateX = m_fRotateX;
        m_fRotateX = clockwise ? rotate : -rotate;
        m_fRotateDeltaX = 0.0f;
		m_fRotateSpeedX = speed;

        if (bSetCurrent)
        {
            m_fCurPreRotateX = m_fPreRotateX;
            m_fCurRotateX = m_fRotateX;
        }

        m_bRotateX = true;
    }

// 	public void RotateCameraY(float rotate, bool clockwise = true)
// 	{
//         if (rotate == m_fRotateY)
//         {
//             return;
//         }
// 
//         m_fRotateDiff = mytransform.eulerAngles.x;
//         m_fRotateY = m_fRotateDiff + rotate;
//         m_fRotateDeltaY = 0.0f;
// 	}

    //@bSetCurrent 如果为true，则保存摄像机的状态，保存的状态不会被reset重置
	public void RotateCameraToY(float rotate, float speed = CameraSetting.ROTATEY_SPEED, bool bSetCurrent = false, bool clockwise = true)
    {
        if (rotate == m_fRotateY)
        {
            return;
        }

        m_fRotateDiff = m_fRotateY;
        m_fRotateY = clockwise ? rotate : rotate;
        m_fRotateDeltaY = 0.0f;
		m_fRotateSpeedY = speed;

        if (bSetCurrent)
        {
            m_fCurRotateDiff = m_fRotateDiff;
            m_fCurRotateY = m_fRotateY;
        }

        m_bRotateY = true;
        
    }

    public void ZoomIn(float fFov, float fZoomSpeed = CameraSetting.FOV_SPEED, bool bSetCurrent = false)
	{
		m_fZoomSpeed = fZoomSpeed;
		m_fFov =fFov;
        m_fZoomDelta = 0.0f;

        if (bSetCurrent)
        {
            m_fCurFov = fFov;
        }
	}

    public void ZoomInWithDelay(float fFov, float fDelayTime,float fZoomSpeed = CameraSetting.FOV_SPEED, bool bSetCurrent = false)
    {
        m_fZoomDelayTime = fDelayTime;
    }

    public void BossCamera(Transform transBoss)
    {
        m_bIsBossCamera = true;
        m_transBoss = transBoss;
    }

    public void StopBossCamera()
    {
        m_bIsBossCamera = false;
    }

#region XXX
    private bool m_bFPSCamera = false;
    public bool FPSCamera
    {
        get { return m_bFPSCamera; }
        set { m_bFPSCamera = value; }
    }
//     public void FPSCamera(bool bOpen)
//     {
//         m_bFPSCamera = bOpen;
//     }
#endregion


}
