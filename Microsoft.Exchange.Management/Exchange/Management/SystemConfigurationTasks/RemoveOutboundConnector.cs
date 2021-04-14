using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.MessagingPolicies.Rules;
using Microsoft.Exchange.MessagingPolicies.Rules.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Remove", "OutboundConnector", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
	public class RemoveOutboundConnector : RemoveSystemConfigurationObjectTask<OutboundConnectorIdParameter, TenantOutboundConnector>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageRemoveOutboundConnector(this.Identity.ToString());
			}
		}

		protected override void InternalValidate()
		{
			base.InternalValidate();
			try
			{
				ManageTenantOutboundConnectors.ValidateIfAcceptedDomainsCanBeRoutedWithConnectors(base.DataObject, base.DataSession, this, true);
				IEnumerable<TransportRule> source;
				if (Utils.TryGetTransportRules(base.DataSession, new Utils.TransportRuleSelectionDelegate(Utils.RuleHasOutboundConnectorReference), out source, base.DataObject.Name) && source.Any<TransportRule>())
				{
					base.WriteError(new ConnectorIncorrectUsageConnectorStillReferencedException(), ErrorCategory.InvalidOperation, null);
				}
			}
			catch (ParserException exception)
			{
				base.WriteError(exception, ErrorCategory.InvalidData, null);
			}
		}
	}
}
