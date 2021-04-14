using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Directory.ABProviderFramework
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IABProviderFactory
	{
		ABSession Create(IABSessionSettings addressBookSessionSettings);

		ABProviderCapabilities GetProviderCapabilities(IABSessionSettings addressBookSessionSettings);
	}
}
