using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManagerMultiplayer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("LevelManagerMultiplayer");
        PhotonNetwork.Instantiate("PlayerMultiplayer", new Vector3(0,0,0), Quaternion.identity);
    }

}
