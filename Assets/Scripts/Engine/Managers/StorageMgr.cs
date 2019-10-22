using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Linq;
using System.Text;
using System.Reflection;
using System;
//using System.Xml.XmlNodeList;

public delegate object DeserializeMethod(string sType, string value);

public abstract class StorageMgr
{
    public StorageMgr(string dataFileName, DeserializeMethod ds)
    {
        Deserializer = ds;
        m_datafileMgr = dataFileName;
        ProcessAttributes();
    }




    public void ShowDebug(string section)
    {
        StorageKey sk = m_serialized[section];
        Debug.Log("--------------" + section + "---------------------");
        if (sk == null)
        {
            Debug.Log("NULL");
        }
        else
        {
            sk.ShowDebug();
        }
        Debug.Log("-----------------------------------");
    }

    //Application.persistentDataPath
    //Removes all keys and values from the preferences. Use with caution.
    public void DeleteAll()
    {
        m_serialized.Clear();
    }

    //Removes key and its corresponding value from the preferences.
    public void Delete(string section)
    {
        if (m_serialized.ContainsKey(section))
            m_serialized.Remove(section);
    }

    public void Delete(string section, string key)
    {
        if (m_serialized.ContainsKey(section))
        {
            StorageKey sk = m_serialized[section];
            sk.Delete(key);
        }
    }

    public void Set(string section, string key, object a_value)
    {
        // TODO 2: Almacenamos una Key en el diccionario m_serialized (Solución 002 A) 
        if (!m_serialized.ContainsKey(section))
        {
            m_serialized.Add(section, new StorageKey(m_allowedTypes));
        }
        m_serialized[section].Set(key, a_value);

    }


    public T Get<T>(string section, string key)
    {
        // TODO 2: Almacenamos una Key en el diccionario m_serialized
        Debug.Assert(m_serialized.ContainsKey(section), "Section " + section + " not exist");        StorageKey sk = m_serialized[section];
        Debug.Assert(sk.ContainsKey(key), "The Key " + key + " in section " + section + " not exist");
        return sk.Get<T>(key);
    }


    public bool Contains(string section)
    {
        return m_serialized.ContainsKey(section);
    }

    public bool Contains(string section, string key)
    {
        bool result = m_serialized.ContainsKey(section);
        if (result)
        {
            StorageKey sk = m_serialized[section];
            result = sk.ContainsKey(key);
        }
        return result;
    }

    public void Store()
    {
        string fileFullName = Application.persistentDataPath + "/" + m_datafileMgr;
        string data = "<save>\n";

        foreach (KeyValuePair<string, StorageKey> sectionPair in m_serialized)
        {
            StorageKey sk = sectionPair.Value;
            data += "\t<section name=\"" + sectionPair.Key + "\">\n";
            List<KeyValuePair<string, object>> pairList = sk.GetList();
            foreach (KeyValuePair<string, object> pairStrObj in pairList)
            {
                System.Type type = pairStrObj.Value.GetType();
                string strType = type.ToString();
                string strValue = pairStrObj.Value.ToString();
                data += "\t\t<field name=\"" + pairStrObj.Key + "\" type=\"" + strType + "\" value=\"" + strValue + "\"/>\n";
            }
            data += "\t</section>\n";
        }
        data += "</save>";
        System.IO.File.WriteAllText(fileFullName, data);

    }

    public long GetSize(string file)
    {
        string fileFullName = Application.persistentDataPath + "/" + file;
        System.IO.FileInfo info = new System.IO.FileInfo(fileFullName);
        if (!info.Exists)
            return -1;
        else
            return info.Length;
    }

    /******************************** FILE *****************************/

    public void Store(string fileName, string data)
    {
        string fileFullName = Application.persistentDataPath + "/" + fileName;
        System.IO.File.WriteAllText(fileFullName, data);
    }

    public string Load(string fileName)
    {
        string fileFullName = Application.persistentDataPath + "/" + fileName;
        return System.IO.File.ReadAllText(fileFullName);
    }

    public bool ExistFile(string fileName)
    {
        string fileFullName = Application.persistentDataPath + "/" + fileName;
        return System.IO.File.Exists(fileFullName);
    }

    public void Append(string fileName, string data)
    {
        string fileFullName = Application.persistentDataPath + "/" + fileName;
        System.IO.StreamWriter file = new System.IO.StreamWriter(fileFullName, true);
        file.Write(data);
        file.Close();
    }

