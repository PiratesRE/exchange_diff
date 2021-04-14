using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.ProvisioningCache;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.ProvisioningAgent;
using Microsoft.Exchange.Provisioning;
using Microsoft.Exchange.ProvisioningAgent;

namespace Microsoft.Exchange.DefaultProvisioningAgent.Rus
{
	internal class EmailAddressPolicyHandler : RusDataHandler
	{
		public EmailAddressPolicyHandler(string configurationDomainController, string recipientDomainController, string globalCatalog, NetworkCredential credential, PartitionId partitionId, UserScope userScope, ProvisioningCache provisioningCache, LogMessageDelegate logger) : this(configurationDomainController, recipientDomainController, globalCatalog, credential, partitionId, userScope, provisioningCache, false, logger)
		{
		}

		public EmailAddressPolicyHandler(string configurationDomainController, string recipientDomainController, string globalCatalog, NetworkCredential credential, PartitionId partitionId, UserScope userScope, ProvisioningCache provisioningCache, bool updateSecondaryAddressesOnly, LogMessageDelegate logger) : this(configurationDomainController, recipientDomainController, globalCatalog, credential, partitionId, userScope, provisioningCache, updateSecondaryAddressesOnly, null, logger)
		{
		}

		public EmailAddressPolicyHandler(string configurationDomainController, string recipientDomainController, string globalCatalog, NetworkCredential credential, PartitionId partitionId, UserScope userScope, ProvisioningCache provisioningCache, bool updateSecondaryAddressesOnly, MultiValuedProperty<ProxyAddressTemplate> specificAddressTemplates, LogMessageDelegate logger) : base(configurationDomainController, recipientDomainController, globalCatalog, credential, partitionId, provisioningCache, logger)
		{
			this.updateSecondaryAddressesOnly = updateSecondaryAddressesOnly;
			this.specificAddressTemplates = specificAddressTemplates;
		}

		protected EmailAddressPolicyDataProvider Provider
		{
			get
			{
				if (this.provider == null)
				{
					this.provider = new EmailAddressPolicyDataProvider(base.ConfigurationSession, base.RootOrgConfigurationSession, base.ProvisioningCache);
				}
				return this.provider;
			}
		}

		protected LdapFilterProvider FilterProvider
		{
			get
			{
				if (this.filterProvider == null)
				{
					if (base.Credential != null)
					{
						this.filterProvider = LdapFilterProvider.GetLdapFilterProvider(base.ConfigurationDomainController, base.Credential);
					}
					else
					{
						this.filterProvider = LdapFilterProvider.GetLdapFilterProvider(base.CurrentOrganizationId.PartitionId);
					}
				}
				return this.filterProvider;
			}
		}

		public ProvisioningValidationError[] Validate(EmailAddressPolicy emailAddressPolicy)
		{
			if (emailAddressPolicy == null)
			{
				throw new ArgumentNullException("emailAddressPolicy");
			}
			if (base.Logger != null)
			{
				base.Logger(Strings.VerboseToBeValidateObject(emailAddressPolicy.Identity.ToString(), emailAddressPolicy.GetType().Name));
			}
			List<ProvisioningValidationError> list = new List<ProvisioningValidationError>();
			if (emailAddressPolicy.IsModified(EmailAddressPolicySchema.LdapRecipientFilter) && !string.IsNullOrEmpty(emailAddressPolicy.LdapRecipientFilter))
			{
				try
				{
					LdapFilter.Parse(emailAddressPolicy.LdapRecipientFilter, this.FilterProvider);
				}
				catch (LdapFilterException ex)
				{
					list.Add(new ProvisioningValidationError(Strings.ErrorInvalidLdapFilter(ex.Message), ExchangeErrorCategory.Client, ex));
				}
			}
			List<ProxyAddressTemplate> list2 = new List<ProxyAddressTemplate>();
			if (emailAddressPolicy.IsModified(EmailAddressPolicySchema.RawEnabledEmailAddressTemplates) && ((MultiValuedProperty<ProxyAddressTemplate>)emailAddressPolicy[EmailAddressPolicySchema.RawEnabledEmailAddressTemplates]).Count != 0)
			{
				list2.AddRange((MultiValuedProperty<ProxyAddressTemplate>)emailAddressPolicy[EmailAddressPolicySchema.RawEnabledEmailAddressTemplates]);
			}
			if (emailAddressPolicy.IsModified(EmailAddressPolicySchema.DisabledEmailAddressTemplates) && emailAddressPolicy.DisabledEmailAddressTemplates.Count != 0)
			{
				list2.AddRange(emailAddressPolicy.DisabledEmailAddressTemplates);
			}
			ProxySession instance = ProxySession.Instance;
			foreach (ProxyAddressTemplate proxyAddressTemplate in list2)
			{
				try
				{
					if (!instance.ValidateBaseAddress(base.ConfigurationSession, proxyAddressTemplate))
					{
						list.Add(new ProvisioningValidationError(Strings.ErrorInvalidBaseAddress(proxyAddressTemplate.ToString()), ExchangeErrorCategory.Client));
					}
				}
				catch (RusException ex2)
				{
					list.Add(new ProvisioningValidationError(Strings.ErrorFailedToValidBaseAddress(proxyAddressTemplate.ToString(), ex2.Message), ExchangeErrorCategory.ServerOperation));
				}
			}
			return list.ToArray();
		}

