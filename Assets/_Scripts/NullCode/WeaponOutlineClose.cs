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

				outline.OutlineWidth = 4f;                              // даём ширину линии				
				
			}			
		}
	}

	public void OnTriggerExit(Collider collision)
	{
		if (collision.tag == "Weapon")
		{
			Outline outline = collision.transform.GetComponent<Outline>();        // если есть подсветка
			if (outline != null)
			{

				outline.OutlineWidth = 0f;                              // убираем ширину линии				

			}
		}
	}
}
