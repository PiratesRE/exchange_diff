using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.TransportConfig
{
	[Cmdlet("Get", "TransportConfig")]
	public sealed class GetTransportConfig : GetMultitenancySingletonSystemConfigurationObjectTask<TransportConfigContainer>
	{
		protected override bool DeepSearch
		{
			get
			{
				return true;
			}
		}

		protected override void WriteResult(IConfigurable dataObject)
		{
			TransportConfigContainer transportConfigContainer = (TransportConfigContainer)dataObject;
			base.WriteResult(transportConfigContainer);
			IConfigurationSession configurationSession = (IConfigurationSession)base.DataSession;
			MessageDeliveryGlobalSettings[] array = configurationSession.Find<MessageDeliveryGlobalSettings>(configurationSession.GetOrgContainerId(), QueryScope.SubTree, new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Name, MessageDeliveryGlobalSettings.DefaultName), null, 1);
			if (array.Length > 0 && (array[0].MaxReceiveSize != transportConfigContainer.MaxReceiveSize || array[0].MaxSendSize != transportConfigContainer.MaxSendSize || array[0].MaxRecipientEnvelopeLimit != transportConfigContainer.MaxRecipientEnvelopeLimit) && !this.IsPureE12Environment())
			{
				this.WriteWarning(Strings.WarningMessageSizeRestrictionOutOfSync);
			}
		}

		private bool IsPureE12Environment()
		{
			IConfigurationSession configurationSession = (IConfigurationSession)base.DataSession;
			ADPagedReader<Server> adpagedReader = configurationSession.FindAllPaged<Server>();
			foreach (Server server in adpagedReader)
			{
				if (!server.IsExchange2007OrLater)
				{
					return false;
				}
			}
			return true;
		}
	}
}
