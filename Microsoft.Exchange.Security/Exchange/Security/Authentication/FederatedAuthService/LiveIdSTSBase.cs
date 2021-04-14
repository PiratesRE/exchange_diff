using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.Security;
using Microsoft.Passport.RPS;

namespace Microsoft.Exchange.Security.Authentication.FederatedAuthService
{
	internal abstract class LiveIdSTSBase : STSBase
	{
		static LiveIdSTSBase()
		{
			LiveIdSTSBase.usAsciiXmlWriterSettings = new XmlWriterSettings();
			LiveIdSTSBase.usAsciiXmlWriterSettings.Encoding = Encoding.ASCII;
			LiveIdSTSBase.usAsciiXmlWriterSettings.OmitXmlDeclaration = true;
			LiveIdSTSBase.usAsciiXmlWriterSettings.ConformanceLevel = ConformanceLevel.Fragment;
			LiveIdSTSBase.RpsTicketLifetime = 3600;
		}

		protected LiveIdSTSBase()
		{
		}

		public LiveIdSTSBase(int traceId, LiveIdInstanceType instance, NamespaceStats stats) : base(traceId, instance, stats)
		{
			this.TokenConsumer = LiveIdSTSBase.SiteName;
		}

		public static string SiteName { get; internal set; }

		public static int RpsTicketLifetime { get; internal set; }

		public string ConnectionGroupName
		{
			get
			{
				if (string.IsNullOrEmpty(this.connectionGroupName))
				{
					this.connectionGroupName = AuthServiceHelper.GetConnectionGroup(this.traceId);
				}
				return this.connectionGroupName;
			}
		}

		public bool EnableRemoteRPS { get; internal set; }

		protected static RPS RpsSession
		{
			get
			{
				if (LiveIdSTSBase.privateRpsSession == null)
				{
					lock (LiveIdSTSBase.lockRoot)
					{
						if (LiveIdSTSBase.privateRpsSession == null)
						{
							try
							{
								RPS rps = new RPS();
								rps.Initialize(null);
								LiveIdSTSBase.privateRpsSession = rps;
								ExTraceGlobals.AuthenticationTracer.TraceDebug(0L, "RPS Session initialized successfully");
							}
							catch (COMException ex)
							{
								ExTraceGlobals.AuthenticationTracer.TraceError<int>(0L, "RPS initialization failed with error {0}", ex.ErrorCode);
								STSBase.eventLogger.LogEvent(SecurityEventLogConstants.Tuple_AuthServiceFailedToInitRPS, null, new object[]
								{
									ex.ErrorCode
								});
								throw;
							}
						}
					}
				}
				return LiveIdSTSBase.privateRpsSession;
			}
		}

		public static bool ParseCompactTicket(string token, int traceId, out RPSProfile rpsProfile, out string errorString)
		{
			if (string.IsNullOrEmpty(token))
			{
				throw new ArgumentException("token");
			}
			rpsProfile = null;
			errorString = null;
			try
			{
				using (RPSTicket rpsticket = new RPSCompactTicket(LiveIdSTSBase.RpsSession))
				{
					rpsticket.ProcessToken(LiveIdSTSBase.SiteName, token);
					using (RPSPropBag rpspropBag = new RPSPropBag(LiveIdSTSBase.RpsSession))
					{
						if (!rpsticket.Validate(rpspropBag))
						{
							int num = (int)rpspropBag["reasonhr"];
							RPSErrorCategory rpserrorCategory = RPSErrorHandler.CategorizeRPSError(num);
							errorString = string.Format("Live token failed Validate() with {0}: {1} error=0x{2:x}.", rpserrorCategory, Enum.GetName(typeof(RPSErrorCode), num) ?? string.Empty, num);
							return false;
						}
					}
					rpsProfile = RPSCommon.ParseRPSTicket(rpsticket, LiveIdSTSBase.RpsTicketLifetime, traceId, true, out errorString, false);
				}
			}
			catch (COMException ex)
			{
				errorString = string.Format("Error parsing compact token {0} {1}", ex.ErrorCode, ex.ToString());
				return false;
			}
			return rpsProfile != null;
		}

