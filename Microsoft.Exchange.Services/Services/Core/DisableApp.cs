using System;
using Microsoft.Exchange.Data.ApplicationLogic;
using Microsoft.Exchange.Data.ApplicationLogic.Extension;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Services.Core
{
	internal sealed class DisableApp : SingleStepServiceCommand<DisableAppRequest, ServiceResultNone>
	{
		public DisableApp(CallContext callContext, DisableAppRequest request) : base(callContext, request)
		{
			this.Id = request.ID;
			ServiceCommandBase.ThrowIfNull(this.Id, "Id", "DisableApp::ctor");
			this.DisableReason = request.DisableReason;
			ServiceCommandBase.ThrowIfNull(this.DisableReason, "DisableReason", "DisableApp::ctor");
		}

		internal override IExchangeWebMethodResponse GetResponse()
		{
			return new DisableAppResponse(base.Result.Code, base.Result.Error);
		}

		internal override ServiceResult<ServiceResultNone> Execute()
		{
			OrgEmptyMasterTableCache orgEmptyMailboxSessionCache = null;
			bool isUserScope = true;
			bool cannotDisableMandatoryExtension = false;
			bool extensionNotFound = false;
			ServiceError serviceError = GetExtensibilityContext.RunClientExtensionAction(delegate
			{
				MailboxSession mailboxIdentityMailboxSession = this.CallContext.SessionCache.GetMailboxIdentityMailboxSession();
				using (InstalledExtensionTable installedExtensionTable = InstalledExtensionTable.CreateInstalledExtensionTable(null, isUserScope, orgEmptyMailboxSessionCache, mailboxIdentityMailboxSession))
				{
					try
					{
						installedExtensionTable.DisableExtension(this.Id, this.DisableReason);
						RequestDetailsLogger.Current.AppendGenericInfo("DisableApp", this.Id);
					}
					catch (CannotDisableMandatoryExtensionException ex)
					{
						cannotDisableMandatoryExtension = true;
						RequestDetailsLoggerBase<RequestDetailsLogger>.SafeLogRequestException(this.CallContext.ProtocolLog, ex, "DisableApp_Execute");
					}
					catch (ExtensionNotFoundException ex2)
					{
						extensionNotFound = true;
						RequestDetailsLoggerBase<RequestDetailsLogger>.SafeLogRequestException(this.CallContext.ProtocolLog, ex2, "DisableApp_Execute");
					}
				}
			});
			if (cannotDisableMandatoryExtension)
			{
				serviceError = new ServiceError(CoreResources.IDs.ErrorCannotDisableMandatoryExtension, ResponseCodeType.ErrorCannotDisableMandatoryExtension, 0, ExchangeVersion.Exchange2012);
			}
			else if (extensionNotFound)
			{
				serviceError = new ServiceError(CoreResources.ErrorExtensionNotFound(this.Id), ResponseCodeType.ErrorExtensionNotFound, 0, ExchangeVersion.Exchange2012);
			}
			if (serviceError != null)
			{
				return new ServiceResult<ServiceResultNone>(serviceError);
			}
			return new ServiceResult<ServiceResultNone>(new ServiceResultNone());
		}

		private readonly string Id;

		private readonly DisableReasonType DisableReason;
	}
}
