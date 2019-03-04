using UnityEngine;

public class UIEventListener : MonoBehaviour
{
	public delegate void VoidDelegate (GameObject go);
	public delegate void BoolDelegate (GameObject go, bool state);
	public delegate void VectorDelegate (GameObject go, Vector2 delta);
	public delegate void ObjectDelegate (GameObject go, GameObject obj);

	public VoidDelegate onClick;
	public BoolDelegate onPress;
	public VectorDelegate onDrag;
	public ObjectDelegate onDrop;

	bool isColliderEnabled
	{
		get
		{
			Collider c = GetComponent<Collider>();
			if (c != null) return c.enabled;
			Collider2D b = GetComponent<Collider2D>();
			return (b != null && b.enabled);
		}
	}

	void OnClick ()					{ if (isColliderEnabled && onClick != null) onClick(gameObject); }
    void OnPress(bool isPressed) { if (isColliderEnabled && onPress != null) onPress(gameObject, isPressed); }
	void OnDrag (Vector2 delta)		{ if (onDrag != null) onDrag(gameObject, delta); }
	void OnDrop (GameObject go)		{ if (isColliderEnabled && onDrop != null) onDrop(gameObject, go); }

	public void Clear ()
	{
		onClick = null;
		onPress = null;
		onDrag = null;
		onDrop = null;
	}

	static public UIEventListener Get (GameObject go)
	{
		UIEventListener listener = go.GetComponent<UIEventListener>();
		if (listener == null) listener = go.AddComponent<UIEventListener>();
		return listener;
	}
}
