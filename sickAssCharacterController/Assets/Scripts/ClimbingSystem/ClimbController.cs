using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

public class ClimbController : MonoBehaviour
{
    EnvironmentScanner envScanner;
    AnimatorManager animatorManager;
    

    private void Awake()
    {
        envScanner = GetComponent<EnvironmentScanner>();
        animatorManager = GetComponent<AnimatorManager>();  
    }

   

}
