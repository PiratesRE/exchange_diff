using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.Management.SystemConfigurationTasks;
using Microsoft.Exchange.UM.UMCommon;
using Microsoft.Exchange.UM.UMCommon.Exceptions;
using Microsoft.Rtc.Collaboration;
using Microsoft.Rtc.Collaboration.AudioVideo;
using Microsoft.Rtc.Signaling;

namespace Microsoft.Exchange.Management.Tasks.UM
{
	internal abstract class BaseUMconnectivityTester
	{
		internal BaseUMconnectivityTester()
		{
		}

		internal ManualResetEvent CallEndedEvent
		{
			get
			{
				return this.callEndedEvent;
			}
			set
			{
				this.callEndedEvent = value;
			}
		}

		internal bool RunSecured
		{
			get
			{
				return this.runSecured;
			}
		}

		internal string UmIP
		{
			get
			{
				return this.umIP;
			}
			set
			{
				this.umIP = value;
			}
		}

		internal string CurrCalls
		{
			get
			{
				return this.currCalls;
			}
			set
			{
				this.currCalls = value;
			}
		}

		internal bool IsCallEstablished
		{
			get
			{
				return this.isCallEstablished;
			}
			set
			{
				this.isCallEstablished = value;
			}
		}

		internal LocalizedException Error
		{
			get
			{
				return this.error;
			}
			set
			{
				this.error = value;
			}
		}

		internal string MsDiagnosticsHeaderValue { get; private set; }

		protected AudioVideoCall AudioCall
		{
			get
			{
				return this.audioCall;
			}
		}

		protected virtual string SignalingHeaderValue
		{
			get
			{
				return "local";
			}
		}

		internal bool IsCallGone()
		{
			bool result;
			lock (this)
			{
				result = (this.audioCall == null || this.audioCall.State == 8 || this.audioCall.State == 7 || this.audioCall.Flow.State == 2);
			}
			return result;
		}

		internal bool Initialize(int sipPort, bool secured, bool mediaSecured, string certificateThumbprint)
		{
			this.DebugTrace("Inside BaseUMconnectivityTester Initialize with port ={0}, isSecured={1}, isMediaSecured={2}", new object[]
			{
				sipPort,
				secured,
				mediaSecured
			});
			bool result;
			lock (this)
			{
				try
				{
					this.runSecured = secured;
					this.mediaSecured = mediaSecured;
					if (!secured)
					{
						this.endPoint = BaseUMconnectivityTester.SipPlatformConnectionManager.GetTcpEndPoint(sipPort);
					}
					else
					{
						this.endPoint = BaseUMconnectivityTester.SipPlatformConnectionManager.GetTlsEndPoint(sipPort, certificateThumbprint);
					}
					this.isInitialized = true;
					result = true;
				}
				catch (TUC_CertNotFound tuc_CertNotFound)
				{
					this.error = tuc_CertNotFound;
					this.ErrorTrace("Inside BaseUMconnectivityTester Initialize, error ={0}", new object[]
					{
						tuc_CertNotFound
					});
					result = false;
				}
				catch (TlsFailureException ex)
				{
					this.error = new TUC_InitializeError(BaseUMconnectivityTester.GetTlsError(ex), ex);
					this.ErrorTrace("Inside BaseUMconnectivityTester Initialize, error ={0}", new object[]
					{
						ex
					});
					result = false;
				}
				catch (RealTimeException ex2)
				{
					this.error = new TUC_InitializeError(ex2.Message, ex2);
					this.ErrorTrace("Inside BaseUMconnectivityTester Initialize, error ={0}", new object[]
					{
						ex2
					});
					result = false;
				}
				catch (SocketException ex3)
				{
					this.error = new TUC_InitializeError(ex3.Message, ex3);
					this.ErrorTrace("Inside BaseUMconnectivityTester Initialize, error ={0}", new object[]
					{
						ex3
					});
					result = false;
				}
				catch (ObjectDisposedException ex4)
				{
					this.error = new TUC_InitializeError(ex4.Message, ex4);
					this.ErrorTrace("Inside BaseUMconnectivityTester Initialize, error ={0}", new object[]
					{
						ex4
					});
					result = false;
				}
				catch (InvalidOperationException ex5)
				{
					this.error = new TUC_InitializeError(ex5.Message, ex5);
					this.ErrorTrace("Inside BaseUMconnectivityTester Initialize, error ={0}", new object[]
					{
						ex5
					});
					result = false;
				}
			}
			return result;
		}

