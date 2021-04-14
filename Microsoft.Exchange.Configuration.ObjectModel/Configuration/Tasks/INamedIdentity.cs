using System;

namespace Microsoft.Exchange.Configuration.Tasks
{
	public interface INamedIdentity
	{
		string Identity { get; }

		string DisplayName { get; }
	}
}
