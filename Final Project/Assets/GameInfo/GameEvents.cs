using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SoundEventArgs : EventArgs
{
    public AudioClip soundClip;
}

public class GameEvents
{
    public static event EventHandler gameOver;
    public static event EventHandler levelStart;
    public static event EventHandler levelFinish;

    public static void InvokeGameOver() {
        gameOver(null, EventArgs.Empty);
    }

    public static void InvokeLevelStart() {
        levelStart(null, EventArgs.Empty);
    }

    public static void InvokeLevelFinish() {
        levelFinish(null, EventArgs.Empty);
    }
}
