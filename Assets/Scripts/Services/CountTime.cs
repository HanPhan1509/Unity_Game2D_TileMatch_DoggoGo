using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CountTime : MonoBehaviour
{
	private int limitTime = 0;
	private int loadTime = 0;
	private int countTime = 0;
	private Action action;
	private void Start()
	{
		//limitTime = 5 * 60;
		limitTime = 10;
		countTime = limitTime;
		StartCoroutine(CountTimeInGame());
	}

	public void SetTimeAndAction(int loadTime, Action action)
	{
		this.loadTime = loadTime;
		this.action = action;
	}

	IEnumerator CountTimeInGame()
	{
		while (true)
		{
			yield return new WaitForSeconds(1f);
			countTime += 1;
			if (loadTime <= countTime)
			{
				action?.Invoke();
				countTime = 0;
			}
		};
	}
}
