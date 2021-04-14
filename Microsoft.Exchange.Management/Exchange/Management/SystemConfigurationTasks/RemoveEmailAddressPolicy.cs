using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Remove", "EmailAddressPolicy", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
	public sealed class RemoveEmailAddressPolicy : RemoveSystemConfigurationObjectTask<EmailAddressPolicyIdParameter, EmailAddressPolicy>
	{
		protected override void InternalBeginProcessing()
		{
			TaskLogger.LogEnter();
			base.InternalBeginProcessing();
			TaskLogger.LogExit();
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageRemoveEmailAddressPolicy(this.Identity.ToString());
			}
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.InternalValidate();
			if (!base.HasErrors)
			{
				if (base.DataObject.Priority == EmailAddressPolicyPriority.Lowest)
				{
					this.WriteError(new InvalidOperationException(Strings.ErrorInvalidOperationOnLowestEap(this.Identity.ToString())), ErrorCategory.InvalidOperation, base.DataObject.Identity, false);
				}
				this.affectedPolicies = null;
				if (EmailAddressPolicySchema.Priority.ValidateValue(base.DataObject.Priority, false) == null)
				{
					base.DataObject.Priority = EmailAddressPolicyPriority.Lowest;
					UpdateEmailAddressPolicy.PreparePriorityOfEapObjects(base.DataObject.OrganizationId, base.DataObject, base.DataSession, new TaskExtendedErrorLoggingDelegate(this.WriteError), out this.affectedPolicies, out this.affectedPoliciesOriginalPriority);
				}
			}
			TaskLogger.LogExit();
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			bool flag = this.affectedPolicies != null && this.affectedPolicies.Length > 0;
			List<EmailAddressPolicy> list = new List<EmailAddressPolicy>();
			try
			{
				if (this.affectedPolicies != null)
				{
					for (int i = 0; i < this.affectedPolicies.Length; i++)
					{
						if (flag)
						{
							base.WriteProgress(Strings.ProgressEmailAddressPolicyPreparingPriority, Strings.ProgressEmailAddressPolicyAdjustingPriority(this.affectedPolicies[i].Identity.ToString()), i * 99 / this.affectedPolicies.Length + 1);
						}
						base.DataSession.Save(this.affectedPolicies[i]);
						this.affectedPolicies[i].Priority = this.affectedPoliciesOriginalPriority[i];
						list.Add(this.affectedPolicies[i]);
					}
				}
				base.InternalProcessRecord();
				if (!base.HasErrors)
				{
					list.Clear();
				}
				if (base.IsProvisioningLayerAvailable)
				{
					base.UserSpecifiedParameters["DomainController"] = base.DataObject.OriginatingServer;
				}
				else
				{
					base.WriteError(new InvalidOperationException(Strings.ErrorNoProvisioningHandlerAvailable), ErrorCategory.InvalidOperation, null);
				}
			}
			finally
			{
				for (int j = 0; j < list.Count; j++)
				{
					EmailAddressPolicy emailAddressPolicy = list[j];
					try
					{
						if (flag)
						{
							base.WriteProgress(Strings.ProgressEmailAddressPolicyPreparingPriority, Strings.ProgressEmailAddressPolicyRollingBackPriority(emailAddressPolicy.Identity.ToString()), j * 99 / list.Count + 1);
						}
						base.DataSession.Save(emailAddressPolicy);
					}
					catch (DataSourceTransientException)
					{
						this.WriteWarning(Strings.VerboseFailedToRollbackPriority(emailAddressPolicy.Id.ToString()));
					}
				}
			}
			if (!base.HasErrors)
			{
				if (base.IsProvisioningLayerAvailable)
				{
					try
					{
						OrganizationId organizationId = base.DataObject.OrganizationId;
						ADSessionSettings sessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopes(base.RootOrgContainerId, organizationId, base.ExecutingUserOrganizationId, false);
						IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(base.TenantGlobalCatalogSession.DomainController, true, ConsistencyMode.PartiallyConsistent, sessionSettings, 190, "InternalProcessRecord", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\SystemConfigurationTasks\\RecipientPolicy\\RemoveEmailAddressPolicy.cs");
						tenantOrRootOrgRecipientSession.UseGlobalCatalog = true;
						UpdateEmailAddressPolicy.UpdateRecipients(base.DataObject, base.DataObject.OrganizationId, base.DomainController, tenantOrRootOrgRecipientSession, new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), new Task.TaskWarningLoggingDelegate(this.WriteWarning), new WriteProgress(base.WriteProgress), this);
						goto IL_2DE;
					}
					catch (DataSourceTransientException ex)
					{
						this.WriteWarning(Strings.ErrorReadMatchingRecipients(this.Identity.ToString(), base.DataObject.LdapRecipientFilter, ex.Message));
						TaskLogger.Trace("Exception is raised while reading recipients: {0}", new object[]
						{
							ex.ToString()
						});
						goto IL_2DE;
					}
					catch (DataSourceOperationException ex2)
					{
						this.WriteWarning(Strings.ErrorReadMatchingRecipients(this.Identity.ToString(), base.DataObject.LdapRecipientFilter, ex2.Message));
						TaskLogger.Trace("Exception is raised while reading recipients matching filter: {0}", new object[]
						{
							ex2.ToString()
						});
						goto IL_2DE;
					}
				}
				base.WriteError(new InvalidOperationException(Strings.ErrorNoProvisioningHandlerAvailable), ErrorCategory.InvalidOperation, null);
			}
			IL_2DE:
			TaskLogger.LogExit();
		}

		private EmailAddressPolicy[] affectedPolicies;

		private EmailAddressPolicyPriority[] affectedPoliciesOriginalPriority;
	}
}
