using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.PushNotifications.Utils
{
	internal sealed class PropertyReader
	{
		public PropertyReader(string[] propertySeparator, string[] valueSeparator)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("propertySeparator", propertySeparator);
			ArgumentValidator.ThrowIfNullOrEmpty("valueSeparator", valueSeparator);
			this.PropertySeparator = propertySeparator;
			this.ValueSeparator = valueSeparator;
		}

		private string[] PropertySeparator { get; set; }

		private string[] ValueSeparator { get; set; }

		public Dictionary<string, string> Read(string payload)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
			if (!string.IsNullOrWhiteSpace(payload))
			{
				string[] array = payload.Split(this.PropertySeparator, StringSplitOptions.RemoveEmptyEntries);
				foreach (string text in array)
				{
					string[] array3 = text.Split(this.ValueSeparator, StringSplitOptions.RemoveEmptyEntries);
					if (array3.Length == 2)
					{
						dictionary.Add(Uri.UnescapeDataString(array3[0]), Uri.UnescapeDataString(array3[1]));
					}
				}
			}
			return dictionary;
		}
	}
}
