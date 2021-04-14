using System;
using System.Reflection;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Management.SystemManager;

namespace Microsoft.Exchange.Management.DDIService
{
	public class ExchangeConfigObjectInfoRetriever : IDataObjectInfoRetriever
	{
		public virtual void Retrieve(Type dataObjectType, string propertyName, out Type objectType, out PropertyDefinition propertyDefinition)
		{
			PropertyInfo propertyEx = dataObjectType.GetPropertyEx(propertyName);
			if (propertyEx != null)
			{
				objectType = propertyEx.PropertyType;
			}
			else
			{
				objectType = null;
			}
			propertyDefinition = PropertyConstraintProvider.GetPropertyDefinition(dataObjectType, propertyName);
		}
	}
}
