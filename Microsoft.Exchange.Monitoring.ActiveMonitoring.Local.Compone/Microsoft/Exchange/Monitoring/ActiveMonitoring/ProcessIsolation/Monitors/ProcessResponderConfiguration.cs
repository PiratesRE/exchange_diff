using System;
using System.Collections.Generic;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.ProcessIsolation.Monitors
{
	internal class ProcessResponderConfiguration
	{
		internal ProcessResponderConfiguration(Dictionary<MonitorStateTransition, ResponderDefinitionDelegate> privateWorkingSetTriggerWarningResponders, Dictionary<MonitorStateTransition, ResponderDefinitionDelegate> privateWorkingSetTriggerErrorResponders, Dictionary<MonitorStateTransition, ResponderDefinitionDelegate> processProcessorTimeTriggerWarningResponders, Dictionary<MonitorStateTransition, ResponderDefinitionDelegate> processProcessorTimeTriggerErrorResponders, Dictionary<MonitorStateTransition, ResponderDefinitionDelegate> exchangeCrashEventTriggerErrorResponders, Dictionary<MonitorStateTransition, ResponderDefinitionDelegate> watsonWarningResponders)
		{
			this.PrivateWorkingSetTriggerWarningResponders = privateWorkingSetTriggerWarningResponders;
			this.PrivateWorkingSetTriggerErrorResponders = privateWorkingSetTriggerErrorResponders;
			this.ProcessProcessorTimeTriggerWarningResponders = processProcessorTimeTriggerWarningResponders;
			this.ProcessProcessorTimeTriggerErrorResponders = processProcessorTimeTriggerErrorResponders;
			this.ExchangeCrashEventTriggerErrorResponders = exchangeCrashEventTriggerErrorResponders;
			this.WatsonWarningResponders = watsonWarningResponders;
		}

		internal static ProcessResponderConfiguration CollectDiagnosticsWatsonEscalateResponse
		{
			get
			{
				return ProcessResponderConfiguration.collectDiagnosticsWatsonEscalateResponse;
			}
		}

		internal static ProcessResponderConfiguration CollectDiagnosticsWatsonRestartEscalateResponse
		{
			get
			{
				return ProcessResponderConfiguration.collectDiagnosticsWatsonRestartEscalateResponse;
			}
		}

		internal static ProcessResponderConfiguration CreateBugsAndEscalateResponse
		{
			get
			{
				return ProcessResponderConfiguration.createBugsAndEscalateResponse;
			}
		}

		internal static ProcessResponderConfiguration EscalateOnlyResponse
		{
			get
			{
				return ProcessResponderConfiguration.escalateOnlyResponse;
			}
		}

		internal static ProcessResponderConfiguration RestartNodeRunnerEscalateResponse
		{
			get
			{
				return ProcessResponderConfiguration.restartNodeRunnerEscalateResponse;
			}
		}

		internal static ProcessResponderConfiguration RestartIndexNodeRunnerEscalateResponse
		{
			get
			{
				return ProcessResponderConfiguration.restartIndexNodeRunnerEscalateResponse;
			}
		}

		internal static ProcessResponderConfiguration NoResponse
		{
			get
			{
				return ProcessResponderConfiguration.noResponse;
			}
		}

		internal static ProcessResponderConfiguration WatsonDiagKillEscalateResponse
		{
			get
			{
				return ProcessResponderConfiguration.watsonDiagKillEscalateResponse;
			}
		}

		internal static ProcessResponderConfiguration KillProcessEscalateResponse
		{
			get
			{
				return ProcessResponderConfiguration.killProcessEscalateResponse;
			}
		}

		internal Dictionary<MonitorStateTransition, ResponderDefinitionDelegate> PrivateWorkingSetTriggerWarningResponders { get; private set; }

		internal Dictionary<MonitorStateTransition, ResponderDefinitionDelegate> PrivateWorkingSetTriggerErrorResponders { get; private set; }

		internal Dictionary<MonitorStateTransition, ResponderDefinitionDelegate> ProcessProcessorTimeTriggerWarningResponders { get; private set; }

		internal Dictionary<MonitorStateTransition, ResponderDefinitionDelegate> ProcessProcessorTimeTriggerErrorResponders { get; private set; }

		internal Dictionary<MonitorStateTransition, ResponderDefinitionDelegate> ExchangeCrashEventTriggerErrorResponders { get; private set; }

		internal Dictionary<MonitorStateTransition, ResponderDefinitionDelegate> WatsonWarningResponders { get; private set; }

		private static readonly Dictionary<MonitorStateTransition, ResponderDefinitionDelegate> watsonWithF1TraceChain = new Dictionary<MonitorStateTransition, ResponderDefinitionDelegate>
		{
			{
				new MonitorStateTransition(ServiceHealthStatus.Degraded, 0),
				new ResponderDefinitionDelegate(ProcessIsolationDiscovery.WatsonInformationalWithF1Trace)
			}
		};

		private static readonly Dictionary<MonitorStateTransition, ResponderDefinitionDelegate> watsonWithF1TraceEscalateChain = new Dictionary<MonitorStateTransition, ResponderDefinitionDelegate>
		{
			{
				new MonitorStateTransition(ServiceHealthStatus.Degraded, 0),
				new ResponderDefinitionDelegate(ProcessIsolationDiscovery.WatsonInformationalWithF1Trace)
			},
			{
				new MonitorStateTransition(ServiceHealthStatus.Unhealthy, (int)TimeSpan.FromMinutes(60.0).TotalSeconds),
				new ResponderDefinitionDelegate(ProcessIsolationDiscovery.CreateEscalateResponder)
			}
		};

		private static readonly Dictionary<MonitorStateTransition, ResponderDefinitionDelegate> watsonWithDumpChain = new Dictionary<MonitorStateTransition, ResponderDefinitionDelegate>
		{
			{
				new MonitorStateTransition(ServiceHealthStatus.Degraded, 0),
				new ResponderDefinitionDelegate(ProcessIsolationDiscovery.WatsonWithDump)
			}
		};

		private static readonly Dictionary<MonitorStateTransition, ResponderDefinitionDelegate> watsonWithDumpRestartChain = new Dictionary<MonitorStateTransition, ResponderDefinitionDelegate>
		{
			{
				new MonitorStateTransition(ServiceHealthStatus.Degraded, 0),
				new ResponderDefinitionDelegate(ProcessIsolationDiscovery.WatsonWithDump)
			},
			{
				new MonitorStateTransition(ServiceHealthStatus.Degraded1, (int)TimeSpan.FromMinutes(10.0).TotalSeconds),
				new ResponderDefinitionDelegate(ProcessIsolationDiscovery.RestartProcess)
			}
		};

		private static readonly Dictionary<MonitorStateTransition, ResponderDefinitionDelegate> watsonWithDumpEscalateChain = new Dictionary<MonitorStateTransition, ResponderDefinitionDelegate>
		{
			{
				new MonitorStateTransition(ServiceHealthStatus.Degraded, 0),
				new ResponderDefinitionDelegate(ProcessIsolationDiscovery.WatsonWithDump)
			},
			{
				new MonitorStateTransition(ServiceHealthStatus.Unhealthy, (int)TimeSpan.FromMinutes(60.0).TotalSeconds),
				new ResponderDefinitionDelegate(ProcessIsolationDiscovery.CreateEscalateResponder)
			}
		};

		private static readonly Dictionary<MonitorStateTransition, ResponderDefinitionDelegate> watsonWithDumpRestartEscalateChain = new Dictionary<MonitorStateTransition, ResponderDefinitionDelegate>
		{
			{
				new MonitorStateTransition(ServiceHealthStatus.Degraded, 0),
				new ResponderDefinitionDelegate(ProcessIsolationDiscovery.WatsonWithDump)
			},
			{
				new MonitorStateTransition(ServiceHealthStatus.Degraded1, (int)TimeSpan.FromMinutes(10.0).TotalSeconds),
				new ResponderDefinitionDelegate(ProcessIsolationDiscovery.RestartProcess)
			},
			{
				new MonitorStateTransition(ServiceHealthStatus.Unhealthy, (int)TimeSpan.FromMinutes(60.0).TotalSeconds),
				new ResponderDefinitionDelegate(ProcessIsolationDiscovery.CreateEscalateResponder)
			}
		};

		private static readonly Dictionary<MonitorStateTransition, ResponderDefinitionDelegate> watsonInformationalWithF1TraceChain = new Dictionary<MonitorStateTransition, ResponderDefinitionDelegate>
		{
			{
				new MonitorStateTransition(ServiceHealthStatus.Degraded, 0),
				new ResponderDefinitionDelegate(ProcessIsolationDiscovery.WatsonInformationalWithF1Trace)
			}
		};

		private static readonly Dictionary<MonitorStateTransition, ResponderDefinitionDelegate> watsonInformationalWithF1TraceAndEscalateChain = new Dictionary<MonitorStateTransition, ResponderDefinitionDelegate>
		{
			{
				new MonitorStateTransition(ServiceHealthStatus.Degraded, 0),
				new ResponderDefinitionDelegate(ProcessIsolationDiscovery.WatsonInformationalWithF1Trace)
			},
			{
				new MonitorStateTransition(ServiceHealthStatus.Unhealthy, (int)TimeSpan.FromMinutes(10.0).TotalSeconds),
				new ResponderDefinitionDelegate(ProcessIsolationDiscovery.CreateEscalateResponder)
			}
		};

		private static readonly Dictionary<MonitorStateTransition, ResponderDefinitionDelegate> watsonInformationalAndEscalateChain = new Dictionary<MonitorStateTransition, ResponderDefinitionDelegate>
		{
			{
				new MonitorStateTransition(ServiceHealthStatus.Degraded, 0),
				new ResponderDefinitionDelegate(ProcessIsolationDiscovery.WatsonInformational)
			},
			{
				new MonitorStateTransition(ServiceHealthStatus.Unhealthy, (int)TimeSpan.FromMinutes(60.0).TotalSeconds),
				new ResponderDefinitionDelegate(ProcessIsolationDiscovery.CreateEscalateResponder)
			}
		};

		private static readonly Dictionary<MonitorStateTransition, ResponderDefinitionDelegate> watsonInformationalChain = new Dictionary<MonitorStateTransition, ResponderDefinitionDelegate>
		{
			{
				new MonitorStateTransition(ServiceHealthStatus.Degraded, 0),
				new ResponderDefinitionDelegate(ProcessIsolationDiscovery.WatsonInformational)
			}
		};

		private static readonly Dictionary<MonitorStateTransition, ResponderDefinitionDelegate> escalateChain = new Dictionary<MonitorStateTransition, ResponderDefinitionDelegate>
		{
			{
				new MonitorStateTransition(ServiceHealthStatus.Unhealthy, 0),
				new ResponderDefinitionDelegate(ProcessIsolationDiscovery.CreateEscalateResponder)
			}
		};

		private static readonly Dictionary<MonitorStateTransition, ResponderDefinitionDelegate> restartNodeRunnerEscalateChain = new Dictionary<MonitorStateTransition, ResponderDefinitionDelegate>
		{
			{
				new MonitorStateTransition(ServiceHealthStatus.Degraded, 0),
				new ResponderDefinitionDelegate(ProcessIsolationDiscovery.RestartNodeRunner)
			},
			{
				new MonitorStateTransition(ServiceHealthStatus.Unhealthy, (int)TimeSpan.FromMinutes(60.0).TotalSeconds),
				new ResponderDefinitionDelegate(ProcessIsolationDiscovery.CreateEscalateResponder)
			}
		};

		private static readonly Dictionary<MonitorStateTransition, ResponderDefinitionDelegate> f1RestartNodeRunnerEscalateProcessorAffinitizeChain = new Dictionary<MonitorStateTransition, ResponderDefinitionDelegate>
		{
			{
				new MonitorStateTransition(ServiceHealthStatus.Degraded, 0),
				new ResponderDefinitionDelegate(ProcessIsolationDiscovery.WatsonInformationalWithF1Trace)
			},
			{
				new MonitorStateTransition(ServiceHealthStatus.Unhealthy, (int)TimeSpan.FromMinutes(10.0).TotalSeconds),
				new ResponderDefinitionDelegate(ProcessIsolationDiscovery.RestartNodeRunner)
			},
			{
				new MonitorStateTransition(ServiceHealthStatus.Unhealthy1, (int)TimeSpan.FromMinutes(60.0).TotalSeconds),
				new ResponderDefinitionDelegate(ProcessIsolationDiscovery.CreateEscalateResponder)
			},
			{
				new MonitorStateTransition(ServiceHealthStatus.Unhealthy2, (int)TimeSpan.FromMinutes(65.0).TotalSeconds),
				new ResponderDefinitionDelegate(ProcessIsolationDiscovery.ProcessorAffinitize)
			}
		};

		private static readonly Dictionary<MonitorStateTransition, ResponderDefinitionDelegate> watsonDiagChain = new Dictionary<MonitorStateTransition, ResponderDefinitionDelegate>
		{
			{
				new MonitorStateTransition(ServiceHealthStatus.Degraded, (int)TimeSpan.FromSeconds(0.0).TotalSeconds),
				new ResponderDefinitionDelegate(ProcessIsolationDiscovery.KillProcess)
			},
			{
				new MonitorStateTransition(ServiceHealthStatus.Degraded1, (int)TimeSpan.FromSeconds(5.0).TotalSeconds),
				new ResponderDefinitionDelegate(ProcessIsolationDiscovery.DeleteWatsonTempDumpFiles)
			}
		};

		private static readonly Dictionary<MonitorStateTransition, ResponderDefinitionDelegate> killProcessEscalateChain = new Dictionary<MonitorStateTransition, ResponderDefinitionDelegate>
		{
			{
				new MonitorStateTransition(ServiceHealthStatus.Degraded, (int)TimeSpan.FromSeconds(0.0).TotalSeconds),
				new ResponderDefinitionDelegate(ProcessIsolationDiscovery.KillProcess)
			},
			{
				new MonitorStateTransition(ServiceHealthStatus.Unhealthy, (int)TimeSpan.FromMinutes(5.0).TotalSeconds),
				new ResponderDefinitionDelegate(ProcessIsolationDiscovery.CreateEscalateResponder)
			}
		};

		private static readonly Dictionary<MonitorStateTransition, ResponderDefinitionDelegate> noResponseChain = new Dictionary<MonitorStateTransition, ResponderDefinitionDelegate>();

		private static readonly ProcessResponderConfiguration collectDiagnosticsWatsonEscalateResponse = new ProcessResponderConfiguration(ProcessResponderConfiguration.watsonWithDumpChain, ProcessResponderConfiguration.watsonWithDumpEscalateChain, ProcessResponderConfiguration.watsonWithF1TraceChain, ProcessResponderConfiguration.watsonWithF1TraceEscalateChain, ProcessResponderConfiguration.escalateChain, ProcessResponderConfiguration.noResponseChain);

		private static readonly ProcessResponderConfiguration collectDiagnosticsWatsonRestartEscalateResponse = new ProcessResponderConfiguration(ProcessResponderConfiguration.watsonWithDumpRestartChain, ProcessResponderConfiguration.watsonWithDumpRestartEscalateChain, ProcessResponderConfiguration.watsonWithF1TraceChain, ProcessResponderConfiguration.watsonWithF1TraceEscalateChain, ProcessResponderConfiguration.escalateChain, ProcessResponderConfiguration.noResponseChain);

		private static readonly ProcessResponderConfiguration createBugsAndEscalateResponse = new ProcessResponderConfiguration(ProcessResponderConfiguration.watsonInformationalChain, ProcessResponderConfiguration.watsonInformationalAndEscalateChain, ProcessResponderConfiguration.watsonInformationalWithF1TraceChain, ProcessResponderConfiguration.watsonInformationalWithF1TraceAndEscalateChain, ProcessResponderConfiguration.escalateChain, ProcessResponderConfiguration.noResponseChain);

		private static readonly ProcessResponderConfiguration escalateOnlyResponse = new ProcessResponderConfiguration(ProcessResponderConfiguration.escalateChain, ProcessResponderConfiguration.escalateChain, ProcessResponderConfiguration.escalateChain, ProcessResponderConfiguration.escalateChain, ProcessResponderConfiguration.escalateChain, ProcessResponderConfiguration.noResponseChain);

		private static readonly ProcessResponderConfiguration restartNodeRunnerEscalateResponse = new ProcessResponderConfiguration(ProcessResponderConfiguration.restartNodeRunnerEscalateChain, ProcessResponderConfiguration.restartNodeRunnerEscalateChain, ProcessResponderConfiguration.f1RestartNodeRunnerEscalateProcessorAffinitizeChain, ProcessResponderConfiguration.f1RestartNodeRunnerEscalateProcessorAffinitizeChain, ProcessResponderConfiguration.escalateChain, ProcessResponderConfiguration.noResponseChain);

		private static readonly ProcessResponderConfiguration restartIndexNodeRunnerEscalateResponse = new ProcessResponderConfiguration(ProcessResponderConfiguration.restartNodeRunnerEscalateChain, ProcessResponderConfiguration.restartNodeRunnerEscalateChain, ProcessResponderConfiguration.watsonWithF1TraceChain, ProcessResponderConfiguration.f1RestartNodeRunnerEscalateProcessorAffinitizeChain, ProcessResponderConfiguration.escalateChain, ProcessResponderConfiguration.noResponseChain);

		private static readonly ProcessResponderConfiguration noResponse = new ProcessResponderConfiguration(ProcessResponderConfiguration.noResponseChain, ProcessResponderConfiguration.noResponseChain, ProcessResponderConfiguration.noResponseChain, ProcessResponderConfiguration.noResponseChain, ProcessResponderConfiguration.noResponseChain, ProcessResponderConfiguration.noResponseChain);

		private static readonly ProcessResponderConfiguration watsonDiagKillEscalateResponse = new ProcessResponderConfiguration(ProcessResponderConfiguration.watsonDiagChain, ProcessResponderConfiguration.watsonDiagChain, ProcessResponderConfiguration.noResponseChain, ProcessResponderConfiguration.noResponseChain, ProcessResponderConfiguration.noResponseChain, ProcessResponderConfiguration.watsonDiagChain);

		private static readonly ProcessResponderConfiguration killProcessEscalateResponse = new ProcessResponderConfiguration(ProcessResponderConfiguration.killProcessEscalateChain, ProcessResponderConfiguration.killProcessEscalateChain, ProcessResponderConfiguration.watsonWithF1TraceChain, ProcessResponderConfiguration.watsonWithF1TraceEscalateChain, ProcessResponderConfiguration.escalateChain, ProcessResponderConfiguration.noResponseChain);
	}
}
