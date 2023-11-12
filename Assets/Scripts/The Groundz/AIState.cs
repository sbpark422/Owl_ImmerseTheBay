using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface AIState	{ 
	
	void Start(GameManager manager, AI ai);

	void Update(GameManager manager, AI ai);

	void Action(GameManager manager, AI ai, float dur, Vector3 target);

    int GetNum();

    // state index 
    // idle 0, GetBall 1, ThrowBall 2, Ready 3, Panic 4, Retreat 5, Shake 6

    string GetName();

    void SetInAction(bool x);
}


