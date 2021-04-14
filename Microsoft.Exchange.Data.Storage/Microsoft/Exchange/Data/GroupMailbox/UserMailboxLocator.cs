using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.GroupMailbox
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class UserMailboxLocator : MailboxLocator
	{
		public UserMailboxLocator(IRecipientSession adSession, string externalDirectoryObjectId, string legacyDn) : base(adSession, externalDirectoryObjectId, legacyDn)
		{
		}

		private UserMailboxLocator(IRecipientSession adSession) : base(adSession)
		{
		}

		public override string LocatorType
		{
			get
			{
				return UserMailboxLocator.MailboxLocatorType;
			}
		}

		public static UserMailboxLocator Instantiate(IRecipientSession adSession, ProxyAddress proxyAddress)
		{
			UserMailboxLocator userMailboxLocator = new UserMailboxLocator(adSession);
			userMailboxLocator.InitializeFromAd(proxyAddress);
			return userMailboxLocator;
		}

		public static List<UserMailboxLocator> Instantiate(IRecipientSession adSession, params ProxyAddress[] proxyAddresses)
		{
			ArgumentValidator.ThrowIfNull("adSession", adSession);
			ArgumentValidator.ThrowIfNull("proxyAddresses", proxyAddresses);
			ArgumentValidator.ThrowIfZeroOrNegative("proxyAddresses.Length", proxyAddresses.Length);
			Result<ADUser>[] array = adSession.FindByProxyAddresses<ADUser>(proxyAddresses);
			List<UserMailboxLocator> list = new List<UserMailboxLocator>(proxyAddresses.Length);
			if (array == null)
			{
				MailboxLocator.Tracer.TraceDebug(0L, "UserMailboxLocator::Instantiate. FindByProxyAddresses returned no results");
				return null;
			}
			for (int i = 0; i < proxyAddresses.Length; i++)
			{
				Result<ADUser> result = array[i];
				if (result.Data == null)
				{
					MailboxLocator.Tracer.TraceError<string, ProviderError>(0L, "UserMailboxLocator::Instantiate. FindByProxyAddresses returned error for address {0}. Error: {1}", proxyAddresses[i].ProxyAddressString, result.Error);
					throw new MailboxNotFoundException(ServerStrings.InvalidAddressError(proxyAddresses[i].ProxyAddressString));
				}
				UserMailboxLocator userMailboxLocator = new UserMailboxLocator(adSession);
				userMailboxLocator.InitializeFromAd(result.Data);
				list.Add(userMailboxLocator);
				MailboxLocator.Tracer.TraceDebug<string, UserMailboxLocator>(0L, "UserMailboxLocator::Instantiate. FindByProxyAddresses found user. Address: {0}. Locator: {1}", proxyAddresses[i].ProxyAddressString, userMailboxLocator);
			}
			return list;
		}

		public static List<UserMailboxLocator> Instantiate(IRecipientSession adSession, params ADUser[] users)
		{
			ArgumentValidator.ThrowIfNull("adSession", adSession);
			ArgumentValidator.ThrowIfNull("users", users);
			ArgumentValidator.ThrowIfZeroOrNegative("users.Length", users.Length);
			List<UserMailboxLocator> list = new List<UserMailboxLocator>(users.Length);
			foreach (ADUser adUser in users)
			{
				UserMailboxLocator userMailboxLocator = new UserMailboxLocator(adSession);
				userMailboxLocator.InitializeFromAd(adUser);
				list.Add(userMailboxLocator);
			}
			return list;
		}

		public static UserMailboxLocator Instantiate(IRecipientSession adSession, ADUser adUser)
		{
			UserMailboxLocator userMailboxLocator = new UserMailboxLocator(adSession);
			userMailboxLocator.InitializeFromAd(adUser);
			return userMailboxLocator;
		}

		public override bool IsValidReplicationTarget()
		{
			ADUser aduser = base.FindAdUser();
			return aduser.RecipientTypeDetails == RecipientTypeDetails.UserMailbox;
		}

		protected override bool IsValidAdUser(ADUser adUser)
		{
			return MailboxLocatorValidator.IsValidUserLocator(adUser);
		}

		public static readonly string MailboxLocatorType = "User Mailbox";
	}
}
