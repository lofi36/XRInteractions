using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class AnchorEvent:UnityEvent<Anchor>{}
public class SceneManager : MonoBehaviour
{
    [SerializeField]
    public GameObject m_anchorPrefab;
    private GameObject m_AnchorRoot;
    private Dictionary<int, GameObject> m_Anchors;
    private int m_IDCounter = 0;
    public static AnchorEvent OnAnchorDelete;
    public static AnchorEvent OnAnchorLoadImage;

    void Start()
    {
        Init();        
    }
    private void OnEnable()
    {
        if(OnAnchorDelete == null)
        {
            OnAnchorDelete = new AnchorEvent();

        }
        if(OnAnchorLoadImage == null)
        {
            OnAnchorLoadImage = new AnchorEvent();
        }

        OnAnchorDelete.AddListener(DeleteAnchor);
        OnAnchorLoadImage.AddListener(LoadImage);
    }

    private void Init()
    {
        m_AnchorRoot = new GameObject();
        m_AnchorRoot.transform.position = Vector3.zero;

        m_Anchors = new Dictionary<int, GameObject>();
    }

    public void CreateAnchor(Vector3 position)
    {
        GameObject anchor = GameObject.Instantiate(m_anchorPrefab, m_AnchorRoot.transform);
        anchor.transform.position = position;

        Anchor data = anchor.AddComponent<Anchor>();
        data.enabled = true;
        data.ID = m_IDCounter;
        data.imagePath = string.Empty;

        m_Anchors.Add(m_IDCounter, anchor);
        m_IDCounter++;
    }
    public void DeleteAnchor(Anchor anchor)
    {
        int ID = anchor.ID;

        GameObject anchorToDelete = m_Anchors[ID];
        GameObject.Destroy(anchorToDelete);

        m_Anchors.Remove(ID);
    }
    public void LoadImage(Anchor anchor)
    {

    }

    public void SaveScene()
    {

    }

    public void LoadScene()
    {

    }

    void Update()
    {
        
    }
    private void OnDestroy()
    {
        if(OnAnchorDelete is not null)
        {
        OnAnchorDelete.RemoveListener(DeleteAnchor);
        OnAnchorDelete = null;
        }
        
        if(OnAnchorLoadImage is not null)
        {
        OnAnchorLoadImage.RemoveListener(LoadImage);
        OnAnchorLoadImage = null;
        }
    }
}
