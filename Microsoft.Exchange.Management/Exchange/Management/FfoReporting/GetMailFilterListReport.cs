using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.FfoReporting.Common;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.MessagingPolicies.CompliancePrograms.Tasks;
using Microsoft.Exchange.MessagingPolicies.Rules.Tasks;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Management.FfoReporting
{
	[Cmdlet("Get", "MailFilterListReport")]
	[OutputType(new Type[]
	{
		typeof(MailFilterListReport)
	})]
	public sealed class GetMailFilterListReport : FfoReportingTask<MailFilterListReport>
	{
		public GetMailFilterListReport()
		{
			this.Domain = new MultiValuedProperty<Fqdn>();
			this.delegates.Add(GetMailFilterListReport.SelectionTargets.DlpPolicy.ToString().ToLower(), new GetMailFilterListReport.GetFilterListDelegate(this.GetDlpPolicies));
			this.delegates.Add(GetMailFilterListReport.SelectionTargets.DlpRule.ToString().ToLower(), new GetMailFilterListReport.GetFilterListDelegate(this.GetDlpRules));
			this.delegates.Add(GetMailFilterListReport.SelectionTargets.TransportRule.ToString().ToLower(), new GetMailFilterListReport.GetFilterListDelegate(this.GetPolicyRules));
			this.delegates.Add(GetMailFilterListReport.SelectionTargets.Domain.ToString().ToLower(), new GetMailFilterListReport.GetFilterListDelegate(this.GetDomains));
			this.delegates.Add(GetMailFilterListReport.SelectionTargets.EventTypes.ToString().ToLower(), new GetMailFilterListReport.GetFilterListDelegate(this.GetEventTypes));
			this.delegates.Add(GetMailFilterListReport.SelectionTargets.Actions.ToString().ToLower(), new GetMailFilterListReport.GetFilterListDelegate(this.GetActions));
			this.delegates.Add(GetMailFilterListReport.SelectionTargets.FindOnPremConnector.ToString().ToLower(), new GetMailFilterListReport.GetFilterListDelegate(this.FindOnPremConnector));
			this.delegates.Add(GetMailFilterListReport.SelectionTargets.Sources.ToString().ToLower(), new GetMailFilterListReport.GetFilterListDelegate(this.GetSources));
		}

		[QueryParameter("DomainListQueryDefinition", new string[]
		{

		})]
		[Parameter(Mandatory = false, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
		public MultiValuedProperty<Fqdn> Domain { get; set; }

		[Parameter(Mandatory = false)]
		[CmdletValidator("ValidateEnum", new object[]
		{
			typeof(GetMailFilterListReport.SelectionTargets)
		}, ErrorMessage = Strings.IDs.InvalidSelectionTarget)]
		public MultiValuedProperty<string> SelectionTarget
		{
			get
			{
				return this.selectionTarget;
			}
			set
			{
				this.selectionTarget = value;
			}
		}

		public override string ComponentName
		{
			get
			{
				return ExchangeComponent.FfoRws.Name;
			}
		}

		public override string MonitorEventName
		{
			get
			{
				return "FFO GetFilterValueList Status Monitor";
			}
		}

		protected override void CustomInternalValidate()
		{
			base.CustomInternalValidate();
			if (this.SelectionTarget.Count == 0)
			{
				this.SelectionTarget.Add(GetMailFilterListReport.SelectionTargets.DlpPolicy.ToString());
				this.SelectionTarget.Add(GetMailFilterListReport.SelectionTargets.DlpRule.ToString());
				this.SelectionTarget.Add(GetMailFilterListReport.SelectionTargets.TransportRule.ToString());
				this.SelectionTarget.Add(GetMailFilterListReport.SelectionTargets.Domain.ToString());
				this.SelectionTarget.Add(GetMailFilterListReport.SelectionTargets.EventTypes.ToString());
				this.SelectionTarget.Add(GetMailFilterListReport.SelectionTargets.Actions.ToString());
				this.SelectionTarget.Add(GetMailFilterListReport.SelectionTargets.FindOnPremConnector.ToString());
				this.SelectionTarget.Add(GetMailFilterListReport.SelectionTargets.Sources.ToString());
			}
		}

		protected override IReadOnlyList<MailFilterListReport> AggregateOutput()
		{
			List<MailFilterListReport> list = new List<MailFilterListReport>();
			foreach (string text in this.SelectionTarget)
			{
				GetMailFilterListReport.GetFilterListDelegate getFilterListDelegate;
				if (!string.IsNullOrEmpty(text) && this.delegates.TryGetValue(text.ToLower(), out getFilterListDelegate))
				{
					try
					{
						base.Diagnostics.StartTimer(text);
						GetMailFilterListReport.SelectionTargets selectionTargets = (GetMailFilterListReport.SelectionTargets)Enum.Parse(typeof(GetMailFilterListReport.SelectionTargets), text, true);
						getFilterListDelegate(selectionTargets.ToString(), list);
					}
					finally
					{
						base.Diagnostics.StopTimer(text);
					}
				}
			}
			list.Sort(new Comparison<MailFilterListReport>(GetMailFilterListReport.CompareMailFilterListReport));
			return list;
		}

		private static int CompareMailFilterListReport(MailFilterListReport first, MailFilterListReport second)
		{
			if (first == null)
			{
				if (second == null)
				{
					return 0;
				}
				return -1;
			}
			else
			{
				if (second == null)
				{
					return 1;
				}
				int num = string.Compare(first.SelectionTarget, second.SelectionTarget);
				if (num == 0)
				{
					num = string.Compare(first.Display, second.Display);
				}
				return num;
			}
		}

		private void GetDlpPolicies(string target, List<MailFilterListReport> values)
		{
			if (Schema.Utilities.HasDlpRole(this))
			{
				foreach (ADComplianceProgram adcomplianceProgram in DlpUtils.GetInstalledTenantDlpPolicies(base.ConfigSession))
				{
					values.Add(new MailFilterListReport
					{
						Organization = base.Organization.ToString(),
						SelectionTarget = target,
						Display = adcomplianceProgram.Name,
						Value = adcomplianceProgram.Name
					});
				}
			}
		}

		private void GetDlpRules(string target, List<MailFilterListReport> values)
		{
			if (Schema.Utilities.HasDlpRole(this))
			{
				values.AddRange(from rule in DlpUtils.GetTransportRules(base.ConfigSession, (Rule rule) => rule.DlpPolicyId != Guid.Empty)
				select new MailFilterListReport
				{
					Organization = this.Organization.ToString(),
					SelectionTarget = target,
					Display = rule.Name,
					Value = rule.Name,
					ParentTarget = GetMailFilterListReport.SelectionTargets.DlpPolicy.ToString(),
					ParentValue = rule.DlpPolicy
				});
			}
		}

		private void GetPolicyRules(string target, List<MailFilterListReport> values)
		{
			values.AddRange(from rule in DlpUtils.GetTransportRules(base.ConfigSession, (Rule rule) => rule.DlpPolicyId == Guid.Empty)
			select new MailFilterListReport
			{
				Organization = this.Organization.ToString(),
				SelectionTarget = target,
				Display = rule.Name,
				Value = rule.Name
			});
		}

		private void GetDomains(string target, List<MailFilterListReport> values)
		{
			AcceptedDomainIdParameter acceptedDomainIdParameter = AcceptedDomainIdParameter.Parse("*");
			foreach (AcceptedDomain acceptedDomain in acceptedDomainIdParameter.GetObjects<AcceptedDomain>(null, base.ConfigSession))
			{
				values.Add(new MailFilterListReport
				{
					Organization = base.Organization.ToString(),
					SelectionTarget = target,
					Display = acceptedDomain.Name,
					Value = acceptedDomain.Name
				});
			}
		}

		private void GetEventTypes(string target, List<MailFilterListReport> values)
		{
			IList<string> eventTypes = Schema.Utilities.GetEventTypes(this);
			foreach (string text in eventTypes)
			{
				values.Add(new MailFilterListReport
				{
					Organization = base.Organization.ToString(),
					SelectionTarget = target,
					Display = text,
					Value = text
				});
			}
		}

		private void GetSources(string target, List<MailFilterListReport> values)
		{
			IList<string> sources = Schema.Utilities.GetSources(this);
			foreach (string text in sources)
			{
				values.Add(new MailFilterListReport
				{
					Organization = base.Organization.ToString(),
					SelectionTarget = target,
					Display = text,
					Value = text
				});
			}
		}

		private void GetActions(string target, List<MailFilterListReport> values)
		{
			string[] names = Enum.GetNames(typeof(Schema.Actions));
			foreach (string text in names)
			{
				values.Add(new MailFilterListReport
				{
					Organization = base.Organization.ToString(),
					SelectionTarget = target,
					Display = text,
					Value = text
				});
			}
		}

		private void FindOnPremConnector(string target, List<MailFilterListReport> values)
		{
			if (this.Domain.Count == 0)
			{
				return;
			}
			OutboundConnectorIdParameter outboundConnectorIdParameter = OutboundConnectorIdParameter.Parse("*");
			TenantOutboundConnector tenantOutboundConnector = null;
			int num = -1;
			string domain = this.Domain[0].ToString();
			foreach (TenantOutboundConnector tenantOutboundConnector2 in outboundConnectorIdParameter.GetObjects<TenantOutboundConnector>(null, base.ConfigSession))
			{
				if (tenantOutboundConnector2.Enabled && tenantOutboundConnector2.ConnectorType == TenantConnectorType.OnPremises)
				{
					if (tenantOutboundConnector2.RecipientDomains.Any((SmtpDomainWithSubdomains smtpDomain) => smtpDomain.Domain.Equals(domain, StringComparison.InvariantCultureIgnoreCase)))
					{
						tenantOutboundConnector = tenantOutboundConnector2;
						break;
					}
					foreach (SmtpDomainWithSubdomains smtpDomainWithSubdomains in tenantOutboundConnector2.RecipientDomains)
					{
						WildcardPattern wildcardPattern = new WildcardPattern(smtpDomainWithSubdomains.Address);
						int num2 = wildcardPattern.Match(domain);
						if (num2 > num)
						{
							num = num2;
							tenantOutboundConnector = tenantOutboundConnector2;
						}
					}
				}
			}
			if (tenantOutboundConnector != null)
			{
				values.Add(new MailFilterListReport
				{
					Organization = base.Organization.ToString(),
					SelectionTarget = target,
					Display = "Name",
					Value = tenantOutboundConnector.Name
				});
			}
		}

		private Dictionary<string, GetMailFilterListReport.GetFilterListDelegate> delegates = new Dictionary<string, GetMailFilterListReport.GetFilterListDelegate>();

		private MultiValuedProperty<string> selectionTarget = new MultiValuedProperty<string>();

		[Flags]
		private enum SelectionTargets
		{
			Actions = 0,
			DlpPolicy = 1,
			DlpRule = 2,
			Domain = 3,
			EventTypes = 4,
			FindOnPremConnector = 5,
			TransportRule = 6,
			Sources = 7
		}

		private delegate void GetFilterListDelegate(string target, List<MailFilterListReport> list);
	}
}
