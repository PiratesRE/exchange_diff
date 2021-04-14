using System;
using System.IO;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.ApplicationLogic.Extension;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Services.Core
{
	internal sealed class InstallApp : SingleStepServiceCommand<InstallAppRequest, ServiceResultNone>
	{
		public InstallApp(CallContext callContext, InstallAppRequest request) : base(callContext, request)
		{
			this.manifestString = request.Manifest;
			ServiceCommandBase.ThrowIfNull(this.manifestString, "manifestString", "InstallApp::ctor");
		}

		internal static ServiceResult<ServiceResultNone> InternalExecute(CallContext callContext, bool isUserScope, OrgEmptyMasterTableCache orgEmptyMasterTableCache, string manifestString, Action<ExtensionData> configure)
		{
			ServiceError serviceError = GetExtensibilityContext.RunClientExtensionAction(delegate
			{
				ExtensionData extensionData;
				try
				{
					byte[] buffer = Convert.FromBase64String(manifestString);
					using (Stream stream = new MemoryStream(buffer))
					{
						ExtensionData.ValidateManifestSize(stream.Length, true);
						extensionData = ExtensionData.ParseOsfManifest(stream, null, null, ExtensionType.Private, isUserScope ? ExtensionInstallScope.User : ExtensionInstallScope.Organization, true, DisableReasonType.NotDisabled, string.Empty, null);
					}
				}
				catch (FormatException innerException)
				{
					throw new OwaExtensionOperationException(innerException);
				}
				if (configure != null)
				{
					configure(extensionData);
				}
				MailboxSession mailboxIdentityMailboxSession = callContext.SessionCache.GetMailboxIdentityMailboxSession();
				using (InstalledExtensionTable installedExtensionTable = InstalledExtensionTable.CreateInstalledExtensionTable(null, isUserScope, orgEmptyMasterTableCache, mailboxIdentityMailboxSession))
				{
					installedExtensionTable.InstallExtension(extensionData, isUserScope);
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
			return new InstallAppResponse(base.Result.Code, base.Result.Error);
		}

		internal override ServiceResult<ServiceResultNone> Execute()
		{
			return InstallApp.InternalExecute(base.CallContext, true, null, this.manifestString, null);
		}

		private readonly string manifestString;
	}
}
