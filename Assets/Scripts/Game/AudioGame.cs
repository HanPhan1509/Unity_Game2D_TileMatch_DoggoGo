using Audio;
using Services;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Home
{
	public class AudioGame : MonoBehaviour
	{
		[SerializeField] private Sounds sounds;

		public void Initialized(AudioService audioService)
		{
			sounds.Initialized(audioService);
		}

		public void PlayUseBooster()
		{
			sounds.PlaySound("boost");
		}
		public void PlayButton1()
		{
			sounds.PlaySound("button1");
		}
		public void PlayButton2()
		{
			sounds.PlaySound("button2");
		}
		public void PlayCombo()
		{
			sounds.PlaySound("combo");
		}
		public void PlayLose()
		{
			sounds.PlaySound("lose");
		}
		public void PlayPop()
		{
			sounds.PlaySound("pop");
		}
		public void PlayWin()
		{
			sounds.PlaySound("win");
		}
	}
}
