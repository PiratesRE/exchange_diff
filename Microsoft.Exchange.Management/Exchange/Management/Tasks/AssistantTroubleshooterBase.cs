using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Management;
using System.Management.Automation;
using System.Security;
using System.ServiceProcess;
using Microsoft.Exchange.Configuration.Common;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.Common;
using Microsoft.Exchange.Monitoring;
using Microsoft.Mapi;
using Microsoft.Win32;

namespace Microsoft.Exchange.Management.Tasks
{
	internal abstract class AssistantTroubleshooterBase : TroubleshooterCheck
	{
		public AssistantTroubleshooterBase(PropertyBag fields) : base(fields)
		{
		}

		public ExchangeServer ExchangeServer
		{
			get
			{
				return (ExchangeServer)this.fields["ExchangeServer"];
			}
		}

		public bool SendCrashDump
		{
			get
			{
				return ((SwitchParameter)(this.fields["IncludeCrashDump"] ?? new SwitchParameter(false))).IsPresent;
			}
		}

		public override MonitoringData Resolve(MonitoringData monitoringData)
		{
			foreach (MonitoringEvent monitoringEvent in monitoringData.Events)
			{
				if (monitoringEvent.EventIdentifier == 5002)
				{
					return monitoringData;
				}
			}
			this.GetWatsonExceptionBucketAndSendWatson(false);
			this.RestartAssistantService(monitoringData);
			monitoringData.PerformanceCounters.Add(this.GetCrashDumpCountPerformanceCounter());
			return monitoringData;
		}

		internal MonitoringPerformanceCounter GetCrashDumpCountPerformanceCounter()
		{
			return new MonitoringPerformanceCounter("Watson", "CrashDumpCount", "MsExchangeMailboxAssistants", (double)this.crashDumpCount);
		}

		protected void RestartAssistantService(MonitoringData monitoringData)
		{
			this.StopAssistantService(this.ExchangeServer, monitoringData);
			this.StartAssistantService(this.ExchangeServer, monitoringData);
		}

		protected void StopAssistantService(ExchangeServer server, MonitoringData monitoringData)
		{
			try
			{
				using (ServiceController serviceController = new ServiceController("MsExchangeMailboxAssistants", server.Fqdn))
				{
					if (serviceController.Status == ServiceControllerStatus.Running || serviceController.Status == ServiceControllerStatus.StartPending)
					{
						serviceController.Stop();
					}
					serviceController.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromMinutes(5.0));
					monitoringData.Events.Add(new MonitoringEvent(AssistantTroubleshooterBase.EventSource, 5001, EventTypeEnumeration.Information, Strings.MailboxAssistantsServiceStopped(this.ExchangeServer.Name)));
				}
			}
			catch (System.ServiceProcess.TimeoutException ex)
			{
				this.SendWatsonForAssistantProcess(ex, true);
				monitoringData.Events.Add(this.MailboxAssistantsServiceCouldNotBeStopped(this.ExchangeServer.Name, ex.Message));
			}
			catch (InvalidOperationException ex2)
			{
				monitoringData.Events.Add(this.MailboxAssistantsServiceCouldNotBeStopped(this.ExchangeServer.Name, ex2.Message));
			}
			catch (Win32Exception ex3)
			{
				monitoringData.Events.Add(this.MailboxAssistantsServiceCouldNotBeStopped(this.ExchangeServer.Name, ex3.Message));
			}
		}

