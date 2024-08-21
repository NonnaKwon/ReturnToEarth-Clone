using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dot : MonoBehaviour
{
    public DotCoordinator dotCoordinator;

    private void Start()
    {
        dotCoordinator = GetComponentInParent<DotCoordinator>();
    }
}