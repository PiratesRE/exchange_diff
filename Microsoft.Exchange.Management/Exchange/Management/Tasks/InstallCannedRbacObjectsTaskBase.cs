using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.Deployment;

namespace Microsoft.Exchange.Management.Tasks
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	public abstract class InstallCannedRbacObjectsTaskBase : SetupTaskBase
	{
		[Parameter(Mandatory = true, ParameterSetName = "Organization")]
		[ValidateNotNullOrEmpty]
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

		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = true, ParameterSetName = "Organization")]
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

		[Parameter(Mandatory = false)]
		[ValidateNotNullOrEmpty]
		public InvocationMode InvocationMode
		{
			get
			{
				return (InvocationMode)(base.Fields["InvocationMode"] ?? InvocationMode.Install);
			}
			set
			{
				base.Fields["InvocationMode"] = value;
			}
		}

		protected override void InternalProcessRecord()
		{
			base.InternalProcessRecord();
			try
			{
				this.rolesContainerId = base.OrgContainerId.GetDescendantId(ExchangeRole.RdnContainer);
				this.roleAssignmentContainerId = base.OrgContainerId.GetDescendantId(ExchangeRoleAssignment.RdnContainer);
			}
			catch (OrgContainerNotFoundException exception)
			{
				base.WriteError(exception, ErrorCategory.ObjectNotFound, null);
			}
			catch (TenantOrgContainerNotFoundException exception2)
			{
				base.WriteError(exception2, ErrorCategory.ObjectNotFound, null);
			}
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			this.rolesContainerId = null;
		}

		private bool FindAnyRoleAssignment()
		{
			ExchangeRoleAssignment[] array = this.configurationSession.Find<ExchangeRoleAssignment>(base.OrgContainerId, QueryScope.SubTree, null, null, 1);
			int num = 0;
			if (num >= array.Length)
			{
				return false;
			}
			ExchangeRoleAssignment exchangeRoleAssignment = array[num];
			return true;
		}

		protected ExchangeBuild GetCurrentRBACConfigVersion(Container rbacContainer)
		{
			if (rbacContainer == null)
			{
				throw new ArgumentNullException("rbacContainer");
			}
			ExchangeBuild exchangeBuild = rbacContainer.ExchangeVersion.ExchangeBuild;
			if (!(exchangeBuild <= RbacContainer.InitialRBACBuild))
			{
				return exchangeBuild;
			}
			if (this.FindAnyRoleAssignment())
			{
				return RbacContainer.E14RTMBuild;
			}
			return RbacContainer.FirstRGRABuild;
		}

		protected ADObjectId rolesContainerId;

		protected ADObjectId roleAssignmentContainerId;

		internal static string[] ObsoleteRoleNamesEnterprise = new string[]
		{
			"ApplicationImpersonation_Enterprise",
			"CustomScripts_Enterprise",
			"Helpdesk_Enterprise",
			"RecordsManagement_Enterprise",
			"Reset Password",
			"UMPromptManagement",
			"UmRecipientManagement",
			"UnScopedRoleManagement",
			"MyMailSubscriptions",
			"GALSynchronizationManagement",
			"Reporting"
		};

		internal static string[] ObsoleteRoleNamesDatacenter = new string[]
		{
			"UnScopedRoleManagement",
			"CustomScripts"
		};

		internal static string[] ObsoleteRoleNamesTenant = new string[]
		{
			"CustomScripts",
			"UMPromptManagement",
			"UmRecipientManagement"
		};

		internal static string[] ObsoleteRoleNamesHosting = new string[0];

		internal static string[] ObsoleteRoleNamesHostedTenant = new string[0];

		internal static RoleNameMappingCollection RoleNameMappingEnterpriseR4 = new RoleNameMappingCollection
		{
			new RoleNameMapping("RecipientManagement", new string[]
			{
				"Distribution Groups",
				"Mail Enabled Public Folders",
				"Mail Recipient Creation",
				"Mail Recipients",
				"Message Tracking",
				"Migration",
				"Move Mailboxes",
				"Recipient Management",
				"Recipient Policies",
				"Reset Password"
			}),
			new RoleNameMapping("UmManagement", new string[]
			{
				"UM Mailboxes",
				"UM Prompts",
				"Unified Messaging"
			}),
			new RoleNameMapping("RecordsManagement", new string[]
			{
				"Audit Logs",
				"Journaling",
				"Message Tracking",
				"Retention Management",
				"Transport Rules"
			}),
			new RoleNameMapping("DiscoveryManagement", new string[]
			{
				"Legal Hold",
				"Mailbox Search"
			}),
			new RoleNameMapping("ViewOnlyOrganizationManagement", new string[]
			{
				"View-Only Recipients",
				"View-Only Configuration",
				"Monitoring"
			}),
			new RoleNameMapping("CAS Mailbox Policies", "Recipient Policies"),
			new RoleNameMapping("OrganizationManagement", new string[]
			{
				"Active Directory Permissions",
				"Address Lists",
				"Audit Logs",
				"Cmdlet Extension Agents",
				"Database Availability Groups",
				"Database Copies",
				"Databases",
				"DataCenter Operations",
				"Disaster Recovery",
				"Distribution Groups",
				"Edge Subscriptions",
				"E-Mail Address Policies",
				"Exchange Connectors",
				"Exchange Server Certificates",
				"Exchange Servers",
				"Exchange Virtual Directories",
				"Federated Sharing",
				"Information Rights Management",
				"Journaling",
				"Legal Hold",
				"Mail Enabled Public Folders",
				"Mail Recipient Creation",
				"Mail Recipients",
				"Mail Tips",
				"Mailbox Search",
				"Message Tracking",
				"Migration",
				"Monitoring",
				"Move Mailboxes",
				"Organization Client Access",
				"Organization Configuration",
				"Organization Transport Settings",
				"POP3 And IMAP4 Protocols",
				"Public Folders",
				"Receive Connectors",
				"Recipient Policies",
				"Remote and Accepted Domains",
				"Reset Password",
				"Retention Management",
				"Role Management",
				"Security Group Creation and Membership",
				"Send Connectors",
				"Supervision",
				"Support Diagnostics",
				"Transport Agents",
				"Transport Hygiene",
				"Transport Queues",
				"Transport Rules",
				"UM Mailboxes",
				"UM Prompts",
				"Unified Messaging",
				"User Options",
				"View-Only Configuration",
				"View-Only Recipients"
			}),
			new RoleNameMapping("MyOptions", new string[]
			{
				"MyBaseOptions",
				"MyProfileInformation",
				"MyMailSubscriptions",
				"MyContactInformation",
				"MyRetentionPolicies",
				"MyTextMessaging",
				"MyVoiceMail"
			}),
			new RoleNameMapping("Public Folder Replication", new string[]
			{
				"Mail Enabled Public Folders",
				"Public Folders"
			})
		};

		internal static RoleNameMappingCollection RoleNameMappingDatacenterR4 = new RoleNameMappingCollection
		{
			new RoleNameMapping("MyOptions", new string[]
			{
				"MyBaseOptions",
				"MyContactInformation",
				"MyMailSubscriptions",
				"MyProfileInformation",
				"MyRetentionPolicies",
				"MyTextMessaging",
				"MyVoiceMail"
			}),
			new RoleNameMapping("CAS Mailbox Policies", "Recipient Policies"),
			new RoleNameMapping("Public Folder Replication", new string[]
			{
				"Mail Enabled Public Folders",
				"Public Folders"
			})
		};

		internal static RoleNameMappingCollection RoleNameMappingTenantR4 = new RoleNameMappingCollection();

		internal static RoleNameMappingCollection RoleNameMappingHostingR4 = new RoleNameMappingCollection();

		internal static RoleNameMappingCollection RoleNameMappingHostedTenantR4 = new RoleNameMappingCollection();
	}
}
