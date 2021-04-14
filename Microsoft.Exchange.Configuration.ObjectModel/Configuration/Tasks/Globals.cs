using System;

namespace Microsoft.Exchange.Configuration.Tasks
{
	internal class Globals
	{
		public static string AuditingComponentName
		{
			get
			{
				return Globals.auditingComponentName;
			}
		}

		private static readonly string auditingComponentName = "Exchange Management";
	}
}
