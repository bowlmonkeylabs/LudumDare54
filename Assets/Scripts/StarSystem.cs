using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StarSystemTrait {
    Populous,
    Fertile,
    Hostile
}

public class StarSystem : MonoBehaviour
{
    [SerializeField] private List<StarSystemTrait> _traits;
}
