using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

public class Client : MonoBehaviour
{
    private static Client _instance;

    private static byte[] result = new byte[1024];
    private static Socket clientSocket;

    //是否已连接的标识
    public bool IsConnected = false;

    //定义标识符
    private char[] terminate = new char[] {'\r','\n','\t' }; //消息的结尾标记 

    public static Client Instance { get { return _instance; } }

    public Client()
    {
        _instance = this;

        clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    }

    /// <summary>
    /// 连接指定IP和端口的服务器
    /// </summary>
    /// <param name="ip"></param>
    /// <param name="port"></param>
    public void ConnectServer(string ip, int port)
    {
        IPAddress mIp = IPAddress.Parse(ip);
        IPEndPoint ip_end_point = new IPEndPoint(mIp, port);

        try
        {
            clientSocket.Connect(ip_end_point);
            IsConnected = true;
            Debug.Log("连接服务器成功");
        }
        catch
        {
            IsConnected = false;
            Debug.Log("连接服务器失败");
            return;
        }
        //服务器下发数据长度
        int receiveLength = clientSocket.Receive(result);
        ByteBuffer buffer = new ByteBuffer(result);
        int len = buffer.ReadShort();
        string data = buffer.ReadString();
        Debug.Log("服务器返回数据：" + data);
    }

    /// <summary>
    /// 发送数据给服务器
    /// </summary>
    public void SendMessage(string data)
    {
        if (IsConnected == false)
            return;
        try
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteString(data);                   //吧string转换添加入流 并在头以int类型写入数据的长度
            clientSocket.Send(WriteMessage(buffer.ToBytes()));
            Debug.Log("发送消息"+ data  );
            return;
        }
        catch
        {
            Debug.Log("服务器关闭");
            IsConnected = false;
            clientSocket.Shutdown(SocketShutdown.Both);
            clientSocket.Close();
        }
    }
    byte[] depthData;

    public void SendMessage(ushort[] data)
    {
        if (IsConnected == false)
            return;
        try
        {
            depthData = new byte[data.Length * 2];
           ByteBuffer buffer = new ByteBuffer();
            for (int i = 0; i < data.Length; i++)
            {

                //if (i % 5 != 0)
                //{
                //    continue;
                //}
                //Buffer.BlockCopy(BitConverter.GetBytes(data[i]), 0, depthData, i+2, 2);
                buffer.WriteUshort(data[i]);
            }
            clientSocket.Send(WriteMessage(buffer.ToBytes()));
            //数据获取转换 发送时间为0.03秒左右
            //Debug.Log("数据打包时间"+times);
        }
        catch
        {
            
            IsConnected = false;
            clientSocket.Shutdown(SocketShutdown.Both);
            clientSocket.Close();
        }
    }

    /// <summary>
    /// 数据转换，
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    private byte[] WriteMessage(byte[] message)
    {
        MemoryStream ms = null;
        using (ms = new MemoryStream())
        {

            ms.Position = 0;
            BinaryWriter writer = new BinaryWriter(ms);
            string xx = terminate.ToString();
           // ushort msglen = (ushort)message.Length;
            writer.Write(terminate);        //定位标志
            writer.Write(message.Length);
            Debug.LogError(message.Length);
            writer.Write(message);          //添加数据 
            writer.Flush();

            return  ms.ToArray();
        }
    }
    private void OnDestroy()
    {
        clientSocket.Shutdown(SocketShutdown.Both);
        clientSocket.Close();
    }
}