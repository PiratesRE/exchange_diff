using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Remoting;
using System.Management.Automation.Runspaces;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using Microsoft.Exchange.Configuration.Authorization;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.Extensions;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;
using Microsoft.PowerShell.HostingTools;
using Microsoft.Win32;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Rps.Probes
{
	public class RPSLogonProbe : ProbeWorkItem
	{
		protected Runspace Runspace { get; set; }

		public string ResolvedEndPoint { get; protected set; }

		protected bool UseCertAuth
		{
			get
			{
				return string.IsNullOrWhiteSpace(base.Definition.AccountPassword);
			}
		}

		static RPSLogonProbe()
		{
			CertificateValidationManager.RegisterCallback("RPSProtocol", (object param0, X509Certificate param1, X509Chain param2, SslPolicyErrors param3) => true);
			RPSLogonProbe.failureCategoryRegex = new Regex("FailureCategory=(?<fc>[^\\]]+)\\]", RegexOptions.Compiled);
			RPSLogonProbe.backendServerInRPSErrorRegex = new Regex("BackEndServer=(?<BEServer>[^,]+)", RegexOptions.Compiled);
			RPSLogonProbe.cafeServerInRPSErrorRegex = new Regex("ClientAccessServer=(?<CAServer>[^,]+)", RegexOptions.Compiled);
			RPSLogonProbe.backendServerInCmdletQualifiedIdRegex = new Regex("\\[Server=(?<BEServer>[^,]+)", RegexOptions.Compiled);
			RPSLogonProbe.rpsClientErrors = new Dictionary<string, string>
			{
				{
					"InvalidShellID",
					"failed because the shell was not found on the server"
				},
				{
					"WinRMTimeout",
					"WinRM cannot complete the operation."
				},
				{
					"AccessIsDenied",
					"Access is denied."
				},
				{
					"SSLConnectionFailed",
					"The SSL connection cannot be established"
				},
				{
					"ServerNameResolveFailed",
					"The WinRM client cannot process the request because the server name cannot be resolved"
				},
				{
					"DatabaseServerRoutingError",
					"The WinRM client received an HTTP status code of 555 from the remote WS-Management service."
				},
				{
					"HttpResponseContentTypeCannotBeDetermined",
					"It cannot determine the content type of the HTTP response"
				},
				{
					"WinRMShellClientCannotProcessRequest",
					"The WinRM Shell client cannot process the request"
				}
			};
		}

		protected override void DoWork(CancellationToken cancellationToken)
		{
			this.maxInternalRetryCount = this.ReadAttribute("MaxInternalRetryCount", 1);
			if (this.maxInternalRetryCount < 0)
			{
				this.maxInternalRetryCount = 0;
			}
			this.suppressErrorIfRetrySucceed = this.ReadAttribute("SuppressErrorIfRetrySucceed", false);
			this.RecordProbeInfoInResult(string.Format("DefinitionName:{0};RecurInterval:{1};MaxRetryCount:{2};SuppressErrorWhenRetrySuccess:{3}", new object[]
			{
				base.Definition.Name,
				base.Definition.RecurrenceIntervalSeconds,
				this.maxInternalRetryCount,
				this.suppressErrorIfRetrySucceed
			}));
			this.LoadIgnoredExcpetionRegex();
			this.isRetrying = false;
			int i = -1;
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			Exception ex = null;
			Exception lastRetryOrOriginalException = null;
			while (i < this.maxInternalRetryCount)
			{
				bool flag = false;
				bool flag2 = false;
				bool flag3 = false;
				try
				{
					try
					{
						this.OpenRPSConnection();
						flag = true;
						this.InvokeCmdlet();
						flag2 = true;
						break;
					}
					finally
					{
						this.CloseRPSConnection();
						flag3 = true;
					}
				}
				catch (Exception ex2)
				{
					if (ex == null)
					{
						ex = ex2;
						if (this.maxInternalRetryCount > 0)
						{
							this.isRetrying = true;
						}
					}
					else
					{
						lastRetryOrOriginalException = ex2;
						if (!flag)
						{
							num++;
						}
						if (flag && !flag2)
						{
							num2++;
						}
						if (!flag3)
						{
							num3++;
						}
					}
					i++;
					WTFDiagnostics.TraceFunction(ExTraceGlobals.RPSTracer, base.TraceContext, "RPS Logon Test failed, will test again.", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\RPS\\RPSLogonProbe.cs", 305);
					Thread.Sleep(3000);
				}
			}
			string text = string.Empty;
			string text2 = string.Empty;
			int num4;
			if (this.maxInternalRetryCount == 0)
			{
				num4 = 0;
				text2 = "No Retry";
			}
			else if (this.maxInternalRetryCount == i)
			{
				num4 = this.maxInternalRetryCount;
				text2 = "Failure";
			}
			else
			{
				num4 = i + 1;
				text2 = "Success";
			}
			if (ex != null)
			{
				text = string.Format("Retry Result({0}) -- Total Count {1}/{2}(OpenRPSFailureCount:{3}, InvokeCmdletFailureCount:{4}, CloseRPSFailureCount:{5}), FailureCategories:{6}", new object[]
				{
					text2,
					num4,
					this.maxInternalRetryCount,
					num,
					num2,
					num3,
					(this.retryFailureCategories == null) ? "N/A" : this.retryFailureCategories.ToString()
				});
				if (!this.suppressErrorIfRetrySucceed || this.maxInternalRetryCount == i)
				{
					this.RecordRetrySummaryInResult(text, lastRetryOrOriginalException);
					string message = string.Format("{0}{1}{1}{2}{1}{1}", ex.Message, Environment.NewLine, text);
					throw new Exception(message, (ex.InnerException == null) ? ex : ex.InnerException);
				}
				this.RecordRetrySummaryInResult(text, ex);
			}
		}

		public override void PopulateDefinition<TDefinition>(TDefinition definition, Dictionary<string, string> propertyBag)
		{
			RPSLogonProbe.PopulateProbeDefinition(definition, propertyBag);
		}

		internal static void PopulateProbeDefinition(WorkDefinition definition, Dictionary<string, string> propertyBag)
		{
			definition.TimeoutSeconds = 300;
			foreach (string text in propertyBag.Keys)
			{
				string text2 = propertyBag[text];
				if (text.Equals("TimeoutSeconds", StringComparison.OrdinalIgnoreCase))
				{
					int timeoutSeconds;
					if (int.TryParse(text2, out timeoutSeconds))
					{
						definition.TimeoutSeconds = timeoutSeconds;
					}
				}
				else
				{
					string text3 = text;
					if (text.Equals("Password", StringComparison.OrdinalIgnoreCase))
					{
						text3 = "AccountPassword";
					}
					PropertyInfo property = definition.GetType().GetProperty(text3);
					if (property != null)
					{
						property.SetValue(definition, text2, null);
					}
					else
					{
						definition.Attributes[text3] = text2;
					}
				}
			}
		}

		protected Runspace OpenRPSConnection()
		{
			WTFDiagnostics.TraceFunction(ExTraceGlobals.RPSTracer, base.TraceContext, "Entering OpenRPSConnection()", null, "OpenRPSConnection", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\RPS\\RPSLogonProbe.cs", 417);
			WSManConnectionInfo connectionInfo = this.CreateWSManConnectionInfo();
			DateTime utcNow = DateTime.UtcNow;
			try
			{
				WTFDiagnostics.TraceInformation<string, string>(ExTraceGlobals.RPSTracer, base.TraceContext, "RPSLogonProbe starts as user: {0}, EndPoint: {1}", base.Definition.Account, this.ResolvedEndPoint, null, "OpenRPSConnection", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\RPS\\RPSLogonProbe.cs", 423);
				this.Runspace = RunspaceFactory.CreateRunspace(new RunspaceHost(true), connectionInfo);
				this.ClearWSManConnectionCookie();
				this.Runspace.Open();
				if (!this.isRetrying)
				{
					base.Result.SampleValue = (DateTime.UtcNow - utcNow).TotalMilliseconds;
				}
				WTFDiagnostics.TraceInformation(ExTraceGlobals.RPSTracer, base.TraceContext, "Create RPS connection successfully", null, "OpenRPSConnection", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\RPS\\RPSLogonProbe.cs", 431);
			}
			catch (Exception innerException)
			{
				if (this.Runspace != null)
				{
					this.Runspace.Dispose();
					this.Runspace = null;
				}
				TimeSpan latency = DateTime.UtcNow - utcNow;
				if (!this.isRetrying)
				{
					base.Result.SampleValue = latency.TotalMilliseconds;
				}
				this.ThrowFailureException("Test RPS connectivity failed.", latency, innerException);
			}
			WTFDiagnostics.TraceFunction(ExTraceGlobals.RPSTracer, base.TraceContext, "Leaving OpenRPSConnection()", null, "OpenRPSConnection", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\RPS\\RPSLogonProbe.cs", 449);
			return this.Runspace;
		}

		protected void CloseRPSConnection()
		{
			WTFDiagnostics.TraceFunction(ExTraceGlobals.RPSTracer, base.TraceContext, "Entering CloseRPSConnection()", null, "CloseRPSConnection", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\RPS\\RPSLogonProbe.cs", 458);
			DateTime utcNow = DateTime.UtcNow;
			try
			{
				if (this.Runspace != null)
				{
					this.Runspace.Close();
					this.Runspace.Dispose();
					this.Runspace = null;
					WTFDiagnostics.TraceInformation(ExTraceGlobals.RPSTracer, base.TraceContext, "Close RPS connection successfully", null, "CloseRPSConnection", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\RPS\\RPSLogonProbe.cs", 467);
				}
			}
			catch (Exception ex)
			{
				this.RecordRPSCloseFailureInResult(string.Format("Close RPS connection failed:{0}", ex.ToString()));
			}
			WTFDiagnostics.TraceFunction(ExTraceGlobals.RPSTracer, base.TraceContext, "Leaving CloseRPSConnection()", null, "CloseRPSConnection", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\RPS\\RPSLogonProbe.cs", 476);
		}

		protected void ThrowFailureException(string description, TimeSpan latency, Exception innerException)
		{
			if (this.ignoredExceptionRegex != null && this.ignoredExceptionRegex.IsMatch(innerException.Message))
			{
				this.skipRestError = true;
				return;
			}
			if (this.skipRestError)
			{
				return;
			}
			string str = base.Definition.Endpoint;
			if (string.Compare(base.Definition.Endpoint, this.ResolvedEndPoint, true) != 0)
			{
				str = str + " (" + this.ResolvedEndPoint + ")";
			}
			string text = string.Format("{0}EndPoint = {1}Account = {2}Password = {3}SmokeTestPage = {4}Latency = {5} (seconds){6}", new object[]
			{
				description + Environment.NewLine,
				str + Environment.NewLine,
				base.Definition.Account + Environment.NewLine,
				base.Definition.AccountPassword + Environment.NewLine,
				((base.Broker != null && base.Broker.IsLocal()) ? "LocalHost" : this.GetConnectedCASInfo()) + Environment.NewLine,
				latency.TotalSeconds,
				Environment.NewLine
			});
			if (!string.IsNullOrEmpty(base.Definition.AccountPassword))
			{
				try
				{
					text = text + Environment.NewLine + this.GetPingResult() + Environment.NewLine;
				}
				catch (Exception ex)
				{
					text = string.Concat(new string[]
					{
						text,
						Environment.NewLine,
						"Ping failed:",
						Environment.NewLine,
						ex.ToString(),
						Environment.NewLine
					});
				}
			}
			if (innerException is RemoteException)
			{
				RemoteException ex2 = (RemoteException)innerException;
				if (ex2.ErrorRecord != null && !string.IsNullOrEmpty(ex2.ErrorRecord.FullyQualifiedErrorId))
				{
					text = string.Concat(new string[]
					{
						text,
						Environment.NewLine,
						"FullyQualifiedErrorId:",
						Environment.NewLine,
						ex2.ErrorRecord.FullyQualifiedErrorId,
						Environment.NewLine
					});
				}
			}
			text = string.Concat(new object[]
			{
				text,
				Environment.NewLine,
				"InnerException:",
				Environment.NewLine,
				innerException
			});
			WTFDiagnostics.TraceError(ExTraceGlobals.RPSTracer, base.TraceContext, text, null, "ThrowFailureException", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\RPS\\RPSLogonProbe.cs", 540);
			this.CollectFailureCategoryInfomation(innerException, text);
			this.RecordWinRMErrorCodeInResult(innerException);
			throw new Exception(text);
		}

		internal static string AppendUrlParameter(string url, string parameter, string value)
		{
			Uri uri = new Uri(url);
			string str = parameter + "=" + HttpUtility.UrlEncode(value);
			if (string.IsNullOrEmpty(uri.Query))
			{
				url = url + "?" + str;
			}
			else
			{
				url = url + ";" + str;
			}
			return url;
		}

		protected virtual Runspace InvokeCmdlet()
		{
			WTFDiagnostics.TraceFunction(ExTraceGlobals.RPSTracer, base.TraceContext, "Entering InvokeCmdlet()", null, "InvokeCmdlet", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\RPS\\RPSLogonProbe.cs", 575);
			DateTime utcNow = DateTime.UtcNow;
			if (this.Runspace == null)
			{
				return null;
			}
			try
			{
				Command command;
				if (this.UseCertAuth)
				{
					command = new Command("Get-Command");
					command.Parameters.Add("Name", "Get-Command");
				}
				else
				{
					command = new Command("Get-Mailbox");
					command.Parameters.Add("ResultSize", 1);
				}
				using (PowerShell powerShell = PowerShell.Create())
				{
					powerShell.Commands.AddCommand(command);
					powerShell.Runspace = this.Runspace;
					powerShell.Invoke();
					if (powerShell.Streams.Error != null && powerShell.Streams.Error.Count > 0)
					{
						StringBuilder stringBuilder = new StringBuilder();
						stringBuilder.AppendLine("Cmdlet return the following error :");
						foreach (ErrorRecord errorRecord in powerShell.Streams.Error)
						{
							stringBuilder.AppendLine(errorRecord.Exception.ToString());
							this.RecordWinRMErrorCodeInResult(errorRecord.Exception);
						}
						throw new ApplicationException(stringBuilder.ToString());
					}
				}
				WTFDiagnostics.TraceInformation(ExTraceGlobals.RPSTracer, base.TraceContext, "Execute cmdlet successfully", null, "InvokeCmdlet", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\RPS\\RPSLogonProbe.cs", 621);
			}
			catch (Exception innerException)
			{
				this.ThrowFailureException("Execute cmdlet failed.", DateTime.UtcNow - utcNow, innerException);
			}
			WTFDiagnostics.TraceFunction(ExTraceGlobals.RPSTracer, base.TraceContext, "Leaving InvokeCmdlet()", null, "InvokeCmdlet", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\RPS\\RPSLogonProbe.cs", 628);
			return this.Runspace;
		}

		private WSManConnectionInfo CreateWSManConnectionInfo()
		{
			this.ResolvedEndPoint = this.ResolveHostNameToIPAddress(base.Definition.Endpoint);
			string text = RPSLogonProbe.AppendUrlParameter(base.Definition.Endpoint, "clientApplication", ExchangeRunspaceConfigurationSettings.ExchangeApplication.ActiveMonitor.ToString());
			string text2 = "diag";
			if (base.Definition.Attributes.ContainsKey(text2))
			{
				text = RPSLogonProbe.AppendUrlParameter(text, text2, base.Definition.Attributes[text2]);
			}
			WSManConnectionInfo wsmanConnectionInfo;
			if (this.UseCertAuth)
			{
				wsmanConnectionInfo = new WSManConnectionInfo(new Uri(text), "http://schemas.microsoft.com/powershell/Microsoft.Exchange", base.Definition.Account);
			}
			else
			{
				wsmanConnectionInfo = new WSManConnectionInfo(new Uri(text), "http://schemas.microsoft.com/powershell/Microsoft.Exchange", new PSCredential(base.Definition.Account, base.Definition.AccountPassword.ConvertToSecureString()));
			}
			AuthenticationMechanism authenticationMechanism = this.UseCertAuth ? AuthenticationMechanism.Default : AuthenticationMechanism.Basic;
			if (base.Definition.Attributes.ContainsKey("AuthenticationType"))
			{
				Enum.TryParse<AuthenticationMechanism>(base.Definition.Attributes["AuthenticationType"], out authenticationMechanism);
			}
			wsmanConnectionInfo.AuthenticationMechanism = authenticationMechanism;
			wsmanConnectionInfo.OpenTimeout = 180000;
			wsmanConnectionInfo.OperationTimeout = 180000;
			int maximumConnectionRedirectionCount = (!this.EnforceRPSRedirectInLocalProbe && base.Broker != null && base.Broker.IsLocal()) ? 0 : 5;
			if (base.Definition.Attributes.ContainsKey("MaximumConnectionRedirectionCount"))
			{
				int.TryParse(base.Definition.Attributes["MaximumConnectionRedirectionCount"], out maximumConnectionRedirectionCount);
			}
			wsmanConnectionInfo.MaximumConnectionRedirectionCount = maximumConnectionRedirectionCount;
			bool flag = base.Broker != null && base.Broker.IsLocal();
			if (base.Definition.Attributes.ContainsKey("TrustAnySslCertificate"))
			{
				bool.TryParse(base.Definition.Attributes["TrustAnySslCertificate"], out flag);
			}
			wsmanConnectionInfo.SkipCACheck = flag;
			wsmanConnectionInfo.SkipCNCheck = flag;
			wsmanConnectionInfo.SkipRevocationCheck = flag;
			return wsmanConnectionInfo;
		}

		private string GetPingResult()
		{
			string text = null;
			string text2 = null;
			HttpWebResponse httpWebResponse = null;
			DateTime utcNow = DateTime.UtcNow;
			try
			{
				string requestUriString;
				if (base.Definition.Endpoint.Contains('?'))
				{
					requestUriString = base.Definition.Endpoint + "&ping=probe";
				}
				else
				{
					requestUriString = base.Definition.Endpoint + "?ping=probe";
				}
				HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(requestUriString);
				byte[] bytes = Encoding.ASCII.GetBytes(base.Definition.Account + ":" + base.Definition.AccountPassword);
				string str = Convert.ToBase64String(bytes);
				httpWebRequest.Headers.Add("Authorization", "Basic " + str);
				text = Guid.NewGuid().ToString();
				httpWebRequest.Headers.Add("client-request-id", text);
				httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
			}
			catch (Exception ex)
			{
				text2 = ex.ToString();
				if (ex is WebException)
				{
					httpWebResponse = (HttpWebResponse)((WebException)ex).Response;
					if (httpWebResponse != null)
					{
						HttpWebRequestUtility httpWebRequestUtility = new HttpWebRequestUtility(base.TraceContext);
						text2 = httpWebRequestUtility.GetHttpResponseBody(httpWebResponse);
					}
				}
				WTFDiagnostics.TraceDebug(ExTraceGlobals.RPSTracer, base.TraceContext, string.Format("GetPingResult() gets exception: " + ex, new object[0]), null, "GetPingResult", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\RPS\\RPSLogonProbe.cs", 758);
			}
			string text3;
			if (httpWebResponse == null)
			{
				text3 = string.Format("Didn't get ping response within {0} milliseconds.{1}ClientRequestId = {2}Exception: {3}", new object[]
				{
					(DateTime.UtcNow - utcNow).TotalMilliseconds,
					Environment.NewLine,
					text + Environment.NewLine,
					Environment.NewLine + text2
				});
			}
			else
			{
				text3 = string.Format("PingResultStatus = {0}X-FEServer = {1}X-BEServer = {2}X-CalculatedBETarget = {3}request-id = {4}", new object[]
				{
					httpWebResponse.StatusCode + Environment.NewLine,
					httpWebResponse.GetResponseHeader("X-FEServer") + Environment.NewLine,
					httpWebResponse.GetResponseHeader("X-BEServer") + Environment.NewLine,
					httpWebResponse.GetResponseHeader("X-CalculatedBETarget") + Environment.NewLine,
					httpWebResponse.GetResponseHeader("request-id") + Environment.NewLine
				});
				this.RecordBackEndServerInResult(httpWebResponse.GetResponseHeader("X-BEServer"));
				if (!string.IsNullOrEmpty(text2))
				{
					text3 = string.Concat(new string[]
					{
						text3,
						"Exception of ping requst: ",
						Environment.NewLine,
						text2,
						Environment.NewLine
					});
				}
			}
			WTFDiagnostics.TraceInformation(ExTraceGlobals.RPSTracer, base.TraceContext, "PingResult: " + Environment.NewLine + text3, null, "GetPingResult", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\RPS\\RPSLogonProbe.cs", 788);
			return text3;
		}

		private string GetConnectedCASInfo()
		{
			string text = "None";
			string text2 = null;
			try
			{
				int num = this.ResolvedEndPoint.IndexOf("/", "https://".Length);
				if (num < 0)
				{
					return text;
				}
				string url = this.ResolvedEndPoint.Remove(num) + "/smoketest/test.htm";
				HttpWebRequestUtility httpWebRequestUtility = new HttpWebRequestUtility(base.TraceContext);
				HttpWebRequest httpWebRequest = httpWebRequestUtility.CreateCommonHttpWebRequest(url, false, false);
				CertificateValidationManager.SetComponentId(httpWebRequest, "RPSProtocol");
				HttpWebResponse response = (HttpWebResponse)httpWebRequest.GetResponse();
				text2 = httpWebRequestUtility.GetHttpResponseBody(response);
				Match match = Regex.Match(text2, "Connectivity test page\\s*-\\s*(?<casFQDN>[^<]+)</BODY>", RegexOptions.IgnoreCase);
				if (match.Success)
				{
					text = match.Groups["casFQDN"].Value.Trim();
				}
			}
			catch (Exception ex)
			{
				WTFDiagnostics.TraceDebug(ExTraceGlobals.RPSTracer, base.TraceContext, string.Format("GetConnectedCASInfo() gets exception:{0}{1}Body={2} ", ex, Environment.NewLine, text2), null, "GetConnectedCASInfo", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\RPS\\RPSLogonProbe.cs", 828);
				text = "Failed :" + ex.Message;
			}
			WTFDiagnostics.TraceInformation(ExTraceGlobals.RPSTracer, base.TraceContext, "Inner CAS FQDN: " + text, null, "GetConnectedCASInfo", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\RPS\\RPSLogonProbe.cs", 832);
			return text;
		}

		private void ClearWSManConnectionCookie()
		{
			try
			{
				using (RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\WSMAN\\Client", true))
				{
					string text = "ConnectionCookies";
					if (registryKey.GetSubKeyNames().Contains(text))
					{
						registryKey.DeleteSubKey(text);
					}
				}
			}
			catch (Exception arg)
			{
				this.RecordCookieCleanErrorInResult(string.Format("Failed to clear WSMan connection cookie, Exception = {0}", arg));
			}
		}

		private string ResolveHostNameToIPAddress(string url)
		{
			bool flag = false;
			if (base.Definition.Attributes.ContainsKey("ResolveHostName"))
			{
				bool.TryParse(base.Definition.Attributes["ResolveHostName"], out flag);
			}
			UriBuilder uriBuilder = new UriBuilder(url);
			this.RecordHostNameInResult(uriBuilder.Host);
			if (flag)
			{
				try
				{
					IPHostEntry hostEntry = Dns.GetHostEntry(uriBuilder.Host);
					IPAddress ipaddress = hostEntry.AddressList.First((IPAddress addr) => addr.AddressFamily == AddressFamily.InterNetwork);
					if (ipaddress != null)
					{
						uriBuilder.Host = ipaddress.ToString();
						this.RecordTargetIPInResult(ipaddress.ToString());
						WTFDiagnostics.TraceInformation<string, string>(ExTraceGlobals.RPSTracer, base.TraceContext, "Resolve hostname to IPAddress, original url = {0}, resolved url = {1}", url, uriBuilder.ToString(), null, "ResolveHostNameToIPAddress", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\RPS\\RPSLogonProbe.cs", 890);
						return uriBuilder.ToString();
					}
					return url;
				}
				catch (Exception arg)
				{
					this.RecordHostNameResolveError(string.Format("Resolve host name failed. Exception={0}", arg));
					return url;
				}
			}
			this.RecordTargetIPInResult("Unknown");
			return url;
		}

		private void CollectFailureCategoryInfomation(Exception failureExcpetion, string failureMessage)
		{
			string text = null;
			Match match = RPSLogonProbe.failureCategoryRegex.Match(failureExcpetion.Message);
			if (match.Success)
			{
				text = match.Groups["fc"].Value;
			}
			if (string.IsNullOrEmpty(text) && failureExcpetion is RemoteException)
			{
				RemoteException ex = (RemoteException)failureExcpetion;
				if (ex.ErrorRecord != null && !string.IsNullOrEmpty(ex.ErrorRecord.FullyQualifiedErrorId))
				{
					match = RPSLogonProbe.failureCategoryRegex.Match(ex.ErrorRecord.FullyQualifiedErrorId);
					if (match.Success)
					{
						text = match.Groups["fc"].Value;
					}
					match = RPSLogonProbe.backendServerInCmdletQualifiedIdRegex.Match(ex.ErrorRecord.FullyQualifiedErrorId);
					if (match.Success)
					{
						this.RecordBackEndServerInResult(match.Groups["BEServer"].Value);
					}
				}
			}
			if (string.IsNullOrEmpty(text))
			{
				text = "Unknown";
				foreach (string text2 in RPSLogonProbe.rpsClientErrors.Keys)
				{
					if (failureExcpetion.Message.IndexOf(RPSLogonProbe.rpsClientErrors[text2], StringComparison.OrdinalIgnoreCase) >= 0)
					{
						text = text2;
						break;
					}
				}
			}
			if (!this.isRetrying)
			{
				this.RecordOriginalFailureCategoryInResult(text);
			}
			else if (this.retryFailureCategories == null)
			{
				this.retryFailureCategories = new StringBuilder();
				this.retryFailureCategories.Append(text);
			}
			else
			{
				this.retryFailureCategories.Append(";" + text);
			}
			if (base.Broker != null && !base.Broker.IsLocal())
			{
				string text3 = "Unknown";
				Match match2 = RPSLogonProbe.backendServerInRPSErrorRegex.Match(failureExcpetion.Message);
				if (match2.Success)
				{
					text3 = match2.Groups["BEServer"].Value;
					if (text3.Contains('.'))
					{
						text3 = text3.Remove(text3.IndexOf('.'));
					}
				}
				this.RecordBackEndServerInResult(text3);
				string cafeServer = "Unknown";
				Match match3 = RPSLogonProbe.cafeServerInRPSErrorRegex.Match(failureExcpetion.Message);
				if (match3.Success)
				{
					cafeServer = match3.Groups["CAServer"].Value;
				}
				this.RecordCafeServerInResult(cafeServer);
			}
			this.RecordExceptionNameInResult(failureExcpetion.GetType().Name);
			this.RecordAccountInResult(base.Definition.Account);
		}

		private void LoadIgnoredExcpetionRegex()
		{
			if (base.Definition.Attributes.ContainsKey("IgnoredExceptionRegex"))
			{
				try
				{
					string pattern = base.Definition.Attributes["IgnoredExceptionRegex"];
					this.ignoredExceptionRegex = new Regex(pattern);
				}
				catch (Exception arg)
				{
					WTFDiagnostics.TraceInformation<Exception>(ExTraceGlobals.RPSTracer, base.TraceContext, "LoadIgnoredExcpetionRegex failed, Exception = {0}", arg, null, "LoadIgnoredExcpetionRegex", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\RPS\\RPSLogonProbe.cs", 1023);
				}
			}
		}

		private void RecordCookieCleanErrorInResult(string errMsg)
		{
			base.Result.StateAttribute24 = errMsg;
		}

		private void RecordHostNameResolveError(string errMsg)
		{
			base.Result.StateAttribute25 = errMsg;
		}

		private void RecordProbeInfoInResult(string probeInfo)
		{
			if (string.IsNullOrEmpty(base.Result.StateAttribute4))
			{
				base.Result.StateAttribute4 = probeInfo;
				return;
			}
			base.Result.StateAttribute4 = string.Format("{0};{1}", base.Result.StateAttribute4, probeInfo);
		}

		private void RecordRPSCloseFailureInResult(string errMsg)
		{
			base.Result.StateAttribute11 = errMsg;
		}

		private void RecordOriginalFailureCategoryInResult(string fcStr)
		{
			base.Result.StateAttribute12 = fcStr;
		}

		private void RecordRetrySummaryInResult(string summaryStr, Exception lastRetryOrOriginalException)
		{
			if (lastRetryOrOriginalException != null)
			{
				base.Result.StateAttribute13 = ((lastRetryOrOriginalException.InnerException == null) ? lastRetryOrOriginalException.ToString() : lastRetryOrOriginalException.InnerException.ToString());
			}
			base.Result.StateAttribute14 = summaryStr;
		}

		private void RecordBackEndServerInResult(string serverName)
		{
			if (string.IsNullOrEmpty(base.Result.StateAttribute15) || "Unknown".Equals(base.Result.StateAttribute15, StringComparison.OrdinalIgnoreCase))
			{
				base.Result.StateAttribute15 = serverName;
			}
		}

		private void RecordHostNameInResult(string endPoint)
		{
			base.Result.StateAttribute21 = endPoint;
		}

		private void RecordTargetIPInResult(string targetIP)
		{
			if (!this.isRetrying)
			{
				base.Result.StateAttribute22 = targetIP;
			}
		}

		private void RecordCafeServerInResult(string cafeServer)
		{
			if (!this.isRetrying)
			{
				base.Result.StateAttribute23 = cafeServer;
			}
		}

		private void RecordAccountInResult(string accountName)
		{
			base.Result.StateAttribute2 = accountName;
		}

		private void RecordExceptionNameInResult(string exceptionName)
		{
			if (!this.isRetrying)
			{
				base.Result.StateAttribute3 = exceptionName;
			}
		}

		private void RecordWinRMErrorCodeInResult(Exception lastRetryOrOriginalException)
		{
			try
			{
				if (lastRetryOrOriginalException is PSRemotingTransportException)
				{
					string text = ((PSRemotingTransportException)lastRetryOrOriginalException).ErrorCode.ToString();
					bool flag = false;
					foreach (string text2 in base.Result.StateAttribute5.Split(new char[]
					{
						';'
					}))
					{
						if (text2.Equals(text, StringComparison.OrdinalIgnoreCase))
						{
							flag = true;
						}
					}
					if (!flag)
					{
						ProbeResult result = base.Result;
						result.StateAttribute5 = result.StateAttribute5 + text + ';';
					}
				}
			}
			catch (Exception)
			{
			}
		}

		public const string TrustAnySSLCertParamterName = "TrustAnySslCertificate";

		public const string MaximumConnectionRedirectionCountParameterName = "MaximumConnectionRedirectionCount";

		public const string IgnoredExceptionRegexParameterName = "IgnoredExceptionRegex";

		public const string ResolveHostNameParameterName = "ResolveHostName";

		public const string AuthenticationTypeParameterName = "AuthenticationType";

		public const string ExecutionIdParameterName = "ExecutionId";

		public const string MaxInternalRetryCountParam = "MaxInternalRetryCount";

		public const string SuppressErrorIfRetrySucceedParam = "SuppressErrorIfRetrySucceed";

		private const string UnknownTag = "Unknown";

		public const int DefaultMaxInternalRetryCount = 1;

		public const bool DefaultSuppressErrorIfRetrySucceed = false;

		private const string RPSProtocolId = "RPSProtocol";

		private const string ServerAddressPrefix = "https://";

		private const int DefaultMaximumConnectionRedirectionCount = 5;

		protected bool EnforceRPSRedirectInLocalProbe;

		private static readonly Regex failureCategoryRegex;

		private static readonly Regex backendServerInRPSErrorRegex;

		private static readonly Regex cafeServerInRPSErrorRegex;

		private static readonly Regex backendServerInCmdletQualifiedIdRegex;

		private Regex ignoredExceptionRegex;

		private bool skipRestError;

		private static Dictionary<string, string> rpsClientErrors;

		private int maxInternalRetryCount;

		private bool suppressErrorIfRetrySucceed;

		private bool isRetrying;

		private StringBuilder retryFailureCategories;
	}
}
