using System;
using Microsoft.Exchange.AirSync;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.EventLogs;

namespace Microsoft.Exchange.Services.Core
{
	internal sealed class Provision : BaseProvisionCommand<ProvisionRequest, ProvisionResponse>
	{
		public Provision(CallContext callContext, ProvisionRequest request) : base(callContext, request, request.HasPAL, request.DeviceType, request.DeviceID, request.SpecifyProtocol, request.Protocol)
		{
		}

		protected override void InternalExecute()
		{
			using (SyncStateStorage syncStateStorage = SyncStateStorage.Create(base.MailboxIdentityMailboxSession, new DeviceIdentity(base.Request.DeviceID, base.Request.DeviceType, base.Protocol), StateStorageFeatures.ContentState, null))
			{
				using (GlobalInfo globalInfo = GlobalInfo.LoadFromMailbox(base.MailboxIdentityMailboxSession, syncStateStorage, null))
				{
					globalInfo.DeviceActiveSyncVersion = base.Request.ClientVersion;
					globalInfo.DeviceFriendlyName = base.Request.DeviceFriendlyName;
					globalInfo.DeviceImei = base.Request.DeviceImei;
					globalInfo.DeviceMobileOperator = base.Request.DeviceMobileOperator;
					globalInfo.DeviceModel = base.Request.DeviceModel;
					globalInfo.DeviceOS = base.Request.DeviceOS;
					globalInfo.DeviceOSLanguage = base.Request.DeviceOSLanguage;
					globalInfo.DevicePhoneNumber = base.Request.DevicePhoneNumber;
					globalInfo.UserAgent = base.CallContext.UserAgent;
					globalInfo.UserADObjectId = base.CallContext.AccessingPrincipal.ObjectId;
					if (base.Request.HasPAL)
					{
						ADDeviceManager addeviceManager = new ADDeviceManager(syncStateStorage.DeviceIdentity, MobileClientType.MOWA, base.CallContext.AccessingPrincipal.MailboxInfo.OrganizationId, base.CallContext.HttpContext.Request.ServerVariables["LOGON_USER"], base.CallContext.AccessingPrincipal.ObjectId, base.CallContext.ADRecipientSessionContext.GetADRecipientSession().FindADUserByObjectId(base.CallContext.AccessingPrincipal.ObjectId), null, base.CallContext.Budget, ExTraceGlobals.ProvisionCallTracer, ServicesEventLogConstants.Tuple_UnableToCreateADDevice, ServicesEventLogConstants.Tuple_DirectoryAccessDenied);
						addeviceManager.CreateMobileDevice(globalInfo, syncStateStorage.CreationTime, true, base.MailboxIdentityMailboxSession);
						MobileDevice mobileDevice = addeviceManager.GetMobileDevice();
						globalInfo.DeviceADObjectId = mobileDevice.Id;
					}
					globalInfo.SaveToMailbox();
				}
			}
		}
	}
}
