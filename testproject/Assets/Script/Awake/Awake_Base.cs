using UnityEngine;
using System.Collections;

public abstract class Awake_Base : MonoBehaviour {

    public bool is_finish { get; protected set; }

    public abstract void Init();
    public abstract void Proccess_Start();
}
