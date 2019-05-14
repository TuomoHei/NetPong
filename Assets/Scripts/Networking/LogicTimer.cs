using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using System;

public class LogicTimer
{

    public const float FRAMES_PER_SECOND = 30.0f;
    public const float FIXED_DELTA = 1.0f / FRAMES_PER_SECOND;
    public float fd { get { return FIXED_DELTA; } }

    private double accumulator;
    private long lastTime;

    private readonly Stopwatch stopwatch;
    private readonly Action action;

    public float LerpAlpha => (float)accumulator / FIXED_DELTA;

    public LogicTimer(Action action)
    {
        stopwatch = new Stopwatch();
        this.action = action;
    }

    public void Start()
    {
        lastTime = 0;
        accumulator = 0.0;
        stopwatch.Start();
    }

    public void Stop()
    {
        stopwatch.Stop();
    }

    public void Update()
    {
        long elapsedTicks = stopwatch.ElapsedTicks;
        accumulator += (double)(elapsedTicks - lastTime) / Stopwatch.Frequency;
        lastTime = elapsedTicks;

        while (accumulator >= FIXED_DELTA)
        {
            action();
            accumulator -= FIXED_DELTA;
        }
    }

}
