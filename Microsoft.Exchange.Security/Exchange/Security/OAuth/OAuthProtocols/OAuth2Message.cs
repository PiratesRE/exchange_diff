using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Security.OAuth.OAuthProtocols
{
	internal abstract class OAuth2Message
	{
		public override string ToString()
		{
			return this.Encode();
		}

		protected bool ContainsKey(string key)
		{
			return this._message.ContainsKey(key);
		}

		protected void Decode(string message)
		{
			this._message.Decode(message);
		}

		protected void DecodeFromJson(string message)
		{
			this._message.DecodeFromJson(message);
		}

		protected string Encode()
		{
			return this._message.Encode();
		}

		protected string EncodeToJson()
		{
			return this._message.EncodeToJson();
		}

		protected string this[string index]
		{
			get
			{
				return this.GetValue(index);
			}
			set
			{
				this._message[index] = value;
			}
		}

		protected IEnumerable<string> Keys
		{
			get
			{
				return this._message.Keys;
			}
		}

		public Dictionary<string, string> Message
		{
			get
			{
				return this._message;
			}
		}

		protected string GetValue(string key)
		{
			if (string.IsNullOrEmpty(key))
			{
				throw new ArgumentException("The input string parameter is either null or empty.", "key");
			}
			string result = null;
			this._message.TryGetValue(key, out result);
			return result;
		}

		private Dictionary<string, string> _message = new Dictionary<string, string>(StringComparer.Ordinal);
	}
}
