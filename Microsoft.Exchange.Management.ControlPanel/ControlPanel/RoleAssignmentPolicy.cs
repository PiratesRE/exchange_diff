using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.ControlPanel.WebControls;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	[KnownType(typeof(RoleAssignmentPolicy))]
	public class RoleAssignmentPolicy : RoleAssignmentPolicyRow
	{
		[DataMember]
		public string CaptionText { get; private set; }

		[DataMember]
		public string Description { get; private set; }

		[DataMember]
		public IEnumerable<Identity> AssignedEndUserRoles
		{
			get
			{
				if (this.assignedEndUserRoles == null)
				{
					IEnumerable<ExchangeRoleObject> source = ExchangeRoleObjectResolver.Instance.ResolveObjects(this.assignedRoles);
					IEnumerable<ExchangeRoleObject> enumerable = from role in source
					where role.IsEndUserRole
					select role;
					if (Util.IsDataCenter)
					{
						bool flag = false;
						foreach (ExchangeRoleObject exchangeRoleObject in enumerable)
						{
							if (!string.IsNullOrEmpty(exchangeRoleObject.MailboxPlanIndex))
							{
								flag = true;
								break;
							}
						}
						if (flag)
						{
							string mailboxplan = this.MailboxPlanIndex;
							enumerable = from role in enumerable
							where string.IsNullOrEmpty(role.MailboxPlanIndex) || role.MailboxPlanIndex == mailboxplan
							select role;
						}
					}
					this.assignedEndUserRoles = from role in enumerable
					select role.Identity;
				}
				return this.assignedEndUserRoles;
			}
		}

		internal string MailboxPlanIndex
		{
			get
			{
				if (Util.IsDataCenter)
				{
					if (string.IsNullOrEmpty(this.mailboxPlanIndex))
					{
						PowerShellResults<MailboxPlan> list = new MailboxPlans().GetList(null, null);
						if (!list.Succeeded)
						{
							ErrorHandlingUtil.TransferToErrorPage("notsupportrap");
						}
						MailboxPlan[] array = Array.FindAll<MailboxPlan>(list.Output, (MailboxPlan x) => x.RoleAssignmentPolicy.ToIdentity().RawIdentity == base.Identity.RawIdentity);
						if (array == null || array.Length != 1)
						{
							ErrorHandlingUtil.TransferToErrorPage("notsupportrap");
						}
						else
						{
							this.mailboxPlanIndex = array[0].MailboxPlanIndex;
						}
					}
					return this.mailboxPlanIndex;
				}
				throw new NotSupportedException("MailboxPlanIndex for RoleAssignmentPolicy is not supported in non-Datacenter environment");
			}
		}

		public RoleAssignmentPolicy(RoleAssignmentPolicy policy) : base(policy)
		{
			this.CaptionText = Strings.EditRoleAssignmentPolicyCaption(policy.Name);
			this.Description = policy.Description;
			this.assignedRoles = policy.AssignedRoles;
		}

		private IEnumerable<Identity> assignedEndUserRoles;

		private IEnumerable<ADObjectId> assignedRoles;

		private string mailboxPlanIndex;
	}
}
