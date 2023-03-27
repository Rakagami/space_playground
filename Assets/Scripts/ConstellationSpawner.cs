using System;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
    using UnityEditor;
    using UnityEditorInternal;
#endif
using UnityEngine;

public class ConstellationSpawner : MonoBehaviour
{
    [Serializable]
    public enum ConstellationType
    {
        WalkerDelta, WalkerStar
    }

    [Serializable]
    public class OrbitalShell
    {
        public float inclination;
        public int n_planes;
        public Color color;
        public ConstellationType ctype;
    }

    [Header("Constellation Settings")]
    public GameObject orbit_prefab;
    public OrbitalShell[] shells;
    public bool reset;

    private List<GameObject> all_objects;

    // Start is called before the first frame update
    void Start()
    {
        reset = false;
        all_objects = new List<GameObject>();
        foreach (OrbitalShell shell in shells)
        {
            float surrounddegrees;
            switch (shell.ctype) {
                case ConstellationType.WalkerDelta:
                    surrounddegrees = 360;
                    break;
                case ConstellationType.WalkerStar:
                    surrounddegrees = 180;
                    break;
                default:
                    surrounddegrees = 360;
                    break;
            }
            for (int i=0; i<shell.n_planes; i++)
            {
                GameObject go = Instantiate(orbit_prefab, new Vector3(0, 0, 0), Quaternion.Euler(shell.inclination, (surrounddegrees/shell.n_planes) * i, 0));
                all_objects.Add(go);
                Renderer rend = go.transform.GetChild(0).GetComponent<Renderer>();
                rend.material.color = shell.color;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(reset)
        {
            foreach(var obj in all_objects)
            {
                Destroy(obj);
            }
            all_objects.Clear();
            Start();
        }
    }
}
