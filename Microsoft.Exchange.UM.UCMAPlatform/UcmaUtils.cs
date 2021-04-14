using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.UM.TroubleshootingTool.Shared;
using Microsoft.Exchange.UM.UMCommon;
using Microsoft.Exchange.UM.UMCommon.Exceptions;
using Microsoft.Exchange.UM.UMCommon.FaultInjection;
using Microsoft.Exchange.UM.UMCore;
using Microsoft.Rtc.Collaboration;
using Microsoft.Rtc.Collaboration.AudioVideo;
using Microsoft.Rtc.Signaling;
using Microsoft.Speech.Recognition;
using Microsoft.Speech.Recognition.SrgsGrammar;

namespace Microsoft.Exchange.UM.UcmaPlatform
{
	internal class UcmaUtils
	{
		public static string GetUserAgent()
		{
			return string.Format(CultureInfo.InvariantCulture, "MSExchangeUM/{0}", new object[]
			{
				FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion
			});
		}

		public static string GetOwnerUri()
		{
			return "sip:MSExchangeUM@" + Utils.GetOwnerHostFqdn();
		}

		public static string GetTlsError(TlsFailureException e)
		{
			ValidateArgument.NotNull(e, "TlsFailureException");
			switch (e.FailureReason)
			{
			case 0:
				return Strings.TlsOther(e.ErrorCode, e.Message);
			case 1:
				return Strings.TlsLocalCertificateNotFound(e.ErrorCode, e.Message);
			case 2:
				return Strings.TlsUntrustedRemoteCertificate(e.ErrorCode, e.Message);
			case 3:
				return Strings.TlsIncorrectNameInRemoteCertificate(e.ErrorCode, e.Message);
			case 4:
				return Strings.TlsCertificateExpired(e.ErrorCode, e.Message);
			case 5:
				return Strings.TlsTlsNegotiationFailure(e.ErrorCode, e.Message);
			case 6:
				return Strings.TlsRemoteDisconnected(e.ErrorCode, e.Message);
			case 7:
				return Strings.TlsRemoteCertificateRevoked(e.ErrorCode, e.Message);
			case 8:
				return Strings.TlsRemoteCertificateInvalidUsage(e.ErrorCode, e.Message);
			default:
				return e.Message;
			}
		}

		public static CollaborationPlatform GetTCPPlatform(int port, AddressFamilyHint addressFamilyHint, bool optimizeForNoMediaHandling, DiagnosticHelper diag, IPAddress ipAddress = null)
		{
			return new CollaborationPlatform(UcmaUtils.GetServerPlatformSettings(1, diag, port, addressFamilyHint, optimizeForNoMediaHandling, ipAddress));
		}

		public static CollaborationPlatform GetTLSPlatform(int port, AddressFamilyHint addressFamilyHint, bool optimizeForNoMediaHandling, DiagnosticHelper diag, EventHandler<ErrorEventArgs> tlsFailureHandler, IPAddress ipAddress = null)
		{
			ValidateArgument.NotNull(diag, "DiagnosticHelper");
			ValidateArgument.NotNull(tlsFailureHandler, "tlsFailureHandler");
			ServerPlatformSettings serverPlatformSettings = UcmaUtils.GetServerPlatformSettings(2, diag, port, addressFamilyHint, optimizeForNoMediaHandling, ipAddress);
			CollaborationPlatform collaborationPlatform = new CollaborationPlatform(serverPlatformSettings);
			RealTimeServerTlsConnectionManager realTimeServerTlsConnectionManager = (RealTimeServerTlsConnectionManager)collaborationPlatform.ConnectionManager;
			realTimeServerTlsConnectionManager.DnsLoadBalancingDisabled = false;
			realTimeServerTlsConnectionManager.IncomingTlsNegotiationFailed += tlsFailureHandler;
			return collaborationPlatform;
		}

