using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class ApplicationSettingsType
	{
		[DataMember]
		public bool AnalyticsEnabled { get; set; }

		[DataMember]
		public bool CoreAnalyticsEnabled { get; set; }

		[DataMember]
		public bool InferenceEnabled { get; set; }

		[DataMember]
		public TraceLevel DefaultTraceLevel { get; set; }

		[DataMember]
		public bool ConsoleTracingEnabled { get; set; }

		[DataMember]
		public PerfTraceLevel DefaultPerfTraceLevel { get; set; }

		[DataMember]
		public JsMvvmPerfTraceLevel DefaultJsMvvmPerfTraceLevel { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public string[] TraceInfoComponents { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public string[] TraceVerboseComponents { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public string[] TracePerfComponents { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public string[] TraceWarningComponents { get; set; }

		[DataMember]
		public bool WatsonEnabled { get; set; }

		[DataMember]
		public bool ManualPerfTracerEnabled { get; set; }

		[DataMember]
		public int InstrumentationSendIntervalSeconds { get; set; }

		[DataMember]
		public string StaticMapUrl { get; set; }

		[DataMember]
		public string MapControlKey { get; set; }

		[DataMember]
		public string DirectionsPageUrl { get; set; }

		[DataMember]
		public bool CheckForForgottenAttachmentsEnabled { get; set; }

		[DataMember]
		public bool ControlTasksQueueDisabled { get; set; }

		[DataMember]
		public bool CloseWindowOnLogout { get; set; }

		[DataMember]
		public bool IsLegacySignOut { get; set; }

		[DataMember]
		public WebBeaconFilterLevels FilterWebBeaconsAndHtmlForms { get; set; }

		[DataMember]
		public int FindFolderCountLimit { get; set; }
	}
}
