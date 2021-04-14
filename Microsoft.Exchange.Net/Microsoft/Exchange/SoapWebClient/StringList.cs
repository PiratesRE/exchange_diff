using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Exchange.SoapWebClient
{
	[Serializable]
	public sealed class StringList : List<string>
	{
		public StringList()
		{
		}

		public StringList(IEnumerable<string> collection) : base(collection)
		{
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(40 * base.Count);
			foreach (string value in this)
			{
				if (stringBuilder.Length > 0)
				{
					stringBuilder.Append(",");
				}
				stringBuilder.Append(value);
			}
			return stringBuilder.ToString();
		}
	}
}
