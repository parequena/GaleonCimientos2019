using UnityEngine;
using System.Collections;

/// <summary>
/// Point and click input. Heredamos de InputController. Los InputController no son mas que controladores 
/// del input concretos al juego que estamos desarrollando y que se almacenan en el InputMgr.
/// Solo puede haber uno por escena.
/// 
/// Componente que implementa un imput PointAndClick. Se registra automaticamente en el InputManager y extiende la funcionalidad de este
/// aportando funcionalidad concreta de este tipo de control de Input.
/// </summary>
public class PointAndClickInput : InputController {
	
	//Evento de Cambiar Arma definido como un delegado.
	public delegate void ChangeWapon(int index );
	
	protected int m_nextWeaponMovile = 0;
	
	//Clase interna serializable para poder mostrarla desde el inspector.
	[System.Serializable]
	public class TWeapons
	{
		public int weaponIndex;
		public KeyCode keyCode;
	}
	
	//array de armas disponibles.
	public TWeapons[] m_weaponKeyAsign;
	
	//Ofrecemos una interfaz para que los componentes se registren al Input y pueda recibir eventos.
	public void RegisterChangeWeapon(ChangeWapon del)
	{
		m_changeWeapon += del;
	}
	
	//Desregistramos al componente del evento.
	public void UnRegisterChangeWeapon(ChangeWapon del)
	{
		m_changeWeapon -= del;
	}
	
	
	
	protected override void Update()
	{
		base.Update();
		#if UNITY_IPHONE || UNITY_ANDROID
		ProccessTouchEvent();
		#else
		OnChangeWeapon();
		#endif
	}
	
	protected void ProccessTouchEvent()
	{
		//Hemos decidido que para cambiar de arma pulsaremos con dos dedos la pantalla.
		if(Input.touchCount == 2)
		{
			if(m_changeWeapon != null)
			{
				m_nextWeaponMovile++;
				m_nextWeaponMovile = m_nextWeaponMovile %  m_weaponKeyAsign.Length;
				m_changeWeapon(m_nextWeaponMovile);
			}
		}
	}
	
	protected void OnChangeWeapon()
	{
		//En la version de pc, usaremos las teclas KeyCode del array apra cambiar de arma.
		foreach(TWeapons weapon in  m_weaponKeyAsign)
		{
            //TODO 1: lanzamos el evento m_changeWeapon si Input.GetKeyDown(weapon.keyCode)


        }
    }
	
	protected ChangeWapon m_changeWeapon;
}
