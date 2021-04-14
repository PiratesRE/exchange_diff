using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal static class ParallelPublicFolderMigrationVersionChecker
	{
		public static void ThrowIfMinimumRequiredVersionNotInstalled(int sourceServerVersion)
		{
			ParallelPublicFolderMigrationVersionChecker.ThrowIfMinimumRequiredVersionNotInstalled(new ServerVersion(sourceServerVersion));
		}

		public static void ThrowIfMinimumRequiredVersionNotInstalled(ServerVersion sourceServerVersion)
		{
			LocalizedString? localizedString = ParallelPublicFolderMigrationVersionChecker.CheckForMinimumRequiredVersion(sourceServerVersion);
			if (localizedString != null)
			{
				throw new PublicFolderMigrationNotSupportedFromVersionException(localizedString.Value);
			}
		}

		public static LocalizedString? CheckForMinimumRequiredVersion(int sourceServerVersion)
		{
			return ParallelPublicFolderMigrationVersionChecker.CheckForMinimumRequiredVersion(new ServerVersion(sourceServerVersion));
		}

		public static LocalizedString? CheckForMinimumRequiredVersion(ServerVersion sourceServerVersion)
		{
			LocalizedString? result = null;
			int num = sourceServerVersion.ToInt();
			if (sourceServerVersion.Major < 8)
			{
				result = new LocalizedString?(MrsStrings.PublicFolderMigrationNotSupportedFromExchange2003OrEarlier(sourceServerVersion.Major, sourceServerVersion.Minor, sourceServerVersion.Build, sourceServerVersion.Revision));
			}
			else if (sourceServerVersion.Major == 8 && num < ParallelPublicFolderMigrationVersionChecker.E12MinVersionNumber)
			{
				result = new LocalizedString?(MrsStrings.PublicFolderMigrationNotSupportedFromCurrentExchange2007Version(sourceServerVersion.Major, sourceServerVersion.Minor, sourceServerVersion.Build, sourceServerVersion.Revision));
			}
			else if (sourceServerVersion.Major == 14 && num < ParallelPublicFolderMigrationVersionChecker.E14MinVersionNumber)
			{
				result = new LocalizedString?(MrsStrings.PublicFolderMigrationNotSupportedFromCurrentExchange2010Version(sourceServerVersion.Major, sourceServerVersion.Minor, sourceServerVersion.Build, sourceServerVersion.Revision));
			}
			return result;
		}

		private static readonly int E12MinVersionNumber = new ServerVersion(8, 3, 385, 0).ToInt();

		private static readonly int E14MinVersionNumber = new ServerVersion(14, 3, 215, 0).ToInt();
	}
}
