using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.MailboxLoadBalance.Config;
using Microsoft.Exchange.MailboxLoadBalance.Providers;

namespace Microsoft.Exchange.MailboxLoadBalance.Directory
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[DataContract]
	internal abstract class DirectoryContainerParent : DirectoryObject
	{
		protected DirectoryContainerParent(IDirectoryProvider directory, DirectoryIdentity identity) : base(directory, identity)
		{
		}

		public IEnumerable<DirectoryObject> Children
		{
			get
			{
				if (this.ShouldRetrieveChildren)
				{
					List<DirectoryObject> list = new List<DirectoryObject>();
					foreach (DirectoryObject directoryObject in this.FetchChildren())
					{
						directoryObject.Parent = this;
						list.Add(directoryObject);
					}
					this.children = list;
					this.childrenRetrievalTimestamp = ExDateTime.UtcNow;
				}
				return this.children;
			}
		}

		protected virtual bool ShouldRetrieveChildren
		{
			get
			{
				return this.children == null || this.childrenRetrievalTimestamp.Add(LoadBalanceADSettings.Instance.Value.LocalCacheRefreshPeriod) < ExDateTime.UtcNow;
			}
		}

		protected abstract IEnumerable<DirectoryObject> FetchChildren();

		private IList<DirectoryObject> children;

		private ExDateTime childrenRetrievalTimestamp = ExDateTime.MinValue;
	}
}
