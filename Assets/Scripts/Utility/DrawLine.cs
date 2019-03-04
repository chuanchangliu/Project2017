using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawLine : MonoBehaviour {

    private GameObject clone;
    private LineRenderer line;
    private int vertexCount;
    public GameObject target;

	// Use this for initialization
	void Start () {
		
	}

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {  
            clone = (GameObject)Instantiate(target, target.transform.position, Quaternion.identity);
 
            line = clone.GetComponent<LineRenderer>();
            
            line.startColor = Color.red;
            line.endColor = Color.blue;

            line.startWidth = 0.1f;
            line.endWidth = 0.1f;
           
            vertexCount = 0;
        }
        if (Input.GetMouseButton(0))
        {
            //每一帧检测，按下鼠标的时间越长，计数越多  
            vertexCount++;
            //设置顶点数  
            line.SetVertexCount(vertexCount);
            //设置顶点位置(顶点的索引，将鼠标点击的屏幕坐标转换为世界坐标)  
            line.SetPosition(vertexCount - 1, Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 15)));


        }

    }
}
