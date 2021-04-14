using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common.SmartCategorization;
using Microsoft.Exchange.WebServices.Autodiscover;
using Microsoft.Exchange.WebServices.Data;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Ews.Probes
{
	public abstract class EWSCommon : ProbeWorkItem
	{
		protected EWSCommon()
		{
			this.VerboseResultPairs = new Dictionary<string, string>();
			this.VitalResultPairs = new Dictionary<string, string>();
			this.TraceBuilder = new StringBuilder();
			this.LatencyMeasurement = new Stopwatch();
			this.Tracer = ExTraceGlobals.EWSTracer;
			this.Breadcrumbs = new Breadcrumbs(1024);
			this.InitErrorClassificationDictionaries();
			this.ProbeStartTime = DateTime.UtcNow;
			this.Breadcrumbs.Drop("EWSCommon start: {0}", new object[]
			{
				this.ProbeStartTime
			});
		}

		protected Microsoft.Exchange.Diagnostics.Trace Tracer { get; set; }

		protected string ClientStatistics { get; set; }

		protected Dictionary<string, string> VerboseResultPairs { get; set; }

		protected StringBuilder TraceBuilder { get; set; }

		protected Dictionary<string, string> VitalResultPairs { get; set; }

		protected Stopwatch LatencyMeasurement { get; set; }

		protected int ApiRetryCount { get; set; }

		protected int ApiRetrySleepInMilliseconds { get; set; }

		protected int MailboxVersion { get; set; }

		protected string UnifiedNamespace { get; set; }

		protected bool CafeVipModeWhenPossible { get; set; }

		protected string AuthenticationMethod { get; set; }

		protected bool UseXropEndpoint { get; set; }

		protected int TargetPort { get; set; }

		protected string AuthenticationDomain { get; set; }

		protected ProbeAuthNType PrimaryAuthN { get; set; }

		protected ProbeAuthNType AlternateAuthN { get; set; }

		protected bool IsOutsideInMonitoring { get; set; }

		public Datacenter.ExchangeSku ExchangeSku { get; set; }

		protected bool TrustAnySSLCertificate { get; set; }

		private protected int ProbeTimeLimit { protected get; private set; }

		private protected int HttpRequestTimeout { protected get; private set; }

		private protected int SslValidationDelaySeconds { protected get; private set; }

		private protected int PreRequestDelaySeconds { protected get; private set; }

		protected string UserAgentPart { get; set; }

		protected bool Verbose { get; set; }

		protected DateTime ProbeStartTime { get; set; }

		protected ProbeAuthNType EffectiveAuthN
		{
			get
			{
				if (this.PrimaryAuthN == ProbeAuthNType.LiveIdOrNegotiate)
				{
					if (Datacenter.ExchangeSku.ExchangeDatacenter != this.ExchangeSku)
					{
						return ProbeAuthNType.Negotiate;
					}
					return ProbeAuthNType.LiveId;
				}
				else
				{
					if (this.PrimaryAuthN != ProbeAuthNType.LiveIdOrCafe)
					{
						return this.PrimaryAuthN;
					}
					if (Datacenter.ExchangeSku.ExchangeDatacenter != this.ExchangeSku)
					{
						return ProbeAuthNType.Cafe;
					}
					return ProbeAuthNType.LiveId;
				}
			}
		}

		private protected Breadcrumbs Breadcrumbs { protected get; private set; }

		private static Dictionary<int, EWSAutoDProtocolDependency> KnownHTTPErrors { get; set; }

		private static Dictionary<ServiceError, EWSAutoDProtocolDependency> KnownEWSErrorCodes { get; set; }

		protected string TransformResultPairsToString(Dictionary<string, string> keyPairs)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (string text in keyPairs.Keys)
			{
				string arg = text;
				EWSExtensions.TryConvertRegularLogKey(text, out arg);
				if (string.IsNullOrEmpty(keyPairs[text]))
				{
					stringBuilder.AppendFormat("{0}{1}", arg, Environment.NewLine);
				}
				else
				{
					stringBuilder.AppendFormat("{0}={1}{2}", arg, keyPairs[text], Environment.NewLine);
				}
			}
			return stringBuilder.ToString();
		}

		protected ExchangeService GetExchangeService(string username, string password, ITraceListener traceListener, ExchangeVersion version)
		{
			return this.GetExchangeService(username, password, traceListener, base.Definition.Endpoint, version);
		}

		protected ExchangeService GetExchangeService(string username, string password, ITraceListener traceListener, string ewsURL, ExchangeVersion version)
		{
			this.VerboseResultPairs.AddUnique("UserName/Password", string.Format("{0}/{1}", username, this.ReadAttribute("HidePasswordInLog", true) ? "******" : password));
			ExchangeService exchangeService = new ExchangeService(version);
			this.SetupTracer(exchangeService, traceListener);
			exchangeService.PreAuthenticate = true;
			exchangeService.Url = new Uri(ewsURL);
			exchangeService.UserAgent = this.UserAgentPart;
			exchangeService.KeepAlive = false;
			if (!this.IsOutsideInMonitoring)
			{
				exchangeService.WebProxy = NullWebProxy.Instance;
			}
			this.ApplyAuthentication(exchangeService, exchangeService.Url, username, password, traceListener);
			if (this.HttpRequestTimeout > 0)
			{
				this.Breadcrumbs.Drop("using HTTP request timeout: {0} ms", new object[]
				{
					this.HttpRequestTimeout
				});
				exchangeService.Timeout = this.HttpRequestTimeout;
			}
			else
			{
				this.Breadcrumbs.Drop("using HTTP request timeout: NONE", new object[]
				{
					this.HttpRequestTimeout
				});
			}
			return exchangeService;
		}

		protected void ApplyAuthentication(ExchangeServiceBase service, Uri url, string username, string password, ITraceListener traceListener)
		{
			string text = (service is AutodiscoverService) ? "Autodiscover" : "EWS";
			switch (this.EffectiveAuthN)
			{
			case ProbeAuthNType.LiveIdOrNegotiate:
				throw new InvalidOperationException("EWSCommon.ApplyAuthentication: EffectiveAuthentication accessor is broken!");
			case ProbeAuthNType.LiveId:
				this.AuthenticationMethod = "Basic";
				this.LogTrace(string.Format("{0} URL: {1} Using {2} authentication.", text, url, this.AuthenticationMethod));
				if (text.Equals("Autodiscover") && this.IsOutsideInMonitoring)
				{
					string arg = Convert.ToBase64String(Encoding.ASCII.GetBytes(string.Format("{0}:{1}", username, password)));
					service.HttpHeaders["Authorization"] = string.Format("Basic {0}", arg);
				}
				else
				{
					service.Credentials = new CredentialCache
					{
						{
							url,
							"Basic",
							new NetworkCredential(username, password)
						}
					};
				}
				break;
			case ProbeAuthNType.Negotiate:
				this.AuthenticationMethod = "Negotiate";
				this.LogTrace(string.Format("{0} URL: {1} Using {2} authentication.", text, url, this.AuthenticationMethod));
				if (!string.IsNullOrEmpty(this.AuthenticationDomain))
				{
					SmtpAddress smtpAddress = SmtpAddress.Parse(username);
					this.LogTrace(string.Format("user: {0} domain: {1}", smtpAddress.Local, this.AuthenticationDomain));
					service.Credentials = new NetworkCredential(smtpAddress.Local, password, this.AuthenticationDomain);
				}
				else
				{
					this.LogTrace(string.Format("user: {0}", username));
					service.Credentials = new NetworkCredential(username, password);
				}
				break;
			case ProbeAuthNType.Cafe:
				this.AuthenticationMethod = "CAFE";
				this.LogTrace(string.Format("{0} URL: {1} Using {2} authentication.", text, url, this.AuthenticationMethod));
				service.ApplyCafeAuthentication(username, password);
				break;
			case ProbeAuthNType.Web:
				this.AuthenticationMethod = "Web";
				this.LogTrace(string.Format("{0} URL: {1} Using {2} authentication.", text, url, this.AuthenticationMethod));
				service.Credentials = new WebCredentials(username, password);
				break;
			default:
				throw new InvalidOperationException("EWSCommon.ApplyAuthentication: do not attempt to use ProbeAuthNType.Unused!");
			}
			this.Breadcrumbs.Drop("using authN: {0} {1} {2}", new object[]
			{
				this.AuthenticationMethod,
				username,
				this.ReadAttribute("HidePasswordInLog", true) ? "******" : password
			});
			this.VerboseResultPairs.AddUnique("AuthMethod", this.AuthenticationMethod);
		}

		protected void Initialize(Microsoft.Exchange.Diagnostics.Trace tracer)
		{
			this.Tracer = tracer;
			this.ProbeStartTime = DateTime.UtcNow;
			this.Configure();
		}

		protected virtual void Configure()
		{
			this.Breadcrumbs.Drop("Configuring EWScommon");
			this.ApiRetryCount = this.ReadAttribute("ApiRetryCount", 1);
			this.ApiRetrySleepInMilliseconds = this.ReadAttribute("ApiRetrySleepInMilliseconds", 5000);
			this.UseXropEndpoint = this.ReadAttribute("UseXropEndPoint", false);
			this.TargetPort = this.ReadAttribute("TargetPort", -1);
			this.MailboxVersion = this.ReadAttribute("MailboxVersion", 14);
			this.UnifiedNamespace = this.ReadAttribute("UnifiedNamespace", "outlook.com");
			this.CafeVipModeWhenPossible = this.ReadAttribute("Cas15VipModeWhenPossible", false);
			this.AuthenticationDomain = this.ReadAttribute("Domain", null);
			this.IsOutsideInMonitoring = this.ReadAttribute("IsOutsideInMonitoring", true);
			if (this.IsOutsideInMonitoring)
			{
				this.ExchangeSku = this.ReadAttribute("ExchangeSku", Datacenter.ExchangeSku.ExchangeDatacenter);
			}
			else
			{
				this.ExchangeSku = Datacenter.GetExchangeSku();
			}
			this.PrimaryAuthN = this.ReadAttribute("PrimaryAuthN", ProbeAuthNType.LiveIdOrNegotiate);
			this.AlternateAuthN = this.ReadAttribute("AlternateAuthN", ProbeAuthNType.Unused);
			this.TrustAnySSLCertificate = this.ReadAttribute("TrustAnySslCertificate", false);
			this.UserAgentPart = this.ReadAttribute("UserAgentPart", "AMProbe");
			this.Verbose = this.ReadAttribute("Verbose", true);
			int val = 1000 * ((base.Definition.TimeoutSeconds > 0) ? base.Definition.TimeoutSeconds : 180);
			this.ProbeTimeLimit = Math.Max(val, 5000);
			this.HttpRequestTimeout = (int)this.ReadAttribute("HttpRequestTimeoutSpan", TimeSpan.FromMilliseconds((double)((this.ProbeTimeLimit - 1000) / (this.ApiRetryCount + 1)))).TotalMilliseconds;
			this.Breadcrumbs.Drop("Probe time limit: {0}ms, HTTP timeout: {1}ms, RetryCount: {2}", new object[]
			{
				this.ProbeTimeLimit,
				this.HttpRequestTimeout,
				this.ApiRetryCount
			});
			this.SslValidationDelaySeconds = Utils.LoadAppconfigIntSetting("SslValidationDelaySeconds", 0);
			this.PreRequestDelaySeconds = Utils.LoadAppconfigIntSetting("PreRequestDelaySeconds", 0);
			if (this.IsOutsideInMonitoring)
			{
				if (Datacenter.ExchangeSku.ExchangeDatacenter == this.ExchangeSku)
				{
					this.PrimaryAuthN = ProbeAuthNType.LiveId;
					this.AlternateAuthN = ProbeAuthNType.Negotiate;
				}
				if (this.UserAgentPart.Equals("AMProbe"))
				{
					this.UserAgentPart = "AMProbe/OutsideIn/" + Guid.NewGuid().ToString();
					this.Breadcrumbs.Drop("UserAgent: {0}", new object[]
					{
						this.UserAgentPart
					});
				}
			}
		}

		protected ProbeAuthNType ReadAttribute(string name, ProbeAuthNType defaultValue)
		{
			string value = this.ReadAttribute(name, string.Empty);
			ProbeAuthNType result;
			if (!string.IsNullOrEmpty(value) && Enum.TryParse<ProbeAuthNType>(value, true, out result))
			{
				return result;
			}
			return defaultValue;
		}

		protected Datacenter.ExchangeSku ReadAttribute(string name, Datacenter.ExchangeSku defaultValue)
		{
			string value = this.ReadAttribute(name, string.Empty);
			Datacenter.ExchangeSku result;
			if (!string.IsNullOrEmpty(value) && Enum.TryParse<Datacenter.ExchangeSku>(value, true, out result))
			{
				return result;
			}
			return defaultValue;
		}

		protected void SetClientStatistics(string requestId, string soapAction, double clientResponseTimeInMs)
		{
			this.ClientStatistics = string.Format("MessageId={0},SoapAction={1},ResponseTime={2};", requestId, soapAction, clientResponseTimeInMs);
		}

		protected void SetupTracer(ExchangeServiceBase service, ITraceListener traceListener)
		{
			service.TraceListener = traceListener;
			service.TraceFlags = 54L;
			service.TraceEnabled = true;
		}

		protected void LogTrace(string msg)
		{
			WTFDiagnostics.TraceInformation(this.Tracer, base.TraceContext, msg, null, "LogTrace", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Ews\\EWSCommon.cs", 686);
		}

		protected void WriteErrorData(object key, object exceptiondata, string logDetails = "")
		{
			this.LatencyMeasurement.Stop();
			base.Result.StateAttribute20 = (double)this.LatencyMeasurement.ElapsedMilliseconds;
			this.VitalResultPairs.AddUnique(string.Format("Latency(failed_tag={0})", key), base.Result.StateAttribute20.ToString());
			this.VerboseResultPairs.AddUnique(string.Format("{0}:Exception", key), exceptiondata.ToString());
			if (!string.IsNullOrEmpty(logDetails))
			{
				this.TraceBuilder.AppendLine(logDetails);
			}
			if (this.Verbose)
			{
				ProbeResult result = base.Result;
				result.ExecutionContext += this.TransformResultPairsToString(this.VerboseResultPairs);
				this.SplitTraceDataInto1024ChunksAndWriteitToDatabaseFields();
			}
			this.WriteVitalInfoToExecutionContext();
		}

		protected void SplitTraceDataInto1024ChunksAndWriteitToDatabaseFields()
		{
			int num = 1024;
			string text = this.TraceBuilder.ToString();
			List<string> list = new List<string>(text.Length / num + 1);
			for (int i = 0; i < text.Length; i += num)
			{
				list.Add(text.Substring(i, Math.Min(num, text.Length - i)));
			}
			int num2 = 0;
			if (list.Count > num2)
			{
				base.Result.FailureContext = list[num2];
				num2++;
			}
			if (list.Count > num2)
			{
				base.Result.StateAttribute22 = list[num2];
				num2++;
			}
			if (list.Count > num2)
			{
				base.Result.StateAttribute23 = list[num2];
				num2++;
			}
			if (list.Count > num2)
			{
				base.Result.StateAttribute24 = list[num2];
				num2++;
			}
			if (list.Count > num2)
			{
				base.Result.StateAttribute25 = list[num2];
				num2++;
			}
		}

		protected void ThrowError(object key, object exceptiondata, string logDetails = "")
		{
			this.SetDefaultValuesForStateAttributes1And2();
			this.WriteErrorData(key, exceptiondata, logDetails);
			throw new Exception(exceptiondata.ToString());
		}

		protected string ConstructEwsUrl()
		{
			return this.ConstructEwsUrl(base.Definition.Endpoint);
		}

		protected string ConstructEwsUrl(string endPoint)
		{
			Uri uri = new Uri(endPoint);
			string text = this.UseXropEndpoint ? "xrop-" : string.Empty;
			string text2 = uri.Authority;
			if (CafeUtils.RunningUnderCafeVipMode && 15 == this.MailboxVersion)
			{
				text = string.Empty;
				text2 = CafeUtils.CurrentCafeVipAddress.ToString();
			}
			Uri uri2;
			if (this.TargetPort != -1)
			{
				uri2 = new Uri(string.Format("https://{0}{1}:{2}{3}", new object[]
				{
					text,
					text2,
					this.TargetPort,
					uri.PathAndQuery
				}));
			}
			else
			{
				uri2 = new Uri(string.Format("https://{0}{1}{2}", text, text2, uri.PathAndQuery));
			}
			this.VerboseResultPairs.AddUnique("EwsUrl", uri2.ToString());
			return uri2.ToString();
		}

		protected void LogIpAddresses(Uri uri)
		{
			string key = string.Format("ip-addresses/{0}", uri.Host);
			try
			{
				IPAddress[] hostAddresses = Dns.GetHostAddresses(uri.Host);
				StringBuilder stringBuilder = new StringBuilder(1024);
				if (hostAddresses != null && hostAddresses.Length > 0)
				{
					stringBuilder.Append(hostAddresses[0]);
					for (int i = 1; i < hostAddresses.Length; i++)
					{
						stringBuilder.AppendFormat("{0}{1}", "; ", hostAddresses[i]);
					}
					this.VerboseResultPairs.AddUnique(key, stringBuilder.ToString());
				}
			}
			catch (Exception)
			{
				this.VerboseResultPairs.AddUnique(key, "could not be resolved");
			}
		}

		protected void DecorateLogSection(string title)
		{
			this.VerboseResultPairs.AddUnique(string.Empty, string.Empty);
			this.VerboseResultPairs.AddUnique(string.Format(">>> {0}", title), string.Empty);
			this.VerboseResultPairs.AddUnique(string.Empty, string.Empty);
		}

		protected void WriteVitalInfoToExecutionContext()
		{
			ProbeResult result = base.Result;
			result.ExecutionContext += "Important:\r\n";
			ProbeResult result2 = base.Result;
			result2.ExecutionContext += this.TransformResultPairsToString(this.VitalResultPairs);
		}

		protected void RetrySoapActionAndThrow(Action operation, string soapAction, ExchangeServiceBase service)
		{
			this.RetrySoapActionAndThrow(operation, soapAction, service, CancellationToken.None, false);
		}

		protected void RetrySoapActionAndThrow(Action operation, string soapAction, ExchangeServiceBase service, CancellationToken cancellationToken)
		{
			this.RetrySoapActionAndThrow(operation, soapAction, service, cancellationToken, false);
		}

		protected void RetrySoapActionAndThrow(Action operation, string soapAction, ExchangeServiceBase service, bool trackLatency)
		{
			this.RetrySoapActionAndThrow(operation, soapAction, service, CancellationToken.None, trackLatency);
		}

		protected void RetrySoapActionAndThrow(Action operation, string soapAction, ExchangeServiceBase service, CancellationToken cancellationToken, bool trackLatency)
		{
			this.Breadcrumbs.Drop("Entering RetrySoapActionAndThrow. soapAction = {0}.", new object[]
			{
				soapAction
			});
			DateTime utcNow = DateTime.UtcNow;
			int num = this.ApiRetryCount;
			Stopwatch stopwatch = new Stopwatch();
			if (trackLatency)
			{
				base.Result.SampleValue = 0.0;
			}
			for (int i = 0; i <= num; i++)
			{
				try
				{
					this.Breadcrumbs.Drop("Action iteration {0}", new object[]
					{
						i
					});
					if (this.ClientStatistics != null)
					{
						service.HttpHeaders["X-ClientStatistics"] = this.ClientStatistics;
						this.ClientStatistics = null;
					}
					utcNow = DateTime.UtcNow;
					TimeSpan timeSpan = DateTime.UtcNow - base.Result.ExecutionStartTime;
					int num2 = Math.Max(0, this.ProbeTimeLimit - (int)timeSpan.TotalMilliseconds);
					this.Breadcrumbs.Drop("Starting (total time left {0} ms)", new object[]
					{
						num2
					});
					Task task = new Task(delegate()
					{
						if (this.PreRequestDelaySeconds > 0)
						{
							Thread.Sleep(this.PreRequestDelaySeconds * 1000);
						}
						operation();
					});
					if (trackLatency)
					{
						stopwatch.Start();
					}
					task.Start();
					if (!task.IsCompleted && num2 > 0)
					{
						task.Wait(num2);
					}
					if (trackLatency)
					{
						stopwatch.Stop();
						base.Result.SampleValue = base.Result.SampleValue + (double)stopwatch.ElapsedMilliseconds;
					}
					DateTime utcNow2 = DateTime.UtcNow;
					if (!task.IsCompleted || cancellationToken.IsCancellationRequested)
					{
						this.Breadcrumbs.Drop("Action timed out!");
						string message = string.Format("Iteration {0}; {1} seconds elapsed", i, (utcNow2 - utcNow).TotalSeconds);
						throw new TimeoutException(message);
					}
					this.Breadcrumbs.Drop("Action completed normally");
					this.UpdateStatsAndLogs(service, soapAction, utcNow, DateTime.UtcNow, "success", i);
					break;
				}
				catch (Exception innerException)
				{
					if (trackLatency)
					{
						stopwatch.Stop();
						base.Result.SampleValue = base.Result.SampleValue + (double)stopwatch.ElapsedMilliseconds;
					}
					AggregateException ex = innerException as AggregateException;
					if (ex != null)
					{
						innerException = ex.Flatten().InnerException;
					}
					this.Breadcrumbs.Drop("Action threw {0}: {1}", new object[]
					{
						innerException.GetType().ToString(),
						innerException.Message
					});
					if (innerException.Message.Contains("The underlying connection was closed") && num == 0)
					{
						this.Breadcrumbs.Drop("Exception was related to connection being closed. Setting internalRetryCount to 1.");
						num = 1;
					}
					else
					{
						this.VitalResultPairs.AddUnique(string.Format("{0} (Attempt #{1})/exception", soapAction, i), innerException.Message);
						this.UpdateStatsAndLogs(service, soapAction, utcNow, DateTime.UtcNow, innerException.Message, i);
						if (num == i)
						{
							this.ClassifyErrorAndUpdateStateAttributes(innerException, service);
							this.Breadcrumbs.Drop("EWS call {0} failed after retrying {1} time(s)", new object[]
							{
								soapAction,
								i
							});
							this.TraceBuilder.AppendLine(((EWSCommon.TraceListener)service.TraceListener).RequestLog);
							StringBuilder stringBuilder = new StringBuilder(this.TransformResultPairsToString(this.VerboseResultPairs));
							this.VerboseResultPairs.Clear();
							this.VitalResultPairs.Clear();
							throw new Exception(stringBuilder.ToString());
						}
					}
					this.LogTrace("EWS call failed - attempt count: " + i.ToString());
					Thread.Sleep(TimeSpan.FromMilliseconds((double)this.ApiRetrySleepInMilliseconds));
				}
				finally
				{
					ProbeResult result = base.Result;
					result.StateAttribute21 += this.Breadcrumbs.ToString();
					this.Breadcrumbs.Clear();
				}
			}
		}

		protected string GetFailureContext(IDictionary<string, string> headerContent)
		{
			RequestFailureContext requestFailureContext;
			if (RequestFailureContext.TryCreateFromResponseHeaders(headerContent, out requestFailureContext))
			{
				string text = CafeFailureAnalyser.Instance.Analyse(RequestContext.Monitoring, requestFailureContext).ToString() + "\n" + requestFailureContext.ToString();
				this.LogTrace(text);
				return text;
			}
			return string.Empty;
		}

		protected void UpdateLogWithHeader(string[] headers, IDictionary<string, string> headerContent, int iteration, StateAttribute[] updateAttributePositions)
		{
			if (headers == null)
			{
				throw new ArgumentException("headers can not be null.");
			}
			if (headerContent == null)
			{
				throw new ArgumentException("headerContent can not be null.");
			}
			if (updateAttributePositions == null)
			{
				throw new ArgumentException("updateAttributePositions can not be null.");
			}
			if (headers.Length != updateAttributePositions.Length)
			{
				throw new ArgumentException("headers array size and updateAttributePositions need to be equal.");
			}
			for (int i = 0; i < headers.Length; i++)
			{
				if (headerContent.ContainsKey(headers[i]))
				{
					this.VerboseResultPairs.AddUnique(string.Format("(Attempt #{0}) {1}", iteration, headers[i]), headerContent[headers[i]]);
					switch (updateAttributePositions[i])
					{
					case StateAttribute.StateAttribute3:
						base.Result.StateAttribute3 = headerContent[headers[i]];
						break;
					case StateAttribute.StateAttribute4:
						base.Result.StateAttribute4 = headerContent[headers[i]];
						break;
					case StateAttribute.StateAttribute11:
						base.Result.StateAttribute11 = headerContent[headers[i]];
						break;
					case StateAttribute.StateAttribute12:
						base.Result.StateAttribute12 = headerContent[headers[i]];
						break;
					case StateAttribute.StateAttribute13:
						base.Result.StateAttribute13 = headerContent[headers[i]];
						break;
					case StateAttribute.StateAttribute14:
						base.Result.StateAttribute14 = headerContent[headers[i]];
						break;
					case StateAttribute.StateAttribute15:
						base.Result.StateAttribute15 = headerContent[headers[i]];
						break;
					}
				}
			}
		}

		private void ClassifyErrorAndUpdateStateAttributes(Exception ewsException, ExchangeServiceBase service)
		{
			Exception ex = ewsException;
			this.SetDefaultValuesForStateAttributes1And2();
			try
			{
				FailureDetails failureDetails = null;
				RequestFailureContext requestFailureContext;
				if (RequestFailureContext.TryCreateFromResponseHeaders(service.HttpResponseHeaders, out requestFailureContext))
				{
					failureDetails = CafeFailureAnalyser.Instance.Analyse(RequestContext.Monitoring, requestFailureContext);
				}
				if (failureDetails != null)
				{
					if (failureDetails.Component == ExchangeComponent.Cafe)
					{
						base.Result.FailureCategory = 4;
						base.Result.StateAttribute1 = EWSAutoDProtocolDependency.Cafe.ToString();
					}
					else if (failureDetails.Component == ExchangeComponent.AD)
					{
						base.Result.FailureCategory = 5;
						base.Result.StateAttribute1 = EWSAutoDProtocolDependency.AD.ToString();
					}
					else if (failureDetails.Component == ExchangeComponent.Gls)
					{
						base.Result.FailureCategory = 6;
						base.Result.StateAttribute1 = EWSAutoDProtocolDependency.Gls.ToString();
					}
					else if (failureDetails.Component == ExchangeComponent.LiveId)
					{
						base.Result.FailureCategory = 7;
						base.Result.StateAttribute1 = EWSAutoDProtocolDependency.LiveId.ToString();
					}
					else if (failureDetails.Component == ExchangeComponent.Monitoring)
					{
						base.Result.FailureCategory = 8;
						base.Result.StateAttribute1 = EWSAutoDProtocolDependency.Monitoring.ToString();
					}
					base.Result.StateAttribute2 = failureDetails.Categorization;
				}
				else if (ewsException.GetType().Equals(typeof(ServiceResponseException)))
				{
					ServiceResponseException ex2 = ewsException as ServiceResponseException;
					if (ex2 != null)
					{
						base.Result.StateAttribute2 = ex2.ErrorCode.ToString();
						if (EWSCommon.KnownEWSErrorCodes.ContainsKey(ex2.ErrorCode))
						{
							base.Result.FailureCategory = (int)EWSCommon.KnownEWSErrorCodes[ex2.ErrorCode];
							base.Result.StateAttribute1 = EWSCommon.KnownEWSErrorCodes[ex2.ErrorCode].ToString();
						}
						else
						{
							base.Result.FailureCategory = 1;
							base.Result.StateAttribute1 = EWSAutoDProtocolDependency.EWS.ToString();
						}
					}
				}
				else if (ewsException.GetType().Equals(typeof(ServiceRequestException)))
				{
					while (ex.InnerException != null)
					{
						ex = ex.InnerException;
					}
					if (ex.GetType().Equals(typeof(WebException)))
					{
						HttpWebResponse httpWebResponse = ((WebException)ex).Response as HttpWebResponse;
						if (httpWebResponse != null)
						{
							int statusCode = (int)httpWebResponse.StatusCode;
							base.Result.StateAttribute2 = httpWebResponse.StatusCode.ToString();
							if (EWSCommon.KnownHTTPErrors.ContainsKey(statusCode))
							{
								base.Result.FailureCategory = (int)EWSCommon.KnownHTTPErrors[statusCode];
								base.Result.StateAttribute1 = EWSCommon.KnownHTTPErrors[statusCode].ToString();
							}
							else
							{
								base.Result.FailureCategory = 1;
								base.Result.StateAttribute1 = EWSAutoDProtocolDependency.EWS.ToString();
							}
						}
					}
					else if (ex.GetType().Equals(typeof(SocketException)))
					{
						base.Result.StateAttribute2 = ((SocketException)ex).SocketErrorCode.ToString();
						base.Result.FailureCategory = 2;
						base.Result.StateAttribute1 = EWSAutoDProtocolDependency.Network.ToString();
					}
				}
			}
			catch (Exception)
			{
				this.SetDefaultValuesForStateAttributes1And2();
			}
		}

		private void SetDefaultValuesForStateAttributes1And2()
		{
			if (string.IsNullOrEmpty(base.Result.StateAttribute2))
			{
				base.Result.StateAttribute2 = "Unknown";
			}
			if (string.IsNullOrEmpty(base.Result.StateAttribute1))
			{
				base.Result.FailureCategory = 1;
				base.Result.StateAttribute1 = EWSAutoDProtocolDependency.EWS.ToString();
			}
		}

		private void InitErrorClassificationDictionaries()
		{
			EWSCommon.KnownHTTPErrors = new Dictionary<int, EWSAutoDProtocolDependency>();
			EWSCommon.KnownHTTPErrors[401] = EWSAutoDProtocolDependency.Auth;
			EWSCommon.KnownEWSErrorCodes = new Dictionary<ServiceError, EWSAutoDProtocolDependency>();
			EWSCommon.KnownEWSErrorCodes[263] = EWSAutoDProtocolDependency.Store;
			EWSCommon.KnownEWSErrorCodes[262] = EWSAutoDProtocolDependency.Store;
			EWSCommon.KnownEWSErrorCodes[6] = EWSAutoDProtocolDependency.AD;
			EWSCommon.KnownEWSErrorCodes[7] = EWSAutoDProtocolDependency.AD;
			EWSCommon.KnownEWSErrorCodes[8] = EWSAutoDProtocolDependency.AD;
		}

		private void UpdateStatsAndLogs(ExchangeServiceBase service, string soapAction, DateTime startTime, DateTime endTime, string traceMsg, int attemptCount)
		{
			string arg = string.Format("{0} (Attempt #{1})", soapAction, attemptCount);
			if (service.HttpResponseHeaders.ContainsKey("RequestId"))
			{
				this.SetClientStatistics(service.HttpResponseHeaders["RequestId"], soapAction, (endTime - startTime).TotalMilliseconds);
			}
			this.VerboseResultPairs.AddUnique(string.Format("{0} Status", arg), traceMsg);
			this.VerboseResultPairs.AddUnique(string.Format("{0} Latency", arg), (endTime - startTime).TotalMilliseconds.ToString());
			this.VitalResultPairs.AddUnique(string.Format("{0} Latency", arg), (endTime - startTime).TotalMilliseconds.ToString());
			if (service.HttpResponseHeaders != null)
			{
				this.UpdateLogWithHeader(new string[]
				{
					"X-DiagInfo",
					"X-FEServer",
					"X-BEServer",
					"X-TargetBEServer"
				}, service.HttpResponseHeaders, attemptCount, new StateAttribute[]
				{
					StateAttribute.StateAttribute11,
					StateAttribute.StateAttribute12,
					StateAttribute.StateAttribute13,
					StateAttribute.StateAttribute14
				});
				base.Result.StateAttribute5 = this.GetFailureContext(service.HttpResponseHeaders);
			}
			this.LogTrace(traceMsg);
		}

		private bool HidePasswordInLog()
		{
			bool result;
			if (base.Definition.Attributes.ContainsKey("HidePasswordInLog"))
			{
				string value = base.Definition.Attributes["HidePasswordInLog"];
				if (!bool.TryParse(value, out result))
				{
					result = false;
				}
			}
			else
			{
				result = false;
			}
			return result;
		}

		protected class TraceListener : ITraceListener
		{
			public TraceListener(TracingContext tracingContext, Microsoft.Exchange.Diagnostics.Trace exTraceGlobal)
			{
				this.tracingContext = tracingContext;
				this.exTraceGlobal = exTraceGlobal;
			}

			public string RequestLog { get; internal set; }

			public void Trace(string traceType, string traceMessage)
			{
				this.RequestLog += string.Format("{0}: {1}\n", traceType, traceMessage);
				WTFDiagnostics.TraceInformation<string, string>(this.exTraceGlobal, this.tracingContext, "Tracetype: {0}\nTraceMessage: {1}\n", traceType, traceMessage, null, "Trace", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Ews\\EWSCommon.cs", 1464);
			}

			private TracingContext tracingContext;

			private Microsoft.Exchange.Diagnostics.Trace exTraceGlobal;
		}
	}
}
