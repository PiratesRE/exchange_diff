using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading;
using System.Xml.Linq;
using Microsoft.Exchange.Data.ApplicationLogic;
using Microsoft.Exchange.Data.Directory.ResourceHealth;
using Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.WorkloadManagement
{
	internal class SystemWorkloadManager : DisposeTrackableBase, IDiagnosable, ITaskProviderManager
	{
		private SystemWorkloadManager(IWorkloadLogger logger, bool ignoreImplicitLocalCpuResource)
		{
			if (logger == null)
			{
				throw new ArgumentNullException("logger");
			}
			this.perfCounters = new WorkloadManagementPerfCounterWrapper();
			this.resourceReservationContext = new ResourceReservationContext(ignoreImplicitLocalCpuResource);
			this.classificationBlocks = new ClassificationDictionary<ClassificationBlock>((WorkloadClassification classification) => new ClassificationBlock(classification)
			{
				FairnessFactor = SystemWorkloadManager.ReadAppSettingAsInt(classification.ToString() + "Factor", this.GetDefaultFactor(classification))
			});
			this.workloadExecution = new WorkloadExecution(this);
			this.logger = new WorkloadManagementLogger(logger);
			this.classificationUpdateTimer = new Timer(new TimerCallback(this.ClassificationUpdate), null, TimeSpan.FromMinutes(1.0), TimeSpan.FromMinutes(1.0));
		}

		public static WorkloadExecutionStatus Status
		{
			get
			{
				SystemWorkloadManager systemWorkloadManager = SystemWorkloadManager.manager;
				if (systemWorkloadManager != null)
				{
					return systemWorkloadManager.workloadExecution.Status;
				}
				return WorkloadExecutionStatus.NotInitialized;
			}
		}

		public static void Initialize(IWorkloadLogger logger)
		{
			SystemWorkloadManager.Initialize(logger, true);
		}

		public static void Initialize(IWorkloadLogger logger, bool registerDiagnostics)
		{
			SystemWorkloadManager.InternalInitialize(logger, registerDiagnostics, false);
		}

		public static void Shutdown()
		{
			SystemWorkloadManager systemWorkloadManager = SystemWorkloadManager.manager;
			if (systemWorkloadManager != null)
			{
				lock (systemWorkloadManager.instanceLock)
				{
					if (SystemWorkloadManager.manager != null)
					{
						SystemWorkloadManager.manager = null;
					}
					else
					{
						systemWorkloadManager = null;
					}
				}
			}
			if (systemWorkloadManager != null)
			{
				systemWorkloadManager.Dispose();
			}
		}

		public static void RegisterWorkload(SystemWorkloadBase workload)
		{
			SystemWorkloadManager systemWorkloadManager = SystemWorkloadManager.manager;
			if (systemWorkloadManager == null)
			{
				throw new InvalidOperationException("System workload manager is not initialized.");
			}
			systemWorkloadManager.InternalRegisterWorkload(workload);
		}

		public static void UnregisterWorkload(SystemWorkloadBase workload)
		{
			SystemWorkloadManager systemWorkloadManager = SystemWorkloadManager.manager;
			if (systemWorkloadManager == null)
			{
				throw new InvalidOperationException("System workload manager is not initialized.");
			}
			systemWorkloadManager.InternalUnregisterWorkload(workload);
		}

		public static bool IsClassificationActive(WorkloadClassification classification)
		{
			if (SystemWorkloadManager.manager == null)
			{
				throw new InvalidOperationException("System workload manager is not initialized.");
			}
			return SystemWorkloadManager.manager.classificationBlocks[classification].WorkloadCount > 0;
		}

		string IDiagnosable.GetDiagnosticComponentName()
		{
			return "SystemWorkloadManager";
		}

		XElement IDiagnosable.GetDiagnosticInfo(DiagnosableParameters parameters)
		{
			XElement xelement = new XElement("SystemWorkloadManager");
			if (string.IsNullOrEmpty(parameters.Argument) || string.Equals(parameters.Argument, "help", StringComparison.OrdinalIgnoreCase))
			{
				xelement.Add(new XElement("help", "Supported arguments are Workload, Task, Resource, Admission, Policy, History,."));
			}
			else
			{
				if (0 <= parameters.Argument.IndexOf("workload", StringComparison.OrdinalIgnoreCase))
				{
					this.GenerateWorkloadDiagnosticsInfo(xelement);
				}
				if (0 <= parameters.Argument.IndexOf("task", StringComparison.OrdinalIgnoreCase))
				{
					this.GenerateTaskDiagnosticsInfo(xelement);
				}
				if (0 <= parameters.Argument.IndexOf("resource", StringComparison.OrdinalIgnoreCase))
				{
					this.GenerateResourceDiagnosticsInfo(xelement);
				}
				if (0 <= parameters.Argument.IndexOf("history", StringComparison.OrdinalIgnoreCase))
				{
					this.GenerateBlackBoxDiagnosticsInfo(xelement);
				}
				if (0 <= parameters.Argument.IndexOf("admission", StringComparison.OrdinalIgnoreCase))
				{
					this.GenerateAdmissionDiagnosticsInfo(xelement);
				}
				if (0 <= parameters.Argument.IndexOf("policy", StringComparison.OrdinalIgnoreCase))
				{
					this.GeneratePoliciesDiagnosticsInfo(xelement);
				}
			}
			return xelement;
		}

		ITaskProvider ITaskProviderManager.GetNextProvider()
		{
			ITaskProvider result;
			lock (this.instanceLock)
			{
				if (this.fairnessAssignments == null || this.fairnessAssignments.Length == 0)
				{
					result = null;
				}
				else
				{
					ClassificationBlock classificationBlock = this.fairnessAssignments[this.fairnessAssignmentCursor];
					this.fairnessAssignmentCursor++;
					if (this.fairnessAssignmentCursor >= this.fairnessAssignments.Length)
					{
						this.fairnessAssignmentCursor = 0;
					}
					classificationBlock.Activate();
					result = new TaskProvider(classificationBlock);
				}
			}
			return result;
		}

		int ITaskProviderManager.GetProviderCount()
		{
			int result;
			lock (this.instanceLock)
			{
				if (this.fairnessAssignments == null)
				{
					result = 0;
				}
				else
				{
					result = this.fairnessAssignments.Length;
				}
			}
			return result;
		}

		internal static void InitializeForTesting()
		{
			SystemWorkloadManager.InitializeForTesting(false);
		}

		internal static void InitializeForTesting(bool ignoreImplicitLocalCpuResource)
		{
			SystemWorkloadManager.InitializeForTesting(DummyWorkloadLogger.Instance, ignoreImplicitLocalCpuResource);
		}

		internal static void InitializeForTesting(IWorkloadLogger logger)
		{
			SystemWorkloadManager.InitializeForTesting(logger, false);
		}

		internal static void InitializeForTesting(IWorkloadLogger logger, bool ignoreImplicitLocalCpuResource)
		{
			if (SystemWorkloadManager.manager != null)
			{
				SystemWorkloadManager.Shutdown();
			}
			SystemWorkloadManager.InternalInitialize(logger, false, ignoreImplicitLocalCpuResource);
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				this.classificationUpdateTimer.Dispose();
				this.workloadExecution.Dispose();
				this.UnregisterDiagnosticInfo();
				this.resourceReservationContext.Dispose();
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<SystemWorkloadManager>(this);
		}

		private static void InternalInitialize(IWorkloadLogger logger, bool registerDiagnostics, bool ignoreImplicitLocalCpuResource)
		{
			if (logger == null)
			{
				throw new ArgumentNullException("logger");
			}
			if (SystemWorkloadManager.manager != null)
			{
				throw new InvalidOperationException("System workload manager is already initialized.");
			}
			SettingOverrideSync.Instance.Start(true);
			SystemWorkloadManager systemWorkloadManager = new SystemWorkloadManager(logger, ignoreImplicitLocalCpuResource);
			if (registerDiagnostics)
			{
				systemWorkloadManager.RegisterDiagnosticInfo();
			}
			SystemWorkloadManager.manager = systemWorkloadManager;
		}

		private static int ReadAppSettingAsInt(string keyName, int defaultValue)
		{
			string s = ConfigurationManager.AppSettings[keyName];
			int num;
			if (int.TryParse(s, out num) && num > 0 && num < 100)
			{
				return num;
			}
			return defaultValue;
		}

		private void GenerateWorkloadDiagnosticsInfo(XElement componentElement)
		{
			lock (this.instanceLock)
			{
				foreach (ClassificationBlock classificationBlock in this.classificationBlocks.Values)
				{
					SystemWorkloadBase[] workloads = classificationBlock.GetWorkloads();
					if (workloads != null)
					{
						XElement xelement = new XElement("Workloads");
						componentElement.Add(xelement);
						foreach (SystemWorkloadBase systemWorkloadBase in workloads)
						{
							XElement xelement2 = new XElement("Workload");
							xelement.Add(xelement2);
							xelement2.Add(new XElement("Identity", systemWorkloadBase.Id));
							xelement2.Add(new XElement("Type", systemWorkloadBase.WorkloadType));
							xelement2.Add(new XElement("Classification", systemWorkloadBase.Classification));
							xelement2.Add(new XElement("Registered", systemWorkloadBase.Registered));
							xelement2.Add(new XElement("Paused", systemWorkloadBase.Paused));
							xelement2.Add(new XElement("TaskCount", systemWorkloadBase.TaskCount));
							xelement2.Add(new XElement("BlockedTaskCount", systemWorkloadBase.BlockedTaskCount));
						}
					}
				}
			}
		}

		private void GenerateTaskDiagnosticsInfo(XElement componentElement)
		{
			XElement xelement = new XElement("Tasks");
			componentElement.Add(xelement);
			lock (this.instanceLock)
			{
				int num = 0;
				foreach (ClassificationBlock classificationBlock in this.classificationBlocks.Values)
				{
					SystemWorkloadBase[] workloads = classificationBlock.GetWorkloads();
					if (workloads != null)
					{
						foreach (SystemWorkloadBase systemWorkloadBase in workloads)
						{
							SystemTaskBase[] runningTasks = systemWorkloadBase.GetRunningTasks();
							if (runningTasks != null)
							{
								num += runningTasks.Length;
								foreach (SystemTaskBase systemTaskBase in runningTasks)
								{
									XElement xelement2 = new XElement("Task");
									xelement.Add(xelement2);
									xelement2.Add(new XElement("Identity", systemTaskBase.Identity));
									xelement2.Add(new XElement("WorkloadIdentity", systemWorkloadBase.Id));
									xelement2.Add(new XElement("WorkloadType", systemWorkloadBase.WorkloadType));
									xelement2.Add(new XElement("Classification", systemWorkloadBase.Classification));
									xelement2.Add(new XElement("CreationTime", systemTaskBase.CreationTime));
									if (systemTaskBase.ResourceReservation != null)
									{
										foreach (ResourceKey content in systemTaskBase.ResourceReservation.Resources)
										{
											xelement2.Add(new XElement("ResourceKey", content));
										}
									}
								}
							}
						}
					}
				}
			}
		}

		private void GenerateResourceDiagnosticsInfo(XElement componentElement)
		{
			XElement xelement = new XElement("Resources");
			componentElement.Add(xelement);
			lock (this.instanceLock)
			{
				foreach (ClassificationBlock classificationBlock in this.classificationBlocks.Values)
				{
					if (SystemWorkloadManager.IsClassificationActive(classificationBlock.WorkloadClassification))
					{
						foreach (ResourceKey resourceKey in ResourceHealthMonitorManager.Singleton.ResourceKeys)
						{
							XElement xelement2 = new XElement("Resource");
							xelement.Add(xelement2);
							xelement2.Add(new XElement("ResourceKey", resourceKey));
							xelement2.Add(new XElement("Classification", classificationBlock.WorkloadClassification));
							IResourceLoadMonitor resourceLoadMonitor = ResourceHealthMonitorManager.Singleton.Get(resourceKey);
							xelement2.Add(new XElement("Update", resourceLoadMonitor.LastUpdateUtc));
							ResourceLoad resourceLoad = resourceLoadMonitor.GetResourceLoad(classificationBlock.WorkloadClassification, false, null);
							xelement2.Add(new XElement("Load", resourceLoad));
							if (resourceLoad.Info != null)
							{
								xelement2.Add(new XElement("Info", resourceLoad.Info.ToString()));
							}
						}
					}
				}
			}
		}

		private void GenerateAdmissionDiagnosticsInfo(XElement componentElement)
		{
			XElement xelement = new XElement("Admissions");
			componentElement.Add(xelement);
			lock (this.instanceLock)
			{
				foreach (IResourceAdmissionControl resourceAdmissionControl in this.resourceReservationContext.AdmissionControlManager.Values)
				{
					DefaultAdmissionControl defaultAdmissionControl = (DefaultAdmissionControl)resourceAdmissionControl;
					XElement xelement2 = new XElement("Admission");
					xelement.Add(xelement2);
					xelement2.Add(new XElement("Resource", defaultAdmissionControl.ResourceKey));
					xelement2.Add(new XElement("IsAcquired", defaultAdmissionControl.IsAcquired));
					xelement2.Add(new XElement("LastRefresh", defaultAdmissionControl.LastRefreshUtc));
					xelement2.Add(new XElement("RefreshCycle", defaultAdmissionControl.RefreshCycle));
					xelement2.Add(new XElement("MaxConcurrency", defaultAdmissionControl.MaxConcurrency));
					foreach (object obj2 in Enum.GetValues(typeof(WorkloadClassification)))
					{
						WorkloadClassification workloadClassification = (WorkloadClassification)obj2;
						if (workloadClassification != WorkloadClassification.Unknown)
						{
							XElement xelement3 = new XElement(workloadClassification.ToString());
							xelement2.Add(xelement3);
							xelement3.Add(new XElement("ActiveConcurrency", defaultAdmissionControl.GetActiveConcurrency(workloadClassification)));
							double num = 0.0;
							xelement3.Add(new XElement("ConcurrencyLimit", defaultAdmissionControl.GetConcurrencyLimit(workloadClassification, out num)));
							xelement3.Add(new XElement("DelayFactor", num));
						}
					}
				}
			}
		}

		private void GenerateBlackBoxDiagnosticsInfo(XElement componentElement)
		{
			XElement xelement = new XElement("History");
			componentElement.Add(xelement);
			foreach (SystemWorkloadManagerLogEntry systemWorkloadManagerLogEntry in SystemWorkloadManagerBlackBox.GetRecords(false))
			{
				XElement xelement2 = new XElement("Entry");
				xelement.Add(xelement2);
				xelement2.Add(new XElement("Type", systemWorkloadManagerLogEntry.Type));
				xelement2.Add(new XElement("ResourceKey", systemWorkloadManagerLogEntry.Resource));
				xelement2.Add(new XElement("Classification", systemWorkloadManagerLogEntry.Classification));
				this.GenerateBlackBoxEventDiagnosticsInfo(xelement2, systemWorkloadManagerLogEntry.Type, systemWorkloadManagerLogEntry.CurrentEvent);
				if (systemWorkloadManagerLogEntry.PreviousEvent != null)
				{
					XElement xelement3 = new XElement("Previous");
					xelement2.Add(xelement3);
					this.GenerateBlackBoxEventDiagnosticsInfo(xelement3, systemWorkloadManagerLogEntry.Type, systemWorkloadManagerLogEntry.PreviousEvent);
				}
			}
		}

		private void GenerateBlackBoxEventDiagnosticsInfo(XElement parent, SystemWorkloadManagerLogEntryType entryType, SystemWorkloadManagerEvent blackBoxEvent)
		{
			parent.Add(new XElement("DateTime", blackBoxEvent.DateTime));
			parent.Add(new XElement("Load", blackBoxEvent.Load));
			if (blackBoxEvent.Load.Info != null)
			{
				parent.Add(new XElement("Info", blackBoxEvent.Load.Info.ToString()));
			}
			if (entryType == SystemWorkloadManagerLogEntryType.Admission)
			{
				parent.Add(new XElement("ConcurrencyLimit", blackBoxEvent.Slots));
				parent.Add(new XElement("Delayed", blackBoxEvent.Delayed));
			}
		}

		private void GeneratePoliciesDiagnosticsInfo(XElement componentElement)
		{
			VariantConfigurationSnapshot snapshot = VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null);
			XElement xelement = new XElement("Policy");
			componentElement.Add(xelement);
			foreach (object obj in Enum.GetValues(typeof(ResourceMetricType)))
			{
				ResourceMetricType resourceMetricType = (ResourceMetricType)obj;
				if (resourceMetricType != ResourceMetricType.None && resourceMetricType != ResourceMetricType.Remote)
				{
					XElement xelement2 = new XElement("ResourcePolicy");
					xelement2.Add(new XElement("MetricType", resourceMetricType));
					xelement2.Add(new XElement("MaxConcurrency", snapshot.WorkloadManagement.GetObject<IResourceSettings>(resourceMetricType, new object[0]).MaxConcurrency));
					foreach (object obj2 in Enum.GetValues(typeof(WorkloadClassification)))
					{
						WorkloadClassification workloadClassification = (WorkloadClassification)obj2;
						if (workloadClassification != WorkloadClassification.Unknown)
						{
							ResourceMetricPolicy resourceMetricPolicy = new ResourceMetricPolicy(resourceMetricType, workloadClassification, snapshot);
							xelement2.Add(resourceMetricPolicy.GetDiagnosticInfo());
						}
					}
					xelement.Add(xelement2);
				}
			}
			foreach (object obj3 in Enum.GetValues(typeof(WorkloadType)))
			{
				WorkloadType workloadType = (WorkloadType)obj3;
				if (workloadType != WorkloadType.Unknown)
				{
					WorkloadPolicy workloadPolicy = new WorkloadPolicy(workloadType, snapshot);
					xelement.Add(workloadPolicy.GetDiagnosticInfo());
				}
			}
			ISystemWorkloadManagerSettings systemWorkloadManager = snapshot.WorkloadManagement.SystemWorkloadManager;
			XElement xelement3 = new XElement("WorkloadManagerPolicy");
			xelement3.Add(new XElement("MaxConcurrency", systemWorkloadManager.MaxConcurrency));
			xelement3.Add(new XElement("RefreshCycle", systemWorkloadManager.RefreshCycle));
			xelement.Add(xelement3);
		}

		private void InternalRegisterWorkload(SystemWorkloadBase workload)
		{
			if (workload == null)
			{
				throw new ArgumentNullException("workload");
			}
			WorkloadClassification classification = workload.Classification;
			if (classification == WorkloadClassification.Unknown)
			{
				throw new ArgumentException("Workload classification cannot be Unknown.", "workload");
			}
			if (workload.Registered)
			{
				throw new InvalidOperationException(string.Format("Workload {0} is already registered", workload.Id));
			}
			int num = 0;
			ClassificationBlock classificationBlock = null;
			try
			{
				classificationBlock = this.classificationBlocks[classification];
			}
			catch (KeyNotFoundException innerException)
			{
				throw new InvalidOperationException(string.Format("Classification {0} was not found in classification block dictionary.", classification), innerException);
			}
			lock (this.instanceLock)
			{
				workload.SetResourceReservationContext(this.resourceReservationContext);
				classificationBlock.AddWorkload(workload);
				num = this.GetWorkloadCount();
				this.factorDenominator += classificationBlock.FairnessFactor;
				this.RecalculateFairnessMap();
				if (this.workloadExecution.Status != WorkloadExecutionStatus.Started)
				{
					this.workloadExecution.Start();
				}
			}
			this.perfCounters.UpdateWorkloadCount((long)num);
		}

		private bool InternalUnregisterWorkload(SystemWorkloadBase workload)
		{
			if (workload == null)
			{
				throw new ArgumentNullException("workload");
			}
			int num = 0;
			bool flag = false;
			lock (this.instanceLock)
			{
				if (!workload.Registered)
				{
					return false;
				}
				ClassificationBlock classificationBlock = workload.ClassificationBlock;
				flag = classificationBlock.RemoveWorkload(workload);
				workload.SetResourceReservationContext(null);
				if (flag)
				{
					num = this.GetWorkloadCount();
					this.factorDenominator -= classificationBlock.FairnessFactor;
					this.RecalculateFairnessMap();
				}
				if (this.factorDenominator == 0)
				{
					this.workloadExecution.Stop();
				}
			}
			if (flag)
			{
				this.perfCounters.UpdateWorkloadCount((long)num);
			}
			return flag;
		}

		private void RegisterDiagnosticInfo()
		{
			if (!this.registered)
			{
				ProcessAccessManager.RegisterComponent(this);
				this.registered = true;
			}
		}

		private void UnregisterDiagnosticInfo()
		{
			if (this.registered)
			{
				ProcessAccessManager.UnregisterComponent(this);
				this.registered = false;
			}
		}

		private void ClassificationUpdate(object state)
		{
			VariantConfigurationSnapshot snapshot = VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null);
			foreach (ClassificationBlock classificationBlock in this.classificationBlocks.Values)
			{
				bool flag = false;
				do
				{
					flag = false;
					lock (this.instanceLock)
					{
						SystemWorkloadBase[] workloads = classificationBlock.GetWorkloads();
						if (workloads != null)
						{
							foreach (SystemWorkloadBase systemWorkloadBase in workloads)
							{
								WorkloadClassification classification = snapshot.WorkloadManagement.GetObject<IWorkloadSettings>(systemWorkloadBase.WorkloadType, new object[0]).Classification;
								if (classification != classificationBlock.WorkloadClassification)
								{
									if (classificationBlock.RemoveWorkload(systemWorkloadBase))
									{
										this.factorDenominator -= classificationBlock.FairnessFactor;
										ClassificationBlock classificationBlock2 = this.classificationBlocks[classification];
										classificationBlock2.AddWorkload(systemWorkloadBase);
										this.factorDenominator += classificationBlock2.FairnessFactor;
										this.RecalculateFairnessMap();
									}
									flag = true;
									break;
								}
							}
						}
					}
				}
				while (flag);
			}
		}

		private void RecalculateFairnessMap()
		{
			int num = 0;
			lock (this.instanceLock)
			{
				this.fairnessAssignmentCursor = 0;
				if (this.factorDenominator == 0)
				{
					this.fairnessAssignments = null;
					return;
				}
				this.fairnessAssignments = new ClassificationBlock[this.factorDenominator];
				int num2 = 0;
				foreach (ClassificationBlock classificationBlock in this.classificationBlocks.Values)
				{
					if (classificationBlock.WorkloadCount > 0)
					{
						num++;
						SystemWorkloadManagerBlackBox.AddActiveClassification(classificationBlock.WorkloadClassification);
					}
					for (int i = 0; i < classificationBlock.FairnessFactor * classificationBlock.WorkloadCount; i++)
					{
						this.fairnessAssignments[num2] = classificationBlock;
						num2++;
					}
				}
			}
			this.perfCounters.UpdateActiveClassifications((long)num);
		}

		private int GetWorkloadCount()
		{
			int num = 0;
			lock (this.instanceLock)
			{
				foreach (ClassificationBlock classificationBlock in this.classificationBlocks.Values)
				{
					num += classificationBlock.WorkloadCount;
				}
			}
			return num;
		}

		private int GetDefaultFactor(WorkloadClassification classification)
		{
			switch (classification)
			{
			case WorkloadClassification.Unknown:
				return 0;
			case WorkloadClassification.Discretionary:
				return 1;
			case WorkloadClassification.InternalMaintenance:
				return 2;
			case WorkloadClassification.CustomerExpectation:
				return 4;
			case WorkloadClassification.Urgent:
				return 8;
			default:
				throw new ArgumentException("Unexpected classification: " + classification, "classification");
			}
		}

		private const int DefaultUrgentFactor = 8;

		private const int DefaultCustomerExpectationFactor = 4;

		private const int DefaultInternalMaintenanceFactor = 2;

		private const int DefaultDiscretionaryFactor = 1;

		private const string ProcessAccessManagerComponentName = "SystemWorkloadManager";

		private static SystemWorkloadManager manager;

		private ResourceReservationContext resourceReservationContext;

		private ClassificationDictionary<ClassificationBlock> classificationBlocks;

		private ClassificationBlock[] fairnessAssignments;

		private int fairnessAssignmentCursor;

		private object instanceLock = new object();

		private WorkloadExecution workloadExecution;

		private int factorDenominator;

		private WorkloadManagementLogger logger;

		private bool registered;

		private WorkloadManagementPerfCounterWrapper perfCounters;

		private Timer classificationUpdateTimer;
	}
}
