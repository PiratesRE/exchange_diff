using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;
using Microsoft.Win32;

namespace Microsoft.Office.Datacenter.ActiveMonitoring
{
	public abstract class EscalateResponderBase : ResponderWorkItem
	{
		protected NotificationServiceClass? EscalationNotificationType
		{
			get
			{
				return this.escalationNotificationType;
			}
			set
			{
				this.escalationNotificationType = value;
			}
		}

		protected string CustomEscalationSubject
		{
			get
			{
				return this.customEscalationSubject;
			}
			set
			{
				this.customEscalationSubject = value;
			}
		}

		protected string CustomEscalationMessage
		{
			get
			{
				return this.customEscalationMessage;
			}
			set
			{
				this.customEscalationMessage = value;
			}
		}

		public EscalateResponderBase()
		{
		}

		internal static string DefaultEscalationSubject
		{
			get
			{
				return EscalateResponderBase.DefaultEscalationSubjectInternal;
			}
			set
			{
				EscalateResponderBase.DefaultEscalationSubjectInternal = value;
			}
		}

		internal static string DefaultEscalationMessage
		{
			get
			{
				return EscalateResponderBase.DefaultEscalationMessageInternal;
			}
			set
			{
				EscalateResponderBase.DefaultEscalationMessageInternal = value;
			}
		}

		internal static string HealthSetEscalationSubjectPrefix
		{
			get
			{
				return EscalateResponderBase.HealthSetEscalationSubjectPrefixInternal;
			}
			set
			{
				EscalateResponderBase.HealthSetEscalationSubjectPrefixInternal = value;
			}
		}

		internal static string HealthSetMaintenanceEscalationSubjectPrefix
		{
			get
			{
				return EscalateResponderBase.HealthSetMaintenanceEscalationSubjectPrefixInternal;
			}
			set
			{
				EscalateResponderBase.HealthSetMaintenanceEscalationSubjectPrefixInternal = value;
			}
		}

		internal static HealthSetEscalationHelper EscalationHelper
		{
			get
			{
				return EscalateResponderBase.EscalationHelperInternal;
			}
			set
			{
				EscalateResponderBase.EscalationHelperInternal = value;
			}
		}

		protected static string LoadFromResourceAttributeValue
		{
			get
			{
				return "LoadFromResourceAttributeValue";
			}
		}

		protected CancellationToken LocalCancellationToken
		{
			get
			{
				return this.localCancellationToken;
			}
		}

		protected RemotePowerShell RemotePowershell
		{
			get
			{
				return this.remotePowerShell;
			}
		}

		protected virtual bool IncludeHealthSetEscalationInfo
		{
			get
			{
				return true;
			}
		}

		protected static bool IsOBDGallatinMachine
		{
			get
			{
				bool result = false;
				using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\ExchangeLabs", false))
				{
					if (registryKey != null)
					{
						object value = registryKey.GetValue("IsOBDGallatinMachine", null);
						if (value != null)
						{
							result = ((int)value == 1);
						}
					}
				}
				return result;
			}
		}

