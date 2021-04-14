using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Data.Directory.Provisioning
{
	internal interface IProvisioningRule
	{
		ICollection<Type> TargetObjectTypes { get; }

		ProvisioningContext Context { get; set; }
	}
}
