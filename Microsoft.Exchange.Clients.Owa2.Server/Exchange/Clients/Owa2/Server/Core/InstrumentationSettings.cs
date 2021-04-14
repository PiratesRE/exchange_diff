using System;
using System.Collections.Specialized;
using System.Configuration;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class InstrumentationSettings
	{
		public InstrumentationSettings()
		{
		}

		public InstrumentationSettings(NameValueCollection settings)
		{
			float analyticsProbability;
			if (float.TryParse(settings["AnalyticsProbability"], out analyticsProbability))
			{
				this.AnalyticsProbability = analyticsProbability;
			}
			float coreAnalyticsProbability;
			if (float.TryParse(settings["CoreAnalyticsProbability"], out coreAnalyticsProbability))
			{
				this.CoreAnalyticsProbability = coreAnalyticsProbability;
			}
			bool isInferenceEnabled;
			if (bool.TryParse(settings["InferenceEnabled"], out isInferenceEnabled))
			{
				this.IsInferenceEnabled = isInferenceEnabled;
			}
			bool isConsoleTracingEnabled;
			if (bool.TryParse(settings["ConsoleTracingEnabled"], out isConsoleTracingEnabled))
			{
				this.IsConsoleTracingEnabled = isConsoleTracingEnabled;
			}
			TraceLevel defaultTraceLevel;
			if (Enum.TryParse<TraceLevel>(settings["DefaultTraceLevel"], out defaultTraceLevel))
			{
				this.DefaultTraceLevel = defaultTraceLevel;
			}
			JsMvvmPerfTraceLevel jsMvvmPerfTraceLevel;
			if (Enum.TryParse<JsMvvmPerfTraceLevel>(settings["DefaultPerfTraceLevel"], out jsMvvmPerfTraceLevel))
			{
				this.DefaultJsMvvmPerfTraceLevel = jsMvvmPerfTraceLevel;
				this.DefaultPerfTraceLevel = InstrumentationSettings.ConvertToOldPerfTraceLevel(jsMvvmPerfTraceLevel);
			}
			string value = settings["TraceInfoComponents"];
			if (!string.IsNullOrEmpty(value))
			{
				this.TraceInfoComponents = InstrumentationSettings.CommaSeperatedStringToArray(value);
			}
			string value2 = settings["TracePerfComponents"];
			if (!string.IsNullOrEmpty(value2))
			{
				this.TracePerfComponents = InstrumentationSettings.CommaSeperatedStringToArray(value2);
			}
			string value3 = settings["TraceVerboseComponents"];
			if (!string.IsNullOrEmpty(value3))
			{
				this.TraceVerboseComponents = InstrumentationSettings.CommaSeperatedStringToArray(value3);
			}
			string value4 = settings["TraceWarningComponents"];
			if (!string.IsNullOrEmpty(value4))
			{
				this.TraceWarningComponents = InstrumentationSettings.CommaSeperatedStringToArray(value4);
			}
			bool isClientWatsonEnabled;
			if (bool.TryParse(settings["ClientWatsonEnabled"], out isClientWatsonEnabled))
			{
				this.IsClientWatsonEnabled = isClientWatsonEnabled;
			}
			TimeSpan sendInterval;
			if (TimeSpan.TryParse(settings["SendInterval"], out sendInterval))
			{
				this.SendInterval = sendInterval;
			}
			bool isManualPerfTracerEnabled;
			if (bool.TryParse(settings["ManualPerfTracerEnabled"], out isManualPerfTracerEnabled))
			{
				this.IsManualPerfTracerEnabled = isManualPerfTracerEnabled;
			}
		}

		public static InstrumentationSettings Instance
		{
			get
			{
				if (InstrumentationSettings.instance == null)
				{
					InstrumentationSettings.instance = new InstrumentationSettings(ConfigurationManager.AppSettings);
				}
				return InstrumentationSettings.instance;
			}
		}

		public float AnalyticsProbability { get; set; }

		public float CoreAnalyticsProbability { get; set; }

		public bool IsInferenceEnabled { get; set; }

		public bool IsConsoleTracingEnabled { get; set; }

		public TraceLevel DefaultTraceLevel { get; set; }

		public PerfTraceLevel DefaultPerfTraceLevel { get; set; }

		public JsMvvmPerfTraceLevel DefaultJsMvvmPerfTraceLevel { get; set; }

		public string[] TraceInfoComponents { get; set; }

		public string[] TracePerfComponents { get; set; }

		public string[] TraceVerboseComponents { get; set; }

		public string[] TraceWarningComponents { get; set; }

		public bool IsClientWatsonEnabled { get; set; }

		public TimeSpan SendInterval { get; set; }

		public bool IsManualPerfTracerEnabled { get; set; }

		public bool IsInstrumentationEnabled()
		{
			return this.DefaultTraceLevel != TraceLevel.Off || this.DefaultPerfTraceLevel != PerfTraceLevel.Off || this.IsInferenceEnabled || this.IsClientWatsonEnabled || this.CoreAnalyticsProbability > 0f || this.AnalyticsProbability > 0f;
		}

		internal static PerfTraceLevel ConvertToOldPerfTraceLevel(JsMvvmPerfTraceLevel perfLevel)
		{
			switch (perfLevel)
			{
			case JsMvvmPerfTraceLevel.Essential:
				return PerfTraceLevel.Execution;
			case JsMvvmPerfTraceLevel.Info:
				return PerfTraceLevel.Detailed;
			case JsMvvmPerfTraceLevel.Verbose:
				return PerfTraceLevel.Component;
			case JsMvvmPerfTraceLevel.Debug:
				return PerfTraceLevel.Logging;
			default:
				return PerfTraceLevel.Off;
			}
		}

		private static string[] CommaSeperatedStringToArray(string value)
		{
			return Array.FindAll<string>(value.Replace(" ", string.Empty).Split(new char[]
			{
				','
			}), (string component) => !string.IsNullOrEmpty(component));
		}

		public const string AnalyticsProbabilityKey = "AnalyticsProbability";

		public const string CoreAnalyticsProbabilityKey = "CoreAnalyticsProbability";

		public const string InferenceEnabledKey = "InferenceEnabled";

		public const string ConsoleTracingEnabledKey = "ConsoleTracingEnabled";

		public const string DefaultTraceLevelKey = "DefaultTraceLevel";

		public const string DefaultPerfTraceLevelKey = "DefaultPerfTraceLevel";

		public const string TraceInfoComponentsKey = "TraceInfoComponents";

		public const string TracePerfComponentsKey = "TracePerfComponents";

		public const string TraceVerboseComponentsKey = "TraceVerboseComponents";

		public const string TraceWarningComponentsKey = "TraceWarningComponents";

		public const string ClientWatsonEnabledKey = "ClientWatsonEnabled";

		public const string SendIntervalKey = "SendInterval";

		public const string ManualPerfTracerEnabled = "ManualPerfTracerEnabled";

		private static InstrumentationSettings instance;
	}
}
