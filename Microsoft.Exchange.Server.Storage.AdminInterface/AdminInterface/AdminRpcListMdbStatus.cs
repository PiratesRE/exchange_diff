using System;
using Microsoft.Exchange.Protocols.MAPI;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Server.Storage.AdminInterface
{
	internal class AdminRpcListMdbStatus : AdminRpc
	{
		public AdminRpcListMdbStatus(ClientSecurityContext callerSecurityContext, Guid[] mdbGuids, byte[] auxiliaryIn) : base(AdminMethod.EcListMdbStatus50, callerSecurityContext, auxiliaryIn)
		{
			this.mdbGuids = mdbGuids;
		}

		public uint[] MdbStatus
		{
			get
			{
				return this.mdbStatus;
			}
		}

		protected override ErrorCode EcExecuteRpc(MapiContext context)
		{
			this.mdbStatus = new uint[this.mdbGuids.Length];
			for (int i = 0; i < this.mdbGuids.Length; i++)
			{
				StoreDatabase storeDatabase = Storage.FindDatabase(this.mdbGuids[i]);
				if (storeDatabase == null)
				{
					this.mdbStatus[i] = 0U;
				}
				else
				{
					storeDatabase.GetSharedLock(context.Diagnostics);
					try
					{
						this.mdbStatus[i] = storeDatabase.ExternalMdbStatus;
					}
					finally
					{
						storeDatabase.ReleaseSharedLock();
					}
				}
			}
			return ErrorCode.NoError;
		}

		private Guid[] mdbGuids;

		private uint[] mdbStatus;
	}
}
