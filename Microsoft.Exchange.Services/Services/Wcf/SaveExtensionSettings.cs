using System;
using System.Collections;
using Microsoft.Exchange.Data.ApplicationLogic.Extension;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.StoreConfigurableType;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	internal class SaveExtensionSettings : ServiceCommand<SaveExtensionSettingsResponse>
	{
		public SaveExtensionSettings(CallContext callContext, string extensionId, string extensionVersion, string settings) : base(callContext)
		{
			this.extensionId = extensionId;
			this.extensionVersion = extensionVersion;
			this.settings = settings;
		}

		protected override SaveExtensionSettingsResponse InternalExecute()
		{
			MailboxSession mailboxIdentityMailboxSession = base.CallContext.SessionCache.GetMailboxIdentityMailboxSession();
			SaveExtensionSettingsResponse result;
			using (UserConfiguration folderConfiguration = UserConfigurationHelper.GetFolderConfiguration(mailboxIdentityMailboxSession, ExtensionPackageManager.GetExtensionFolderId(mailboxIdentityMailboxSession), ExtensionPackageManager.GetFaiName(this.extensionId, this.extensionVersion), UserConfigurationTypes.Dictionary, true, false))
			{
				IDictionary dictionary = folderConfiguration.GetDictionary();
				dictionary["ExtensionSettings"] = this.settings;
				folderConfiguration.Save();
				result = new SaveExtensionSettingsResponse();
			}
			return result;
		}

		private readonly string extensionId;

		private readonly string extensionVersion;

		private readonly string settings;
	}
}
