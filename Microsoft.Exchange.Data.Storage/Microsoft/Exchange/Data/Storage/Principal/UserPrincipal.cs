using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Principal
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class UserPrincipal : ExchangePrincipal, IUserPrincipal, IExchangePrincipal
	{
		public UserPrincipal(IGenericADUser adUser, IEnumerable<IMailboxInfo> allMailboxes, Func<IMailboxInfo, bool> mailboxSelector, RemotingOptions remotingOptions) : base(adUser, allMailboxes, mailboxSelector, remotingOptions)
		{
			this.UserPrincipalName = adUser.UserPrincipalName;
			this.WindowsLiveId = adUser.WindowsLiveID;
			this.NetId = adUser.NetId;
		}

		private UserPrincipal(UserPrincipal sourceExchangePrincipal) : base(sourceExchangePrincipal)
		{
			this.UserPrincipalName = sourceExchangePrincipal.UserPrincipalName;
			this.WindowsLiveId = sourceExchangePrincipal.WindowsLiveId;
			this.NetId = sourceExchangePrincipal.NetId;
		}

		public string UserPrincipalName { get; private set; }

		public SmtpAddress WindowsLiveId { get; private set; }

		public NetID NetId { get; private set; }

		public override string PrincipalId
		{
			get
			{
				if (!string.IsNullOrEmpty(this.UserPrincipalName))
				{
					return this.UserPrincipalName.Split(new char[]
					{
						'@'
					})[0];
				}
				if (base.MailboxInfo.PrimarySmtpAddress.IsValidAddress && base.MailboxInfo.PrimarySmtpAddress.Local != null)
				{
					return base.MailboxInfo.PrimarySmtpAddress.Local;
				}
				return string.Empty;
			}
		}

		protected override ExchangePrincipal Clone()
		{
			return new UserPrincipal(this);
		}
	}
}
