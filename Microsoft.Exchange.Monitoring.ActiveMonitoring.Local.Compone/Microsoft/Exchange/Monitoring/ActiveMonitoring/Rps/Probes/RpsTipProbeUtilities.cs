using System;
using System.Collections;
using System.Management.Automation;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Rps.Probes
{
	internal static class RpsTipProbeUtilities
	{
		public static object GetPropertyValue(this PSObject psObject, string propertyName)
		{
			object obj = psObject.Properties[propertyName].Value;
			if (obj != null && obj is PSObject)
			{
				obj = ((PSObject)obj).BaseObject;
			}
			return obj;
		}

		public static T GetPropertyValue<T>(this PSObject psObject, string propertyName, T defaultValue)
		{
			object propertyValue = psObject.GetPropertyValue(propertyName);
			if (propertyValue != null)
			{
				return (T)((object)propertyValue);
			}
			return defaultValue;
		}

		public static string GetStringValue(this PSObject psObject, string propertyName)
		{
			object propertyValue = psObject.GetPropertyValue(propertyName);
			if (propertyValue != null && propertyValue is IList)
			{
				return ((IList)propertyValue)[0] as string;
			}
			return propertyValue as string;
		}
	}
}
