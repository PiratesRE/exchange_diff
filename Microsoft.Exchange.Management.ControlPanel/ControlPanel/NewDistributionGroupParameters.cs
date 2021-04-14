using System;
using System.Management.Automation;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Management.RecipientTasks;
using Microsoft.Exchange.PowerShell.RbacHostingTools;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class NewDistributionGroupParameters : WebServiceParameters
	{
		public override string AssociatedCmdlet
		{
			get
			{
				return "New-DistributionGroup";
			}
		}

		public override string RbacScope
		{
			get
			{
				return "@W:MyDistributionGroups|Organization";
			}
		}

		[OnDeserialized]
		private void OnDeserialized(StreamingContext context)
		{
			if (RbacPrincipal.Current.IsInRole("MultiTenant") && (this.PrimaryEAAlias != null || this.DomainName != null))
			{
				this.PrimaryEAAlias.FaultIfNullOrEmpty(Strings.AliasForDataCenterNotSetError);
				this.DomainName.FaultIfNullOrEmpty(Strings.GroupEmailDomainNameNotSetError);
				this.PrimarySmtpAddress = this.PrimaryEAAlias + "@" + this.DomainName;
			}
		}

		[DataMember]
		public string Name
		{
			get
			{
				return (string)base[ADObjectSchema.Name];
			}
			set
			{
				base[ADObjectSchema.Name] = value;
			}
		}

		[DataMember]
		public string Alias
		{
			get
			{
				return (string)base[MailEnabledRecipientSchema.Alias];
			}
			set
			{
				base[MailEnabledRecipientSchema.Alias] = value;
			}
		}

		[DataMember]
		public Identity OrganizationalUnit
		{
			get
			{
				return Identity.ParseIdentity((string)base[MailEnabledRecipientSchema.OrganizationalUnit]);
			}
			set
			{
				base[MailEnabledRecipientSchema.OrganizationalUnit] = value.ToIdParameter();
			}
		}

		[DataMember]
		public string PrimaryEAAlias { get; set; }

		[DataMember]
		public string DomainName { get; set; }

		public string PrimarySmtpAddress
		{
			get
			{
				return (string)base[MailEnabledRecipientSchema.PrimarySmtpAddress];
			}
			set
			{
				base[MailEnabledRecipientSchema.PrimarySmtpAddress] = value;
			}
		}

		public bool IgnoreNamingPolicy
		{
			get
			{
				return base.ParameterIsSpecified("IgnoreNamingPolicy") && ((SwitchParameter)base["IgnoreNamingPolicy"]).ToBool();
			}
			set
			{
				base["IgnoreNamingPolicy"] = new SwitchParameter(value);
			}
		}

		[DataMember]
		public bool IsSecurityGroupMemberJoinApprovalRequired
		{
			get
			{
				return this.MemberJoinRestriction == "ApprovalRequired";
			}
			set
			{
				this.MemberJoinRestriction = (value ? "ApprovalRequired" : "Closed");
			}
		}

		[DataMember]
		public string MemberJoinRestriction
		{
			get
			{
				return (string)base[DistributionGroupSchema.MemberJoinRestriction];
			}
			set
			{
				base[DistributionGroupSchema.MemberJoinRestriction] = value;
			}
		}

		[DataMember]
		public string MemberDepartRestriction
		{
			get
			{
				return (string)base[DistributionGroupSchema.MemberDepartRestriction];
			}
			set
			{
				base[DistributionGroupSchema.MemberDepartRestriction] = value;
			}
		}

		[DataMember]
		public Identity[] ManagedBy
		{
			get
			{
				return Identity.FromIdParameters(base[DistributionGroupSchema.ManagedBy]);
			}
			set
			{
				base[DistributionGroupSchema.ManagedBy] = value.ToIdParameters();
			}
		}

		[DataMember]
		public bool CopyOwnerToMember
		{
			get
			{
				return (bool)(base["CopyOwnerToMember"] ?? false);
			}
			set
			{
				base["CopyOwnerToMember"] = value;
			}
		}

		[DataMember]
		public bool IsSecurityGroupType
		{
			get
			{
				return (GroupType)(base["Type"] ?? GroupType.Distribution) == GroupType.Security;
			}
			set
			{
				if (value)
				{
					base["Type"] = GroupType.Security;
				}
			}
		}

		[DataMember]
		public string Notes
		{
			get
			{
				return (string)base[WindowsGroupSchema.Notes];
			}
			set
			{
				base[WindowsGroupSchema.Notes] = value;
			}
		}

		[DataMember]
		public Identity[] Members
		{
			get
			{
				return Identity.FromIdParameters(base[WindowsGroupSchema.Members]);
			}
			set
			{
				base[WindowsGroupSchema.Members] = value.ToIdParameters();
			}
		}

		public const string RbacParameters_Enterprise = "?Name&Alias";

		public const string RbacParameters_MultiTenant = "?Name&Alias&PrimarySmtpAddress";
	}
}
