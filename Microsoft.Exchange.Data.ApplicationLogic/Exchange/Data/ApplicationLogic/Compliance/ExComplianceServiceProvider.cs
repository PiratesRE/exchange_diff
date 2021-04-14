using System;
using System.Text;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.UnifiedPolicy;
using Microsoft.Office.CompliancePolicy;
using Microsoft.Office.CompliancePolicy.ComplianceData;
using Microsoft.Office.CompliancePolicy.ComplianceTask;
using Microsoft.Office.CompliancePolicy.PolicyConfiguration;
using Microsoft.Office.CompliancePolicy.PolicyEvaluation;

namespace Microsoft.Exchange.Data.ApplicationLogic.Compliance
{
	internal class ExComplianceServiceProvider : ComplianceServiceProvider
	{
		public string PreferredDomainController { get; set; }

		internal ExComplianceServiceProvider()
		{
		}

		internal ExComplianceServiceProvider(string preferredDomainController, ExecutionLog logger)
		{
			this.PreferredDomainController = preferredDomainController;
			this.logger = logger;
		}

		public override Auditor GetAuditor()
		{
			throw new NotImplementedException();
		}

		public override ExecutionLog GetExecutionLog()
		{
			throw new NotImplementedException();
		}

		public override ComplianceItemPagedReader GetPagedReader(ComplianceItemContainer container)
		{
			return ExComplianceServiceProvider.GetExComplianceContainer(container).ComplianceItemPagedReader;
		}

		public override PolicyConfigProvider GetPolicyStore(ComplianceItemContainer rootContainer)
		{
			MailboxSession session = ExComplianceServiceProvider.GetExComplianceContainer(rootContainer).Session;
			OrganizationId organizationId = session.OrganizationId;
			return this.GetPolicyStore(organizationId);
		}

		public override PolicyConfigProvider GetPolicyStore(string tenantId)
		{
			if (tenantId == null)
			{
				throw new ArgumentNullException("tenantId");
			}
			OrganizationId organizationId;
			if (!OrganizationId.TryCreateFromBytes(Convert.FromBase64String(tenantId), Encoding.UTF8, out organizationId))
			{
				throw new ArgumentException("Cannot create OrganizationId from such tenantId: " + tenantId);
			}
			return this.GetPolicyStore(organizationId);
		}

		public PolicyConfigProvider GetPolicyStore(OrganizationId organizationId)
		{
			if (organizationId == null)
			{
				throw new ArgumentNullException("organizationId");
			}
			ExPolicyConfigProvider exPolicyConfigProvider = (ExPolicyConfigProvider)PolicyConfigProviderManager<ExPolicyConfigProviderManager>.Instance.CreateForProcessingEngine(organizationId, this.logger, this.PreferredDomainController);
			this.PreferredDomainController = exPolicyConfigProvider.LastUsedDc;
			return exPolicyConfigProvider;
		}

		public override RuleParser GetRuleParser()
		{
			if (this.ruleParser == null)
			{
				this.ruleParser = new RuleParser(new SimplePolicyParserFactory());
			}
			return this.ruleParser;
		}

		public override ComplianceItemContainer GetComplianceItemContainer(string tenantId, string scope)
		{
			if (tenantId == null)
			{
				throw new ArgumentNullException("Either this.session or tenantId should be not null");
			}
			OrganizationId scopingOrganizationId;
			if (!OrganizationId.TryCreateFromBytes(Convert.FromBase64String(tenantId), Encoding.UTF8, out scopingOrganizationId))
			{
				throw new ArgumentException("Cannot create OrganizationId from such tenantId: " + tenantId);
			}
			IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(true, ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(scopingOrganizationId), 169, "GetComplianceItemContainer", "f:\\15.00.1497\\sources\\dev\\data\\src\\ApplicationLogic\\Compliance\\ExComplianceServiceProvider.cs");
			return new ExMailboxComplianceItemContainer(tenantOrRootOrgRecipientSession, scope);
		}

		private static ExComplianceItemContainer GetExComplianceContainer(ComplianceItemContainer container)
		{
			ExComplianceItemContainer exComplianceItemContainer = container as ExComplianceItemContainer;
			if (exComplianceItemContainer != null)
			{
				return exComplianceItemContainer;
			}
			throw new ArgumentException("Operation can be invoked only with an ExComplianceContainer");
		}

		private readonly ExecutionLog logger;

		private RuleParser ruleParser;
	}
}
