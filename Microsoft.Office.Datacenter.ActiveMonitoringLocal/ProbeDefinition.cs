using System;
using System.Collections.Generic;
using System.Data.Linq.Mapping;
using System.Xml;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Office.Datacenter.ActiveMonitoring
{
	[Table]
	public sealed class ProbeDefinition : WorkDefinition, IPersistence
	{
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

		[Column]
		[PropertyInformation("The email address identifying the primary account.", false)]
		public string Account { get; set; }

		[Column]
		[PropertyInformation("The password of the primary account.", false)]
		public string AccountPassword { get; set; }

		[Column]
		[PropertyInformation("The display name of the primary account.", false)]
		public string AccountDisplayName { get; set; }

		[Column]
		[PropertyInformation("The primary protocol URL used by the probe.", false)]
		public string Endpoint { get; set; }

		[Column]
		[PropertyInformation("The email address identifying the secondary account.", false)]
		public string SecondaryAccount { get; set; }

		[PropertyInformation("The password of the secondary account.", false)]
		[Column]
		public string SecondaryAccountPassword { get; set; }

		[Column]
		[PropertyInformation("The display name of the secondary account.", false)]
		public string SecondaryAccountDisplayName { get; set; }

		[Column]
		[PropertyInformation("The secondary protocol URL used by the probe.", false)]
		public string SecondaryEndpoint { get; set; }

		[PropertyInformation("The extension endpoints or vips to be used by the probe.", false)]
		[Column]
		public string ExtensionEndpoints { get; set; }

		[Column]
		internal override int Version { get; set; }

		[Column]
		internal int ExecutionType { get; set; }

		public override void FromXml(XmlNode definition)
		{
			base.FromXml(definition);
			this.Account = base.GetOptionalXmlAttribute<string>(definition, "Account", string.Empty);
			this.AccountPassword = base.GetOptionalXmlAttribute<string>(definition, "AccountPassword", string.Empty);
			this.AccountDisplayName = base.GetOptionalXmlAttribute<string>(definition, "AccountDisplayName", string.Empty);
			this.Endpoint = base.GetOptionalXmlAttribute<string>(definition, "Endpoint", string.Empty);
			this.SecondaryAccount = base.GetOptionalXmlAttribute<string>(definition, "SecondaryAccount", string.Empty);
			this.SecondaryAccountPassword = base.GetOptionalXmlAttribute<string>(definition, "SecondaryAccountPassword", string.Empty);
			this.SecondaryAccountDisplayName = base.GetOptionalXmlAttribute<string>(definition, "SecondaryAccountDisplayName", string.Empty);
			this.SecondaryEndpoint = base.GetOptionalXmlAttribute<string>(definition, "SecondaryEndpoint", string.Empty);
			this.ExecutionType = base.GetOptionalXmlAttribute<int>(definition, "ExecutionType", 0);
		}

		internal override WorkItemResult CreateResult()
		{
			return new ProbeResult(this);
		}

		public LocalDataAccessMetaData LocalDataAccessMetaData { get; private set; }

		internal static int SchemaVersion
		{
			get
			{
				return ProbeDefinition.schemaVersion;
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
			if (propertyBag.TryGetValue("Account", out text))
			{
				this.Account = CrimsonHelper.NullDecode(text);
			}
			if (propertyBag.TryGetValue("AccountDisplayName", out text))
			{
				this.AccountDisplayName = CrimsonHelper.NullDecode(text);
			}
			if (propertyBag.TryGetValue("Endpoint", out text))
			{
				this.Endpoint = CrimsonHelper.NullDecode(text);
			}
			if (propertyBag.TryGetValue("SecondaryAccount", out text))
			{
				this.SecondaryAccount = CrimsonHelper.NullDecode(text);
			}
			if (propertyBag.TryGetValue("SecondaryAccountDisplayName", out text))
			{
				this.SecondaryAccountDisplayName = CrimsonHelper.NullDecode(text);
			}
			if (propertyBag.TryGetValue("SecondaryEndpoint", out text))
			{
				this.SecondaryEndpoint = CrimsonHelper.NullDecode(text);
			}
			if (propertyBag.TryGetValue("ExtensionEndpoints", out text))
			{
				this.ExtensionEndpoints = CrimsonHelper.NullDecode(text);
			}
			if (propertyBag.TryGetValue("Version", out text) && !string.IsNullOrEmpty(text))
			{
				this.Version = int.Parse(text);
			}
			if (propertyBag.TryGetValue("ExecutionType", out text) && !string.IsNullOrEmpty(text))
			{
				this.ExecutionType = int.Parse(text);
			}
		}

		public void Write(Action<IPersistence> preWriteHandler = null)
		{
			DefinitionIdGenerator<ProbeDefinition>.AssignId(this);
			Update<ProbeDefinition>.ApplyUpdates(this);
			if (ProbeDefinition.GlobalOverrides != null)
			{
				foreach (WorkDefinitionOverride definitionOverride in ProbeDefinition.GlobalOverrides)
				{
					definitionOverride.TryApplyTo(this, base.TraceContext);
				}
			}
			if (ProbeDefinition.LocalOverrides != null)
			{
				foreach (WorkDefinitionOverride definitionOverride2 in ProbeDefinition.LocalOverrides)
				{
					definitionOverride2.TryApplyTo(this, base.TraceContext);
				}
			}
			if (preWriteHandler != null)
			{
				preWriteHandler(this);
			}
			NativeMethods.ProbeDefinitionUnmanaged probeDefinitionUnmanaged = this.ToUnmanaged();
			NativeMethods.WriteProbeDefinition(ref probeDefinitionUnmanaged);
		}

		internal NativeMethods.ProbeDefinitionUnmanaged ToUnmanaged()
		{
			return new NativeMethods.ProbeDefinitionUnmanaged
			{
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
				Account = CrimsonHelper.NullCode(this.Account),
				AccountDisplayName = CrimsonHelper.NullCode(this.AccountDisplayName),
				Endpoint = CrimsonHelper.NullCode(this.Endpoint),
				SecondaryAccount = CrimsonHelper.NullCode(this.SecondaryAccount),
				SecondaryAccountDisplayName = CrimsonHelper.NullCode(this.SecondaryAccountDisplayName),
				SecondaryEndpoint = CrimsonHelper.NullCode(this.SecondaryEndpoint),
				ExtensionEndpoints = CrimsonHelper.NullCode(this.ExtensionEndpoints),
				Version = this.Version,
				ExecutionType = this.ExecutionType
			};
		}

		private static int schemaVersion = 65536;
	}
}
