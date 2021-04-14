using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage.ActiveMonitoring;

namespace Microsoft.Office.Datacenter.ActiveMonitoring.Management.Common
{
	[Serializable]
	public class ConsolidatedHealth : ConfigurableObject
	{
		public ConsolidatedHealth() : base(new SimpleProviderPropertyBag())
		{
		}

		internal ConsolidatedHealth(string server, string healthSet, string healthGroup) : this(server, healthSet, healthGroup, MonitorAlertState.Unknown, MonitorServerComponentState.Unknown, 0, 0, DateTime.MinValue, new List<MonitorHealthEntry>())
		{
		}

		internal ConsolidatedHealth(string server, string healthSet, string healthGroup, MonitorAlertState alertValue, MonitorServerComponentState state, int monitorCount, int haImpactingMonitorCount, DateTime lastTransitionTime, IEnumerable<MonitorHealthEntry> groupedEntries) : this()
		{
			this.Server = server;
			this.HealthSet = healthSet;
			this.HealthGroup = healthGroup;
			this.AlertValue = alertValue;
			this.State = state;
			this.MonitorCount = monitorCount;
			this.HaImpactingMonitorCount = haImpactingMonitorCount;
			this.LastTransitionTime = lastTransitionTime;
			this.Entries = new MultiValuedProperty<MonitorHealthEntry>(groupedEntries.ToArray<MonitorHealthEntry>());
			this[SimpleProviderObjectSchema.Identity] = new ConsolidatedHealth.ConsolidatedHealthId(healthSet, server);
		}

		internal ConsolidatedHealth(string healthSet, string healthGroup, MonitorAlertState alertValue, MonitorServerComponentState state, int monitorCount, int haImpactingMonitorCount, DateTime lastTransitionTime, IEnumerable<ConsolidatedHealth> consolidatedEntries) : this()
		{
			this.Server = null;
			this.HealthSet = healthSet;
			this.HealthGroup = healthGroup;
			this.AlertValue = alertValue;
			this.State = state;
			this.MonitorCount = monitorCount;
			this.HaImpactingMonitorCount = haImpactingMonitorCount;
			this.LastTransitionTime = lastTransitionTime;
			MultiValuedProperty<MonitorHealthEntry> multiValuedProperty = new MultiValuedProperty<MonitorHealthEntry>();
			foreach (ConsolidatedHealth consolidatedHealth in consolidatedEntries)
			{
				foreach (MonitorHealthEntry item in consolidatedHealth.Entries)
				{
					multiValuedProperty.Add(item);
				}
			}
			this.Entries = multiValuedProperty;
			this[SimpleProviderObjectSchema.Identity] = new ConsolidatedHealth.ConsolidatedHealthId(healthSet, null);
		}

		public string Server
		{
			get
			{
				return (string)this[ConsolidatedHealthSchema.Server];
			}
			private set
			{
				this[ConsolidatedHealthSchema.Server] = value;
			}
		}

		public MonitorServerComponentState State
		{
			get
			{
				return (MonitorServerComponentState)this[ConsolidatedHealthSchema.State];
			}
			private set
			{
				this[ConsolidatedHealthSchema.State] = value;
			}
		}

		public string HealthSet
		{
			get
			{
				return (string)this[ConsolidatedHealthSchema.HealthSet];
			}
			private set
			{
				this[ConsolidatedHealthSchema.HealthSet] = value;
			}
		}

		public string HealthGroup
		{
			get
			{
				return (string)this[ConsolidatedHealthSchema.HealthGroup];
			}
			private set
			{
				this[ConsolidatedHealthSchema.HealthGroup] = value;
			}
		}

		public MonitorAlertState AlertValue
		{
			get
			{
				return (MonitorAlertState)this[ConsolidatedHealthSchema.AlertValue];
			}
			private set
			{
				this[ConsolidatedHealthSchema.AlertValue] = value;
			}
		}

		public DateTime LastTransitionTime
		{
			get
			{
				return (DateTime)this[ConsolidatedHealthSchema.LastTransitionTime];
			}
			private set
			{
				this[ConsolidatedHealthSchema.LastTransitionTime] = value;
			}
		}

		public int MonitorCount
		{
			get
			{
				return (int)this[ConsolidatedHealthSchema.MonitorCount];
			}
			private set
			{
				this[ConsolidatedHealthSchema.MonitorCount] = value;
			}
		}

		public int HaImpactingMonitorCount
		{
			get
			{
				return (int)this[ConsolidatedHealthSchema.HaImpactingMonitorCount];
			}
			private set
			{
				this[ConsolidatedHealthSchema.HaImpactingMonitorCount] = value;
			}
		}

		public MultiValuedProperty<MonitorHealthEntry> Entries
		{
			get
			{
				return this.healthEntries;
			}
			private set
			{
				this.healthEntries = value;
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2010;
			}
		}

		internal override ObjectSchema ObjectSchema
		{
			get
			{
				return ConsolidatedHealth.schema;
			}
		}

		private static ConsolidatedHealthSchema schema = ObjectSchema.GetInstance<ConsolidatedHealthSchema>();

		private MultiValuedProperty<MonitorHealthEntry> healthEntries;

		[Serializable]
		public class ConsolidatedHealthId : ObjectId
		{
			public ConsolidatedHealthId(string healthSetName, string server)
			{
				string arg = (server != null) ? server : string.Empty;
				this.identity = string.Format("{0}\\{1}", healthSetName, arg);
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
