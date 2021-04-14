using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Deployment;

namespace Microsoft.Exchange.Management.Tasks
{
	[Cmdlet("Clear", "ObsoleteRBACRoles")]
	public sealed class ClearObsoleteRBACRoles : SetupTaskBase
	{
		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = true)]
		public override OrganizationIdParameter Organization
		{
			get
			{
				return base.Organization;
			}
			set
			{
				base.Organization = value;
			}
		}

		[Parameter(Mandatory = true)]
		[ValidateNotNullOrEmpty]
		public ServicePlan ServicePlanSettings
		{
			get
			{
				return (ServicePlan)base.Fields["ServicePlanSettings"];
			}
			set
			{
				base.Fields["ServicePlanSettings"] = value;
			}
		}

		protected override void InternalProcessRecord()
		{
			base.InternalProcessRecord();
			Dictionary<string, bool> dictionary = new Dictionary<string, bool>();
			List<ExchangeRole> list = new List<ExchangeRole>();
			foreach (ServicePlan.MailboxPlan mailboxPlan in this.ServicePlanSettings.MailboxPlans)
			{
				if (!string.IsNullOrEmpty(mailboxPlan.MailboxPlanIndex))
				{
					dictionary[mailboxPlan.MailboxPlanIndex] = true;
				}
			}
			ADPagedReader<ExchangeRole> adpagedReader = this.configurationSession.FindAllPaged<ExchangeRole>();
			foreach (ExchangeRole exchangeRole in adpagedReader)
			{
				if (exchangeRole.IsRootRole && !string.IsNullOrEmpty(exchangeRole.MailboxPlanIndex) && dictionary.ContainsKey(exchangeRole.MailboxPlanIndex))
				{
					list.Add(exchangeRole);
				}
			}
			ADObjectId descendantId = base.OrgContainerId.GetDescendantId(ExchangeRoleAssignment.RdnContainer);
			foreach (ExchangeRole exchangeRole2 in list)
			{
				this.RemoveRoleTreeAndAssignments(exchangeRole2.Id, descendantId);
			}
		}

		private void RemoveRoleTreeAndAssignments(ADObjectId roleId, ADObjectId roleAssignmentContainerId)
		{
			TaskLogger.LogEnter(new object[]
			{
				"roleId"
			});
			ExchangeRole[] array = this.configurationSession.Find<ExchangeRole>(roleId, QueryScope.SubTree, null, null, 0);
			if (array.Length > 0)
			{
				ExchangeRole exchangeRole = null;
				foreach (ExchangeRole exchangeRole2 in array)
				{
					base.LogReadObject(exchangeRole2);
					if (exchangeRole2.Id.Equals(roleId))
					{
						exchangeRole = exchangeRole2;
					}
					ADPagedReader<ExchangeRoleAssignment> adpagedReader = this.configurationSession.FindPaged<ExchangeRoleAssignment>(roleAssignmentContainerId, QueryScope.OneLevel, new ComparisonFilter(ComparisonOperator.Equal, ExchangeRoleAssignmentSchema.Role, exchangeRole2.Id), null, 0);
					foreach (ExchangeRoleAssignment exchangeRoleAssignment in adpagedReader)
					{
						base.LogReadObject(exchangeRoleAssignment);
						this.configurationSession.Delete(exchangeRoleAssignment);
						base.LogWriteObject(exchangeRoleAssignment);
					}
				}
				this.configurationSession.DeleteTree(exchangeRole, delegate(ADTreeDeleteNotFinishedException de)
				{
					if (de != null)
					{
						base.WriteVerbose(de.LocalizedString);
					}
				});
				base.LogWriteObject(exchangeRole);
			}
			TaskLogger.LogExit();
		}
	}
}
