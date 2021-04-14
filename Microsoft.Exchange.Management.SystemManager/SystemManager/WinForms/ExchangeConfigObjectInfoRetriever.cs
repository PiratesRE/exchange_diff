using System;
using System.Reflection;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class ExchangeConfigObjectInfoRetriever : IDataObjectInfoRetriever
	{
		public virtual void Retrieve(Type dataObjectType, string propertyName, out Type objectType)
		{
			PropertyInfo property = dataObjectType.GetProperty(propertyName);
			if (property != null)
			{
				objectType = property.PropertyType;
				return;
			}
			objectType = null;
		}
	}
}
