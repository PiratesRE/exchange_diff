using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.InfoWorker.Common.OOF;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class SetUserOofSettings : ServiceCommand<bool>
	{
		public SetUserOofSettings(CallContext callContext, UserOofSettingsType userOofSettings) : base(callContext)
		{
			this.userOofSettings = userOofSettings;
		}

		protected override bool InternalExecute()
		{
			MailboxSession mailboxIdentityMailboxSession = base.CallContext.SessionCache.GetMailboxIdentityMailboxSession();
			UserOofSettings userOofSettings = UserOofSettings.GetUserOofSettings(mailboxIdentityMailboxSession);
			userOofSettings.OofState = (this.userOofSettings.IsOofOn ? OofState.Enabled : OofState.Disabled);
			userOofSettings.ExternalAudience = this.userOofSettings.ExternalAudience;
			userOofSettings.InternalReply.Message = UserOofSettingsType.ConvertTextToHtml(this.userOofSettings.InternalReply);
			userOofSettings.ExternalReply.Message = UserOofSettingsType.ConvertTextToHtml(this.userOofSettings.ExternalReply);
			userOofSettings.Save(mailboxIdentityMailboxSession);
			return true;
		}

		private UserOofSettingsType userOofSettings;
	}
}
