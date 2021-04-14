using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class MigrationConstraints
	{
		public const int MaxImapPortValue = 65535;

		public static readonly StringLengthConstraint NameLengthConstraint = new StringLengthConstraint(1, 256);

		public static readonly StringLengthConstraint RemoteServerNameConstraint = new StringLengthConstraint(1, 256);

		public static readonly RangedValueConstraint<int> PortRangeConstraint = new RangedValueConstraint<int>(1, 65535);

		public static readonly RangedValueConstraint<Unlimited<int>> MaxConcurrentMigrationsConstraint = new RangedValueConstraint<Unlimited<int>>(1, Unlimited<int>.UnlimitedValue);

		public static readonly RangedValueConstraint<int> PasswordRangeConstraint = new RangedValueConstraint<int>(1, 256);

		public static readonly RangedValueConstraint<int> ExportMigrationReportRowCountConstraint = new RangedValueConstraint<int>(0, 2000);
	}
}