		internal bool SendOptions(string targetUri)
		{
			int num = 0;
			SipResponseData sipResponseData = null;
			this.Error = null;
			this.MsDiagnosticsHeaderValue = string.Empty;
			try
			{
				RealTimeEndpoint innerEndpoint = this.endPoint.InnerEndpoint;
				new SendMessageOptions();
				sipResponseData = innerEndpoint.EndSendMessage(innerEndpoint.BeginSendMessage(2, new RealTimeAddress(targetUri), null, null, null, null));
			}
			catch (Exception ex)
			{
				this.ErrorTrace("SendOptions. Error: {0}", new object[]
				{
					ex
				});
				this.Error = new TUC_SipOptionsError(targetUri, ex.Message, ex);
				FailureResponseException ex2 = ex as FailureResponseException;
				if (ex2 != null)
				{
					this.DebugTrace("SendOptions. Known FailureResponseException. Getting response data.", new object[0]);
					sipResponseData = ex2.ResponseData;
				}
				else
				{
					if (!(ex is RealTimeException) && !(ex is InvalidOperationException) && !(ex is ArgumentException))
					{
						throw ex;
					}
					this.DebugTrace("SendOptions. Known exception {0}", new object[]
					{
						ex.GetType()
					});
				}
			}
			if (sipResponseData != null)
			{
				num = sipResponseData.ResponseCode;
				SignalingHeader signalingHeader = sipResponseData.SignalingHeaders.FirstOrDefault((SignalingHeader x) => x.Name.Equals("ms-diagnostics", StringComparison.OrdinalIgnoreCase));
				this.MsDiagnosticsHeaderValue = ((signalingHeader != null) ? signalingHeader.GetValue() : string.Empty);
			}
			return num == 200;
		}

		internal bool MakeCall(string calledPartyUri, string callerPartyUri)
		{
			this.DebugTrace("Inside BaseUMconnectivityTester MakeCall, calledParty ={0}, callerParty = {1}", new object[]
			{
				calledPartyUri,
				callerPartyUri
			});
			lock (this)
			{
				try
				{
					Conversation conversation = new Conversation(this.endPoint);
					if (!string.IsNullOrEmpty(callerPartyUri))
					{
						conversation.Impersonate(callerPartyUri, null, null);
					}
					this.audioCall = new AudioVideoCall(conversation);
					this.audioCall.StateChanged += this.OnStateChanged;
					this.audioCall.InfoReceived += this.OnMessageReceived;
					this.audioCall.AudioVideoFlowConfigurationRequested += this.OnAudioVideoFlowConfigurationRequested;
					this.audioCall.Forwarded += this.OnRedirect;
					CallEstablishOptions callEstablishOptions = new CallEstablishOptions();
					callEstablishOptions.ConnectionContext = this.GetConnectionContext();
					this.callEndedEvent = new ManualResetEvent(false);
					this.mediaEstablishedEvent = new ManualResetEvent(false);
					callEstablishOptions.Headers.Add(new SignalingHeader("msexum-connectivitytest", this.SignalingHeaderValue));
					this.audioCall.EndEstablish(this.audioCall.BeginEstablish(calledPartyUri, callEstablishOptions, null, null));
					this.WaitForMediaEstablishment(calledPartyUri);
					this.AttachToneController();
					this.HandleConnectionEstablished();
					this.isCallEstablished = true;
				}
				catch (TUC_MakeCallError tuc_MakeCallError)
				{
					this.error = tuc_MakeCallError;
					this.ErrorTrace("Inside BaseUMconnectivityTester MakeCall, error ={0}", new object[]
					{
						tuc_MakeCallError
					});
					return false;
				}
				catch (RealTimeException ex)
				{
					this.error = new TUC_MakeCallError(calledPartyUri, ex.Message, ex);
					this.ErrorTrace("Inside BaseUMconnectivityTester MakeCall, error ={0}", new object[]
					{
						ex
					});
					return false;
				}
				catch (InvalidOperationException ex2)
				{
					this.error = new TUC_MakeCallError(calledPartyUri, ex2.Message, ex2);
					this.ErrorTrace("Inside BaseUMconnectivityTester MakeCall, error ={0}", new object[]
					{
						ex2
					});
					return false;
				}
				catch (ArgumentException ex3)
				{
					this.error = new TUC_MakeCallError(calledPartyUri, ex3.Message, ex3);
					this.ErrorTrace("Inside BaseUMconnectivityTester MakeCall, error ={0}", new object[]
					{
						ex3
					});
					return false;
				}
			}
			return this.isCallEstablished;
		}

