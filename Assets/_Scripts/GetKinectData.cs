using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Windows.Kinect;

public class GetKinectData : MonoBehaviour {

    static GetKinectData _instance;

    public Material mater;
    public Client funClient;
    private KinectSensor kinect;
    private ushort[] depthData;
    public DepthSourceManager depth;

    public static GetKinectData Instance { get { return _instance; } }
    private void Start()
    {
        _instance = this;

        kinect = KinectSensor.GetDefault();
        if (!kinect.IsOpen)
        {
            kinect.Open();
        }
       depthData = depth.GetData();
       var depthProperty =  kinect.DepthFrameSource.FrameDescription;
    }
    private void Update()
    {
        if (Client.Instance.IsConnected)
        {
            SendMessageDepth(depthData);
        }


    }
    public void SendMessageDepth(ushort[] depth)
    {
        funClient.SendMessage(depth);
    }
}
