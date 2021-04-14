using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	internal class SetImListMigrationCompleted : SingleStepServiceCommand<SetImListMigrationCompletedRequest, ServiceResultNone>
	{
		public SetImListMigrationCompleted(CallContext callContext, SetImListMigrationCompletedRequest request) : base(callContext, request)
		{
		}

		internal override IExchangeWebMethodResponse GetResponse()
		{
			return new SetImListMigrationCompletedResponseMessage(base.Result.Code, base.Result.Error);
		}

		internal override ServiceResult<ServiceResultNone> Execute()
		{
			this.ThrowIfNull(base.CallContext.ADRecipientSessionContext, "this.CallContext.ADRecipientSessionContext");
			IRecipientSession adrecipientSession = base.CallContext.ADRecipientSessionContext.GetADRecipientSession();
			this.ThrowIfNull(adrecipientSession, "readonlySession");
			IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(false, ConsistencyMode.IgnoreInvalid, adrecipientSession.SessionSettings, 54, "Execute", "f:\\15.00.1497\\sources\\dev\\services\\src\\Core\\servicecommands\\SetImListMigrationCompleted.cs");
			this.ThrowIfNull(tenantOrRootOrgRecipientSession, "readWriteSession");
			this.ThrowIfNull(base.CallContext.AccessingPrincipal, "this.CallContext.AccessingPrincipal");
			ADRecipient adrecipient = tenantOrRootOrgRecipientSession.Read(base.CallContext.AccessingPrincipal.ObjectId);
			this.ThrowIfNull(adrecipient, "recipient");
			this.ThrowIfNull(base.Request, "this.Request");
			adrecipient.UCSImListMigrationCompleted = base.Request.ImListMigrationCompleted;
			RequestDetailsLoggerBase<RequestDetailsLogger>.SafeAppendGenericInfo(RequestDetailsLogger.Current, "migrated", base.Request.ImListMigrationCompleted);
			tenantOrRootOrgRecipientSession.Save(adrecipient);
			return new ServiceResult<ServiceResultNone>(new ServiceResultNone());
		}

		private void ThrowIfNull(object var, string varName)
		{
			if (var != null)
			{
				return;
			}
			string text = string.Empty;
			object obj = null;
			string text2 = string.Empty;
			if (base.CallContext.AccessingPrincipal != null)
			{
				text = base.CallContext.AccessingPrincipal.Alias;
				obj = base.CallContext.AccessingPrincipal.MailboxInfo.PrimarySmtpAddress;
			}
			else if (base.CallContext.AccessingADUser != null)
			{
				text = base.CallContext.AccessingADUser.Alias;
				obj = base.CallContext.AccessingADUser.UserPrincipalName;
				text2 = base.CallContext.AccessingADUser.WindowsLiveID.ToString();
			}
			RequestDetailsLoggerBase<RequestDetailsLogger>.SafeAppendGenericError(RequestDetailsLogger.Current, "failedImMigration", string.Format("{0} is null: alias={1},principalName={2},windowsLiveId={3}", new object[]
			{
				varName,
				text,
				obj,
				text2
			}));
			throw new ServiceInvalidOperationException(CoreResources.IDs.ErrorImListMigration);
		}
	}
}