		public bool UpdateRecipient(ADRecipient recipient)
		{
			if (recipient == null)
			{
				throw new ArgumentNullException("recipient");
			}
			bool result = false;
			ExTraceGlobals.RusTracer.TraceDebug<string>((long)this.GetHashCode(), "EmailAddressPolicyHandler.UpdateRecipient: recipient={0}", recipient.Identity.ToString());
			if (string.IsNullOrEmpty(recipient.Alias))
			{
				return false;
			}
			if (base.RecipientDomainController == null)
			{
				base.RecipientSession.DomainController = recipient.OriginatingServer;
			}
			base.CurrentOrganizationId = recipient.OrganizationId;
			if (recipient.PoliciesExcluded.Contains(this.eapPolicyIdentity))
			{
				if (recipient.PoliciesIncluded.Count > 0)
				{
					recipient.PoliciesIncluded.Clear();
					result = true;
				}
				if (this.UpdateEmailAddressesBySpecificAddressTemplates(recipient))
				{
					result = true;
				}
			}
			else
			{
				List<ADRawEntry> list = new List<ADRawEntry>();
				IEnumerable<ADRawEntry> policies = this.Provider.GetPolicies(recipient.OrganizationId, base.Logger);
				foreach (ADRawEntry adrawEntry in policies)
				{
					if (base.Logger != null)
					{
						base.Logger(Strings.VerboseEmailAddressPolicy(adrawEntry.Id.ToString()));
					}
					ADObjectId adobjectId = (ADObjectId)adrawEntry[EmailAddressPolicySchema.RecipientContainer];
					if (adobjectId == null || recipient.DistinguishedName.EndsWith(adobjectId.DistinguishedName, StringComparison.InvariantCultureIgnoreCase))
					{
						string text = (string)adrawEntry[EmailAddressPolicySchema.LdapRecipientFilter];
						if (!string.IsNullOrEmpty(text))
						{
							try
							{
								this.FilterProvider.PrepareLdapFilter(text);
							}
							catch (LdapFilterException ex)
							{
								throw new RusException(Strings.ErrorAddressListInvalidLdapFilter(text, adrawEntry.Identity.ToString(), ex.Message), ex);
							}
							list.Add(adrawEntry);
						}
					}
				}
				if (list.Count != 0)
				{
					list.Sort(delegate(ADRawEntry left, ADRawEntry right)
					{
						EmailAddressPolicyPriority emailAddressPolicyPriority = (EmailAddressPolicyPriority)left[EmailAddressPolicySchema.Priority];
						EmailAddressPolicyPriority value = (EmailAddressPolicyPriority)right[EmailAddressPolicySchema.Priority];
						return emailAddressPolicyPriority.CompareTo(value);
					});
					result = this.UpdateEmailAddresses(list, recipient, this.FilterProvider);
				}
			}
			foreach (ProxyAddress proxyAddress in recipient.EmailAddresses)
			{
				if (proxyAddress is X400ProxyAddress && proxyAddress.IsPrimaryAddress && !string.Equals(recipient.TextEncodedORAddress, proxyAddress.ToString(), StringComparison.InvariantCultureIgnoreCase))
				{
					recipient.TextEncodedORAddress = proxyAddress.ToString();
					result = true;
				}
			}
			if (recipient.WindowsEmailAddress != recipient.PrimarySmtpAddress && ADRecipientSchema.WindowsEmailAddress.ValidateValue(recipient.PrimarySmtpAddress, false) == null)
			{
				if (base.Logger != null)
				{
					base.Logger(Strings.VerboseSetWindowsEmailAddress(recipient.PrimarySmtpAddress.ToString()));
				}
				recipient.WindowsEmailAddress = recipient.PrimarySmtpAddress;
				result = true;
			}
			if (recipient.OriginalPrimarySmtpAddress != recipient.PrimarySmtpAddress)
			{
				recipient.OriginalPrimarySmtpAddress = recipient.PrimarySmtpAddress;
			}
			if (recipient.OriginalWindowsEmailAddress != recipient.WindowsEmailAddress)
			{
				recipient.OriginalWindowsEmailAddress = recipient.WindowsEmailAddress;
			}
			return result;
		}

