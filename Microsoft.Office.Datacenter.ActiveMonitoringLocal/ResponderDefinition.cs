using System;
using System.Collections.Generic;
using System.Data.Linq.Mapping;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Office.Datacenter.ActiveMonitoring
{
	[Table]
	public sealed class ResponderDefinition : WorkDefinition, IPersistence
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

		[PropertyInformation("The filter (by default a prefix filter over MonitorResult.AlertName) identifying the alerts to which this responder reacts.", false)]
		[Column]
		public string AlertMask { get; set; }

		[Column]
		[PropertyInformation("The amount of time to wait after attempting another response.", false)]
		public int WaitIntervalSeconds { get; set; }

		[Column]
		[PropertyInformation("The amount of time between two escalates.", false)]
		public int MinimumSecondsBetweenEscalates { get; set; }

		[Column]
		[PropertyInformation("The subject text to be included in the escalation mail.", false)]
		public string EscalationSubject { get; set; }

		[Column]
		[PropertyInformation("The message body to be included in the escalation mail.", false)]
		public string EscalationMessage { get; set; }

		[PropertyInformation("The name of the service that the team belongs to.", false)]
		[Column]
		public string EscalationService { get; set; }

		[Column]
		[PropertyInformation("The name of the team to escalate to.", false)]
		public string EscalationTeam { get; set; }

		[Column]
		public NotificationServiceClass NotificationServiceClass { get; set; }

		[Column]
		public string DailySchedulePattern { get; set; }

		[Column]
		public bool AlwaysEscalateOnMonitorChanges { get; set; }

		[PropertyInformation("The CentralAdmin powershell endpoint to use.", false)]
		[Column]
		public string Endpoint { get; set; }

		[Column]
		public override int CreatedById { get; set; }

		[Column]
		[PropertyInformation("The certificate subject or account to use when connection to CentralAdmin.", false)]
		public string Account { get; set; }

		[PropertyInformation("The password to use when connecting to CentralAdmin. This should be set only if the auth method is not cert-based.", false)]
		[Column]
		public string AccountPassword { get; set; }

		[PropertyInformation("The unique alert id associated with this type of alert. This should be a human readable string in the format [Team]/[Component]/[AlertType].", false)]
		[Column]
		public string AlertTypeId { get; set; }

		[Column]
		public ServiceHealthStatus TargetHealthState { get; internal set; }

		[Column]
		public string CorrelatedMonitorsXml
		{
			get
			{
				return this.correlatedMonitorsXml;
			}
			set
			{
				this.correlatedMonitorsXml = value;
				this.correlatedMonitors = this.ConvertXmlToCorrelatedMonitors(this.correlatedMonitorsXml);
			}
		}

		[Column]
		public CorrelatedMonitorAction ActionOnCorrelatedMonitors { get; internal set; }

		[Column]
		public string ResponderCategory { get; set; }

		[Column]
		public string ThrottleGroupName { get; set; }

		[Column]
		public string ThrottlePolicyXml { get; set; }

		[Column]
		public bool UploadScopeNotification { get; set; }

		[Column]
		public bool SuppressEscalation { get; set; }

		[Column]
		internal override int Version { get; set; }

		internal CorrelatedMonitorInfo[] CorrelatedMonitors
		{
			get
			{
				return this.correlatedMonitors;
			}
			set
			{
				if (value != null && value.Length > 10)
				{
					throw new InvalidOperationException("Maximum number of CorrelationMonitors can not exceeed 10");
				}
				this.correlatedMonitors = value;
				this.correlatedMonitorsXml = this.ConvertCorrelatedMonitorsToXml(this.correlatedMonitors);
			}
		}

		public override void FromXml(XmlNode definition)
		{
			base.FromXml(definition);
			this.AlertMask = base.GetMandatoryXmlAttribute<string>(definition, "AlertMask");
			this.WaitIntervalSeconds = base.GetMandatoryXmlAttribute<int>(definition, "WaitIntervalSeconds");
			this.AlertTypeId = base.GetMandatoryXmlAttribute<string>(definition, "AlertTypeId");
			this.Account = base.GetOptionalXmlAttribute<string>(definition, "Account", string.Empty);
			this.AccountPassword = base.GetOptionalXmlAttribute<string>(definition, "AccountPassword", string.Empty);
			this.EscalationSubject = base.GetOptionalXmlAttribute<string>(definition, "EscalationSubject", string.Empty);
			this.EscalationMessage = base.GetOptionalXmlAttribute<string>(definition, "EscalationMessage", string.Empty);
			this.EscalationService = base.GetOptionalXmlAttribute<string>(definition, "EscalationService", string.Empty);
			this.EscalationTeam = base.GetOptionalXmlAttribute<string>(definition, "EscalationTeam", string.Empty);
			this.Endpoint = base.GetOptionalXmlAttribute<string>(definition, "Endpoint", string.Empty);
			this.TargetHealthState = base.GetOptionalXmlEnumAttribute<ServiceHealthStatus>(definition, "TargetHealthState", ServiceHealthStatus.None);
		}

		internal string ConvertCorrelatedMonitorsToXml(CorrelatedMonitorInfo[] monitors)
		{
			if (monitors != null && monitors.Length > 0)
			{
				XElement xelement = new XElement("CorrelatedMonitors");
				foreach (CorrelatedMonitorInfo correlatedMonitorInfo in monitors)
				{
					xelement.Add(new XElement("Monitor", new object[]
					{
						new XAttribute("Identity", correlatedMonitorInfo.Identity),
						new XAttribute("ExceptionMatchString", correlatedMonitorInfo.MatchString ?? string.Empty),
						new XAttribute("ModeOfMatch", correlatedMonitorInfo.ModeOfMatch.ToString())
					}));
				}
				return xelement.ToString();
			}
			return string.Empty;
		}

		internal CorrelatedMonitorInfo[] ConvertXmlToCorrelatedMonitors(string correlatedMonitorsXml)
		{
			if (!string.IsNullOrEmpty(correlatedMonitorsXml))
			{
				XmlDocument xmlDocument = new XmlDocument();
				xmlDocument.LoadXml(correlatedMonitorsXml);
				return this.GetCorrelatedMonitors(xmlDocument);
			}
			return new CorrelatedMonitorInfo[0];
		}

		internal CorrelatedMonitorInfo[] GetCorrelatedMonitors(XmlNode definition)
		{
			List<CorrelatedMonitorInfo> list = new List<CorrelatedMonitorInfo>(10);
			XmlNode xmlNode = definition.SelectSingleNode("CorrelatedMonitors");
			if (xmlNode != null)
			{
				using (XmlNodeList childNodes = xmlNode.ChildNodes)
				{
					if (childNodes != null)
					{
						foreach (object obj in childNodes)
						{
							XmlNode definition2 = (XmlNode)obj;
							string mandatoryXmlAttribute = base.GetMandatoryXmlAttribute<string>(definition2, "Identity");
							string optionalXmlAttribute = base.GetOptionalXmlAttribute<string>(definition2, "ExceptionMatchString", string.Empty);
							CorrelatedMonitorInfo.MatchMode optionalXmlEnumAttribute = base.GetOptionalXmlEnumAttribute<CorrelatedMonitorInfo.MatchMode>(definition2, "ModeOfMatch", CorrelatedMonitorInfo.MatchMode.Wildcard);
							CorrelatedMonitorInfo correlatedMonitorInfo = new CorrelatedMonitorInfo(mandatoryXmlAttribute, optionalXmlAttribute, optionalXmlEnumAttribute);
							WTFDiagnostics.TraceDebug<string, string, CorrelatedMonitorInfo.MatchMode>(WTFLog.ManagedAvailability, base.TraceContext, "[CorrelatedMonitor] {0} ExceptionMatch:{1} ModeOfMatch:{2}", correlatedMonitorInfo.Identity, correlatedMonitorInfo.MatchString, correlatedMonitorInfo.ModeOfMatch, null, "GetCorrelatedMonitors", "f:\\15.00.1497\\sources\\dev\\common\\src\\WorkerTaskFramework\\ActiveMonitoring\\ResponderWorkDefinition.cs", 543);
							list.Add(correlatedMonitorInfo);
						}
					}
				}
			}
			return list.ToArray();
		}

		internal override WorkItemResult CreateResult()
		{
			return new ResponderResult(this);
		}

		internal override bool Validate(List<string> errors)
		{
			int count = errors.Count;
			base.Validate(errors);
			if (string.IsNullOrWhiteSpace(this.AlertMask))
			{
				errors.Add("AlertMask cannot be null or empty. ");
			}
			if (string.IsNullOrWhiteSpace(this.AlertTypeId))
			{
				errors.Add("AlertTypeId cannot be null or empty. ");
			}
			if (this.WaitIntervalSeconds < -1)
			{
				errors.Add("WaitIntervalSeconds cannot be less than -1. ");
			}
			return count == errors.Count;
		}

		public LocalDataAccessMetaData LocalDataAccessMetaData { get; private set; }

		internal static int SchemaVersion
		{
			get
			{
				return ResponderDefinition.schemaVersion;
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
			if (propertyBag.TryGetValue("AlertMask", out text))
			{
				this.AlertMask = CrimsonHelper.NullDecode(text);
			}
			if (propertyBag.TryGetValue("WaitIntervalSeconds", out text) && !string.IsNullOrEmpty(text))
			{
				this.WaitIntervalSeconds = int.Parse(text);
			}
			if (propertyBag.TryGetValue("MinimumSecondsBetweenEscalates", out text) && !string.IsNullOrEmpty(text))
			{
				this.MinimumSecondsBetweenEscalates = int.Parse(text);
			}
			if (propertyBag.TryGetValue("EscalationSubject", out text))
			{
				this.EscalationSubject = CrimsonHelper.NullDecode(text);
			}
			if (propertyBag.TryGetValue("EscalationMessage", out text))
			{
				this.EscalationMessage = CrimsonHelper.NullDecode(text);
			}
			if (propertyBag.TryGetValue("EscalationService", out text))
			{
				this.EscalationService = CrimsonHelper.NullDecode(text);
			}
			if (propertyBag.TryGetValue("EscalationTeam", out text))
			{
				this.EscalationTeam = CrimsonHelper.NullDecode(text);
			}
			if (propertyBag.TryGetValue("NotificationServiceClass", out text) && !string.IsNullOrEmpty(text))
			{
				this.NotificationServiceClass = (NotificationServiceClass)Enum.Parse(typeof(NotificationServiceClass), text);
			}
			if (propertyBag.TryGetValue("DailySchedulePattern", out text))
			{
				this.DailySchedulePattern = CrimsonHelper.NullDecode(text);
			}
			if (propertyBag.TryGetValue("AlwaysEscalateOnMonitorChanges", out text) && !string.IsNullOrEmpty(text))
			{
				this.AlwaysEscalateOnMonitorChanges = CrimsonHelper.ParseIntStringAsBool(text);
			}
			if (propertyBag.TryGetValue("Endpoint", out text))
			{
				this.Endpoint = CrimsonHelper.NullDecode(text);
			}
			if (propertyBag.TryGetValue("CreatedById", out text) && !string.IsNullOrEmpty(text))
			{
				this.CreatedById = int.Parse(text);
			}
			if (propertyBag.TryGetValue("Account", out text))
			{
				this.Account = CrimsonHelper.NullDecode(text);
			}
			if (propertyBag.TryGetValue("AlertTypeId", out text))
			{
				this.AlertTypeId = CrimsonHelper.NullDecode(text);
			}
			if (propertyBag.TryGetValue("TargetHealthState", out text) && !string.IsNullOrEmpty(text))
			{
				this.TargetHealthState = (ServiceHealthStatus)Enum.Parse(typeof(ServiceHealthStatus), text);
			}
			if (propertyBag.TryGetValue("CorrelatedMonitorsXml", out text))
			{
				this.CorrelatedMonitorsXml = CrimsonHelper.NullDecode(text);
			}
			if (propertyBag.TryGetValue("ActionOnCorrelatedMonitors", out text) && !string.IsNullOrEmpty(text))
			{
				this.ActionOnCorrelatedMonitors = (CorrelatedMonitorAction)Enum.Parse(typeof(CorrelatedMonitorAction), text);
			}
			if (propertyBag.TryGetValue("ResponderCategory", out text))
			{
				this.ResponderCategory = CrimsonHelper.NullDecode(text);
			}
			if (propertyBag.TryGetValue("ThrottleGroupName", out text))
			{
				this.ThrottleGroupName = CrimsonHelper.NullDecode(text);
			}
			if (propertyBag.TryGetValue("ThrottlePolicyXml", out text))
			{
				this.ThrottlePolicyXml = CrimsonHelper.NullDecode(text);
			}
			if (propertyBag.TryGetValue("UploadScopeNotification", out text) && !string.IsNullOrEmpty(text))
			{
				this.UploadScopeNotification = CrimsonHelper.ParseIntStringAsBool(text);
			}
			if (propertyBag.TryGetValue("SuppressEscalation", out text) && !string.IsNullOrEmpty(text))
			{
				this.SuppressEscalation = CrimsonHelper.ParseIntStringAsBool(text);
			}
			if (propertyBag.TryGetValue("Version", out text) && !string.IsNullOrEmpty(text))
			{
				this.Version = int.Parse(text);
			}
		}

		public void Write(Action<IPersistence> preWriteHandler = null)
		{
			DefinitionIdGenerator<ResponderDefinition>.AssignId(this);
			Update<ResponderDefinition>.ApplyUpdates(this);
			if (ResponderDefinition.GlobalOverrides != null)
			{
				foreach (WorkDefinitionOverride definitionOverride in ResponderDefinition.GlobalOverrides)
				{
					definitionOverride.TryApplyTo(this, base.TraceContext);
				}
			}
			if (ResponderDefinition.LocalOverrides != null)
			{
				foreach (WorkDefinitionOverride definitionOverride2 in ResponderDefinition.LocalOverrides)
				{
					definitionOverride2.TryApplyTo(this, base.TraceContext);
				}
			}
			if (preWriteHandler != null)
			{
				preWriteHandler(this);
			}
			NativeMethods.ResponderDefinitionUnmanaged responderDefinitionUnmanaged = this.ToUnmanaged();
			NativeMethods.WriteResponderDefinition(ref responderDefinitionUnmanaged);
		}

		internal NativeMethods.ResponderDefinitionUnmanaged ToUnmanaged()
		{
			return new NativeMethods.ResponderDefinitionUnmanaged
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
				AlertMask = CrimsonHelper.NullCode(this.AlertMask),
				WaitIntervalSeconds = this.WaitIntervalSeconds,
				MinimumSecondsBetweenEscalates = this.MinimumSecondsBetweenEscalates,
				EscalationSubject = CrimsonHelper.NullCode(this.EscalationSubject),
				EscalationMessage = CrimsonHelper.NullCode(this.EscalationMessage),
				EscalationService = CrimsonHelper.NullCode(this.EscalationService),
				EscalationTeam = CrimsonHelper.NullCode(this.EscalationTeam),
				NotificationServiceClass = this.NotificationServiceClass,
				DailySchedulePattern = CrimsonHelper.NullCode(this.DailySchedulePattern),
				AlwaysEscalateOnMonitorChanges = (this.AlwaysEscalateOnMonitorChanges ? 1 : 0),
				Endpoint = CrimsonHelper.NullCode(this.Endpoint),
				CreatedById = this.CreatedById,
				Account = CrimsonHelper.NullCode(this.Account),
				AlertTypeId = CrimsonHelper.NullCode(this.AlertTypeId),
				TargetHealthState = this.TargetHealthState,
				CorrelatedMonitorsXml = CrimsonHelper.NullCode(this.CorrelatedMonitorsXml),
				ActionOnCorrelatedMonitors = this.ActionOnCorrelatedMonitors,
				ResponderCategory = CrimsonHelper.NullCode(this.ResponderCategory),
				ThrottleGroupName = CrimsonHelper.NullCode(this.ThrottleGroupName),
				ThrottlePolicyXml = CrimsonHelper.NullCode(this.ThrottlePolicyXml),
				UploadScopeNotification = (this.UploadScopeNotification ? 1 : 0),
				SuppressEscalation = (this.SuppressEscalation ? 1 : 0),
				Version = this.Version
			};
		}

		private CorrelatedMonitorInfo[] correlatedMonitors;

		private string correlatedMonitorsXml;

		private static int schemaVersion = 65536;
	}
}
