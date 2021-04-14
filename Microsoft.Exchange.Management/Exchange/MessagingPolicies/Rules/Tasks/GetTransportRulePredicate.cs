using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.MessagingPolicies.Rules.Tasks
{
	[OutputType(new Type[]
	{
		typeof(TransportRulePredicate)
	})]
	[Cmdlet("Get", "TransportRulePredicate")]
	public sealed class GetTransportRulePredicate : Task
	{
		[Parameter(Mandatory = false, Position = 0)]
		public string Name
		{
			get
			{
				return this.name;
			}
			set
			{
				this.name = value;
			}
		}

		protected override void InternalProcessRecord()
		{
			bool enabled = VariantConfiguration.InvariantNoFlightingSnapshot.CompliancePolicy.ShowSupervisionPredicate.Enabled;
			IConfigDataProvider session = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 49, "InternalProcessRecord", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\SystemConfigurationTasks\\TransportRule\\GetTransportRulePredicate.cs");
			TypeMapping[] availablePredicateMappings = TransportRulePredicate.GetAvailablePredicateMappings();
			if (string.IsNullOrEmpty(this.name))
			{
				foreach (TransportRulePredicate transportRulePredicate in TransportRulePredicate.CreateAllAvailablePredicates(availablePredicateMappings, session))
				{
					if (enabled || (!(transportRulePredicate is SenderInRecipientListPredicate) && !(transportRulePredicate is RecipientInSenderListPredicate)))
					{
						base.WriteObject(transportRulePredicate);
					}
				}
				return;
			}
			TransportRulePredicate transportRulePredicate2 = TransportRulePredicate.CreatePredicate(availablePredicateMappings, this.name, session);
			if (!enabled && (transportRulePredicate2 is SenderInRecipientListPredicate || transportRulePredicate2 is RecipientInSenderListPredicate))
			{
				transportRulePredicate2 = null;
			}
			if (transportRulePredicate2 == null)
			{
				base.WriteError(new ArgumentException(Strings.InvalidPredicate, "Name"), ErrorCategory.InvalidArgument, this.Name);
				return;
			}
			base.WriteObject(transportRulePredicate2);
		}

		private string name;
	}
}
