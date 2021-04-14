using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Net;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class DDIObjectTypeConverter : TypeConverter
	{
		static DDIObjectTypeConverter()
		{
			DDIObjectTypeConverter.unHandledTypeList["System.Net.IPAddress"] = typeof(IPAddress);
			DDIObjectTypeConverter.unHandledTypeList["Microsoft.Exchange.Data.Common.LocalizedString, Microsoft.Exchange.Data.Common"] = typeof(LocalizedString);
			DDIObjectTypeConverter.unHandledTypeList["Microsoft.Exchange.Data.Common.LocalizedString"] = typeof(LocalizedString);
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			string text = value.ToString();
			if (DDIObjectTypeConverter.unHandledTypeList.ContainsKey(text))
			{
				return DDIObjectTypeConverter.unHandledTypeList[text];
			}
			object result;
			lock (DDIObjectTypeConverter.syncInstance)
			{
				Type type = Type.GetType(text);
				if (!DDIObjectTypeConverter.unHandledTypeList.ContainsKey(text))
				{
					DDIObjectTypeConverter.unHandledTypeList.Add(text, type);
				}
				result = type;
			}
			return result;
		}

		private static object syncInstance = new object();

		private static Dictionary<string, Type> unHandledTypeList = new Dictionary<string, Type>();
	}
}
