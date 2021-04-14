using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class OwaOtherMailboxConfiguration
	{
		[DataMember]
		internal OtherMailboxConfigEntry[] OtherMailboxEntries
		{
			get
			{
				return this.otherMailboxEntries;
			}
		}

		internal static bool AddOtherMailboxConfig(CallContext callContext, string displayName, string primarySMTPAddress)
		{
			if (string.IsNullOrWhiteSpace(primarySMTPAddress))
			{
				return false;
			}
			UserContext userContext = UserContextManager.GetUserContext(callContext.HttpContext, callContext.EffectiveCaller, true);
			if (userContext.IsExplicitLogon)
			{
				throw new OwaInvalidRequestException("Cannot open other's folder in explict logon mode");
			}
			SmtpAddress smtpAddress = SmtpAddress.Parse(primarySMTPAddress);
			IRecipientSession adrecipientSession = CallContext.Current.ADRecipientSessionContext.GetADRecipientSession();
			ADRecipient adrecipient = adrecipientSession.FindByProxyAddress(ProxyAddress.Parse(smtpAddress.ToString()));
			if (string.Equals(userContext.ExchangePrincipal.LegacyDn, adrecipient.LegacyExchangeDN, StringComparison.OrdinalIgnoreCase))
			{
				throw new OwaInvalidOperationException("Cannot open own folder");
			}
			SimpleConfiguration<OtherMailboxConfigEntry> otherMailboxConfig = OwaOtherMailboxConfiguration.GetOtherMailboxConfig(CallContext.Current);
			if (OwaOtherMailboxConfiguration.FindOtherMailboxConfigEntry(otherMailboxConfig, primarySMTPAddress) == null)
			{
				OtherMailboxConfigEntry otherMailboxConfigEntry = new OtherMailboxConfigEntry();
				otherMailboxConfigEntry.DisplayName = displayName;
				otherMailboxConfigEntry.PrincipalSMTPAddress = primarySMTPAddress;
				otherMailboxConfig.Entries.Add(otherMailboxConfigEntry);
				otherMailboxConfig.Save(CallContext.Current);
			}
			return true;
		}

		internal static bool RemoveOtherMailboxConfig(CallContext callContext, string primarySMTPAddress)
		{
			if (string.IsNullOrWhiteSpace(primarySMTPAddress))
			{
				return false;
			}
			SimpleConfiguration<OtherMailboxConfigEntry> otherMailboxConfig = OwaOtherMailboxConfiguration.GetOtherMailboxConfig(CallContext.Current);
			OtherMailboxConfigEntry otherMailboxConfigEntry = OwaOtherMailboxConfiguration.FindOtherMailboxConfigEntry(otherMailboxConfig, primarySMTPAddress);
			if (otherMailboxConfigEntry != null)
			{
				otherMailboxConfig.Entries.Remove(otherMailboxConfigEntry);
				otherMailboxConfig.Save(CallContext.Current);
			}
			return true;
		}

		internal static SimpleConfiguration<OtherMailboxConfigEntry> GetOtherMailboxConfig(CallContext callContext)
		{
			SimpleConfiguration<OtherMailboxConfigEntry> simpleConfiguration = new SimpleConfiguration<OtherMailboxConfigEntry>();
			simpleConfiguration.Load(callContext);
			OwaOtherMailboxConfiguration.ConvertLegacyItemIdFormatIfNecessary(simpleConfiguration, callContext);
			return simpleConfiguration;
		}

		internal void LoadAll(CallContext callContext)
		{
			SimpleConfiguration<OtherMailboxConfigEntry> otherMailboxConfig = OwaOtherMailboxConfiguration.GetOtherMailboxConfig(callContext);
			this.PopulateConfigEntries(otherMailboxConfig.Entries);
		}

		private static void ConvertLegacyItemIdFormatIfNecessary(SimpleConfiguration<OtherMailboxConfigEntry> otherMailboxConfig, CallContext callContext)
		{
			bool flag = false;
			for (int i = otherMailboxConfig.Entries.Count - 1; i >= 0; i--)
			{
				OtherMailboxConfigEntry otherMailboxConfigEntry = otherMailboxConfig.Entries[i];
				if (!string.IsNullOrEmpty(otherMailboxConfigEntry.InboxFolderOwaStoreObjectId))
				{
					flag = true;
					bool flag2 = false;
					try
					{
						OwaStoreObjectId owaStoreObjectId = OwaStoreObjectId.CreateFromString(otherMailboxConfigEntry.InboxFolderOwaStoreObjectId);
						ExchangePrincipal exchangePrincipal = (owaStoreObjectId.MailboxOwnerLegacyDN != null) ? ExchangePrincipal.FromLegacyDN(callContext.SessionCache.GetMailboxIdentityMailboxSession().GetADSessionSettings(), owaStoreObjectId.MailboxOwnerLegacyDN) : callContext.AccessingPrincipal;
						otherMailboxConfigEntry.PrincipalSMTPAddress = exchangePrincipal.MailboxInfo.PrimarySmtpAddress.ToString();
						otherMailboxConfigEntry.InboxFolderOwaStoreObjectId = null;
						flag2 = true;
					}
					catch (OwaInvalidIdFormatException)
					{
					}
					catch (OwaInvalidRequestException)
					{
					}
					catch (ObjectNotFoundException)
					{
					}
					finally
					{
						if (!flag2)
						{
							otherMailboxConfig.Entries.RemoveAt(i);
						}
					}
				}
			}
			if (flag)
			{
				otherMailboxConfig.Save(callContext);
			}
		}

		private static OtherMailboxConfigEntry FindOtherMailboxConfigEntry(SimpleConfiguration<OtherMailboxConfigEntry> otherMailboxConfig, string primarySMTPAddress)
		{
			foreach (OtherMailboxConfigEntry otherMailboxConfigEntry in otherMailboxConfig.Entries)
			{
				if (primarySMTPAddress.Equals(otherMailboxConfigEntry.PrincipalSMTPAddress, StringComparison.OrdinalIgnoreCase))
				{
					return otherMailboxConfigEntry;
				}
			}
			return null;
		}

		private void PopulateConfigEntries(IList<OtherMailboxConfigEntry> entries)
		{
			this.otherMailboxEntries = new OtherMailboxConfigEntry[entries.Count];
			for (int i = 0; i < entries.Count; i++)
			{
				this.otherMailboxEntries[i] = entries[i];
			}
		}

		private OtherMailboxConfigEntry[] otherMailboxEntries;
	}
}
