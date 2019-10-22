using UnityEngine;
using System.Collections;

[System.Serializable]
public class StorageMgrConfig
{
    [SerializeField]
    private string m_storageFileName;

    public string StorageFileName { get { return m_storageFileName; } set { m_storageFileName = value; } }
}


[System.Serializable]
public class InputMgrConfig
{
    [SerializeField]
    private InputMgr.TMouseButtonID m_buttonIdToPointAndClick;
    [SerializeField]
    private bool m_pointAndClickActive;

    public InputMgr.TMouseButtonID ButtonIdToPointAndClick { get { return m_buttonIdToPointAndClick; } set { m_buttonIdToPointAndClick = value; } }
    public bool PointAndClickActive { get { return m_pointAndClickActive; } set { m_pointAndClickActive = value; } }
}

[CreateAssetMenu(fileName = "BaseConfig", menuName = "Create GameMgrConfig")]
public class GameMgrConfig : ScriptableObject
{
    public StorageMgrConfig m_storageMgrConfig;
    public InputMgrConfig m_inputMgrConfig;

    public GameMgrConfig()
    {
        m_storageMgrConfig = new StorageMgrConfig();
        m_inputMgrConfig = new InputMgrConfig();
    }
}
