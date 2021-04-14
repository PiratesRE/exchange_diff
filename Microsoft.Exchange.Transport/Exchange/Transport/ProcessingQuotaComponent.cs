using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Security;
using System.Threading;
using System.Xml.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Reporting;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Transport;
using Microsoft.Exchange.Transport.Common;

namespace Microsoft.Exchange.Transport
{
	internal class ProcessingQuotaComponent : IProcessingQuotaComponent, ITransportComponent, IDiagnosable
	{
		private PerformanceCounter ProcessorTimeCounter
		{
			get
			{
				if (this.processorTimeCounter == null)
				{
					this.processorTimeCounter = new PerformanceCounter("Process", "% Processor Time", "EdgeTransport");
				}
				return this.processorTimeCounter;
			}
		}

		private Type SessionType
		{
			get
			{
				if (this.sessionType == null)
				{
					Assembly assembly = Assembly.Load("Microsoft.Exchange.Hygiene.Data");
					this.sessionType = assembly.GetType("Microsoft.Exchange.Hygiene.Data.Reporting.ReportingSession");
				}
				return this.sessionType;
			}
		}

		private ITenantThrottlingSession TenantSession
		{
			get
			{
				if (this.tenantSession == null)
				{
					this.tenantSession = (ITenantThrottlingSession)Activator.CreateInstance(this.SessionType);
				}
				return this.tenantSession;
			}
		}

		public ProcessingQuotaComponent.ProcessingData GetQuotaOverride(Guid externalOrgId)
		{
			ProcessingQuotaComponent.ProcessingData processingData;
			if (this.tenantOverrides.TryGetValue(externalOrgId, out processingData) && (processingData.IsAdmin || this.cpuTriggered))
			{
				return processingData;
			}
			return null;
		}

		public ProcessingQuotaComponent.ProcessingData GetQuotaOverride(WaitCondition condition)
		{
			TenantBasedCondition tenantBasedCondition = condition as TenantBasedCondition;
			if (tenantBasedCondition != null)
			{
				return this.GetQuotaOverride(tenantBasedCondition.TenantId);
			}
			return null;
		}

		public void SetLoadTimeDependencies(TransportAppConfig.IProcessingQuotaConfig processingQuota)
		{
			this.config = processingQuota;
			this.enabled = (this.config.EnforceProcessingQuota || this.config.TestProcessingQuota);
			this.lastUpdateTime = DateTime.UtcNow - this.config.UpdateThrottlingDataInterval;
		}

		public void TimedUpdate()
		{
			if (!this.enabled)
			{
				return;
			}
			bool flag = this.processorTimeCounter != null;
			this.currentProcessorTimeValue = this.ProcessorTimeCounter.NextValue() / (float)Environment.ProcessorCount;
			if (flag && (this.currentProcessorTimeValue >= (float)this.config.HighWatermarkCpuPercentage || (this.cpuTriggered && this.currentProcessorTimeValue >= (float)this.config.LowWatermarkCpuPercentage)))
			{
				this.cpuTriggered = true;
			}
			else
			{
				this.cpuTriggered = false;
			}
			if (this.lastUpdateTime.Add(this.config.UpdateThrottlingDataInterval) < DateTime.UtcNow && Interlocked.CompareExchange(ref this.updateInProgress, 1, 0) == 0)
			{
				ThreadPool.QueueUserWorkItem(new WaitCallback(this.FetchTenantOverrides));
			}
		}

		private void FetchTenantOverrides(object state)
		{
			try
			{
				if (!string.IsNullOrEmpty(this.config.ThrottleDataFilePath))
				{
					this.ReadTestOverrideData();
				}
				else
				{
					try
					{
						IEnumerable<TenantThrottleInfo> tenantThrottlingDigest = this.TenantSession.GetTenantThrottlingDigest(0, null, false, 5000, true);
						this.ParseTenantOverrideData(tenantThrottlingDigest);
					}
					catch (TransientException ex)
					{
						ExTraceGlobals.GeneralTracer.TraceError<TransientException>((long)this.GetHashCode(), "Transient exception when fetching override data: {0}", ex);
						ProcessingQuotaComponent.logger.LogEvent(TransportEventLogConstants.Tuple_CategorizerErrorRetrievingTenantOverride, this.GetDiagnosticComponentName(), new object[]
						{
							ex
						});
					}
					catch (DataSourceOperationException ex2)
					{
						ExTraceGlobals.GeneralTracer.TraceError<DataSourceOperationException>((long)this.GetHashCode(), "Permanent exception when fetching override data: {0}", ex2);
						ProcessingQuotaComponent.logger.LogEvent(TransportEventLogConstants.Tuple_CategorizerErrorRetrievingTenantOverride, this.GetDiagnosticComponentName(), new object[]
						{
							ex2
						});
					}
					catch (DataValidationException ex3)
					{
						ExTraceGlobals.GeneralTracer.TraceError<DataValidationException>((long)this.GetHashCode(), "Data exception when fetching override data: {0}", ex3);
						ProcessingQuotaComponent.logger.LogEvent(TransportEventLogConstants.Tuple_CategorizerErrorRetrievingTenantOverride, this.GetDiagnosticComponentName(), new object[]
						{
							ex3
						});
					}
				}
			}
			finally
			{
				Interlocked.CompareExchange(ref this.updateInProgress, 0, 1);
			}
		}

