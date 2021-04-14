using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	public enum PersistableRelocationConstraintType
	{
		ObsoleteDataConstraint = 6,
		CNFConstraint,
		InitialDomainConstraint,
		OutlookConstraint,
		FFOUpgradeConstraint,
		MonitoringConstraint
	}
}
