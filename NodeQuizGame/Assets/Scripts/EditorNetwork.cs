using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using SocketIO;

[CustomEditor(typeof(Network))]
public class EditorNetwork : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();
		
		Network network = (Network)target;

		if( GUILayout.Button("Load Data") ) {
			network.LoadData();
		}

		if( GUILayout.Button("Send Data") ) {
			network.SendData();
		}
	}
}