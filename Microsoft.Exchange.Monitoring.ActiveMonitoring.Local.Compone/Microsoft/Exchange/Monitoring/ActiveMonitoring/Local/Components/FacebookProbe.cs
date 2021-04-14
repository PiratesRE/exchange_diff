using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Web.Script.Serialization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.ApplicationLogic;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;
using Microsoft.Win32;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Local.Components
{
	internal sealed class FacebookProbe : ProbeWorkItem
	{
		protected override void DoWork(CancellationToken cancellationToken)
		{
			if (!PeopleConnectMaintenance.ShouldRun(base.TraceContext))
			{
				base.Result.StateAttribute1 = "Probe not run, since this server is not primary active manager of the DAG";
				WTFDiagnostics.TraceInformation(ExTraceGlobals.PeopleConnectTracer, base.TraceContext, "FacebookProbe.DoWork(): Not run because local server is not PAM of the DAG.", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\peopleconnect\\facebookprobe.cs", 46);
				return;
			}
			if (this.IsGallatin())
			{
				base.Result.StateAttribute1 = "Probe not run, since this server is Gallatin";
				WTFDiagnostics.TraceInformation(ExTraceGlobals.PeopleConnectTracer, base.TraceContext, "FacebookProbe.DoWork(): Not run because local server is Gallatin.", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\peopleconnect\\facebookprobe.cs", 53);
				return;
			}
			HttpWebResponse httpWebResponse = null;
			try
			{
				IPeopleConnectApplicationConfig peopleConnectApplicationConfig = CachedPeopleConnectApplicationConfig.Instance.ReadFacebook();
				PeopleConnectMaintenance.LogApplicationConfig(base.Result, peopleConnectApplicationConfig);
				string endpoint = base.Definition.Endpoint;
				base.Result.StateAttribute2 = "Redirect Url = " + endpoint;
				string text = string.Format("{0}oauth/authorize?client_id={1}&redirect_uri={2}&type=web_server", peopleConnectApplicationConfig.GraphApiEndpoint, peopleConnectApplicationConfig.AppId, endpoint);
				base.Result.StateAttribute3 = "Authorization url = " + text;
				HttpWebRequest httpWebRequest = WebRequest.Create(text) as HttpWebRequest;
				httpWebRequest.UserAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 6.0; Trident/5.0)";
				httpWebRequest.ServicePoint.Expect100Continue = false;
				httpWebResponse = (httpWebRequest.GetResponse() as HttpWebResponse);
			}
			catch (ExchangeConfigurationException ex)
			{
				Exception ex2 = ex.InnerException ?? ex;
				base.Result.StateAttribute1 = ex2.GetType().Name;
				base.Result.Exception = ex2.GetType().Name;
				base.Result.FailureContext = ex2.StackTrace;
				base.Result.Error = ex2.Message;
				throw;
			}
			catch (WebException ex3)
			{
				httpWebResponse = (ex3.Response as HttpWebResponse);
				base.Result.StateAttribute1 = ex3.GetType().Name;
				throw;
			}
			finally
			{
				if (httpWebResponse != null)
				{
					string text2;
					using (StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
					{
						text2 = streamReader.ReadToEnd();
					}
					base.Result.ExecutionContext = text2;
					base.Result.StateAttribute4 = httpWebResponse.StatusCode.ToString();
					if (httpWebResponse.StatusCode == HttpStatusCode.BadRequest && text2.Contains("OAuthException"))
					{
						FacebookProbe.OAuthException ex4 = new JavaScriptSerializer().Deserialize<FacebookProbe.OAuthException>(text2);
						base.Result.StateAttribute1 = ex4.Error.Message;
						base.Result.Error = ex4.Error.Code;
						base.Result.Exception = ex4.Error.Type;
					}
				}
			}
		}

		private bool IsGallatin()
		{
			bool result = false;
			using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\ExchangeLabs"))
			{
				if (registryKey == null || registryKey.GetValue("ServiceName") == null)
				{
					WTFDiagnostics.TraceInformation(ExTraceGlobals.SecurityTracer, base.TraceContext, "Registry does not have ServiceName key.", null, "IsGallatin", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\peopleconnect\\facebookprobe.cs", 175);
				}
				else if (string.Equals(registryKey.GetValue("ServiceName").ToString(), "Gallatin", StringComparison.CurrentCultureIgnoreCase))
				{
					result = true;
				}
			}
			return result;
		}

		private class OAuthException
		{
			public FacebookProbe.OAuthInnerException Error { get; set; }
		}

		private class OAuthInnerException
		{
			public string Message { get; set; }

			public string Type { get; set; }

			public string Code { get; set; }
		}
	}
}
