using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	public class QueryStringParameters : Dictionary<string, string>
	{
		public string GetValue(string name)
		{
			if (base.ContainsKey(name))
			{
				return base[name];
			}
			return null;
		}

		public string QueryString
		{
			get
			{
				StringBuilder stringBuilder = new StringBuilder(64);
				stringBuilder.Append("?");
				bool flag = false;
				foreach (string text in base.Keys)
				{
					if (!string.IsNullOrEmpty(text))
					{
						string text2 = base[text];
						if (!string.IsNullOrEmpty(text2))
						{
							if (flag)
							{
								stringBuilder.Append("&");
							}
							else
							{
								flag = true;
							}
							stringBuilder.Append(text);
							stringBuilder.Append("=");
							stringBuilder.Append(Utilities.UrlEncode(text2));
						}
					}
				}
				return stringBuilder.ToString();
			}
		}
	}
}