		internal void EndCall()
		{
			this.DebugTrace("Inside BaseUMconnectivityTester EndCall", new object[0]);
			lock (this)
			{
				try
				{
					if (this.audioCall != null)
					{
						this.audioCall.StateChanged -= this.OnStateChanged;
						this.audioCall.InfoReceived -= this.OnMessageReceived;
						this.audioCall.AudioVideoFlowConfigurationRequested -= this.OnAudioVideoFlowConfigurationRequested;
						this.audioCall.Forwarded -= this.OnRedirect;
						if (this.audioCall.Flow != null)
						{
							this.audioCall.Flow.StateChanged -= this.OnMediaStateChanged;
							if (this.audioCall.Flow.ToneController != null)
							{
								this.audioCall.Flow.ToneController.ToneReceived -= this.OnToneReceived;
								this.audioCall.Flow.ToneController.DetachFlow();
							}
						}
						if (this.audioCall.State != 8 || this.audioCall.State != 7)
						{
							this.audioCall.EndTerminate(this.audioCall.BeginTerminate(null, null));
						}
					}
				}
				catch (RealTimeException ex)
				{
					this.ErrorTrace("Inside BaseUMconnectivityTester EndCall, error ={0}", new object[]
					{
						ex
					});
				}
				catch (InvalidOperationException ex2)
				{
					this.ErrorTrace("Inside BaseUMconnectivityTester EndCall, error ={0}", new object[]
					{
						ex2
					});
				}
			}
		}

		internal void Shutdown()
		{
			this.DebugTrace("Inside BaseUMconnectivityTester shutdown with isInitialized ={0}", new object[]
			{
				this.isInitialized
			});
		}

		internal bool SendDTMFTone(char tone)
		{
			this.DebugTrace("Inside BaseUMconnectivityTester SendDTMFTone", new object[0]);
			return this.SendDTMFTone(tone, 100);
		}

		internal bool SendDTMFTone(char tone, int duration)
		{
			this.DebugTrace("Inside BaseUMconnectivityTester SendDTMFTone", new object[0]);
			bool result;
			lock (this)
			{
				if (this.IsCallGone())
				{
					this.Error = new TUC_RemoteEndDisconnected();
					result = false;
				}
				else
				{
					ToneId? toneId = BaseUMconnectivityTester.CharToToneId(tone);
					if (toneId != null)
					{
						this.AudioCall.Flow.ToneController.Send(toneId.Value, 17f);
						Thread.Sleep(duration);
						result = true;
					}
					else
					{
						this.Error = new TUC_SendSequenceError(Strings.InvalidDtmfChar(tone));
						result = false;
					}
				}
			}
			return result;
		}

		internal abstract bool ExecuteTest(TestParameters testparams);

		internal void DebugTrace(string formatString, params object[] formatObjects)
		{
			ExTraceGlobals.DiagnosticTracer.TraceDebug((long)this.GetHashCode(), formatString, formatObjects);
		}

		internal void ErrorTrace(string formatString, params object[] formatObjects)
		{
			ExTraceGlobals.DiagnosticTracer.TraceError((long)this.GetHashCode(), formatString, formatObjects);
		}

