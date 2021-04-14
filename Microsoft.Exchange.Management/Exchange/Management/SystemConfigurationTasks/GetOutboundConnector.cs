using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Get", "OutboundConnector", DefaultParameterSetName = "Identity")]
	public class GetOutboundConnector : GetMultitenancySystemConfigurationObjectTask<OutboundConnectorIdParameter, TenantOutboundConnector>
	{
		protected override bool DeepSearch
		{
			get
			{
				return true;
			}
		}

		[Parameter(Mandatory = false)]
		public bool IsTransportRuleScoped
		{
			get
			{
				return (bool)base.Fields["IsTransportRuleScoped"];
			}
			set
			{
				base.Fields["IsTransportRuleScoped"] = value;
			}
		}

		protected override void WriteResult(IConfigurable dataObject)
		{
			if (base.Fields.IsModified("IsTransportRuleScoped"))
			{
				if (((TenantOutboundConnector)dataObject).IsTransportRuleScoped == this.IsTransportRuleScoped)
				{
					base.WriteResult(dataObject);
					return;
				}
			}
			else
			{
				base.WriteResult(dataObject);
			}
		}
	}
}
