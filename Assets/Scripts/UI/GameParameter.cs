using Game;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameParameter : MonoBehaviour
{
	public GameMode OnGameMode;
	public int LoadLevel;

	// Start is called before the first frame update
	void Start()
    {
		DontDestroyOnLoad(gameObject);
	}
}
