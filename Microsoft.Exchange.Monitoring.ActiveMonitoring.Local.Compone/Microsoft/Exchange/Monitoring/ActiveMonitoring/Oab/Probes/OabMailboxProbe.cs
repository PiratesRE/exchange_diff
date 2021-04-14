using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Exchange.Data.ApplicationLogic.Cafe;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local;
using Microsoft.Exchange.OAB;
using Microsoft.Office.Datacenter.WorkerTaskFramework;
using Microsoft.Win32;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Oab.Probes
{
	public sealed class OabMailboxProbe : OabBaseLocalProbe
	{
		protected override void DoWork(CancellationToken cancellationToken)
		{
			if (this.ShouldRun())
			{
				try
				{
					HttpWebRequest request = base.GetRequest();
					WTFDiagnostics.TraceInformation(ExTraceGlobals.OABTracer, base.TraceContext, "OabMailboxProbe:Sending request to OAB " + base.Definition.TargetResource, null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Oab\\OabMailboxProbe.cs", 67);
					Task<WebResponse> task = base.WebRequestUtil.SendRequest(request);
					HttpWebResponse response = (HttpWebResponse)task.Result;
					OABManifest oabManifest = this.CheckManifestFile(response);
					this.VerifyOABFiles(oabManifest);
				}
				catch (AggregateException ex)
				{
					WebException ex2 = ex.Flatten().InnerException as WebException;
					HttpWebResponse httpWebResponse = (HttpWebResponse)ex2.Response;
					base.Result.StateAttribute5 = httpWebResponse.StatusCode.ToString();
					throw ex2;
				}
			}
		}

		private static string InitializeOABFolderPath()
		{
			string text = null;
			using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("System\\CurrentControlSet\\Services\\MSExchangeMailboxAssistants\\Parameters"))
			{
				text = (registryKey.GetValue("OABGeneratorPath") as string);
			}
			if (text == null || !Directory.Exists(text))
			{
				using (RegistryKey registryKey2 = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Setup"))
				{
					text = (registryKey2.GetValue("MsiInstallPath") as string);
				}
				if (text == null)
				{
					using (Process currentProcess = Process.GetCurrentProcess())
					{
						text = currentProcess.MainModule.FileName;
						text = Directory.GetParent(text).FullName;
						DirectoryInfo parent = Directory.GetParent(text);
						text = parent.FullName;
					}
				}
				text = Path.Combine(text, "ClientAccess\\OAB");
			}
			return text;
		}

		private void VerifyOABFiles(OABManifest oabManifest)
		{
			string targetResource = base.Definition.TargetResource;
			foreach (OABManifestAddressList oabmanifestAddressList in oabManifest.AddressLists)
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.OABTracer, base.TraceContext, "Verifying OAB files for " + oabmanifestAddressList.DN, null, "VerifyOABFiles", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Oab\\OabMailboxProbe.cs", 143);
				foreach (OABManifestFile oabmanifestFile in oabmanifestAddressList.Files)
				{
					string text = string.Format("{0}\\{1}\\{2}", OabMailboxProbe.oabFolderPath, targetResource, oabmanifestFile.FileName);
					WTFDiagnostics.TraceInformation(ExTraceGlobals.OABTracer, base.TraceContext, "Verifying file " + text, null, "VerifyOABFiles", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Oab\\OabMailboxProbe.cs", 147);
					if (!File.Exists(text))
					{
						throw new FileNotFoundException(Strings.OabMailboxFileNotFound(text), text);
					}
				}
			}
		}

		private OABManifest CheckManifestFile(HttpWebResponse response)
		{
			string responseBody = this.GetResponseBody(response);
			if (responseBody.Length <= 0)
			{
				throw new InvalidDataException(Strings.OabMailboxManifestEmpty);
			}
			OABManifest oabmanifest = OABManifest.Deserialize(new MemoryStream(Encoding.UTF8.GetBytes(responseBody)));
			if (oabmanifest.AddressLists == null || oabmanifest.AddressLists.Length == 0)
			{
				throw new InvalidDataException(Strings.OabMailboxManifestAddressListEmpty(string.Empty));
			}
			foreach (OABManifestAddressList oabmanifestAddressList in oabmanifest.AddressLists)
			{
				if (oabmanifestAddressList.Files == null || oabmanifestAddressList.Files.Length == 0)
				{
					throw new InvalidDataException(Strings.OabMailboxManifestAddressListEmpty(oabmanifestAddressList.Name));
				}
			}
			return oabmanifest;
		}

		private string GetResponseBody(HttpWebResponse response)
		{
			base.Result.FailureContext = response.ResponseUri.ToString();
			string httpResponseBody = base.WebRequestUtil.GetHttpResponseBody(response);
			if (HttpStatusCode.OK != response.StatusCode)
			{
				string message = string.Format("{0}:{1}", response.StatusCode, httpResponseBody);
				throw new WebException(message);
			}
			string text = response.Headers["X-DiagInfo"];
			if (!string.IsNullOrEmpty(text))
			{
				base.Result.ExecutionContext = text;
			}
			return httpResponseBody;
		}

		private bool ShouldRun()
		{
			string text = base.Definition.Attributes["OrgMbxDBId"];
			string[] array = text.Split(new string[]
			{
				","
			}, StringSplitOptions.RemoveEmptyEntries);
			foreach (string g in array)
			{
				Guid databaseGuid = new Guid(g);
				using (MailboxServerLocator mailboxServerLocator = MailboxServerLocator.CreateWithResourceForestFqdn(databaseGuid, null))
				{
					BackEndServer server = mailboxServerLocator.GetServer();
					if (server.Fqdn.Contains(Environment.MachineName))
					{
						return true;
					}
				}
			}
			return false;
		}

		public const string OabOrgMailboxDatabaseId = "OrgMbxDBId";

		public const string DatabaseListSeperator = ",";

		private const string OABDirectoryPartialPath = "ClientAccess\\OAB";

		private static readonly string oabFolderPath = OabMailboxProbe.InitializeOABFolderPath();
	}
}
