using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Management.RightsManagement
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	public interface ITpdImporter
	{
		TrustedDocDomain Import(Guid tenantId);
	}
}
