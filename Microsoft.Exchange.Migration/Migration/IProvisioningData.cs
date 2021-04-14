using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IProvisioningData
	{
		ProvisioningType ProvisioningType { get; }

		ProvisioningComponent Component { get; }

		Dictionary<string, object> Parameters { get; }

		PersistableDictionary ToPersistableDictionary();
	}
}
