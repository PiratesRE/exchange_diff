using System;
using System.Diagnostics;
using System.Net;
using System.Net.Security;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.ActiveMonitoring.Responders;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local;
using Microsoft.Exchange.WebServices.Data;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.PublicFolders
{
	internal class PublicFolderLocalEWSLogon : ProbeWorkItem
	{
		public static ProbeDefinition CreateProbeDefinition(MaintenanceDefinition definition, string fqdn, string account, string password)
		{
			ProbeDefinition probeDefinition = new ProbeDefinition();
			PublicFolderLocalEWSLogon.CreateProbeDefinition(probeDefinition, fqdn, account, password);
			probeDefinition.Enabled = bool.Parse(definition.Attributes["LocalEWSLogonProbeEnabled"]);
			probeDefinition.MaxRetryAttempts = int.Parse(definition.Attributes["LocalEWSLogonMaxRetryAttempts"]);
			probeDefinition.RecurrenceIntervalSeconds = (int)TimeSpan.Parse(definition.Attributes["LocalEWSLogonProbeRecurrenceInterval"]).TotalSeconds;
			probeDefinition.TimeoutSeconds = (int)TimeSpan.Parse(definition.Attributes["LocalEWSLogonTimeout"]).TotalSeconds;
			return probeDefinition;
		}

		public static ProbeDefinition CreateProbeDefinition(ProbeDefinition probe, string fqdn, string emailAddress, string password)
		{
			probe.Account = emailAddress;
			probe.AccountPassword = password;
			probe.AssemblyPath = Assembly.GetExecutingAssembly().Location;
			probe.Endpoint = "https://" + Environment.MachineName + "/ews/Exchange.asmx";
			probe.Name = PublicFolderLocalEWSLogon.Name + "Probe";
			probe.ServiceName = ExchangeComponent.PublicFolders.Name;
			probe.TargetResource = fqdn;
			probe.TypeName = typeof(PublicFolderLocalEWSLogon).FullName;
			return probe;
		}

		protected override void DoWork(CancellationToken cancellationToken)
		{
			NetworkCredential networkCredential = new NetworkCredential(base.Definition.Account, base.Definition.AccountPassword);
			ExchangeService exchangeService = new ExchangeService();
			exchangeService.Credentials = networkCredential;
			exchangeService.Url = new Uri(base.Definition.Endpoint);
			ServicePointManager.ServerCertificateValidationCallback = ((object param0, X509Certificate param1, X509Chain param2, SslPolicyErrors param3) => true);
			base.Result.StateAttribute2 = base.Definition.Account;
			base.Result.StateAttribute3 = exchangeService.Url.ToString();
			this.Logon(exchangeService);
		}

		internal void Logon(ExchangeService exchService)
		{
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			Folder folder = Folder.Bind(exchService, 11, new PropertySet(1, new PropertyDefinitionBase[]
			{
				FolderSchema.Permissions
			}));
			try
			{
				folder = Folder.Bind(exchService, 11, new PropertySet(1, new PropertyDefinitionBase[]
				{
					FolderSchema.Permissions
				}));
				Folder folder2 = null;
				bool flag = false;
				SearchFilter searchFilter = new SearchFilter.IsGreaterThan(FolderSchema.DisplayName, PublicFolderLocalEWSLogon.PFID);
				FindFoldersResults findFoldersResults = folder.FindFolders(searchFilter, new FolderView(int.MaxValue));
				for (int i = 0; i < findFoldersResults.TotalCount; i++)
				{
					folder2 = findFoldersResults.Folders[i];
					if (folder2.DisplayName.Contains(PublicFolderLocalEWSLogon.PFID))
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					WTFDiagnostics.TraceWarning<string>(ExTraceGlobals.PublicFoldersTracer, base.Definition.TraceContext, "Monitoring public folder not found on server {0}.", Environment.MachineName, null, "Logon", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\PublicFolders\\PublicFolderLocalEWSLogon.cs", 143);
				}
				else
				{
					PostItem postItem = new PostItem(exchService);
					postItem.Subject = PublicFolderLocalEWSLogon.Name;
					WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.PublicFoldersTracer, base.Definition.TraceContext, "Posting message on public folder: {0}...", folder2.DisplayName, null, "Logon", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\PublicFolders\\PublicFolderLocalEWSLogon.cs", 153);
					postItem.Save(folder2.Id);
					try
					{
						postItem.Delete(0);
					}
					catch
					{
						WTFDiagnostics.TraceError<string>(ExTraceGlobals.PublicFoldersTracer, base.Definition.TraceContext, "The post message on public folder: {0} could not be deleted.", folder2.DisplayName, null, "Logon", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\PublicFolders\\PublicFolderLocalEWSLogon.cs", 164);
					}
				}
			}
			catch (Exception ex)
			{
				if (!ex.ToString().ToLower().Contains(PublicFolderLocalEWSLogon.NoPFMbxFound))
				{
					throw;
				}
				base.Result.StateAttribute4 = "Skipping server.";
			}
			finally
			{
				stopwatch.Stop();
				base.Result.SampleValue = (double)stopwatch.ElapsedMilliseconds;
				base.Result.StateAttribute1 = base.Result.SampleValue.ToString();
			}
		}

		public static MonitorDefinition CreateMonitorDefinition(MaintenanceDefinition definition, string fqdn, string probeResult)
		{
			MonitorDefinition monitorDefinition = OverallXFailuresMonitor.CreateDefinition(PublicFolderLocalEWSLogon.Name + "Monitor", probeResult, ExchangeComponent.PublicFolders.Name, ExchangeComponent.PublicFolders, (int)TimeSpan.Parse(definition.Attributes["LocalEWSLogonMonitorInterval"]).TotalSeconds, (int)TimeSpan.Parse(definition.Attributes["LocalEWSLogonMonitorRecurranceInterval"]).TotalSeconds, int.Parse(definition.Attributes["LocalEWSLogonMonitorFailedProbeThreshold"]), true);
			monitorDefinition.Enabled = bool.Parse(definition.Attributes["LocalEWSLogonMonitorEnabled"]);
			monitorDefinition.MaxRetryAttempts = int.Parse(definition.Attributes["LocalEWSLogonMaxRetryAttempts"]);
			monitorDefinition.TargetResource = fqdn;
			monitorDefinition.TimeoutSeconds = (int)TimeSpan.Parse(definition.Attributes["LocalEWSLogonTimeout"]).TotalSeconds;
			monitorDefinition.MonitorStateTransitions = new MonitorStateTransition[]
			{
				new MonitorStateTransition(ServiceHealthStatus.Unrecoverable, (int)TimeSpan.Parse(definition.Attributes["LocalEWSLogonResponderTimeToEscalate"]).TotalSeconds)
			};
			monitorDefinition.ServicePriority = 2;
			monitorDefinition.ScenarioDescription = "Validate PublicFolder health is not impacted by EWS logon issues";
			return monitorDefinition;
		}

		public static ResponderDefinition CreateEscalateResponderDefinition(MaintenanceDefinition definition, string fqdn, string monitorResult)
		{
			string escalationMessageUnhealthy = Strings.PublicFolderLocalEWSLogonEscalationMessage;
			string escalationSubjectUnhealthy = Strings.PublicFolderLocalEWSLogonEscalationSubject;
			bool flag = bool.Parse(definition.Attributes["LocalEWSLogonResponderEnabledPagedAlerts"]);
			string text = PublicFolderLocalEWSLogon.Name + "Escalate";
			ResponderDefinition responderDefinition = EscalateResponder.CreateDefinition(text, ExchangeComponent.PublicFolders.Name, text, monitorResult, fqdn, ServiceHealthStatus.Unrecoverable, ExchangeComponent.PublicFolders.EscalationTeam, escalationSubjectUnhealthy, escalationMessageUnhealthy, true, NotificationServiceClass.Urgent, 14400, "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59", false);
			responderDefinition.Enabled = bool.Parse(definition.Attributes["LocalEWSLogonResponderEnabled"]);
			responderDefinition.NotificationServiceClass = (flag ? NotificationServiceClass.Urgent : NotificationServiceClass.UrgentInTraining);
			responderDefinition.AssemblyPath = Assembly.GetExecutingAssembly().Location;
			responderDefinition.MinimumSecondsBetweenEscalates = (int)TimeSpan.Parse(definition.Attributes["LocalEWSLogonResponderTimeBetweenEscalates"]).TotalSeconds;
			responderDefinition.RecurrenceIntervalSeconds = (int)TimeSpan.Parse(definition.Attributes["LocalEWSLogonResponderRecurrenceInterval"]).TotalSeconds;
			return responderDefinition;
		}

		internal static readonly string Name = "PublicFolderLocalEWSLogon";

		internal static readonly string PFID = "Folder_PFM_";

		internal static readonly string NoPFMbxFound = "there are no public folder servers available";
	}
}
