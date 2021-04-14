using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Directory.ABProviderFramework
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IABSessionSettings
	{
		bool TryGet<T>(string propertyName, out T propertyValue);

		T Get<T>(string propertyName);
	}
}
