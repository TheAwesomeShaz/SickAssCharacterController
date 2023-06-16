using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbAction : ScriptableObject
{
    [field: SerializeField] string AnimName { get; set; }
    [field: SerializeField] float MatchStartTime {get; set; }
    [field: SerializeField] string MatchTargetTime { get; set; }
}
