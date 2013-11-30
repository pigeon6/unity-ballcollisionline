using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BallCollisionLine : MonoBehaviour {

	[SerializeField]
	private Vector2 m_ballSizeMinMax = new Vector3(0.1f, 0.3f);

	[SerializeField]
	private Vector2 m_velocityMinMax = new Vector3(-0.1f, 0.1f);

	[SerializeField]
	private Vector2 m_sizeMinMax;
	
	public int m_circleSize = 50;

	[Range(0.01f, 10.0f)]
	public float
		lineWidth = 1.5f;

	[System.Serializable]
	public class Circle {
		public Vector3 position;
		public Vector3 vp;
		public Vector3 velocity;
		public float size;
	}

	[SerializeField]
	public List<Circle> m_circles;

	[SerializeField]
	public List<Vector3> m_middlePoints;

	public Material mat;

	// Use this for initialization
	IEnumerator Start () {

		m_middlePoints = new List<Vector3>();


		while(true) {
			 
			if( m_circleSize != m_circles.Count ) {
				_ReInitializeCircles();
			}

			foreach(Circle c in m_circles) {
				Vector3 newPos = c.position + (c.velocity * Time.deltaTime);
				c.position = new Vector3( _MapTo01(newPos.x),_MapTo01(newPos.y),_MapTo01(newPos.z));
				c.vp = Camera.main.ViewportToWorldPoint(c.position);
			}

			m_middlePoints.Clear ();
			foreach(Circle src in m_circles) {
				foreach(Circle dst in m_circles) {
					if( src != dst ) {
						if( Vector3.Distance(src.position, dst.position) <= ( src.size + dst.size ) ) {
							Vector3 mp = Vector3.Lerp (src.position, dst.position, 0.5f);
							if( !m_middlePoints.Contains(mp) ) {
								m_middlePoints.Add (mp);
							}
						}
					}
				}
			}

			yield return new WaitForEndOfFrame();
		}
	}

	private void _ReInitializeCircles() {
		m_circles = new List<Circle>();
		for(int i = 0; i<m_circleSize;++i) {
			Circle c = new Circle();
			c.position = new Vector3(Random.Range (0.0f, 1.0f), Random.Range (0.0f,1.0f), 0.0f);
			c.velocity = new Vector3(Random.Range (m_velocityMinMax.x, m_velocityMinMax.y), Random.Range (m_velocityMinMax.x, m_velocityMinMax.y), 0.0f);
			c.size = Random.Range (m_ballSizeMinMax.x, m_ballSizeMinMax.y);
			m_circles.Add (c);
		}
	}

	private float _MapTo01(float f) {
		if(f > 1.0f) {
			return f - 1.0f;
		}
		if(f < 0.0f) {
			return f + 1.0f;
		}

		return f;
	}

	// Enables the line width before rendering the scene.
//	void OnPreRender ()
//	{
//		UnityLineWidthPlugin_Initialize ();
//		GL.IssuePluginEvent ((int)(lineWidth * 100));
//	}
	
	void OnPostRender() {
		if (!mat) {
			Debug.LogError("Please Assign a material on the inspector");
			return;
		}

		if( m_circles != null ) {

			GL.PushMatrix();
			mat.SetPass(0);
			GL.LoadOrtho();

			for(int i=0; i<m_middlePoints.Count-1; ++i) {
				GL.Begin(GL.LINES);
				GL.Color(Color.red);
				GL.Vertex(m_middlePoints[i]);
				GL.Vertex(m_middlePoints[i+1]);
				GL.End();
			}
			GL.PopMatrix();
		}
//		GL.IssuePluginEvent (0);
	}

//	[DllImport ("UnityLineWidthPlugin")]
//	private static extern void UnityLineWidthPlugin_Initialize ();

	void OnDrawGizmos() {		

//		Gizmos.color = Color.yellow;
//		Gizmos.DrawSphere(p, 0.1F);

		if( m_circles != null ) {
			foreach(Circle c in m_circles) {
				Gizmos.color = Color.red;
				Vector3 p = camera.ViewportToWorldPoint(new Vector3(c.position.x, c.position.y, camera.nearClipPlane));
				Gizmos.DrawWireSphere(p, c.size);
			}
		}
	}
}
