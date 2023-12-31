﻿using BML.ScriptableObjectCore.Scripts.SceneReferences;
using BML.Scripts.SpaceGraph;
using UnityEngine;

namespace BML.Scripts.SpaceGraph
{
    [CreateAssetMenu(fileName = "SpaceNodeReference", menuName = "BML/SpaceGraph/SpaceNodeReference", order = 0)]
    public class SpaceNodeReference : SceneReference<SpaceNodeComponent> {}
}