		public static InfoMessage.PlatformMessageReceivedEventArgs GetPlatformMessageReceivedEventArgs(UcmaCallInfo callInfo, MessageReceivedEventArgs e)
		{
			InfoMessage infoMessage = new InfoMessage();
			infoMessage.Body = e.GetBody();
			infoMessage.ContentType = e.ContentType;
			foreach (SignalingHeader signalingHeader in e.RequestData.SignalingHeaders)
			{
				infoMessage.Headers[signalingHeader.Name] = signalingHeader.GetValue();
			}
			return new InfoMessage.PlatformMessageReceivedEventArgs(callInfo, infoMessage, e.MessageType == 2);
		}

		public static void StartPlatform(CollaborationPlatform platform)
		{
			ValidateArgument.NotNull(platform, "CollaborationPlatform");
			IAsyncResult asyncResult = platform.BeginStartup(null, null);
			platform.EndStartup(asyncResult);
		}

		public static void StartEndpoint(ApplicationEndpoint endpoint)
		{
			ValidateArgument.NotNull(endpoint, "ApplicationEndpoint");
			IAsyncResult asyncResult = endpoint.BeginEstablish(null, null);
			endpoint.EndEstablish(asyncResult);
		}

		public static void StopPlatform(CollaborationPlatform platform)
		{
			ValidateArgument.NotNull(platform, "CollaborationPlatform");
			IAsyncResult asyncResult = platform.BeginShutdown(null, null);
			platform.EndShutdown(asyncResult);
		}

		public static void ChangeCertificate(CollaborationPlatform platform)
		{
			ValidateArgument.NotNull(platform, "CollaborationPlatform");
			IAsyncResult asyncResult = platform.BeginChangeCertificate(CertificateUtils.UMCertificate, null, null);
			platform.EndChangeCertificate(asyncResult);
		}

		public static bool IsPortValid(int port)
		{
			return port > 0 && port < 65535;
		}

		public static ConnectionContext CreateConnectionContext(string host, int port)
		{
			return new ConnectionContext(host, port);
		}

		public static void HandleMessageReceived(DiagnosticHelper diagnostics, MessageReceivedEventArgs e, HandleMessageReceivedDelegate handler)
		{
			RealTimeConnection connection = e.Connection;
			if (connection == null)
			{
				diagnostics.Trace("HandleMessageReceived: connection is null. Ignoring message.", new object[0]);
				return;
			}
			int responseCode = 403;
			SignalingHeader[] responseHeaders = null;
			try
			{
				diagnostics.Trace("HandleMessageReceived: Type:{0}. Body:{1}.", new object[]
				{
					e.MessageType,
					e.HasTextBody ? e.TextBody : string.Empty
				});
				if (e.MessageType == 4 || e.MessageType == 2)
				{
					UcmaCallInfo ucmaCallInfo = new UcmaCallInfo(e, connection);
					InfoMessage.PlatformMessageReceivedEventArgs args = UcmaUtils.GetPlatformMessageReceivedEventArgs(ucmaCallInfo, e);
					args.ResponseCode = responseCode;
					PlatformSignalingHeader platformSignalingHeader;
					Exception ex = UcmaUtils.ProcessPlatformRequestAndReportErrors(delegate
					{
						handler(args);
					}, ucmaCallInfo.CallId, out platformSignalingHeader);
					if (ex != null)
					{
						CallRejectedException ex2 = ex as CallRejectedException;
						args.ResponseCode = ((ex2 != null && ex2.Reason.ErrorCode == CallEndingReason.PipelineFull.ErrorCode) ? 503 : 500);
						responseHeaders = new SignalingHeader[]
						{
							new SignalingHeader(platformSignalingHeader.Name, platformSignalingHeader.Value)
						};
					}
					else if (args.ResponseContactUri != null)
					{
						string text = string.Format(CultureInfo.InvariantCulture, "<{0}>", new object[]
						{
							args.ResponseContactUri.ToString()
						});
						responseHeaders = new SignalingHeader[]
						{
							new SignalingHeader("Contact", text)
						};
					}
					else
					{
						platformSignalingHeader = CallRejectedException.RenderDiagnosticHeader(CallEndingReason.SipMessageReceived, null, new object[0]);
						responseHeaders = new SignalingHeader[]
						{
							new SignalingHeader(platformSignalingHeader.Name, platformSignalingHeader.Value)
						};
					}
					responseCode = args.ResponseCode;
				}
			}
			finally
			{
				string signalingHeadersLogString = UcmaUtils.GetSignalingHeadersLogString(responseHeaders);
				diagnostics.Trace("A {0} message was received and processed: From:{1}. To:{2}. Response Code:{3}. Response Headers:{4}.", new object[]
				{
					e.MessageType,
					e.RequestData.FromHeader.Uri,
					e.RequestData.ToHeader.Uri,
					responseCode,
					signalingHeadersLogString
				});
				if (responseCode != 200)
				{
					ExEventLog.EventTuple tuple = (e.MessageType == 4) ? UMEventLogConstants.Tuple_ServiceRequestRejected : UMEventLogConstants.Tuple_OptionsMessageRejected;
					string periodicKey = string.Format(CultureInfo.InvariantCulture, "{0}:{1}:{2}", new object[]
					{
						e.RequestData.FromHeader.Uri,
						e.RequestData.ToHeader.Uri,
						responseCode
					});
					UmGlobals.ExEvent.LogEvent(tuple, periodicKey, new object[]
					{
						CommonUtil.ToEventLogString(e.RequestData.FromHeader.Uri),
						CommonUtil.ToEventLogString(e.RequestData.ToHeader.Uri),
						CommonUtil.ToEventLogString(responseCode),
						CommonUtil.ToEventLogString(signalingHeadersLogString)
					});
				}
				UcmaUtils.CatchRealtimeErrors(delegate
				{
					e.SendResponse(responseCode, null, null, responseHeaders);
				}, diagnostics);
			}
		}

