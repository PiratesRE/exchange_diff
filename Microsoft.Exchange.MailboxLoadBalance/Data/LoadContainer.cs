using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.MailboxLoadBalance.CapacityData;
using Microsoft.Exchange.MailboxLoadBalance.Constraints;
using Microsoft.Exchange.MailboxLoadBalance.Data.LoadMetrics;
using Microsoft.Exchange.MailboxLoadBalance.Directory;

namespace Microsoft.Exchange.MailboxLoadBalance.Data
{
	[DataContract]
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class LoadContainer : LoadEntity
	{
		public LoadContainer(DirectoryObject directoryObject, ContainerType type = ContainerType.Generic) : base(directoryObject)
		{
			this.ContainerType = type;
			this.DataRetrievedTimestamp = ExDateTime.UtcNow;
			this.children = new List<LoadEntity>();
			this.reusableCapacity = new LoadMetricStorage();
			this.maximumLoad = new LoadMetricStorage();
			this.committedLoad = new LoadMetricStorage();
			this.Constraint = new LoadCapacityConstraint(this);
		}

		public LoadMetricStorage AvailableCapacity
		{
			get
			{
				return this.MaximumLoad - base.ConsumedLoad - this.CommittedLoad + this.ReusableCapacity;
			}
		}

		public bool CanAcceptBalancingLoad
		{
			get
			{
				if (this.canAcceptBalancingLoad != null)
				{
					return this.canAcceptBalancingLoad.Value;
				}
				return this.Children.OfType<LoadContainer>().Any((LoadContainer c) => c.CanAcceptBalancingLoad);
			}
			set
			{
				this.canAcceptBalancingLoad = new bool?(value);
			}
		}

		public bool CanAcceptRegularLoad
		{
			get
			{
				if (this.canAcceptRegularLoad != null)
				{
					return this.canAcceptRegularLoad.Value;
				}
				return this.Children.OfType<LoadContainer>().Any((LoadContainer c) => c.CanAcceptRegularLoad);
			}
			set
			{
				this.canAcceptRegularLoad = new bool?(value);
			}
		}

		public List<LoadEntity> Children
		{
			get
			{
				if (this.children == null)
				{
					List<LoadEntity> value = new List<LoadEntity>();
					Interlocked.CompareExchange<List<LoadEntity>>(ref this.children, value, null);
				}
				return this.children;
			}
		}

		public LoadMetricStorage CommittedLoad
		{
			get
			{
				return this.committedLoad;
			}
			set
			{
				this.committedLoad = value;
			}
		}

		[DataMember]
		public IAllocationConstraint Constraint { get; set; }

		[DataMember]
		public ContainerType ContainerType { get; private set; }

		[DataMember]
		public ExDateTime DataRetrievedTimestamp { get; set; }

		[DataMember(EmitDefaultValue = true, IsRequired = false, Order = 1)]
		public DateTime DataRetrievedTimestampUtc
		{
			get
			{
				return this.DataRetrievedTimestamp.UniversalTime;
			}
			set
			{
				this.DataRetrievedTimestamp = (ExDateTime)value.ToUniversalTime();
			}
		}

		public LoadMetricStorage MaximumLoad
		{
			get
			{
				return this.maximumLoad;
			}
			set
			{
				this.maximumLoad = value;
			}
		}

		[DataMember]
		public int RelativeLoadWeight { get; set; }

		public LoadMetricStorage ReusableCapacity
		{
			get
			{
				return this.reusableCapacity;
			}
			set
			{
				this.reusableCapacity = value;
			}
		}

		public LoadEntity this[Guid objectGuid]
		{
			get
			{
				LoadEntity result;
				lock (this.Children.GetSyncRoot<LoadEntity>())
				{
					result = this.Children.FirstOrDefault((LoadEntity child) => child.Guid == objectGuid);
				}
				return result;
			}
		}

		public override void Accept(ILoadEntityVisitor visitor)
		{
			if (!visitor.Visit(this))
			{
				return;
			}
			lock (this.Children.GetSyncRoot<LoadEntity>())
			{
				foreach (LoadEntity loadEntity in this.Children)
				{
					loadEntity.Accept(visitor);
				}
			}
		}

		public void AddChild(LoadEntity child)
		{
			lock (this.Children.GetSyncRoot<LoadEntity>())
			{
				if (this.Children.All((LoadEntity le) => le.Guid != child.Guid))
				{
					this.Children.Add(child);
					child.Parent = this;
				}
			}
		}

		public override void ConvertFromSerializationFormat()
		{
			base.ConsumedLoad.ConvertFromSerializationFormat();
			this.MaximumLoad.ConvertFromSerializationFormat();
			this.ReusableCapacity.ConvertFromSerializationFormat();
			this.CommittedLoad.ConvertFromSerializationFormat();
			foreach (LoadEntity loadEntity in this.Children)
			{
				loadEntity.ConvertFromSerializationFormat();
			}
		}

		public override long GetAggregateConsumedLoad(LoadMetric metric)
		{
			long num = base.ConsumedLoad[metric];
			if (this.children.Count == 0)
			{
				return num;
			}
			return num + this.Children.Sum((LoadEntity child) => child.GetAggregateConsumedLoad(metric));
		}

		public long GetAggregateMaximumLoad(LoadMetric metric)
		{
			long num = this.MaximumLoad[metric];
			if (this.children.Count == 0)
			{
				return num;
			}
			return num + this.Children.OfType<LoadContainer>().Sum((LoadContainer child) => child.GetAggregateMaximumLoad(metric));
		}

		public LoadContainer GetShallowCopy()
		{
			LoadContainer loadContainer = new LoadContainer(base.DirectoryObject, this.ContainerType);
			loadContainer.MaximumLoad = new LoadMetricStorage(this.MaximumLoad);
			loadContainer.ConsumedLoad = new LoadMetricStorage(base.ConsumedLoad);
			loadContainer.ReusableCapacity = new LoadMetricStorage(this.ReusableCapacity);
			loadContainer.CommittedLoad = new LoadMetricStorage(this.CommittedLoad);
			loadContainer.RelativeLoadWeight = this.RelativeLoadWeight;
			loadContainer.DataRetrievedTimestamp = this.DataRetrievedTimestamp;
			loadContainer.CanAcceptBalancingLoad = this.CanAcceptBalancingLoad;
			loadContainer.CanAcceptRegularLoad = this.CanAcceptRegularLoad;
			loadContainer.Constraint = this.Constraint.CloneForContainer(loadContainer);
			return loadContainer;
		}

		public void RemoveChild(Guid guid)
		{
			lock (this.Children.GetSyncRoot<LoadEntity>())
			{
				this.Children.RemoveAll((LoadEntity child) => child.Guid == guid);
			}
		}

		public HeatMapCapacityData ToCapacityData()
		{
			DirectoryIdentity directoryObjectIdentity = base.DirectoryObjectIdentity;
			long aggregateConsumedLoad = this.GetAggregateConsumedLoad(ConsumerMailboxSize.Instance);
			long value = this.GetAggregateConsumedLoad(PhysicalSize.Instance) - aggregateConsumedLoad;
			long aggregateMaximumLoad = this.GetAggregateMaximumLoad(PhysicalSize.Instance);
			long aggregateConsumedLoad2 = this.GetAggregateConsumedLoad(LogicalSize.Instance);
			long aggregateConsumedLoad3 = this.GetAggregateConsumedLoad(ConsumerMailboxCount.Instance);
			HeatMapCapacityData heatMapCapacityData = new HeatMapCapacityData();
			heatMapCapacityData.TotalCapacity = PhysicalSize.Instance.ToByteQuantifiedSize(aggregateMaximumLoad);
			heatMapCapacityData.ConsumerSize = ConsumerMailboxSize.Instance.ToByteQuantifiedSize(aggregateConsumedLoad);
			heatMapCapacityData.OrganizationSize = PhysicalSize.Instance.ToByteQuantifiedSize(value);
			heatMapCapacityData.LogicalSize = LogicalSize.Instance.ToByteQuantifiedSize(aggregateConsumedLoad2);
			heatMapCapacityData.Identity = directoryObjectIdentity;
			heatMapCapacityData.TotalMailboxCount = aggregateConsumedLoad3;
			heatMapCapacityData.RetrievedTimestamp = this.DataRetrievedTimestampUtc;
			heatMapCapacityData.LoadMetrics = new LoadMetricStorage();
			foreach (LoadMetric metric in base.ConsumedLoad.Metrics)
			{
				heatMapCapacityData.LoadMetrics[metric] = this.GetAggregateConsumedLoad(metric);
			}
			return heatMapCapacityData;
		}

		public override LoadEntity ToSerializationFormat(bool convertBandToBandData)
		{
			LoadContainer shallowCopy = this.GetShallowCopy();
			shallowCopy.ConsumedLoad.ConvertToSerializationMetrics(convertBandToBandData);
			shallowCopy.ReusableCapacity.ConvertToSerializationMetrics(convertBandToBandData);
			shallowCopy.MaximumLoad.ConvertToSerializationMetrics(convertBandToBandData);
			shallowCopy.CommittedLoad.ConvertToSerializationMetrics(convertBandToBandData);
			foreach (LoadEntity loadEntity in this.Children)
			{
				shallowCopy.AddChild(loadEntity.ToSerializationFormat(convertBandToBandData));
			}
			return shallowCopy;
		}

		public override string ToString()
		{
			return string.Format("LoadContainer{{Name={0},GUID={1},Type={2},ConsumedLoad={3}}}", new object[]
			{
				base.Name,
				base.Guid,
				this.ContainerType,
				base.ConsumedLoad
			});
		}

		[DataMember]
		private bool? canAcceptBalancingLoad;

		[DataMember]
		private bool? canAcceptRegularLoad;

		[DataMember]
		private List<LoadEntity> children;

		[DataMember]
		private LoadMetricStorage committedLoad;

		[DataMember]
		private LoadMetricStorage maximumLoad;

		[DataMember]
		private LoadMetricStorage reusableCapacity;
	}
}
