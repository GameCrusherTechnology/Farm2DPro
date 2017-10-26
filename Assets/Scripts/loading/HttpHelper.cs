using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HttpHelper
{
	private static IList<HttpHelperItem> poolItemList = new List<HttpHelperItem> ();
	public static void Request(string url, string method, Dictionary<string, object> formData, Action<object> callback, string responseType)
	{
		HttpHelperItem httpHelperItem = GetOrCreateItem ();
		if(method == MethodTypeInfo.GET)
		{
			httpHelperItem.Request(CreateGetData(url, formData), null, callback, responseType);
		}
		else
		{
			httpHelperItem.Request(url, CreatePostData(formData), callback, responseType);
		}
	}

	private static HttpHelperItem GetOrCreateItem()
	{
		foreach(HttpHelperItem item in poolItemList)
		{
			if(item.isDone) return item;
		}

		GameObject httpHelperObject = new GameObject ();
		httpHelperObject.name = "HttpHelperItem";
		HttpHelperItem httpHelperItem = httpHelperObject.AddComponent<HttpHelperItem> ();
		poolItemList.Add (httpHelperItem);
		return httpHelperItem;
	}

	protected static string CreateGetData(string url, Dictionary<string, object> formData)
	{
		StringBuilder stringBuilder = new StringBuilder ();
		if(formData != null && formData.Count > 0)
		{
			foreach(KeyValuePair<string, object> keyValuePair in formData)
			{
				stringBuilder.Append(keyValuePair.Key);
				stringBuilder.Append("=");
				stringBuilder.Append(keyValuePair.Value.ToString());
				stringBuilder.Append("&");
			}
		}
		if(url.IndexOf("?") == -1)
		{
			url += "?";
		}else
		{
			url += "&";
		}
		url += stringBuilder.ToString ().TrimEnd (new char[]{'&'});
		return url;
	}

	protected static WWWForm CreatePostData(Dictionary<string, object> formData)
	{
		WWWForm form = new WWWForm ();
		if(formData != null && formData.Count > 0)
		{
			foreach(KeyValuePair<string, object> keyValuePair in formData)
			{
				if(keyValuePair.Value is byte[])
				{
					form.AddBinaryData(keyValuePair.Key, keyValuePair.Value as byte[]);
				}else
				{
					form.AddField(keyValuePair.Key, keyValuePair.Value.ToString());
				}
			}
		}
		return form;
	}
}
public class MethodTypeInfo
{
	public static readonly string GET  = "get";
	public static readonly string POST  = "post";
}
public class ResponseTypeInfo
{
	public static readonly string TEXT = "text";
	public static readonly string BYTE = "byte";
}
public class HttpHelperItem : MonoBehaviour
{
	private Action<object> callback;
	private string responseType;
	private string url;
	private WWWForm formData;

	public bool isDone;

	public void Request(string url, WWWForm formData, Action<object> callback, string responseType)
	{
		this.callback = callback;
		this.responseType = responseType;
		this.url = url;
		this.formData = formData;

		this.isDone = false;

		this.StartCoroutine ("StartRequest");
	}

	private IEnumerator StartRequest(string url, WWWForm formData)
	{
		WWW www = null;
		if(formData == null)
		{
			www = new WWW (url);
		}else
		{
			www = new WWW(url, formData);
		}

		yield return www;
		if(www.isDone && string.IsNullOrEmpty(www.error))
		{
			if(this.responseType == ResponseTypeInfo.BYTE)
			{
				if(this.callback != null) this.callback(www.bytes);
			}else if(this.responseType == ResponseTypeInfo.TEXT)
			{
				if(this.callback != null) this.callback(www.text);
			}
		}
		else
		{
			if(this.callback != null) this.callback("error");
		}
		www.Dispose ();
		this.StopCoroutine ("StartRequest");
		this.isDone = true;

	}
}

