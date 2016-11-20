using System;
using System.Collections.Generic;
using System.Linq;
using Hexamaze;
using UnityEngine;
using Rnd = UnityEngine.Random;

/// <summary>
/// On the Subject of Hexamazes
/// Created by Timwi
/// </summary>
public class HexamazeModule : MonoBehaviour
{
    public KMBombInfo Bomb;
    public KMBombModule Module;
    public KMAudio Audio;

    void Start()
    {
        Debug.Log("[Hexamaze] Started");
        Module.OnActivate += ActivateModule;
    }

    void ActivateModule()
    {
    }
}
