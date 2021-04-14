using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.Transport;
using Microsoft.Office.CompliancePolicy.PolicyConfiguration;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Office.CompliancePolicy.Validators
{
	internal class ExchangeValidator : SourceValidator
	{
		public ExchangeValidator(IRecipientSession recipientSession, Func<RecipientIdParameter, IRecipientSession, ReducedRecipient> recipientGetter, Task.TaskErrorLoggingDelegate writeErrorDelegate, Action<LocalizedString> writeWarningDelegate, Func<LocalizedString, bool> shouldContinueDelegate, bool allowGroups, int maxRecipientsLimit, ExecutionLog logger, string logTag, string tenantId, SourceValidator.Clients client) : base(writeErrorDelegate, writeWarningDelegate, shouldContinueDelegate, logger, logTag, tenantId, client)
		{
			ArgumentValidator.ThrowIfNull("recipientSession", recipientSession);
			ArgumentValidator.ThrowIfNull("recipientGetter", recipientGetter);
			this.recipientSession = recipientSession;
			this.recipientGetter = recipientGetter;
			this.allowGroups = allowGroups;
			this.maxRecipientsLimit = maxRecipientsLimit;
		}

		public MultiValuedProperty<BindingMetadata> ValidateRecipients(MultiValuedProperty<string> recipients)
		{
			ArgumentValidator.ThrowIfNull("recipients", recipients);
			base.LogOneEntry(ExecutionLog.EventType.Verbose, "Executing ValidateRecipients", new object[0]);
			MultiValuedProperty<BindingMetadata> result;
			try
			{
				result = this.ValidateRecipientsImpl(recipients);
			}
			catch (ExValidatorException exception)
			{
				base.LogOneEntry(ExecutionLog.EventType.Error, exception, "Exception occured when Validating recipients", new object[0]);
				throw;
			}
			catch (ManagementObjectNotFoundException exception2)
			{
				base.LogOneEntry(ExecutionLog.EventType.Error, exception2, "Exception occured when Validating recipients", new object[0]);
				throw;
			}
			catch (ManagementObjectAmbiguousException exception3)
			{
				base.LogOneEntry(ExecutionLog.EventType.Error, exception3, "Exception occured when Validating recipients", new object[0]);
				throw;
			}
			catch (Exception ex)
			{
				EventNotificationItem.Publish(ExchangeComponent.UnifiedComplianceSourceValidation.Name, "ExchangeValidatorUnexpectedError", base.Client.ToString(), string.Format("Unexpected exception occured when validating the recipients. Exception:{0}", ex), ResultSeverityLevel.Error, false);
				base.LogOneEntry(ExecutionLog.EventType.Error, ex, "Unexcepted exception occured when Validating recipients", new object[0]);
				throw;
			}
			finally
			{
				base.LogOneEntry(ExecutionLog.EventType.Verbose, "Finished ValidateRecipients", new object[0]);
			}
			return result;
		}

		public static ExchangeValidator Create(IConfigurationSession configSession, Func<RecipientIdParameter, IRecipientSession, ReducedRecipient> recipientGetter, Task.TaskErrorLoggingDelegate writeErrorDelegate, Action<LocalizedString> writeWarningDelegate, Func<LocalizedString, bool> shouldContinueDelegate, bool allowGroups, string logTag, SourceValidator.Clients client, int existingRecipientsCount, ExecutionLog logger)
		{
			ArgumentValidator.ThrowIfNull("configSession", configSession);
			OrganizationId organizationId = configSession.GetOrgContainer().OrganizationId;
			ADSessionSettings sessionSettings = ADSessionSettings.FromExternalDirectoryOrganizationId(new Guid(organizationId.ToExternalDirectoryOrganizationId()));
			IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(ConsistencyMode.IgnoreInvalid, sessionSettings, 205, "Create", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\transport\\UnifiedPolicy\\Validators\\ExchangeValidator.cs");
			int maxLimitFromConfig = SourceValidator.GetMaxLimitFromConfig("MaxRecipientsLimit", 1000, existingRecipientsCount);
			return new ExchangeValidator(tenantOrRootOrgRecipientSession, recipientGetter, writeErrorDelegate, writeWarningDelegate, shouldContinueDelegate, allowGroups, maxLimitFromConfig, logger, logTag, organizationId.ToExternalDirectoryOrganizationId(), client);
		}

		private MultiValuedProperty<BindingMetadata> ValidateRecipientsImpl(IEnumerable<string> recipients)
		{
			MultiValuedProperty<BindingMetadata> multiValuedProperty = new MultiValuedProperty<BindingMetadata>();
			bool flag = false;
			this.warnedSkipInvalidTypeInGroup = false;
			if (recipients.Count<string>() > this.maxRecipientsLimit)
			{
				base.LogOneEntry(ExecutionLog.EventType.Error, "InvalidArgument: {0}", new object[]
				{
					Strings.ErrorMaxMailboxLimit(this.maxRecipientsLimit, recipients.Count<string>())
				});
				base.WriteError(new ExValidatorException(Strings.ErrorMaxMailboxLimit(this.maxRecipientsLimit, recipients.Count<string>())), ErrorCategory.InvalidArgument);
			}
			else
			{
				foreach (string text in recipients)
				{
					if (SourceValidator.IsWideScope(text))
					{
						base.LogOneEntry(ExecutionLog.EventType.Verbose, "Skipping validation for wide scoped value '{0}", new object[]
						{
							text
						});
						multiValuedProperty.Add(new BindingMetadata(text, text, text, SourceValidator.GetBindingType(text)));
					}
					else
					{
						RecipientIdParameter recipientIdParameter = RecipientIdParameter.Parse(text);
						ReducedRecipient reducedRecipient = this.recipientGetter(recipientIdParameter, this.recipientSession);
						if (ExchangeValidator.IsMembershipGroup(reducedRecipient) && this.allowGroups)
						{
							if (!flag)
							{
								if (base.ShouldContinue(Strings.ShouldExpandGroups))
								{
									flag = true;
									base.LogOneEntry(ExecutionLog.EventType.Information, "Got confirmation to expand groups", new object[0]);
								}
								else
								{
									base.LogOneEntry(ExecutionLog.EventType.Error, "User selected not to expand groups. {0}", new object[]
									{
										Strings.GroupsIsNotAllowedForHold(recipientIdParameter.RawIdentity)
									});
									base.WriteError(new ExValidatorException(Strings.GroupsIsNotAllowedForHold(recipientIdParameter.RawIdentity)), ErrorCategory.InvalidArgument);
								}
							}
							if (flag)
							{
								this.ExpandGroupMembership(reducedRecipient, multiValuedProperty);
							}
						}
						else
						{
							this.VerifyAndAddRecipient(reducedRecipient, multiValuedProperty, false);
						}
					}
				}
			}
			return multiValuedProperty;
		}

		private void VerifyAndAddRecipient(ReducedRecipient recipient, MultiValuedProperty<BindingMetadata> validatedRecipients, bool inGroupExpansion)
		{
			if (ExchangeValidator.IsValidRecipientType(recipient))
			{
				if (!validatedRecipients.Any((BindingMetadata p) => string.Equals(recipient.ExternalDirectoryObjectId, p.ImmutableIdentity, StringComparison.OrdinalIgnoreCase)))
				{
					if (validatedRecipients.Count < this.maxRecipientsLimit)
					{
						validatedRecipients.Add(new BindingMetadata(recipient.Name, recipient.PrimarySmtpAddress.ToString(), recipient.ExternalDirectoryObjectId, PolicyBindingTypes.IndividualResource));
						return;
					}
					base.LogOneEntry(ExecutionLog.EventType.Error, "InvalidArgument: {0}", new object[]
					{
						Strings.ErrorMaxMailboxLimitReachedInGroupExpansion(this.maxRecipientsLimit)
					});
					base.WriteError(new ExValidatorException(Strings.ErrorMaxMailboxLimitReachedInGroupExpansion(this.maxRecipientsLimit)), ErrorCategory.InvalidArgument);
					return;
				}
			}
			else
			{
				if (inGroupExpansion)
				{
					if (!this.warnedSkipInvalidTypeInGroup)
					{
						this.warnedSkipInvalidTypeInGroup = true;
						base.WriteWarning(Strings.SkippingInvalidTypeInGroupExpansion);
					}
					base.LogOneEntry(ExecutionLog.EventType.Warning, "Invalid group member '{0}' skipped as the type '{1}' is not supported", new object[]
					{
						recipient.PrimarySmtpAddress.ToString(),
						recipient.RecipientTypeDetails
					});
					return;
				}
				base.LogOneEntry(ExecutionLog.EventType.Error, "InvalidArgument: {0}", new object[]
				{
					Strings.ErrorInvalidRecipientType(recipient.PrimarySmtpAddress.ToString(), recipient.RecipientTypeDetails.ToString())
				});
				base.WriteError(new ExValidatorException(Strings.ErrorInvalidRecipientType(recipient.PrimarySmtpAddress.ToString(), recipient.RecipientTypeDetails.ToString())), ErrorCategory.InvalidArgument);
			}
		}

		private static bool IsValidRecipientType(ReducedRecipient recipient)
		{
			return recipient.RecipientTypeDetails.HasFlag(RecipientTypeDetails.MailUser);
		}

		private static bool IsMembershipGroup(ReducedRecipient recipient)
		{
			return recipient.RecipientTypeDetails.HasFlag(RecipientTypeDetails.MailNonUniversalGroup) || recipient.RecipientTypeDetails.HasFlag(RecipientTypeDetails.MailUniversalDistributionGroup) || recipient.RecipientTypeDetails.HasFlag(RecipientTypeDetails.MailUniversalSecurityGroup);
		}

		private void ExpandGroupMembership(ReducedRecipient groupRecipient, MultiValuedProperty<BindingMetadata> validatedRecipients)
		{
			QueryFilter filter = QueryFilter.AndTogether(new QueryFilter[]
			{
				new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.MemberOfGroup, groupRecipient.Guid),
				new ComparisonFilter(ComparisonOperator.Equal, SharedPropertyDefinitions.FfoExpansionSizeUpperBoundFilter, this.maxRecipientsLimit)
			});
			try
			{
				ADPagedReader<ADRawEntry> adpagedReader = this.recipientSession.FindPagedADRawEntry(null, QueryScope.SubTree, filter, null, 1000, ExchangeValidator.memberOfProperties);
				foreach (ADRawEntry adrawEntry in adpagedReader)
				{
					RecipientIdParameter arg = RecipientIdParameter.Parse(adrawEntry.Id.ObjectGuid.ToString());
					ReducedRecipient recipient = this.recipientGetter(arg, this.recipientSession);
					this.VerifyAndAddRecipient(recipient, validatedRecipients, true);
				}
			}
			catch (FfoSizeLimitReachedException ex)
			{
				base.LogOneEntry(ExecutionLog.EventType.Error, "InvalidArgument: {0}, Exception: {1}", new object[]
				{
					Strings.ErrorMaxMailboxLimitReachedInGroupExpansion(this.maxRecipientsLimit),
					ex
				});
				base.WriteError(new ExValidatorException(Strings.ErrorMaxMailboxLimitReachedInGroupExpansion(this.maxRecipientsLimit), ex), ErrorCategory.InvalidArgument);
			}
		}

		private const int MaxRecipientsDefaultLimit = 1000;

		private const string MaxRecipientsLimitKey = "MaxRecipientsLimit";

		private const int MemberExpansionPageSize = 1000;

		private const string UnexpectedErrorEvent = "ExchangeValidatorUnexpectedError";

		private static readonly PropertyDefinition[] memberOfProperties = new PropertyDefinition[]
		{
			ADObjectSchema.Id,
			ADRecipientSchema.RecipientType
		};

		private readonly IRecipientSession recipientSession;

		private readonly Func<RecipientIdParameter, IRecipientSession, ReducedRecipient> recipientGetter;

		private readonly int maxRecipientsLimit;

		private readonly bool allowGroups;

		private bool warnedSkipInvalidTypeInGroup;
	}
}
