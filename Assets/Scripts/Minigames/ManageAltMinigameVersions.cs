using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManageAltMinigameVersions : MonoBehaviour {

    [SerializeField]
    private List<GameObject> versions;


    /// <summary>
    /// Selects one random version
    /// </summary>
    /// <returns></returns>
    public GameObject GetRandomVersion() {

        return versions[Random.Range(0, versions.Count)];
    }

}