		private void ParseTenantOverrideData(IEnumerable<TenantThrottleInfo> tenants)
		{
			if (tenants == null)
			{
				return;
			}
			Dictionary<Guid, ProcessingQuotaComponent.ProcessingData> currentEntries = new Dictionary<Guid, ProcessingQuotaComponent.ProcessingData>();
			foreach (TenantThrottleInfo tenantThrottleInfo in tenants)
			{
				ProcessingQuotaComponent.ProcessingData processingData = new ProcessingQuotaComponent.ProcessingData();
				switch (tenantThrottleInfo.ThrottleState)
				{
				case TenantThrottleState.Auto:
					if (tenantThrottleInfo.ThrottlingFactor < 0.0 || tenantThrottleInfo.ThrottlingFactor > 1.0)
					{
						ExTraceGlobals.GeneralTracer.TraceError<double, Guid>((long)this.GetHashCode(), "ThrottlingFactor of {0} for tenant {1} is outside allowed range [0,1]", tenantThrottleInfo.ThrottlingFactor, tenantThrottleInfo.TenantId);
						continue;
					}
					processingData.ThrottlingFactor = tenantThrottleInfo.ThrottlingFactor;
					break;
				case TenantThrottleState.Throttled:
					processingData.ThrottlingFactor = 0.0;
					processingData.IsAdmin = true;
					break;
				case TenantThrottleState.Unthrottled:
					processingData.ThrottlingFactor = 1.0;
					processingData.IsAdmin = true;
					break;
				default:
					ExTraceGlobals.GeneralTracer.TraceError<TenantThrottleState, Guid>((long)this.GetHashCode(), "ThrottleState of {0} for tenant {1} unrecognized", tenantThrottleInfo.ThrottleState, tenantThrottleInfo.TenantId);
					continue;
				}
				TransportHelpers.AttemptAddToDictionary<Guid, ProcessingQuotaComponent.ProcessingData>(currentEntries, tenantThrottleInfo.TenantId, processingData, null);
			}
			this.tenantOverrides = currentEntries;
			this.lastUpdateTime = DateTime.UtcNow;
		}

		private void ReadTestOverrideData()
		{
			FileStream fileStream = null;
			try
			{
				fileStream = new FileStream(this.config.ThrottleDataFilePath, FileMode.Open, FileAccess.Read);
				StreamReader streamReader = new StreamReader(fileStream);
				Dictionary<Guid, ProcessingQuotaComponent.ProcessingData> currentEntries = new Dictionary<Guid, ProcessingQuotaComponent.ProcessingData>();
				while (!streamReader.EndOfStream)
				{
					string[] array = streamReader.ReadLine().Split(new char[]
					{
						','
					});
					Guid keyToAdd;
					double num;
					if (array.Length != 2)
					{
						ExTraceGlobals.GeneralTracer.TraceError<string>((long)this.GetHashCode(), "skipping malformed line {0}", string.Join("", array));
					}
					else if (!Guid.TryParse(array[0], out keyToAdd))
					{
						ExTraceGlobals.GeneralTracer.TraceError<string>((long)this.GetHashCode(), "skipping malformed tenant {0}", array[0]);
					}
					else if (!double.TryParse(array[1], out num) || num < 0.0 || num > 1.0)
					{
						ExTraceGlobals.GeneralTracer.TraceError<string, string>((long)this.GetHashCode(), "skipping tenant {0} because of malformed throttling factor {1} ", array[0], array[1]);
					}
					else
					{
						ProcessingQuotaComponent.ProcessingData processingData = new ProcessingQuotaComponent.ProcessingData();
						processingData.ThrottlingFactor = num;
						processingData.IsAdmin = (processingData.ThrottlingFactor == 1.0 || processingData.ThrottlingFactor == 0.0);
						TransportHelpers.AttemptAddToDictionary<Guid, ProcessingQuotaComponent.ProcessingData>(currentEntries, keyToAdd, processingData, null);
					}
				}
				this.tenantOverrides = currentEntries;
				this.lastUpdateTime = DateTime.UtcNow;
			}
			catch (IOException arg)
			{
				ExTraceGlobals.GeneralTracer.TraceError<IOException>((long)this.GetHashCode(), "ioexception when reading file {0}", arg);
			}
			catch (SecurityException arg2)
			{
				ExTraceGlobals.GeneralTracer.TraceError<SecurityException>((long)this.GetHashCode(), "securityException when reading file {0}", arg2);
			}
			finally
			{
				if (fileStream != null)
				{
					fileStream.Close();
				}
			}
		}

