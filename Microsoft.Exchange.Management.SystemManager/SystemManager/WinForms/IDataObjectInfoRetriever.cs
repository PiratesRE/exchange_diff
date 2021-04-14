using System;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public interface IDataObjectInfoRetriever
	{
		void Retrieve(Type dataObjectType, string propertyName, out Type type);
	}
}
