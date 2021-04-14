using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.InfoWorker.Common.MailTips
{
	internal sealed class MailTipsPermission
	{
		internal bool AccessEnabled { get; private set; }

		internal MailTipsAccessLevel AccessLevel { get; private set; }

		internal bool InAccessScope { get; private set; }

		internal MailTipsPermission(bool mailTipsAccessEnabled, MailTipsAccessLevel mailTipsAccessLevel, bool requesterInAccessScope)
		{
			this.AccessEnabled = mailTipsAccessEnabled;
			this.AccessLevel = mailTipsAccessLevel;
			this.InAccessScope = requesterInAccessScope;
		}

		internal bool CanAccessAMailTip()
		{
			return this.InAccessScope && this.AccessEnabled && MailTipsAccessLevel.None != this.AccessLevel;
		}

		internal static readonly MailTipsPermission AllAccess = new MailTipsPermission(true, MailTipsAccessLevel.All, true);

		internal static MailTipsPermission NoAccess = new MailTipsPermission(false, MailTipsAccessLevel.None, false);
	}
}
