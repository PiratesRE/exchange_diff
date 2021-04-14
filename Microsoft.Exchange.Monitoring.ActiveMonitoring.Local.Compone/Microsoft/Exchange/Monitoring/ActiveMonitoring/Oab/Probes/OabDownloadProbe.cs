using System;
using System.Collections;
using System.Management.Automation;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.Exchange.Compliance.Xml;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Ews.Probes;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Oab.Probes
{
	public class OabDownloadProbe : AutodiscoverCommon
	{
		private string ExternalOABUrl { get; set; }

		private string RemotePowershellEndpoint { get; set; }

		private HttpWebRequestUtility WebRequest { get; set; }

		protected override void DoWork(CancellationToken cancellationToken)
		{
			base.Initialize(ExTraceGlobals.OABTracer);
			this.cookieContainer = new CookieContainer();
			base.ExchangeServerVersion = 0;
			base.AutodiscoverEndpoint = base.Definition.Endpoint;
			base.LookupUsername = base.Definition.Account;
			bool flag = false;
			if (base.Definition.Attributes != null)
			{
				if (base.Definition.Attributes.ContainsKey("UseBasicAuthentication"))
				{
					bool.TryParse(base.Definition.Attributes["UseBasicAuthentication"], out this.useBasicAuthentication);
				}
				if (base.Definition.Attributes.ContainsKey("ExchangeVersion"))
				{
					string text = base.Definition.Attributes["ExchangeVersion"];
					if (!string.IsNullOrEmpty(text))
					{
						this.targetMailboxVersion = int.Parse(text);
						base.Result.StateAttribute1 = text;
					}
				}
				if (base.Definition.Attributes.ContainsKey("UseTargetedOrgMailbox"))
				{
					flag = true;
				}
				if (base.Definition.Attributes.ContainsKey("ExternalOABUrl"))
				{
					this.ExternalOABUrl = base.Definition.Attributes["ExternalOABUrl"];
				}
				if (base.Definition.Attributes.ContainsKey("RemotePowershellEndpoint"))
				{
					this.RemotePowershellEndpoint = base.Definition.Attributes["RemotePowershellEndpoint"];
				}
			}
			WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.OABTracer, base.TraceContext, "Starting OabDownloadProbe with Username: {0} ", base.LookupUsername, null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Oab\\OabDownloadProbe.cs", 193);
			DateTime utcNow = DateTime.UtcNow;
			try
			{
				string oabmanifestFile = this.GetOABManifestFile();
				WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.OABTracer, base.TraceContext, "Starting a request for the manifest file on {0} ", oabmanifestFile, null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Oab\\OabDownloadProbe.cs", 202);
				this.WebRequest = new HttpWebRequestUtility(base.TraceContext);
				HttpWebRequest httpWebRequest = this.WebRequest.PrepareRequest(oabmanifestFile, true, base.Definition.Account, base.Definition.AccountPassword, this.useBasicAuthentication);
				httpWebRequest.CookieContainer = this.cookieContainer;
				if (this.targetMailboxVersion != 0 && this.targetMailboxVersion.CompareTo((int)ExchangeObjectVersion.Exchange2012.ExchangeBuild.Major) >= 0)
				{
					this.outdatedThreshold = OabDownloadProbe.MaxE15OutdatedThreshold;
					if (flag)
					{
						httpWebRequest.Headers.Add("TargetOrgMailbox", "Org_" + base.Definition.Account);
					}
				}
				else
				{
					this.outdatedThreshold = OabDownloadProbe.MaxE14OutdatedThreshold;
				}
				HttpWebResponse response = (HttpWebResponse)httpWebRequest.GetResponse();
				string fullOABPath = this.GetFullOABPath(response);
				Task<WebResponse> task = this.StartFullOABDownload(httpWebRequest.Headers, fullOABPath);
				task.Continue(new Func<Task<WebResponse>, string>(this.ValidateOABFile), cancellationToken, TaskContinuationOptions.AttachedToParent | TaskContinuationOptions.NotOnFaulted | TaskContinuationOptions.NotOnCanceled);
				task.ContinueWith(new Action<Task<WebResponse>>(this.HandleException), TaskContinuationOptions.AttachedToParent | TaskContinuationOptions.NotOnRanToCompletion | TaskContinuationOptions.NotOnCanceled);
				if (!string.IsNullOrEmpty(this.RemotePowershellEndpoint))
				{
					this.UpdateUserAttribute();
				}
			}
			catch (WebException ex)
			{
				if (ex.Response == null || ((HttpWebResponse)ex.Response).StatusCode != HttpStatusCode.NotFound)
				{
					this.LogException(ex);
					throw;
				}
				base.Result.ExecutionContext = "Server didn't have the test tenant OAB folder. This is a valid scenario during upgrades.";
			}
			catch (Exception e)
			{
				if (string.IsNullOrEmpty(base.Result.ExecutionContext))
				{
					this.LogException(e);
				}
				throw;
			}
			finally
			{
				base.Result.SampleValue = (double)((int)(DateTime.UtcNow - utcNow).TotalMilliseconds);
				WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.OABTracer, base.TraceContext, "OabDownloadProbe finished with Username: {0}", base.LookupUsername, null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Oab\\OabDownloadProbe.cs", 276);
			}
		}

		private string GetOABManifestFile()
		{
			if (string.IsNullOrEmpty(this.ExternalOABUrl))
			{
				EWSCommon.TraceListener traceListener = new EWSCommon.TraceListener(base.TraceContext, ExTraceGlobals.OABTracer);
				base.GetAutodiscoverService(base.Definition.Account, base.Definition.AccountPassword, base.ExchangeServerVersion, base.AutodiscoverEndpoint, traceListener);
				base.GetUserSettings(new ArrayList
				{
					60
				}, base.LookupUsername);
				object obj = null;
				if (!base.UserSettings.Settings.TryGetValue(60, out obj))
				{
					throw new AutodiscoverOabUrlNotFound(base.Definition.Account);
				}
				this.ExternalOABUrl = obj.ToString();
			}
			return this.ExternalOABUrl + "oab.xml";
		}

		private string GetFullOABPath(HttpWebResponse response)
		{
			string responseBody = this.GetResponseBody(response);
			SafeXmlDocument safeXmlDocument = new SafeXmlDocument();
			safeXmlDocument.LoadXml(responseBody);
			string result;
			using (XmlNodeList elementsByTagName = safeXmlDocument.GetElementsByTagName("Full"))
			{
				using (XmlNodeList elementsByTagName2 = safeXmlDocument.GetElementsByTagName("Template"))
				{
					if (elementsByTagName.Count != 1 || elementsByTagName2.Count != 100)
					{
						string message = string.Format("Expected a single full OAB element and {0} templates, but found {1} full OAB and {2} templates.", 100, elementsByTagName.Count, elementsByTagName2.Count);
						throw new InvalidOabManifestFileException(base.Definition.Account, message);
					}
					if (elementsByTagName[0] == null || string.IsNullOrWhiteSpace(elementsByTagName[0].InnerText))
					{
						throw new InvalidOabManifestFileException(base.Definition.Account, "Full OAB element is empty.");
					}
					result = elementsByTagName[0].InnerText.Trim();
				}
			}
			return result;
		}

		private Task<WebResponse> StartFullOABDownload(WebHeaderCollection headers, string fullOABFilename)
		{
			string text = this.ExternalOABUrl + fullOABFilename;
			WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.OABTracer, base.TraceContext, "Starting full OAB download on {0} ", text, null, "StartFullOABDownload", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Oab\\OabDownloadProbe.cs", 362);
			HttpWebRequest httpWebRequest = this.WebRequest.PrepareRequest(text, true, base.Definition.Account, base.Definition.AccountPassword, this.useBasicAuthentication);
			httpWebRequest.CookieContainer = this.cookieContainer;
			httpWebRequest.Headers.Add("TargetOrgMailbox", headers["TargetOrgMailbox"]);
			return this.WebRequest.SendRequest(httpWebRequest);
		}

		private string ValidateOABFile(Task<WebResponse> task)
		{
			HttpWebResponse httpResponse = this.WebRequest.GetHttpResponse(task);
			string responseBody = this.GetResponseBody(httpResponse);
			string text = httpResponse.Headers["Last-Modified"];
			DateTime dateTime;
			if (!DateTime.TryParse(text, out dateTime))
			{
				string message = string.Format("Could not parse Last-Modified header {0}", text);
				throw new InvalidOabDataFileException(base.Definition.Account, message);
			}
			TimeSpan timeSpan = DateTime.UtcNow.Subtract(dateTime.ToUniversalTime());
			base.Result.StateAttribute11 = timeSpan.ToString();
			if (timeSpan > this.outdatedThreshold)
			{
				string message2 = string.Format("Server returned an outdated OAB file. Expected Last-Modified header less than {0} but it was {1}.", this.outdatedThreshold, timeSpan);
				throw new InvalidOabDataFileException(base.Definition.Account, message2);
			}
			if (responseBody.Length <= 0)
			{
				throw new InvalidOabDataFileException(base.Definition.Account, "Server returned an empty OAB file.");
			}
			return responseBody;
		}

		private void UpdateUserAttribute()
		{
			WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.OABTracer, base.TraceContext, "Updating AD information on {0} ", base.Definition.Account, null, "UpdateUserAttribute", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Oab\\OabDownloadProbe.cs", 417);
			try
			{
				RemotePowerShell remotePowerShell = RemotePowerShell.CreateRemotePowerShellByCredential(new Uri(this.RemotePowershellEndpoint), base.Definition.Account, base.Definition.AccountPassword, false);
				PSCommand pscommand = new PSCommand();
				pscommand.AddCommand("Set-User");
				pscommand.AddParameter("Identity", base.Definition.Account);
				pscommand.AddParameter("Office", DateTime.UtcNow.ToString());
				remotePowerShell.InvokePSCommand(pscommand);
				base.Result.StateAttribute4 = string.Format("UpdateUserAttribute::Successfully set an attribute on {0}", base.Definition.Account);
			}
			catch (Exception ex)
			{
				base.Result.StateAttribute4 = string.Format("UpdateUserAttribute::Error setting an attribute on {0}: {1}", base.Definition.Account, ex.Message);
				base.Result.StateAttribute5 = ex.StackTrace;
			}
			WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.OABTracer, base.TraceContext, "OabUpdateUserAttributeProbe succeeded with Username: {0}", base.Definition.Account, null, "UpdateUserAttribute", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Oab\\OabDownloadProbe.cs", 443);
		}

		private void HandleException(Task<WebResponse> task)
		{
			try
			{
				Exception innerException = task.Exception.InnerException;
				this.LogException(innerException);
				throw innerException;
			}
			catch (Exception arg)
			{
				WTFDiagnostics.TraceInformation<Exception>(ExTraceGlobals.OABTracer, base.TraceContext, "Exception thrown on HandleException: {0}", arg, null, "HandleException", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Oab\\OabDownloadProbe.cs", 462);
				throw;
			}
		}

		private string GetResponseBody(HttpWebResponse response)
		{
			base.Result.FailureContext = response.ResponseUri.ToString();
			string httpResponseBody = this.WebRequest.GetHttpResponseBody(response);
			if (HttpStatusCode.OK != response.StatusCode)
			{
				string message = string.Format("{0}:{1}", response.StatusCode, httpResponseBody);
				throw new WebException(message);
			}
			string text = response.Headers["X-DiagInfo"];
			if (!string.IsNullOrEmpty(text))
			{
				base.Result.StateAttribute2 = text;
			}
			return httpResponseBody;
		}

		private void LogException(Exception e)
		{
			WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.OABTracer, base.TraceContext, "OabDownloadProbe failed with exception: {0}", e.ToString(), null, "LogException", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Oab\\OabDownloadProbe.cs", 502);
			base.Result.ExecutionContext = e.Message;
			base.Result.StateAttribute3 = e.StackTrace;
		}

		private const int ExpectedNumberOfTemplates = 100;

		private const string ManifestFile = "oab.xml";

		private const string FullOABElementTagName = "Full";

		private const string TemplateElementTagName = "Template";

		private const string ExchangeVersionKey = "ExchangeVersion";

		private const string ExternalOABUrlKey = "ExternalOABUrl";

		private const string RemotePowershellEndpointKey = "RemotePowershellEndpoint";

		private const string TargetOrgMailboxHeader = "TargetOrgMailbox";

		private static readonly TimeSpan MaxE14OutdatedThreshold = TimeSpan.FromHours(72.0);

		private static readonly TimeSpan MaxE15OutdatedThreshold = TimeSpan.FromHours(36.0);

		private TimeSpan outdatedThreshold;

		private bool useBasicAuthentication = true;

		private int targetMailboxVersion;

		private CookieContainer cookieContainer;
	}
}
