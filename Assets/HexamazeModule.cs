using System;
using System.Collections.Generic;
using System.IO;
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

    public KMSelectable[] Buttons;
    public Mesh PlaneMesh;
    public GameObject Playfield;
    public GameObject Pawn;
    public Material[] PawnMaterials;

    private Dictionary<Hex, Marking> markings = new Dictionary<Hex, Marking>();
    private Dictionary<Hex, bool?[]> walls = new Dictionary<Hex, bool?[]>();

    private Hex _submazeCenter;
    private int _submazeRotation;
    private Hex _pawnPos;
    private int _pawnColor;
    private bool _isSolved;

    private static int _moduleIdCounter = 1;
    private int _moduleId;

    void Start()
    {
        _moduleId = _moduleIdCounter++;
        _isSolved = false;

        for (int i = 0; i < Buttons.Length; i++)
        {
            var j = i;
            Buttons[i].OnInteract += delegate { PushButton(j); return false; };
        }

        markings[new Hex(0, 0)] = Marking.Hexagon;
        markings[new Hex(-8, 7)] = Marking.TriangleLeft;
        markings[new Hex(-5, 8)] = Marking.Hexagon;
        markings[new Hex(5, -5)] = Marking.Circle;
        markings[new Hex(-3, -3)] = Marking.TriangleDown;
        markings[new Hex(5, 3)] = Marking.TriangleRight;
        markings[new Hex(-6, 1)] = Marking.Circle;
        markings[new Hex(2, -7)] = Marking.TriangleUp;
        markings[new Hex(-6, 4)] = Marking.Circle;
        markings[new Hex(-9, 4)] = Marking.Hexagon;
        markings[new Hex(-1, 5)] = Marking.TriangleRight;
        markings[new Hex(6, -8)] = Marking.TriangleLeft;
        markings[new Hex(5, -1)] = Marking.TriangleDown;
        markings[new Hex(2, 4)] = Marking.Circle;
        markings[new Hex(3, -2)] = Marking.Hexagon;
        markings[new Hex(-2, 8)] = Marking.Circle;
        markings[new Hex(8, -4)] = Marking.Hexagon;
        markings[new Hex(-3, 1)] = Marking.TriangleUp;
        markings[new Hex(-1, -6)] = Marking.TriangleRight;

        walls[new Hex(0, 0)] = new bool?[] { true, false, true };
        walls[new Hex(0, -1)] = new bool?[] { false, true, false };
        walls[new Hex(1, -1)] = new bool?[] { true, false, true };
        walls[new Hex(2, -1)] = new bool?[] { false, false, false };
        walls[new Hex(3, -1)] = new bool?[] { false, true, true };
        walls[new Hex(4, -1)] = new bool?[] { false, true, true };
        walls[new Hex(5, -1)] = new bool?[] { false, false, true };
        walls[new Hex(6, -2)] = new bool?[] { false, true, true };
        walls[new Hex(8, -3)] = new bool?[] { false, false, false };
        walls[new Hex(9, -3)] = new bool?[] { false, true, true };
        walls[new Hex(8, -2)] = new bool?[] { false, true, false };
        walls[new Hex(7, -1)] = new bool?[] { true, false, true };
        walls[new Hex(6, 0)] = new bool?[] { false, true, false };
        walls[new Hex(5, 0)] = new bool?[] { false, true, false };
        walls[new Hex(5, 1)] = new bool?[] { false, false, true };
        walls[new Hex(6, 1)] = new bool?[] { true, true, false };
        walls[new Hex(8, 0)] = new bool?[] { false, false, true };
        walls[new Hex(8, -1)] = new bool?[] { true, false, false };
        walls[new Hex(9, -2)] = new bool?[] { true, true, false };
        walls[new Hex(10, -3)] = new bool?[] { true, true, false };
        walls[new Hex(11, -3)] = new bool?[] { true, true, true };
        walls[new Hex(10, -2)] = new bool?[] { false, true, false };
        walls[new Hex(11, -2)] = new bool?[] { true, true, true };
        walls[new Hex(10, -1)] = new bool?[] { true, false, false };
        walls[new Hex(11, -1)] = new bool?[] { true, true, true };
        walls[new Hex(10, 0)] = new bool?[] { true, true, false };
        walls[new Hex(10, 1)] = new bool?[] { true, false, false };
        walls[new Hex(9, 2)] = new bool?[] { true, true, true };
        walls[new Hex(8, 3)] = new bool?[] { true, false, false };
        walls[new Hex(8, 2)] = new bool?[] { false, true, true };
        walls[new Hex(7, 3)] = new bool?[] { true, false, true };
        walls[new Hex(6, 4)] = new bool?[] { true, false, true };
        walls[new Hex(7, 4)] = new bool?[] { false, true, true };
        walls[new Hex(6, 5)] = new bool?[] { true, true, false };
        walls[new Hex(5, 6)] = new bool?[] { false, true, false };
        walls[new Hex(4, 7)] = new bool?[] { true, false, true };
        walls[new Hex(3, 8)] = new bool?[] { false, true, true };
        walls[new Hex(1, 9)] = new bool?[] { true, true, false };
        walls[new Hex(1, 10)] = new bool?[] { true, true, false };
        walls[new Hex(0, 10)] = new bool?[] { true, false, true };
        walls[new Hex(0, 9)] = new bool?[] { true, false, false };
        walls[new Hex(1, 8)] = new bool?[] { true, true, false };
        walls[new Hex(2, 7)] = new bool?[] { true, false, true };
        walls[new Hex(2, 6)] = new bool?[] { true, false, true };
        walls[new Hex(1, 6)] = new bool?[] { false, false, true };
        walls[new Hex(-1, 7)] = new bool?[] { true, false, true };
        walls[new Hex(-2, 8)] = new bool?[] { true, false, true };
        walls[new Hex(-2, 9)] = new bool?[] { true, false, true };
        walls[new Hex(-3, 10)] = new bool?[] { true, true, false };
        walls[new Hex(-3, 11)] = new bool?[] { true, false, true };
        walls[new Hex(-2, 11)] = new bool?[] { false, true, true };
        walls[new Hex(-2, 10)] = new bool?[] { true, true, false };
        walls[new Hex(-1, 9)] = new bool?[] { true, false, true };
        walls[new Hex(-1, 8)] = new bool?[] { false, true, false };
        walls[new Hex(0, 8)] = new bool?[] { true, false, true };
        walls[new Hex(-1, 10)] = new bool?[] { true, false, true };
        walls[new Hex(-1, 11)] = new bool?[] { true, false, true };
        walls[new Hex(0, 11)] = new bool?[] { false, true, true };
        walls[new Hex(-4, 11)] = new bool?[] { true, false, true };
        walls[new Hex(-5, 11)] = new bool?[] { false, true, false };
        walls[new Hex(-6, 11)] = new bool?[] { false, true, true };
        walls[new Hex(-7, 11)] = new bool?[] { false, true, true };
        walls[new Hex(-8, 11)] = new bool?[] { false, false, true };
        walls[new Hex(-8, 10)] = new bool?[] { true, true, false };
        walls[new Hex(-7, 9)] = new bool?[] { false, true, false };
        walls[new Hex(-8, 9)] = new bool?[] { false, false, true };
        walls[new Hex(-8, 8)] = new bool?[] { false, false, true };
        walls[new Hex(-9, 8)] = new bool?[] { true, true, true };
        walls[new Hex(-10, 8)] = new bool?[] { true, true, false };
        walls[new Hex(-9, 7)] = new bool?[] { true, true, false };
        walls[new Hex(-8, 6)] = new bool?[] { false, true, false };
        walls[new Hex(-8, 7)] = new bool?[] { true, false, false };
        walls[new Hex(-6, 6)] = new bool?[] { true, true, false };
        walls[new Hex(-7, 7)] = new bool?[] { true, true, false };
        walls[new Hex(-7, 8)] = new bool?[] { true, false, false };
        walls[new Hex(-6, 7)] = new bool?[] { true, true, false };
        walls[new Hex(-4, 6)] = new bool?[] { false, true, false };
        walls[new Hex(-4, 7)] = new bool?[] { false, false, true };
        walls[new Hex(-6, 8)] = new bool?[] { true, true, false };
        walls[new Hex(-5, 8)] = new bool?[] { true, false, true };
        walls[new Hex(-5, 9)] = new bool?[] { true, false, true };
        walls[new Hex(-5, 10)] = new bool?[] { true, false, false };
        walls[new Hex(-6, 10)] = new bool?[] { true, false, true };
        walls[new Hex(-7, 10)] = new bool?[] { true, true, false };
        walls[new Hex(-3, 9)] = new bool?[] { true, false, true };
        walls[new Hex(-3, 8)] = new bool?[] { true, true, false };
        walls[new Hex(-2, 7)] = new bool?[] { true, true, false };
        walls[new Hex(-1, 6)] = new bool?[] { false, true, true };
        walls[new Hex(-3, 7)] = new bool?[] { true, false, false };
        walls[new Hex(-4, 8)] = new bool?[] { true, false, false };
        walls[new Hex(-3, 6)] = new bool?[] { true, false, true };
        walls[new Hex(-3, 5)] = new bool?[] { true, true, false };
        walls[new Hex(-2, 4)] = new bool?[] { true, true, false };
        walls[new Hex(0, 3)] = new bool?[] { false, false, true };
        walls[new Hex(0, 4)] = new bool?[] { true, false, true };
        walls[new Hex(1, 4)] = new bool?[] { true, false, true };
        walls[new Hex(1, 5)] = new bool?[] { false, false, false };
        walls[new Hex(2, 4)] = new bool?[] { true, false, true };
        walls[new Hex(2, 3)] = new bool?[] { true, false, false };
        walls[new Hex(4, 2)] = new bool?[] { false, false, true };
        walls[new Hex(5, 2)] = new bool?[] { true, true, false };
        walls[new Hex(4, 3)] = new bool?[] { true, true, false };
        walls[new Hex(5, 3)] = new bool?[] { false, true, false };
        walls[new Hex(6, 3)] = new bool?[] { true, false, true };
        walls[new Hex(5, 4)] = new bool?[] { false, true, true };
        walls[new Hex(4, 5)] = new bool?[] { false, false, true };
        walls[new Hex(3, 5)] = new bool?[] { false, true, true };
        walls[new Hex(2, 5)] = new bool?[] { false, true, false };
        walls[new Hex(3, 4)] = new bool?[] { true, false, false };
        walls[new Hex(3, 6)] = new bool?[] { true, false, true };
        walls[new Hex(3, 7)] = new bool?[] { true, false, true };
        walls[new Hex(5, 5)] = new bool?[] { false, true, true };
        walls[new Hex(6, 2)] = new bool?[] { true, true, false };
        walls[new Hex(8, 1)] = new bool?[] { false, true, true };
        walls[new Hex(9, 1)] = new bool?[] { false, false, false };
        walls[new Hex(9, 0)] = new bool?[] { true, false, true };
        walls[new Hex(4, 1)] = new bool?[] { false, false, true };
        walls[new Hex(4, 0)] = new bool?[] { false, true, true };
        walls[new Hex(2, 1)] = new bool?[] { false, true, true };
        walls[new Hex(3, 1)] = new bool?[] { false, true, true };
        walls[new Hex(2, 2)] = new bool?[] { false, true, false };
        walls[new Hex(1, 2)] = new bool?[] { true, false, true };
        walls[new Hex(1, 1)] = new bool?[] { false, true, true };
        walls[new Hex(0, 1)] = new bool?[] { true, false, false };
        walls[new Hex(2, 0)] = new bool?[] { false, true, false };
        walls[new Hex(0, 2)] = new bool?[] { false, false, true };
        walls[new Hex(-2, 3)] = new bool?[] { true, true, false };
        walls[new Hex(-3, 4)] = new bool?[] { true, true, false };
        walls[new Hex(-4, 5)] = new bool?[] { false, true, false };
        walls[new Hex(-5, 5)] = new bool?[] { true, false, false };
        walls[new Hex(-4, 4)] = new bool?[] { true, true, false };
        walls[new Hex(-3, 3)] = new bool?[] { true, true, false };
        walls[new Hex(-4, 3)] = new bool?[] { true, false, false };
        walls[new Hex(-5, 3)] = new bool?[] { true, false, false };
        walls[new Hex(-5, 2)] = new bool?[] { true, false, true };
        walls[new Hex(-6, 2)] = new bool?[] { false, false, true };
        walls[new Hex(-6, 3)] = new bool?[] { false, false, true };
        walls[new Hex(-7, 4)] = new bool?[] { true, true, false };
        walls[new Hex(-6, 4)] = new bool?[] { false, false, false };
        walls[new Hex(-7, 5)] = new bool?[] { false, true, false };
        walls[new Hex(-8, 5)] = new bool?[] { false, false, true };
        walls[new Hex(-10, 6)] = new bool?[] { true, false, false };
        walls[new Hex(-11, 6)] = new bool?[] { true, false, true };
        walls[new Hex(-11, 7)] = new bool?[] { false, false, true };
        walls[new Hex(-11, 8)] = new bool?[] { true, false, false };
        walls[new Hex(-11, 9)] = new bool?[] { true, false, true };
        walls[new Hex(-11, 10)] = new bool?[] { true, false, true };
        walls[new Hex(-11, 11)] = new bool?[] { true, true, false };
        walls[new Hex(-9, 10)] = new bool?[] { false, false, true };
        walls[new Hex(-9, 9)] = new bool?[] { false, true, true };
        walls[new Hex(-10, 11)] = new bool?[] { true, true, false };
        walls[new Hex(-9, 11)] = new bool?[] { true, true, true };
        walls[new Hex(-11, 5)] = new bool?[] { true, true, false };
        walls[new Hex(-10, 4)] = new bool?[] { true, true, false };
        walls[new Hex(-11, 4)] = new bool?[] { true, true, false };
        walls[new Hex(-10, 3)] = new bool?[] { true, true, false };
        walls[new Hex(-10, 2)] = new bool?[] { true, false, false };
        walls[new Hex(-11, 2)] = new bool?[] { true, true, true };
        walls[new Hex(-10, 1)] = new bool?[] { true, false, true };
        walls[new Hex(-10, 0)] = new bool?[] { true, false, true };
        walls[new Hex(-10, -1)] = new bool?[] { true, true, true };
        walls[new Hex(-9, -2)] = new bool?[] { true, true, false };
        walls[new Hex(-7, -3)] = new bool?[] { false, true, true };
        walls[new Hex(-6, -3)] = new bool?[] { false, true, true };
        walls[new Hex(-5, -3)] = new bool?[] { false, false, false };
        walls[new Hex(-4, -4)] = new bool?[] { true, true, true };
        walls[new Hex(-3, -4)] = new bool?[] { false, false, false };
        walls[new Hex(-4, -3)] = new bool?[] { true, true, false };
        walls[new Hex(-5, -2)] = new bool?[] { true, true, false };
        walls[new Hex(-5, -1)] = new bool?[] { false, false, false };
        walls[new Hex(-4, -2)] = new bool?[] { true, true, false };
        walls[new Hex(-2, -3)] = new bool?[] { false, false, true };
        walls[new Hex(-1, -3)] = new bool?[] { false, false, true };
        walls[new Hex(-1, -2)] = new bool?[] { false, false, true };
        walls[new Hex(-2, -2)] = new bool?[] { false, true, true };
        walls[new Hex(-4, -1)] = new bool?[] { true, true, false };
        walls[new Hex(-3, -1)] = new bool?[] { false, true, true };
        walls[new Hex(-2, -1)] = new bool?[] { false, true, true };
        walls[new Hex(-1, -1)] = new bool?[] { false, false, false };
        walls[new Hex(-1, 0)] = new bool?[] { false, false, true };
        walls[new Hex(-3, 1)] = new bool?[] { false, true, true };
        walls[new Hex(-2, 1)] = new bool?[] { false, true, true };
        walls[new Hex(-1, 1)] = new bool?[] { false, false, true };
        walls[new Hex(-2, 2)] = new bool?[] { true, true, false };
        walls[new Hex(-4, 1)] = new bool?[] { true, false, true };
        walls[new Hex(-4, 0)] = new bool?[] { false, true, true };
        walls[new Hex(-6, 1)] = new bool?[] { false, true, false };
        walls[new Hex(-7, 1)] = new bool?[] { true, true, false };
        walls[new Hex(-6, 0)] = new bool?[] { false, true, false };
        walls[new Hex(-7, 0)] = new bool?[] { false, true, true };
        walls[new Hex(-7, -1)] = new bool?[] { false, true, true };
        walls[new Hex(-8, -1)] = new bool?[] { false, true, true };
        walls[new Hex(-9, -1)] = new bool?[] { true, true, true };
        walls[new Hex(-7, -2)] = new bool?[] { false, true, true };
        walls[new Hex(-6, -2)] = new bool?[] { false, true, false };
        walls[new Hex(-6, -1)] = new bool?[] { false, false, true };
        walls[new Hex(-9, 0)] = new bool?[] { true, true, true };
        walls[new Hex(-8, 0)] = new bool?[] { false, true, true };
        walls[new Hex(-9, 1)] = new bool?[] { true, true, false };
        walls[new Hex(-9, 2)] = new bool?[] { true, false, true };
        walls[new Hex(-9, 3)] = new bool?[] { true, false, true };
        walls[new Hex(-8, 3)] = new bool?[] { false, true, true };
        walls[new Hex(-8, 4)] = new bool?[] { false, false, true };
        walls[new Hex(-7, 3)] = new bool?[] { false, true, true };
        walls[new Hex(-7, 2)] = new bool?[] { false, true, true };
        walls[new Hex(-8, 2)] = new bool?[] { true, false, true };
        walls[new Hex(-3, 0)] = new bool?[] { false, true, true };
        walls[new Hex(0, -2)] = new bool?[] { true, false, false };
        walls[new Hex(1, -3)] = new bool?[] { true, false, true };
        walls[new Hex(2, -4)] = new bool?[] { false, true, true };
        walls[new Hex(3, -4)] = new bool?[] { false, true, true };
        walls[new Hex(2, -3)] = new bool?[] { false, true, true };
        walls[new Hex(3, -3)] = new bool?[] { false, true, true };
        walls[new Hex(4, -3)] = new bool?[] { false, false, true };
        walls[new Hex(4, -2)] = new bool?[] { false, false, false };
        walls[new Hex(5, -3)] = new bool?[] { true, false, false };
        walls[new Hex(5, -4)] = new bool?[] { true, false, false };
        walls[new Hex(6, -4)] = new bool?[] { true, true, false };
        walls[new Hex(7, -4)] = new bool?[] { true, true, false };
        walls[new Hex(6, -3)] = new bool?[] { true, true, false };
        walls[new Hex(8, -4)] = new bool?[] { false, true, false };
        walls[new Hex(9, -5)] = new bool?[] { true, true, false };
        walls[new Hex(10, -6)] = new bool?[] { true, true, false };
        walls[new Hex(11, -7)] = new bool?[] { true, false, true };
        walls[new Hex(11, -8)] = new bool?[] { false, true, false };
        walls[new Hex(10, -7)] = new bool?[] { true, true, true };
        walls[new Hex(9, -6)] = new bool?[] { false, true, false };
        walls[new Hex(8, -5)] = new bool?[] { true, false, true };
        walls[new Hex(8, -6)] = new bool?[] { true, true, false };
        walls[new Hex(9, -7)] = new bool?[] { false, true, true };
        walls[new Hex(7, -6)] = new bool?[] { true, false, false };
        walls[new Hex(7, -7)] = new bool?[] { true, false, false };
        walls[new Hex(8, -8)] = new bool?[] { true, true, true };
        walls[new Hex(7, -8)] = new bool?[] { false, false, true };
        walls[new Hex(8, -9)] = new bool?[] { false, true, true };
        walls[new Hex(9, -9)] = new bool?[] { false, true, true };
        walls[new Hex(10, -9)] = new bool?[] { false, true, false };
        walls[new Hex(11, -9)] = new bool?[] { true, true, true };
        walls[new Hex(11, -10)] = new bool?[] { false, true, true };
        walls[new Hex(10, -10)] = new bool?[] { false, true, true };
        walls[new Hex(9, -10)] = new bool?[] { false, false, true };
        walls[new Hex(8, -10)] = new bool?[] { false, false, true };
        walls[new Hex(8, -11)] = new bool?[] { true, true, true };
        walls[new Hex(6, -10)] = new bool?[] { false, true, false };
        walls[new Hex(5, -10)] = new bool?[] { true, true, false };
        walls[new Hex(6, -11)] = new bool?[] { false, true, true };
        walls[new Hex(5, -11)] = new bool?[] { false, true, true };
        walls[new Hex(4, -11)] = new bool?[] { false, true, true };
        walls[new Hex(3, -11)] = new bool?[] { false, true, false };
        walls[new Hex(2, -10)] = new bool?[] { true, false, true };
        walls[new Hex(3, -10)] = new bool?[] { true, true, true };
        walls[new Hex(3, -9)] = new bool?[] { true, false, false };
        walls[new Hex(4, -9)] = new bool?[] { true, false, false };
        walls[new Hex(3, -8)] = new bool?[] { true, true, false };
        walls[new Hex(2, -7)] = new bool?[] { true, true, false };
        walls[new Hex(3, -7)] = new bool?[] { false, true, false };
        walls[new Hex(5, -8)] = new bool?[] { false, false, true };
        walls[new Hex(4, -7)] = new bool?[] { true, true, true };
        walls[new Hex(3, -6)] = new bool?[] { true, false, false };
        walls[new Hex(3, -5)] = new bool?[] { false, false, false };
        walls[new Hex(4, -5)] = new bool?[] { false, false, false };
        walls[new Hex(5, -6)] = new bool?[] { true, false, false };
        walls[new Hex(6, -7)] = new bool?[] { true, false, true };
        walls[new Hex(6, -8)] = new bool?[] { false, false, true };
        walls[new Hex(6, -9)] = new bool?[] { true, true, false };
        walls[new Hex(5, -7)] = new bool?[] { false, true, true };
        walls[new Hex(6, -6)] = new bool?[] { true, false, true };
        walls[new Hex(5, -5)] = new bool?[] { true, true, false };
        walls[new Hex(4, -4)] = new bool?[] { false, false, false };
        walls[new Hex(2, -5)] = new bool?[] { false, false, true };
        walls[new Hex(1, -5)] = new bool?[] { false, true, true };
        walls[new Hex(-1, -4)] = new bool?[] { true, false, true };
        walls[new Hex(-1, -5)] = new bool?[] { false, true, true };
        walls[new Hex(-2, -5)] = new bool?[] { true, false, true };
        walls[new Hex(-1, -6)] = new bool?[] { false, false, false };
        walls[new Hex(0, -7)] = new bool?[] { true, true, false };
        walls[new Hex(-1, -7)] = new bool?[] { true, true, false };
        walls[new Hex(1, -8)] = new bool?[] { false, true, false };
        walls[new Hex(2, -9)] = new bool?[] { false, true, true };
        walls[new Hex(1, -9)] = new bool?[] { false, false, false };
        walls[new Hex(1, -10)] = new bool?[] { true, false, true };
        walls[new Hex(1, -11)] = new bool?[] { true, true, true };
        walls[new Hex(-1, -10)] = new bool?[] { true, true, false };
        walls[new Hex(-1, -9)] = new bool?[] { true, false, false };
        walls[new Hex(0, -9)] = new bool?[] { true, false, true };
        walls[new Hex(-1, -8)] = new bool?[] { true, true, false };
        walls[new Hex(-2, -7)] = new bool?[] { true, true, false };
        walls[new Hex(-3, -6)] = new bool?[] { false, true, false };
        walls[new Hex(-4, -5)] = new bool?[] { false, true, true };
        walls[new Hex(-5, -5)] = new bool?[] { true, false, true };
        walls[new Hex(-6, -5)] = new bool?[] { true, true, false };
        walls[new Hex(-7, -4)] = new bool?[] { true, true, false };
        walls[new Hex(-6, -4)] = new bool?[] { false, true, true };
        walls[new Hex(-5, -6)] = new bool?[] { true, true, false };
        walls[new Hex(-4, -7)] = new bool?[] { false, true, false };
        walls[new Hex(-3, -8)] = new bool?[] { true, true, false };
        walls[new Hex(-2, -8)] = new bool?[] { true, false, false };
        walls[new Hex(-3, -7)] = new bool?[] { true, true, false };
        walls[new Hex(-4, -6)] = new bool?[] { true, true, false };
        walls[new Hex(-3, -5)] = new bool?[] { false, false, true };
        walls[new Hex(2, -8)] = new bool?[] { true, false, false };
        walls[new Hex(1, -7)] = new bool?[] { true, true, false };
        walls[new Hex(1, -6)] = new bool?[] { false, false, false };
        walls[new Hex(2, -6)] = new bool?[] { false, true, true };
        walls[new Hex(-2, -4)] = new bool?[] { true, false, true };
        walls[new Hex(0, -4)] = new bool?[] { true, false, true };
        walls[new Hex(0, -3)] = new bool?[] { true, false, true };
        walls[new Hex(10, -11)] = new bool?[] { false, true, true };
        walls[new Hex(11, -11)] = new bool?[] { false, false, true };
        walls[new Hex(9, -8)] = new bool?[] { true, true, false };
        walls[new Hex(11, -6)] = new bool?[] { true, true, true };
        walls[new Hex(10, -5)] = new bool?[] { true, true, false };
        walls[new Hex(9, -4)] = new bool?[] { true, true, false };
        walls[new Hex(10, -4)] = new bool?[] { false, true, false };
        walls[new Hex(-11, 0)] = new bool?[] { false, true, false };
        walls[new Hex(-11, 1)] = new bool?[] { true, true, false };
        walls[new Hex(-11, 3)] = new bool?[] { true, false, false };
        walls[new Hex(-6, 5)] = new bool?[] { true, true, false };
        walls[new Hex(1, 3)] = new bool?[] { true, false, true };
        walls[new Hex(0, 5)] = new bool?[] { false, false, true };
        walls[new Hex(-1, 5)] = new bool?[] { false, true, true };
        walls[new Hex(-2, 5)] = new bool?[] { false, true, false };
        walls[new Hex(-8, 1)] = new bool?[] { true, false, true };
        walls[new Hex(-8, -2)] = new bool?[] { true, true, true };
        walls[new Hex(-9, 4)] = new bool?[] { true, true, true };
        walls[new Hex(-5, 0)] = new bool?[] { true, true, false };
        walls[new Hex(-5, 1)] = new bool?[] { true, false, true };
        walls[new Hex(-9, 5)] = new bool?[] { true, true, true };
        walls[new Hex(-10, 5)] = new bool?[] { true, false, true };
        walls[new Hex(-9, 6)] = new bool?[] { true, true, true };
        walls[new Hex(-7, 6)] = new bool?[] { true, true, false };
        walls[new Hex(-10, 7)] = new bool?[] { true, true, false };
        walls[new Hex(-5, 4)] = new bool?[] { true, true, false };
        walls[new Hex(-12, 7)] = new bool?[] { true, true, true };
        walls[new Hex(-12, 6)] = new bool?[] { true, true, true };
        walls[new Hex(-12, 8)] = new bool?[] { true, true, true };
        walls[new Hex(-12, 9)] = new bool?[] { true, true, true };
        walls[new Hex(-10, 9)] = new bool?[] { true, true, true };
        walls[new Hex(-5, 6)] = new bool?[] { true, true, false };
        walls[new Hex(-5, 7)] = new bool?[] { false, true, true };
        walls[new Hex(-6, 9)] = new bool?[] { true, false, true };
        walls[new Hex(-8, 12)] = new bool?[] { true, false, true };
        walls[new Hex(-9, 12)] = new bool?[] { true, true, true };
        walls[new Hex(-4, 2)] = new bool?[] { true, false, true };
        walls[new Hex(-3, 2)] = new bool?[] { true, true, false };
        walls[new Hex(-4, 9)] = new bool?[] { true, true, false };
        walls[new Hex(-2, 0)] = new bool?[] { false, true, true };
        walls[new Hex(-4, 10)] = new bool?[] { true, false, true };
        walls[new Hex(-5, -4)] = new bool?[] { false, true, true };
        walls[new Hex(-8, -3)] = new bool?[] { true, false, true };
        walls[new Hex(-3, -3)] = new bool?[] { true, true, true };
        walls[new Hex(-3, -2)] = new bool?[] { true, false, true };
        walls[new Hex(-1, 3)] = new bool?[] { false, true, true };
        walls[new Hex(-1, 2)] = new bool?[] { true, false, true };
        walls[new Hex(0, 6)] = new bool?[] { false, true, true };
        walls[new Hex(-2, 6)] = new bool?[] { true, true, false };
        walls[new Hex(-1, 4)] = new bool?[] { true, true, false };
        walls[new Hex(-2, -6)] = new bool?[] { true, true, true };
        walls[new Hex(0, -8)] = new bool?[] { true, true, true };
        walls[new Hex(1, -2)] = new bool?[] { true, true, false };
        walls[new Hex(0, -6)] = new bool?[] { false, true, true };
        walls[new Hex(0, -5)] = new bool?[] { false, true, true };
        walls[new Hex(1, -4)] = new bool?[] { true, false, true };
        walls[new Hex(1, 0)] = new bool?[] { true, false, true };
        walls[new Hex(0, 7)] = new bool?[] { true, false, true };
        walls[new Hex(1, 7)] = new bool?[] { true, false, true };
        walls[new Hex(-3, 12)] = new bool?[] { true, true, true };
        walls[new Hex(-4, 12)] = new bool?[] { true, true, false };
        walls[new Hex(-5, 12)] = new bool?[] { true, true, true };
        walls[new Hex(-2, 12)] = new bool?[] { true, true, true };
        walls[new Hex(-6, 12)] = new bool?[] { true, false, true };
        walls[new Hex(3, 0)] = new bool?[] { false, true, true };
        walls[new Hex(3, 2)] = new bool?[] { true, false, true };
        walls[new Hex(3, 3)] = new bool?[] { true, false, true };
        walls[new Hex(2, 8)] = new bool?[] { false, true, true };
        walls[new Hex(-1, 12)] = new bool?[] { true, true, true };
        walls[new Hex(2, -11)] = new bool?[] { true, true, true };
        walls[new Hex(0, -10)] = new bool?[] { true, true, true };
        walls[new Hex(0, -11)] = new bool?[] { false, true, true };
        walls[new Hex(-2, -9)] = new bool?[] { true, true, true };
        walls[new Hex(4, -6)] = new bool?[] { true, true, true };
        walls[new Hex(2, -2)] = new bool?[] { true, false, true };
        walls[new Hex(3, -2)] = new bool?[] { true, false, true };
        walls[new Hex(1, 11)] = new bool?[] { true, true, true };
        walls[new Hex(0, 12)] = new bool?[] { true, true, true };
        walls[new Hex(2, 10)] = new bool?[] { true, true, true };
        walls[new Hex(2, 9)] = new bool?[] { true, false, true };
        walls[new Hex(3, 9)] = new bool?[] { false, true, true };
        walls[new Hex(4, 8)] = new bool?[] { true, true, true };
        walls[new Hex(4, -8)] = new bool?[] { true, true, true };
        walls[new Hex(5, -9)] = new bool?[] { true, true, false };
        walls[new Hex(4, -10)] = new bool?[] { true, true, true };
        walls[new Hex(5, -2)] = new bool?[] { false, true, false };
        walls[new Hex(6, -1)] = new bool?[] { false, true, true };
        walls[new Hex(4, 4)] = new bool?[] { true, false, true };
        walls[new Hex(4, 6)] = new bool?[] { false, true, true };
        walls[new Hex(7, -9)] = new bool?[] { true, true, true };
        walls[new Hex(6, -5)] = new bool?[] { true, true, false };
        walls[new Hex(7, -5)] = new bool?[] { true, true, false };
        walls[new Hex(7, -2)] = new bool?[] { false, true, true };
        walls[new Hex(7, 2)] = new bool?[] { true, false, true };
        walls[new Hex(7, -10)] = new bool?[] { true, true, true };
        walls[new Hex(7, -3)] = new bool?[] { false, true, true };
        walls[new Hex(7, 1)] = new bool?[] { true, false, true };
        walls[new Hex(7, 0)] = new bool?[] { true, false, true };
        walls[new Hex(9, -1)] = new bool?[] { true, false, true };
        walls[new Hex(8, -7)] = new bool?[] { true, true, false };
        walls[new Hex(10, -8)] = new bool?[] { false, true, false };
        walls[new Hex(12, -10)] = new bool?[] { true, true, true };
        walls[new Hex(12, -9)] = new bool?[] { true, true, true };
        walls[new Hex(9, -11)] = new bool?[] { true, true, true };
        walls[new Hex(12, -8)] = new bool?[] { true, true, true };
        walls[new Hex(12, -11)] = new bool?[] { true, true, true };
        walls[new Hex(12, -7)] = new bool?[] { true, true, true };
        walls[new Hex(12, -6)] = new bool?[] { true, true, true };
        walls[new Hex(11, -5)] = new bool?[] { true, true, true };
        walls[new Hex(11, -4)] = new bool?[] { true, true, false };
        walls[new Hex(12, -5)] = new bool?[] { true, true, true };
        walls[new Hex(12, -4)] = new bool?[] { true, true, true };
        walls[new Hex(12, -3)] = new bool?[] { false, true, true };
        walls[new Hex(12, -2)] = new bool?[] { true, true, true };
        walls[new Hex(12, -1)] = new bool?[] { true, true, true };
        walls[new Hex(11, 0)] = new bool?[] { true, true, true };
        walls[new Hex(-12, 5)] = new bool?[] { true, true, false };
        walls[new Hex(-12, 10)] = new bool?[] { true, true, true };
        walls[new Hex(-7, 12)] = new bool?[] { true, true, true };
        walls[new Hex(5, 7)] = new bool?[] { true, false, true };
        walls[new Hex(8, 4)] = new bool?[] { true, true, true };
        walls[new Hex(7, 5)] = new bool?[] { true, true, true };
        walls[new Hex(6, 6)] = new bool?[] { true, true, true };
        walls[new Hex(9, 3)] = new bool?[] { true, true, true };
        walls[new Hex(10, 2)] = new bool?[] { false, true, true };
        walls[new Hex(11, 1)] = new bool?[] { true, true, true };
        walls[new Hex(-12, 3)] = new bool?[] { true, true, true };
        walls[new Hex(-12, 4)] = new bool?[] { true, true, true };
        walls[new Hex(-12, 1)] = new bool?[] { true, true, true };
        walls[new Hex(-12, 2)] = new bool?[] { true, true, true };
        walls[new Hex(-10, 10)] = new bool?[] { true, true, true };
        walls[new Hex(-10, 12)] = new bool?[] { true, true, true };
        walls[new Hex(-11, 12)] = new bool?[] { true, true, true };
        walls[new Hex(-12, 12)] = new bool?[] { true, true, true };
        walls[new Hex(-12, 11)] = new bool?[] { true, true, false };
        walls[new Hex(7, -11)] = new bool?[] { true, false, true };
        walls[new Hex(12, 0)] = new bool?[] { true, true, true };

        _submazeCenter = Hex.LargeHexagon(9).PickRandom();
        _submazeRotation = Rnd.Range(0, 6);
        _pawnColor = Rnd.Range(0, 6);
        Pawn.GetComponent<MeshRenderer>().material = PawnMaterials[_pawnColor];

        // Find a starting position for the pawn that is at least 4 steps and one bend away
        var dic = new Dictionary<Hex, QueueItem>();
        var queue = new Queue<QueueItem>();
        foreach (var goal in Enumerable.Range(0, 4).Select(x => new Hex(x - 3, -x).Rotate(_pawnColor) + _submazeCenter))
        {
            var directions = new List<int>();
            if (hasWall(goal, _pawnColor) == false)
                directions.Add(_pawnColor);
            if (hasWall(goal, (_pawnColor + 1) % 6) == false)
                directions.Add((_pawnColor + 1) % 6);
            if (directions.Count > 0)
            {
                var qi = new QueueItem { Hex = goal, Directions = directions.ToArray(), Distance = 1 };
                queue.Enqueue(qi);
            }
        }
        while (queue.Count > 0)
        {
            var item = queue.Dequeue();
            var already = dic.Get(item.Hex, null);
            if (already != null && ((already.Directions != null && item.Directions == null) || ((already.Directions == null) == (item.Directions == null) && already.Distance <= item.Distance)))
                continue;
            dic[item.Hex] = item;
            var neigh = item.Hex.Neighbors;
            for (int i = 0; i < 6; i++)
                if (hasWall(item.Hex, i) == false && (neigh[i] - _submazeCenter).Distance < 4)
                    queue.Enqueue(new QueueItem
                    {
                        Hex = neigh[i],
                        Directions = item.Directions != null && item.Directions.Contains((i + 3) % 6) ? new[] { (i + 3) % 6 } : null,
                        Distance = item.Distance + 1,
                        Parent = item
                    });
        }

        var pool = dic.Values.Where(inf => inf.Distance >= 4 && inf.Directions == null && !markings.ContainsKey(inf.Hex));
        //File.WriteAllLines(@"D:\temp\temp.txt", dic.Values.Select(v => v.ToString()).ToArray());
        //File.AppendAllText(@"D:\temp\temp.txt", "Available starting locations: " + string.Join(", ", pool.Select(qi => qi.Hex.ToString()).ToArray()));
        placePawn((pool.PickRandom().Hex - _submazeCenter).Rotate(_submazeRotation));

        Debug.LogFormat("[Hexamaze #{4}] Submaze center: {0}, submaze rotation: {1}, pawn: {2} (global), pawn color: {3}.", _submazeCenter.ConvertCoordinates(12), _submazeRotation, _pawnPos.ConvertCoordinates(12), "red|yellow|green|cyan|blue|pink".Split('|')[_pawnColor], _moduleId);

        foreach (var hex in Hex.LargeHexagon(4))
        {
            var globalHex = hex.Rotate(-_submazeRotation) + _submazeCenter;
            var origMarking = markings.Get(globalHex, Marking.None);
            var rotMarking = origMarking.Rotate(_submazeRotation);
            if (rotMarking != Marking.None)
                Debug.LogFormat("[Hexamaze #{4}] Marking at {0} (screen)/{1} (maze): {2}, after rotation: {3}", hex.ConvertCoordinates(4), globalHex.ConvertCoordinates(12), origMarking, rotMarking, _moduleId);
            CreateGraphic("Marking " + hex, hex.GetCenter(1, 1e-4f), MarkingPngs.RawBytes[rotMarking]);
        }
    }

    sealed class QueueItem
    {
        public Hex Hex;
        public int[] Directions;
        public int Distance;
        public QueueItem Parent;
        public override string ToString()
        {
            return string.Format("{3}{0} dir={1} dist={2}", Hex, Directions == null ? "null" : string.Join("/", Directions.Select(d => d.ToString()).ToArray()), Distance, Parent == null ? null : Parent.ToString() + " → ");
        }
    }

    private void placePawn(Hex hex)
    {
        _pawnPos = hex;
        Pawn.transform.localPosition = hex.GetCenter(1, 0);
    }

    private void CreateGraphic(string name, Vector3 position, byte[] rawBytes, float scale = 0.1f, int rotation = 0)
    {
        var go = new GameObject { name = name };
        go.transform.parent = Playfield.transform;
        go.transform.localPosition = position;
        go.transform.localEulerAngles = new Vector3(0, 180 + 60 * rotation, 0);
        go.transform.localScale = new Vector3(scale, scale, scale);
        go.AddComponent<MeshFilter>().mesh = PlaneMesh;
        var mr = go.AddComponent<MeshRenderer>();
        var tex = new Texture2D(2, 2);
        tex.LoadImage(rawBytes);
        mr.material.mainTexture = tex;
        mr.material.shader = Shader.Find("Unlit/Transparent");
    }

    private void PushButton(int direction)
    {
        Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, Buttons[direction].transform);
        Buttons[direction].AddInteractionPunch(.1f);

        if (_isSolved)
            return;

        var globalPos = _pawnPos.Rotate(-_submazeRotation) + _submazeCenter;
        var newPawnPos = _pawnPos.Neighbors[direction];
        var newGlobalPos = globalPos.Neighbors[direction];
        Debug.LogFormat("[Hexamaze #{4}] Moving from {0} to {1} (screen) / from {2} to {3} (maze).",
            _pawnPos.ConvertCoordinates(4), newPawnPos.ConvertCoordinates(4),
            globalPos.ConvertCoordinates(12), newGlobalPos.ConvertCoordinates(12),
            _moduleId);
        var globalDirection = (direction - _submazeRotation + 6) % 6;

        // false = no wall, true = invisible wall, null = visible wall.
        var wallInfo = hasWall(globalPos, globalDirection);
        if (wallInfo != false)
        {
            Debug.LogFormat("[Hexamaze #{0}] There’s {1} wall there.", _moduleId, wallInfo == null ? "a visible" : "an invisible");
            Module.HandleStrike();
            if (wallInfo != null && newPawnPos.Distance < 4)
            {
                CreateGraphic(string.Format("Wall {0}/{1}", globalPos, globalDirection), _pawnPos.GetCenter(1, 1e-4f), MarkingPngs.LineRawBytes, scale: .15f, rotation: direction);
                setWallVisible(globalPos, globalDirection);
            }
        }
        else
        {
            if (newPawnPos.Distance >= 4)
            {
                var edges = newPawnPos.GetEdges(4).ToArray();
                var globalEdges = edges.Select(e => (e - _submazeRotation + 6) % 6).ToArray();
                Debug.LogFormat("[Hexamaze #{2}] Walking out of the submaze through edge(s) {0} (screen) / {1} (maze).{3}",
                    string.Join(", ", edges.Select(e => e.ToString()).ToArray()), string.Join(", ", globalEdges.Select(e => e.ToString()).ToArray()), _moduleId,
                    globalEdges.Contains(_pawnColor) ? " Solved!" : "");

                if (globalEdges.Contains(_pawnColor))
                {
                    _isSolved = true;
                    Pawn.gameObject.SetActive(false);
                    Module.HandlePass();
                }
                else
                {
                    Debug.LogFormat("[Hexamaze #{2}] However, we wanted edge {0} (screen) / {1} (maze). Strike.", (_pawnColor + _submazeRotation) % 6, _pawnColor, _moduleId);
                    Module.HandleStrike();
                }
            }
            else
            {
                placePawn(newPawnPos);
            }
        }
    }

    private static bool?[] _allFalse = new bool?[] { false, false, false };
    private bool? hasWall(Hex hex, int n) { return walls.Get(n < 3 ? hex : hex.Neighbors[n], _allFalse)[n % 3]; }
    private void setWallVisible(Hex hex, int n) { walls[n < 3 ? hex : hex.Neighbors[n]][n % 3] = null; }

    KMSelectable[] ProcessTwitchCommand(string command)
    {
        if (!command.StartsWith("move "))
            return null;

        return command.Substring("move ".Length).Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(move =>
        {
            switch (move)
            {
                case "nw": case "10": case "upleft": case "leftup": return Buttons[0];
                case "n": case "12": case "up": return Buttons[1];
                case "ne": case "2": case "upright": case "rightup": return Buttons[2];
                case "se": case "4": case "downright": case "rightdown": return Buttons[3];
                case "s": case "6": case "down": return Buttons[4];
                case "sw": case "8": case "downleft": case "leftdown": return Buttons[5];
            }
            return null;
        }).TakeWhile(b => b != null).ToArray();
    }
}
