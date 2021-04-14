using System;

namespace Microsoft.Exchange.Management.Hybrid
{
	internal interface IContextParameters
	{
		T Get<T>(string name);

		void Set<T>(string name, T value);
	}
}
