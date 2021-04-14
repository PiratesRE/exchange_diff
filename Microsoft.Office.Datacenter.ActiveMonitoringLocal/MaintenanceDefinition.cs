using System;
using System.Collections.Generic;
using System.Data.Linq.Mapping;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Office.Datacenter.ActiveMonitoring
{
	[Table]
	public sealed class MaintenanceDefinition : WorkDefinition, IPersistence
	{
		public override string SecondaryExternalStorageKey
		{
			get
			{
				return string.Format("{0}_{1}_{2}_{3}", new object[]
				{
					Settings.InstanceName,
					Settings.MachineName,
					base.DeploymentSourceName,
					this.Id
				});
			}
		}

		[Column]
		public int MaxRestartRequestAllowedPerHour { get; set; }

		[Column(IsPrimaryKey = true, IsDbGenerated = true)]
		public override int Id { get; internal set; }

		[Column]
		public override string AssemblyPath { get; set; }

		[Column]
		public override string TypeName { get; set; }

		[Column]
		public override string Name { get; set; }

		[Column]
		public override string WorkItemVersion { get; set; }

		[Column]
		public override string ServiceName { get; set; }

		[Column]
		public override int DeploymentId { get; set; }

		[Column]
		public override string ExecutionLocation { get; set; }

		[Column]
		public override DateTime CreatedTime { get; internal set; }

		[Column]
		public override bool Enabled { get; set; }

		[Column]
		public override string TargetPartition { get; set; }

		[Column]
		public override string TargetGroup { get; set; }

		[Column]
		public override string TargetResource { get; set; }

		[Column]
		public override string TargetExtension { get; set; }

		[Column]
		public override string TargetVersion { get; set; }

		[Column]
		public override int RecurrenceIntervalSeconds { get; set; }

		[Column]
		public override int TimeoutSeconds { get; set; }

		[Column]
		public override DateTime StartTime { get; set; }

		[Column]
		public override DateTime UpdateTime { get; internal set; }

		[Column]
		public override int MaxRetryAttempts { get; set; }

		[Column]
		public override string ExtensionAttributes { get; internal set; }

		[Column]
		public override int CreatedById { get; set; }

		public override string InternalStorageKey
		{
			get
			{
				return base.DeploymentSourceName;
			}
		}

		[Column]
		internal override int Version { get; set; }

		internal int ProbeDefinitionScopeTokenCount
		{
			get
			{
				return this.probeDefinitionScopeTokenCount;
			}
		}

		internal void SetGeneratedDefinitionScopeTokenCount(int numberOfScopeTokens)
		{
			this.probeDefinitionScopeTokenCount = numberOfScopeTokens;
		}

		internal override WorkItemResult CreateResult()
		{
			return new MaintenanceResult(this);
		}

		internal string GetKeyForGeneratedItems()
		{
			return string.Format("{0}_{1}", this.Id, this.TargetVersion);
		}

		internal string GetKeyForGeneratedItems(string state)
		{
			return string.Format("{0}_{1}", this.Id, state);
		}

		internal void SignalAndRemove(Action remover)
		{
			lock (this.definitionLock)
			{
				this.Enabled = false;
				remover();
			}
		}

		internal void SignalAndRegenerate(Action regenerator)
		{
			lock (this.definitionLock)
			{
				if (this.Enabled)
				{
					regenerator();
				}
			}
		}

		public LocalDataAccessMetaData LocalDataAccessMetaData { get; private set; }

		internal static int SchemaVersion
		{
			get
			{
				return MaintenanceDefinition.schemaVersion;
			}
		}

		internal static IEnumerable<WorkDefinitionOverride> GlobalOverrides { get; set; }

		internal static IEnumerable<WorkDefinitionOverride> LocalOverrides { get; set; }

		public void Initialize(Dictionary<string, string> propertyBag, LocalDataAccessMetaData metaData)
		{
			this.LocalDataAccessMetaData = metaData;
			this.SetProperties(propertyBag);
		}

		public void SetProperties(Dictionary<string, string> propertyBag)
		{
			string text;
			if (propertyBag.TryGetValue("MaxRestartRequestAllowedPerHour", out text) && !string.IsNullOrEmpty(text))
			{
				this.MaxRestartRequestAllowedPerHour = int.Parse(text);
			}
			if (propertyBag.TryGetValue("Id", out text) && !string.IsNullOrEmpty(text))
			{
				this.Id = int.Parse(text);
			}
			if (propertyBag.TryGetValue("AssemblyPath", out text))
			{
				this.AssemblyPath = CrimsonHelper.NullDecode(text);
			}
			if (propertyBag.TryGetValue("TypeName", out text))
			{
				this.TypeName = CrimsonHelper.NullDecode(text);
			}
			if (propertyBag.TryGetValue("Name", out text))
			{
				this.Name = CrimsonHelper.NullDecode(text);
			}
			if (propertyBag.TryGetValue("WorkItemVersion", out text))
			{
				this.WorkItemVersion = CrimsonHelper.NullDecode(text);
			}
			if (propertyBag.TryGetValue("ServiceName", out text))
			{
				this.ServiceName = CrimsonHelper.NullDecode(text);
			}
			if (propertyBag.TryGetValue("DeploymentId", out text) && !string.IsNullOrEmpty(text))
			{
				this.DeploymentId = int.Parse(text);
			}
			if (propertyBag.TryGetValue("ExecutionLocation", out text))
			{
				this.ExecutionLocation = CrimsonHelper.NullDecode(text);
			}
			if (propertyBag.TryGetValue("CreatedTime", out text) && !string.IsNullOrEmpty(text))
			{
				this.CreatedTime = DateTime.Parse(text).ToUniversalTime();
			}
			if (propertyBag.TryGetValue("Enabled", out text) && !string.IsNullOrEmpty(text))
			{
				this.Enabled = CrimsonHelper.ParseIntStringAsBool(text);
			}
			if (propertyBag.TryGetValue("TargetPartition", out text))
			{
				this.TargetPartition = CrimsonHelper.NullDecode(text);
			}
			if (propertyBag.TryGetValue("TargetGroup", out text))
			{
				this.TargetGroup = CrimsonHelper.NullDecode(text);
			}
			if (propertyBag.TryGetValue("TargetResource", out text))
			{
				this.TargetResource = CrimsonHelper.NullDecode(text);
			}
			if (propertyBag.TryGetValue("TargetExtension", out text))
			{
				this.TargetExtension = CrimsonHelper.NullDecode(text);
			}
			if (propertyBag.TryGetValue("TargetVersion", out text))
			{
				this.TargetVersion = CrimsonHelper.NullDecode(text);
			}
			if (propertyBag.TryGetValue("RecurrenceIntervalSeconds", out text) && !string.IsNullOrEmpty(text))
			{
				this.RecurrenceIntervalSeconds = int.Parse(text);
			}
			if (propertyBag.TryGetValue("TimeoutSeconds", out text) && !string.IsNullOrEmpty(text))
			{
				this.TimeoutSeconds = int.Parse(text);
			}
			if (propertyBag.TryGetValue("StartTime", out text) && !string.IsNullOrEmpty(text))
			{
				this.StartTime = DateTime.Parse(text).ToUniversalTime();
			}
			if (propertyBag.TryGetValue("UpdateTime", out text) && !string.IsNullOrEmpty(text))
			{
				this.UpdateTime = DateTime.Parse(text).ToUniversalTime();
			}
			if (propertyBag.TryGetValue("MaxRetryAttempts", out text) && !string.IsNullOrEmpty(text))
			{
				this.MaxRetryAttempts = int.Parse(text);
			}
			if (propertyBag.TryGetValue("ExtensionAttributes", out text))
			{
				this.ExtensionAttributes = CrimsonHelper.NullDecode(text);
			}
			if (propertyBag.TryGetValue("CreatedById", out text) && !string.IsNullOrEmpty(text))
			{
				this.CreatedById = int.Parse(text);
			}
			if (propertyBag.TryGetValue("Version", out text) && !string.IsNullOrEmpty(text))
			{
				this.Version = int.Parse(text);
			}
		}

		public void Write(Action<IPersistence> preWriteHandler = null)
		{
			DefinitionIdGenerator<MaintenanceDefinition>.AssignId(this);
			Update<MaintenanceDefinition>.ApplyUpdates(this);
			if (MaintenanceDefinition.GlobalOverrides != null)
			{
				foreach (WorkDefinitionOverride definitionOverride in MaintenanceDefinition.GlobalOverrides)
				{
					definitionOverride.TryApplyTo(this, base.TraceContext);
				}
			}
			if (MaintenanceDefinition.LocalOverrides != null)
			{
				foreach (WorkDefinitionOverride definitionOverride2 in MaintenanceDefinition.LocalOverrides)
				{
					definitionOverride2.TryApplyTo(this, base.TraceContext);
				}
			}
			if (preWriteHandler != null)
			{
				preWriteHandler(this);
			}
			NativeMethods.MaintenanceDefinitionUnmanaged maintenanceDefinitionUnmanaged = this.ToUnmanaged();
			NativeMethods.WriteMaintenanceDefinition(ref maintenanceDefinitionUnmanaged);
		}

		internal NativeMethods.MaintenanceDefinitionUnmanaged ToUnmanaged()
		{
			return new NativeMethods.MaintenanceDefinitionUnmanaged
			{
				MaxRestartRequestAllowedPerHour = this.MaxRestartRequestAllowedPerHour,
				Id = this.Id,
				AssemblyPath = CrimsonHelper.NullCode(this.AssemblyPath),
				TypeName = CrimsonHelper.NullCode(this.TypeName),
				Name = CrimsonHelper.NullCode(this.Name),
				WorkItemVersion = CrimsonHelper.NullCode(this.WorkItemVersion),
				ServiceName = CrimsonHelper.NullCode(this.ServiceName),
				DeploymentId = this.DeploymentId,
				ExecutionLocation = CrimsonHelper.NullCode(this.ExecutionLocation),
				CreatedTime = this.CreatedTime.ToUniversalTime().ToString("o"),
				Enabled = (this.Enabled ? 1 : 0),
				TargetPartition = CrimsonHelper.NullCode(this.TargetPartition),
				TargetGroup = CrimsonHelper.NullCode(this.TargetGroup),
				TargetResource = CrimsonHelper.NullCode(this.TargetResource),
				TargetExtension = CrimsonHelper.NullCode(this.TargetExtension),
				TargetVersion = CrimsonHelper.NullCode(this.TargetVersion),
				RecurrenceIntervalSeconds = this.RecurrenceIntervalSeconds,
				TimeoutSeconds = this.TimeoutSeconds,
				StartTime = this.StartTime.ToUniversalTime().ToString("o"),
				UpdateTime = this.UpdateTime.ToUniversalTime().ToString("o"),
				MaxRetryAttempts = this.MaxRetryAttempts,
				ExtensionAttributes = CrimsonHelper.NullCode(this.ExtensionAttributes),
				CreatedById = this.CreatedById,
				Version = this.Version
			};
		}

		private const string generatedItemKeyFormat = "{0}_{1}";

		private object definitionLock = new object();

		private int probeDefinitionScopeTokenCount = 3;

		private static int schemaVersion = 65536;
	}
}
