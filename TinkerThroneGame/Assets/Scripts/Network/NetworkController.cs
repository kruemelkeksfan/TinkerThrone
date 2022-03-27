using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkController : MonoBehaviour
{
	public delegate void NetworkMethod(string[] parameters);

	public static NetworkController instance = null;

	[SerializeField] private string hostAddress = "127.0.0.1";
	[SerializeField] private string username = "Dummy";
	[SerializeField] private string password = "GEHEIM";
	[SerializeField] private bool registerUser = false;
	private Dictionary<string, string> parameterDictionary = null;
	private HashSet<UnityWebRequest> runningRequests = null;
	private List<UnityWebRequest> deleteRequests = null;
	private Dictionary<string, NetworkMethod> listeners = null;
	private bool loggedIn = false;

	public static NetworkController GetInstance()
	{
		return instance;
	}

	private void Awake()
	{
		parameterDictionary = new Dictionary<string, string>();
		runningRequests = new HashSet<UnityWebRequest>();
		deleteRequests = new List<UnityWebRequest>();
		listeners = new Dictionary<string, NetworkMethod>();

		instance = this;
	}

	private void Start()
	{
		RegisterListener(Register);
		RegisterListener(Login);
		RegisterListener(Logout);
		// RegisterListener(Save);
		// RegisterListener(Load);

		if(registerUser)
		{
			SendRequest(Register, new KeyValuePair<string, string>("Username", username),
				new KeyValuePair<string, string>("Password", password),
				new KeyValuePair<string, string>("RepeatPassword", password));
		}
		else
		{
			SendRequest(Login, new KeyValuePair<string, string>("Username", username),
				new KeyValuePair<string, string>("Password", password));
		}

		// DEBUG
		// StartCoroutine(Test());
	}

	public IEnumerator Test()
	{
		// yield return new WaitForSeconds(1.0f);
		// SendRequest(Logout);

		yield return new WaitForSeconds(1.0f);
		SendRequest(Save, new KeyValuePair<string, string>("Timestamp", DateTime.UtcNow.ToString("yyyy/MM/dd/HH/mm/ss", CultureInfo.InvariantCulture)),
				new KeyValuePair<string, string>("Save", "Definitely a JSON"));

		yield return new WaitForSeconds(1.0f);
		SendRequest(Load);

		yield return new WaitForSeconds(1.0f);
		SendRequest(Logout);
	}

	private void Update()
	{
		deleteRequests.Clear();
		foreach(UnityWebRequest request in runningRequests)
		{
			if(request.isDone)
			{
				deleteRequests.Add(request);

				string[] reply = request.downloadHandler.text.Split(':', System.StringSplitOptions.RemoveEmptyEntries);
				if(reply.Length != 2)
				{
					Debug.LogError("Invalid Server Reply '" + request.downloadHandler.text + "' in NetworkController!");
					continue;
				}

				if(listeners.ContainsKey(reply[0]))
				{
					listeners[reply[0]](reply[1].Split('|', System.StringSplitOptions.RemoveEmptyEntries));
				}
				else
				{
					Debug.LogError("Invalid Method Name '" + reply[0] + "' in NetworkController, did you forget to register a Callback?");
					continue;
				}
			}
		}
		foreach(UnityWebRequest request in deleteRequests)
		{
			// https://github.com/LastAbyss/SimpleGraphQL-For-Unity/issues/28
			request.uploadHandler.Dispose();
			request.downloadHandler.Dispose();
			request.Dispose();

			runningRequests.Remove(request);
		}
	}

	public void SendRequest(NetworkMethod callback, params KeyValuePair<string, string>[] parameters)
	{
		parameterDictionary.Clear();
		parameterDictionary.Add("MethodName", callback.Method.Name);
		foreach(KeyValuePair<string, string> parameter in parameters)
		{
			parameterDictionary.Add(parameter.Key, parameter.Value);
		}

		UnityWebRequest request = UnityWebRequest.Post(hostAddress, parameterDictionary);
		request.SendWebRequest();
		runningRequests.Add(request);
	}

	public void Register(string[] parameters)
	{
		if(parameters[0] == "Successful")
		{
			loggedIn = true;
		}

		/*if(parameters[0] == "Successful")
		{
			Debug.Log("Registration successful");
		}
		else
		{
			Debug.Log("Registration failed:");
			foreach(string parameter in parameters)
			{
				Debug.Log(parameter);
			}
		}*/
	}

	public void Login(string[] parameters)
	{
		if(parameters[0] == "Successful")
		{
			loggedIn = true;
		}

		/*if(parameters[0] == "Successful")
		{
			Debug.Log("Login successful");
		}
		else
		{
			Debug.Log("Login failed:");
			foreach(string parameter in parameters)
			{
				Debug.Log(parameter);
			}
		}*/
	}

	public void Logout(string[] parameters)
	{
		loggedIn = false;

		/*if(parameters[0] == "Successful")
		{
			Debug.Log("Logout successful");
		}
		else
		{
			Debug.Log("Logout failed:");
			foreach(string parameter in parameters)
			{
				Debug.Log(parameter);
			}
		}*/
	}

	public void Save(string[] parameters)
	{
		/*if(parameters[0] == "Successful")
		{
			Debug.Log("Save successful");
		}
		else
		{
			Debug.Log("Save failed:");
			foreach(string parameter in parameters)
			{
				Debug.Log(parameter);
			}
		}*/
	}

	public void Load(string[] parameters)
	{
		/*if(parameters[0] == "Successful")
		{
			Debug.Log("Load successful");
			Debug.Log("Timestamp: " + parameters[1]);
			Debug.Log("Save: " + parameters[2]);
		}
		else
		{
			Debug.Log("Load failed:");
			foreach(string parameter in parameters)
			{
				Debug.Log(parameter);
			}
		}*/
	}

	public void RegisterListener(NetworkMethod networkMethod)
	{
		listeners.Add(networkMethod.Method.Name, networkMethod);
	}

	public bool IsLoggedIn()
	{
		return loggedIn;
	}
}
