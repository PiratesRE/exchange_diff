using System;

namespace Microsoft.Exchange.Configuration.Tasks
{
	public interface ISessionState
	{
		string CurrentPath { get; }

		string CurrentPathProviderName { get; }

		IVariableDictionary Variables { get; }
	}
}
