using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Services.Core
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	public interface IUnifiedContactStoreConfiguration
	{
		int MaxImGroups { get; }

		int MaxImContacts { get; }
	}
}
