using System;
using System.Collections.Generic;

namespace WordSearch.API
{

	[Serializable]
	public class GoogleSheetResponse
	{
		public string range;
		public string majorDimension;
		public List<List<string>> values;
	}
}