using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.InfoWorker.Common.OOF;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class GetUserOofSettings : ServiceCommand<UserOofSettingsType>
	{
		public GetUserOofSettings(CallContext callContext, ExTimeZone timeZone) : base(callContext)
		{
			this.timeZone = timeZone;
		}

		protected override UserOofSettingsType InternalExecute()
		{
			return GetUserOofSettings.GetSetting(base.MailboxIdentityMailboxSession, this.timeZone);
		}

		public static UserOofSettingsType GetSetting(MailboxSession mailboxSession, ExTimeZone timeZone)
		{
			UserOofSettings userOofSettings = UserOofSettings.GetUserOofSettings(mailboxSession);
			return new UserOofSettingsType(userOofSettings, timeZone);
		}

		private readonly ExTimeZone timeZone;
	}
}
