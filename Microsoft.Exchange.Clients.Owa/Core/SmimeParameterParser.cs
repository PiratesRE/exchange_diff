using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	internal class SmimeParameterParser
	{
		public SmimeParameterParser(string smimeParameter)
		{
			string[] array = smimeParameter.Split(new char[]
			{
				';'
			});
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			foreach (string text in array)
			{
				if (!string.IsNullOrEmpty(text))
				{
					string[] array3 = text.Split(new char[]
					{
						':'
					});
					if (array3.Length != 2)
					{
						throw new OwaInvalidRequestException("Invalid S/MIME Parameter, the format should be  [key1]:[value1];[key2]:[value2]...");
					}
					dictionary.Add(array3[0], array3[1]);
				}
			}
			if (!dictionary.TryGetValue("Ver", out this.smimeControlVersion))
			{
				this.smimeControlVersion = null;
			}
			string text2;
			if (dictionary.TryGetValue("SSL", out text2) && text2.Equals("1"))
			{
				this.connectionIsSSL = true;
				return;
			}
			this.connectionIsSSL = false;
		}

		public bool ConnectionIsSSL
		{
			get
			{
				return this.connectionIsSSL;
			}
		}

		public string SmimeControlVersion
		{
			get
			{
				return this.smimeControlVersion;
			}
		}

		private const string StatusSSLKeyName = "SSL";

		private const string StatusVersionKeyName = "Ver";

		private const string BoolTrueValueName = "1";

		private const char KeyValuePairsSeperator = ';';

		private const char KeyValueSeperator = ':';

		private bool connectionIsSSL;

		private string smimeControlVersion;
	}
}
