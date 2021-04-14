using System;

namespace Microsoft.Exchange.EDiscovery.Export
{
	internal class DataContext
	{
		public DataContext(SourceInformation sourceInformation, IItemIdList itemIdList)
		{
			this.sourceInformation = sourceInformation;
			this.ItemIdList = itemIdList;
		}

		public SourceInformation SourceInformation
		{
			get
			{
				return this.sourceInformation;
			}
		}

		public string SourceId
		{
			get
			{
				return this.sourceInformation.Configuration.Id;
			}
		}

		public string SourceName
		{
			get
			{
				return this.sourceInformation.Configuration.Name;
			}
		}

		public string SourceLegacyExchangeDN
		{
			get
			{
				return this.sourceInformation.Configuration.LegacyExchangeDN;
			}
		}

		public ISourceDataProvider ServiceClient
		{
			get
			{
				return this.sourceInformation.ServiceClient;
			}
		}

		public IItemIdList ItemIdList { get; private set; }

		public bool IsUnsearchable
		{
			get
			{
				return this.ItemIdList != null && this.ItemIdList.IsUnsearchable;
			}
		}

		public bool IsPublicFolder
		{
			get
			{
				return this.SourceId.StartsWith("\\");
			}
		}

		public int ItemCount
		{
			get
			{
				if (!this.IsUnsearchable)
				{
					return this.sourceInformation.Status.ItemCount;
				}
				return this.sourceInformation.Status.UnsearchableItemCount;
			}
		}

		public int ProcessedItemCount
		{
			get
			{
				if (!this.IsUnsearchable)
				{
					return this.sourceInformation.Status.ProcessedItemCount;
				}
				return this.sourceInformation.Status.ProcessedUnsearchableItemCount;
			}
			set
			{
				if (this.IsUnsearchable)
				{
					this.sourceInformation.Status.ProcessedUnsearchableItemCount = value;
					return;
				}
				this.sourceInformation.Status.ProcessedItemCount = value;
			}
		}

		private SourceInformation sourceInformation;
	}
}
