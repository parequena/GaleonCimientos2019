using UnityEngine;
using System.Collections;
using System;
using System.Runtime.CompilerServices;
using Object = UnityEngine.Object;



/// <summary>
/// Game mgr. Singleton que gestiona todos los managers del juego.
/// </summary>
/// 
public class GameMgr 
{
    public const string CONFIGURATION_FOLDER = "Config";
    public const string CONFIGURATION_FILE = "BaseConfig";
    /// <summary>
    /// Clase de la que hay que heredar para añadir managers specíficos del juego que desarrollemos.
    /// </summary>
    public abstract class ProjectSpecificMgrs
    {
        public ProjectSpecificMgrs(GameMgr gameMgr)
        {
            m_gameMgrRef = gameMgr;
        }

        protected void AddServerInGameMgr<T>() where T : Component
        {
            m_gameMgrRef.AddServer<T>();
        }

        protected GameMgr GetGameMgr() { return m_gameMgrRef; }
        private GameMgr m_gameMgrRef;
    }

    public static object Deserializer(string type, string data)
    {

        object obj = null;

        if (type == typeof(bool).ToString())
        {
            obj = System.Convert.ToBoolean(data);
        }
        else if (type == typeof(int).ToString())
        {
            obj = System.Convert.ToInt32(data);
        }
        else if (type == typeof(string).ToString())
        {
            obj = data;
        }
        else if (type == typeof(float).ToString())
        {
            obj = System.Convert.ToSingle(data);
        }
        else if ((type == typeof(Vector2).ToString()) || (type == typeof(Vector3).ToString()) || (type == typeof(Quaternion).ToString()))
        {
            //procesamos la cadena...
            string vector2Str = data.Substring(1);
            vector2Str = vector2Str.Substring(0, vector2Str.Length - 1);
            string[] vectorComponent = vector2Str.Split(',');
            Debug.Assert(vectorComponent.Length >= 2 && vectorComponent.Length <= 4, "Incorrect Format");
            string xStr = vectorComponent[0].Trim();
            string yStr = vectorComponent[1].Trim();
            if (type == typeof(Vector2).ToString())
            {
                obj = new Vector2(System.Convert.ToSingle(xStr), System.Convert.ToSingle(yStr));
            }
            else
            {
                string zStr = vectorComponent[2].Trim();
                if (type == typeof(Vector3).ToString())
                {
                    obj = new Vector3(System.Convert.ToSingle(xStr), System.Convert.ToSingle(yStr), System.Convert.ToSingle(zStr));
                }
                else
                {
                    string wStr = vectorComponent[3].Trim();
                    obj = new Quaternion(System.Convert.ToSingle(xStr), System.Convert.ToSingle(yStr), System.Convert.ToSingle(zStr), System.Convert.ToSingle(wStr));
                }
            }

        }
        else
        {
            Debug.Assert(false, "Incompatible Format: " + type);
        }
        return obj;
    }

    //Implementacion del storageMng que nos permite declarar los tipos de datos que permitimos almacenar en nuestros objetos.
    //TODO AllowedType
    [AllowedTypeToStorage(typeof(bool))]
    [AllowedTypeToStorage(typeof(int))]
    [AllowedTypeToStorage(typeof(string))]
    [AllowedTypeToStorage(typeof(float))]
    [AllowedTypeToStorage(typeof(Vector2))]
    [AllowedTypeToStorage(typeof(Vector3))]
    [AllowedTypeToStorage(typeof(Vector4))]
    [AllowedTypeToStorage(typeof(Quaternion))]
	private class StorageMgrImp : StorageMgr
	{
        public StorageMgrImp(string fileName) : base(fileName, GameMgr.Deserializer) { }
	}
	
	/// <summary>
	/// Gets the instance of the GameMgr.
	/// </summary>
	/// <returns>
	/// The instance.
	/// </returns>
    
	public static GameMgr GetInstance()
	{
		if(m_instance == null)
		{
			m_instance = new GameMgr();
		}
		return m_instance;
	}
	
