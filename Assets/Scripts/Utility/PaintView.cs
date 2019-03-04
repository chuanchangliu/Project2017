using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PaintView : MonoBehaviour {

    #region 相关属性

    [SerializeField] //绘制用的Shader和Material
    private Shader _paintBrushShader;
    private Material _paintBurshMater;

    [SerializeField] //清理用的Shader和Material
    private Shader _clearBrushShader;
    private Material _clearBurshMater;

    [SerializeField] //默认笔刷图 
    private RawImage _defaultBurshRawImage;
    [SerializeField] // 笔刷图合集
    private Texture _defaultBrushTexture;

    [SerializeField] //默认笔刷颜色
    private Color _defaultColor;
    [SerializeField]
    private Image _colorImageL;
    [SerializeField]
    private Image _colorImageR;
    
    [SerializeField] //画布和渲染贴图
    private RawImage _paintBoard;
    private RenderTexture _renderTexture;

    //笔刷粗细和粗细描述文字
    private float _brushSize;
    private Text _brushSizeText;

    [SerializeField] //屏幕尺寸
    private int _screenWidth;
    private int _screenHeight;

    //笔刷的间隔大小和上一次的位置
    private float _brushLerpSize;
    private Vector2 _lastPoint;
    #endregion

    // Use this for initialization
    void Start () {
        InitData();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    #region 对外函数

    #endregion

    #region 内部函数

    void InitData()
    {
        _brushSize = 300f;
        _brushLerpSize = (_defaultBrushTexture.width + _defaultBrushTexture.height) / 2.0f / _brushSize;
        Debug.Log("_brushLerpSize is : "+_brushLerpSize.ToString());
    }

    #endregion
}