    /******************VOLATILE**********************************/


    public void DeleteVolatile(string section)
    {
        if (m_nonserialized.ContainsKey(section))
            m_nonserialized.Remove(section);
    }

    public void DeleteVolatile(string section, string key)
    {
        if (m_nonserialized.ContainsKey(section))
        {
            m_nonserialized[section].Remove(key);
        }
    }

    public void DeleteAllVolatile()
    {
        m_nonserialized.Clear();
    }

    public void SetVolatile(string section, string key, object a_value)
    {
        if (!m_nonserialized.ContainsKey(section))
        {
            m_nonserialized.Add(section, new Dictionary<string, object>());
        }
        m_nonserialized[section][key] = a_value;
    }


    public T GetVolatile<T>(string section, string key)
    {
        Debug.Assert(m_nonserialized.ContainsKey(section), "Section " + section + " not exist");
        Dictionary<string, object> dictionary = m_nonserialized[section];
        Debug.Assert(dictionary.ContainsKey(key), "The Key " + key + " in section " + section + " not exist");
        return (T)dictionary[key];
    }


    public bool ContainsVolatile(string section)
    {
        return m_nonserialized.ContainsKey(section);
    }

    public bool ContainsVolatile(string section, string key)
    {
        bool result = m_nonserialized.ContainsKey(section);
        if (result)
        {
            result = m_nonserialized[section].ContainsKey(key);
        }
        return result;
    }



    /*public T GetUnserialized<T>(string section, string key)
	{
		
	}*/

    public bool ExistFile()
    {
        string fileFullName = Application.persistentDataPath + "/" + m_datafileMgr;
        return System.IO.File.Exists(fileFullName);
    }


    public void Load()
    {

        string fileFullName = Application.persistentDataPath + "/" + m_datafileMgr;

        XmlDocument root = new XmlDocument();

        m_serialized.Clear();

        root.Load(fileFullName);
        XmlNodeList sectionsList = root.GetElementsByTagName("section");
        for (int i = 0; i < sectionsList.Count; ++i)
        {
            XmlNode section = sectionsList[i];
            string name = section.Attributes["name"].Value;

            StorageKey sk = new StorageKey(m_allowedTypes);
            m_serialized.Add(name, sk);

            List<XmlNode> fieldList = XMLUtils.GetNodes(section, "field");
            for (int j = 0; j < fieldList.Count; ++j)
            {
                XmlNode fieldnode = fieldList[j];
                string strType = fieldnode.Attributes["type"].Value;
                string strValuej = fieldnode.Attributes["value"].Value;
                string fieldName = fieldnode.Attributes["name"].Value;

                sk.Set(fieldName, Deserializer(strType, strValuej));
            }
        }
    }

    public static DeserializeMethod Deserializer = null;

    protected void ProcessAttributes()
    {
        m_allowedTypes = new List<Type>();
        int count = 0;
        System.Reflection.MemberInfo info = GetType();
        object[] attributes = info.GetCustomAttributes(true);
        // Esto se puede realizar ahora.
        // info.GetCustomAttribute<AllowedTypes>();
        for (int i = 0; i < attributes.Length; ++i)
        {
            if (attributes[i].GetType() == typeof(AllowedTypeToStorage))
            {
                //TODO 1: Añadir a la lista de atributos permitidos. (solucion 001)
                AllowedTypeToStorage allowedType = attributes[i] as AllowedTypeToStorage;
                m_allowedTypes.Add(allowedType.m_type);
                count++;

            }
        }

        Debug.Assert(count > 0, "Error: It Must be defined a Allowed types Attributes");
    }

    List<System.Type> m_allowedTypes;
    protected Dictionary<string, StorageKey> m_serialized = new Dictionary<string, StorageKey>();
    protected Dictionary<string, Dictionary<string, object>> m_nonserialized = new Dictionary<string, Dictionary<string, object>>();
    protected string m_datafileMgr;


    /*GetFloat 	Returns the value corresponding to key in the preference file if it exists.
    GetInt 	Returns the value corresponding to key in the preference file if it exists.
    GetString 	Returns the value corresponding to key in the preference file if it exists.
    HasKey 	Returns true if key exists in the preferences.
    Save 	Writes all modified preferences to disk.
    SetFloat 	Sets the value of the preference identified by key.
    SetInt 	Sets the value of the preference identified by key.
    SetString 	Sets th*/
}
