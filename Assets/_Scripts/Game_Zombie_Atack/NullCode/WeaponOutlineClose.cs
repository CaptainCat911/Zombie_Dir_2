// NULLcode Studio © 2015
// null-code.ru

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WeaponOutlineClose : MonoBehaviour
{


	public void OnTriggerEnter(Collider collision)
	{
		if (collision.tag == "Weapon")
		{
			Outline outline = collision.transform.GetComponent<Outline>();        // если есть подсветка
			if (outline != null)
			{

				outline.OutlineWidth = 2f;                              // даём ширину линии				
				
			}			
		}
	}
}
