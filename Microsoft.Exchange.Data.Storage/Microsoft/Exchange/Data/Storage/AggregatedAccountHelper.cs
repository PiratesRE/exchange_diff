using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class AggregatedAccountHelper
	{
		public Guid AccountMailboxGuid { get; set; }

		public AggregatedAccountHelper()
		{
		}

		public AggregatedAccountHelper(MailboxSession session, ADUser adUser)
		{
			Util.ThrowOnNullArgument(session, "session");
			Util.ThrowOnNullArgument(adUser, "adUser");
			this.session = session;
			this.adUser = adUser;
		}

		public virtual AggregatedAccountInfo AddAccount(SmtpAddress smtpAddress, Guid aggregatedMailboxGuid, Guid requestGuid)
		{
			Util.ThrowOnNullArgument(smtpAddress, "smtpAddress");
			AggregatedAccountListConfiguration aggregatedAccountListConfiguration = new AggregatedAccountListConfiguration();
			aggregatedAccountListConfiguration.Principal = ExchangePrincipal.FromADUser(this.adUser, null);
			MailboxStoreTypeProvider mailboxStoreTypeProvider = new MailboxStoreTypeProvider(this.adUser)
			{
				MailboxSession = this.session
			};
			this.AccountMailboxGuid = aggregatedMailboxGuid;
			AggregatedAccountInfo result = new AggregatedAccountInfo(this.AccountMailboxGuid, smtpAddress, requestGuid);
			aggregatedAccountListConfiguration.RequestGuid = requestGuid;
			aggregatedAccountListConfiguration.SmtpAddress = smtpAddress;
			aggregatedAccountListConfiguration.AggregatedMailboxGuid = this.AccountMailboxGuid;
			aggregatedAccountListConfiguration.Save(mailboxStoreTypeProvider);
			return result;
		}

		public virtual AggregatedAccountInfo GetAccount(SmtpAddress smtpAddress)
		{
			Util.ThrowOnNullArgument(smtpAddress, "smtpAddress");
			List<AggregatedAccountInfo> listOfAccounts = this.GetListOfAccounts();
			return listOfAccounts.FirstOrDefault((AggregatedAccountInfo o) => StringComparer.OrdinalIgnoreCase.Equals(o.SmtpAddress.ToString(), smtpAddress.ToString()));
		}

		public virtual List<AggregatedAccountInfo> GetListOfAccounts()
		{
			AggregatedAccountListConfiguration aggregatedAccountListConfiguration = new AggregatedAccountListConfiguration
			{
				Principal = ExchangePrincipal.FromADUser(this.adUser, null)
			};
			MailboxStoreTypeProvider mailboxStoreTypeProvider = new MailboxStoreTypeProvider(this.adUser)
			{
				MailboxSession = this.session
			};
			aggregatedAccountListConfiguration = (aggregatedAccountListConfiguration.Read(mailboxStoreTypeProvider, null) as AggregatedAccountListConfiguration);
			if (aggregatedAccountListConfiguration.AggregatedAccountList == null)
			{
				return new List<AggregatedAccountInfo>();
			}
			return aggregatedAccountListConfiguration.AggregatedAccountList;
		}

		public virtual void RemoveAccount(Guid mailboxGuid)
		{
			Util.ThrowOnNullArgument(mailboxGuid, "id");
			AggregatedAccountListConfiguration aggregatedAccountListConfiguration = new AggregatedAccountListConfiguration();
			aggregatedAccountListConfiguration.Principal = ExchangePrincipal.FromADUser(this.adUser, null);
			MailboxStoreTypeProvider mailboxStoreTypeProvider = new MailboxStoreTypeProvider(this.adUser)
			{
				MailboxSession = this.session
			};
			List<AggregatedAccountInfo> listOfAccounts = this.GetListOfAccounts();
			AggregatedAccountInfo aggregatedAccountInfo = listOfAccounts.Find((AggregatedAccountInfo o) => o.MailboxGuid == mailboxGuid);
			if (aggregatedAccountInfo == null)
			{
				return;
			}
			aggregatedAccountListConfiguration.RequestGuid = aggregatedAccountInfo.RequestGuid;
			aggregatedAccountListConfiguration.Delete(mailboxStoreTypeProvider);
		}

		public virtual void RemoveAccount(SmtpAddress smtpAddress)
		{
			Util.ThrowOnNullArgument(smtpAddress, "smtpAddress");
			AggregatedAccountListConfiguration aggregatedAccountListConfiguration = new AggregatedAccountListConfiguration();
			aggregatedAccountListConfiguration.Principal = ExchangePrincipal.FromADUser(this.adUser, null);
			MailboxStoreTypeProvider mailboxStoreTypeProvider = new MailboxStoreTypeProvider(this.adUser)
			{
				MailboxSession = this.session
			};
			List<AggregatedAccountInfo> listOfAccounts = this.GetListOfAccounts();
			AggregatedAccountInfo aggregatedAccountInfo = listOfAccounts.Find((AggregatedAccountInfo o) => o.SmtpAddress == smtpAddress);
			if (aggregatedAccountInfo == null)
			{
				return;
			}
			aggregatedAccountListConfiguration.RequestGuid = aggregatedAccountInfo.RequestGuid;
			aggregatedAccountListConfiguration.Delete(mailboxStoreTypeProvider);
		}

		private readonly MailboxSession session;

		private readonly ADUser adUser;
	}
}
