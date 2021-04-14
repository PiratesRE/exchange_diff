using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Directory.Probes
{
	public class OfflineGLSProbe : ProbeWorkItem
	{
		public override void PopulateDefinition<ProbeDefinition>(ProbeDefinition pDef, Dictionary<string, string> propertyBag)
		{
		}

		protected override void DoWork(CancellationToken cancellationToken)
		{
			DirectoryUtils.Logger(this, StxLogType.TestOfflineGLS, delegate
			{
				bool flag = false;
				WTFDiagnostics.TraceInformation(ExTraceGlobals.DirectoryTracer, base.TraceContext, "Starting Offline GLS test against local server", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Directory\\OfflineGLSProbe.cs", 72);
				string text;
				string value;
				string text2;
				if (!DirectoryUtils.GetCredentials(out text, out value, out text2, this))
				{
					base.Result.StateAttribute1 = "No Monitoring users";
					return;
				}
				base.Result.StateAttribute3 = text;
				if (!string.IsNullOrEmpty(text) && SmtpAddress.IsValidSmtpAddress(text) && !string.IsNullOrEmpty(value) && !string.IsNullOrEmpty(text2))
				{
					bool flag2 = false;
					try
					{
						GlsDirectorySession glsDirectorySession = new GlsDirectorySession();
						string text3;
						flag2 = glsDirectorySession.TryGetRedirectServer(text, GlsCacheServiceMode.CacheOnly, out text3);
					}
					catch (Exception ex)
					{
						string text4 = string.Format("Exception occured when user {0} getting redirect server in offline GLS. Offline GLS file info obtained as {1}. Exception is {2}.", text, this.GetOfflineGLSFileInfo(), ex.ToString());
						base.Result.Error = text4;
						throw new Exception(text4);
					}
					if (!flag2)
					{
						base.Result.StateAttribute1 = "Domain not found";
					}
					else
					{
						base.Result.StateAttribute1 = "Domain Found";
						flag = true;
					}
					WTFDiagnostics.TraceInformation<bool, double, string, string>(ExTraceGlobals.DirectoryTracer, base.TraceContext, "Operation succeeded: {0} Time Taken {1} Output {2} Error{3}", flag, base.Result.SampleValue, base.Result.StateAttribute1, base.Result.Error, null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Directory\\OfflineGLSProbe.cs", 115);
					if (!flag)
					{
						string text4 = string.Format("GLS does not find domain {0} for user {1}. Offline GLS file info obtained as {2}", text2, text, this.GetOfflineGLSFileInfo());
						base.Result.Error = text4;
						throw new Exception(text4);
					}
				}
				else
				{
					base.Result.StateAttribute1 = "Empty or invalid Monitoring user, check StateAttribute3 for username";
				}
			});
		}

		public string GetOfflineGLSFileInfo()
		{
			string value = string.Empty;
			value = DirectoryUtils.GetExchangeBinPath();
			string result = string.Empty;
			if (!string.IsNullOrEmpty(value))
			{
				string text = Path.Combine(DirectoryUtils.GetExchangeBinPath(), "OfflineGlobalLocatorServiceData.csv");
				if (File.Exists(text))
				{
					FileInfo fileInfo = new FileInfo(text);
					result = string.Format("Offline GLS file {0}, created time {1}, size {2} bytes.", text, File.GetCreationTime(text).ToString(), fileInfo.Length);
				}
				else
				{
					result = string.Format("Could not find expected Offline GLS file {0}", text);
				}
			}
			else
			{
				result = "Could not find Exchange Bin Folder on local server";
			}
			return result;
		}

		private const string GlsCacheEndpoint = "GlsCacheService";

		private const string OfflineGlsUrl = "net.pipe://localhost/GlsCacheService/service.svc";

		private const string GlobalLocatorCacheFile = "OfflineGlobalLocatorServiceData.csv";
	}
}
