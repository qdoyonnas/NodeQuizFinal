using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;

public class Network : MonoBehaviour
{
	[NonSerialized]
	public SocketIOComponent socket;
	[NonSerialized]
	public DataController dataController;

	void Start ()
	{
		socket = GetComponent<SocketIOComponent>();
		dataController = GetComponent<DataController>();
		socket.On("open", OnConnected);
		socket.On("loadData", OnLoadData);
		socket.On("disconnect", OnDisconnect);
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

	bool Connect()
	{
		if( socket == null ) {
			socket = GetComponent<SocketIOComponent>();

			if( socket == null ) {
				Debug.Log("Could not find SocketIOComponent");
				return false;
			}
		} 

		if( dataController == null ) {
			dataController = GetComponent<DataController>();

			if( dataController == null ) {
				Debug.Log("Could not find Data Controller");
				return false;
			}
		}
		

		if( !socket.IsConnected ) {
			Debug.Log("Connecting...");
			socket.Connect();

			if( !socket.IsConnected ) {
				Debug.Log("Could not find server");
				return false;
			}
		}

		return true;
	}

	public void LoadData()
	{
		if( !Connect() ) { return; }

		Debug.Log("Loading Data");
		socket.Emit("loadData");
	}

	public void SendData()
	{
		if( !Connect() ) { return; }

		string jsonObj = JsonUtility.ToJson(dataController.GetCurrentRoundData());

		Debug.Log("Sending Data");
		socket.Emit("sendData", new JSONObject(jsonObj));
	}
}