using System;
using System.Collections;
using System.Collections.Generic;
using System.DirectoryServices.ActiveDirectory;
using System.Management.Automation;
using System.Net.Sockets;
using System.Security.Authentication;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Common;
using Microsoft.Exchange.Management.EventMessages;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Provisioning;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("update", "EmailAddressPolicy", SupportsShouldProcess = true)]
	public sealed class UpdateEmailAddressPolicy : SystemConfigurationObjectActionTask<EmailAddressPolicyIdParameter, EmailAddressPolicy>
	{
		[Parameter]
		public SwitchParameter FixMissingAlias
		{
			get
			{
				return (SwitchParameter)(base.Fields["Status"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["Status"] = value;
			}
		}

		[Parameter]
		public SwitchParameter UpdateSecondaryAddressesOnly
		{
			get
			{
				return (SwitchParameter)(base.Fields["UpdateSecondaryAddressesOnly"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["UpdateSecondaryAddressesOnly"] = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageUpdateEmailAddressPolicy(this.Identity.ToString());
			}
		}

		internal static int PreparePriorityOfEapObjects(OrganizationId organizationId, EmailAddressPolicy policy, IConfigDataProvider session, TaskExtendedErrorLoggingDelegate writeError, out EmailAddressPolicy[] policiesAdjusted, out EmailAddressPolicyPriority[] originalPriorities)
		{
			policiesAdjusted = null;
			originalPriorities = null;
			try
			{
				QueryFilter queryFilter = new AndFilter(new QueryFilter[]
				{
					new ComparisonFilter(ComparisonOperator.LessThan, EmailAddressPolicySchema.Priority, EmailAddressPolicyPriority.Lowest),
					new ComparisonFilter(ComparisonOperator.GreaterThanOrEqual, EmailAddressPolicySchema.Priority, EmailAddressPolicyPriority.Highest)
				});
				if (policy.ObjectState != ObjectState.New)
				{
					queryFilter = new AndFilter(new QueryFilter[]
					{
						queryFilter,
						new ComparisonFilter(ComparisonOperator.NotEqual, ADObjectSchema.Id, new ADObjectId(null, policy.Guid))
					});
				}
				IConfigurationSession configurationSession = (IConfigurationSession)session;
				ADPagedReader<EmailAddressPolicy> adpagedReader = configurationSession.FindPaged<EmailAddressPolicy>(EmailAddressPolicyIdParameter.GetRootContainerId(configurationSession, organizationId), QueryScope.OneLevel, queryFilter, new SortBy(EmailAddressPolicySchema.Priority, SortOrder.Ascending), 0);
				EmailAddressPolicy[] array = adpagedReader.ReadAllPages();
				Array.Sort<EmailAddressPolicy>(array, EmailAddressPolicy.PriorityComparer);
				if (policy.Priority != EmailAddressPolicyPriority.Lowest)
				{
					if (policy.ObjectState == ObjectState.New)
					{
						if ((array.Length == 0 && policy.Priority != EmailAddressPolicyPriority.Highest) || (array.Length > 0 && policy.Priority > 1 + array[array.Length - 1].Priority))
						{
							writeError(new ArgumentException(Strings.ErrorInvalidEapNewPriority(policy.Priority.ToString()), "Priority"), ErrorCategory.InvalidArgument, policy.Id, true);
						}
					}
					else if ((array.Length == 0 && policy.Priority != EmailAddressPolicyPriority.Highest) || (array.Length > 0 && policy.Priority > array[array.Length - 1].Priority))
					{
						writeError(new ArgumentException(Strings.ErrorInvalidEapSetPriority(policy.Priority.ToString()), "Priority"), ErrorCategory.InvalidArgument, policy.Id, true);
					}
				}
				List<EmailAddressPolicy> list = new List<EmailAddressPolicy>();
				List<EmailAddressPolicyPriority> list2 = new List<EmailAddressPolicyPriority>();
				bool flag = false;
				for (int i = 0; i < array.Length; i++)
				{
					int num = i + (flag ? 1 : 0) + EmailAddressPolicyPriority.Highest;
					if (!flag && num == policy.Priority)
					{
						flag = true;
						num++;
					}
					EmailAddressPolicyPriority priority = array[i].Priority;
					UpdateEmailAddressPolicy.CheckEapVersion(array[i]);
					array[i].Priority = (EmailAddressPolicyPriority)num;
					if (array[i].IsChanged(EmailAddressPolicySchema.Priority))
					{
						list.Add(array[i]);
						list2.Add(priority);
					}
				}
				if (!flag)
				{
					UpdateEmailAddressPolicy.CheckEapVersion(policy);
					policy.Priority = (EmailAddressPolicyPriority)(array.Length + EmailAddressPolicyPriority.Highest);
				}
				foreach (EmailAddressPolicy emailAddressPolicy in list)
				{
					ValidationError[] array2 = emailAddressPolicy.Validate();
					int num2 = 0;
					while (array2.Length > num2)
					{
						writeError(new InvalidOperationException(Strings.ErrorInvalidOperationOnEapObject(emailAddressPolicy.Id.ToString(), array2[num2].Description)), ErrorCategory.InvalidData, policy.Id, array2.Length - 1 == num2);
						num2++;
					}
				}
				policiesAdjusted = list.ToArray();
				originalPriorities = list2.ToArray();
				return list.Count;
			}
			catch (DataSourceTransientException exception)
			{
				writeError(exception, ErrorCategory.ReadError, policy.Id, true);
			}
			return -1;
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.InternalValidate();
			if (!this.DataObject.HasEmailAddressSetting)
			{
				base.WriteError(new InvalidOperationException(Strings.ErrorCanNotApplyMailboxSettingOnlyPolicy(this.Identity.ToString())), ErrorCategory.InvalidOperation, this.DataObject.Id);
			}
			if (!base.HasErrors)
			{
				OrganizationId organizationId = this.DataObject.OrganizationId;
				if (this.domainValidator == null || !this.domainValidator.OrganizationId.Equals(organizationId))
				{
					this.domainValidator = new UpdateEmailAddressPolicy.WritableDomainValidator(organizationId, this.ConfigurationSession, new Task.TaskErrorLoggingDelegate(base.WriteError));
				}
				this.domainValidator.Validate(this.DataObject);
			}
			TaskLogger.LogExit();
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			if (base.IsProvisioningLayerAvailable)
			{
				base.UserSpecifiedParameters["DomainController"] = this.DataObject.OriginatingServer;
				try
				{
					OrganizationId organizationId = this.DataObject.OrganizationId;
					ADSessionSettings sessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopes(base.RootOrgContainerId, organizationId, base.ExecutingUserOrganizationId, false);
					IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(base.TenantGlobalCatalogSession.DomainController, true, ConsistencyMode.PartiallyConsistent, sessionSettings, 287, "InternalProcessRecord", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\SystemConfigurationTasks\\RecipientPolicy\\UpdateEmailAddressPolicy.cs");
					tenantOrRootOrgRecipientSession.UseGlobalCatalog = true;
					UpdateEmailAddressPolicy.UpdateRecipients(this.DataObject, this.DataObject.OrganizationId, base.DomainController, tenantOrRootOrgRecipientSession, new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), new Task.TaskWarningLoggingDelegate(this.WriteWarning), new WriteProgress(base.WriteProgress), this, this.FixMissingAlias.IsPresent);
					goto IL_1AF;
				}
				catch (DataSourceTransientException ex)
				{
					base.WriteError(new InvalidOperationException(Strings.ErrorReadMatchingRecipients(this.Identity.ToString(), this.DataObject.LdapRecipientFilter, ex.Message), ex), ErrorCategory.InvalidOperation, this.DataObject.Id);
					TaskLogger.Trace("Exception is raised while reading recipients: {0}", new object[]
					{
						ex.ToString()
					});
					goto IL_1AF;
				}
				catch (DataSourceOperationException ex2)
				{
					base.WriteError(new InvalidOperationException(Strings.ErrorReadMatchingRecipients(this.Identity.ToString(), this.DataObject.LdapRecipientFilter, ex2.Message), ex2), ErrorCategory.InvalidOperation, this.DataObject.Id);
					TaskLogger.Trace("Exception is raised while reading recipients matching filter: {0}", new object[]
					{
						ex2.ToString()
					});
					goto IL_1AF;
				}
			}
			base.WriteError(new InvalidOperationException(Strings.ErrorNoProvisioningHandlerAvailable), ErrorCategory.InvalidOperation, null);
			IL_1AF:
			if (!this.DataObject.ExchangeVersion.IsOlderThan(ExchangeObjectVersion.Exchange2007))
			{
				this.DataObject[EmailAddressPolicySchema.LastUpdatedRecipientFilter] = this.DataObject.RecipientFilter;
				this.DataObject[EmailAddressPolicySchema.RecipientFilterApplied] = true;
			}
			base.InternalProcessRecord();
			TaskLogger.LogExit();
		}

		internal static void UpdateRecipients(EmailAddressPolicy eap, OrganizationId organizationId, string domainController, IRecipientSession globalCatalogSession, Task.TaskVerboseLoggingDelegate writeVerbose, Task.TaskWarningLoggingDelegate writeWarning, WriteProgress writeProgress, Task cmdlet)
		{
			UpdateEmailAddressPolicy.UpdateRecipients(eap, organizationId, domainController, globalCatalogSession, writeVerbose, writeWarning, writeProgress, cmdlet, false);
		}

		private static void UpdateRecipients(EmailAddressPolicy eap, OrganizationId organizationId, string domainController, IRecipientSession globalCatalogSession, Task.TaskVerboseLoggingDelegate writeVerbose, Task.TaskWarningLoggingDelegate writeWarning, WriteProgress writeProgress, Task cmdlet, bool fixMissingAlias)
		{
			UpdateEmailAddressPolicy.AssertArgumentNotNull(eap, "eap");
			UpdateEmailAddressPolicy.AssertArgumentNotNull(writeVerbose, "writeVerbose");
			UpdateEmailAddressPolicy.AssertArgumentNotNull(writeWarning, "writeWarning");
			UpdateEmailAddressPolicy.AssertArgumentNotNull(writeProgress, "writeProgress");
			if (string.IsNullOrEmpty(eap.LdapRecipientFilter) && !fixMissingAlias)
			{
				return;
			}
			int num = 0;
			try
			{
				if (cmdlet != null && cmdlet.Stopping)
				{
					return;
				}
				IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(false, ConsistencyMode.PartiallyConsistent, globalCatalogSession.SessionSettings, 409, "UpdateRecipients", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\SystemConfigurationTasks\\RecipientPolicy\\UpdateEmailAddressPolicy.cs");
				tenantOrRootOrgRecipientSession.EnforceDefaultScope = false;
				IEnumerable<ADRecipient> enumerable = eap.FindMatchingRecipientsPaged(globalCatalogSession, organizationId, null, fixMissingAlias);
				string text = null;
				Hashtable hashtable = new Hashtable();
				foreach (ADRecipient adrecipient in enumerable)
				{
					if (cmdlet != null && cmdlet.Stopping)
					{
						return;
					}
					if (!string.IsNullOrEmpty(domainController) && string.IsNullOrEmpty(text))
					{
						try
						{
							string configurationDomainControllerFqdn = SystemConfigurationTasksHelper.GetConfigurationDomainControllerFqdn(domainController);
							int num2 = configurationDomainControllerFqdn.IndexOf(".");
							if (0 <= num2)
							{
								text = configurationDomainControllerFqdn.Substring(num2);
							}
						}
						catch (SocketException ex)
						{
							writeWarning(Strings.ErrorResolveFqdnForDomainController(domainController, ex.Message));
							return;
						}
					}
					string text2 = adrecipient.Id.DomainId.DistinguishedName.ToLowerInvariant();
					if (!hashtable.ContainsKey(text2))
					{
						SystemConfigurationTasksHelper.PrepareDomainControllerRecipientSessionForUpdate(tenantOrRootOrgRecipientSession, adrecipient.Id, domainController, text);
						IEnumerable<ADRecipient> collection = eap.FindMatchingRecipientsPaged(tenantOrRootOrgRecipientSession, organizationId, adrecipient.Id, fixMissingAlias);
						List<ADRecipient> list = new List<ADRecipient>();
						Exception ex2 = null;
						Exception ex3 = null;
						try
						{
							list.AddRange(collection);
						}
						catch (DataSourceOperationException ex4)
						{
							TaskLogger.Trace("Exception caught when re-read recipient from DC : {0}", new object[]
							{
								ex4.ToString()
							});
							if (ex4.InnerException is ActiveDirectoryObjectNotFoundException || ex4.InnerException is AuthenticationException)
							{
								ex3 = ex4;
							}
							else
							{
								ex2 = ex4;
							}
						}
						catch (DataSourceTransientException ex5)
						{
							TaskLogger.Trace("Exception caught when re-read recipient from DC : {0}", new object[]
							{
								ex5.ToString()
							});
							if (ex5.InnerException is ActiveDirectoryOperationException || ex5.InnerException is ActiveDirectoryServerDownException)
							{
								ex3 = ex5;
							}
							else
							{
								ex2 = ex5;
							}
						}
						if (ex3 != null)
						{
							hashtable.Add(text2, null);
							writeWarning(Strings.ErrorCannotUpdateRecipientOfDomain(DNConvertor.FqdnFromDomainDistinguishedName(text2), ex3.Message));
						}
						else if (ex2 != null)
						{
							writeWarning(Strings.ErrorFailedToReadRecipientForUpdate(adrecipient.Id.ToString(), ex2.Message));
						}
						else if (1 == list.Count)
						{
							ADRecipient adrecipient2 = list[0];
							if (cmdlet != null && cmdlet.Stopping)
							{
								return;
							}
							num = num++ % 99 + 1;
							writeProgress(Strings.ProgressActivityUpdateRecipient, Strings.ProgressStatusUpdateRecipient(adrecipient2.Id.ToString()), num);
							writeVerbose(Strings.ProgressStatusUpdateRecipient(adrecipient2.Id.ToString()));
							try
							{
								if (fixMissingAlias && string.IsNullOrEmpty(adrecipient2.Alias))
								{
									if (adrecipient2 is ADMicrosoftExchangeRecipient)
									{
										adrecipient2.Alias = RecipientTaskHelper.GenerateUniqueAlias(globalCatalogSession, adrecipient2.OrganizationId, ADMicrosoftExchangeRecipient.DefaultName, writeVerbose);
									}
									else if (adrecipient2 is ADSystemAttendantMailbox)
									{
										adrecipient2.Alias = RecipientTaskHelper.GenerateUniqueAlias(globalCatalogSession, adrecipient2.OrganizationId, (adrecipient2 as ADSystemAttendantMailbox).ServerName + "-SA", writeVerbose);
									}
									else
									{
										adrecipient2.Alias = RecipientTaskHelper.GenerateUniqueAlias(globalCatalogSession, adrecipient2.OrganizationId, adrecipient2.Name, writeVerbose);
									}
									writeWarning(Strings.WarningGeneratingMissingAlias(adrecipient2.Identity.ToString(), adrecipient2.Alias));
								}
								if (!adrecipient2.IsReadOnly)
								{
									ProvisioningLayer.UpdateAffectedIConfigurable(cmdlet, RecipientTaskHelper.ConvertRecipientToPresentationObject(adrecipient2), true);
								}
								if (!adrecipient2.IsValid || adrecipient2.IsReadOnly)
								{
									writeWarning(Strings.ErrorCannotUpdateInvalidRecipient(adrecipient2.Id.ToString()));
								}
								else
								{
									if (cmdlet.IsVerboseOn && adrecipient2.ObjectState != ObjectState.Unchanged)
									{
										writeVerbose(TaskVerboseStringHelper.GetConfigurableObjectChangedProperties(adrecipient2));
									}
									tenantOrRootOrgRecipientSession.Save(adrecipient2);
								}
							}
							catch (DataSourceTransientException ex6)
							{
								writeWarning(Strings.ErrorUpdateRecipient(adrecipient2.Id.ToString(), ex6.Message));
								TaskLogger.Trace("Exception is raised while updating recipient '{0}': {1}", new object[]
								{
									adrecipient2.Id.ToString(),
									ex6.Message
								});
							}
							catch (DataSourceOperationException ex7)
							{
								writeWarning(Strings.ErrorUpdateRecipient(adrecipient2.Id.ToString(), ex7.Message));
								TaskLogger.Trace("Exception is raised while updating recipient '{0}': {1}", new object[]
								{
									adrecipient2.Id.ToString(),
									ex7.Message
								});
							}
							catch (DataValidationException ex8)
							{
								writeWarning(Strings.ErrorUpdateRecipient(adrecipient2.Id.ToString(), ex8.Message));
								TaskLogger.Trace("Exception is raised while updating recipient '{0}': {1}", new object[]
								{
									adrecipient2.Id.ToString(),
									ex8.Message
								});
							}
						}
					}
				}
			}
			finally
			{
				if (cmdlet != null && cmdlet.Stopping)
				{
					ExManagementApplicationLogger.LogEvent(ManagementEventLogConstants.Tuple_RecipientsUpdateForEmailAddressPolicyCancelled, new string[]
					{
						eap.Identity.ToString(),
						eap.LdapRecipientFilter,
						ADRecipientSchema.EmailAddresses.Name
					});
				}
			}
			if (num != 0)
			{
				writeVerbose(Strings.ProgressStatusFinished);
				writeProgress(Strings.ProgressActivityUpdateRecipient, Strings.ProgressStatusFinished, 100);
			}
		}

		private static void AssertArgumentNotNull(object argument, string name)
		{
			if (argument == null)
			{
				throw new ArgumentNullException(name);
			}
		}

		public static void CheckEapVersion(EmailAddressPolicy policy)
		{
			if (policy.MaximumSupportedExchangeObjectVersion.IsOlderThan(policy.ExchangeVersion))
			{
				throw new DataValidationException(new PropertyValidationError(Strings.ErrorEapObjectTooNew(policy.ExchangeVersion.ToString(), policy.MaximumSupportedExchangeObjectVersion.ToString()), ADObjectSchema.ExchangeVersion, policy.ExchangeVersion));
			}
		}

		private UpdateEmailAddressPolicy.WritableDomainValidator domainValidator;

		internal abstract class DomainValidator
		{
			protected DomainValidator(IEnumerable<AcceptedDomain> domains)
			{
				this.map = new DomainMatchMap<UpdateEmailAddressPolicy.DomainValidator.TemplateValidationEntry>(UpdateEmailAddressPolicy.DomainValidator.TemplateValidationEntry.FromDomains(domains));
			}

			public void Validate(EmailAddressPolicy policy)
			{
				HashSet<SmtpDomain> hashSet = new HashSet<SmtpDomain>();
				foreach (ProxyAddressTemplate proxyAddressTemplate in UpdateEmailAddressPolicy.DomainValidator.EnumerateTemplates(policy))
				{
					SmtpDomain smtpDomain;
					if (UpdateEmailAddressPolicy.DomainValidator.TryGetDomain(proxyAddressTemplate, out smtpDomain))
					{
						UpdateEmailAddressPolicy.DomainValidator.TemplateValidationEntry bestMatch = this.map.GetBestMatch(smtpDomain);
						if (bestMatch == null || !bestMatch.AllowsPolicies)
						{
							this.WriteInvalidTemplate(proxyAddressTemplate as SmtpProxyAddressTemplate);
						}
						else if (!bestMatch.IsAuthoritative)
						{
							hashSet.TryAdd(smtpDomain);
						}
					}
				}
				this.HandleNonAuthoritativeDomains(policy, hashSet);
			}

			protected static bool TryGetDomain(ProxyAddressTemplate template, out SmtpDomain domain)
			{
				SmtpProxyAddressTemplate smtpProxyAddressTemplate = template as SmtpProxyAddressTemplate;
				if (smtpProxyAddressTemplate != null)
				{
					int num = smtpProxyAddressTemplate.AddressTemplateString.LastIndexOf('@');
					string domain2 = smtpProxyAddressTemplate.AddressTemplateString.Substring(num + 1);
					return SmtpDomain.TryParse(domain2, out domain);
				}
				domain = null;
				return false;
			}

			protected abstract void HandleNonAuthoritativeDomains(EmailAddressPolicy policy, HashSet<SmtpDomain> domains);

			protected abstract void WriteInvalidTemplate(SmtpProxyAddressTemplate template);

			private static IEnumerable<ProxyAddressTemplate> EnumerateTemplates(EmailAddressPolicy policy)
			{
				foreach (ProxyAddressTemplate template in policy.EnabledEmailAddressTemplates)
				{
					yield return template;
				}
				foreach (ProxyAddressTemplate template2 in policy.DisabledEmailAddressTemplates)
				{
					yield return template2;
				}
				yield break;
			}

			private DomainMatchMap<UpdateEmailAddressPolicy.DomainValidator.TemplateValidationEntry> map;

			private class TemplateValidationEntry : DomainMatchMap<UpdateEmailAddressPolicy.DomainValidator.TemplateValidationEntry>.IDomainEntry
			{
				private TemplateValidationEntry(AcceptedDomain domain)
				{
					this.domain = domain;
				}

				public SmtpDomainWithSubdomains DomainName
				{
					get
					{
						return this.domain.DomainName;
					}
				}

				public bool IsAuthoritative
				{
					get
					{
						return this.domain.DomainType == AcceptedDomainType.Authoritative;
					}
				}

				public bool AllowsPolicies
				{
					get
					{
						return this.domain.DomainType != AcceptedDomainType.ExternalRelay;
					}
				}

				public static List<UpdateEmailAddressPolicy.DomainValidator.TemplateValidationEntry> FromDomains(IEnumerable<AcceptedDomain> domains)
				{
					List<UpdateEmailAddressPolicy.DomainValidator.TemplateValidationEntry> list = new List<UpdateEmailAddressPolicy.DomainValidator.TemplateValidationEntry>();
					foreach (AcceptedDomain acceptedDomain in domains)
					{
						if (acceptedDomain != null && acceptedDomain.IsValid)
						{
							list.Add(new UpdateEmailAddressPolicy.DomainValidator.TemplateValidationEntry(acceptedDomain));
						}
					}
					return list;
				}

				private AcceptedDomain domain;
			}
		}

		internal class ReadOnlyDomainValidator : UpdateEmailAddressPolicy.DomainValidator
		{
			public OrganizationId OrganizationId
			{
				get
				{
					return this.organizationId;
				}
			}

			public ReadOnlyDomainValidator(OrganizationId organizationId, IConfigurationSession session, Task.TaskWarningLoggingDelegate warningWriter) : base(session.FindPaged<AcceptedDomain>(organizationId.ConfigurationUnit ?? session.GetOrgContainerId(), QueryScope.SubTree, null, null, 0))
			{
				this.organizationId = organizationId;
				this.warningWriter = warningWriter;
			}

			protected override void HandleNonAuthoritativeDomains(EmailAddressPolicy policy, HashSet<SmtpDomain> domains)
			{
				HashSet<SmtpDomain> hashSet = new HashSet<SmtpDomain>(policy.NonAuthoritativeDomains.Count);
				foreach (ProxyAddressTemplate template in policy.NonAuthoritativeDomains)
				{
					SmtpDomain item;
					if (UpdateEmailAddressPolicy.DomainValidator.TryGetDomain(template, out item))
					{
						hashSet.TryAdd(item);
					}
				}
				foreach (SmtpDomain smtpDomain in hashSet)
				{
					if (!domains.Contains(smtpDomain))
					{
						this.warningWriter(Strings.UnexpectedNonAuthoritativeDomain(smtpDomain));
					}
				}
				foreach (SmtpDomain smtpDomain2 in domains)
				{
					if (!hashSet.Contains(smtpDomain2))
					{
						this.warningWriter(Strings.MissingNonAuthoritativeDomain(smtpDomain2));
					}
				}
			}

			protected override void WriteInvalidTemplate(SmtpProxyAddressTemplate template)
			{
				this.warningWriter(Strings.ErrorInvalidDomainInSmtpAddressTemplate(template));
			}

			private readonly Task.TaskWarningLoggingDelegate warningWriter;

			private readonly OrganizationId organizationId;
		}

		internal class WritableDomainValidator : UpdateEmailAddressPolicy.DomainValidator
		{
			public OrganizationId OrganizationId
			{
				get
				{
					return this.organizationId;
				}
			}

			public WritableDomainValidator(OrganizationId organizationId, IConfigurationSession session, Task.TaskErrorLoggingDelegate errorWriter) : base(session.FindPaged<AcceptedDomain>(organizationId.ConfigurationUnit ?? session.GetOrgContainerId(), QueryScope.SubTree, null, null, 0))
			{
				this.organizationId = organizationId;
				this.errorWriter = errorWriter;
			}

			protected override void HandleNonAuthoritativeDomains(EmailAddressPolicy policy, HashSet<SmtpDomain> domains)
			{
				ProxyAddressTemplateCollection proxyAddressTemplateCollection = new ProxyAddressTemplateCollection();
				foreach (ProxyAddressTemplate proxyAddressTemplate in policy.NonAuthoritativeDomains)
				{
					SmtpDomain smtpDomain;
					if (!UpdateEmailAddressPolicy.DomainValidator.TryGetDomain(proxyAddressTemplate, out smtpDomain))
					{
						proxyAddressTemplateCollection.Add(proxyAddressTemplate);
					}
				}
				bool flag = false;
				foreach (SmtpDomain smtpDomain2 in domains)
				{
					SmtpProxyAddressTemplate item = new SmtpProxyAddressTemplate("@" + smtpDomain2.Domain, false);
					proxyAddressTemplateCollection.Add(item);
					if (!flag && !policy.NonAuthoritativeDomains.Contains(item))
					{
						flag = true;
					}
				}
				if (!flag)
				{
					flag = (proxyAddressTemplateCollection.Count != policy.NonAuthoritativeDomains.Count);
				}
				if (flag)
				{
					UpdateEmailAddressPolicy.CheckEapVersion(policy);
					policy.NonAuthoritativeDomains = proxyAddressTemplateCollection;
				}
			}

			protected override void WriteInvalidTemplate(SmtpProxyAddressTemplate template)
			{
				this.errorWriter(new ArgumentException(Strings.ErrorInvalidDomainInSmtpAddressTemplate(template)), ErrorCategory.InvalidData, template);
			}

			private readonly Task.TaskErrorLoggingDelegate errorWriter;

			private readonly OrganizationId organizationId;
		}
	}
}
