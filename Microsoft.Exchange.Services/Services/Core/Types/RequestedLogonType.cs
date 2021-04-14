using System;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal class RequestedLogonType
	{
		private RequestedLogonType(LogonTypeSource source, LogonType type)
		{
			this.Source = source;
			this.LogonType = type;
		}

		public LogonType LogonType { get; private set; }

		public LogonTypeSource Source { get; private set; }

		public static readonly RequestedLogonType AdminFromManagementRoleHeader = new RequestedLogonType(LogonTypeSource.ManagementRoleHeader, LogonType.Admin);

		public static readonly RequestedLogonType SystemServiceFromManagementRoleHeader = new RequestedLogonType(LogonTypeSource.ManagementRoleHeader, LogonType.SystemService);

		public static readonly RequestedLogonType AdminFromOpenAsAdminOrSystemServiceHeader = new RequestedLogonType(LogonTypeSource.OpenAsAdminOrSystemServiceHeader, LogonType.Admin);

		public static readonly RequestedLogonType SystemServiceFromOpenAsAdminOrSystemServiceHeader = new RequestedLogonType(LogonTypeSource.OpenAsAdminOrSystemServiceHeader, LogonType.SystemService);

		public static readonly RequestedLogonType Default = new RequestedLogonType(LogonTypeSource.Default, LogonType.BestAccess);
	}
}
