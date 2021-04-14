using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.DirectoryServices;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Server.Storage.AdminInterface
{
	internal class AdminRpcPermissionChecks
	{
		public static ErrorCode EcDefaultCheck(Context context, DatabaseInfo databaseInfo)
		{
			return AdminRpcPermissionChecks.defaultChecker.Value.EcCheckPermissions(context, databaseInfo);
		}

		public static ErrorCode EcCheckConstrainedDelegationRights(Context context, DatabaseInfo databaseInfo)
		{
			return AdminRpcPermissionChecks.constrainedDelegationChecker.Value.EcCheckPermissions(context, databaseInfo);
		}

		internal static IDisposable SetDefaultCheckTestHook(AdminRpcPermissionChecks.IChecker testChecker)
		{
			return AdminRpcPermissionChecks.defaultChecker.SetTestHook(testChecker);
		}

		internal static IDisposable SetConstrainedDelegationCheckTestHook(AdminRpcPermissionChecks.IChecker testChecker)
		{
			return AdminRpcPermissionChecks.constrainedDelegationChecker.SetTestHook(testChecker);
		}

		private static Hookable<AdminRpcPermissionChecks.IChecker> defaultChecker = Hookable<AdminRpcPermissionChecks.IChecker>.Create(true, new AdminRpcPermissionChecks.DefaultChecker());

		private static Hookable<AdminRpcPermissionChecks.IChecker> constrainedDelegationChecker = Hookable<AdminRpcPermissionChecks.IChecker>.Create(true, new AdminRpcPermissionChecks.ConstrainedDelegationChecker());

		public interface IChecker
		{
			ErrorCode EcCheckPermissions(Context context, DatabaseInfo databaseInfo);
		}

		private class DefaultChecker : AdminRpcPermissionChecks.IChecker
		{
			public ErrorCode EcCheckPermissions(Context context, DatabaseInfo databaseInfo)
			{
				ErrorCode result = ErrorCode.NoError;
				if (!context.SecurityContext.IsAuthenticated)
				{
					result = ErrorCode.CreateNoAccess((LID)50813U);
				}
				else if (databaseInfo != null && !SecurityHelper.CheckAdministrativeRights(context.SecurityContext, databaseInfo.NTSecurityDescriptor))
				{
					result = ErrorCode.CreateNoAccess((LID)34991U);
				}
				return result;
			}
		}

		private class ConstrainedDelegationChecker : AdminRpcPermissionChecks.IChecker
		{
			public ErrorCode EcCheckPermissions(Context context, DatabaseInfo databaseInfo)
			{
				ErrorCode result = ErrorCode.NoError;
				if (!SecurityHelper.CheckConstrainedDelegationPrivilege(context.SecurityContext, Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetServerInfo(context).NTSecurityDescriptor))
				{
					result = ErrorCode.CreateNoAccess((LID)51375U);
				}
				return result;
			}
		}
	}
}