		public static void SetPingInfoErrorResult(PingInfo info, SipResponseData data, Exception ex)
		{
			info.Error = ex;
			info.Diagnostics = string.Empty;
			info.ResponseCode = 0;
			info.ResponseText = string.Empty;
			FailureResponseException ex2 = ex as FailureResponseException;
			if (data != null)
			{
				info.ResponseCode = data.ResponseCode;
				info.ResponseText = data.ResponseText;
				SignalingHeader signalingHeader = data.SignalingHeaders.FirstOrDefault((SignalingHeader x) => x.Name.Equals("ms-diagnostics", StringComparison.OrdinalIgnoreCase));
				info.Diagnostics = ((signalingHeader != null) ? signalingHeader.GetValue() : string.Empty);
				return;
			}
			if (ex2 != null)
			{
				info.ResponseCode = ex2.ResponseData.ResponseCode;
				info.ResponseText = ex2.ResponseData.ResponseText;
				return;
			}
			if (ex.Message != null)
			{
				int num = ex.Message.IndexOf(Environment.NewLine);
				info.ResponseText = ((num > 0) ? ex.Message.Substring(0, num) : ex.Message);
			}
		}

		public static void HandleSIPPeerListChanged(CollaborationPlatform tlsPlatform, DiagnosticHelper diag)
		{
			ValidateArgument.NotNull(tlsPlatform, "TLS CollaborationPlatform");
			ValidateArgument.NotNull(diag, "DiagnosticHelper");
			diag.Trace("UcmaUtils: HandleSIPPeerListChanged updating TrustedDomain list", new object[0]);
			Dictionary<TrustedDomain, bool> dictionary = new Dictionary<TrustedDomain, bool>(16, new UcmaUtils.TrustedDomainComparer());
			foreach (TrustedDomain key in tlsPlatform.GetTrustedDomains())
			{
				dictionary[key] = false;
			}
			foreach (UMSipPeer umsipPeer in SipPeerManager.Instance.GetSecuredSipPeers())
			{
				string text = umsipPeer.Address.ToString();
				TrustedDomainMode trustedDomainMode = umsipPeer.IsOcs ? 0 : 1;
				TrustedDomain key2 = new TrustedDomain(text, trustedDomainMode);
				dictionary[key2] = true;
			}
			foreach (KeyValuePair<TrustedDomain, bool> keyValuePair in dictionary)
			{
				if (!keyValuePair.Value)
				{
					tlsPlatform.RemoveTrustedDomain(keyValuePair.Key);
					diag.Trace("UcmaVoipPlatform removed TrustedDomain {0}:{1}", new object[]
					{
						keyValuePair.Key,
						keyValuePair.Value
					});
				}
			}
			foreach (KeyValuePair<TrustedDomain, bool> keyValuePair2 in dictionary)
			{
				if (keyValuePair2.Value)
				{
					tlsPlatform.AddTrustedDomain(keyValuePair2.Key);
					diag.Trace("UcmaVoipPlatform added/retained TrustedDomain {0}:{1}", new object[]
					{
						keyValuePair2.Key,
						keyValuePair2.Value
					});
				}
			}
		}