		private bool UpdateEmailAddresses(List<ADRawEntry> allPolicies, ADRecipient recipient, LdapFilterProvider filterProvider)
		{
			if (!Datacenter.IsMultiTenancyEnabled())
			{
				string forestFQDN = base.ConfigurationSession.SessionSettings.PartitionId.ForestFQDN;
				if (!ADForest.IsLocalForestFqdn(forestFQDN))
				{
					if (base.Logger != null)
					{
						base.Logger(Strings.VerboseSkipRemoteForestEmailAddressUpdate(recipient.Id.ToString(), forestFQDN));
					}
					return false;
				}
			}
			string[] allAttributes = filterProvider.GetAllAttributes();
			object[] marshalledAttributes = RUSMarshal.MarshalWholeObject(recipient, base.RecipientSession, allAttributes);
			ProxySession instance = ProxySession.Instance;
			bool result = false;
			ProxyAddress[] array = null;
			for (int i = 0; i < allPolicies.Count; i++)
			{
				string text = (string)allPolicies[i][EmailAddressPolicySchema.LdapRecipientFilter];
				ExTraceGlobals.RusTracer.TraceDebug<string, string, string>((long)this.GetHashCode(), "EmailAddressPolicyHandler.UpdateEmailAddresses: evaluate recipient={0} against EmailAddressPolicy={1} with LdapRecipientFilter={2}", recipient.Identity.ToString(), allPolicies[i].Identity.ToString(), text);
				if (!string.IsNullOrEmpty(text))
				{
					LdapFilter ldapFilter = filterProvider.PrepareLdapFilter(text);
					if (ldapFilter.Evaluate(marshalledAttributes))
					{
						if (!recipient.PoliciesIncluded.Contains(allPolicies[i].Id.ObjectGuid.ToString()))
						{
							recipient.PoliciesIncluded.Clear();
							recipient.PoliciesIncluded.Add(this.eapPolicyIdentity);
							recipient.PoliciesIncluded.Add(allPolicies[i].Id.ObjectGuid.ToString());
							result = true;
						}
						MultiValuedProperty<ProxyAddressTemplate> baseAddresses = (MultiValuedProperty<ProxyAddressTemplate>)allPolicies[i][EmailAddressPolicySchema.EnabledEmailAddressTemplates];
						ExTraceGlobals.RusTracer.TraceDebug<string, string>((long)this.GetHashCode(), "EmailAddressPolicyHandler.UpdateEmailAddresses: Update EmailAddresses of recipient={0} based on EmailAddressPolicy={1}", recipient.Identity.ToString(), allPolicies[i].Identity.ToString());
						IRecipientSession tenantLocalRecipientSession = base.GetTenantLocalRecipientSession(recipient.OrganizationId);
						IRecipientSession tenantLocalGlobalCatalogSession = base.GetTenantLocalGlobalCatalogSession(recipient.OrganizationId);
						array = instance.CreateProxies(base.ConfigurationSession, tenantLocalRecipientSession, tenantLocalGlobalCatalogSession, baseAddresses, recipient, base.Logger);
						break;
					}
				}
			}
			if (array == null)
			{
				return result;
			}
			if (this.HandleNewProxies(recipient, array))
			{
				result = true;
			}
			return result;
		}

