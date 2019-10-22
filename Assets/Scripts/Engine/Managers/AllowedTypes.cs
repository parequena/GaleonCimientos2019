using UnityEngine;
using System;
using System.Linq;
using System.Text;
/// <summary>
/// Permited types. Tipos permitidos en el alamcenamiento persistente.
/// </summary>
[AttributeUsage(AttributeTargets.All)]
public class AllowedTypes : System.Attribute
{
    public readonly System.Type[] m_types;

    public AllowedTypes(System.Type[] types)
    {
        m_types = types;
    }

    public override String ToString()
    {
        string s = "";
        if(m_types.Length > 0)
        {
            s = m_types[0].ToString();
            for (int i = 1; i < m_types.Length; ++i)
            {
                s += "," + m_types[i];
            }
        }
        return s;
    }
}
