using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal class GcmPayloadWriter
	{
		public GcmPayloadWriter()
		{
			this.properties = new List<string>();
		}

		public void WriteProperty(string key, string value)
		{
			ArgumentValidator.ThrowIfNullOrWhiteSpace("key", key);
			if (!string.IsNullOrWhiteSpace(value))
			{
				this.AddProperty(key, value);
			}
		}

		public void WriteProperty<T>(string key, T? value) where T : struct
		{
			ArgumentValidator.ThrowIfNullOrWhiteSpace("key", key);
			if (value != null)
			{
				T value2 = value.Value;
				this.AddProperty(key, value2.ToString());
			}
		}

		public void WriteProperty<T>(string key, T value) where T : struct
		{
			ArgumentValidator.ThrowIfNullOrWhiteSpace("key", key);
			if (!object.Equals(value, default(T)))
			{
				this.AddProperty(key, value.ToString());
			}
		}

		public override string ToString()
		{
			return string.Join("&", this.properties);
		}

		private void AddProperty(string key, string value)
		{
			this.properties.Add(string.Format("{0}{1}{2}", Uri.EscapeUriString(key), "=", Uri.EscapeUriString(value)));
		}

		public const string PropertySeparator = "&";

		public const string ValueSeparator = "=";

		private List<string> properties;
	}
}
