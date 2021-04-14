using System;
using System.Runtime.InteropServices;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Office.Datacenter.ActiveMonitoring
{
	internal static class NativeMethods
	{
		[DllImport("wevtapi.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		internal static extern bool EvtClearLog(IntPtr sessionHandle, string channelName, string targetPath, int flags);

		[DllImport("ActiveMonitoringEventMsg.dll", CharSet = CharSet.Unicode)]
		internal static extern int WriteProbeResult(ref NativeMethods.ProbeResultUnmanaged result, ResultSeverityLevel severity);

		[DllImport("ActiveMonitoringEventMsg.dll", CharSet = CharSet.Unicode)]
		internal static extern int WriteMonitorResult(ref NativeMethods.MonitorResultUnmanaged result, ResultSeverityLevel severity);

		[DllImport("ActiveMonitoringEventMsg.dll", CharSet = CharSet.Unicode)]
		internal static extern int WriteResponderResult(ref NativeMethods.ResponderResultUnmanaged result, ResultSeverityLevel severity);

		[DllImport("ActiveMonitoringEventMsg.dll", CharSet = CharSet.Unicode)]
		internal static extern int WriteMaintenanceResult(ref NativeMethods.MaintenanceResultUnmanaged result, ResultSeverityLevel severity);

		[DllImport("ActiveMonitoringEventMsg.dll", CharSet = CharSet.Unicode)]
		internal static extern int WriteProbeDefinition(ref NativeMethods.ProbeDefinitionUnmanaged definition);

		[DllImport("ActiveMonitoringEventMsg.dll", CharSet = CharSet.Unicode)]
		internal static extern int WriteMonitorDefinition(ref NativeMethods.MonitorDefinitionUnmanaged definition);

		[DllImport("ActiveMonitoringEventMsg.dll", CharSet = CharSet.Unicode)]
		internal static extern int WriteResponderDefinition(ref NativeMethods.ResponderDefinitionUnmanaged definition);

		[DllImport("ActiveMonitoringEventMsg.dll", CharSet = CharSet.Unicode)]
		internal static extern int WriteMaintenanceDefinition(ref NativeMethods.MaintenanceDefinitionUnmanaged definition);

		[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		internal static extern long GetTickCount64();

		private const string ActiveMonitoringDll = "ActiveMonitoringEventMsg.dll";

		private const string WindowsEventingApiDll = "wevtapi.dll";

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		internal struct ProbeResultUnmanaged
		{
			internal int ResultId;

			internal string ServiceName;

			internal int IsNotified;

			internal string ResultName;

			internal int WorkItemId;

			internal int DeploymentId;

			internal string MachineName;

			internal string Error;

			internal string Exception;

			internal byte RetryCount;

			internal string StateAttribute1;

			internal string StateAttribute2;

			internal string StateAttribute3;

			internal string StateAttribute4;

			internal string StateAttribute5;

			internal double StateAttribute6;

			internal double StateAttribute7;

			internal double StateAttribute8;

			internal double StateAttribute9;

			internal double StateAttribute10;

			internal string StateAttribute11;

			internal string StateAttribute12;

			internal string StateAttribute13;

			internal string StateAttribute14;

			internal string StateAttribute15;

			internal double StateAttribute16;

			internal double StateAttribute17;

			internal double StateAttribute18;

			internal double StateAttribute19;

			internal double StateAttribute20;

			internal string StateAttribute21;

			internal string StateAttribute22;

			internal string StateAttribute23;

			internal string StateAttribute24;

			internal string StateAttribute25;

			internal ResultType ResultType;

			internal int ExecutionId;

			internal string ExecutionStartTime;

			internal string ExecutionEndTime;

			internal byte PoisonedCount;

			internal string ExtensionXml;

			internal double SampleValue;

			internal string ExecutionContext;

			internal string FailureContext;

			internal int FailureCategory;

			internal string ScopeName;

			internal string ScopeType;

			internal string HealthSetName;

			internal string Data;

			internal int Version;
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		internal struct MonitorResultUnmanaged
		{
			internal int ResultId;

			internal string ServiceName;

			internal int IsNotified;

			internal string ResultName;

			internal int WorkItemId;

			internal int DeploymentId;

			internal string MachineName;

			internal string Error;

			internal string Exception;

			internal byte RetryCount;

			internal string StateAttribute1;

			internal string StateAttribute2;

			internal string StateAttribute3;

			internal string StateAttribute4;

			internal string StateAttribute5;

			internal double StateAttribute6;

			internal double StateAttribute7;

			internal double StateAttribute8;

			internal double StateAttribute9;

			internal double StateAttribute10;

			internal ResultType ResultType;

			internal int ExecutionId;

			internal string ExecutionStartTime;

			internal string ExecutionEndTime;

			internal byte PoisonedCount;

			internal int IsAlert;

			internal double TotalValue;

			internal int TotalSampleCount;

			internal int TotalFailedCount;

			internal double NewValue;

			internal int NewSampleCount;

			internal int NewFailedCount;

			internal int LastFailedProbeId;

			internal int LastFailedProbeResultId;

			internal ServiceHealthStatus HealthState;

			internal int HealthStateTransitionId;

			internal string HealthStateChangedTime;

			internal string FirstAlertObservedTime;

			internal string FirstInsufficientSamplesObservedTime;

			internal string NewStateAttribute1Value;

			internal int NewStateAttribute1Count;

			internal double NewStateAttribute1Percent;

			internal string TotalStateAttribute1Value;

			internal int TotalStateAttribute1Count;

			internal double TotalStateAttribute1Percent;

			internal int NewFailureCategoryValue;

			internal int NewFailureCategoryCount;

			internal double NewFailureCategoryPercent;

			internal int TotalFailureCategoryValue;

			internal int TotalFailureCategoryCount;

			internal double TotalFailureCategoryPercent;

			internal string ComponentName;

			internal int IsHaImpacting;

			internal string SourceScope;

			internal string TargetScopes;

			internal string HaScope;

			internal int Version;
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		internal struct ResponderResultUnmanaged
		{
			internal int ResultId;

			internal string ServiceName;

			internal int IsNotified;

			internal string ResultName;

			internal int WorkItemId;

			internal int DeploymentId;

			internal string MachineName;

			internal string Error;

			internal string Exception;

			internal byte RetryCount;

			internal string StateAttribute1;

			internal string StateAttribute2;

			internal string StateAttribute3;

			internal string StateAttribute4;

			internal string StateAttribute5;

			internal double StateAttribute6;

			internal double StateAttribute7;

			internal double StateAttribute8;

			internal double StateAttribute9;

			internal double StateAttribute10;

			internal ResultType ResultType;

			internal int ExecutionId;

			internal string ExecutionStartTime;

			internal string ExecutionEndTime;

			internal byte PoisonedCount;

			internal int IsThrottled;

			internal string ResponseResource;

			internal string ResponseAction;

			internal ServiceHealthStatus TargetHealthState;

			internal int TargetHealthStateTransitionId;

			internal string FirstAlertObservedTime;

			internal ServiceRecoveryResult RecoveryResult;

			internal int IsRecoveryAttempted;

			internal string CorrelationResultsXml;

			internal CorrelatedMonitorAction CorrelationAction;

			internal int Version;
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		internal struct MaintenanceResultUnmanaged
		{
			internal int ResultId;

			internal string ServiceName;

			internal int IsNotified;

			internal string ResultName;

			internal int WorkItemId;

			internal int DeploymentId;

			internal string MachineName;

			internal string Error;

			internal string Exception;

			internal byte RetryCount;

			internal string StateAttribute1;

			internal string StateAttribute2;

			internal string StateAttribute3;

			internal string StateAttribute4;

			internal string StateAttribute5;

			internal double StateAttribute6;

			internal double StateAttribute7;

			internal double StateAttribute8;

			internal double StateAttribute9;

			internal double StateAttribute10;

			internal ResultType ResultType;

			internal int ExecutionId;

			internal string ExecutionStartTime;

			internal string ExecutionEndTime;

			internal byte PoisonedCount;

			internal int Version;
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		internal struct ProbeDefinitionUnmanaged
		{
			internal int Id;

			internal string AssemblyPath;

			internal string TypeName;

			internal string Name;

			internal string WorkItemVersion;

			internal string ServiceName;

			internal int DeploymentId;

			internal string ExecutionLocation;

			internal string CreatedTime;

			internal int Enabled;

			internal string TargetPartition;

			internal string TargetGroup;

			internal string TargetResource;

			internal string TargetExtension;

			internal string TargetVersion;

			internal int RecurrenceIntervalSeconds;

			internal int TimeoutSeconds;

			internal string StartTime;

			internal string UpdateTime;

			internal int MaxRetryAttempts;

			internal string ExtensionAttributes;

			internal int CreatedById;

			internal string Account;

			internal string AccountDisplayName;

			internal string Endpoint;

			internal string SecondaryAccount;

			internal string SecondaryAccountDisplayName;

			internal string SecondaryEndpoint;

			internal string ExtensionEndpoints;

			internal int Version;

			internal int ExecutionType;
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		internal struct MonitorDefinitionUnmanaged
		{
			internal int Id;

			internal string AssemblyPath;

			internal string TypeName;

			internal string Name;

			internal string WorkItemVersion;

			internal string ServiceName;

			internal int DeploymentId;

			internal string ExecutionLocation;

			internal string CreatedTime;

			internal int Enabled;

			internal string TargetPartition;

			internal string TargetGroup;

			internal string TargetResource;

			internal string TargetExtension;

			internal string TargetVersion;

			internal int RecurrenceIntervalSeconds;

			internal int TimeoutSeconds;

			internal string StartTime;

			internal string UpdateTime;

			internal int MaxRetryAttempts;

			internal string ExtensionAttributes;

			internal string SampleMask;

			internal int MonitoringIntervalSeconds;

			internal int MinimumErrorCount;

			internal double MonitoringThreshold;

			internal double SecondaryMonitoringThreshold;

			internal double MonitoringSamplesThreshold;

			internal int ServicePriority;

			internal ServiceSeverity ServiceSeverity;

			internal int IsHaImpacting;

			internal int CreatedById;

			internal int InsufficientSamplesIntervalSeconds;

			internal string StateAttribute1Mask;

			internal int FailureCategoryMask;

			internal string ComponentName;

			internal string StateTransitionsXml;

			internal int AllowCorrelationToMonitor;

			internal string ScenarioDescription;

			internal string SourceScope;

			internal string TargetScopes;

			internal string HaScope;

			internal int Version;
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		internal struct ResponderDefinitionUnmanaged
		{
			internal int Id;

			internal string AssemblyPath;

			internal string TypeName;

			internal string Name;

			internal string WorkItemVersion;

			internal string ServiceName;

			internal int DeploymentId;

			internal string ExecutionLocation;

			internal string CreatedTime;

			internal int Enabled;

			internal string TargetPartition;

			internal string TargetGroup;

			internal string TargetResource;

			internal string TargetExtension;

			internal string TargetVersion;

			internal int RecurrenceIntervalSeconds;

			internal int TimeoutSeconds;

			internal string StartTime;

			internal string UpdateTime;

			internal int MaxRetryAttempts;

			internal string ExtensionAttributes;

			internal string AlertMask;

			internal int WaitIntervalSeconds;

			internal int MinimumSecondsBetweenEscalates;

			internal string EscalationSubject;

			internal string EscalationMessage;

			internal string EscalationService;

			internal string EscalationTeam;

			internal NotificationServiceClass NotificationServiceClass;

			internal string DailySchedulePattern;

			internal int AlwaysEscalateOnMonitorChanges;

			internal string Endpoint;

			internal int CreatedById;

			internal string Account;

			internal string AlertTypeId;

			internal ServiceHealthStatus TargetHealthState;

			internal string CorrelatedMonitorsXml;

			internal CorrelatedMonitorAction ActionOnCorrelatedMonitors;

			internal string ResponderCategory;

			internal string ThrottleGroupName;

			internal string ThrottlePolicyXml;

			internal int UploadScopeNotification;

			internal int SuppressEscalation;

			internal int Version;
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		internal struct MaintenanceDefinitionUnmanaged
		{
			internal int MaxRestartRequestAllowedPerHour;

			internal int Id;

			internal string AssemblyPath;

			internal string TypeName;

			internal string Name;

			internal string WorkItemVersion;

			internal string ServiceName;

			internal int DeploymentId;

			internal string ExecutionLocation;

			internal string CreatedTime;

			internal int Enabled;

			internal string TargetPartition;

			internal string TargetGroup;

			internal string TargetResource;

			internal string TargetExtension;

			internal string TargetVersion;

			internal int RecurrenceIntervalSeconds;

			internal int TimeoutSeconds;

			internal string StartTime;

			internal string UpdateTime;

			internal int MaxRetryAttempts;

			internal string ExtensionAttributes;

			internal int CreatedById;

			internal int Version;
		}
	}
}