		public static Exception CatchRealtimeErrors(GrayException.UserCodeDelegate function, DiagnosticHelper diag)
		{
			ValidateArgument.NotNull(function, "GrayException.UserCodeDelegate");
			ValidateArgument.NotNull(diag, "DiagnosticHelper");
			Exception error = null;
			try
			{
				ExceptionHandling.MapAndReportGrayExceptions(delegate()
				{
					try
					{
						function();
					}
					catch (RealTimeException error2)
					{
						error = error2;
					}
					catch (InvalidOperationException error3)
					{
						error = error3;
					}
					catch (ArgumentException error4)
					{
						error = error4;
					}
				});
			}
			catch (UMGrayException error)
			{
				UMGrayException error5;
				error = error5;
			}
			if (error != null)
			{
				diag.Trace("CatchRealTimeErrors caught e={0}", new object[]
				{
					error
				});
			}
			return error;
		}

		public static Exception ProcessPlatformRequestAndReportErrors(GrayException.UserCodeDelegate function, string callId, out PlatformSignalingHeader diagnosticHeader)
		{
			ValidateArgument.NotNull(callId, "Call Id");
			ValidateArgument.NotNull(function, "GrayException UserCodeDelegate");
			Exception error = null;
			PlatformSignalingHeader diagHeader = null;
			try
			{
				ExceptionHandling.MapAndReportGrayExceptions(delegate()
				{
					try
					{
						function();
					}
					catch (CallRejectedException ex)
					{
						error = ex;
						diagHeader = ex.DiagnosticHeader;
					}
					catch (CryptographicException error2)
					{
						error = error2;
					}
					catch (InvalidSIPHeaderException ex2)
					{
						UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_InvalidSipHeader, null, new object[]
						{
							callId,
							CommonUtil.ToEventLogString(ex2)
						});
						error = ex2;
						diagHeader = CallRejectedException.RenderDiagnosticHeader(CallEndingReason.InvalidSIPheader, null, new object[0]);
					}
					catch (FormatException ex3)
					{
						UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_InvalidSipHeader, null, new object[]
						{
							callId,
							CommonUtil.ToEventLogString(ex3)
						});
						error = ex3;
						diagHeader = CallRejectedException.RenderDiagnosticHeader(CallEndingReason.InvalidRequest, null, new object[0]);
					}
					catch (MessageParsingException ex4)
					{
						error = ex4;
						UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_InvalidSipHeader, null, new object[]
						{
							callId,
							CommonUtil.ToEventLogString(ex4)
						});
						diagHeader = CallRejectedException.RenderDiagnosticHeader(CallEndingReason.InvalidRequest, null, new object[0]);
					}
					catch (ADTransientException ex5)
					{
						UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_ADTransientError, null, new object[]
						{
							callId,
							CommonUtil.ToEventLogString(ex5)
						});
						error = ex5;
						diagHeader = CallRejectedException.RenderDiagnosticHeader(CallEndingReason.ADError, ex5.GetType().FullName, new object[0]);
					}
					catch (StorageTransientException ex6)
					{
						UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_FailedToConnectToMailbox, null, new object[]
						{
							callId,
							CommonUtil.ToEventLogString(ex6)
						});
						error = ex6;
						diagHeader = CallRejectedException.RenderDiagnosticHeader(CallEndingReason.StorageError, ex6.GetType().FullName, new object[0]);
					}
					catch (StoragePermanentException ex7)
					{
						UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_FailedToConnectToMailbox, null, new object[]
						{
							callId,
							CommonUtil.ToEventLogString(ex7)
						});
						error = ex7;
						diagHeader = CallRejectedException.RenderDiagnosticHeader(CallEndingReason.StorageError, ex7.GetType().FullName, new object[0]);
					}
					catch (ExchangeServerNotFoundException ex8)
					{
						UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_ADTransientError, null, new object[]
						{
							callId,
							CommonUtil.ToEventLogString(ex8)
						});
						error = ex8;
						diagHeader = CallRejectedException.RenderDiagnosticHeader(CallEndingReason.ADError, ex8.GetType().FullName, new object[0]);
					}
					catch (DataSourceOperationException ex9)
					{
						UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_ADPermanentError, null, new object[]
						{
							callId,
							CommonUtil.ToEventLogString(ex9)
						});
						error = ex9;
						diagHeader = CallRejectedException.RenderDiagnosticHeader(CallEndingReason.ADError, ex9.GetType().FullName, new object[0]);
					}
					catch (DataValidationException ex10)
					{
						UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_ADDataError, null, new object[]
						{
							callId,
							CommonUtil.ToEventLogString(ex10)
						});
						error = ex10;
						diagHeader = CallRejectedException.RenderDiagnosticHeader(CallEndingReason.ADError, ex10.GetType().FullName, new object[0]);
					}
				}, new GrayException.IsGrayExceptionDelegate(GrayException.IsGrayException));
			}
			catch (UMGrayException error)
			{
				UMGrayException error3;
				error = error3;
			}
			diagnosticHeader = diagHeader;
			return error;
		}

		public static Grammar CreateGrammar(UMGrammar grammar)
		{
			Grammar result;
			if (string.IsNullOrEmpty(grammar.Script))
			{
				if (grammar.BaseUri != null)
				{
					result = new Grammar(grammar.Path, grammar.RuleName, grammar.BaseUri);
				}
				else
				{
					result = new Grammar(grammar.Path, grammar.RuleName);
				}
			}
			else
			{
				SrgsDocument srgsDocument = new SrgsDocument(grammar.Path);
				foreach (SrgsRule srgsRule in srgsDocument.Rules)
				{
					srgsRule.Elements.Insert(0, new SrgsSemanticInterpretationTag(grammar.Script));
				}
				if (grammar.BaseUri != null)
				{
					result = new Grammar(srgsDocument, grammar.RuleName, grammar.BaseUri);
				}
				else
				{
					result = new Grammar(srgsDocument, grammar.RuleName);
				}
			}
			return result;
		}

		public static AddressFamilyHint MapIPAddressFamilyToHint(IPAddressFamily ipAddressFamily)
		{
			switch (ipAddressFamily)
			{
			case IPAddressFamily.IPv4Only:
				return 1;
			case IPAddressFamily.IPv6Only:
				return 2;
			case IPAddressFamily.Any:
				return 0;
			default:
				throw new ArgumentException("Unsupported value of IPAddressFamily", "ipAddressFamily");
			}
		}

		private static string GetSignalingHeadersLogString(IEnumerable<SignalingHeader> headers)
		{
			string result = string.Empty;
			if (headers != null)
			{
				StringBuilder stringBuilder = new StringBuilder();
				foreach (SignalingHeader signalingHeader in headers)
				{
					stringBuilder.AppendLine(string.Format(CultureInfo.InvariantCulture, "{0}: {1}", new object[]
					{
						signalingHeader.Name,
						signalingHeader.GetValue()
					}));
				}
				result = stringBuilder.ToString();
			}
			return result;
		}

		private static ServerPlatformSettings GetServerPlatformSettings(SipTransportType mode, DiagnosticHelper diag, int port, AddressFamilyHint addressFamilyHint, bool optimizeForNoMediaHandling, IPAddress ipAddress)
		{
			string ownerHostFqdn = Utils.GetOwnerHostFqdn();
			string userAgent = UcmaUtils.GetUserAgent();
			ServerPlatformSettings serverPlatformSettings;
			if (mode == 1)
			{
				diag.Trace("Creating TcpServerPlatformSettings.  Agent={0}, Fqdn={1}, Port={2}", new object[]
				{
					userAgent,
					ownerHostFqdn,
					port
				});
				serverPlatformSettings = new ServerPlatformSettings(userAgent, ownerHostFqdn, port, null);
			}
			else
			{
				diag.Trace("Creating TlsServerPlatformSettings.  Agent={0}, Fqdn={1}, Port={2}, Cert thumbprint={3} Cert SubjectName={4}", new object[]
				{
					userAgent,
					ownerHostFqdn,
					port,
					CertificateUtils.UMCertificate.Thumbprint,
					CertificateUtils.UMCertificate.SubjectName.Name
				});
				serverPlatformSettings = new ServerPlatformSettings(UcmaUtils.GetUserAgent(), Utils.GetOwnerHostFqdn(), port, null, CertificateUtils.UMCertificate);
				serverPlatformSettings.TrustedDomains.Clear();
				foreach (UMSipPeer umsipPeer in SipPeerManager.Instance.GetSecuredSipPeers())
				{
					string text = umsipPeer.Address.ToString();
					TrustedDomainMode trustedDomainMode = umsipPeer.IsOcs ? 0 : 1;
					serverPlatformSettings.TrustedDomains.Add(new TrustedDomain(text, trustedDomainMode));
					diag.Trace("Added TrustedDomain address '{0}', mode '{1}'", new object[]
					{
						text,
						mode
					});
				}
			}
			if (optimizeForNoMediaHandling)
			{
				serverPlatformSettings.DefaultAudioVideoProviderEnabled = false;
			}
			if (ipAddress != null)
			{
				serverPlatformSettings.ListeningIPAddress = ipAddress;
			}
			else
			{
				serverPlatformSettings.OutboundConnectionConfiguration.DefaultAddressFamilyHint = new AddressFamilyHint?(addressFamilyHint);
				bool flag;
				bool flag2;
				Utils.GetLocalIPv4IPv6Support(out flag, out flag2);
				if (flag2 && !flag)
				{
					serverPlatformSettings.ListeningIPAddress = IPAddress.IPv6Any;
				}
			}
			return serverPlatformSettings;
		}

		private static CallProvisionalResponseOptions CreateProvisionalResponseDiagnosticInfo(string headerValue, DiagnosticHelper diag)
		{
			CallProvisionalResponseOptions callProvisionalResponseOptions = new CallProvisionalResponseOptions();
			string text = "ms-diagnostics-public";
			diag.Trace("{0}:{1}", new object[]
			{
				text,
				headerValue
			});
			callProvisionalResponseOptions.Headers.Add(new SignalingHeader(text, headerValue));
			callProvisionalResponseOptions.ResponseText = "Diagnostics";
			return callProvisionalResponseOptions;
		}

		internal static void SendDiagnosticsInfoCallReceived(AudioVideoCall call, DiagnosticHelper diag)
		{
			diag.Trace("Sending 101 Diagnostic message for ActiveMonitoring client to track call received", new object[0]);
			string headerValue = Util.FormatDiagnosticsInfoCallReceived();
			CallProvisionalResponseOptions callProvisionalResponseOptions = UcmaUtils.CreateProvisionalResponseDiagnosticInfo(headerValue, diag);
			call.SendProvisionalResponse(101, callProvisionalResponseOptions);
		}

		internal static void SendDiagnosticsInfoCallRedirect(AudioVideoCall call, string uri, DiagnosticHelper diag)
		{
			diag.Trace("Sending 101 Diagnostic message for ActiveMonitoring client to track Redirects", new object[0]);
			string headerValue = Util.FormatDiagnosticsInfoRedirect(uri);
			CallProvisionalResponseOptions callProvisionalResponseOptions = UcmaUtils.CreateProvisionalResponseDiagnosticInfo(headerValue, diag);
			call.SendProvisionalResponse(101, callProvisionalResponseOptions);
		}

		private static void SendDiagnosticsInfoServerHealth(AudioVideoCall call, DiagnosticHelper diag)
		{
			diag.Trace("Sending 101 Diagnostic message for ActiveMonitoring client to track server health", new object[0]);
			string headerValue = Util.FormatDiagnosticsInfoServerHealth();
			CallProvisionalResponseOptions provisionalResponseOptions = UcmaUtils.CreateProvisionalResponseDiagnosticInfo(headerValue, diag);
			UcmaUtils.CatchRealtimeErrors(delegate
			{
				call.SendProvisionalResponse(101, provisionalResponseOptions);
			}, diag);
		}

		private static void DeclineCallWithTimeout(AudioVideoCall call, DiagnosticHelper diag)
		{
			diag.Trace("Sending 504 time out with Diagnostic message for ActiveMonitoring client to track server timeout", new object[0]);
			string text = Util.FormatDiagnosticsInfoCallTimeout();
			string text2 = "ms-diagnostics-public";
			diag.Trace("{0}:{1}", new object[]
			{
				text2,
				text
			});
			CallDeclineOptions options = new CallDeclineOptions();
			options.ResponseCode = 504;
			options.ResponseText = "Server time-out";
			options.Headers.Add(new SignalingHeader(text2, text));
			UcmaUtils.CatchRealtimeErrors(delegate
			{
				call.Decline(options);
			}, diag);
		}

		internal static void CreateDiagnosticsTimers(AudioVideoCall call, Action<Timer> trackTimer, DiagnosticHelper diag)
		{
			Timer obj = UcmaUtils.CreateDiagnosticsTimer(0L, 8000L, call, (AudioVideoCall c) => c.State == 1, delegate(AudioVideoCall c)
			{
				UcmaUtils.SendDiagnosticsInfoServerHealth(c, diag);
			});
			Timer obj2 = UcmaUtils.CreateDiagnosticsTimer(30000L, -1L, call, (AudioVideoCall c) => c.State == 1, delegate(AudioVideoCall c)
			{
				UcmaUtils.DeclineCallWithTimeout(c, diag);
			});
			if (trackTimer != null)
			{
				trackTimer(obj);
				trackTimer(obj2);
			}
		}

		private static Timer CreateDiagnosticsTimer(long dueTime, long period, AudioVideoCall call, Predicate<AudioVideoCall> condition, Action<AudioVideoCall> action)
		{
			return new Timer(delegate(object o)
			{
				AudioVideoCall audioVideoCall = o as AudioVideoCall;
				if (audioVideoCall != null && condition != null && action != null && condition(audioVideoCall))
				{
					action(audioVideoCall);
				}
			}, call, dueTime, period);
		}

		internal static void AddCallDelayFaultInjectionInTestMode(uint faultInjectionLid)
		{
			int num = 0;
			FaultInjectionUtils.FaultInjectChangeValue<int>(faultInjectionLid, ref num);
			if (num > 0)
			{
				Thread.Sleep(TimeSpan.FromSeconds((double)num));
			}
		}

		private const string UserAgentFormat = "MSExchangeUM/{0}";

		private const string OwnerUriPrefix = "sip:MSExchangeUM@";

		private class TrustedDomainComparer : IEqualityComparer<TrustedDomain>
		{
			public bool Equals(TrustedDomain x, TrustedDomain y)
			{
				return x.DomainName.Equals(y.DomainName, StringComparison.OrdinalIgnoreCase) && x.DomainMode == y.DomainMode;
			}

			public int GetHashCode(TrustedDomain obj)
			{
				string text = obj.DomainName.ToLowerInvariant();
				return text.GetHashCode() ^ obj.DomainMode.GetHashCode();
			}
		}
	}
}
