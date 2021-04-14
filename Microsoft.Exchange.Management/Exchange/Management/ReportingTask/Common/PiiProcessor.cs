using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Configuration.Tasks;

namespace Microsoft.Exchange.Management.ReportingTask.Common
{
	internal class PiiProcessor
	{
		public bool SuppressPiiEnabled { get; set; }

		public void Supress(object dataObject)
		{
			if (!this.SuppressPiiEnabled)
			{
				return;
			}
			Type type = dataObject.GetType();
			List<PiiProcessor.PiiProperty> piiProperties = this.GetPiiProperties(type);
			foreach (PiiProcessor.PiiProperty piiProperty in piiProperties)
			{
				string text = (string)piiProperty.PropertyInfo.GetValue(dataObject, null);
				if (piiProperty.PiiDataType == PiiDataType.Smtp)
				{
					piiProperty.PropertyInfo.SetValue(dataObject, SuppressingPiiData.RedactSmtpAddress(text), null);
				}
				else
				{
					if (piiProperty.PiiDataType != PiiDataType.String)
					{
						throw new NotSupportedException("Unsupported PII data type: " + piiProperty.PiiDataType);
					}
					piiProperty.PropertyInfo.SetValue(dataObject, SuppressingPiiData.Redact(text), null);
				}
			}
		}

		private List<PiiProcessor.PiiProperty> GetPiiProperties(Type type)
		{
			List<PiiProcessor.PiiProperty> list;
			if (this.piiPropertyDictionary.ContainsKey(type))
			{
				list = this.piiPropertyDictionary[type];
			}
			else
			{
				list = new List<PiiProcessor.PiiProperty>();
				PropertyInfo[] properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
				foreach (PropertyInfo propertyInfo in properties)
				{
					object[] customAttributes = propertyInfo.GetCustomAttributes(typeof(SuppressPiiAttribute), true);
					if (propertyInfo.PropertyType == typeof(string) && customAttributes.Length > 0)
					{
						SuppressPiiAttribute suppressPiiAttribute = (SuppressPiiAttribute)customAttributes[0];
						list.Add(new PiiProcessor.PiiProperty
						{
							PropertyInfo = propertyInfo,
							PiiDataType = suppressPiiAttribute.PiiDataType
						});
					}
				}
				this.piiPropertyDictionary.Add(type, list);
			}
			return list;
		}

		private readonly Dictionary<Type, List<PiiProcessor.PiiProperty>> piiPropertyDictionary = new Dictionary<Type, List<PiiProcessor.PiiProperty>>();

		private class PiiProperty
		{
			public PropertyInfo PropertyInfo { get; set; }

			public PiiDataType PiiDataType { get; set; }
		}
	}
}