		protected void StartAssistantService(ExchangeServer server, MonitoringData monitoringData)
		{
			try
			{
				using (ServiceController serviceController = new ServiceController("MsExchangeMailboxAssistants", server.Fqdn))
				{
					serviceController.Start();
					serviceController.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromMinutes(5.0));
					monitoringData.Events.Add(new MonitoringEvent(AssistantTroubleshooterBase.EventSource, 5002, EventTypeEnumeration.Information, Strings.MailboxAssistantsServiceStarted(this.ExchangeServer.Name)));
				}
			}
			catch (System.ServiceProcess.TimeoutException ex)
			{
				this.SendWatsonForAssistantProcess(ex, false);
				monitoringData.Events.Add(this.MailboxAssistantsServiceCouldNotBeStarted(this.ExchangeServer.Name, ex.Message));
			}
			catch (InvalidOperationException ex2)
			{
				monitoringData.Events.Add(this.MailboxAssistantsServiceCouldNotBeStarted(this.ExchangeServer.Name, ex2.Message));
			}
			catch (Win32Exception ex3)
			{
				monitoringData.Events.Add(this.MailboxAssistantsServiceCouldNotBeStarted(this.ExchangeServer.Name, ex3.Message));
			}
		}

		protected bool GetWatsonExceptionBucketAndSendWatson(bool killProcess)
		{
			Exception e = new AssistantServiceHungException();
			return this.SendWatsonForAssistantProcess(e, killProcess);
		}

		protected bool SendWatsonForAssistantProcess(Exception e, bool killProcessAfterWatson)
		{
			bool result;
			using (Process mailboxAssistantProcess = this.GetMailboxAssistantProcess(this.ExchangeServer.Fqdn))
			{
				if (mailboxAssistantProcess == null)
				{
					result = false;
				}
				else
				{
					bool flag = false;
					if (this.SendCrashDump)
					{
						this.crashDumpCount++;
						ExWatson.SendHangWatsonReport(e, mailboxAssistantProcess);
						flag = true;
					}
					if (killProcessAfterWatson)
					{
						this.ForceKillAssistantService(mailboxAssistantProcess);
					}
					result = flag;
				}
			}
			return result;
		}

		protected PerformanceCounter GetLocalizedPerformanceCounter(string categoryName, string counterName, string instanceName, string computerName)
		{
			PerformanceCounter performanceCounter = null;
			try
			{
				string localizedPerformanceCounterName = this.GetLocalizedPerformanceCounterName(categoryName, categoryName, computerName);
				string localizedPerformanceCounterName2 = this.GetLocalizedPerformanceCounterName(categoryName, counterName, computerName);
				if (localizedPerformanceCounterName != null && localizedPerformanceCounterName2 != null)
				{
					performanceCounter = new PerformanceCounter(localizedPerformanceCounterName, localizedPerformanceCounterName2, instanceName, computerName);
				}
			}
			catch (UnauthorizedAccessException)
			{
			}
			catch (SecurityException)
			{
			}
			catch (IOException)
			{
			}
			if (performanceCounter == null)
			{
				performanceCounter = new PerformanceCounter(categoryName, counterName, instanceName, computerName);
			}
			return performanceCounter;
		}

		protected Process GetMailboxAssistantProcess(string serverName)
		{
			Process[] processesByName;
			if (string.IsNullOrEmpty(serverName) || serverName.StartsWith(Environment.MachineName + ".", StringComparison.InvariantCultureIgnoreCase) || serverName.Equals(Environment.MachineName, StringComparison.InvariantCultureIgnoreCase))
			{
				processesByName = Process.GetProcessesByName("MsExchangeMailboxAssistants");
			}
			else
			{
				processesByName = Process.GetProcessesByName("MsExchangeMailboxAssistants", serverName);
			}
			if (processesByName != null && processesByName.Length > 0)
			{
				return processesByName[0];
			}
			return null;
		}

		private void ForceKillAssistantService(Process process)
		{
			ManagementObject processObject = WmiWrapper.GetProcessObject(this.ExchangeServer.Fqdn, "MsExchangeMailboxAssistants.exe");
			if (processObject != null)
			{
				uint num = (uint)processObject.InvokeMethod("Terminate", new object[]
				{
					0
				});
				if (num != 0U)
				{
					throw new Win32Exception((int)num, Strings.MailboxAssistantsServiceCouldNotBeKilled(process.MachineName).ToString());
				}
			}
		}

		protected List<MdbStatus> GetOnlineMDBList()
		{
			List<MdbStatus> onlineMDBList;
			using (ExRpcAdmin exRpcAdmin = ExRpcAdmin.Create("Client=Management", this.ExchangeServer.Name, null, null, null))
			{
				onlineMDBList = this.GetOnlineMDBList(exRpcAdmin);
			}
			return onlineMDBList;
		}

		protected List<MdbStatus> GetOnlineMDBList(ExRpcAdmin exrpcAdmin)
		{
			MdbStatus[] array = exrpcAdmin.ListMdbStatus(false);
			List<MdbStatus> list = new List<MdbStatus>(array.Length);
			foreach (MdbStatus mdbStatus in array)
			{
				if ((mdbStatus.Status & MdbStatusFlags.Online) == MdbStatusFlags.Online && StringComparer.OrdinalIgnoreCase.Equals(mdbStatus.VServerName, this.ExchangeServer.Name))
				{
					list.Add(mdbStatus);
				}
			}
			return list;
		}

		private string GetLocalizedPerformanceCounterName(string categoryName, string categoryOrCounterName, string machineName)
		{
			CultureInfo cultureInfo = CultureInfo.InstalledUICulture;
			string name = string.Format("SYSTEM\\CurrentControlSet\\Services\\{0}\\Performance", categoryName);
			int? num = new int?(0);
			string[] array = null;
			string result;
			using (RegistryKey registryKey = RegistryKey.OpenRemoteBaseKey(RegistryHive.LocalMachine, machineName))
			{
				if (registryKey == null)
				{
					result = null;
				}
				else
				{
					using (RegistryKey registryKey2 = registryKey.OpenSubKey("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\"))
					{
						cultureInfo = new CultureInfo((int)registryKey2.GetValue("Server Language", cultureInfo.LCID));
						if (cultureInfo.LCID == 1033)
						{
							return categoryOrCounterName;
						}
					}
					using (RegistryKey registryKey3 = registryKey.OpenSubKey(name))
					{
						if (registryKey3 == null)
						{
							return null;
						}
						string[] array2 = (string[])registryKey3.GetValue("Counter Names", null);
						if (array2 == null)
						{
							return null;
						}
						num = (int?)registryKey3.GetValue("First Counter", null);
						if (num == null)
						{
							return null;
						}
						for (int i = 0; i < array2.Length; i++)
						{
							if (StringComparer.OrdinalIgnoreCase.Equals(array2[i], categoryOrCounterName))
							{
								num += (i + 1) * 2;
								break;
							}
						}
					}
					using (RegistryKey registryKey4 = registryKey.OpenSubKey(this.GetPerformanceCounterLanguageKey(cultureInfo)))
					{
						if (registryKey4 == null)
						{
							return null;
						}
						array = (string[])registryKey4.GetValue("Counter", null);
						if (array == null)
						{
							return null;
						}
					}
					for (int j = 0; j < array.Length; j++)
					{
						if (StringComparer.OrdinalIgnoreCase.Equals(array[j], num.ToString()))
						{
							return array[j + 1];
						}
					}
					result = null;
				}
			}
			return result;
		}

		private string GetPerformanceCounterLanguageKey(CultureInfo cultureInfo)
		{
			int lcid = cultureInfo.LCID;
			int num = lcid & 1023;
			if (num == 4 || num == 22)
			{
				num = lcid;
			}
			return string.Format("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Perflib\\{0}", num.ToString("X3"));
		}

		private MonitoringEvent MailboxAssistantsServiceCouldNotBeStarted(string serverName, string errorMessage)
		{
			return new MonitoringEvent(AssistantTroubleshooterBase.EventSource, 5203, EventTypeEnumeration.Error, Strings.MailboxAssistantsServiceCouldNotBeStarted(serverName, errorMessage));
		}

		private MonitoringEvent MailboxAssistantsServiceCouldNotBeStopped(string serverName, string errorMessage)
		{
			return new MonitoringEvent(AssistantTroubleshooterBase.EventSource, 5202, EventTypeEnumeration.Error, Strings.MailboxAssistantsServiceCouldNotBeStopped(serverName, errorMessage));
		}

		public const string MailboxAssistantServiceName = "MsExchangeMailboxAssistants";

		public const string ExchangeServerPropertyName = "ExchangeServer";

		public const string IncludeCrashDump = "IncludeCrashDump";

		public const string WatsonPerformanceObject = "Watson";

		public const string CrashDumpCountCounter = "CrashDumpCount";

		public static string EventSource = "MSExchange Monitoring MsExchangeMailboxAssistants Troubleshooter";

		private int crashDumpCount;
	}
}
