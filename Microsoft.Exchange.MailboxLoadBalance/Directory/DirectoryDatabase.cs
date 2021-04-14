using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.MailboxLoadBalance.Data;
using Microsoft.Exchange.MailboxLoadBalance.Data.LoadMetrics;
using Microsoft.Exchange.MailboxLoadBalance.LoadBalance;
using Microsoft.Exchange.MailboxLoadBalance.Providers;
using Microsoft.Exchange.MailboxLoadBalance.ServiceSupport;

namespace Microsoft.Exchange.MailboxLoadBalance.Directory
{
	[DataContract]
	internal class DirectoryDatabase : DirectoryObject
	{
		public DirectoryDatabase(IDirectoryProvider directory, DirectoryIdentity identity, IClientFactory clientFactory, bool isExcludedFromProvisioning, bool isExcludedFromInitialProvisioning, MailboxProvisioningAttributes mailboxProvisioningAttributes = null) : base(directory, identity)
		{
			this.clientFactory = clientFactory;
			this.IsExcludedFromProvisioning = isExcludedFromProvisioning;
			this.IsExcludedFromInitialProvisioning = isExcludedFromInitialProvisioning;
			this.MailboxProvisioningAttributes = mailboxProvisioningAttributes;
		}

		public IEnumerable<DirectoryServer> ActivationOrder
		{
			get
			{
				return base.Directory.GetActivationPreferenceForDatabase(this);
			}
		}

		[DataMember]
		public bool IsExcludedFromInitialProvisioning { get; private set; }

		[DataMember]
		public bool IsExcludedFromProvisioning { get; private set; }

		[DataMember]
		public MailboxProvisioningAttributes MailboxProvisioningAttributes { get; set; }

		[IgnoreDataMember]
		public ByteQuantifiedSize MaximumSize { get; set; }

		[DataMember]
		public int RelativeLoadCapacity { get; set; }

		public virtual IEnumerable<NonConnectedMailbox> GetDisconnectedMailboxes()
		{
			return base.Directory.GetDisconnectedMailboxesForDatabase(this);
		}

		public virtual DirectoryMailbox GetMailbox(DirectoryIdentity identity)
		{
			return base.Directory.GetDirectoryObject(identity) as DirectoryMailbox;
		}

		public virtual IEnumerable<DirectoryMailbox> GetMailboxes()
		{
			return base.Directory.GetMailboxesForDatabase(this);
		}

		public virtual DatabaseSizeInfo GetSize()
		{
			DatabaseSizeInfo databaseSizeInformation;
			using (ILoadBalanceService loadBalanceClientForDatabase = this.clientFactory.GetLoadBalanceClientForDatabase(this))
			{
				databaseSizeInformation = loadBalanceClientForDatabase.GetDatabaseSizeInformation(base.Identity);
			}
			return databaseSizeInformation;
		}

		public LoadContainer ToLoadContainer()
		{
			DatabaseSizeInfo size = this.GetSize();
			LoadContainer loadContainer = new LoadContainer(this, ContainerType.Database);
			loadContainer.RelativeLoadWeight = this.RelativeLoadCapacity;
			loadContainer.CanAcceptRegularLoad = (!this.IsExcludedFromProvisioning && size.CurrentPhysicalSize < this.MaximumSize);
			loadContainer.CanAcceptBalancingLoad = (!this.IsExcludedFromInitialProvisioning && loadContainer.CanAcceptRegularLoad);
			loadContainer.MaximumLoad[PhysicalSize.Instance] = (long)this.MaximumSize.ToBytes();
			loadContainer.MaximumLoad[LogicalSize.Instance] = (long)this.MaximumSize.ToBytes();
			loadContainer.ReusableCapacity[LogicalSize.Instance] = (long)size.AvailableWhitespace.ToBytes();
			loadContainer.ReusableCapacity[PhysicalSize.Instance] = (long)size.AvailableWhitespace.ToBytes();
			ByteQuantifiedSize byteQuantifiedSize = size.CurrentPhysicalSize - size.AvailableWhitespace;
			loadContainer.ConsumedLoad[LogicalSize.Instance] = (long)byteQuantifiedSize.ToBytes();
			loadContainer.ConsumedLoad[PhysicalSize.Instance] = (long)byteQuantifiedSize.ToBytes();
			return loadContainer;
		}

		public virtual bool IsOwnedBy(DirectoryIdentity directoryIdentity)
		{
			DirectoryServer directoryServer = this.ActivationOrder.First<DirectoryServer>();
			return directoryServer != null && directoryServer.Identity.Equals(directoryIdentity);
		}

		private readonly IClientFactory clientFactory;
	}
}
