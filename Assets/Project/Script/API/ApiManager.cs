using System;
using System.Collections;
using System.Net;

using UnityEngine;
using UnityEngine.Networking;

using static WordSearch.UniversalConstants;

namespace WordSearch.API
{
	public static class ApiManager
	{
		private static string _wordListURL = $"https://sheets.googleapis.com/v4/spreadsheets/{SPREADSHEET_ID}/values/{SHEET_NAME}?key={GOOGLE_SHEETS_API_KEY}";
		private static string _wordListWithRangeURL = $"https://sheets.googleapis.com/v4/spreadsheets/{SPREADSHEET_ID}/values/{SHEET_NAME}!{WORD_LIST_RANGE}?key={GOOGLE_SHEETS_API_KEY}";

		public static IEnumerator GetWordList(Action<string, HttpStatusCode> OnResultReceived)
		{
			Debug.Log($"Checking URL: {_wordListURL}");
			using (UnityWebRequest webRequest = UnityWebRequest.Get(_wordListURL))
			{
				yield return webRequest.SendWebRequest();

				HttpStatusCode statusCode = (HttpStatusCode)webRequest.responseCode;
				if (webRequest.result == UnityWebRequest.Result.Success)
				{
					Debug.Log("Received: " + webRequest.downloadHandler.text);
					OnResultReceived?.Invoke(webRequest.downloadHandler.text, statusCode);
				}
				else if (webRequest.result == UnityWebRequest.Result.ProtocolError)
				{
					Debug.LogError("ProtocolError: " + webRequest.error);
					OnResultReceived?.Invoke(webRequest.downloadHandler.text, statusCode);
				}
				else // if (webRequest.result == UnityWebRequest.Result.ConnectionError)
				{
					Debug.LogError("Error: " + webRequest.error);
					OnResultReceived?.Invoke(webRequest.error, statusCode);
				}
			}
		}
	}
}