using System;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Directory
{
	public class DirectoryMonitoringScenario
	{
		internal string Scenario { get; private set; }

		internal string EscalationMessageSubject { get; private set; }

		public DirectoryMonitoringScenario(string scenario, string escalationMessageSubject, bool allowCorrelation)
		{
			this.Scenario = scenario;
			this.EscalationMessageSubject = escalationMessageSubject;
			this.AllowCorrelationToMonitoring = allowCorrelation;
		}

		public DirectoryMonitoringScenario(string scenario, string escalationMessageSubject) : this(scenario, escalationMessageSubject, false)
		{
		}

		public string ProbeName
		{
			get
			{
				return string.Format("{0}Probe", this.Scenario);
			}
		}

		public string MonitorName
		{
			get
			{
				return string.Format("{0}Monitor", this.Scenario);
			}
		}

		public string EscalateResponderName
		{
			get
			{
				return string.Format("{0}Escalate", this.Scenario);
			}
		}

		public string EscalatePerExceptionResponderName(string exception)
		{
			return string.Format("{0}{1}Escalate", this.Scenario, exception);
		}

		public string RestartResponderName
		{
			get
			{
				return string.Format("{0}Restart", this.Scenario);
			}
		}

		public string RestartServerResponderName
		{
			get
			{
				return string.Format("{0}ServerReboot", this.Scenario);
			}
		}

		public string PutDCInMMResponderName
		{
			get
			{
				return string.Format("{0}PutDCIntoMM", this.Scenario);
			}
		}

		public string PutMultipleDCInMMResponderName
		{
			get
			{
				return string.Format("{0}PutMultipleDCInMM", this.Scenario);
			}
		}

		public string CheckDCInMMEscalateResponderName
		{
			get
			{
				return string.Format("{0}CheckDCInMMEscalate", this.Scenario);
			}
		}

		public string CheckMultipleDCInMMEscalateResponderName
		{
			get
			{
				return string.Format("{0}CheckMultipleDCInMMEscalate", this.Scenario);
			}
		}

		public string RenameNTDSPowerOffResponderName
		{
			get
			{
				return string.Format("{0}RenameNTDSPowerOff", this.Scenario);
			}
		}

		public string ADDiagTraceResponderName
		{
			get
			{
				return string.Format("{0}ADDiagTrace", this.Scenario);
			}
		}

		public bool AllowCorrelationToMonitoring { get; private set; }
	}
}
