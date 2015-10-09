using UnityEngine;
using Sfs2X;

public class SmartFoxConnection : MonoBehaviour {

    private static SmartFoxConnection mInstance;
    private static SmartFox sfs;

    public static SmartFox Connection
    {
        get
        {
            if(mInstance == null)
            {
                mInstance = new GameObject("SmartFoxConnection").AddComponent(typeof(SmartFoxConnection)) as SmartFoxConnection;
            } return sfs;
        }
        set
        {
            if(mInstance == null)
            {
                mInstance = new GameObject("SmartFoxConnection").AddComponent(typeof(SmartFoxConnection)) as SmartFoxConnection;
            }
            sfs = value;
        }
    }

    public static bool IsInitialized
    {
        get
        {
            return (sfs != null);
        }
    }

    void OnApplicationQuit()
    {
        if(sfs.IsConnected)
        {
            sfs.Disconnect();
        }
    }
}
