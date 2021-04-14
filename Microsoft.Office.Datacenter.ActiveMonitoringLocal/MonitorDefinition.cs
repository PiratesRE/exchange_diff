using System;
using System.Collections.Generic;
using System.Data.Linq.Mapping;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Office.Datacenter.ActiveMonitoring
{
	[Table]
	public sealed class MonitorDefinition : WorkDefinition, IPersistence
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
		[PropertyInformation("The filter (by default, a prefix filter over ProbeResult.SampleName) used to find the included samples.", false)]
		public string SampleMask { get; set; }

		[Column]
		[PropertyInformation("The time window used in calculating the monitoring state.", false)]
		public int MonitoringIntervalSeconds { get; set; }

		[PropertyInformation("The minimum number of errors to look for before firing the monitor.", false)]
		[Column]
		public int MinimumErrorCount { get; set; }

		[PropertyInformation("The threshold used in calculating the monitoring state.", false)]
		[Column]
		public double MonitoringThreshold { get; set; }

		[PropertyInformation("The secondary threshold used in calculating the monitoring state.", false)]
		[Column]
		public double SecondaryMonitoringThreshold { get; set; }

		[PropertyInformation("The percentage of monitoring samples that need to meet MonitoringThreshold.", false)]
		[Column]
		public double MonitoringSamplesThreshold { get; set; }

		[Column]
		public int ServicePriority { get; internal set; }

		[Column]
		public ServiceSeverity ServiceSeverity { get; internal set; }

		[Column]
		public bool IsHaImpacting { get; internal set; }

		[Column]
		public override int CreatedById { get; set; }

		[Column]
		public int InsufficientSamplesIntervalSeconds { get; set; }

		public Component Component { get; set; }

		[Column]
		public string StateAttribute1Mask { get; set; }

		[Column]
		public int FailureCategoryMask { get; set; }

		[Column]
		public string ComponentName
		{
			get
			{
				if (!(this.Component != null))
				{
					return string.Empty;
				}
				return this.Component.ToString();
			}
			set
			{
				this.Component = new Component(value);
			}
		}

		[Column]
		public string StateTransitionsXml
		{
			get
			{
				return this.stateTransitionsXml;
			}
			set
			{
				this.stateTransitionsXml = value;
				this.monitorStateTransitions = this.ConvertXmlToStateTransitions(this.stateTransitionsXml);
			}
		}

		[Column]
		public bool AllowCorrelationToMonitor { get; internal set; }

		[Column]
		public string ScenarioDescription { get; set; }

		[Column]
		public string SourceScope { get; set; }

		[Column]
		public string TargetScopes { get; set; }

		[Column]
		public string HaScope
		{
			get
			{
				return this.haScopeString;
			}
			internal set
			{
				this.haScopeEnum = MonitorDefinition.HaScopeToEnum(value);
				this.haScopeString = this.haScopeEnum.ToString();
			}
		}

		public static HaScopeEnum HaScopeToEnum(string scope)
		{
			HaScopeEnum result = HaScopeEnum.Server;
			Enum.TryParse<HaScopeEnum>(scope, true, out result);
			return result;
		}

		public void SetHaScope(HaScopeEnum eScope)
		{
			this.haScopeEnum = eScope;
			this.haScopeString = eScope.ToString();
		}

		public HaScopeEnum GetHaScope()
		{
			return this.haScopeEnum;
		}

		[Column]
		internal override int Version { get; set; }

		public MonitorStateTransition[] MonitorStateTransitions
		{
			get
			{
				return this.monitorStateTransitions;
			}
			set
			{
				this.monitorStateTransitions = value;
				this.stateTransitionsXml = this.ConvertStateTransitionsToXml(this.monitorStateTransitions);
			}
		}

		public MonitorDefinition()
		{
			this.ServicePriority = 2;
			this.MonitoringSamplesThreshold = 100.0;
			this.SetHaScope(HaScopeEnum.Server);
		}

		public override void FromXml(XmlNode definition)
		{
			base.FromXml(definition);
			this.SampleMask = base.GetMandatoryXmlAttribute<string>(definition, "SampleMask");
			this.MonitoringIntervalSeconds = base.GetMandatoryXmlAttribute<int>(definition, "MonitoringIntervalSeconds");
			this.ComponentName = base.GetMandatoryXmlAttribute<string>(definition, "ComponentName");
			this.MonitoringThreshold = base.GetOptionalXmlAttribute<double>(definition, "MonitoringThreshold", 0.0);
			this.SecondaryMonitoringThreshold = base.GetOptionalXmlAttribute<double>(definition, "SecondaryMonitoringThreshold", 0.0);
			this.ServicePriority = base.GetOptionalXmlAttribute<int>(definition, "ServicePriority", 1);
			this.ServiceSeverity = base.GetOptionalXmlEnumAttribute<ServiceSeverity>(definition, "ServiceSeverity", ServiceSeverity.Critical);
			this.MonitorStateTransitions = this.GetStateTransitions(definition);
		}

		internal string ConvertStateTransitionsToXml(MonitorStateTransition[] transitions)
		{
			if (transitions != null && transitions.Length > 0)
			{
				XElement xelement = new XElement("StateTransitions");
				foreach (MonitorStateTransition monitorStateTransition in transitions)
				{
					xelement.Add(new XElement("Transition", new object[]
					{
						new XAttribute("ToState", monitorStateTransition.ToState),
						new XAttribute("TimeoutInSeconds", (int)monitorStateTransition.TransitionTimeout.TotalSeconds)
					}));
				}
				return xelement.ToString();
			}
			return null;
		}

		internal MonitorStateTransition[] ConvertXmlToStateTransitions(string stateTransitionXml)
		{
			if (!string.IsNullOrEmpty(stateTransitionXml))
			{
				XmlDocument xmlDocument = new XmlDocument();
				xmlDocument.LoadXml(stateTransitionXml);
				return this.GetStateTransitions(xmlDocument);
			}
			return new MonitorStateTransition[0];
		}

		internal MonitorStateTransition[] GetStateTransitions(XmlNode definition)
		{
			List<MonitorStateTransition> list = new List<MonitorStateTransition>(4);
			XmlNode xmlNode = definition.SelectSingleNode("StateTransitions");
			if (xmlNode != null)
			{
				using (XmlNodeList childNodes = xmlNode.ChildNodes)
				{
					if (childNodes != null)
					{
						foreach (object obj in childNodes)
						{
							XmlNode definition2 = (XmlNode)obj;
							ServiceHealthStatus mandatoryXmlEnumAttribute = base.GetMandatoryXmlEnumAttribute<ServiceHealthStatus>(definition2, "ToState");
							int mandatoryXmlAttribute = base.GetMandatoryXmlAttribute<int>(definition2, "TimeoutInSeconds");
							MonitorStateTransition monitorStateTransition = new MonitorStateTransition(mandatoryXmlEnumAttribute, mandatoryXmlAttribute);
							WTFDiagnostics.TraceDebug<ServiceHealthStatus, TimeSpan>(WTFLog.ManagedAvailability, base.TraceContext, "[Transition] {0} Timeout:{1}", monitorStateTransition.ToState, monitorStateTransition.TransitionTimeout, null, "GetStateTransitions", "f:\\15.00.1497\\sources\\dev\\common\\src\\WorkerTaskFramework\\ActiveMonitoring\\MonitorWorkDefinition.cs", 528);
							list.Add(monitorStateTransition);
						}
					}
				}
			}
			return list.ToArray();
		}

		internal override WorkItemResult CreateResult()
		{
			return new MonitorResult(this);
		}

		internal override bool Validate(List<string> errors)
		{
			int count = errors.Count;
			base.Validate(errors);
			if (string.IsNullOrWhiteSpace(this.SampleMask))
			{
				errors.Add("SampleMask cannot be null or empty. ");
			}
			if (this.MonitoringIntervalSeconds <= 0)
			{
				errors.Add("MonitoringIntervalSeconds cannot be less than or equal to 0. ");
			}
			if (this.Component == null)
			{
				errors.Add("Component cannot be null. ");
			}
			return count == errors.Count;
		}

		public LocalDataAccessMetaData LocalDataAccessMetaData { get; private set; }

		internal static int SchemaVersion
		{
			get
			{
				return MonitorDefinition.schemaVersion;
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
			if (propertyBag.TryGetValue("SampleMask", out text))
			{
				this.SampleMask = CrimsonHelper.NullDecode(text);
			}
			if (propertyBag.TryGetValue("MonitoringIntervalSeconds", out text) && !string.IsNullOrEmpty(text))
			{
				this.MonitoringIntervalSeconds = int.Parse(text);
			}
			if (propertyBag.TryGetValue("MinimumErrorCount", out text) && !string.IsNullOrEmpty(text))
			{
				this.MinimumErrorCount = int.Parse(text);
			}
			if (propertyBag.TryGetValue("MonitoringThreshold", out text) && !string.IsNullOrEmpty(text))
			{
				this.MonitoringThreshold = CrimsonHelper.ParseDouble(text);
			}
			if (propertyBag.TryGetValue("SecondaryMonitoringThreshold", out text) && !string.IsNullOrEmpty(text))
			{
				this.SecondaryMonitoringThreshold = CrimsonHelper.ParseDouble(text);
			}
			if (propertyBag.TryGetValue("MonitoringSamplesThreshold", out text) && !string.IsNullOrEmpty(text))
			{
				this.MonitoringSamplesThreshold = CrimsonHelper.ParseDouble(text);
			}
			if (propertyBag.TryGetValue("ServicePriority", out text) && !string.IsNullOrEmpty(text))
			{
				this.ServicePriority = int.Parse(text);
			}
			if (propertyBag.TryGetValue("ServiceSeverity", out text) && !string.IsNullOrEmpty(text))
			{
				this.ServiceSeverity = (ServiceSeverity)Enum.Parse(typeof(ServiceSeverity), text);
			}
			if (propertyBag.TryGetValue("IsHaImpacting", out text) && !string.IsNullOrEmpty(text))
			{
				this.IsHaImpacting = CrimsonHelper.ParseIntStringAsBool(text);
			}
			if (propertyBag.TryGetValue("CreatedById", out text) && !string.IsNullOrEmpty(text))
			{
				this.CreatedById = int.Parse(text);
			}
			if (propertyBag.TryGetValue("InsufficientSamplesIntervalSeconds", out text) && !string.IsNullOrEmpty(text))
			{
				this.InsufficientSamplesIntervalSeconds = int.Parse(text);
			}
			if (propertyBag.TryGetValue("StateAttribute1Mask", out text))
			{
				this.StateAttribute1Mask = CrimsonHelper.NullDecode(text);
			}
			if (propertyBag.TryGetValue("FailureCategoryMask", out text) && !string.IsNullOrEmpty(text))
			{
				this.FailureCategoryMask = int.Parse(text);
			}
			if (propertyBag.TryGetValue("ComponentName", out text))
			{
				this.ComponentName = CrimsonHelper.NullDecode(text);
			}
			if (propertyBag.TryGetValue("StateTransitionsXml", out text))
			{
				this.StateTransitionsXml = CrimsonHelper.NullDecode(text);
			}
			if (propertyBag.TryGetValue("AllowCorrelationToMonitor", out text) && !string.IsNullOrEmpty(text))
			{
				this.AllowCorrelationToMonitor = CrimsonHelper.ParseIntStringAsBool(text);
			}
			if (propertyBag.TryGetValue("ScenarioDescription", out text))
			{
				this.ScenarioDescription = CrimsonHelper.NullDecode(text);
			}
			if (propertyBag.TryGetValue("SourceScope", out text))
			{
				this.SourceScope = CrimsonHelper.NullDecode(text);
			}
			if (propertyBag.TryGetValue("TargetScopes", out text))
			{
				this.TargetScopes = CrimsonHelper.NullDecode(text);
			}
			if (propertyBag.TryGetValue("HaScope", out text))
			{
				this.HaScope = CrimsonHelper.NullDecode(text);
			}
			if (propertyBag.TryGetValue("Version", out text) && !string.IsNullOrEmpty(text))
			{
				this.Version = int.Parse(text);
			}
		}

		public void Write(Action<IPersistence> preWriteHandler = null)
		{
			DefinitionIdGenerator<MonitorDefinition>.AssignId(this);
			Update<MonitorDefinition>.ApplyUpdates(this);
			if (MonitorDefinition.GlobalOverrides != null)
			{
				foreach (WorkDefinitionOverride definitionOverride in MonitorDefinition.GlobalOverrides)
				{
					definitionOverride.TryApplyTo(this, base.TraceContext);
				}
			}
			if (MonitorDefinition.LocalOverrides != null)
			{
				foreach (WorkDefinitionOverride definitionOverride2 in MonitorDefinition.LocalOverrides)
				{
					definitionOverride2.TryApplyTo(this, base.TraceContext);
				}
			}
			if (preWriteHandler != null)
			{
				preWriteHandler(this);
			}
			NativeMethods.MonitorDefinitionUnmanaged monitorDefinitionUnmanaged = this.ToUnmanaged();
			NativeMethods.WriteMonitorDefinition(ref monitorDefinitionUnmanaged);
		}

		internal NativeMethods.MonitorDefinitionUnmanaged ToUnmanaged()
		{
			return new NativeMethods.MonitorDefinitionUnmanaged
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
				SampleMask = CrimsonHelper.NullCode(this.SampleMask),
				MonitoringIntervalSeconds = this.MonitoringIntervalSeconds,
				MinimumErrorCount = this.MinimumErrorCount,
				MonitoringThreshold = this.MonitoringThreshold,
				SecondaryMonitoringThreshold = this.SecondaryMonitoringThreshold,
				MonitoringSamplesThreshold = this.MonitoringSamplesThreshold,
				ServicePriority = this.ServicePriority,
				ServiceSeverity = this.ServiceSeverity,
				IsHaImpacting = (this.IsHaImpacting ? 1 : 0),
				CreatedById = this.CreatedById,
				InsufficientSamplesIntervalSeconds = this.InsufficientSamplesIntervalSeconds,
				StateAttribute1Mask = CrimsonHelper.NullCode(this.StateAttribute1Mask),
				FailureCategoryMask = this.FailureCategoryMask,
				ComponentName = CrimsonHelper.NullCode(this.ComponentName),
				StateTransitionsXml = CrimsonHelper.NullCode(this.StateTransitionsXml),
				AllowCorrelationToMonitor = (this.AllowCorrelationToMonitor ? 1 : 0),
				ScenarioDescription = CrimsonHelper.NullCode(this.ScenarioDescription),
				SourceScope = CrimsonHelper.NullCode(this.SourceScope),
				TargetScopes = CrimsonHelper.NullCode(this.TargetScopes),
				HaScope = CrimsonHelper.NullCode(this.HaScope),
				Version = this.Version
			};
		}

		private string haScopeString;

		private HaScopeEnum haScopeEnum;

		private MonitorStateTransition[] monitorStateTransitions;

		private string stateTransitionsXml;

		private static int schemaVersion = 65536;
	}
}
