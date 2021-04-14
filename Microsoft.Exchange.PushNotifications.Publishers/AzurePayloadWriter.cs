using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal class AzurePayloadWriter
	{
		public AzurePayloadWriter()
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
			this.AddProperty(key, value.ToString());
		}

		public override string ToString()
		{
			return string.Format("{{{0}}}", string.Join(", ", this.properties));
		}

		private void AddProperty(string key, string value)
		{
			this.properties.Add(string.Format("\"{0}\": \"{1}\"", key, value));
		}

		public const string PropertySeparator = ", ";

		private List<string> properties;
	}
}
