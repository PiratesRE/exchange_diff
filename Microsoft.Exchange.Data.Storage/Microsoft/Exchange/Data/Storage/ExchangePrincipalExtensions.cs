using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class ExchangePrincipalExtensions
	{
		public static IMailboxInfo GetPrimaryMailbox(this IExchangePrincipal exchangePrincipal)
		{
			return exchangePrincipal.AllMailboxes.FirstOrDefault((IMailboxInfo mailbox) => !mailbox.IsArchive && !mailbox.IsAggregated);
		}

		public static IMailboxInfo GetArchiveMailbox(this IExchangePrincipal exchangePrincipal)
		{
			return exchangePrincipal.AllMailboxes.FirstOrDefault((IMailboxInfo mailbox) => mailbox.IsArchive);
		}

		public static IMailboxInfo GetAggregatedMailbox(this IExchangePrincipal exchangePrincipal, Guid aggregatedMailboxGuid)
		{
			ArgumentValidator.ThrowIfEmpty("aggregatedMailboxGuid", aggregatedMailboxGuid);
			return exchangePrincipal.AllMailboxes.FirstOrDefault((IMailboxInfo mailbox) => mailbox.IsAggregated && mailbox.MailboxGuid == aggregatedMailboxGuid);
		}

		public static ExchangePrincipal GetAggregatedExchangePrincipal(this ExchangePrincipal exchangePrincipal, Guid aggregatedMailboxGuid)
		{
			if (exchangePrincipal.MailboxInfo.IsAggregated)
			{
				throw new InvalidOperationException("Cannot get aggregated mailbox of an aggregated ExchangePrincipal");
			}
			if (exchangePrincipal.MailboxInfo.IsArchive)
			{
				throw new InvalidOperationException("Cannot get aggregated mailbox of an archive ExchangePrincipal");
			}
			if (exchangePrincipal.AggregatedMailboxGuids.All((Guid mailboxGuid) => aggregatedMailboxGuid != mailboxGuid))
			{
				throw new InvalidOperationException("Invalid aggregated mailbox Guid used");
			}
			return ExchangePrincipalExtensions.CloneExchangePrincipal(exchangePrincipal, false, new Guid?(aggregatedMailboxGuid), null);
		}

		public static ExchangePrincipal GetArchiveExchangePrincipal(this ExchangePrincipal exchangePrincipal)
		{
			return ExchangePrincipalExtensions.GetArchiveExchangePrincipalInternal(exchangePrincipal, null);
		}

		public static ExchangePrincipal GetArchiveExchangePrincipal(this ExchangePrincipal exchangePrincipal, RemotingOptions remotingOptions)
		{
			EnumValidator.ThrowIfInvalid<RemotingOptions>(remotingOptions, "remotingOptions");
			return ExchangePrincipalExtensions.GetArchiveExchangePrincipalInternal(exchangePrincipal, new RemotingOptions?(remotingOptions));
		}

		public static ICollection<string> GetAllEmailAddresses(this IExchangePrincipal exchangePrincipal)
		{
			Util.ThrowOnNullArgument(exchangePrincipal, "exchangePrincipal");
			int num = exchangePrincipal.MailboxInfo.EmailAddresses.Count<ProxyAddress>();
			if (exchangePrincipal.MailboxInfo.EmailAddresses == null || num == 0)
			{
				return new string[]
				{
					exchangePrincipal.MailboxInfo.PrimarySmtpAddress.ToString()
				};
			}
			string[] array = new string[1 + num];
			array[0] = exchangePrincipal.MailboxInfo.PrimarySmtpAddress.ToString();
			int num2 = 1;
			foreach (ProxyAddress proxyAddress in exchangePrincipal.MailboxInfo.EmailAddresses)
			{
				array[num2] = proxyAddress.ToString();
				num2++;
			}
			return array;
		}

		private static ExchangePrincipal GetArchiveExchangePrincipalInternal(ExchangePrincipal exchangePrincipal, RemotingOptions? remotingOptions)
		{
			if (exchangePrincipal.MailboxInfo.IsAggregated)
			{
				throw new InvalidOperationException("Cannot get archive mailbox of an aggregated ExchangePrincipal");
			}
			if (exchangePrincipal.MailboxInfo.IsArchive)
			{
				throw new InvalidOperationException("Cannot get archive mailbox of an archive ExchangePrincipal");
			}
			return ExchangePrincipalExtensions.CloneExchangePrincipal(exchangePrincipal, true, null, remotingOptions);
		}

		private static ExchangePrincipal CloneExchangePrincipal(ExchangePrincipal sourceExchangePrincipal, bool asArchive, Guid? aggregatedMailboxGuid, RemotingOptions? remotingOptions)
		{
			IMailboxInfo selectedMailboxInfo;
			if (asArchive)
			{
				selectedMailboxInfo = sourceExchangePrincipal.GetArchiveMailbox();
			}
			else if (aggregatedMailboxGuid != null && aggregatedMailboxGuid != Guid.Empty)
			{
				selectedMailboxInfo = sourceExchangePrincipal.GetAggregatedMailbox(aggregatedMailboxGuid.Value);
			}
			else
			{
				selectedMailboxInfo = sourceExchangePrincipal.MailboxInfo;
			}
			return sourceExchangePrincipal.WithSelectedMailbox(selectedMailboxInfo, remotingOptions);
		}
	}
}
