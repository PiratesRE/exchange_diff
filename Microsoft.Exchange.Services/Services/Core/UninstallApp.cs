using System;
using Microsoft.Exchange.Data.ApplicationLogic.Extension;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Services.Core
{
	internal sealed class UninstallApp : SingleStepServiceCommand<UninstallAppRequest, ServiceResultNone>
	{
		public UninstallApp(CallContext callContext, UninstallAppRequest request) : base(callContext, request)
		{
			this.ID = request.ID;
			ServiceCommandBase.ThrowIfNull(this.ID, "ID", "UninstallApp::ctor");
		}

		internal static ServiceResult<ServiceResultNone> InternalExecute(CallContext callContext, bool isUserScope, OrgEmptyMasterTableCache orgEmptyMasterTableCache, string extensionId)
		{
			ServiceError serviceError = GetExtensibilityContext.RunClientExtensionAction(delegate
			{
				MailboxSession mailboxIdentityMailboxSession = callContext.SessionCache.GetMailboxIdentityMailboxSession();
				using (InstalledExtensionTable installedExtensionTable = InstalledExtensionTable.CreateInstalledExtensionTable(null, isUserScope, orgEmptyMasterTableCache, mailboxIdentityMailboxSession))
				{
					installedExtensionTable.UninstallExtension(extensionId);
				}
			});
			if (serviceError != null)
			{
				return new ServiceResult<ServiceResultNone>(serviceError);
			}
			return new ServiceResult<ServiceResultNone>(new ServiceResultNone());
		}

		internal override IExchangeWebMethodResponse GetResponse()
		{
			return new UninstallAppResponse(base.Result.Code, base.Result.Error);
		}

		internal override ServiceResult<ServiceResultNone> Execute()
		{
			return UninstallApp.InternalExecute(base.CallContext, true, null, this.ID);
		}

		private readonly string ID;
	}
}
