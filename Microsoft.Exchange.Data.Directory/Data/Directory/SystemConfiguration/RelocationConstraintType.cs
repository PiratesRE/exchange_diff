using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	public enum RelocationConstraintType
	{
		TenantVersionConstraint,
		TenantInTransitionConstraint,
		SCTConstraint,
		E14MailboxesPresentContraint,
		RelocationInProgressConstraint,
		ValidationErrorConstraint,
		ObsoleteDataConstraint,
		CNFConstraint,
		InitialDomainConstraint,
		OutlookConstraint,
		FFOUpgradeConstraint,
		MonitoringConstraint
	}
}
