using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class AuditingOpticsConstants
	{
		public static readonly string SoftwareName = "Microsoft Exchange";

		public static readonly string LoggerComponentName = "AuditingOptics";

		public static readonly string AuditLoggerFileNamePrefix = "Optics";

		public static readonly string AuditLoggerTypeName = " Optics Logs";
	}
}