		public string GetDiagnosticComponentName()
		{
			return "ProcessingQuota";
		}

		public XElement GetDiagnosticInfo(DiagnosableParameters parameters)
		{
			XElement xelement = new XElement(this.GetDiagnosticComponentName());
			bool flag = parameters.Argument.IndexOf("verbose", StringComparison.OrdinalIgnoreCase) != -1;
			bool flag2 = parameters.Argument.IndexOf("config", StringComparison.OrdinalIgnoreCase) != -1;
			bool flag3 = parameters.Argument.IndexOf("help", StringComparison.OrdinalIgnoreCase) != -1;
			if (flag2)
			{
				xelement.Add(TransportAppConfig.GetDiagnosticInfoForType(this.config));
			}
			xelement.Add(new XElement("LastProcessorMeasurement", this.currentProcessorTimeValue));
			xelement.Add(new XElement("LastLoadTime", this.lastUpdateTime));
			xelement.Add(new XElement("ThrottlingTriggered", this.cpuTriggered));
			xelement.Add(new XElement("OverrideCount", this.tenantOverrides.Count));
			if (flag)
			{
				Dictionary<Guid, ProcessingQuotaComponent.ProcessingData> dictionary = this.tenantOverrides;
				XElement xelement2 = new XElement("tenantOverrides");
				foreach (KeyValuePair<Guid, ProcessingQuotaComponent.ProcessingData> keyValuePair in dictionary)
				{
					XElement xelement3 = new XElement("ProcessingOverride");
					xelement3.SetAttributeValue("Tenant", keyValuePair.Key);
					xelement3.SetAttributeValue("ThrottleFactor", keyValuePair.Value.ThrottlingFactor);
					xelement2.Add(xelement3);
				}
				xelement.Add(xelement2);
			}
			if (flag3)
			{
				xelement.Add(new XElement("help", "Supported arguments: verbose, config, help"));
			}
			return xelement;
		}

		public void Load()
		{
			this.TimedUpdate();
		}

		public void Unload()
		{
		}

		public string OnUnhandledException(Exception e)
		{
			return null;
		}

		private const string DiagnosticsComponentName = "ProcessingQuota";

		private static ExEventLog logger = new ExEventLog(ExTraceGlobals.GeneralTracer.Category, TransportEventLog.GetEventSource());

		private TransportAppConfig.IProcessingQuotaConfig config;

		private bool enabled;

		private DateTime lastUpdateTime;

		private int updateInProgress;

		private Dictionary<Guid, ProcessingQuotaComponent.ProcessingData> tenantOverrides = new Dictionary<Guid, ProcessingQuotaComponent.ProcessingData>();

		private Type sessionType;

		private ITenantThrottlingSession tenantSession;

		private PerformanceCounter processorTimeCounter;

		private bool cpuTriggered;

		private float currentProcessorTimeValue;

		internal class ProcessingData
		{
			public ProcessingData()
			{
			}

			public ProcessingData(double throttleFactor)
			{
				this.throttlingFactor = throttleFactor;
			}

			public double ThrottlingFactor
			{
				get
				{
					return this.throttlingFactor;
				}
				set
				{
					this.throttlingFactor = value;
				}
			}

			public bool IsAdmin
			{
				get
				{
					return this.isAdmin;
				}
				set
				{
					this.isAdmin = value;
				}
			}

			public bool IsAllowListed
			{
				get
				{
					return this.throttlingFactor == 1.0;
				}
			}

			public bool IsBlocked
			{
				get
				{
					return this.throttlingFactor == 0.0;
				}
			}

			internal const int AllowThrottlingFactor = 1;

			internal const int BlockThrottlingFactor = 0;

			private double throttlingFactor;

			private bool isAdmin;
		}
	}
}
