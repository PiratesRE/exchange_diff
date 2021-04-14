using System;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	internal class RemoteRoutingAddressGenerator
	{
		public RemoteRoutingAddressGenerator(IConfigurationSession session)
		{
			QueryFilter filter = new BitMaskAndFilter(DomainContentConfigSchema.AcceptMessageTypes, 256UL);
			DomainContentConfig[] array = session.Find<DomainContentConfig>(session.GetOrgContainerId(), QueryScope.SubTree, filter, null, 1);
			if (array.Length > 0)
			{
				this.targetDeliveryDomain = array[0].DomainName.Domain;
			}
		}

		public ProxyAddress GenerateRemoteRoutingAddress(string alias, Task.ErrorLoggerDelegate errorWriter)
		{
			if (string.IsNullOrEmpty(this.targetDeliveryDomain))
			{
				errorWriter(new ErrorCannotFindTargetDeliveryDomainException(), ExchangeErrorCategory.Client, null);
			}
			return ProxyAddress.Parse(alias + "@" + this.targetDeliveryDomain);
		}

		private readonly string targetDeliveryDomain;
	}
}
