using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WidgetEvent : UnityEvent<GameObject>{}

public class Anchor : MonoBehaviour
{
    public int ID
    {get; set;}
    public string imagePath
    {get; set;}

    public WidgetEvent OnWidgetClick;

    private void OnEnable()
    {
        if(OnWidgetClick == null)
        {
            OnWidgetClick = new WidgetEvent();
        }
        OnWidgetClick.AddListener(WidgetClicked);
    }

    private void WidgetClicked(GameObject obj)
    {
        Widget widget = obj.GetComponent<Widget>();

        switch(widget.m_widgetType)
        {
            case WidgetType.LocationWidget:
            break;

            case WidgetType.DeleteWidget:
            SceneManager.OnAnchorDelete.Invoke(this);
            break;

            case WidgetType.ImageWidget:
            SceneManager.OnAnchorLoadImage.Invoke(this);
            break;            
        }
    }

    private void OnDestroy()
    {
        if (OnWidgetClick is not null)
        {
            OnWidgetClick.RemoveAllListeners();
            OnWidgetClick = null;
        }
    }
}