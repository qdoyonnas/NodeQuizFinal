using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;

[ExecuteInEditMode]
public class Network : MonoBehaviour
{
	[NonSerialized]
	public SocketIOComponent socket;
	[NonSerialized]
	public DataController dataController;

	void Awake ()
	{
        Connect();
    }

	void OnConnected(SocketIOEvent e)
	{
		Debug.Log("Connected to server");
	}

	void OnLoadData(SocketIOEvent e)
	{
		Debug.Log("Loading Data from server");
		Debug.Log(e.data);
	}

	void OnDisconnect(SocketIOEvent e)
	{
		Debug.Log("User Disonnected");
	}

	void Connect()
	{
        socket = GetComponent<SocketIOComponent>();
        dataController = GetComponent<DataController>();

        socket.On("open", OnConnected);
        socket.On("recieveData", OnLoadData);
        socket.On("disconnect", OnDisconnect);

        if( !socket.IsConnected )
        {
            socket.Connect();
        }
	}

	public void LoadData()
    {
        if (socket == null) {
            Connect();
        }

        Debug.Log("Loading Data");
		socket.Emit("loadData");
	}

    public void SendData()
    {
        if( socket == null ) {
            Connect();
        }

        string jsonObj = JsonUtility.ToJson(dataController.GetCurrentRoundData());

		Debug.Log("Sending Data");
		socket.Emit("sendData", new JSONObject(jsonObj));
	}
}