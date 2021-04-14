using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.ApplicationLogic.Extension;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Services.Core
{
	internal sealed class SetClientExtension : MultiStepServiceCommand<SetClientExtensionRequest, ServiceResultNone>
	{
		public SetClientExtension(CallContext callContext, SetClientExtensionRequest request) : base(callContext, request)
		{
		}

		internal override int StepCount
		{
			get
			{
				return base.Request.Actions.Length;
			}
		}

		internal override IExchangeWebMethodResponse GetResponse()
		{
			SetClientExtensionResponse setClientExtensionResponse = new SetClientExtensionResponse();
			setClientExtensionResponse.BuildForNoReturnValue(base.Results);
			return setClientExtensionResponse;
		}

		internal override ServiceResult<ServiceResultNone> Execute()
		{
			SetClientExtensionAction action = base.Request.Actions[base.CurrentStep];
			if (action.ActionId == SetClientExtensionActionId.Uninstall)
			{
				return UninstallApp.InternalExecute(base.CallContext, false, OrgEmptyMasterTableCache.Singleton, action.ExtensionId);
			}
			if (action.ActionId == SetClientExtensionActionId.Install)
			{
				return InstallApp.InternalExecute(base.CallContext, false, OrgEmptyMasterTableCache.Singleton, action.ClientExtension.Manifest, delegate(ExtensionData extensionData)
				{
					extensionData.Enabled = action.ClientExtension.IsAvailable;
					extensionData.Type = new ExtensionType?(action.ClientExtension.Type);
					extensionData.Scope = new ExtensionInstallScope?(action.ClientExtension.Scope);
					extensionData.MarketplaceAssetID = action.ClientExtension.MarketplaceAssetId;
					extensionData.MarketplaceContentMarket = action.ClientExtension.MarketplaceContentMarket;
					extensionData.IsEnabledByDefault = action.ClientExtension.IsEnabledByDefault;
					extensionData.IsMandatory = action.ClientExtension.IsMandatory;
					extensionData.ProvidedTo = action.ClientExtension.ProvidedTo;
					extensionData.SpecificUsers = action.ClientExtension.SpecificUsers;
					extensionData.Etoken = action.ClientExtension.Etoken;
				});
			}
			if (action.ActionId == SetClientExtensionActionId.Configure)
			{
				ServiceError serviceError = GetExtensibilityContext.RunClientExtensionAction(delegate
				{
					MailboxSession mailboxIdentityMailboxSession = this.CallContext.SessionCache.GetMailboxIdentityMailboxSession();
					using (InstalledExtensionTable installedExtensionTable = InstalledExtensionTable.CreateInstalledExtensionTable(null, false, OrgEmptyMasterTableCache.Singleton, mailboxIdentityMailboxSession))
					{
						installedExtensionTable.ConfigureOrgExtension(action.ExtensionId, action.ClientExtension.IsAvailable, action.ClientExtension.IsMandatory, action.ClientExtension.IsEnabledByDefault, action.ClientExtension.ProvidedTo, action.ClientExtension.SpecificUsers);
					}
				});
				if (serviceError != null)
				{
					return new ServiceResult<ServiceResultNone>(serviceError);
				}
			}
			return new ServiceResult<ServiceResultNone>(new ServiceResultNone());
		}
	}
}
