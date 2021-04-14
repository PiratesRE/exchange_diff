using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.AnchorService;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxLoadBalance.Directory;

namespace Microsoft.Exchange.MailboxLoadBalance.Data
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[DataContract]
	internal class LoadEntity
	{
		public LoadEntity(DirectoryObject directoryObject)
		{
			AnchorUtil.ThrowOnNullArgument(directoryObject, "directoryObject");
			this.DirectoryObject = directoryObject;
			this.DirectoryObjectIdentity = directoryObject.Identity;
			this.ConsumedLoad = new LoadMetricStorage();
		}

		[DataMember]
		public LoadMetricStorage ConsumedLoad { get; set; }

		[IgnoreDataMember]
		public DirectoryObject DirectoryObject { get; set; }

		[DataMember]
		public DirectoryIdentity DirectoryObjectIdentity { get; private set; }

		public Guid Guid
		{
			get
			{
				return this.DirectoryObjectIdentity.Guid;
			}
		}

		public string Name
		{
			get
			{
				return this.DirectoryObjectIdentity.Name;
			}
		}

		[DataMember]
		public LoadContainer Parent
		{
			get
			{
				return this.parent;
			}
			set
			{
				if (this.parent != null && this.parent != value)
				{
					this.parent.RemoveChild(this.Guid);
				}
				this.parent = value;
			}
		}

		public virtual void Accept(ILoadEntityVisitor visitor)
		{
			visitor.Visit(this);
		}

		public virtual void ConvertFromSerializationFormat()
		{
			this.ConsumedLoad.ConvertFromSerializationFormat();
		}

		public virtual long GetAggregateConsumedLoad(LoadMetric metric)
		{
			return this.ConsumedLoad[metric];
		}

		public virtual LoadEntity ToSerializationFormat(bool convertBandToBandData)
		{
			LoadEntity loadEntity = this.CreateCopy();
			loadEntity.ConsumedLoad.ConvertToSerializationMetrics(convertBandToBandData);
			return loadEntity;
		}

		public override string ToString()
		{
			return string.Format("LoadEntity{{Name={0},GUID={1},Consumed={2}}}", this.Name, this.Guid, this.ConsumedLoad);
		}

		private LoadEntity CreateCopy()
		{
			return new LoadEntity(this.DirectoryObject)
			{
				ConsumedLoad = new LoadMetricStorage(this.ConsumedLoad)
			};
		}

		private LoadContainer parent;
	}
}
