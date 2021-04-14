using System;
using System.ComponentModel;
using System.Configuration;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	internal static class AppSettings
	{
		internal static T GetConfiguredValue<T>(string fieldName, T defaultValue)
		{
			if (string.IsNullOrEmpty(fieldName))
			{
				throw new ArgumentNullException("fieldName");
			}
			T result = defaultValue;
			string text = ConfigurationManager.AppSettings[fieldName];
			if (!string.IsNullOrEmpty(text))
			{
				TypeConverter converter = TypeDescriptor.GetConverter(typeof(T));
				if (converter != null)
				{
					try
					{
						result = (T)((object)converter.ConvertFromInvariantString(text));
					}
					catch (Exception)
					{
					}
				}
			}
			return result;
		}
	}
}
