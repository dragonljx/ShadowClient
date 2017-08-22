using UnityEngine;
using UnityEngine.UI;

public class LinkServer : MonoBehaviour
{

    public Client client;
    public Text ip;
    public Text port;
    public Button button;

    private Transform pConnect;
    private Transform pInput;
    public Text TextMessage;
    private void Start()
    {
        pConnect = transform.Find("PConnect");
        pInput = transform.Find("PInput");


        pInput.gameObject.SetActive(false);

        string x = "127.0.0.1";
        int y = 2222;
        //string x = "192.168.11.231";
        //int y = 2222;
        button.onClick.AddListener(delegate ()
        {
            if (ip.text.ToString() != "")
            {
                //cloud.LinkServer(ip.text.ToString(), int.Parse(port.text.ToString()));
                client.ConnectServer(ip.text.ToString(), int.Parse(port.text.ToString()));
                pConnect.gameObject.SetActive(false);
                pInput.gameObject.SetActive(true);
            }
            else
            {
                client.ConnectServer(x, y);
                pConnect.gameObject.SetActive(false);
                pInput.gameObject.SetActive(true);
                //cloud.LinkServer(x, y);
            }
        });
    }


    public void Send()
    {
        string message = TextMessage.text.ToString();
        client.SendMessage(message);
    }
}