		private static string LocalMachineVersion
		{
			get
			{
				if (EscalateResponderBase.localMachineVersion == null)
				{
					try
					{
						AssemblyFileVersionAttribute assemblyFileVersionAttribute = (AssemblyFileVersionAttribute)Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyFileVersionAttribute), true).Single<object>();
						EscalateResponderBase.localMachineVersion = string.Format("{0}-{1}", assemblyFileVersionAttribute.Version, Environment.OSVersion.Version.ToString());
					}
					catch (InvalidOperationException)
					{
						EscalateResponderBase.localMachineVersion = string.Empty;
					}
				}
				return EscalateResponderBase.localMachineVersion;
			}
		}

		private static bool IsFfoMachine
		{
			get
			{
				return DatacenterRegistry.IsForefrontForOffice() || EscalateResponderBase.IsFfoCentralAdminRoleInstalled;
			}
		}

		private static bool IsFfoCentralAdminRoleInstalled
		{
			get
			{
				RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\CentralAdminRole");
				if (registryKey != null)
				{
					registryKey.Close();
					registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\PowerShell\\1\\PowerShellSnapIns\\Microsoft.Exchange.Management.Powershell.FfoCentralAdmin");
					if (registryKey != null)
					{
						registryKey.Close();
						return true;
					}
				}
				return false;
			}
		}

		public static void SetScopeInformation(ResponderDefinition definition, string location, string forest, string dag, string site, string region)
		{
			EscalateResponderBase.SetScopeInformation(definition, location, forest, dag, site, region, null, null);
		}

		public static void SetScopeInformation(ResponderDefinition definition, string location, string forest, string dag, string site, string region, string capacityUnit)
		{
			EscalateResponderBase.SetScopeInformation(definition, location, forest, dag, site, region, capacityUnit, null);
		}

		public static void SetScopeInformation(ResponderDefinition definition, string location, string forest, string dag, string site, string region, string capacityUnit, string rack)
		{
			if (definition == null)
			{
				throw new ArgumentNullException("definition");
			}
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("ScopeLocation", location);
			dictionary.Add("ScopeForest", forest);
			dictionary.Add("ScopeDag", dag);
			dictionary.Add("ScopeSite", site);
			dictionary.Add("ScopeRegion", region);
			if (!string.IsNullOrEmpty(capacityUnit))
			{
				dictionary.Add("ScopeCapacityUnit", capacityUnit);
			}
			if (!string.IsNullOrEmpty(rack))
			{
				dictionary.Add("ScopeRack", rack);
			}
			EscalateResponderBase.AddExtensionAttributes(definition, dictionary);
		}

		public static void AddExtensionAttributes(ResponderDefinition definition, Dictionary<string, string> attributeValueByName)
		{
			XmlDocument xmlDocument = new XmlDocument();
			if (!string.IsNullOrWhiteSpace(definition.ExtensionAttributes))
			{
				try
				{
					xmlDocument.LoadXml(definition.ExtensionAttributes);
				}
				catch (XmlException)
				{
					throw new Exception("EscalateResponderBase.SetServiceAndAlertSource: Existing ExtensionAttributes contains invalid XML.");
				}
				if (xmlDocument.DocumentElement.Name != "ExtensionAttributes")
				{
					throw new Exception("EscalateResponderBase.SetServiceAndAlertSource: Existing ExtensionAttributes has invalid schema.");
				}
			}
			else
			{
				xmlDocument.LoadXml("<ExtensionAttributes />");
			}
			foreach (KeyValuePair<string, string> keyValuePair in attributeValueByName)
			{
				EscalateResponderBase.SetExtensionAttribute(xmlDocument, keyValuePair.Key, keyValuePair.Value);
			}
			definition.ExtensionAttributes = xmlDocument.OuterXml;
		}

		internal virtual EscalationState GetEscalationState(bool? isHealthy, CancellationToken cancellationToken)
		{
			if (isHealthy == null)
			{
				return EscalationState.Unknown;
			}
			if (isHealthy.Value)
			{
				return EscalationState.Green;
			}
			if (this.GetEscalationEnvironment() == EscalationEnvironment.OnPrem)
			{
				return EscalationState.Red;
			}
			NotificationServiceClass notificationServiceClass = this.escalationNotificationType ?? base.Definition.NotificationServiceClass;
			switch (notificationServiceClass)
			{
			case NotificationServiceClass.Urgent:
				return EscalationState.Red;
			case NotificationServiceClass.UrgentInTraining:
				return EscalationState.Yellow;
			case NotificationServiceClass.Scheduled:
			{
				DailySchedulePattern dailySchedulePattern = new DailySchedulePattern(base.Definition.DailySchedulePattern);
				DateTime dateTime = TimeZoneInfo.ConvertTime(DateTime.UtcNow, dailySchedulePattern.TimeZoneInfo);
				if (dailySchedulePattern.ScheduledDays.Contains(dateTime.DayOfWeek) && dailySchedulePattern.StartTime.TimeOfDay <= dateTime.TimeOfDay && dateTime.TimeOfDay <= dailySchedulePattern.EndTime.TimeOfDay)
				{
					return EscalationState.Orange;
				}
				return EscalationState.DarkYellow;
			}
			default:
				throw new NotSupportedException(string.Format("NotificationServiceClass value not supported: {0}", notificationServiceClass.ToString()));
			}
		}

		internal virtual void BeforeContentGeneration(ResponseMessageReader propertyReader)
		{
		}

		internal virtual void GetEscalationSubjectAndMessage(MonitorResult monitorResult, out string escalationSubject, out string escalationMessage, bool rethrow = false, Action<ResponseMessageReader> textGeneratorModifier = null)
		{
			ResponseMessageReader responseMessageReader = new ResponseMessageReader();
			responseMessageReader.AddObject<string>("0", monitorResult.Component.Name);
			responseMessageReader.AddObjectResolver<string>("1", delegate
			{
				if (this.GetEscalationEnvironment() == EscalationEnvironment.OutsideIn)
				{
					return string.Format("{0}/{1}/{2}", base.Definition.TargetPartition, base.Definition.TargetGroup, base.Definition.TargetResource);
				}
				return base.Definition.TargetResource;
			});
			responseMessageReader.AddObject<DateTime?>("2", monitorResult.FirstAlertObservedTime);
			responseMessageReader.AddObject<double>("3", monitorResult.TotalValue);
			responseMessageReader.AddObject<MonitorResult>("Monitor", monitorResult);
			responseMessageReader.AddObject<ProbeResult>("Probe", this.GetLastFailedProbeResult(monitorResult));
			try
			{
				MonitorDefinition result = base.Broker.GetMonitorDefinition(monitorResult.WorkItemId).ExecuteAsync(this.LocalCancellationToken, base.TraceContext).Result;
				if (result != null)
				{
					responseMessageReader.AddObject<MonitorDefinition>("MonitorDefinition", result);
				}
			}
			catch (Exception ex)
			{
				WTFDiagnostics.TraceError<string>(ExTraceGlobals.CommonComponentsTracer, base.TraceContext, "Failed to get MonitorDefinition to use as a replacement object: {0}", ex.ToString(), null, "GetEscalationSubjectAndMessage", "f:\\15.00.1497\\sources\\dev\\common\\src\\WorkerTaskFramework\\ActiveMonitoring\\WorkItems\\Responders\\EscalateResponderBase.cs", 659);
			}
			this.BeforeContentGeneration(responseMessageReader);
			if (textGeneratorModifier != null)
			{
				textGeneratorModifier(responseMessageReader);
			}
			try
			{
				escalationSubject = responseMessageReader.ReplaceValues((!string.IsNullOrWhiteSpace(this.customEscalationSubject)) ? this.customEscalationSubject : base.Definition.EscalationSubject);
			}
			catch (Exception ex2)
			{
				WTFDiagnostics.TraceError<string>(ExTraceGlobals.CommonComponentsTracer, base.TraceContext, "Failed to replace user-defined escalation subject: {0}", ex2.ToString(), null, "GetEscalationSubjectAndMessage", "f:\\15.00.1497\\sources\\dev\\common\\src\\WorkerTaskFramework\\ActiveMonitoring\\WorkItems\\Responders\\EscalateResponderBase.cs", 675);
				base.Result.StateAttribute4 = "Subject: " + ex2.ToString();
				if (rethrow)
				{
					throw;
				}
				escalationSubject = null;
			}
			if (string.IsNullOrWhiteSpace(escalationSubject))
			{
				escalationSubject = responseMessageReader.ReplaceValues(EscalateResponderBase.DefaultEscalationSubject);
			}
			string text = (!string.IsNullOrWhiteSpace(this.customEscalationMessage)) ? this.customEscalationMessage : base.Definition.EscalationMessage;
			if (this.ReadAttribute(EscalateResponderBase.LoadFromResourceAttributeValue, false))
			{
				text = EscalateResponderBase.GetNonLocalizableResourceAsString(base.GetType().Assembly, text);
			}
			try
			{
				escalationMessage = responseMessageReader.ReplaceValues(text);
			}
			catch (Exception ex3)
			{
				WTFDiagnostics.TraceError<string>(ExTraceGlobals.CommonComponentsTracer, base.TraceContext, "Failed to replace user-defined escalation message: {0}", ex3.ToString(), null, "GetEscalationSubjectAndMessage", "f:\\15.00.1497\\sources\\dev\\common\\src\\WorkerTaskFramework\\ActiveMonitoring\\WorkItems\\Responders\\EscalateResponderBase.cs", 707);
				if (string.IsNullOrEmpty(base.Result.StateAttribute4))
				{
					base.Result.StateAttribute4 = ex3.ToString();
				}
				else
				{
					ResponderResult result2 = base.Result;
					result2.StateAttribute4 = result2.StateAttribute4 + "Body: " + ex3.ToString();
				}
				if (rethrow)
				{
					throw;
				}
				escalationMessage = null;
			}
			if (string.IsNullOrWhiteSpace(escalationMessage))
			{
				escalationMessage = responseMessageReader.ReplaceValues(EscalateResponderBase.DefaultEscalationMessage);
			}
		}

		protected void SetServiceAndTeam(string escalationService, string escalationTeam)
		{
			if (string.IsNullOrEmpty(escalationService))
			{
				throw new ArgumentException("You must specify a service. Use the Get-OnCallService cmdlet in the Smart Alert system for a current list of valid service names.");
			}
			if (string.IsNullOrEmpty(escalationTeam))
			{
				throw new ArgumentException("You must specify a team that belongs to the service. Use the Get-OnCallTeam cmdlet in the Smart Alert system for a current list of valid team names.");
			}
			this.escalationService = escalationService;
			this.escalationTeam = escalationTeam;
		}

		protected internal static void SetActiveMonitoringCertificateSettings(ResponderDefinition definition)
		{
			using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(EscalateResponderBase.ActiveMonitoringRegistryPath, false))
			{
				if (registryKey != null)
				{
					string text;
					if (string.IsNullOrWhiteSpace(definition.Account) && (text = (string)registryKey.GetValue("RPSCertificateSubject", null)) != null)
					{
						definition.Account = text;
					}
					if (string.IsNullOrWhiteSpace(definition.Endpoint) && (text = (string)registryKey.GetValue("RPSEndpoint", null)) != null)
					{
						definition.Endpoint = text;
					}
				}
			}
		}

		protected override void DoResponderWork(CancellationToken cancellationToken)
		{
			this.localCancellationToken = cancellationToken;
			Task<ResponderResult> lastSuccessfulRecoveryAttemptedResponderResult = this.GetLastSuccessfulRecoveryAttemptedResponderResult(cancellationToken);
			lastSuccessfulRecoveryAttemptedResponderResult.Continue(delegate(ResponderResult lastResponderResult)
			{
				this.Result.ResponseAction = EscalateResponderBase.ResponseAction.Defer.ToString();
				DateTime startTime = SqlDateTime.MinValue.Value;
				if (lastResponderResult != null)
				{
					startTime = lastResponderResult.ExecutionStartTime;
					this.Result.StateAttribute1 = lastResponderResult.StateAttribute1;
					this.Result.StateAttribute2 = lastResponderResult.StateAttribute2;
					this.Result.StateAttribute3 = lastResponderResult.StateAttribute3;
				}
				IDataAccessQuery<MonitorResult> lastSuccessfulMonitorResult = this.Broker.GetLastSuccessfulMonitorResult(this.Definition.AlertMask, startTime, this.Result.ExecutionStartTime);
				Task<MonitorResult> task = lastSuccessfulMonitorResult.ExecuteAsync(cancellationToken, this.TraceContext);
				task.Continue(delegate(MonitorResult lastMonitorResult)
				{
					if (lastMonitorResult != null)
					{
						if (lastMonitorResult.Component == null)
						{
							lastMonitorResult.Component = new Component(lastMonitorResult.ComponentName);
						}
						if (lastMonitorResult.IsAlert)
						{
							if (lastResponderResult != null && string.Compare(lastMonitorResult.ComponentName, lastResponderResult.StateAttribute2, true) == 0 && lastResponderResult.StateAttribute1 != null)
							{
								if (this.Definition.MinimumSecondsBetweenEscalates == -1 || this.Definition.WaitIntervalSeconds == -1)
								{
									this.Result.IsThrottled = true;
								}
								else
								{
									this.Result.IsThrottled = (DateTime.Parse(lastResponderResult.StateAttribute1) > this.Result.ExecutionStartTime);
								}
							}
							WTFDiagnostics.TraceDebug<string, string>(ExTraceGlobals.CommonComponentsTracer, this.TraceContext, "EscalateResponderBase.DoResponderWork: Component {0} is Alerting. Throttled:{1}", lastMonitorResult.ComponentName, this.Result.IsThrottled.ToString(), null, "DoResponderWork", "f:\\15.00.1497\\sources\\dev\\common\\src\\WorkerTaskFramework\\ActiveMonitoring\\WorkItems\\Responders\\EscalateResponderBase.cs", 859);
							if (!this.Result.IsThrottled)
							{
								this.BeforeEscalate(cancellationToken);
								if (this.Definition.UploadScopeNotification)
								{
									EscalateResponderBase.UnhealthyMonitoringEvent unhealthyMonitoringEvent = this.GetUnhealthyMonitoringEvent(lastMonitorResult, !this.Definition.AlwaysEscalateOnMonitorChanges && this.IncludeHealthSetEscalationInfo);
									this.AddScopeNotification(lastMonitorResult, unhealthyMonitoringEvent.Subject, unhealthyMonitoringEvent.Message);
								}
								if (this.Definition.SuppressEscalation)
								{
									this.Result.ResponseAction = EscalateResponderBase.ResponseAction.EscalationSuppressed.ToString();
									return;
								}
								EscalationState escalationState = this.GetEscalationState(new bool?(false), cancellationToken);
								bool flag = false;
								if (escalationState == EscalationState.Red || escalationState == EscalationState.Orange)
								{
									flag = true;
								}
								else if (escalationState == EscalationState.DarkYellow)
								{
									flag = false;
									this.Result.ResponseAction = EscalateResponderBase.ResponseAction.DeferNonBusinessHours.ToString();
								}
								else if (escalationState == EscalationState.Yellow)
								{
									flag = true;
								}
								string name = lastMonitorResult.Component.Name;
								bool flag2 = false;
								string text = null;
								if (!this.Definition.AlwaysEscalateOnMonitorChanges)
								{
									text = Guid.NewGuid().ToString();
									HealthSetEscalationState healthSetEscalationState = EscalateResponderBase.EscalationHelper.LockHealthSetEscalationStateIfRequired(name, escalationState, text);
									if (healthSetEscalationState != null)
									{
										WTFDiagnostics.TraceDebug<string, string, string, string, string>(ExTraceGlobals.CommonComponentsTracer, this.TraceContext, "EscalateResponderBase.DoResponderWork: LockHealthSetEscalationStateIfRequired(healthSetName:{0}, escalationState:{1}, lockOwnerId:{2}) returned healthSetEscalationState with: EscalationState:{3} LockOwnerId:{4}", name, escalationState.ToString(), text, healthSetEscalationState.EscalationState.ToString(), healthSetEscalationState.LockOwnerId, null, "DoResponderWork", "f:\\15.00.1497\\sources\\dev\\common\\src\\WorkerTaskFramework\\ActiveMonitoring\\WorkItems\\Responders\\EscalateResponderBase.cs", 930);
										if (healthSetEscalationState.EscalationState >= escalationState && healthSetEscalationState.LockOwnerId == null)
										{
											flag = false;
											flag2 = false;
											this.Result.ResponseAction = EscalateResponderBase.ResponseAction.DeferHealthSetSuppression.ToString();
										}
										else
										{
											if (healthSetEscalationState.EscalationState >= escalationState || !(healthSetEscalationState.LockOwnerId == text))
											{
												return;
											}
											flag2 = true;
										}
									}
								}
								if (flag)
								{
									this.Escalate(lastMonitorResult, escalationState);
									this.Result.ResponseAction = EscalateResponderBase.ResponseAction.Escalate.ToString();
									if (this.Definition.MinimumSecondsBetweenEscalates > 0)
									{
										this.Result.StateAttribute1 = DateTime.UtcNow.AddSeconds((double)this.Definition.MinimumSecondsBetweenEscalates).ToString();
									}
									else if (this.Definition.WaitIntervalSeconds > 0)
									{
										this.Result.StateAttribute1 = DateTime.UtcNow.AddSeconds((double)this.Definition.WaitIntervalSeconds).ToString();
									}
									else
									{
										this.Result.StateAttribute1 = DateTime.UtcNow.AddSeconds(14400.0).ToString();
									}
									this.Result.StateAttribute2 = lastMonitorResult.ComponentName;
									this.AfterEscalate(cancellationToken);
								}
								else
								{
									bool flag3 = false;
									if (lastResponderResult != null && lastResponderResult.StateAttribute3 != null)
									{
										flag3 = (DateTime.Parse(lastResponderResult.StateAttribute3) > this.Result.ExecutionStartTime);
									}
									WTFDiagnostics.TraceDebug<string, string>(ExTraceGlobals.CommonComponentsTracer, this.TraceContext, "EscalateResponderBase.DoResponderWork: Not escalating component {0}. logEventIsThrottled:{1}", lastMonitorResult.ComponentName, flag3.ToString(), null, "DoResponderWork", "f:\\15.00.1497\\sources\\dev\\common\\src\\WorkerTaskFramework\\ActiveMonitoring\\WorkItems\\Responders\\EscalateResponderBase.cs", 1010);
									if (!flag3)
									{
										this.LogCustomUnhealthyEvent(this.GetUnhealthyMonitoringEvent(lastMonitorResult, false));
										this.Result.StateAttribute3 = DateTime.UtcNow.AddSeconds(14400.0).ToString();
									}
								}
								if (!this.Definition.AlwaysEscalateOnMonitorChanges && flag2 && !EscalateResponderBase.EscalationHelper.SetHealthSetEscalationState(name, escalationState, text))
								{
									WTFDiagnostics.TraceDebug<string, string, string>(ExTraceGlobals.CommonComponentsTracer, this.TraceContext, "EscalateResponderBase.DoResponderWork: SetHealthSetEscalationState(healthSetName:{0}, escalationState:{1}, lockOwnerId:{2}) returned false, returning.", name, escalationState.ToString(), text, null, "DoResponderWork", "f:\\15.00.1497\\sources\\dev\\common\\src\\WorkerTaskFramework\\ActiveMonitoring\\WorkItems\\Responders\\EscalateResponderBase.cs", 1031);
									return;
								}
							}
							else
							{
								this.Result.ResponseAction = EscalateResponderBase.ResponseAction.Throttled.ToString();
								this.Result.StateAttribute1 = lastResponderResult.StateAttribute1;
								this.Result.StateAttribute2 = lastMonitorResult.ComponentName;
								this.Result.StateAttribute3 = lastResponderResult.StateAttribute3;
							}
						}
					}
				}, cancellationToken, TaskContinuationOptions.AttachedToParent | TaskContinuationOptions.NotOnFaulted | TaskContinuationOptions.NotOnCanceled);
			}, cancellationToken, TaskContinuationOptions.AttachedToParent);
		}

		protected virtual NotificationServiceClass GetNotificationServiceClass(MonitorResult monitorResult)
		{
			return base.Definition.NotificationServiceClass;
		}

		protected virtual void BeforeEscalate(CancellationToken cancellationToken)
		{
		}

		protected virtual void AfterEscalate(CancellationToken cancellationToken)
		{
		}

		protected virtual void InvokeNewServiceAlert(Guid alertGuid, string alertTypeId, string alertName, string alertDescription, DateTime raisedTime, string escalationTeam, string service, string alertSource, bool isDatacenter, bool urgent, string environment, string location, string forest, string dag, string site, string region, string capacityUnit, string rack, string alertCategory)
		{
			this.InvokeNewServiceAlert(alertGuid, alertTypeId, alertName, alertDescription, raisedTime, escalationTeam, service, alertSource, isDatacenter, urgent, environment, location, forest, dag, site, region, capacityUnit, rack, alertCategory, false, false);
		}

		protected virtual void InvokeNewServiceAlert(Guid alertGuid, string alertTypeId, string alertName, string alertDescription, DateTime raisedTime, string escalationTeam, string service, string alertSource, bool isDatacenter, bool urgent, string environment, string location, string forest, string dag, string site, string region, string capacityUnit, string rack, string alertCategory, bool isIncident)
		{
			this.InvokeNewServiceAlert(alertGuid, alertTypeId, alertName, alertDescription, raisedTime, escalationTeam, service, alertSource, isDatacenter, urgent, environment, location, forest, dag, site, region, capacityUnit, rack, alertCategory, isIncident, false);
		}

		protected virtual void InvokeNewServiceAlert(Guid alertGuid, string alertTypeId, string alertName, string alertDescription, DateTime raisedTime, string escalationTeam, string service, string alertSource, bool isDatacenter, bool urgent, string environment, string location, string forest, string dag, string site, string region, string capacityUnit, string rack, string alertCategory, bool isIncident, bool skipSuppression)
		{
			if (string.IsNullOrWhiteSpace(escalationTeam))
			{
				throw new ArgumentException("escalationTeam");
			}
			if (string.IsNullOrWhiteSpace(service))
			{
				throw new ArgumentException("service");
			}
			if (string.IsNullOrWhiteSpace(alertSource))
			{
				throw new ArgumentException("alertSource");
			}
			if (string.IsNullOrWhiteSpace(alertName))
			{
				throw new ArgumentException("alertName");
			}
			if (string.IsNullOrWhiteSpace(alertDescription))
			{
				throw new ArgumentException("alertDescription");
			}
			this.CreateRunspace();
			PSCommand pscommand = new PSCommand();
			pscommand.AddCommand("New-ServiceAlert");
			pscommand.AddParameter("AlertTypeId", alertTypeId);
			pscommand.AddParameter("AlertId", alertGuid);
			pscommand.AddParameter("AlertName", alertName);
			pscommand.AddParameter("AlertDescription", alertDescription);
			pscommand.AddParameter("RaisedTime", raisedTime);
			pscommand.AddParameter("EscalationTeam", escalationTeam);
			pscommand.AddParameter("Service", service);
			pscommand.AddParameter("AlertSource", alertSource);
			pscommand.AddParameter("IsUrgent", urgent);
			pscommand.AddParameter("IsIncident", isIncident);
			pscommand.AddParameter("SkipSuppression", skipSuppression);
			if (isDatacenter)
			{
				string value;
				if (string.IsNullOrWhiteSpace(base.Definition.TargetGroup))
				{
					value = Environment.MachineName;
				}
				else
				{
					value = base.Definition.TargetGroup;
				}
				pscommand.AddParameter("MachineName", value);
				if (EscalateResponderBase.IsFfoMachine || EscalateResponderBase.IsOBDGallatinMachine)
				{
					pscommand.AddParameter("MachineProvisioningState", "Provisioned");
					pscommand.AddParameter("MachineMonitoringState", "On");
					if (!string.IsNullOrEmpty(EscalateResponderBase.LocalMachineVersion))
					{
						pscommand.AddParameter("MachineVersion", EscalateResponderBase.LocalMachineVersion);
					}
				}
			}
			if (!string.IsNullOrWhiteSpace(environment))
			{
				pscommand.AddParameter("Environment", environment);
			}
			if (!string.IsNullOrWhiteSpace(location))
			{
				pscommand.AddParameter("Location", location);
			}
			if (!string.IsNullOrWhiteSpace(forest))
			{
				pscommand.AddParameter("Forest", forest);
			}
			if (!string.IsNullOrWhiteSpace(dag))
			{
				pscommand.AddParameter("Dag", dag);
			}
			if (!string.IsNullOrWhiteSpace(site))
			{
				pscommand.AddParameter("Site", site);
			}
			if (!string.IsNullOrWhiteSpace(region))
			{
				pscommand.AddParameter("Region", region);
			}
			if (!string.IsNullOrWhiteSpace(capacityUnit))
			{
				pscommand.AddParameter("CapacityUnit", capacityUnit);
			}
			if (!string.IsNullOrWhiteSpace(rack))
			{
				pscommand.AddParameter("Rack", capacityUnit);
			}
			if (!string.IsNullOrEmpty(alertCategory))
			{
				pscommand.AddParameter("AlertCategory", alertCategory);
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(pscommand.Commands[0].CommandText);
			foreach (CommandParameter commandParameter in pscommand.Commands[0].Parameters)
			{
				stringBuilder.AppendFormat(" -{0}:{1}", commandParameter.Name, commandParameter.Value.ToString());
			}
			WTFDiagnostics.TraceDebug<string, string>(ExTraceGlobals.CommonComponentsTracer, base.TraceContext, "EscalateResponderBase.InvokeNewServiceAlert: Escalating alert '{0}' via command '{1}'...", alertName, stringBuilder.ToString(), null, "InvokeNewServiceAlert", "f:\\15.00.1497\\sources\\dev\\common\\src\\WorkerTaskFramework\\ActiveMonitoring\\WorkItems\\Responders\\EscalateResponderBase.cs", 1402);
			try
			{
				this.remotePowerShell.InvokePSCommand(pscommand);
				WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.CommonComponentsTracer, base.TraceContext, "EscalateResponderBase.InvokeNewServiceAlert: Successfully escalated alert '{0}'.", alertName, null, "InvokeNewServiceAlert", "f:\\15.00.1497\\sources\\dev\\common\\src\\WorkerTaskFramework\\ActiveMonitoring\\WorkItems\\Responders\\EscalateResponderBase.cs", 1406);
			}
			catch (Exception ex)
			{
				throw new Exception(string.Format("EscalateResponderBase.InvokeNewServiceAlert: Unexpected failure when escalating alert '{0}'\r\n\r\nException: {1}\r\n\r\nCommand: '{2}'", alertName, ex.ToString(), stringBuilder.ToString()));
			}
		}

		protected virtual bool ShouldRaiseActiveMonitoringAlerts(EscalationEnvironment environment)
		{
			switch (environment)
			{
			case EscalationEnvironment.Datacenter:
			{
				bool result = false;
				using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(EscalateResponderBase.ActiveMonitoringRegistryPath, false))
				{
					if (registryKey != null)
					{
						result = (0 != (int)registryKey.GetValue("AlertsEnabled", 0));
					}
				}
				return result;
			}
			case EscalationEnvironment.OutsideIn:
				return true;
			default:
				return false;
			}
		}

		protected override bool? ShouldAlwaysInvoke()
		{
			WTFDiagnostics.TraceDebug<string>(ExTraceGlobals.CommonComponentsTracer, base.TraceContext, "[{0}] This is an escalation type responder and should always be invoked.", base.GetType().FullName, null, "ShouldAlwaysInvoke", "f:\\15.00.1497\\sources\\dev\\common\\src\\WorkerTaskFramework\\ActiveMonitoring\\WorkItems\\Responders\\EscalateResponderBase.cs", 1452);
			return new bool?(true);
		}

		protected void CreateRunspace()
		{
			if (string.IsNullOrWhiteSpace(base.Definition.Account) || string.IsNullOrWhiteSpace(base.Definition.Endpoint))
			{
				EscalateResponderBase.SetActiveMonitoringCertificateSettings(base.Definition);
				WTFDiagnostics.TraceInformation(ExTraceGlobals.CommonComponentsTracer, base.TraceContext, "No authentication values were defined in EscalateResponderWorkDefinition. Certification settings have now been set.", null, "CreateRunspace", "f:\\15.00.1497\\sources\\dev\\common\\src\\WorkerTaskFramework\\ActiveMonitoring\\WorkItems\\Responders\\EscalateResponderBase.cs", 1469);
			}
			if (!string.IsNullOrWhiteSpace(base.Definition.AccountPassword))
			{
				this.remotePowerShell = RemotePowerShell.CreateRemotePowerShellByCredential(new Uri(base.Definition.Endpoint), base.Definition.Account, base.Definition.AccountPassword, this.GetEscalationEnvironment() != EscalationEnvironment.OutsideIn);
				return;
			}
			if (base.Definition.Endpoint.Contains(";"))
			{
				this.remotePowerShell = RemotePowerShell.CreateRemotePowerShellByCertificate(base.Definition.Endpoint.Split(new char[]
				{
					';'
				}), base.Definition.Account, this.GetEscalationEnvironment() != EscalationEnvironment.OutsideIn);
				return;
			}
			this.remotePowerShell = RemotePowerShell.CreateRemotePowerShellByCertificate(new Uri(base.Definition.Endpoint), base.Definition.Account, this.GetEscalationEnvironment() != EscalationEnvironment.OutsideIn);
		}

		internal virtual void LogCustomUnhealthyEvent(EscalateResponderBase.UnhealthyMonitoringEvent unhealthyEvent)
		{
		}

		internal virtual string GetFFOForestName()
		{
			return string.Empty;
		}

		internal abstract EscalationEnvironment GetEscalationEnvironment();

		internal abstract ScopeMappingEndpoint GetScopeMappingEndpoint();

		public static string GetNonLocalizableResourceAsString(Assembly manifestAssembly, string resourceName)
		{
			string result;
			using (Stream manifestResourceStream = manifestAssembly.GetManifestResourceStream(resourceName))
			{
				using (StreamReader streamReader = new StreamReader(manifestResourceStream))
				{
					result = streamReader.ReadToEnd();
				}
			}
			return result;
		}

		private static void SetExtensionAttribute(XmlDocument xml, string attributeName, string attributeValue)
		{
			if (!string.IsNullOrWhiteSpace(attributeValue))
			{
				if (xml["ExtensionAttributes"].Attributes[attributeName] == null)
				{
					xml["ExtensionAttributes"].Attributes.Append(xml.CreateAttribute(attributeName));
				}
				xml["ExtensionAttributes"].Attributes[attributeName].Value = attributeValue;
			}
		}

		private void Escalate(MonitorResult monitorResult, EscalationState escalationState)
		{
			ServiceHealthStatus serviceHealthStatus;
			if (base.Definition.TargetHealthState == ServiceHealthStatus.None)
			{
				WTFDiagnostics.TraceDebug<string>(ExTraceGlobals.CommonComponentsTracer, base.TraceContext, "EscalateResponderBase.Escalate: TargetHealthState was not defined and ManagedAvailability was not used -- proceed to escalate using health state: {0}", ServiceHealthStatus.Unrecoverable.ToString(), null, "Escalate", "f:\\15.00.1497\\sources\\dev\\common\\src\\WorkerTaskFramework\\ActiveMonitoring\\WorkItems\\Responders\\EscalateResponderBase.cs", 1593);
				serviceHealthStatus = ServiceHealthStatus.Unrecoverable;
			}
			else
			{
				serviceHealthStatus = base.Definition.TargetHealthState;
			}
			EscalateResponderBase.UnhealthyMonitoringEvent unhealthyMonitoringEvent = this.GetUnhealthyMonitoringEvent(monitorResult, !base.Definition.AlwaysEscalateOnMonitorChanges && this.IncludeHealthSetEscalationInfo);
			this.LogCustomUnhealthyEvent(unhealthyMonitoringEvent);
			if (serviceHealthStatus != ServiceHealthStatus.Healthy && this.ShouldRaiseActiveMonitoringAlerts(this.GetEscalationEnvironment()))
			{
				string service;
				if (!string.IsNullOrWhiteSpace(this.escalationService))
				{
					service = this.escalationService;
				}
				else if (!string.IsNullOrWhiteSpace(base.Definition.EscalationService))
				{
					service = base.Definition.EscalationService;
				}
				else
				{
					service = monitorResult.Component.Service;
				}
				string text;
				if (!string.IsNullOrWhiteSpace(this.escalationTeam))
				{
					text = this.escalationTeam;
				}
				else if (!string.IsNullOrWhiteSpace(base.Definition.EscalationTeam))
				{
					text = base.Definition.EscalationTeam;
				}
				else
				{
					text = monitorResult.Component.EscalationTeam;
				}
				string alertSource = "LocalActiveMonitoring";
				bool isDatacenter = this.GetEscalationEnvironment() == EscalationEnvironment.Datacenter;
				string environment = string.Empty;
				string location = (this.GetEscalationEnvironment() == EscalationEnvironment.OutsideIn && base.Definition.Attributes.ContainsKey("ScopeLocation")) ? base.Definition.Attributes["ScopeLocation"] : null;
				string forest = (this.GetEscalationEnvironment() == EscalationEnvironment.OutsideIn && base.Definition.Attributes.ContainsKey("ScopeForest")) ? base.Definition.Attributes["ScopeForest"] : null;
				string dag = (this.GetEscalationEnvironment() == EscalationEnvironment.OutsideIn && base.Definition.Attributes.ContainsKey("ScopeDag")) ? base.Definition.Attributes["ScopeDag"] : null;
				string site = (this.GetEscalationEnvironment() == EscalationEnvironment.OutsideIn && base.Definition.Attributes.ContainsKey("ScopeSite")) ? base.Definition.Attributes["ScopeSite"] : null;
				string region = (this.GetEscalationEnvironment() == EscalationEnvironment.OutsideIn && base.Definition.Attributes.ContainsKey("ScopeRegion")) ? base.Definition.Attributes["ScopeRegion"] : null;
				string capacityUnit = (this.GetEscalationEnvironment() == EscalationEnvironment.OutsideIn && base.Definition.Attributes.ContainsKey("ScopeCapacityUnit")) ? base.Definition.Attributes["ScopeCapacityUnit"] : null;
				string rack = (this.GetEscalationEnvironment() == EscalationEnvironment.OutsideIn && base.Definition.Attributes.ContainsKey("ScopeRack")) ? base.Definition.Attributes["ScopeRack"] : null;
				NotificationServiceClass notificationServiceClass = this.escalationNotificationType ?? base.Definition.NotificationServiceClass;
				string alertCategory;
				switch (notificationServiceClass)
				{
				case NotificationServiceClass.UrgentInTraining:
					alertCategory = NotificationServiceClass.UrgentInTraining.ToString();
					break;
				case NotificationServiceClass.Scheduled:
					alertCategory = NotificationServiceClass.Scheduled.ToString();
					break;
				default:
					alertCategory = null;
					break;
				}
				bool urgent = escalationState == EscalationState.Red;
				if (notificationServiceClass == NotificationServiceClass.UrgentInTraining || notificationServiceClass == NotificationServiceClass.Scheduled)
				{
					urgent = false;
				}
				if (EscalateResponderBase.IsFfoMachine)
				{
					this.FfoEscalationOverrides(ref service, ref text);
					site = this.GetExchangeLabsServiceTag();
					location = this.GetExchangeLabsDatacenterName();
					forest = this.GetFFOForestName();
					string exchangeLabsServiceName;
					if ((exchangeLabsServiceName = this.GetExchangeLabsServiceName()) != null)
					{
						if (!(exchangeLabsServiceName == "FopeDevTest"))
						{
							if (!(exchangeLabsServiceName == "FopeDogfood"))
							{
								if (exchangeLabsServiceName == "FopeProd")
								{
									environment = "FSPROD";
								}
							}
							else
							{
								environment = "FSDF";
							}
						}
						else
						{
							environment = "TEST";
						}
					}
				}
				Guid alertGuid = Guid.NewGuid();
				base.Result.ResponseResource = alertGuid.ToString();
				this.InvokeNewServiceAlert(alertGuid, base.Definition.AlertTypeId, unhealthyMonitoringEvent.Subject, unhealthyMonitoringEvent.Message, monitorResult.ExecutionStartTime, text, service, alertSource, isDatacenter, urgent, environment, location, forest, dag, site, region, capacityUnit, rack, alertCategory);
			}
		}

		private ProbeResult GetLastFailedProbeResult(MonitorResult monitorResult)
		{
			if (this.lastFailedProbeResult == null && monitorResult.LastFailedProbeResultId > 0)
			{
				this.lastFailedProbeResult = base.Broker.GetProbeResult(monitorResult.LastFailedProbeId, monitorResult.LastFailedProbeResultId).ExecuteAsync(this.LocalCancellationToken, base.TraceContext).Result;
			}
			return this.lastFailedProbeResult;
		}

		private EscalateResponderBase.UnhealthyMonitoringEvent GetUnhealthyMonitoringEvent(MonitorResult monitorResult, bool includeHealthSetEscalationInfo = false)
		{
			string text;
			string message;
			this.GetEscalationSubjectAndMessage(monitorResult, out text, out message, false, null);
			string name = monitorResult.Component.Name;
			if (includeHealthSetEscalationInfo)
			{
				string format = EscalateResponderBase.HealthSetEscalationSubjectPrefix;
				if (base.Definition.AlertTypeId.IndexOf("MaintenanceFailureMonitor") == 0)
				{
					format = EscalateResponderBase.HealthSetMaintenanceEscalationSubjectPrefix;
				}
				text = string.Format(format, name, monitorResult.ResultName, text);
				EscalateResponderBase.EscalationHelper.ExtendEscalationMessage(name, ref message);
			}
			if (ScopeMappingEndpoint.IsSystemMonitoringEnvironment)
			{
				this.ExtendEscalationSubjectAndMessageForSystemMonitoring(monitorResult, ref text, ref message);
			}
			if (this.GetEscalationEnvironment() == EscalationEnvironment.OutsideIn)
			{
				text = string.Format("{0}: {1}", Settings.HostedServiceName, text);
			}
			return new EscalateResponderBase.UnhealthyMonitoringEvent
			{
				HealthSet = name,
				Subject = text,
				Message = message,
				Monitor = monitorResult.ResultName
			};
		}

		private void ExtendEscalationSubjectAndMessageForSystemMonitoring(MonitorResult monitorResult, ref string escalationSubject, ref string escalationMessage)
		{
			string sourceScope = monitorResult.SourceScope;
			string name = monitorResult.Component.Name;
			if (string.IsNullOrWhiteSpace(sourceScope))
			{
				return;
			}
			if (!monitorResult.IsAlert)
			{
				return;
			}
			IDataAccessQuery<ProbeResult> probeResults = base.Broker.GetProbeResults(sourceScope, monitorResult.ExecutionStartTime - TimeSpan.FromSeconds((double)base.Definition.MinimumSecondsBetweenEscalates));
			Dictionary<string, List<ProbeResult>> healthSetsMap = new Dictionary<string, List<ProbeResult>>();
			probeResults.ExecuteAsync(delegate(ProbeResult result)
			{
				List<ProbeResult> list4;
				if (!healthSetsMap.TryGetValue(result.HealthSetName, out list4))
				{
					list4 = new List<ProbeResult>();
					healthSetsMap.Add(result.HealthSetName, list4);
				}
				list4.Add(result);
			}, this.localCancellationToken, base.TraceContext).Wait(this.localCancellationToken);
			if (healthSetsMap.Count == 0)
			{
				WTFDiagnostics.TraceError<string>(ExTraceGlobals.CommonComponentsTracer, base.TraceContext, "No probe results found for scope '{0}'.", sourceScope, null, "ExtendEscalationSubjectAndMessageForSystemMonitoring", "f:\\15.00.1497\\sources\\dev\\common\\src\\WorkerTaskFramework\\ActiveMonitoring\\WorkItems\\Responders\\EscalateResponderBase.cs", 1851);
				return;
			}
			List<ProbeResult> list;
			if (!healthSetsMap.TryGetValue(name, out list))
			{
				WTFDiagnostics.TraceError<string, string>(ExTraceGlobals.CommonComponentsTracer, base.TraceContext, "Failed to get the probe results for scope '{0}' and health set '{1}'.", sourceScope, name, null, "ExtendEscalationSubjectAndMessageForSystemMonitoring", "f:\\15.00.1497\\sources\\dev\\common\\src\\WorkerTaskFramework\\ActiveMonitoring\\WorkItems\\Responders\\EscalateResponderBase.cs", 1859);
				return;
			}
			list.Sort((ProbeResult x, ProbeResult y) => y.ExecutionEndTime.CompareTo(x.ExecutionEndTime));
			ProbeResult probeResult = list.Find((ProbeResult r) => r.ResultType == ResultType.Failed);
			if (probeResult != null)
			{
				string valueFromDataXml = this.GetValueFromDataXml("EscalationSubject", probeResult.Data);
				if (!string.IsNullOrWhiteSpace(valueFromDataXml))
				{
					escalationSubject = string.Format("{0}: {1}", escalationSubject, valueFromDataXml);
				}
				string valueFromDataXml2 = this.GetValueFromDataXml("EscalationMessage", probeResult.Data);
				if (!string.IsNullOrWhiteSpace(valueFromDataXml2))
				{
					escalationMessage = string.Format("{0}{1}{2}", escalationMessage, "<br><br><hr><br>", valueFromDataXml2);
				}
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendFormat("<b>States of all monitors within the '{0}' health set:</b><br>", name);
				stringBuilder.AppendFormat("Note: Data may be stale.<br><br>", new object[0]);
				string[] headers = new string[]
				{
					"Name",
					"State",
					"Source",
					"Type"
				};
				List<string[]> list2 = new List<string[]>(list.Count);
				foreach (ProbeResult probeResult2 in list)
				{
					list2.Add(new string[]
					{
						probeResult2.ResultName,
						(probeResult2.ResultType == ResultType.Succeeded) ? "Healthy" : "Unhealthy",
						probeResult2.StateAttribute11,
						probeResult2.StateAttribute12
					});
				}
				stringBuilder.AppendFormat("{0}", TableDecorator.CreateTable(headers, list2));
				StringBuilder stringBuilder2 = new StringBuilder();
				stringBuilder2.AppendFormat("<br><br><b>States of all health sets within the '{0}' {1} scope:</b><br>", sourceScope, probeResult.ScopeType);
				stringBuilder2.AppendFormat("Note: Data may be stale.<br><br>", new object[0]);
				string[] headers2 = new string[]
				{
					"HealthSet",
					"State",
					"LastTransitionTime",
					"MonitorCount"
				};
				List<string[]> list3 = new List<string[]>(healthSetsMap.Count);
				foreach (KeyValuePair<string, List<ProbeResult>> healthSetResults in healthSetsMap)
				{
					list3.Add(this.AggregateHealthSet(healthSetResults));
				}
				stringBuilder2.AppendFormat("{0}", TableDecorator.CreateTable(headers2, list3));
				escalationMessage = string.Format("{0}{1}{2}{3}{1}", new object[]
				{
					escalationMessage,
					"<br><br><hr><br>",
					stringBuilder.ToString(),
					stringBuilder2.ToString()
				});
				return;
			}
			WTFDiagnostics.TraceError<string, string>(ExTraceGlobals.CommonComponentsTracer, base.TraceContext, "Unable to find the last failed probe result for scope '{0}' and health set '{1}'.", sourceScope, name, null, "ExtendEscalationSubjectAndMessageForSystemMonitoring", "f:\\15.00.1497\\sources\\dev\\common\\src\\WorkerTaskFramework\\ActiveMonitoring\\WorkItems\\Responders\\EscalateResponderBase.cs", 1923);
		}

		private string GetValueFromDataXml(string name, string data)
		{
			try
			{
				XElement xelement = XElement.Parse(data);
				return xelement.Element(name).Value;
			}
			catch (Exception)
			{
			}
			return null;
		}

		private string[] AggregateHealthSet(KeyValuePair<string, List<ProbeResult>> healthSetResults)
		{
			List<string> list = new List<string>();
			list.Add(healthSetResults.Key);
			bool flag = true;
			DateTime t = DateTime.MinValue;
			foreach (ProbeResult probeResult in healthSetResults.Value)
			{
				flag &= (probeResult.ResultType == ResultType.Succeeded);
				if (t < probeResult.ExecutionEndTime)
				{
					t = probeResult.ExecutionEndTime;
				}
			}
			list.Add(flag ? "Healthy" : "Unhealthy");
			list.Add(t.ToString());
			list.Add(healthSetResults.Value.Count.ToString());
			return list.ToArray();
		}

		private void AddScopeNotification(MonitorResult monitorResult, string escalationSubject = null, string escalationMessage = null)
		{
			if (!base.Definition.UploadScopeNotification)
			{
				return;
			}
			if (this.GetEscalationEnvironment() == EscalationEnvironment.OnPrem)
			{
				return;
			}
			if (string.IsNullOrWhiteSpace(monitorResult.TargetScopes))
			{
				return;
			}
			try
			{
				ProbeResult probeResult = this.GetLastFailedProbeResult(monitorResult);
				string sourceInstanceType;
				if (base.Broker.IsLocal())
				{
					sourceInstanceType = "LAM";
				}
				else if (ScopeMappingEndpoint.IsSystemMonitoringEnvironment)
				{
					sourceInstanceType = "SM";
				}
				else
				{
					sourceInstanceType = "XAM";
				}
				bool isMultiSourceInstance = !base.Broker.IsLocal();
				string text = null;
				if (!string.IsNullOrWhiteSpace(escalationSubject) && !string.IsNullOrWhiteSpace(escalationMessage))
				{
					text = string.Format("<EscalationSubject><![CDATA[{0}]]></EscalationSubject><EscalationMessage><![CDATA[{1}]]></EscalationMessage>", escalationSubject, escalationMessage);
				}
				if (!string.IsNullOrWhiteSpace(text))
				{
					text = string.Format("<Data>{0}</Data>", text);
				}
				string name = monitorResult.Component.Name;
				string[] array = monitorResult.TargetScopes.Split(new char[]
				{
					';'
				});
				foreach (string scopeName in array)
				{
					ScopeNotificationCache.Instance.AddScopeNotificationRawData(new ScopeNotificationRawData
					{
						NotificationName = string.Format("Escalation/{0}/{1}", name, monitorResult.ResultName),
						ScopeName = scopeName,
						HealthSetName = name,
						HealthState = (int)this.TranslateHealthState(monitorResult.HealthState),
						MachineName = Environment.MachineName,
						SourceInstanceName = ((!string.IsNullOrWhiteSpace(Settings.InstanceName)) ? Settings.InstanceName : Environment.MachineName),
						SourceInstanceType = sourceInstanceType,
						IsMultiSourceInstance = isMultiSourceInstance,
						ExecutionStartTime = (monitorResult.FirstAlertObservedTime ?? monitorResult.ExecutionStartTime),
						ExecutionEndTime = monitorResult.ExecutionEndTime,
						Error = ((probeResult != null) ? probeResult.Error : null),
						Exception = ((probeResult != null) ? probeResult.Exception : null),
						ExecutionContext = ((probeResult != null) ? probeResult.ExecutionContext : null),
						FailureContext = ((probeResult != null) ? probeResult.FailureContext : null),
						Data = text
					});
				}
			}
			catch (Exception ex)
			{
				WTFDiagnostics.TraceError<string>(ExTraceGlobals.CommonComponentsTracer, base.TraceContext, "Failed to add scope data to cache: {0}", ex.ToString(), null, "AddScopeNotification", "f:\\15.00.1497\\sources\\dev\\common\\src\\WorkerTaskFramework\\ActiveMonitoring\\WorkItems\\Responders\\EscalateResponderBase.cs", 2078);
				if (base.Definition.UploadScopeNotification && base.Definition.SuppressEscalation)
				{
					throw;
				}
			}
		}

		private ResultType TranslateHealthState(ServiceHealthStatus healthStateValue)
		{
			switch (healthStateValue)
			{
			case ServiceHealthStatus.None:
			case ServiceHealthStatus.Healthy:
				return ResultType.Succeeded;
			default:
				return ResultType.Failed;
			}
		}

		private void FfoEscalationOverrides(ref string escalationService, ref string escalationTeam)
		{
			string key;
			if (escalationService == "Exchange" && (key = escalationTeam) != null)
			{
				if (<PrivateImplementationDetails>{3E5494A9-60C5-46F7-92C8-8069F1074799}.$$method0x600055d-1 == null)
				{
					<PrivateImplementationDetails>{3E5494A9-60C5-46F7-92C8-8069F1074799}.$$method0x600055d-1 = new Dictionary<string, int>(10)
					{
						{
							"Central Admin",
							0
						},
						{
							"Deployment",
							1
						},
						{
							"Directory and LiveId Auth",
							2
						},
						{
							"High Availability",
							3
						},
						{
							"Monitoring",
							4
						},
						{
							"Networking",
							5
						},
						{
							"Ops support",
							6
						},
						{
							"Optics",
							7
						},
						{
							"Performance",
							8
						},
						{
							"Security",
							9
						}
					};
				}
				int num;
				if (<PrivateImplementationDetails>{3E5494A9-60C5-46F7-92C8-8069F1074799}.$$method0x600055d-1.TryGetValue(key, out num))
				{
					switch (num)
					{
					case 0:
						escalationService = "EOP";
						escalationTeam = "Deployment and Configuration Management";
						return;
					case 1:
						escalationService = "EOP";
						escalationTeam = "Deployment and Configuration Management";
						return;
					case 2:
						escalationService = "EOP";
						escalationTeam = "Directory and Database Infrastructure";
						return;
					case 3:
						escalationService = "EOP";
						escalationTeam = "Service Automation & Monitoring";
						return;
					case 4:
						escalationService = "EOP";
						escalationTeam = "Service Automation & Monitoring";
						return;
					case 5:
						escalationService = "EOP";
						escalationTeam = "Deployment and Configuration Management";
						return;
					case 6:
						escalationService = "EOP";
						escalationTeam = "Ops SE";
						return;
					case 7:
						escalationService = "EOP";
						escalationTeam = "Intelligence and Reporting";
						return;
					case 8:
						escalationService = "EOP";
						escalationTeam = "Intelligence and Reporting";
						return;
					case 9:
						escalationService = "EOP";
						escalationTeam = "Ops SE";
						break;
					default:
						return;
					}
				}
			}
		}

		private string GetExchangeLabsServiceName()
		{
			string result = string.Empty;
			using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\ExchangeLabs", false))
			{
				if (registryKey != null)
				{
					result = (string)registryKey.GetValue("ServiceName", string.Empty);
				}
			}
			return result;
		}

		private string GetExchangeLabsServiceTag()
		{
			string result = string.Empty;
			using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\ExchangeLabs", false))
			{
				if (registryKey != null)
				{
					result = (string)registryKey.GetValue("ServiceTag", string.Empty);
				}
			}
			return result;
		}

		private string GetExchangeLabsDatacenterName()
		{
			string result = string.Empty;
			using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\ExchangeLabs", false))
			{
				if (registryKey != null)
				{
					result = (string)registryKey.GetValue("Datacenter", string.Empty);
				}
			}
			return result;
		}

		private Task<ResponderResult> GetLastSuccessfulRecoveryAttemptedResponderResult(CancellationToken cancellationToken)
		{
			Task<ResponderResult> lastResponderResultById = this.GetLastResponderResultById(cancellationToken);
			return this.ContinueOnTaskWithFallback<ResponderResult>(lastResponderResultById, (CancellationToken cancellation) => this.GetLastResponderResultByName(cancellation), cancellationToken);
		}

		private Task<ResponderResult> GetLastResponderResultById(CancellationToken cancellationToken)
		{
			IDataAccessQuery<ResponderResult> lastSuccessfulRecoveryAttemptedResponderResult = base.Broker.GetLastSuccessfulRecoveryAttemptedResponderResult(base.Definition, DateTime.UtcNow - SqlDateTime.MinValue.Value);
			return lastSuccessfulRecoveryAttemptedResponderResult.ExecuteAsync(cancellationToken, base.TraceContext);
		}

		private Task<ResponderResult> GetLastResponderResultByName(CancellationToken cancellationToken)
		{
			base.Result.StateAttribute6 = 1.0;
			IDataAccessQuery<ResponderResult> lastSuccessfulRecoveryAttemptedResponderResultByName = base.Broker.GetLastSuccessfulRecoveryAttemptedResponderResultByName(base.Definition, DateTime.UtcNow - SqlDateTime.MinValue.Value);
			return lastSuccessfulRecoveryAttemptedResponderResultByName.ExecuteAsync(cancellationToken, base.TraceContext);
		}

		private Task<TResult> ContinueOnTaskWithFallback<TResult>(Task<TResult> task, Func<CancellationToken, Task<TResult>> fallback, CancellationToken cancellationToken)
		{
			TaskCompletionSource<TResult> taskCompletionSource = new TaskCompletionSource<TResult>();
			this.ContinueOnTaskWithFallback<TResult>(task, fallback, taskCompletionSource, cancellationToken);
			return taskCompletionSource.Task;
		}

		private void ContinueOnTaskWithFallback<TResult>(Task<TResult> task, Func<CancellationToken, Task<TResult>> fallback, TaskCompletionSource<TResult> completionSource, CancellationToken cancellationToken)
		{
			task.Continue(delegate(TResult result)
			{
				try
				{
					task.Wait();
				}
				catch (Exception exception)
				{
					if (fallback == null)
					{
						completionSource.SetException(exception);
						return;
					}
				}
				if (result != null || fallback == null)
				{
					completionSource.SetResult(result);
					return;
				}
				this.ContinueOnTaskWithFallback<TResult>(fallback(cancellationToken), null, completionSource, cancellationToken);
			}, cancellationToken, TaskContinuationOptions.AttachedToParent);
		}

		internal const string MaintenanceEscalationAlertTypeId = "MaintenanceFailureMonitor";

		private const string LoadFromResourceAttributeValueName = "LoadFromResourceAttributeValue";

		public const string AlertSource = "LocalActiveMonitoring";

		private string escalationService;

		private string escalationTeam;

		private NotificationServiceClass? escalationNotificationType = null;

		private string customEscalationSubject;

		private string customEscalationMessage;

		private static readonly string ActiveMonitoringRegistryPath = "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\ActiveMonitoring\\";

		protected static string DefaultEscalationSubjectInternal = "Exchange Server Alert: The {0} Health Set detected {1} is unhealthy.";

		protected static string DefaultEscalationMessageInternal = "The {0} Health Set has detected a problem with {1} at {2}. Attempts to auto-recover from this condition have failed and requires Administrator attention.";

		protected static string HealthSetEscalationSubjectPrefixInternal = "{0} Health Set unhealthy ({1}) - {2}";

		protected static string HealthSetMaintenanceEscalationSubjectPrefixInternal = "{0} Health Set maintenance unhealthy ({1}) - {2}";

		protected static HealthSetEscalationHelper EscalationHelperInternal = new HealthSetEscalationHelper();

		private static string localMachineVersion = null;

		private RemotePowerShell remotePowerShell;

		private ProbeResult lastFailedProbeResult;

		private CancellationToken localCancellationToken;

		protected enum ResponseAction
		{
			Escalate,
			Defer,
			DeferNonBusinessHours,
			DeferHealthSetSuppression,
			Throttled,
			EscalationSuppressed
		}

		internal static class AttributeNames
		{
			internal const string MinimumSecondsBetweenEscalates = "MinimumSecondsBetweenEscalates";
		}

		internal class DefaultValues
		{
			internal const int MinimumSecondsBetweenEscalates = 14400;
		}

		internal class UnhealthyMonitoringEvent
		{
			internal string HealthSet { get; set; }

			internal string Subject { get; set; }

			internal string Message { get; set; }

			internal string Monitor { get; set; }
		}
	}
}