	/// <summary>
	/// Initializes a new instance of the <see cref="GameMgr"/> class.
	/// </summary>
    private GameMgr()
	{
		ProcessBaseConfiguration();
		m_storageMgr = new StorageMgrImp(m_storageFileName);
		
		//Inicializamos el servidorPrincipal y le registramos todos los servidores...

        if (m_servers != null) return;

        m_servers = GameObject.Find("Servers");
        if(m_servers == null)
        {
            m_servers = new GameObject("Servers");
        }
        //Registramos todos los servidores...
        //InputServer: servidor de entrada.

        //SceneMgr: Gestiona la carga de escenas... 
        //TODO (SceneMgr)  
        AddServer<SceneMgr>();
        m_servers.GetComponent<SceneMgr>();
        //m_spawnerMgr = new SpawnerMgr(smAux);


        //TODO (InputMgr)
        m_inputMgr = AddServer<InputMgr>();
        m_inputMgr.Configure(0, true);


        //TODO (SceneMgr)  
        //AddServer<InputMgr>();

        // m_customMgrs = new CustomerMgr();
        //m_inputMgr = new InputMgr();
        //m_sceneMgr = new SceneMgr();
        // m_spawnerMgr = new SpawnerMgr(m_sceneMgr);

    }
	
	
	public StorageMgr GetStorageMgr()
	{
		return m_storageMgr;
	}
	
	public SpawnerMgr GetSpawnerMgr()
	{
		return m_spawnerMgr;
	}

    public SceneMgr GetSceneMgr()
    {
        return m_sceneMgr;
    }

    public InputMgr GetInputMgr()
    {
        return m_inputMgr;
    }
	
	public bool ExistServer(string name)
	{
		return m_servers.GetComponent(name) != null;
	}
	
	public bool ExistServer<T>() where T : Component
	{
		return m_servers.GetComponent<T>() != null;
	}
	
	public MonoBehaviour GetServer(string name)
	{
		return m_servers.GetComponent(name) as MonoBehaviour;
	}
	
	public T GetServer<T>() where T : Component
	{
		if(m_servers)
			return m_servers.GetComponent<T>();
		else
			return null;
	}
	
	public void Register<T>() where T : Component
	{
		m_servers.AddComponent<T>();
	}
	
	public void UnRegister<T>() where T : Component
	{
		Object.Destroy(m_servers.GetComponent<T>());
	}

    // --------------------- CustomMgrs ---------------------------------
    public bool IsCustomMgrInit() { return m_customMgrs != null; }
    public ProjectSpecificMgrs CustomMgr
    {
        get { return m_customMgrs; }
        set { m_customMgrs = value; }
    }
    // ------------------------------------------------------------------


	protected T AddServer<T>() where T : Component
    {
        T t = m_servers.GetComponent<T>();
        if (t != null)
            Object.DestroyImmediate(t);
        t = m_servers.AddComponent<T>();
        return t;
    }

	protected void ProcessBaseConfiguration()
	{
        GameMgrConfig gameMgrConfig = ScriptableObjectMgr.Load<GameMgrConfig>(CONFIGURATION_FOLDER+"/"+CONFIGURATION_FILE);

        m_storageFileName = gameMgrConfig.m_storageMgrConfig.StorageFileName;
		
        m_IM_buttonId = gameMgrConfig.m_inputMgrConfig.ButtonIdToPointAndClick;
        m_IM_PAndCActive = gameMgrConfig.m_inputMgrConfig.PointAndClickActive;

	}

    //Statics
    private static GameMgr m_instance = null;
	
    //Managers
	private StorageMgr m_storageMgr =   null;
	private SpawnerMgr m_spawnerMgr =   null;
	private GameObject m_servers    =   null;
    private ProjectSpecificMgrs m_customMgrs =   null;
    private InputMgr m_inputMgr = null;
    private SceneMgr m_sceneMgr = null;

    //Configuration fields...
	private string m_storageFileName;
	
	
	private InputMgr.TMouseButtonID m_IM_buttonId;
	private bool m_IM_PAndCActive;
}
