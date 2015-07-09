using UnityEngine;
using System.Collections;


public class GGUnit
{
	public int HP = 1000;
	public int MaxHp = 1000;
}
public class UIHpBar : MonoBehaviour {

    private GGUnit m_unit;
	//public GameObject owner;
	public Texture2D hpBar;
	public Texture2D hpPanel;

    private float m_fMaxHp = 0;
	private float m_fCurHp = 0;
	
	//zoom out
	private float m_fZoomOut = 0.5f;
	private float m_fNpcHeight = 0.0f;

	//

	// Use this for initialization
	void Start () {
        Init();
//         changeHpStatus(m_nMaxHp);
	}

//     public void setMaxHp(float hp)
//     {
//         if (hp < 0)
//         {
//             hp = 100f;
//         }
//         m_nMaxHp = hp;
//     }
	
	// Update is called once per frame
	void Update () {
	}

	void OnGUI()
	{
        if (null == m_unit)
        {
            return;
        }

        if (0 == m_fMaxHp)
        {
            m_fMaxHp = m_unit.MaxHp;
        }

        if (0 == m_fMaxHp)
        {
            return;
        }

        m_fCurHp = m_unit.HP;

		Vector2 barPos = getScreenPosition();
		Vector2 barSize = GUI.skin.label.CalcSize(new GUIContent(hpBar));

		float hpRatio = m_fCurHp / m_fMaxHp;
        float hpWidth = barSize.x * ((float)m_fCurHp / (float)m_fMaxHp);

        m_fZoomOut = 0.5f /(Camera.main.fov / 30.0f);
// 		GUI.DrawTexture(new Rect(barPos.x - barSize.x/(2 * zoomOut), barPos.y - barSize.y, barSize.x/zoomOut, barSize.y/zoomOut), hpPanel);
// 		GUI.DrawTextureWithTexCoords(new Rect(barPos.x - barSize.x/(2 * zoomOut), barPos.y - barSize.y, hpWidth/zoomOut, barSize.y/zoomOut), 
// 		                             hpBar,
// 		                             new Rect(0, 0, hpRatio, 1));
        float fUpRate = Mathf.Sin(Camera.main.transform.rotation.eulerAngles.x*Mathf.Deg2Rad);
        float test = Mathf.Sin(30);
        float test2 = Mathf.Sin(Mathf.Deg2Rad*30);
        GUI.DrawTexture(new Rect(barPos.x - barSize.x / 2 * m_fZoomOut, barPos.y - 55 * fUpRate*fUpRate*m_fZoomOut, barSize.x * m_fZoomOut, barSize.y * m_fZoomOut), hpPanel);
        GUI.DrawTextureWithTexCoords(new Rect(barPos.x - barSize.x / 2 * m_fZoomOut, barPos.y - 55 * fUpRate * fUpRate * m_fZoomOut, hpWidth * m_fZoomOut, barSize.y * m_fZoomOut),
                                     hpBar,
                                     new Rect(0, 0, hpRatio, 1));

	}

	Vector2 getScreenPosition()
	{
		Camera camera = Camera.main;
        if (null == camera)
        {
            return new Vector2(0.0f,0.0f);
        }
//         Debug.Log("-------------------------------");
		Vector3 worldPosition = new Vector3(transform.position.x, transform.position.y + m_fNpcHeight, transform.position.z);
//         Debug.Log(Time."worldPosition" + worldPosition.ToString());
		Vector2 pos = camera.WorldToScreenPoint(worldPosition);
//         Debug.Log("pos: " + pos.ToString());
//         Debug.Log("Screen.height: " + Screen.height.ToString());
// 		pos =new Vector2(pos.x, Screen.height - pos.y - m_fNpcHeight * Screen.height);
        pos = new Vector2(pos.x, Screen.height - pos.y);

		return pos;
	}

	//test
// 	public void changeHpStatus(float currentHp)
// 	{
// 		if(currentHp < 0)
// 		{
// 			currentHp = 0;
// 		}
// 		if(currentHp > m_nMaxHp)
// 		{
// 			currentHp = m_nMaxHp;
// 		}
// 		m_nCurHp = currentHp;
// 	}

    private void Init()
    {
		m_unit = new GGUnit();//gameObject.GetComponent<GGUnit>();
        if (null == m_unit)
        {
            Debug.LogWarning("UIHpBar 可能绑定了一个非GGUNIT");
            return;
        }

        m_fMaxHp = m_unit.MaxHp;
       // Transform test = transform.FindChild("body");
        //SkinnedMeshRenderer test2 = test.GetComponent<SkinnedMeshRenderer>();
		m_fNpcHeight = 3;//transform.FindChild("body").GetComponent<SkinnedMeshRenderer>().bounds.size.x;
        
    }


}
