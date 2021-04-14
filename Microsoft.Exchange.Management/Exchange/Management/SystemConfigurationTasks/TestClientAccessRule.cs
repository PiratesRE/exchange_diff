using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Net;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Core.RuleTasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.ClientAccessRules;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.MessagingPolicies.Rules;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Test", "ClientAccessRule", SupportsShouldProcess = true)]
	public sealed class TestClientAccessRule : GetMultitenancySingletonSystemConfigurationObjectTask<ADClientAccessRule>
	{
		[Parameter(Mandatory = true)]
		public MailboxIdParameter User { get; set; }

		[Parameter(Mandatory = true)]
		public ClientAccessProtocol Protocol { get; set; }

		[Parameter(Mandatory = true)]
		public IPAddress RemoteAddress { get; set; }

		[Parameter(Mandatory = true)]
		public int RemotePort { get; set; }

		[Parameter(Mandatory = true)]
		public ClientAccessAuthenticationMethod AuthenticationType { get; set; }

		protected override void InternalProcessRecord()
		{
			this.RunClientAccessRules();
		}

		private void RunClientAccessRules()
		{
			long ticks = DateTime.UtcNow.Ticks;
			ClientAccessRuleCollection clientAccessRuleCollection = this.FetchClientAccessRulesCollection();
			ADRawEntry adrawEntry = this.FetchADRawEntry(this.User);
			string usernameFromADRawEntry = ClientAccessRulesUtils.GetUsernameFromADRawEntry(adrawEntry);
			base.WriteVerbose(RulesTasksStrings.TestClientAccessRuleFoundUsername(usernameFromADRawEntry));
			ClientAccessRulesEvaluationContext context = new ClientAccessRulesEvaluationContext(clientAccessRuleCollection, usernameFromADRawEntry, new IPEndPoint(this.RemoteAddress, this.RemotePort), this.Protocol, this.AuthenticationType, adrawEntry, ObjectSchema.GetInstance<ClientAccessRulesRecipientFilterSchema>(), delegate(ClientAccessRulesEvaluationContext evaluationContext)
			{
			}, delegate(Rule rule, ClientAccessRulesAction action)
			{
				ObjectId identity = null;
				ClientAccessRule clientAccessRule = rule as ClientAccessRule;
				if (clientAccessRule != null)
				{
					identity = clientAccessRule.Identity;
				}
				this.WriteResult(new ClientAccessRulesEvaluationResult
				{
					Identity = identity,
					Name = rule.Name,
					Action = action
				});
			}, ticks);
			clientAccessRuleCollection.Run(context);
		}

		private ADRawEntry FetchADRawEntry(MailboxIdParameter user)
		{
			OrganizationId organizationId = ((IConfigurationSession)base.DataSession).GetOrgContainer().OrganizationId;
			IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(true, ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(organizationId), 105, "FetchADRawEntry", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\SystemConfigurationTasks\\ClientAccessRules\\TestClientAccessRule.cs");
			tenantOrRootOrgRecipientSession.UseGlobalCatalog = true;
			List<ADUser> list = new List<ADUser>(base.GetDataObjects<ADUser>(user, tenantOrRootOrgRecipientSession, null));
			if (list.Count != 1)
			{
				base.WriteError(new RecipientTaskException(RulesTasksStrings.TestClientAccessRuleUserNotFoundOrMoreThanOne(user.ToString())), ErrorCategory.InvalidArgument, null);
			}
			return list[0];
		}

		private ClientAccessRuleCollection FetchClientAccessRulesCollection()
		{
			ClientAccessRuleCollection clientAccessRuleCollection = new ClientAccessRuleCollection((base.Identity == null) ? OrganizationId.ForestWideOrgId.ToString() : base.Identity.ToString());
			OrganizationId organizationId = ((IConfigurationSession)base.DataSession).GetOrgContainer().OrganizationId;
			if (organizationId != null && !OrganizationId.ForestWideOrgId.Equals(organizationId))
			{
				IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(true, ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromOrganizationIdWithoutRbacScopes(ADSystemConfigurationSession.GetRootOrgContainerIdForLocalForest(), OrganizationId.ForestWideOrgId, OrganizationId.ForestWideOrgId, false), 133, "FetchClientAccessRulesCollection", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\SystemConfigurationTasks\\ClientAccessRules\\TestClientAccessRule.cs");
				clientAccessRuleCollection.AddClientAccessRuleCollection(this.FetchClientAccessRulesCollection(tenantOrTopologyConfigurationSession));
			}
			clientAccessRuleCollection.AddClientAccessRuleCollection(this.FetchClientAccessRulesCollection((IConfigurationSession)base.DataSession));
			return clientAccessRuleCollection;
		}

		private ClientAccessRuleCollection FetchClientAccessRulesCollection(IConfigurationSession session)
		{
			ClientAccessRulesPriorityManager clientAccessRulesPriorityManager = new ClientAccessRulesPriorityManager(ClientAccessRulesStorageManager.GetClientAccessRules(session));
			ClientAccessRuleCollection clientAccessRuleCollection = new ClientAccessRuleCollection((base.Identity == null) ? OrganizationId.ForestWideOrgId.ToString() : base.Identity.ToString());
			foreach (ADClientAccessRule adclientAccessRule in clientAccessRulesPriorityManager.ADClientAccessRules)
			{
				ClientAccessRule clientAccessRule = adclientAccessRule.GetClientAccessRule();
				if (clientAccessRule.Enabled == RuleState.Disabled)
				{
					base.WriteVerbose(RulesTasksStrings.ClientAccessRuleWillBeConsideredEnabled(clientAccessRule.Name));
					clientAccessRule.Enabled = RuleState.Enabled;
				}
				base.WriteVerbose(RulesTasksStrings.ClientAccessRuleWillBeAddedToCollection(clientAccessRule.Name));
				clientAccessRuleCollection.Add(clientAccessRule);
			}
			return clientAccessRuleCollection;
		}
	}
}
