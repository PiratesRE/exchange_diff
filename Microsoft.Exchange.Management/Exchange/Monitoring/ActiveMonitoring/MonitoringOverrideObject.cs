using System;
using System.Text;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring
{
	[Serializable]
	public class MonitoringOverrideObject : ConfigurableObject
	{
		public MonitoringOverrideObject() : base(new SimpleProviderPropertyBag())
		{
		}

		internal MonitoringOverrideObject(MonitoringOverride monitoringOverride, string workitemType) : this(monitoringOverride.HealthSet, monitoringOverride.MonitoringItemName, string.Empty, workitemType, monitoringOverride.PropertyName, monitoringOverride.PropertyValue, (monitoringOverride.ExpirationTime != null) ? monitoringOverride.ExpirationTime.Value.ToString() : string.Empty, (monitoringOverride.ApplyVersion == null) ? string.Empty : monitoringOverride.ApplyVersion.ToString(), monitoringOverride.CreatedBy, (monitoringOverride.WhenCreated != null) ? monitoringOverride.WhenCreatedUTC.Value.ToString() : string.Empty)
		{
		}

		internal MonitoringOverrideObject(string healthSetName, string monitoringItemName, string targetResource, string itemType, string propertyName, string propertyValue, string expirationTime, string applyVersion, string createdBy, string createdTime) : this()
		{
			this[SimpleProviderObjectSchema.Identity] = new MonitoringOverrideObject.MonitoringItemIdentity(healthSetName, monitoringItemName, targetResource);
			this.ItemType = itemType;
			this.PropertyName = propertyName;
			this.PropertyValue = propertyValue;
			this.HealthSetName = healthSetName;
			this.MonitoringItemName = monitoringItemName;
			this.TargetResource = targetResource;
			this.ExpirationTime = expirationTime;
			this.ApplyVersion = applyVersion;
			this.CreatedBy = createdBy;
			this.CreatedTime = createdTime;
		}

		public string ItemType
		{
			get
			{
				return (string)this[MonitoringOverrideSchema.ItemType];
			}
			private set
			{
				this[MonitoringOverrideSchema.ItemType] = value;
			}
		}

		public string PropertyName
		{
			get
			{
				return (string)this[MonitoringOverrideSchema.PropertyName];
			}
			private set
			{
				this[MonitoringOverrideSchema.PropertyName] = value;
			}
		}

		public string PropertyValue
		{
			get
			{
				return (string)this[MonitoringOverrideSchema.PropertyValue];
			}
			private set
			{
				this[MonitoringOverrideSchema.PropertyValue] = value;
			}
		}

		public string HealthSetName
		{
			get
			{
				return (string)this[MonitoringOverrideSchema.HealthSetName];
			}
			private set
			{
				this[MonitoringOverrideSchema.HealthSetName] = value;
			}
		}

		public string MonitoringItemName
		{
			get
			{
				return (string)this[MonitoringOverrideSchema.MonitoringItemName];
			}
			private set
			{
				this[MonitoringOverrideSchema.MonitoringItemName] = value;
			}
		}

		public string TargetResource
		{
			get
			{
				return (string)this[MonitoringOverrideSchema.TargetResource];
			}
			private set
			{
				this[MonitoringOverrideSchema.TargetResource] = value;
			}
		}

		public string ExpirationTime
		{
			get
			{
				return (string)this[MonitoringOverrideSchema.ExpirationTime];
			}
			private set
			{
				this[MonitoringOverrideSchema.ExpirationTime] = value;
			}
		}

		public string ApplyVersion
		{
			get
			{
				return (string)this[MonitoringOverrideSchema.ApplyVersion];
			}
			private set
			{
				this[MonitoringOverrideSchema.ApplyVersion] = value;
			}
		}

		public string CreatedBy
		{
			get
			{
				return (string)this[MonitoringOverrideSchema.CreatedBy];
			}
			private set
			{
				this[MonitoringOverrideSchema.CreatedBy] = value;
			}
		}

		public string CreatedTime
		{
			get
			{
				return (string)this[MonitoringOverrideSchema.CreatedTime];
			}
			private set
			{
				this[MonitoringOverrideSchema.CreatedTime] = value;
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2012;
			}
		}

		internal override ObjectSchema ObjectSchema
		{
			get
			{
				return MonitoringOverrideObject.schema;
			}
		}

		private static ObjectSchema schema = ObjectSchema.GetInstance<MonitoringOverrideSchema>();

		[Serializable]
		public class MonitoringItemIdentity : ObjectId
		{
			public MonitoringItemIdentity(string healthSetName, string monitoringItemName, string targetResource)
			{
				if (string.IsNullOrWhiteSpace(targetResource))
				{
					this.identity = string.Format("{0}\\{1}", healthSetName, monitoringItemName);
					return;
				}
				this.identity = string.Format("{0}\\{1}\\{2}", healthSetName, monitoringItemName, targetResource);
			}

			public override string ToString()
			{
				return this.identity;
			}

			public override byte[] GetBytes()
			{
				return Encoding.Unicode.GetBytes(this.ToString());
			}

			private readonly string identity;
		}
	}
}
