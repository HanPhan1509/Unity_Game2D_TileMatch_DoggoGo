using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleController : MonoBehaviour
{
	[SerializeField] private Toggle toggle;
	[SerializeField] private Animator animator;
	private string currentAnim = "close";

	private void Start()
	{
		if(toggle.isOn)
		{
			currentAnim = "open";
		} else
		{
			currentAnim = "close";
		}
		animator.SetTrigger(currentAnim);
	}

	public void ToggleChange(Toggle isToggle)
	{
		if(isToggle.isOn)
		{
			SetAnim("open");
		} else
		{
			SetAnim("close");
		}
	}

	private void SetAnim(string anim)
	{
		if (anim != currentAnim)
		{
			animator.ResetTrigger(currentAnim);
			currentAnim = anim;
			animator.SetTrigger(currentAnim);
		}	
	}	
}
