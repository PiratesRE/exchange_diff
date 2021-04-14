using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Principal
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class RemoteUserMailboxPrincipal : ExchangePrincipal, IUserPrincipal, IExchangePrincipal
	{
		public RemoteUserMailboxPrincipal(IGenericADUser adUser, IEnumerable<IMailboxInfo> allMailboxes, Func<IMailboxInfo, bool> mailboxSelector, RemotingOptions remotingOptions) : base(adUser, allMailboxes, mailboxSelector, remotingOptions)
		{
			this.UserPrincipalName = adUser.UserPrincipalName;
			this.WindowsLiveId = adUser.WindowsLiveID;
			this.NetId = adUser.NetId;
			this.PrimarySmtpAddress = adUser.PrimarySmtpAddress;
			this.DisplayName = adUser.DisplayName;
		}

		private RemoteUserMailboxPrincipal(RemoteUserMailboxPrincipal sourceExchangePrincipal) : base(sourceExchangePrincipal)
		{
			this.UserPrincipalName = sourceExchangePrincipal.UserPrincipalName;
			this.WindowsLiveId = sourceExchangePrincipal.WindowsLiveId;
			this.NetId = sourceExchangePrincipal.NetId;
			this.PrimarySmtpAddress = sourceExchangePrincipal.PrimarySmtpAddress;
			this.DisplayName = sourceExchangePrincipal.DisplayName;
		}

		public string UserPrincipalName { get; private set; }

		public SmtpAddress WindowsLiveId { get; private set; }

		public NetID NetId { get; private set; }

		public SmtpAddress PrimarySmtpAddress { get; private set; }

		public string DisplayName { get; private set; }

		public override bool IsMailboxInfoRequired
		{
			get
			{
				return false;
			}
		}

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
				if (this.PrimarySmtpAddress.IsValidAddress && this.PrimarySmtpAddress.Local != null)
				{
					return this.PrimarySmtpAddress.Local;
				}
				return string.Empty;
			}
		}

		protected override ExchangePrincipal Clone()
		{
			return new RemoteUserMailboxPrincipal(this);
		}
	}
}