		private bool UpdateEmailAddressesBySpecificAddressTemplates(ADRecipient recipient)
		{
			if (this.specificAddressTemplates == null || this.specificAddressTemplates.Count == 0)
			{
				return false;
			}
			ExTraceGlobals.RusTracer.TraceDebug<string, int>((long)this.GetHashCode(), "EmailAddressPolicyHandler.UpdateEmailAddressesBySpecificAddressTemplates: recipient={0}; specificAddressTemplates count={1}", recipient.Identity.ToString(), this.specificAddressTemplates.Count);
			ProxySession instance = ProxySession.Instance;
			IRecipientSession tenantLocalRecipientSession = base.GetTenantLocalRecipientSession(recipient.OrganizationId);
			IRecipientSession tenantLocalGlobalCatalogSession = base.GetTenantLocalGlobalCatalogSession(recipient.OrganizationId);
			ProxyAddress[] array = instance.CreateProxies(base.ConfigurationSession, tenantLocalRecipientSession, tenantLocalGlobalCatalogSession, this.specificAddressTemplates, recipient, base.Logger);
			return array != null && this.HandleNewProxies(recipient, array);
		}

		private bool HandleNewProxies(ADRecipient recipient, ProxyAddress[] newProxies)
		{
			bool result = false;
			for (int i = 0; i < newProxies.Length; i++)
			{
				if (newProxies[i].IsPrimaryAddress)
				{
					bool flag = false;
					foreach (ProxyAddress proxyAddress in recipient.EmailAddresses)
					{
						if (proxyAddress == newProxies[i])
						{
							flag = true;
							if (!proxyAddress.IsPrimaryAddress && !this.updateSecondaryAddressesOnly)
							{
								result = true;
								if (base.Logger != null)
								{
									base.Logger(Strings.VerboseMakeEmailAddressToPrimary(proxyAddress.ToString()));
								}
								recipient.EmailAddresses.MakePrimary(proxyAddress);
								break;
							}
							break;
						}
					}
					if (!flag)
					{
						result = true;
						if (this.updateSecondaryAddressesOnly)
						{
							ProxyAddress proxyAddress2 = (ProxyAddress)newProxies[i].ToSecondary();
							if (base.Logger != null)
							{
								base.Logger(Strings.VerboseAddSecondaryEmailAddress(proxyAddress2.ToString()));
							}
							recipient.EmailAddresses.Add(proxyAddress2);
						}
						else
						{
							if (base.Logger != null)
							{
								base.Logger(Strings.VerboseAddPrimaryEmailAddress(newProxies[i].ToString()));
							}
							recipient.EmailAddresses.Add(newProxies[i]);
						}
					}
				}
			}
			for (int j = 0; j < newProxies.Length; j++)
			{
				if (!newProxies[j].IsPrimaryAddress)
				{
					bool flag2 = false;
					foreach (ProxyAddress a in recipient.EmailAddresses)
					{
						if (a == newProxies[j])
						{
							flag2 = true;
							break;
						}
					}
					if (!flag2)
					{
						result = true;
						if (base.Logger != null)
						{
							base.Logger(Strings.VerboseAddSecondaryEmailAddress(newProxies[j].ToString()));
						}
						recipient.EmailAddresses.Add(newProxies[j]);
					}
				}
			}
			return result;
		}

		private EmailAddressPolicyDataProvider provider;

		private LdapFilterProvider filterProvider;

		private readonly bool updateSecondaryAddressesOnly;

		private readonly MultiValuedProperty<ProxyAddressTemplate> specificAddressTemplates;

		private string eapPolicyIdentity = EmailAddressPolicy.PolicyGuid.ToString("B");
	}
}
