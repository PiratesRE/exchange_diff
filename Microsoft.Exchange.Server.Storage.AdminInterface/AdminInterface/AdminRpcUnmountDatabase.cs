using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Protocols.MAPI;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Server.Storage.AdminInterface
{
	internal class AdminRpcUnmountDatabase : AdminRpc
	{
		public AdminRpcUnmountDatabase(ClientSecurityContext callerSecurityContext, Guid mdbGuid, uint flags, byte[] auxiliaryIn) : base(AdminMethod.EcUnmountDatabase50, callerSecurityContext, auxiliaryIn)
		{
			base.MdbGuid = new Guid?(mdbGuid);
		}

		protected override ErrorCode EcCheckPermissions(MapiContext context)
		{
			return AdminRpcPermissionChecks.EcCheckConstrainedDelegationRights(context, base.DatabaseInfo);
		}

		protected override ErrorCode EcExecuteRpc(MapiContext context)
		{
			ErrorCode errorCode = Storage.DismountDatabase(context, base.MdbGuid.Value);
			if (errorCode != ErrorCode.NoError)
			{
				errorCode = errorCode.Propagate((LID)17312U);
			}
			return errorCode;
		}
	}
}
