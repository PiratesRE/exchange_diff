using System;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage.Authentication;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class SharingProviderLocator
	{
		public SharingProviderLocator(ExchangePrincipal mailboxOwner, Func<IRecipientSession> recipientSessionFactory)
		{
			ArgumentValidator.ThrowIfNull("mailboxOwner", mailboxOwner);
			ArgumentValidator.ThrowIfNull("recipientSessionFactory", recipientSessionFactory);
			this.mailboxOwner = mailboxOwner;
			this.recipientSessionFactory = recipientSessionFactory;
		}

		private SharingPolicy SharingPolicy
		{
			get
			{
				SharingPolicy result;
				if ((result = this.sharingPolicy) == null)
				{
					result = (this.sharingPolicy = DirectoryHelper.ReadSharingPolicy(this.mailboxOwner.MailboxInfo.MailboxGuid, this.mailboxOwner.MailboxInfo.IsArchive, this.recipientSessionFactory()));
				}
				return result;
			}
		}

		public SharingProvider GetProvider(SmtpAddress recipientAddress, ADRecipient adRecipient, IFrontEndLocator frontEndLocator)
		{
			SharingProvider result;
			DetailLevelEnumType detailLevelEnumType;
			if (this.TryGetProvider(recipientAddress, adRecipient, frontEndLocator, out result, out detailLevelEnumType))
			{
				return result;
			}
			throw new NoSharingHandlerFoundException(recipientAddress.ToString());
		}

		public bool TryGetProvider(SmtpAddress recipientAddress, ADRecipient adRecipient, IFrontEndLocator frontEndLocator, out SharingProvider provider, out DetailLevelEnumType detailLevel)
		{
			provider = null;
			detailLevel = DetailLevelEnumType.AvailabilityOnly;
			bool result;
			try
			{
				if (this.mailboxOwner.GetConfiguration().DataStorage.XOWAConsumerSharing.Enabled)
				{
					provider = SharingProvider.SharingProviderConsumer;
					detailLevel = DetailLevelEnumType.Editor;
					result = true;
				}
				else
				{
					if (adRecipient != null)
					{
						if (adRecipient.IsValidSecurityPrincipal)
						{
							provider = SharingProvider.SharingProviderInternal;
							detailLevel = DetailLevelEnumType.Editor;
							return true;
						}
						if (DelegateUserCollection.IsCrossPremiseDelegateEnabled(this.mailboxOwner) && (adRecipient.RecipientType == RecipientType.User || adRecipient.RecipientType == RecipientType.UserMailbox || adRecipient.RecipientType == RecipientType.MailUser))
						{
							provider = SharingProvider.SharingProviderInternal;
							detailLevel = DetailLevelEnumType.Editor;
							return true;
						}
						if (adRecipient.RecipientType != RecipientType.User && adRecipient.RecipientType != RecipientType.UserMailbox && adRecipient.RecipientType != RecipientType.MailUser && adRecipient.RecipientType != RecipientType.Contact && adRecipient.RecipientType != RecipientType.MailContact)
						{
							return false;
						}
					}
					SharingPolicyDomain effectiveCalendarSharingPolicy = this.GetEffectiveCalendarSharingPolicy(recipientAddress.Domain, frontEndLocator);
					this.TraceDebug("Policy found:{0}", new object[]
					{
						(effectiveCalendarSharingPolicy == null) ? "none" : effectiveCalendarSharingPolicy.Domain
					});
					int maxAllowed;
					if (effectiveCalendarSharingPolicy != null && (maxAllowed = PolicyAllowedDetailLevel.GetMaxAllowed(effectiveCalendarSharingPolicy.Actions)) > 0)
					{
						detailLevel = (DetailLevelEnumType)maxAllowed;
						if (effectiveCalendarSharingPolicy.Domain == "Anonymous")
						{
							provider = SharingProvider.SharingProviderPublishReach;
						}
						else
						{
							provider = SharingProvider.SharingProviderExternal;
						}
					}
					result = (provider != null);
				}
			}
			finally
			{
				this.TraceDebug("MailboxOwner:{0},Recipient:{1},RecipientType:{2},Handler={2},DetailLevel={3}", new object[]
				{
					this.mailboxOwner.MailboxInfo.PrimarySmtpAddress.ToString(),
					recipientAddress,
					(adRecipient == null) ? "none" : adRecipient.RecipientType.ToString(),
					(provider == null) ? "none" : provider.Name,
					detailLevel
				});
			}
			return result;
		}

		private SharingPolicyDomain GetEffectiveCalendarSharingPolicy(string recipientDomain, IFrontEndLocator frontEndLocator)
		{
			SharingPolicyDomain sharingPolicyDomain = null;
			SharingPolicyDomain sharingPolicyDomain2 = null;
			bool enabled = ExternalAuthentication.GetCurrent().Enabled;
			bool flag = false;
			this.TraceDebug("External authentication enabled:{0}", new object[]
			{
				enabled
			});
			foreach (SharingPolicyDomain sharingPolicyDomain3 in this.SharingPolicy.Domains)
			{
				if (enabled)
				{
					if (StringComparer.OrdinalIgnoreCase.Equals(sharingPolicyDomain3.Domain, recipientDomain))
					{
						this.TraceDebug("Found exact policy for domain {0}", new object[]
						{
							sharingPolicyDomain3.Domain
						});
						sharingPolicyDomain = sharingPolicyDomain3;
					}
					else if (sharingPolicyDomain == null && StringComparer.OrdinalIgnoreCase.Equals(sharingPolicyDomain3.Domain, "*"))
					{
						this.TraceDebug("Found asterisk policy", new object[0]);
						sharingPolicyDomain = sharingPolicyDomain3;
						flag = true;
					}
				}
				if (StringComparer.OrdinalIgnoreCase.Equals(sharingPolicyDomain3.Domain, "Anonymous"))
				{
					if (OwaAnonymousVdirLocater.Instance.IsPublishingAvailable(this.mailboxOwner, frontEndLocator))
					{
						this.TraceDebug("Found anonymous policy", new object[0]);
						sharingPolicyDomain2 = sharingPolicyDomain3;
					}
					else
					{
						this.TraceDebug("Found anonymous policy but publishing is not allowed", new object[0]);
					}
				}
			}
			if (flag && sharingPolicyDomain2 != null && PolicyAllowedDetailLevel.GetMaxAllowed(sharingPolicyDomain.Actions) <= PolicyAllowedDetailLevel.GetMaxAllowed(sharingPolicyDomain2.Actions))
			{
				this.TraceDebug("Override AsteriskPolicy with anonymous policy", new object[0]);
				sharingPolicyDomain = null;
			}
			if (sharingPolicyDomain != null)
			{
				if (TargetUriResolver.Resolve(recipientDomain, this.mailboxOwner.MailboxInfo.OrganizationId) != null)
				{
					return sharingPolicyDomain;
				}
				this.TraceDebug("Target domain '{0}' is not federated", new object[]
				{
					recipientDomain
				});
			}
			return sharingPolicyDomain2;
		}

		private void TraceDebug(string messageFormat, params object[] args)
		{
			ExTraceGlobals.SharingTracer.TraceDebug((long)this.GetHashCode(), messageFormat, args);
		}

		private readonly ExchangePrincipal mailboxOwner;

		private readonly Func<IRecipientSession> recipientSessionFactory;

		private SharingPolicy sharingPolicy;
	}
}
