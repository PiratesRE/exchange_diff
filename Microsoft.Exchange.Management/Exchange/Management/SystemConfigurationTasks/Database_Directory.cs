using System;
using System.Security.AccessControl;
using System.Security.Principal;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	internal static class Database_Directory
	{
		internal static FileSystemAccessRule[] GetDomainWidePermissions()
		{
			FileSystemAccessRule[] result;
			try
			{
				NTAccount ntaccount = new NTAccount(NativeHelpers.GetDomainName() + "\\Organization Management");
				NTAccount ntaccount2 = new NTAccount(NativeHelpers.GetDomainName() + "\\View-Only Organization Management");
				FileSystemAccessRule fileSystemAccessRule = new FileSystemAccessRule(ntaccount.Translate(typeof(SecurityIdentifier)) as SecurityIdentifier, FileSystemRights.FullControl, InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit, PropagationFlags.None, AccessControlType.Allow);
				FileSystemAccessRule fileSystemAccessRule2 = new FileSystemAccessRule(ntaccount2.Translate(typeof(SecurityIdentifier)) as SecurityIdentifier, FileSystemRights.ReadAndExecute, InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit, PropagationFlags.None, AccessControlType.Allow);
				result = new FileSystemAccessRule[]
				{
					Database_Directory.builtInAdmin,
					Database_Directory.builtInLocalSystem,
					fileSystemAccessRule,
					fileSystemAccessRule2
				};
			}
			catch (Exception)
			{
				result = new FileSystemAccessRule[]
				{
					Database_Directory.builtInAdmin,
					Database_Directory.builtInLocalSystem
				};
			}
			return result;
		}

		private const string exchangeOrgAdminName = "Organization Management";

		private const string exchangeViewOnlyAdminName = "View-Only Organization Management";

		private static FileSystemAccessRule builtInAdmin = new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.BuiltinAdministratorsSid, null), FileSystemRights.FullControl, InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit, PropagationFlags.None, AccessControlType.Allow);

		private static FileSystemAccessRule builtInLocalSystem = new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.LocalSystemSid, null), FileSystemRights.FullControl, InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit, PropagationFlags.None, AccessControlType.Allow);
	}
}
