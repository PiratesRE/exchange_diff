using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.DDIService
{
	public interface IDataObjectInfoRetriever
	{
		void Retrieve(Type dataObjectType, string propertyName, out Type type, out PropertyDefinition propertyDefinition);
	}
}
