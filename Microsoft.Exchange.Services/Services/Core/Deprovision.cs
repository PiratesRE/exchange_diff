using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	internal sealed class Deprovision : BaseProvisionCommand<DeprovisionRequest, DeprovisionResponse>
	{
		public Deprovision(CallContext callContext, DeprovisionRequest request) : base(callContext, request, request.HasPAL, request.DeviceType, request.DeviceID, request.SpecifyProtocol, request.Protocol)
		{
		}

		protected override void InternalExecute()
		{
			SyncStateStorage.DeleteSyncStateStorage(base.MailboxIdentityMailboxSession, new DeviceIdentity(base.Request.DeviceID, base.Request.DeviceType, base.Protocol), null);
		}
	}
}
