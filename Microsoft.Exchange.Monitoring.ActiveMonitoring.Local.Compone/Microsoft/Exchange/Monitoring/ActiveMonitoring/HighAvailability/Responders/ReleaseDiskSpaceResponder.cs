using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Reflection;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Exchange.VariantConfiguration;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.Monitoring.ActiveMonitoring.Recovery;
using Microsoft.Office.Datacenter.WorkerTaskFramework;
using Microsoft.Win32;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.HighAvailability.Responders
{
	public class ReleaseDiskSpaceResponder : ResponderWorkItem
	{
		internal static ResponderDefinition CreateDefinition(string responderName, string serviceName, string alertTypeId, string alertMask, string targetResource, ServiceHealthStatus responderTargetState, string driveLetter)
		{
			ResponderDefinition responderDefinition = new ResponderDefinition();
			responderDefinition.AssemblyPath = ReleaseDiskSpaceResponder.AssemblyPath;
			responderDefinition.TypeName = ReleaseDiskSpaceResponder.TypeName;
			responderDefinition.Name = responderName;
			responderDefinition.ServiceName = serviceName;
			responderDefinition.AlertTypeId = alertTypeId;
			responderDefinition.AlertMask = alertTypeId;
			responderDefinition.TargetResource = targetResource;
			responderDefinition.RecurrenceIntervalSeconds = 300;
			responderDefinition.TimeoutSeconds = 300;
			responderDefinition.MaxRetryAttempts = 3;
			responderDefinition.TargetHealthState = responderTargetState;
			responderDefinition.WaitIntervalSeconds = 30;
			responderDefinition.Enabled = true;
			responderDefinition.Attributes["DiskDriverLetter"] = driveLetter;
			return responderDefinition;
		}

		protected virtual void InitializeAttributes(AttributeHelper attributeHelper = null)
		{
			if (attributeHelper == null)
			{
				attributeHelper = new AttributeHelper(base.Definition);
			}
			this.diskDriverLetter = attributeHelper.GetString("DiskDriverLetter", false, null);
		}

		protected override void DoResponderWork(CancellationToken cancellationToken)
		{
			base.Result.StateAttribute1 = "DoResponderWork for ReleaseDiskSpace";
			this.InitializeAttributes(null);
			if (!base.Broker.IsLocal() || !VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).ActiveMonitoring.EscalateResponder.Enabled)
			{
				base.Result.StateAttribute1 = "Responder not running in Datacenter environment, ignored.";
				return;
			}
			WTFDiagnostics.TraceDebug(ExTraceGlobals.CommonComponentsTracer, base.TraceContext, "ReleaseDiskSpaceResponder.DoWork: Start.", null, "DoResponderWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\HighAvailability\\Responders\\ReleaseDiskSpaceResponder.cs", 139);
			List<ReleaseDiskSpaceResponder.CandidateUnleashFolder> list = new List<ReleaseDiskSpaceResponder.CandidateUnleashFolder>();
			List<ReleaseDiskSpaceResponder.CandidateUnleashFolder> list2 = new List<ReleaseDiskSpaceResponder.CandidateUnleashFolder>();
			int num = 0;
			double num2 = 0.0;
			string stateAttribute = null;
			foreach (ReleaseDiskSpaceResponder.CandidateUnleashFolder candidateUnleashFolder in ReleaseDiskSpaceResponder.candidateUnleashFolders)
			{
				if (!Directory.Exists(candidateUnleashFolder.Path))
				{
					WTFDiagnostics.TraceDebug<string>(ExTraceGlobals.CommonComponentsTracer, base.TraceContext, "Folder {0} has not been found, skip it.", candidateUnleashFolder.Path, null, "DoResponderWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\HighAvailability\\Responders\\ReleaseDiskSpaceResponder.cs", 155);
					list.Add(candidateUnleashFolder);
				}
				else if (!candidateUnleashFolder.Path.StartsWith(this.diskDriverLetter, StringComparison.OrdinalIgnoreCase))
				{
					WTFDiagnostics.TraceDebug<string, string>(ExTraceGlobals.CommonComponentsTracer, base.TraceContext, "Folder {0} is not under disk {1}, skip it.", candidateUnleashFolder.Path, this.diskDriverLetter, null, "DoResponderWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\HighAvailability\\Responders\\ReleaseDiskSpaceResponder.cs", 167);
					list.Add(candidateUnleashFolder);
				}
				else
				{
					string[] files = Directory.GetFiles(candidateUnleashFolder.Path, candidateUnleashFolder.Pattern, candidateUnleashFolder.IsRecursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
					DateTime t = DateTime.UtcNow.AddDays((double)(-(double)candidateUnleashFolder.KeepForDays));
					list2.Add(candidateUnleashFolder);
					WTFDiagnostics.TraceDebug<string, int>(ExTraceGlobals.CommonComponentsTracer, base.TraceContext, "Releasing Folder [{0}]. Totally [{1}] has been found.", candidateUnleashFolder.Path, files.Length, null, "DoResponderWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\HighAvailability\\Responders\\ReleaseDiskSpaceResponder.cs", 182);
					candidateUnleashFolder.DeleteFileCount = 0;
					candidateUnleashFolder.UnleashedSizeInMB = 0.0;
					foreach (string text in files)
					{
						try
						{
							FileInfo fileInfo = new FileInfo(text);
							double num3 = (double)(fileInfo.Length / 1000000L);
							if (!(fileInfo.LastWriteTimeUtc >= t))
							{
								File.Delete(text);
								WTFDiagnostics.TraceDebug<string>(ExTraceGlobals.CommonComponentsTracer, base.TraceContext, "Successfully deleted file [{0}].", text, null, "DoResponderWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\HighAvailability\\Responders\\ReleaseDiskSpaceResponder.cs", 208);
								candidateUnleashFolder.DeleteFileCount++;
								candidateUnleashFolder.UnleashedSizeInMB += num3;
							}
						}
						catch (Exception ex)
						{
							WTFDiagnostics.TraceDebug<string, string>(ExTraceGlobals.CommonComponentsTracer, base.TraceContext, "Failed to delete file [{0}], Error {1}", text, ex.Message, null, "DoResponderWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\HighAvailability\\Responders\\ReleaseDiskSpaceResponder.cs", 220);
							stateAttribute = ex.Message;
						}
					}
					num += candidateUnleashFolder.DeleteFileCount;
					num2 += candidateUnleashFolder.UnleashedSizeInMB;
				}
			}
			base.Result.StateAttribute1 = string.Format("Responder finished. Totally [{0}] files deleted, [{1}] MB spaces unleashed.", num, (int)num2);
			base.Result.StateAttribute2 = string.Format("Defined Candidate Folders: {0}", string.Concat<ReleaseDiskSpaceResponder.CandidateUnleashFolder>(ReleaseDiskSpaceResponder.candidateUnleashFolders));
			base.Result.StateAttribute3 = string.Format("Skipped Folders: {0}", string.Concat(from folder in list
			select folder.Path));
			base.Result.StateAttribute4 = stateAttribute;
			string[] headers = new string[]
			{
				"Directory Path",
				"File Pattern",
				"Is Recursive",
				"Retention Days",
				"Deleted Files Count",
				"Unleashed Size In MB"
			};
			List<string[]> list3 = new List<string[]>();
			foreach (ReleaseDiskSpaceResponder.CandidateUnleashFolder candidateUnleashFolder2 in ReleaseDiskSpaceResponder.candidateUnleashFolders)
			{
				string[] item = new string[]
				{
					candidateUnleashFolder2.Path,
					candidateUnleashFolder2.Pattern,
					candidateUnleashFolder2.IsRecursive.ToString(),
					candidateUnleashFolder2.KeepForDays.ToString(),
					candidateUnleashFolder2.DeleteFileCount.ToString(),
					candidateUnleashFolder2.UnleashedSizeInMB.ToString()
				};
				list3.Add(item);
			}
			string toAddresses = "obdateam@microsoft.com";
			string ccAddresses = "yisui@microsoft.com;xfyan@microsoft.com";
			string subject = string.Format("SystemDrive DiskSpace Released on Machine [{0}]. Deleted Files=[{1}]. Unleashed Space (MB)=[{2}]", base.Result.MachineName, num, (int)num2);
			string body = "<body>" + HtmlHelper.CreateTable(headers, list3.ToArray()) + "</body>";
			this.SendEmail("OBD\\LocalActiveMonitoring", toAddresses, ccAddresses, subject, body);
			WTFDiagnostics.TraceDebug(ExTraceGlobals.CommonComponentsTracer, base.TraceContext, "ReleaseDiskSpaceResponder.DoWork: Finished.", null, "DoResponderWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\HighAvailability\\Responders\\ReleaseDiskSpaceResponder.cs", 258);
		}

		private void SendEmail(string alertSource, string toAddresses, string ccAddresses, string subject, string body)
		{
			string text = null;
			string text2 = null;
			using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(ReleaseDiskSpaceResponder.ActiveMonitoringRegistryPath, false))
			{
				if (registryKey != null)
				{
					text2 = (string)registryKey.GetValue("RPSCertificateSubject", null);
					text = (string)registryKey.GetValue("RPSEndpoint", null);
				}
			}
			RemotePowerShell remotePowerShell = null;
			string text3;
			if ((text3 = base.Definition.Endpoint) == null)
			{
				text3 = (text ?? "https://exchangelabs.live-int.com/Powershell");
			}
			text = text3;
			if (!string.IsNullOrWhiteSpace(base.Definition.AccountPassword))
			{
				remotePowerShell = RemotePowerShell.CreateRemotePowerShellByCredential(new Uri(text), base.Definition.Account, base.Definition.AccountPassword, false);
			}
			else
			{
				remotePowerShell = RemotePowerShell.CreateRemotePowerShellByCertificate(new Uri(text), base.Definition.Account ?? text2, true);
			}
			PSCommand pscommand = new PSCommand();
			pscommand.AddCommand("New-OnCallEmail");
			pscommand.AddParameter("Subject", subject);
			pscommand.AddParameter("ToAddresses", toAddresses);
			pscommand.AddParameter("CCAddresses", ccAddresses);
			pscommand.AddParameter("FromAddress", "om-notification@microsoft.com");
			pscommand.AddParameter("Source", alertSource);
			pscommand.AddParameter("Body", body);
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(pscommand.Commands[0].CommandText);
			foreach (CommandParameter commandParameter in pscommand.Commands[0].Parameters)
			{
				stringBuilder.AppendFormat(" -{0}:{1}", commandParameter.Name, commandParameter.Value.ToString());
			}
			WTFDiagnostics.TraceDebug<string>(ExTraceGlobals.CommonComponentsTracer, base.TraceContext, "ReleaseDiskSpaceResponder: Emailing job status summary  via command '{0}'...", stringBuilder.ToString(), null, "SendEmail", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\HighAvailability\\Responders\\ReleaseDiskSpaceResponder.cs", 323);
			try
			{
				remotePowerShell.InvokePSCommand(pscommand);
				base.Result.StateAttribute5 = string.Format("Email has sent to ToAddress=[{0}] CcAddress=[{1}].", toAddresses, ccAddresses);
				WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.CommonComponentsTracer, base.TraceContext, "ReleaseDiskSpaceResponder.SendEmail: Successfully emailed job status summary '{0}'.", stringBuilder.ToString(), null, "SendEmail", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\HighAvailability\\Responders\\ReleaseDiskSpaceResponder.cs", 329);
			}
			catch (Exception ex)
			{
				string text4 = string.Format("Unexpected failure with command '{0}': {1}", stringBuilder.ToString().Substring(0, 255), ex.ToString());
				base.Result.StateAttribute5 = text4;
				WTFDiagnostics.TraceWarning(ExTraceGlobals.CommonComponentsTracer, base.TraceContext, "ReleaseDiskSpaceResponder.SendEmail: " + text4, null, "SendEmail", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\HighAvailability\\Responders\\ReleaseDiskSpaceResponder.cs", 335);
			}
		}

		private static readonly string AssemblyPath = Assembly.GetExecutingAssembly().Location;

		private static readonly string TypeName = typeof(ReleaseDiskSpaceResponder).FullName;

		private static readonly string ActiveMonitoringRegistryPath = "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\ActiveMonitoring\\";

		private static List<ReleaseDiskSpaceResponder.CandidateUnleashFolder> candidateUnleashFolders = new List<ReleaseDiskSpaceResponder.CandidateUnleashFolder>
		{
			new ReleaseDiskSpaceResponder.CandidateUnleashFolder
			{
				Path = "C:\\Datamining\\Logs\\ADDriverLogs",
				Pattern = "*.*",
				IsRecursive = false,
				KeepForDays = 2
			},
			new ReleaseDiskSpaceResponder.CandidateUnleashFolder
			{
				Path = "C:\\Program Files\\Microsoft\\Exchange Server\\V15\\Logging\\Store",
				Pattern = "*.*",
				IsRecursive = true,
				KeepForDays = 7
			},
			new ReleaseDiskSpaceResponder.CandidateUnleashFolder
			{
				Path = "C:\\Program Files\\Microsoft\\Datacenter\\Datamining\\CsLogs\\local",
				Pattern = "*.*",
				IsRecursive = true,
				KeepForDays = 2
			},
			new ReleaseDiskSpaceResponder.CandidateUnleashFolder
			{
				Path = "D:\\Datamining\\Logs\\EDSCertificateLogs",
				Pattern = "*.log",
				IsRecursive = false,
				KeepForDays = 7
			}
		};

		private string diskDriverLetter;

		private class CandidateUnleashFolder
		{
			public override string ToString()
			{
				return string.Format("Folder=[Path={0}][{1}][Recursive={2}][Retention={3}][SizeInMB={4}][Count={5}]", new object[]
				{
					this.Path,
					this.Pattern,
					this.IsRecursive,
					this.KeepForDays,
					(int)this.UnleashedSizeInMB,
					this.DeleteFileCount
				});
			}

			public string Path;

			public string Pattern;

			public bool IsRecursive;

			public int KeepForDays;

			public double UnleashedSizeInMB;

			public int DeleteFileCount;
		}
	}
}
