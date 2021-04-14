using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxLoadBalance.Data;
using Microsoft.Exchange.MailboxLoadBalance.Data.LoadMetrics;

namespace Microsoft.Exchange.MailboxLoadBalance.Provisioning
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[DataContract]
	internal class MailboxProvisioningData : IExtensibleDataObject
	{
		public MailboxProvisioningData(ByteQuantifiedSize totalDataSize, IMailboxProvisioningConstraints mailboxProvisioningConstraintses = null, LoadMetricStorage consumedLoad = null)
		{
			this.TotalDataSize = totalDataSize;
			this.MailboxProvisioningConstraints = mailboxProvisioningConstraintses;
			if (consumedLoad == null)
			{
				this.ConsumedLoad = new LoadMetricStorage();
				this.ConsumedLoad[PhysicalSize.Instance] = PhysicalSize.Instance.GetUnitsForSize(totalDataSize);
				return;
			}
			this.ConsumedLoad = consumedLoad;
		}

		public MailboxProvisioningData()
		{
			this.ConsumedLoad = new LoadMetricStorage();
		}

		public ExtensionDataObject ExtensionData { get; set; }

		[DataMember]
		public ByteQuantifiedSize TotalDataSize { get; private set; }

		[DataMember]
		public IMailboxProvisioningConstraints MailboxProvisioningConstraints { get; private set; }

		[DataMember]
		public LoadMetricStorage ConsumedLoad { get; set; }

		public override string ToString()
		{
			return string.Format("ProvisioningData['{0}' bytes, '{1}' consumed load, '{2}' constraint.", this.TotalDataSize, this.ConsumedLoad, this.MailboxProvisioningConstraints);
		}
	}
}
