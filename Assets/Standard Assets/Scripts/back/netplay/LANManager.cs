using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using UnityEngine.UI;
using RCEDevelopmentUtilities;
using RCEDevelopmentUtilities.RCENetworkUtilities;

public class LANManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Refresh()
    {
        try
        {
            IPAddress addr = NetworkScanner.GetMyRealIP();
            Debug.Log(addr.ToString());
            List<IPAddress> severs = NetworkScanner.GetActiveLANIPs();

            foreach (IPAddress addrr in severs)
            {
                Debug.Log(addrr.Address.ToString());
                GameObject.Find("DebugText").GetComponent<Text>().text = addr.Address.ToString();
            }
        }
        catch (Exception ex)
        {
            Debug.Log(string.Format("Exception\n{0}\nStackTrace\n{1}", ex.Message, ex.StackTrace));
        }
    }
}
