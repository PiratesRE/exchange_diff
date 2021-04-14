using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common.SmartCategorization;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;
using Microsoft.Security.Application;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.ClientAccess
{
	public class CafeLocalProbe : ProbeWorkItem
	{
		public CafeLocalProbe()
		{
			this.Tracer = ExTraceGlobals.CafeTracer;
		}

		private protected bool Verbose { protected get; private set; }

		private protected bool TrustAnySslCertificate { protected get; private set; }

		private protected int ProbeTimeLimit { protected get; private set; }

		private protected int HttpRequestTimeout { protected get; private set; }

		private protected int SslValidationDelaySeconds { protected get; private set; }

		private protected int PreRequestDelaySeconds { protected get; private set; }

		protected Trace Tracer { get; set; }

		public override void PopulateDefinition<Definition>(Definition definition, Dictionary<string, string> propertyBag)
		{
			ProbeDefinition probeDefinition = definition as ProbeDefinition;
			MailboxDatabaseInfo invokeNowMonitoringAccount = this.GetInvokeNowMonitoringAccount();
			CafeLocalProbe.PopulateDefinition(probeDefinition, CafeDiscovery.AssemblyPath, CafeDiscovery.ProbeTypeName, propertyBag["Name"], propertyBag["ServiceName"], propertyBag["TargetResource"], 0, 60, 0, invokeNowMonitoringAccount.MonitoringAccountUserPrincipalName, invokeNowMonitoringAccount.MonitoringAccountPassword, invokeNowMonitoringAccount.MonitoringAccount, invokeNowMonitoringAccount.MonitoringAccountDomain, invokeNowMonitoringAccount.MonitoringAccountMailboxGuid.ToString(), CafeDiscovery.ProbeEndPoint);
			probeDefinition.Attributes["TrustAnySslCertificate"] = true.ToString();
			probeDefinition.Attributes["Verbose"] = true.ToString();
			if (propertyBag.ContainsKey("Account"))
			{
				probeDefinition.Account = propertyBag["Account"];
				probeDefinition.AccountDisplayName = probeDefinition.Account;
			}
			if (propertyBag.ContainsKey("Password"))
			{
				probeDefinition.AccountPassword = propertyBag["Password"];
			}
			if (propertyBag.ContainsKey("Endpoint"))
			{
				probeDefinition.Endpoint = propertyBag["Endpoint"];
			}
			int timeoutSeconds;
			if (propertyBag.ContainsKey("TimeoutSeconds") && int.TryParse(propertyBag["TimeoutSeconds"], out timeoutSeconds))
			{
				probeDefinition.TimeoutSeconds = timeoutSeconds;
			}
		}

		private static void PopulateDefinition(ProbeDefinition probe, string assemblyPath, string typeName, string name, string serviceName, string targetResource, int recurrenceIntervalSeconds, int timeoutSeconds, int maxRetryAttempts, string account, string accountPassword, string accountDisplayName, string accountDomain, string mailboxGuid, string endPoint)
		{
			probe.AssemblyPath = assemblyPath;
			probe.TypeName = typeName;
			probe.Name = name;
			probe.ServiceName = serviceName;
			probe.TargetResource = targetResource;
			probe.RecurrenceIntervalSeconds = recurrenceIntervalSeconds;
			probe.TimeoutSeconds = timeoutSeconds;
			probe.MaxRetryAttempts = maxRetryAttempts;
			probe.Account = account;
			probe.AccountPassword = accountPassword;
			probe.AccountDisplayName = accountDisplayName;
			probe.Endpoint = endPoint;
			probe.Attributes["AccountDomain"] = accountDomain;
			probe.Attributes["MailboxGuid"] = mailboxGuid;
			ProtocolDescriptor protocolDescriptor = CafeProtocols.Protocols.SingleOrDefault((ProtocolDescriptor p) => p.AppPool == probe.TargetResource);
			if (CafeProtocols.VirtualDirectories != null && protocolDescriptor != null && !LocalEndpointManager.IsDataCenter)
			{
				string vdirToMatch = protocolDescriptor.VirtualDirectory.ToLower();
				ADVirtualDirectory advirtualDirectory = CafeProtocols.VirtualDirectories.FirstOrDefault((ADVirtualDirectory item) => item.Identity.ToString().ToLower().Contains(vdirToMatch));
				if (advirtualDirectory != null)
				{
					StringBuilder stringBuilder = new StringBuilder(100);
					foreach (AuthenticationMethod authenticationMethod in advirtualDirectory.InternalAuthenticationMethods)
					{
						if (stringBuilder.Length > 0)
						{
							stringBuilder.Append(',');
						}
						stringBuilder.Append(authenticationMethod.ToString());
					}
					probe.Attributes[CafeLocalProbe.AuthenticationTypes] = stringBuilder.ToString();
				}
			}
		}

		internal static ProbeDefinition CreateDefinition(MailboxDatabaseInfo dbInfo, ProbeIdentity probeIdentity, string endPoint)
		{
			ProbeDefinition probeDefinition = new ProbeDefinition();
			CafeLocalProbe.PopulateDefinition(probeDefinition, CafeDiscovery.AssemblyPath, CafeDiscovery.ProbeTypeName, probeIdentity.Name, probeIdentity.Component.Name, probeIdentity.TargetResource, 30, 10, 0, dbInfo.MonitoringAccount + "@" + dbInfo.MonitoringAccountDomain, dbInfo.MonitoringAccountPassword, dbInfo.MonitoringAccount, dbInfo.MonitoringAccountDomain, dbInfo.MonitoringAccountMailboxGuid.ToString(), endPoint);
			return probeDefinition;
		}

		protected override void DoWork(CancellationToken cancellationToken)
		{
			CafeLocalProbe.HttpProtocolProbe worker = null;
			string postBody = null;
			this.cancellationToken = cancellationToken;
			this.startTime = DateTime.UtcNow;
			this.breadcrumbs = new Breadcrumbs(1024, base.TraceContext);
			this.Configure();
			string targetResource = base.Definition.TargetResource;
			ProtocolDescriptor protocolDescriptor = CafeProtocols.Get(targetResource);
			bool isDataCenter = LocalEndpointManager.IsDataCenter;
			worker = new CafeLocalProbe.HttpProtocolProbe(protocolDescriptor, ProbeState.PreparingRequest);
			worker.Url = this.FormatUrl(protocolDescriptor);
			try
			{
				if (worker.State == ProbeState.PreparingRequest)
				{
					WTFDiagnostics.TraceInformation(this.Tracer, base.TraceContext, "Issuing request", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Cafe\\CafeLocalProbe.cs", 330);
					try
					{
						this.request = (HttpWebRequest)WebRequest.Create(worker.Url);
						this.PrepareRequest(this.request, protocolDescriptor, out postBody);
						worker.State = ProbeState.WaitingResponse;
					}
					catch (Exception ex)
					{
						worker.State = ProbeState.FailedRequest;
						worker.Error = ex;
						worker.TimeCompleted = DateTime.UtcNow;
						this.breadcrumbs.Drop("Request failed: " + CafeLocalProbe.Flatten(ex));
					}
				}
				if (worker.State == ProbeState.WaitingResponse)
				{
					DateTime utcNow = DateTime.UtcNow;
					int num = Math.Max(this.HttpRequestTimeout - (int)(utcNow - this.startTime).TotalMilliseconds, 0);
					try
					{
						string logLine;
						if (this.PreRequestDelaySeconds != 0)
						{
							logLine = string.Format("Pre-request: delay {0} seconds", this.PreRequestDelaySeconds);
							this.breadcrumbs.Drop(logLine);
							WTFDiagnostics.TraceInformation(this.Tracer, base.TraceContext, logLine, null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Cafe\\CafeLocalProbe.cs", 366);
							Thread.Sleep(this.PreRequestDelaySeconds * 1000);
						}
						worker.ResponseTask = new Task(delegate()
						{
							if (postBody == null)
							{
								logLine = "Issuing " + this.request.Method + " against " + worker.Url;
								this.breadcrumbs.Drop(logLine);
								WTFDiagnostics.TraceInformation(this.Tracer, this.TraceContext, logLine, null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Cafe\\CafeLocalProbe.cs", 377);
								logLine = "Awaiting " + this.request.Method + " response";
								this.breadcrumbs.Drop(logLine);
								WTFDiagnostics.TraceInformation(this.Tracer, this.TraceContext, logLine, null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Cafe\\CafeLocalProbe.cs", 385);
								this.response = (HttpWebResponse)this.request.GetResponse();
							}
							else
							{
								logLine = string.Format("Issuing " + this.request.Method + " against {0}", worker.Url);
								this.breadcrumbs.Drop(logLine);
								WTFDiagnostics.TraceInformation(this.Tracer, this.TraceContext, logLine, null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Cafe\\CafeLocalProbe.cs", 393);
								using (StreamWriter streamWriter = new StreamWriter(this.request.GetRequestStream()))
								{
									streamWriter.Write(postBody);
								}
								logLine = "Awaiting " + this.request.Method + " response";
								this.breadcrumbs.Drop(logLine);
								WTFDiagnostics.TraceInformation(this.Tracer, this.TraceContext, logLine, null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Cafe\\CafeLocalProbe.cs", 406);
								this.response = (HttpWebResponse)this.request.GetResponse();
							}
							logLine = "Response received";
							this.breadcrumbs.Drop(logLine);
							WTFDiagnostics.TraceInformation(this.Tracer, this.TraceContext, logLine, null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Cafe\\CafeLocalProbe.cs", 412);
						}, this.cancellationToken);
						this.breadcrumbs.Drop("Starting HTTP request task");
						worker.ResponseTask.Start();
						if (!worker.ResponseTask.IsCompleted && num > 0)
						{
							this.breadcrumbs.Drop("Waiting {0} ms", new object[]
							{
								num
							});
							worker.ResponseTask.Wait(num);
						}
						utcNow = DateTime.UtcNow;
						if (worker.ResponseTask.IsCompleted)
						{
							worker.TimeCompleted = utcNow;
							if (this.response == null)
							{
								worker.State = ProbeState.TimedOut;
								this.breadcrumbs.Drop("Task completed but no response is available");
							}
							else if (this.response.StatusCode == HttpStatusCode.OK)
							{
								worker.State = ProbeState.Passed;
								this.breadcrumbs.Drop("Successfully completed with 200 OK");
							}
							else if (this.response.StatusCode == HttpStatusCode.Found)
							{
								string text = this.response.Headers[HttpResponseHeader.Location] ?? "<empty>";
								if (protocolDescriptor.IsRedirectOK)
								{
									worker.State = ProbeState.Passed;
									this.breadcrumbs.Drop("Successfully completed with 302 redirect");
									if (!string.IsNullOrEmpty(text))
									{
										this.breadcrumbs.Drop("Redirected to {0}", new object[]
										{
											text
										});
									}
								}
								else
								{
									worker.State = ProbeState.FailedResponse;
									this.breadcrumbs.Drop(string.Format("Failed with unexpected 302 redirect to: {0}", text));
								}
							}
							else if (this.response.StatusCode > (HttpStatusCode)0 && this.response.StatusCode < HttpStatusCode.BadRequest)
							{
								worker.State = ProbeState.Passed;
								this.breadcrumbs.Drop("Successfully completed with unexpected response status {0} - {1}", new object[]
								{
									this.response.StatusCode,
									this.response.StatusDescription
								});
							}
							else
							{
								worker.State = ProbeState.FailedResponse;
								this.breadcrumbs.Drop(string.Format("Failed with response: {0} - {1}", this.response.StatusCode, this.response.StatusDescription));
							}
						}
						else
						{
							worker.State = ProbeState.TimedOut;
							worker.TimeCompleted = utcNow;
							this.breadcrumbs.Drop("Timed out!");
						}
					}
					catch (AggregateException ex2)
					{
						Exception innerException = ex2.Flatten().InnerException;
						WebException ex3 = innerException as WebException;
						if (ex3 != null)
						{
							HttpWebResponse httpWebResponse = ex3.Response as HttpWebResponse;
							if (httpWebResponse != null)
							{
								this.response = httpWebResponse;
								if (protocolDescriptor.HttpProtocol == HttpProtocol.PowerShell && httpWebResponse.StatusCode == HttpStatusCode.Unauthorized)
								{
									worker.State = ProbeState.Passed;
									string logLine = "Powershell (regular) probe received expected 401";
									this.breadcrumbs.Drop(logLine);
									WTFDiagnostics.TraceInformation(this.Tracer, base.TraceContext, logLine, null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Cafe\\CafeLocalProbe.cs", 519);
								}
							}
						}
						if (worker.State != ProbeState.Passed)
						{
							worker.State = ProbeState.FailedResponse;
							worker.Error = innerException;
							worker.TimeCompleted = utcNow;
							this.breadcrumbs.Drop("Failed with exception: " + innerException.Message);
						}
					}
					catch (Exception ex4)
					{
						worker.State = ProbeState.FailedResponse;
						worker.Error = ex4;
						worker.TimeCompleted = utcNow;
						this.breadcrumbs.Drop("Failed with exception: " + ex4.Message);
					}
				}
			}
			catch (Exception)
			{
				WTFDiagnostics.TraceInformation(this.Tracer, base.TraceContext, "CafeLocalProbe FAILED: uncaught exception thrown", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Cafe\\CafeLocalProbe.cs", 549);
				throw;
			}
			finally
			{
				int num2 = (int)(DateTime.UtcNow - this.startTime).TotalMilliseconds;
				WTFDiagnostics.TraceInformation<int>(this.Tracer, base.TraceContext, "Probe completed in {0} ms", num2, null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Cafe\\CafeLocalProbe.cs", 557);
				this.ReportHumanReadableProbeDetails(worker);
				base.Result.SampleValue = (double)num2;
				if (this.response != null)
				{
					this.response.Close();
				}
			}
			if (worker == null)
			{
				throw new InvalidOperationException("Reached end of probe without instantiating a worker");
			}
			ProbeState state = worker.State;
			if (state == ProbeState.Passed)
			{
				WTFDiagnostics.TraceInformation<string>(this.Tracer, base.TraceContext, "CafeLocalProbe {0} PASSED", targetResource, null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Cafe\\CafeLocalProbe.cs", 581);
				return;
			}
			WTFDiagnostics.TraceInformation<string, ProbeState>(this.Tracer, base.TraceContext, "CafeLocalProbe {0} FAILED: {1}", targetResource, worker.State, null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Cafe\\CafeLocalProbe.cs", 589);
			string format = (worker.Error != null) ? worker.Error.Message : worker.State.ToString();
			throw new ApplicationException(string.Format(format, targetResource, worker.State));
		}

		private static bool UseFBAAuthForProtocol(ProtocolDescriptor protocol, bool isDatacenter)
		{
			return !isDatacenter && protocol.HttpProtocol == HttpProtocol.OWA;
		}

		private static string Flatten(Exception e)
		{
			return e.ToString().Replace("\r\n", "+");
		}

		private bool ValidateRemoteCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors policyErrors)
		{
			if (this.SslValidationDelaySeconds != 0)
			{
				this.breadcrumbs.Drop("SSL validation: delay {0} seconds", new object[]
				{
					this.SslValidationDelaySeconds
				});
				Thread.Sleep(this.SslValidationDelaySeconds * 1000);
			}
			else
			{
				this.breadcrumbs.Drop("Performing SSL validation");
			}
			return true;
		}

		private void Configure()
		{
			this.Verbose = this.ReadAttribute("Verbose", true);
			this.TrustAnySslCertificate = this.ReadAttribute("TrustAnySslCertificate", false);
			this.ProbeTimeLimit = Math.Max(base.Definition.TimeoutSeconds * 1000, 5000);
			this.HttpRequestTimeout = (int)this.ReadAttribute("HttpRequestTimeoutSpan", TimeSpan.FromMilliseconds((double)(this.ProbeTimeLimit - 1000))).TotalMilliseconds;
			if (this.Verbose)
			{
				string text = string.Format("Probe Absolute Timeout={0}ms, Timeout Value={1}ms, Calculated HttpRequest Timeout={2}ms\r\n", base.Definition.TimeoutSeconds * 1000, this.ProbeTimeLimit, this.HttpRequestTimeout);
				this.breadcrumbs.Drop(text);
				ProbeResult result = base.Result;
				result.ExecutionContext += text;
				WTFDiagnostics.TraceInformation(this.Tracer, base.TraceContext, text, null, "Configure", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Cafe\\CafeLocalProbe.cs", 686);
			}
			this.SslValidationDelaySeconds = Utils.LoadAppconfigIntSetting("SslValidationDelaySeconds", 0);
			this.PreRequestDelaySeconds = Utils.LoadAppconfigIntSetting("PreRequestDelaySeconds", 0);
		}

		private void PrepareRequest(HttpWebRequest request, ProtocolDescriptor protocol, out string postBody)
		{
			postBody = null;
			request.AllowAutoRedirect = false;
			request.ServicePoint.Expect100Continue = false;
			request.Proxy = NullWebProxy.Instance;
			request.Timeout = this.HttpRequestTimeout;
			request.KeepAlive = false;
			request.PreAuthenticate = true;
			request.UserAgent = "AMProbe/Local/ClientAccess";
			request.Headers.Add("X-ProbeType", "X-MS-ClientAccess-LocalProbe");
			if (this.TrustAnySslCertificate)
			{
				string componentId = "Cafe_" + protocol;
				CertificateValidationManager.SetComponentId(request, componentId);
				RemoteCertificateValidationCallback callback = new RemoteCertificateValidationCallback(this.ValidateRemoteCertificate);
				CertificateValidationManager.RegisterCallback(componentId, callback);
				request.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(this.ValidateRemoteCertificate);
			}
			AuthenticationMethod? authTypeForProtocol = this.GetAuthTypeForProtocol(protocol);
			AuthenticationMethod valueOrDefault = authTypeForProtocol.GetValueOrDefault();
			if (authTypeForProtocol != null)
			{
				if (valueOrDefault == AuthenticationMethod.Basic)
				{
					string str = Convert.ToBase64String(Encoding.Default.GetBytes(base.Definition.Account + ":" + base.Definition.AccountPassword));
					request.Headers.Add("Authorization", "Basic " + str);
					goto IL_1E2;
				}
				if (valueOrDefault == AuthenticationMethod.Fba)
				{
					request.Method = "POST";
					request.ContentType = "application/x-www-form-urlencoded";
					string text = string.Format("{0}/{1}/", base.Definition.Endpoint.TrimEnd(CafeLocalProbe.TrailingSlash), protocol.VirtualDirectory);
					postBody = string.Format("destination={0}&flags=4&forcedownlevel=0&trusted=4&username={1}&password={2}&isUtf8=1", Encoder.HtmlFormUrlEncode(text), Encoder.HtmlFormUrlEncode(base.Definition.Account), Encoder.HtmlFormUrlEncode(base.Definition.AccountPassword));
					request.ContentLength = (long)postBody.Length;
					goto IL_1E2;
				}
				if (valueOrDefault == AuthenticationMethod.Misconfigured)
				{
					request.Credentials = null;
					request.UseDefaultCredentials = false;
					request.PreAuthenticate = false;
					goto IL_1E2;
				}
			}
			NetworkCredential credentials = new NetworkCredential(base.Definition.Account, base.Definition.AccountPassword);
			request.Credentials = credentials;
			IL_1E2:
			if (protocol.HttpProtocol == HttpProtocol.RPC)
			{
				request.Method = "RPC_IN_DATA";
				request.ContentLength = 0L;
			}
			if (!authTypeForProtocol.Equals(AuthenticationMethod.Fba))
			{
				request.Method = "GET";
				request.ContentLength = 0L;
			}
		}

		private void ReportHumanReadableProbeDetails(CafeLocalProbe.HttpProtocolProbe worker)
		{
			StringBuilder stringBuilder = new StringBuilder(300);
			int num = (int)(worker.TimeCompleted - worker.TimeStarted).TotalMilliseconds;
			stringBuilder.AppendFormat("{1} after {2} milliseconds. ", worker.Protocol, worker.State, num);
			if (worker.State != ProbeState.Passed)
			{
				base.Result.StateAttribute2 = string.Format("{0} {1}", base.Definition.Account, base.Definition.AccountPassword);
			}
			switch (worker.State)
			{
			case ProbeState.PreparingRequest:
			case ProbeState.WaitingResponse:
			case ProbeState.Passed:
			case ProbeState.TimedOut:
				break;
			case ProbeState.FailedRequest:
			case ProbeState.FailedResponse:
				if (worker.Error != null && worker.Error.Message != null)
				{
					stringBuilder.Append(worker.Error.Message);
				}
				if (this.response != null)
				{
					if (LocalEndpointManager.IsDataCenter)
					{
						RequestFailureContext requestFailureContext;
						if (RequestFailureContext.TryCreateFromResponse(this.response, out requestFailureContext))
						{
							base.Result.FailureContext = requestFailureContext.ToString();
							base.Result.StateAttribute3 = CafeFailureAnalyser.Instance.Analyse(RequestContext.Monitoring, requestFailureContext).ToString();
							base.Result.StateAttribute4 = requestFailureContext.Serialize();
						}
						else
						{
							base.Result.FailureContext = (base.Result.StateAttribute3 = Strings.NoResponseHeadersAvailable);
						}
					}
					base.Result.StateAttribute1 = (this.response.Headers["request-id"] ?? Strings.NoResponseHeadersAvailable);
				}
				break;
			default:
				stringBuilder.AppendFormat("new state {0} - update ReportHumanReadableProbeDetails!", worker.State);
				break;
			}
			if (this.Verbose)
			{
				stringBuilder.Append("\n\n" + this.breadcrumbs.ToString());
			}
			ProbeResult result = base.Result;
			result.ExecutionContext += stringBuilder.ToString();
		}

		private MailboxDatabaseInfo GetInvokeNowMonitoringAccount()
		{
			LocalEndpointManager instance = LocalEndpointManager.Instance;
			if (!instance.ExchangeServerRoleEndpoint.IsCafeRoleInstalled)
			{
				throw new InvalidOperationException("CAS role is not present on this server");
			}
			if (instance.MailboxDatabaseEndpoint == null || instance.MailboxDatabaseEndpoint.MailboxDatabaseInfoCollectionForCafe.Count == 0)
			{
				throw new InvalidOperationException("Mailbox collection is empty on this server");
			}
			return instance.MailboxDatabaseEndpoint.MailboxDatabaseInfoCollectionForCafe.FirstOrDefault((MailboxDatabaseInfo db) => !string.IsNullOrWhiteSpace(db.MonitoringAccountPassword));
		}

		private string FormatUrl(ProtocolDescriptor protocol)
		{
			string text = base.Definition.Endpoint;
			if (string.IsNullOrEmpty(text))
			{
				throw new InvalidOperationException("no URL base from probe config");
			}
			text = text.TrimEnd(CafeLocalProbe.TrailingSlash);
			string text2 = string.Format("{0}/{1}/", text, protocol.VirtualDirectory);
			if (protocol.HttpProtocol == HttpProtocol.RPC)
			{
				text2 += string.Format("rpcproxy.dll?{0}@{1}", this.ReadAttribute("MailboxGuid", string.Empty), this.ReadAttribute("AccountDomain", string.Empty));
			}
			else if (protocol.HttpProtocol == HttpProtocol.OWACalendar)
			{
				text2 += string.Format("{0}/calendar/calendar.html", base.Definition.Account);
			}
			else if (protocol.HttpProtocol == HttpProtocol.XRop)
			{
				text2 += string.Format("?X-AnchorMailbox={0}", base.Definition.Account);
			}
			else if (CafeLocalProbe.UseFBAAuthForProtocol(protocol, LocalEndpointManager.IsDataCenter))
			{
				text2 = string.Format("{0}/{1}/auth.owa", text, CafeProtocols.Get(HttpProtocol.OWA).VirtualDirectory);
			}
			else if (protocol.HttpProtocol == HttpProtocol.Mapi)
			{
				text2 += string.Format("emsmdb?mailboxId={0}@{1}", this.ReadAttribute("MailboxGuid", string.Empty), this.ReadAttribute("AccountDomain", string.Empty));
			}
			return text2;
		}

		private AuthenticationMethod? GetAuthTypeForProtocol(ProtocolDescriptor protocol)
		{
			AuthenticationMethod[] array = LocalEndpointManager.IsDataCenter ? protocol.AuthPreferenceOrderDatacenter : protocol.AuthPreferenceOrderEnterprise;
			if (array == null || array.Length == 0)
			{
				return null;
			}
			if (LocalEndpointManager.IsDataCenter)
			{
				return new AuthenticationMethod?(array[0]);
			}
			string[] source = this.ReadAttribute(CafeLocalProbe.AuthenticationTypes, string.Empty).Split(new char[]
			{
				','
			});
			foreach (AuthenticationMethod authenticationMethod in array)
			{
				if (source.Contains(authenticationMethod.ToString()))
				{
					return new AuthenticationMethod?(authenticationMethod);
				}
			}
			return null;
		}

		public const int ProtocolCount = 13;

		public const int FailureThreshold = 0;

		public const string ProbeHeaderName = "X-ProbeType";

		public const string ProbeHeaderValue = "X-MS-ClientAccess-LocalProbe";

		public const string UserAgentValue = "AMProbe/Local/ClientAccess";

		private const int MinimumTimeLimit = 5000;

		private static readonly char[] TrailingSlash = new char[]
		{
			'/'
		};

		private DateTime startTime;

		private CancellationToken cancellationToken;

		private Breadcrumbs breadcrumbs;

		private HttpWebRequest request;

		private HttpWebResponse response;

		public static readonly string AuthenticationTypes = "AuthenticationTypes";

		internal class HttpProtocolProbe
		{
			public HttpProtocolProbe(ProtocolDescriptor protocol, ProbeState initialState)
			{
				this.Protocol = protocol;
				this.State = initialState;
				this.TimeStarted = DateTime.UtcNow;
			}

			public ProtocolDescriptor Protocol { get; private set; }

			public ProbeState State { get; set; }

			public string Url { get; set; }

			public Exception Error { get; set; }

			public Task ResponseTask { get; set; }

			public DateTime TimeStarted { get; private set; }

			public DateTime TimeCompleted { get; set; }
		}
	}
}
