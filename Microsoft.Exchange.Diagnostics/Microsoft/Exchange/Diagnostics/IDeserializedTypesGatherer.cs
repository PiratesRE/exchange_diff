using System;

namespace Microsoft.Exchange.Diagnostics
{
	public interface IDeserializedTypesGatherer
	{
		void Add(string typeName, string assemblyName);
	}
}