		protected virtual ConnectionContext GetConnectionContext()
		{
			return null;
		}

		protected virtual void HandleConnectionEstablished()
		{
		}

		protected virtual void HandleToneReceived(ToneControllerEventArgs e)
		{
		}

		protected virtual void HandleMessageReceived(MessageReceivedEventArgs e)
		{
		}

		private static string GetTlsError(TlsFailureException e)
		{
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

		private static ToneId? CharToToneId(char c)
		{
			ToneId? result = null;
			c = char.ToUpper(c);
			char c2 = c;
			switch (c2)
			{
			case '#':
				result = new ToneId?(11);
				break;
			case '$':
			case '%':
			case '&':
			case '\'':
			case '(':
			case ')':
			case '+':
			case ',':
			case '-':
			case '.':
			case '/':
				break;
			case '*':
				result = new ToneId?(10);
				break;
			case '0':
				result = new ToneId?(0);
				break;
			case '1':
				result = new ToneId?(1);
				break;
			case '2':
				result = new ToneId?(2);
				break;
			case '3':
				result = new ToneId?(3);
				break;
			case '4':
				result = new ToneId?(4);
				break;
			case '5':
				result = new ToneId?(5);
				break;
			case '6':
				result = new ToneId?(6);
				break;
			case '7':
				result = new ToneId?(7);
				break;
			case '8':
				result = new ToneId?(8);
				break;
			case '9':
				result = new ToneId?(9);
				break;
			default:
				switch (c2)
				{
				case 'A':
					result = new ToneId?(12);
					break;
				case 'B':
					result = new ToneId?(13);
					break;
				case 'C':
					result = new ToneId?(14);
					break;
				case 'D':
					result = new ToneId?(15);
					break;
				}
				break;
			}
			return result;
		}

		private void OnAudioVideoFlowConfigurationRequested(object sender, AudioVideoFlowConfigurationRequestedEventArgs e)
		{
			this.LockAndExecuteOotyCallback(delegate
			{
				this.DebugTrace("Inside BaseUMconnectivityTester OnAudioVideoFlowConfigurationRequested", new object[0]);
				AudioVideoFlowTemplate audioVideoFlowTemplate = new AudioVideoFlowTemplate(e.Flow);
				audioVideoFlowTemplate.EncryptionPolicy = (this.mediaSecured ? 3 : 1);
				e.Flow.Initialize(audioVideoFlowTemplate);
				e.Flow.StateChanged += this.OnMediaStateChanged;
			});
		}

		private void OnMediaStateChanged(object sender, MediaFlowStateChangedEventArgs args)
		{
			this.LockAndExecuteOotyCallback(delegate
			{
				this.DebugTrace("Inside BaseUMconnectivityTester MediaFlowStateChangedEventArgs, new state ={0}, previous state = {1}", new object[]
				{
					args.State,
					args.PreviousState
				});
				if (args.State == 1)
				{
					this.mediaEstablishedEvent.Set();
				}
			});
		}

		private void OnStateChanged(object sender, CallStateChangedEventArgs e)
		{
			this.LockAndExecuteOotyCallback(delegate
			{
				this.DebugTrace("Inside BaseUMconnectivityTester OnStateChanged, new state ={0}, reason = {1}", new object[]
				{
					e.State,
					e.TransitionReason
				});
				if (e.State == 8)
				{
					this.DebugTrace("Inside BaseUMconnectivityTester: Call is disconnected", new object[0]);
					this.callEndedEvent.Set();
				}
			});
		}

		private void OnRedirect(object sender, CallForwardReceivedEventArgs e)
		{
			this.LockAndExecuteOotyCallback(delegate
			{
				this.DebugTrace("Inside BaseUMconnectivityTester OnRedirect", new object[0]);
				e.Accept();
			});
		}

		private void OnToneReceived(object sender, ToneControllerEventArgs e)
		{
			this.LockAndExecuteOotyCallback(delegate
			{
				this.DebugTrace("Inside BaseUMconnectivityTester OnToneReceived", new object[0]);
				this.HandleToneReceived(e);
			});
		}

		private void OnMessageReceived(object sender, MessageReceivedEventArgs e)
		{
			this.LockAndExecuteOotyCallback(delegate
			{
				try
				{
					this.DebugTrace("Inside BaseUMconnectivityTester OnMessageReceived", new object[0]);
					this.HandleMessageReceived(e);
				}
				finally
				{
					e.SendResponse(200);
				}
			});
		}

		private void LockAndExecuteOotyCallback(Action function)
		{
			lock (this.eventSyncLock)
			{
				try
				{
					function();
				}
				catch (Exception ex)
				{
					if (!(ex is RealTimeException) && !(ex is InvalidOperationException))
					{
						if (!GrayException.IsGrayException(ex))
						{
							throw;
						}
						ExceptionHandling.SendWatsonWithoutDump(ex);
					}
					this.ErrorTrace("Error while processing audio call event received event. Exception:{0}", new object[]
					{
						ex
					});
				}
			}
		}

		private void AttachToneController()
		{
			ToneController toneController = new ToneController();
			toneController.AttachFlow(this.audioCall.Flow);
			this.audioCall.Flow.ToneController.ToneReceived += this.OnToneReceived;
		}

		private void WaitForMediaEstablishment(string calledPartyUri)
		{
			int num = WaitHandle.WaitAny(new ManualResetEvent[]
			{
				this.mediaEstablishedEvent,
				this.callEndedEvent
			}, TimeSpan.FromSeconds(60.0), false);
			int num2 = num;
			switch (num2)
			{
			case 0:
				return;
			case 1:
				throw new TUC_MakeCallError(calledPartyUri, new TUC_RemoteEndDisconnected().Message);
			default:
				if (num2 != 258)
				{
					return;
				}
				throw new TUC_MakeCallError(calledPartyUri, Strings.FailedToEstablishMedia(60));
			}
		}

		private const string MsDiagnosticsHeaderName = "ms-diagnostics";

		private object eventSyncLock = new object();

		private ManualResetEvent callEndedEvent;

		private ManualResetEvent mediaEstablishedEvent;

		private ApplicationEndpoint endPoint;

		private volatile AudioVideoCall audioCall;

		private string umIP = string.Empty;

		private string currCalls = string.Empty;

		private LocalizedException error;

		private bool isCallEstablished;

		private volatile bool isInitialized;

		private bool runSecured = true;

		private bool mediaSecured;

		private static class SipPlatformConnectionManager
		{
			private static string OwnerUri
			{
				get
				{
					if (BaseUMconnectivityTester.SipPlatformConnectionManager.ownerUri == null)
					{
						BaseUMconnectivityTester.SipPlatformConnectionManager.ownerUri = "sip:MSExchangeUMTestConnectivity@" + Utils.GetLocalHostFqdn();
					}
					return BaseUMconnectivityTester.SipPlatformConnectionManager.ownerUri;
				}
			}

			internal static ApplicationEndpoint GetTcpEndPoint(int listeningPort)
			{
				BaseUMconnectivityTester.SipPlatformConnectionManager.DebugTrace("Inside SipPlatformConnectionManager GetTcpEndpoint().", new object[0]);
				if (BaseUMconnectivityTester.SipPlatformConnectionManager.tcpEndPoint == null)
				{
					lock (BaseUMconnectivityTester.SipPlatformConnectionManager.syncLock)
					{
						if (BaseUMconnectivityTester.SipPlatformConnectionManager.tcpEndPoint == null)
						{
							BaseUMconnectivityTester.SipPlatformConnectionManager.tcpEndPoint = BaseUMconnectivityTester.SipPlatformConnectionManager.CreateEndPoint(new ServerPlatformSettings("MSExchangeUM-Diagnostics", Utils.GetLocalHostFqdn(), listeningPort, null)
							{
								OutboundConnectionConfiguration = 
								{
									DefaultAddressFamilyHint = new AddressFamilyHint?(0)
								}
							});
						}
					}
				}
				return BaseUMconnectivityTester.SipPlatformConnectionManager.tcpEndPoint;
			}

			internal static ApplicationEndpoint GetTlsEndPoint(int listeningPort, string certificateThumbprint)
			{
				BaseUMconnectivityTester.SipPlatformConnectionManager.DebugTrace("Inside ConnectionManager GetTlsEndpoint().", new object[0]);
				ApplicationEndpoint applicationEndpoint = null;
				X509Certificate2 cert = BaseUMconnectivityTester.SipPlatformConnectionManager.GetCert(certificateThumbprint);
				string key = string.Format(CultureInfo.InvariantCulture, "{0}:{1}", new object[]
				{
					cert.Thumbprint,
					listeningPort
				});
				lock (BaseUMconnectivityTester.SipPlatformConnectionManager.syncLock)
				{
					if (BaseUMconnectivityTester.SipPlatformConnectionManager.tlsEndPoints == null)
					{
						BaseUMconnectivityTester.SipPlatformConnectionManager.tlsEndPoints = new Dictionary<string, ApplicationEndpoint>(StringComparer.OrdinalIgnoreCase);
					}
					if (!BaseUMconnectivityTester.SipPlatformConnectionManager.tlsEndPoints.TryGetValue(key, out applicationEndpoint))
					{
						applicationEndpoint = BaseUMconnectivityTester.SipPlatformConnectionManager.CreateEndPoint(new ServerPlatformSettings("MSExchangeUM-Diagnostics", Utils.GetOwnerHostFqdn(), listeningPort, null, cert)
						{
							OutboundConnectionConfiguration = 
							{
								DefaultAddressFamilyHint = new AddressFamilyHint?(0)
							}
						});
						BaseUMconnectivityTester.SipPlatformConnectionManager.tlsEndPoints.Add(key, applicationEndpoint);
					}
				}
				BaseUMconnectivityTester.SipPlatformConnectionManager.DebugTrace("Returning from GetTLSEndPoint.", new object[0]);
				return applicationEndpoint;
			}

			private static ApplicationEndpoint CreateEndPoint(ServerPlatformSettings settings)
			{
				CollaborationPlatform collaborationPlatform = new CollaborationPlatform(settings);
				collaborationPlatform.EndStartup(collaborationPlatform.BeginStartup(null, null));
				ApplicationEndpoint result;
				try
				{
					ApplicationEndpoint applicationEndpoint = new ApplicationEndpoint(collaborationPlatform, new ApplicationEndpointSettings(BaseUMconnectivityTester.SipPlatformConnectionManager.OwnerUri));
					applicationEndpoint.EndEstablish(applicationEndpoint.BeginEstablish(null, null));
					result = applicationEndpoint;
				}
				catch (Exception)
				{
					collaborationPlatform.EndShutdown(collaborationPlatform.BeginShutdown(null, null));
					throw;
				}
				return result;
			}

			private static X509Certificate2 GetCert(string certificateThumbprint)
			{
				BaseUMconnectivityTester.SipPlatformConnectionManager.DebugTrace("Inside ConnectionManager GetCert() ", new object[0]);
				if (!string.IsNullOrEmpty(certificateThumbprint))
				{
					certificateThumbprint = ManageExchangeCertificate.UnifyThumbprintFormat(certificateThumbprint);
				}
				X509Certificate2 certificateByThumbprintOrServerCertificate = CertificateUtils.GetCertificateByThumbprintOrServerCertificate(certificateThumbprint);
				if (certificateByThumbprintOrServerCertificate == null)
				{
					throw new TUC_CertNotFound();
				}
				return certificateByThumbprintOrServerCertificate;
			}

			private static void DebugTrace(string formatString, params object[] formatObjects)
			{
				ExTraceGlobals.DiagnosticTracer.TraceDebug(0L, formatString, formatObjects);
			}

			private static ApplicationEndpoint tcpEndPoint;

			private static Dictionary<string, ApplicationEndpoint> tlsEndPoints;

			private static string ownerUri;

			private static object syncLock = new object();
		}
	}
}
