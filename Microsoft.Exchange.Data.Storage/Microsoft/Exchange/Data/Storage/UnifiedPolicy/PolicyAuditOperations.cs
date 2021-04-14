using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Office.CompliancePolicy.PolicyConfiguration;

namespace Microsoft.Exchange.Data.Storage.UnifiedPolicy
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class PolicyAuditOperations
	{
		public MultiValuedProperty<AuditableOperations> AuditOperationsDelegate;
	}
}