		public static string UsAsciiEncodedXml(XmlNode xml)
		{
			string @string;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (XmlWriter xmlWriter = XmlWriter.Create(memoryStream, LiveIdSTSBase.usAsciiXmlWriterSettings))
				{
					xml.WriteTo(xmlWriter);
					xmlWriter.Close();
					@string = Encoding.UTF8.GetString(memoryStream.ToArray());
				}
			}
			return @string;
		}

		public static bool IsPossibleAppPassword(byte[] password)
		{
			if (password == null || password.Length != 16)
			{
				return false;
			}
			foreach (byte b in password)
			{
				if (b < 97 || b > 122)
				{
					return false;
				}
				if (b == 97 || b == 101 || b == 105 || b == 111 || b == 117)
				{
					return false;
				}
			}
			return true;
		}

		public abstract IAsyncResult StartRequestChain(string userId, byte[] token, AsyncCallback callback, object state);

		public abstract IAsyncResult ProcessRequest(IAsyncResult asyncResult, AsyncCallback callback, object state);

		public abstract string ProcessResponse(IAsyncResult asyncResult);

		public virtual void ProcessResponse(string userName, IAsyncResult asyncResult, out string compactTokenInRps, out RPSProfile rpsProfile, out string errorString)
		{
			errorString = null;
			compactTokenInRps = null;
			rpsProfile = null;
			string text = this.ProcessResponse(asyncResult);
			if (!string.IsNullOrEmpty(text))
			{
				compactTokenInRps = this.LiveToken();
			}
			if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(compactTokenInRps))
			{
				return;
			}
			if (base.Instance == LiveIdInstanceType.Business)
			{
				Stopwatch stopwatch = Stopwatch.StartNew();
				try
				{
					if (!this.EnableRemoteRPS)
					{
						if (!LiveIdSTSBase.ParseCompactTicket(compactTokenInRps, this.GetHashCode(), out rpsProfile, out errorString))
						{
							rpsProfile = null;
						}
					}
					else if (!MSATokenValidationClient.Instance.ParseCompactToken(2, compactTokenInRps, LiveIdSTSBase.SiteName, LiveIdSTSBase.RpsTicketLifetime, out rpsProfile, out errorString))
					{
						rpsProfile = null;
					}
					return;
				}
				finally
				{
					stopwatch.Stop();
					base.RPSParseLatency = stopwatch.ElapsedMilliseconds;
					STSBase.counters.AverageRPSCallLatency.IncrementBy(stopwatch.ElapsedMilliseconds);
					STSBase.counters.AverageRPSCallLatencyBase.Increment();
				}
			}
			rpsProfile = new RPSProfile
			{
				HexPuid = text
			};
		}

		public abstract void Abort();

		public abstract string LiveToken();

		public string LiveServer { get; protected set; }

		public virtual string LogonUri { get; set; }

		public virtual bool UserRecoveryPossible()
		{
			return false;
		}

		public string TokenConsumer
		{
			get
			{
				return this.tokenConsumer;
			}
			set
			{
				this.tokenConsumer = value;
				this.tokenConsumerBytes = Encoding.UTF8.GetBytes(value);
			}
		}

		public string TokenIssuerUri
		{
			get
			{
				string text = null;
				FederationTrust federationTrust;
				if (base.Instance == LiveIdInstanceType.Consumer)
				{
					federationTrust = FederationTrustCache.GetFederationTrust("WindowsLiveID");
				}
				else
				{
					federationTrust = FederationTrustCache.GetFederationTrust("MicrosoftOnline");
				}
				if (federationTrust != null && federationTrust.TokenIssuerUri != null)
				{
					text = federationTrust.TokenIssuerUri.ToString();
					ExTraceGlobals.AuthenticationTracer.TraceDebug<string>(0L, "FederationTrust object has issuerUri: {0}", text);
				}
				if (string.IsNullOrEmpty(text))
				{
					if (base.Instance == LiveIdInstanceType.Consumer)
					{
						if (!string.IsNullOrEmpty(this.LogonUri) && this.LogonUri.IndexOf("-int.com", StringComparison.OrdinalIgnoreCase) > 0)
						{
							text = "uri:WindowsLiveIDINT";
						}
						else
						{
							text = "uri:WindowsLiveID";
						}
					}
					else if (!string.IsNullOrEmpty(this.LogonUri) && this.LogonUri.IndexOf("-int.com", StringComparison.OrdinalIgnoreCase) > 0)
					{
						text = "urn:federation:MicrosoftOnline";
					}
					else
					{
						text = "urn:federation:MicrosoftOnlineINT";
					}
					ExTraceGlobals.AuthenticationTracer.TraceWarning<string>(0L, "Could not retrieve issuerUri from FederationTrust object, returning best guess: {0}", text);
				}
				return text;
			}
		}

		public string ProfilePolicy
		{
			get
			{
				if (string.IsNullOrEmpty(this.privateProfilePolicy))
				{
					RPS rpsSession = LiveIdSTSBase.RpsSession;
					lock (LiveIdSTSBase.lockRoot)
					{
						if (string.IsNullOrEmpty(this.privateProfilePolicy))
						{
							try
							{
								using (RPSServerConfig rpsserverConfig = new RPSServerConfig(rpsSession))
								{
									RPSPropBag rpspropBag = (RPSPropBag)rpsserverConfig["sites"];
									RPSPropBag rpspropBag2 = (RPSPropBag)rpspropBag[LiveIdSTSBase.SiteName];
									this.privateProfilePolicy = (string)rpspropBag2["authpolicy"];
								}
							}
							catch (COMException ex)
							{
								ExTraceGlobals.AuthenticationTracer.TraceError<int>(0L, "RPSServerConfig lookup of authpolicy failed with error {0}", ex.ErrorCode);
								STSBase.eventLogger.LogEvent(SecurityEventLogConstants.Tuple_AuthServiceFailedToInitRPS, null, new object[]
								{
									ex.ErrorCode
								});
								throw;
							}
						}
					}
				}
				return this.privateProfilePolicy;
			}
			internal set
			{
				this.privateProfilePolicy = value;
			}
		}

		protected byte[] ProfilePolicyBytes
		{
			get
			{
				if (this.privateProfilePolicyBytes == null)
				{
					this.privateProfilePolicyBytes = Encoding.UTF8.GetBytes(this.ProfilePolicy);
				}
				return this.privateProfilePolicyBytes;
			}
		}

		protected void LogRequest(byte[] requestBody, HttpWebRequest request, string redactedCreds = null)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("Request URI: {0} {1}", request.Method, request.RequestUri);
			foreach (string text in request.Headers.AllKeys)
			{
				stringBuilder.AppendLine();
				if (redactedCreds != null && string.Equals(text, "Authorization", StringComparison.OrdinalIgnoreCase))
				{
					stringBuilder.AppendFormat("{0}: {1}", text, redactedCreds);
				}
				else
				{
					stringBuilder.AppendFormat("{0}: {1}", text, request.Headers[text]);
				}
			}
			stringBuilder.AppendLine();
			stringBuilder.Append(Encoding.UTF8.GetString(requestBody));
			string arg = stringBuilder.ToString();
			ExTraceGlobals.AuthenticationTracer.TraceDebug<string>((long)this.traceId, "LiveID STS Sending request {0}", arg);
		}

		private const int bit30LiveTOU = 536870912;

		private const int bit15MsnTOU = 16384;

		private const int bit6LimitedConsent = 32;

		private const int bit7FullConsent = 64;

		private const int bit8IsChild = 128;

		private const int statusMask = 224;

		protected static int numberOfLiveIdRequests;

		protected static int numberOfMsoIdRequests;

		private string connectionGroupName;

		private static object lockRoot = new object();

		private static RPS privateRpsSession;

		private static readonly XmlWriterSettings usAsciiXmlWriterSettings;

		private static DateTime refTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

		private string tokenConsumer;

		protected byte[] tokenConsumerBytes;

		private string privateProfilePolicy;

		private byte[] privateProfilePolicyBytes;
	}
}
