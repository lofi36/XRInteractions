using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UIElements;

public enum WidgetType
{
    LocationWidget,
    DeleteWidget,
    ImageWidget
}
public class Widget : MonoBehaviour
{
    [SerializeField]
    public Color m_normalColor;

    [SerializeField]
    public Color m_onHoverColor;
    
    [SerializeField]
    public WidgetType m_widgetType;

    public string dataPath
    {get;set;}
    private Material m_material;

    void Start()
    {
        Init();
    }

    private void Init()
    {
        m_material = GetComponent<Renderer>().material;
        SetButtonColor(m_normalColor);
    }

    private void OnMouseOver()
    {
        SetButtonColor(m_onHoverColor);
    }
    private void OnMouseExit()
    {
        SetButtonColor(m_normalColor);
    }

    private void OnMouseDown()
    {
        Anchor parent = transform.parent.GetComponent<Anchor>();
        parent.OnWidgetClick.Invoke(this.gameObject);
    }
    private void SetButtonColor(Color color)
    {
         m_material.SetColor("_Color", color);
    }
}
