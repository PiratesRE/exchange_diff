using System;

namespace Microsoft.Exchange.Cluster.Shared
{
	public interface IAmServerNameLookup
	{
		void Enable();

		string GetFqdn(string shortNodeName);

		string GetFqdn(string shortNodeName, bool throwException);

		void RemoveEntry(string serverName);

		void PopulateForDag();
	}
}
