using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Audio;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Office.Datacenter.WorkerTaskFramework;
using Microsoft.Rtc.Collaboration;
using Microsoft.Rtc.Collaboration.AudioVideo;
using Microsoft.Rtc.Signaling;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.UM.Probes
{
	public sealed class VoiceObject : IDisposable
	{
		private bool AudioReceived { get; set; }

		internal DiagnosticsTracker DiagnosticsTracker { get; private set; }

		public VoiceObject(TracingContext traceContext, string remoteFQDN, bool isLyncPeer, X509Certificate2 cert, SipTransportType sipTransportType, MediaProtocol mediaType) : this(traceContext, remoteFQDN, isLyncPeer, cert, sipTransportType, mediaType, false)
		{
		}

		public VoiceObject(TracingContext traceContext, string remoteFQDN, bool isLyncPeer, X509Certificate2 cert, SipTransportType sipTransportType, MediaProtocol mediaType, bool waitForMedia)
		{
			this.isDisposed = false;
			this.traceContext = traceContext;
			this.DiagnosticsTracker = new DiagnosticsTracker();
			this.DiagnosticsTracker.TrackLocalDiagnostics(15900, "VoiceObject created.", new string[0]);
			this.isLyncPeer = isLyncPeer;
			this.mediaType = mediaType;
			this.waitForMedia = waitForMedia;
			this.AudioReceived = false;
			this.sipTransportType = sipTransportType;
			if (this.sipTransportType == SipTransportType.TCP)
			{
				VoiceObject.InitializeTCPCollaborationPlatform();
				VoiceObject.InitializeTCPEndPoint();
			}
			else
			{
				VoiceObject.InitializeTlsCollaborationPlatform(cert);
				this.AddTrustedDomainToTlsCollaborationPlatform(remoteFQDN);
				VoiceObject.InitializeTLSEndPoint();
			}
			this.DiagnosticsTracker.TrackLocalDiagnostics(15900, "VoiceObject initialized.", new string[0]);
		}

		~VoiceObject()
		{
			this.Dispose(false);
		}

		public static X509Certificate2 CertUsedByTlsCollaborationPlatform { get; private set; }

		public VoiceObject.VoErrorInfo VoErrorInformation { get; set; }

		public string CallId { get; set; }

		public string ConnectionEndpoint { get; set; }

		private static CollaborationPlatform TcpCollaborationPlatformInstance { get; set; }

		private static CollaborationPlatform TlsCollaborationPlatformInstance { get; set; }

		private static LocalEndpoint TcpEndpoint { get; set; }

		private static LocalEndpoint TlsEndpoint { get; set; }

		private AudioVideoCall Call
		{
			get
			{
				return this.call;
			}
		}

		public void CallUM(string remoteHostFqdn, int remoteSipPort, string umHuntgroupNumber, string callerId, DiversionType diversionType, string diversionValue, string organizationName)
		{
			IList<VoiceObject.HeaderInfo> list = null;
			Conversation conversation = new Conversation(this.GetEndPoint());
			AudioVideoCall audioVideoCall = new AudioVideoCall(conversation);
			this.RegisterCall(audioVideoCall);
			if (this.isLyncPeer)
			{
				list = new List<VoiceObject.HeaderInfo>();
				list.Add(new VoiceObject.HeaderInfo("Route", string.Format(CultureInfo.InvariantCulture, "<sip:{0};{1}={2};lr>;{3}", new object[]
				{
					umHuntgroupNumber,
					"transport",
					this.sipTransportType,
					"ms-edge-route"
				})));
				list.Add(new VoiceObject.HeaderInfo("ms-split-domain-info", "ms-traffic-type=SplitIntra"));
			}
			this.InternalCallUM(remoteHostFqdn, remoteSipPort, umHuntgroupNumber, callerId, diversionValue, organizationName, list);
		}

		public void DisconnectCall()
		{
			try
			{
				this.WriteTrace("Attempting to disconnect the call...", new object[0]);
				if (this.Call != null)
				{
					Conversation conversation = this.Call.Conversation;
					CallTerminateOptions callTerminateOptions = new CallTerminateOptions();
					if (this.isLyncPeer)
					{
						SignalingHeader item = new SignalingHeader("ms-split-domain-info", "ms-traffic-type=SplitIntra");
						callTerminateOptions.Headers.Add(item);
					}
					this.Call.EndTerminate(this.Call.BeginTerminate(callTerminateOptions, null, null));
					if (conversation != null)
					{
						conversation.EndTerminate(conversation.BeginTerminate(null, null));
					}
					this.WriteTrace("Call Disconnected", new object[0]);
					this.call = null;
				}
				else
				{
					this.WriteTrace("No call to disconnect (call == null)", new object[0]);
				}
			}
			catch (RealTimeException ex)
			{
				this.WriteTrace("RealTimeException occured {0}", new object[]
				{
					ex
				});
			}
			catch (InvalidOperationException ex2)
			{
				this.WriteTrace("InvalidOperationException occured {0}", new object[]
				{
					ex2
				});
			}
		}

		public bool SendSipOptionPing(string remoteHostFqdn, string certificateSubjectName, int remoteSipPort)
		{
			SipResponseData responseData = null;
			int num = 0;
			RealTimeAddress realTimeAddressForRemoteEndpoint;
			if (this.sipTransportType == SipTransportType.TCP)
			{
				realTimeAddressForRemoteEndpoint = new RealTimeAddress(string.Format(CultureInfo.InvariantCulture, "sip:{0}:{1};{2}={3}", new object[]
				{
					remoteHostFqdn,
					remoteSipPort,
					"transport",
					this.sipTransportType
				}));
			}
			else
			{
				realTimeAddressForRemoteEndpoint = new RealTimeAddress(string.Format(CultureInfo.InvariantCulture, "sip:{0}:{1};{2}={3};{4}={5}", new object[]
				{
					certificateSubjectName,
					remoteSipPort,
					"ms-fe",
					remoteHostFqdn,
					"transport",
					this.sipTransportType
				}));
			}
			this.RunAndCatchRealTimeAndInvalidOperationException(delegate
			{
				responseData = this.GetEndPoint().InnerEndpoint.EndSendMessage(this.GetEndPoint().InnerEndpoint.BeginSendMessage(2, realTimeAddressForRemoteEndpoint, null, null, null, null));
			});
			if (responseData != null)
			{
				num = responseData.ResponseCode;
			}
			return num == 200;
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		private static void InitializeTlsCollaborationPlatform(X509Certificate2 cert)
		{
			if (VoiceObject.TlsCollaborationPlatformInstance == null)
			{
				lock (VoiceObject.syncLock)
				{
					if (VoiceObject.TlsCollaborationPlatformInstance == null)
					{
						ServerPlatformSettings serverPlatformSettings = new ServerPlatformSettings("ActiveMonitoringClient", Utils.GetLocalHostIPv4().ToString(), 0, null, cert.Issuer, cert.GetSerialNumber());
						VoiceObject.TlsCollaborationPlatformInstance = VoiceObject.InitializeCollaborationPlatform(serverPlatformSettings);
						VoiceObject.CertUsedByTlsCollaborationPlatform = cert;
					}
				}
			}
		}

		private static void InitializeTCPCollaborationPlatform()
		{
			if (VoiceObject.TcpCollaborationPlatformInstance == null)
			{
				lock (VoiceObject.syncLock)
				{
					if (VoiceObject.TcpCollaborationPlatformInstance == null)
					{
						ServerPlatformSettings serverPlatformSettings = new ServerPlatformSettings("ActiveMonitoringClient", Utils.GetLocalHostIPv4().ToString(), 0, null);
						VoiceObject.TcpCollaborationPlatformInstance = VoiceObject.InitializeCollaborationPlatform(serverPlatformSettings);
					}
				}
			}
		}

		private static CollaborationPlatform InitializeCollaborationPlatform(ServerPlatformSettings serverPlatformSettings)
		{
			CollaborationPlatform collaborationPlatform = new CollaborationPlatform(serverPlatformSettings);
			collaborationPlatform.EndStartup(collaborationPlatform.BeginStartup(null, null));
			return collaborationPlatform;
		}

		private static void InitializeTCPEndPoint()
		{
			if (VoiceObject.TcpEndpoint == null)
			{
				lock (VoiceObject.syncLock)
				{
					if (VoiceObject.TcpEndpoint == null)
					{
						VoiceObject.TcpEndpoint = VoiceObject.InitializeEndPoint(VoiceObject.TcpCollaborationPlatformInstance);
					}
				}
			}
		}

		private static void InitializeTLSEndPoint()
		{
			if (VoiceObject.TlsEndpoint == null)
			{
				lock (VoiceObject.syncLock)
				{
					if (VoiceObject.TlsEndpoint == null)
					{
						VoiceObject.TlsEndpoint = VoiceObject.InitializeEndPoint(VoiceObject.TlsCollaborationPlatformInstance);
					}
				}
			}
		}

		private static LocalEndpoint InitializeEndPoint(CollaborationPlatform collaborationPlatform)
		{
			string text = string.Format(CultureInfo.InvariantCulture, "sip:{0}@{1}", new object[]
			{
				"Callmanager",
				Utils.GetLocalHostIPv4().ToString()
			});
			ApplicationEndpointSettings applicationEndpointSettings = new ApplicationEndpointSettings(text);
			applicationEndpointSettings.IsDefaultRoutingEndpoint = true;
			applicationEndpointSettings.SetEndpointType(1, 0);
			applicationEndpointSettings.ProvisioningDataQueryDisabled = true;
			applicationEndpointSettings.PublishingQoeMetricsDisabled = true;
			LocalEndpoint localEndpoint = new ApplicationEndpoint(collaborationPlatform, applicationEndpointSettings);
			localEndpoint.EndEstablish(localEndpoint.BeginEstablish(null, null));
			return localEndpoint;
		}

		private void AddTrustedDomainToTlsCollaborationPlatform(string remoteHostFqdn)
		{
			TrustedDomainMode trustedDomainMode = this.isLyncPeer ? 0 : 1;
			TrustedDomain trustedDomain = new TrustedDomain(remoteHostFqdn, trustedDomainMode);
			lock (VoiceObject.syncLock)
			{
				VoiceObject.TlsCollaborationPlatformInstance.AddTrustedDomain(trustedDomain);
			}
		}

		private LocalEndpoint GetEndPoint()
		{
			if (this.sipTransportType == SipTransportType.TCP)
			{
				return VoiceObject.TcpEndpoint;
			}
			return VoiceObject.TlsEndpoint;
		}

		private void Dispose(bool disposing)
		{
			if (!this.isDisposed && disposing)
			{
				this.ClearFlowObjects();
				this.ClearCall();
				this.isDisposed = true;
			}
		}

		private void ClearCall()
		{
			if (this.Call != null)
			{
				this.Call.AudioVideoFlowConfigurationRequested -= this.OnAudioVideoFlowConfigurationRequested;
				this.Call.StateChanged -= this.OnStateChanged;
				this.DisconnectCall();
			}
		}

		private void InternalCallUM(string remoteHostFqdn, int remoteSipPort, string umHuntgroupNumber, string callerId, string diversionValue, string organizationName, IList<VoiceObject.HeaderInfo> otherHeaders)
		{
			this.callOptions = new CallEstablishOptions();
			string remoteSipURI;
			string text;
			string text3;
			if (this.isLyncPeer)
			{
				text = string.Format(CultureInfo.InvariantCulture, "<sip:{0}> ;reason=no-answer", new object[]
				{
					diversionValue
				});
				string text2 = string.Format(CultureInfo.InvariantCulture, "{0}={1}", new object[]
				{
					"ms-organization",
					organizationName
				});
				text3 = string.Format(CultureInfo.InvariantCulture, "sip:{0}:{1};{2}={3}", new object[]
				{
					callerId,
					remoteSipPort,
					"transport",
					this.sipTransportType
				});
				remoteSipURI = string.Format(CultureInfo.InvariantCulture, "sip:{0}:{1};{2}", new object[]
				{
					diversionValue,
					remoteSipPort,
					text2
				});
				this.callOptions.ConnectionContext = new ConnectionContext(remoteHostFqdn, remoteSipPort);
				this.callOptions.ConnectionContext.AddressFamilyHint = new AddressFamilyHint?(0);
			}
			else
			{
				text = string.Format("<tel:{0}> ;reason=no-answer", diversionValue);
				text3 = this.GetEndPoint().InnerEndpoint.Uri;
				text3 = text3.Replace("Callmanager", callerId);
				remoteSipURI = string.Format(CultureInfo.InvariantCulture, "sip:{0}@{1}:{2};{3}={4}", new object[]
				{
					umHuntgroupNumber,
					remoteHostFqdn,
					remoteSipPort,
					"transport",
					this.sipTransportType
				});
			}
			this.SetOptionalHeaders(otherHeaders);
			SignalingHeader item = new SignalingHeader("Diversion", text);
			this.callOptions.Headers.Add(item);
			this.WriteTrace("Diversion Header: {0}", new object[]
			{
				text
			});
			this.WriteTrace("local SipURI = {0}", new object[]
			{
				text3
			});
			this.WriteTrace("remote SipURI = {0}", new object[]
			{
				remoteSipURI
			});
			foreach (SignalingHeader signalingHeader in this.callOptions.Headers)
			{
				this.WriteTrace("header {0} = {1}", new object[]
				{
					signalingHeader.Name,
					signalingHeader.GetValue()
				});
			}
			this.Call.Conversation.Impersonate(text3, null, string.Empty);
			this.DiagnosticsTracker.TrackLocalDiagnostics(15901, "VoiceObject internal call UM.", new string[0]);
			this.RunAndCatchRealTimeAndInvalidOperationException(delegate
			{
				this.Call.EndEstablish(this.Call.BeginEstablish(remoteSipURI, this.callOptions, null, null));
			});
			if (this.waitForMedia)
			{
				this.RunAndCatchRealTimeAndInvalidOperationException(delegate
				{
					this.WaitForMedia();
				});
			}
			this.DisconnectCall();
			this.DiagnosticsTracker.TrackLocalDiagnostics(15905, "VoiceObject call disconnected", new string[0]);
		}

		private void WaitForMedia()
		{
			Thread.Sleep(TimeSpan.FromSeconds(30.0));
			if (!this.AudioReceived)
			{
				throw new MediaException("Audio not received");
			}
		}

		private void SetOptionalHeaders(IEnumerable<VoiceObject.HeaderInfo> headers)
		{
			if (headers != null)
			{
				foreach (VoiceObject.HeaderInfo headerInfo in headers)
				{
					this.callOptions.Headers.Add(new SignalingHeader(headerInfo.HeaderName, headerInfo.HeaderValue));
				}
			}
		}

		private void RegisterCall(AudioVideoCall call)
		{
			this.call = call;
			this.Call.AudioVideoFlowConfigurationRequested += this.OnAudioVideoFlowConfigurationRequested;
			this.Call.StateChanged += this.OnStateChanged;
			this.Call.ProvisionalResponseReceived += this.OnProvisionalResponseReceived;
		}

		private void OnAudioVideoFlowConfigurationRequested(object sender, AudioVideoFlowConfigurationRequestedEventArgs e)
		{
			this.RunAndCatchRealTimeAndInvalidOperationException(delegate
			{
				this.WriteTrace("configuring SDP so that it supports {0}", new object[]
				{
					this.mediaType
				});
				this.RegisterFlow(e.Flow);
				AudioVideoFlowTemplate audioVideoFlowTemplate = new AudioVideoFlowTemplate(this.flow);
				audioVideoFlowTemplate.EncryptionPolicy = ((this.mediaType == MediaProtocol.SRTP) ? 3 : 1);
				this.flow.Initialize(audioVideoFlowTemplate);
			});
		}

		private void RegisterFlow(AudioVideoFlow flow)
		{
			this.flow = flow;
			this.flow.StateChanged += this.OnFlowStateChanged;
		}

		private void ClearFlowObjects()
		{
			if (this.flow != null)
			{
				this.flow.StateChanged -= this.OnFlowStateChanged;
				this.flow = null;
			}
		}

		private void OnFlowStateChanged(object sender, MediaFlowStateChangedEventArgs e)
		{
			this.RunAndCatchRealTimeAndInvalidOperationException(delegate
			{
				this.WriteTrace("Media flow state: {0} ==> {1}", new object[]
				{
					e.PreviousState,
					e.State
				});
				if (this.waitForMedia && this.flow.State == 1)
				{
					this.AudioReceived = true;
					this.HandleAudio();
				}
			});
		}

		private void HandleAudio()
		{
			SpeechRecognitionConnector speechRecognitionConnector = new SpeechRecognitionConnector();
			speechRecognitionConnector.AttachFlow(this.flow);
			SpeechRecognitionStream speechRecognitionStream = speechRecognitionConnector.Start();
			byte[] array = new byte[16000];
			bool flag = true;
			int numofBytes;
			while ((numofBytes = speechRecognitionStream.Read(array, 0, array.Length)) != 0)
			{
				double num = AudioNormalizer.CalcEnergyRms(array, numofBytes);
				if (num > 0.088)
				{
					flag = false;
					this.DiagnosticsTracker.TrackLocalDiagnostics(15906, "Received audio in call", new string[0]);
					break;
				}
			}
			if (flag)
			{
				this.WriteTrace("Only noise in the received audio", new object[0]);
				throw new MediaException("Only noise found in media");
			}
		}

		private void OnStateChanged(object sender, CallStateChangedEventArgs e)
		{
			this.RunAndCatchRealTimeAndInvalidOperationException(delegate
			{
				if (string.IsNullOrEmpty(this.CallId))
				{
					if (e.MessageData != null && !string.IsNullOrEmpty(e.MessageData.CallId))
					{
						this.CallId = e.MessageData.CallId;
					}
					else if (!string.IsNullOrEmpty(this.Call.CallId))
					{
						this.CallId = this.Call.CallId;
					}
					if (!string.IsNullOrEmpty(this.CallId))
					{
						this.WriteTrace("CallId for this Call: {0}", new object[]
						{
							this.CallId
						});
					}
				}
				if (string.IsNullOrEmpty(this.ConnectionEndpoint))
				{
					object propertyByName = this.GetPropertyByName("PrimarySession", this.Call);
					object propertyByName2 = this.GetPropertyByName("SignalingSession", propertyByName);
					object propertyByName3 = this.GetPropertyByName("LastKnownConnection", propertyByName2);
					object propertyByName4 = this.GetPropertyByName("SipStackConnection", propertyByName3);
					object propertyByName5 = this.GetPropertyByName("DestinationEndPoint", propertyByName4);
					if (propertyByName5 != null)
					{
						this.ConnectionEndpoint = propertyByName5.ToString();
					}
				}
				this.WriteTrace("Call state: {0} ==> {1}  Reason: {2}", new object[]
				{
					e.PreviousState,
					e.State,
					e.TransitionReason
				});
				if (e.TransitionReason == 3)
				{
					this.DiagnosticsTracker.TrackLocalDiagnostics(15902, "VoiceObject call establishing.", new string[0]);
					return;
				}
				if (e.TransitionReason == 5)
				{
					this.DiagnosticsTracker.TrackLocalDiagnostics(15903, "VoiceObject call established.", new string[0]);
					return;
				}
				if (e.TransitionReason == 6)
				{
					this.DiagnosticsTracker.TrackLocalDiagnostics(15904, "VoiceObject call establish failed.", new string[0]);
				}
			});
		}

		private void OnProvisionalResponseReceived(object sender, CallProvisionalResponseReceivedEventArgs e)
		{
			this.RunAndCatchRealTimeAndInvalidOperationException(delegate
			{
				SipResponseData responseData = e.ResponseData;
				if (responseData.ResponseCode == 101)
				{
					this.ProcessMsDiagnostics(responseData.SignalingHeaders);
				}
			});
		}

		private void ProcessMsDiagnostics(IEnumerable<SignalingHeader> headers)
		{
			foreach (SignalingHeader signalingHeader in headers)
			{
				if (string.Equals(signalingHeader.Name, "ms-diagnostics-public", StringComparison.InvariantCultureIgnoreCase) || string.Equals(signalingHeader.Name, "ms-diagnostics", StringComparison.InvariantCultureIgnoreCase))
				{
					this.DiagnosticsTracker.TrackDiagnostics(signalingHeader.GetValue());
					break;
				}
			}
		}

		private string GetHostNameFromFQDN(string fqdn)
		{
			int num = fqdn.IndexOf('.');
			if (num != -1)
			{
				fqdn = fqdn.Substring(0, num);
			}
			return fqdn;
		}

		private void RunAndCatchRealTimeAndInvalidOperationException(Action action)
		{
			try
			{
				action();
			}
			catch (Exception e)
			{
				this.TryExtractFailureResponseException(e);
			}
		}

		private void TryExtractFailureResponseException(Exception e)
		{
			if (e is RealTimeException)
			{
				SipResponseData sipResponseData = null;
				if (e is FailureResponseException)
				{
					sipResponseData = (e as FailureResponseException).ResponseData;
				}
				else if (e.InnerException is FailureResponseException)
				{
					FailureResponseException ex = e.InnerException as FailureResponseException;
					sipResponseData = ex.ResponseData;
				}
				this.WriteTrace("Get the InnerException error message {0}", new object[]
				{
					e.InnerException
				});
				if (sipResponseData != null)
				{
					IList<VoiceObject.HeaderInfo> list = new List<VoiceObject.HeaderInfo>();
					this.WriteTrace("Get responseData {0}, {1}", new object[]
					{
						sipResponseData.ResponseCode,
						sipResponseData.ResponseText
					});
					StringBuilder stringBuilder = new StringBuilder();
					foreach (SignalingHeader signalingHeader in sipResponseData.SignalingHeaders)
					{
						stringBuilder.AppendLine(signalingHeader.Name + " --> " + signalingHeader.GetValue());
						list.Add(new VoiceObject.HeaderInfo(signalingHeader.Name, signalingHeader.GetValue()));
					}
					this.WriteTrace("ResponseData Header \n{0}", new object[]
					{
						stringBuilder
					});
					this.ProcessMsDiagnostics(sipResponseData.SignalingHeaders);
					this.VoErrorInformation = new VoiceObject.VoErrorInfo(sipResponseData.ResponseCode, sipResponseData.ResponseText, list, e);
					return;
				}
				this.WriteTrace("No response message accept!", new object[0]);
				this.VoErrorInformation = new VoiceObject.VoErrorInfo(e);
				return;
			}
			else
			{
				if (e is InvalidOperationException)
				{
					this.WriteTrace("InvalidOperationException has occurred", new object[]
					{
						e
					});
					this.VoErrorInformation = new VoiceObject.VoErrorInfo(e);
					return;
				}
				if (e is MediaException)
				{
					this.WriteTrace("Media Exception occured", new object[0]);
					this.VoErrorInformation = new VoiceObject.VoErrorInfo(e);
					return;
				}
				throw e;
			}
		}

		private void WriteTrace(string format, params object[] objs)
		{
			WTFDiagnostics.TraceInformation(ExTraceGlobals.UnifiedMessagingTracer, this.traceContext, string.Format(format, objs), null, "WriteTrace", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\UM\\VoiceObject.cs", 1104);
		}

		private object GetPropertyByName(string propertyName, object instance)
		{
			if (instance != null)
			{
				try
				{
					Type type = instance.GetType();
					return type.InvokeMember(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.GetProperty, null, instance, null);
				}
				catch (MissingMemberException ex)
				{
					this.WriteTrace("UCMA property changed. {0}", new object[]
					{
						ex
					});
					return null;
				}
			}
			return null;
		}

		private const string ApplicationUserAgent = "ActiveMonitoringClient";

		private const string Identity = "Callmanager";

		private const string Diversion = "Diversion";

		private const string Route = "Route";

		private const string Transport = "transport";

		private const string MsEdgeRoute = "ms-edge-route";

		private const string MsSplitDomainInfo = "ms-split-domain-info";

		private const string SplitIntra = "ms-traffic-type=SplitIntra";

		private const string MsOrganization = "ms-organization";

		private readonly bool isLyncPeer;

		private static object syncLock = new object();

		private MediaProtocol mediaType;

		private readonly bool waitForMedia;

		private SipTransportType sipTransportType;

		private bool isDisposed;

		private AudioVideoCall call;

		private AudioVideoFlow flow;

		private CallEstablishOptions callOptions;

		private TracingContext traceContext;

		public class HeaderInfo
		{
			public HeaderInfo(string name, string value)
			{
				this.HeaderName = name;
				this.HeaderValue = value;
			}

			public string HeaderName { get; private set; }

			public string HeaderValue { get; private set; }
		}

		public class VoErrorInfo
		{
			public VoErrorInfo(int code, string message, IList<VoiceObject.HeaderInfo> headers, Exception exception)
			{
				this.Code = code;
				this.Message = message;
				this.Headers = headers;
				this.Exception = exception;
			}

			public VoErrorInfo(Exception ex) : this(-1, null, new List<VoiceObject.HeaderInfo>(), ex)
			{
			}

			public int Code { get; private set; }

			public string Message { get; private set; }

			public IList<VoiceObject.HeaderInfo> Headers { get; private set; }

			public Exception Exception { get; private set; }

			public string GetMsDiagnostics()
			{
				string result = null;
				if (this.Headers != null)
				{
					foreach (VoiceObject.HeaderInfo headerInfo in this.Headers)
					{
						if (string.Equals(headerInfo.HeaderName, "ms-diagnostics", StringComparison.InvariantCultureIgnoreCase) || string.Equals(headerInfo.HeaderName, "ms-diagnostics-public", StringComparison.InvariantCultureIgnoreCase))
						{
							result = headerInfo.HeaderValue;
							break;
						}
					}
				}
				return result;
			}
		}
	}
}
