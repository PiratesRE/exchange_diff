using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.ApplicationLogic
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class AdminAuditLogLabels
	{
		public const string CmdletName = "Cmdlet Name";

		public const string ObjectModified = "Object Modified";

		public const string ModifiedObjectResolvedName = "Modified Object Resolved Name";

		public const string Parameter = "Parameter";

		public const string PropertyModified = "Property Modified";

		public const string PropertyOriginal = "Property Original";

		public const string Caller = "Caller";

		public const string ExternalAccess = "ExternalAccess";

		public const string Succeeded = "Succeeded";

		public const string Error = "Error";

		public const string RunDate = "Run Date";

		public const string OriginatingServer = "OriginatingServer";

		public const string UnknownUser = "UnknownUser";
	}
}
