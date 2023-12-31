﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using BML.ScriptableObjectCore.Scripts.Variables.ValueReferences;
using BML.ScriptableObjectCore.Scripts.Variables.ValueTypeVariables;
using Sirenix.OdinInspector;
using UnityEngine;

namespace BML.ScriptableObjectCore.Scripts.Variables
{
    [Required]
    [CreateAssetMenu(fileName = "Vector3Variable", menuName = "BML/Variables/Vector3Variable", order = 0)]
    public class Vector3Variable : ValueTypeVariable<Vector3>, IFloatValue, IVector3Value
    {
#region Inspector

        [SerializeField] private bool UseComponentReferences = false;
        [SerializeField, ShowIf("UseComponentReferences")] private FloatValueReference X;
        [SerializeField, ShowIf("UseComponentReferences")] private FloatValueReference Y;
        [SerializeField, ShowIf("UseComponentReferences")] private FloatValueReference Z;
        
#endregion
        
        public string GetName() => name;
        public string GetDescription() => Description;
        public float GetFloat() => Value.magnitude;
        float IValue<float>.GetValue(Type type) => GetFloat();

        public Vector3 GetVector3()
        {
            if (!UseComponentReferences) return Value;

            return new Vector3(X.Value, Y.Value, Z.Value);
        }

        Vector3 IValue<Vector3>.GetValue(Type type) => GetVector3();
    }

    [Serializable]
    [InlineProperty]
    public class Vector3Reference : Reference<Vector3, Vector3Variable> { }
}