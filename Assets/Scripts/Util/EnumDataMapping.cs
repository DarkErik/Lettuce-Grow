using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EnumDataMapping<TData, TEnum> where TEnum : Enum
{
    [SerializeField] private TData[] mapping;

    //Array access if needed
    public TData this[int index] {
        get { return mapping[index]; }
        set { mapping[index] = value; }
    }

    public TData this[TEnum enm] {
        get { return mapping[Convert.ToInt32(enm)]; }
        set { mapping[Convert.ToInt32(enm)] = value; }
    }

    public int Length { get { return mapping.Length; } }
}