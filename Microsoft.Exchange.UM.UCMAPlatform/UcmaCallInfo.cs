using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.UMCommon;
using Microsoft.Exchange.UM.UMCore;
using Microsoft.Rtc.Collaboration;
using Microsoft.Rtc.Collaboration.AudioVideo;
using Microsoft.Rtc.Signaling;

namespace Microsoft.Exchange.UM.UcmaPlatform
{
	internal class UcmaCallInfo : PlatformCallInfo
	{
		public UcmaCallInfo(CallMessageData d, Conversation conv, IPAddress remotePeer)
		{
			this.provider = new UcmaCallInfo.OutboundCallInfoProvider(d, conv, remotePeer);
		}

		public UcmaCallInfo(CallReceivedEventArgs<AudioVideoCall> e)
		{
			this.provider = new UcmaCallInfo.InboundCallInfoProvider(e);
			this.remoteMatchedFqdn = this.provider.RemoteMatchedFQDN;
		}

		public UcmaCallInfo(MessageReceivedEventArgs e, RealTimeConnection connection)
		{
			this.provider = new UcmaCallInfo.ServiceRequestProvider(e, connection);
			this.RemoteMatchedFQDN = this.provider.RemoteMatchedFQDN;
		}

		public override string RemoteMatchedFQDN
		{
			get
			{
				return this.remoteMatchedFqdn;
			}
			set
			{
				this.remoteMatchedFqdn = value;
			}
		}

		public override X509Certificate RemoteCertificate
		{
			get
			{
				return this.provider.RemoteCertificate;
			}
		}

		public override IPAddress RemotePeer
		{
			get
			{
				return this.provider.RemotePeer;
			}
		}

		public override string CallId
		{
			get
			{
				return this.provider.CallId;
			}
		}

		public override PlatformTelephonyAddress CalledParty
		{
			get
			{
				if (this.lazyCalledParty == null)
				{
					this.lazyCalledParty = UcmaCallInfo.AddressFromFromToHeader(this.provider.ToHeader);
				}
				return this.lazyCalledParty;
			}
		}

		public override PlatformTelephonyAddress CallingParty
		{
			get
			{
				if (this.lazyCallingParty == null)
				{
					this.lazyCallingParty = UcmaCallInfo.AddressFromFromToHeader(this.provider.FromHeader);
				}
				return this.lazyCallingParty;
			}
		}

		public override ReadOnlyCollection<PlatformDiversionInfo> DiversionInfo
		{
			get
			{
				if (this.lazyDiversionInfo == null)
				{
					List<PlatformDiversionInfo> diversionInfo = this.provider.DiversionInfo;
					this.lazyDiversionInfo = new ReadOnlyCollection<PlatformDiversionInfo>(diversionInfo);
				}
				return this.lazyDiversionInfo;
			}
		}

		public override string FromTag
		{
			get
			{
				return UcmaCallInfo.TagFromFromToHeader(this.provider.FromHeader);
			}
		}

		public override ReadOnlyCollection<PlatformSignalingHeader> RemoteHeaders
		{
			get
			{
				if (this.lazyRemoteHeaders == null)
				{
					IEnumerable<SignalingHeader> signalingHeaders = this.provider.SignalingHeaders;
					if (signalingHeaders != null)
					{
						List<PlatformSignalingHeader> list = signalingHeaders.ConvertAll((SignalingHeader x) => UcmaSignalingHeader.FromSignalingHeader(x, "INVITE"));
						this.lazyRemoteHeaders = new ReadOnlyCollection<PlatformSignalingHeader>(list);
					}
				}
				return this.lazyRemoteHeaders;
			}
		}

		public override string RemoteUserAgent
		{
			get
			{
				return this.provider.UserAgent;
			}
		}

		public override PlatformSipUri RequestUri
		{
			get
			{
				if (this.requestUri == null)
				{
					string text = this.provider.RequestUri;
					if (!string.IsNullOrEmpty(text))
					{
						this.requestUri = new UcmaSipUri(text);
					}
				}
				return this.requestUri;
			}
		}

		public override string ToTag
		{
			get
			{
				return UcmaCallInfo.TagFromFromToHeader(this.provider.ToHeader);
			}
		}

		public string QualityReportUri
		{
			get
			{
				return this.provider.QualityReportUri;
			}
		}

		public string RemoteContactUri
		{
			get
			{
				return this.provider.RemoteContactUri;
			}
		}

		public override string ApplicationAor
		{
			get
			{
				return this.provider.ApplicationAor;
			}
		}

		public override bool IsInbound
		{
			get
			{
				return this.provider is UcmaCallInfo.InboundCallInfoProvider;
			}
		}

		public override bool IsServiceRequest
		{
			get
			{
				return this.provider is UcmaCallInfo.ServiceRequestProvider;
			}
		}

		private static PlatformTelephonyAddress AddressFromFromToHeader(FromToHeader h)
		{
			PlatformTelephonyAddress result = null;
			if (h != null)
			{
				result = new PlatformTelephonyAddress(h.DisplayName, new UcmaSipUri(h.Uri));
			}
			return result;
		}

		private static string TagFromFromToHeader(FromToHeader h)
		{
			string result = string.Empty;
			if (h != null)
			{
				result = h.Tag;
			}
			return result;
		}

		private UcmaCallInfo.CallInfoProvider provider;

		private PlatformTelephonyAddress lazyCalledParty;

		private PlatformTelephonyAddress lazyCallingParty;

		private ReadOnlyCollection<PlatformDiversionInfo> lazyDiversionInfo;

		private ReadOnlyCollection<PlatformSignalingHeader> lazyRemoteHeaders;

		private PlatformSipUri requestUri;

		private string remoteMatchedFqdn;

		private abstract class CallInfoProvider
		{
			public abstract string CallId { get; }

			public abstract ConversationParticipant LocalParticipant { get; }

			public abstract ConversationParticipant RemoteParticipant { get; }

			public abstract IEnumerable<SignalingHeader> SignalingHeaders { get; }

			public abstract FromToHeader FromHeader { get; }

			public abstract FromToHeader ToHeader { get; }

			public abstract string UserAgent { get; }

			public abstract string RequestUri { get; }

			public abstract List<PlatformDiversionInfo> DiversionInfo { get; }

			public abstract string RemoteMatchedFQDN { get; }

			public abstract X509Certificate RemoteCertificate { get; }

			public abstract IPAddress RemotePeer { get; }

			public abstract string QualityReportUri { get; }

			public abstract string RemoteContactUri { get; }

			public abstract string ApplicationAor { get; }

			protected string GetHeaderUri(SignalingHeader header)
			{
				string result = null;
				if (header != null)
				{
					SignalingHeaderParser signalingHeaderParser = new SignalingHeaderParser(header);
					if (null != signalingHeaderParser.Uri)
					{
						result = signalingHeaderParser.Uri.ToString();
					}
				}
				return result;
			}
		}

		private class InboundCallInfoProvider : UcmaCallInfo.CallInfoProvider
		{
			public InboundCallInfoProvider(CallReceivedEventArgs<AudioVideoCall> e)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.UCMATracer, this, "InboundCallInfoProvider.  CallReceivedEventArgs={0}, Connection={1}", new object[]
				{
					e,
					e.Connection
				});
				this.callReceivedEventArgs = e;
			}

			public override ConversationParticipant LocalParticipant
			{
				get
				{
					return this.callReceivedEventArgs.Call.Conversation.LocalParticipant;
				}
			}

			public override ConversationParticipant RemoteParticipant
			{
				get
				{
					return this.callReceivedEventArgs.RemoteParticipant;
				}
			}

			public override IEnumerable<SignalingHeader> SignalingHeaders
			{
				get
				{
					return this.callReceivedEventArgs.RequestData.SignalingHeaders;
				}
			}

			public override FromToHeader FromHeader
			{
				get
				{
					return this.callReceivedEventArgs.RequestData.FromHeader;
				}
			}

			public override FromToHeader ToHeader
			{
				get
				{
					return this.callReceivedEventArgs.RequestData.ToHeader;
				}
			}

			public override string UserAgent
			{
				get
				{
					return this.callReceivedEventArgs.RequestData.UserAgent;
				}
			}

			public override string RequestUri
			{
				get
				{
					return this.callReceivedEventArgs.RequestData.RequestUri;
				}
			}

			public override string CallId
			{
				get
				{
					return this.callReceivedEventArgs.Call.CallId;
				}
			}

			public override List<PlatformDiversionInfo> DiversionInfo
			{
				get
				{
					return UcmaDiversionInfo.CreateDiversionInfoList(this.callReceivedEventArgs.DiversionContext);
				}
			}

			public override string RemoteMatchedFQDN
			{
				get
				{
					return this.callReceivedEventArgs.Connection.MatchingDomainName;
				}
			}

			public override X509Certificate RemoteCertificate
			{
				get
				{
					return this.callReceivedEventArgs.Connection.RemoteCertificate;
				}
			}

			public override IPAddress RemotePeer
			{
				get
				{
					return this.callReceivedEventArgs.Connection.RemoteEndpoint.Address;
				}
			}

			public override string QualityReportUri
			{
				get
				{
					string text = this.GetDiversionUri();
					if (!this.IsValidReportingUri(text))
					{
						text = base.GetHeaderUri(this.SignalingHeaders.FirstOrDefault((SignalingHeader x) => x.Name.Equals("ms-application-aor", StringComparison.OrdinalIgnoreCase)));
						if (!this.IsValidReportingUri(text))
						{
							text = this.ToHeader.Uri;
						}
					}
					ExAssert.RetailAssert(this.IsValidReportingUri(text), "There is no valid uri for reporting!");
					return text;
				}
			}

			public override string RemoteContactUri
			{
				get
				{
					return base.GetHeaderUri(this.SignalingHeaders.FirstOrDefault((SignalingHeader x) => x.Name.Equals("Contact", StringComparison.OrdinalIgnoreCase)));
				}
			}

			public override string ApplicationAor
			{
				get
				{
					string result = null;
					SignalingHeader signalingHeader = this.SignalingHeaders.FirstOrDefault((SignalingHeader x) => x.Name.Equals("ms-application-aor", StringComparison.OrdinalIgnoreCase));
					if (signalingHeader != null)
					{
						try
						{
							SignalingHeaderParser signalingHeaderParser = new SignalingHeaderParser(signalingHeader);
							if (null != signalingHeaderParser.Uri)
							{
								result = signalingHeaderParser.Uri.UserAtHost;
							}
						}
						catch (MessageParsingException ex)
						{
							CallIdTracer.TraceDebug(ExTraceGlobals.UCMATracer, this, "Failed to parse the ApplicationAor header. Exception: {0}", new object[]
							{
								ex
							});
							result = null;
						}
					}
					return result;
				}
			}

			private string GetDiversionUri()
			{
				string result = null;
				DiversionContext diversionContext = this.callReceivedEventArgs.DiversionContext;
				if (diversionContext != null)
				{
					Collection<DivertedDestination> allDivertedDestinations = diversionContext.GetAllDivertedDestinations();
					if (allDivertedDestinations != null && allDivertedDestinations.Count > 0)
					{
						result = allDivertedDestinations[0].Uri;
					}
				}
				return result;
			}

			private bool IsValidReportingUri(string input)
			{
				bool result = false;
				if (!string.IsNullOrEmpty(input))
				{
					try
					{
						new RealTimeAddress(input);
						result = true;
					}
					catch (ArgumentException ex)
					{
						CallIdTracer.TraceDebug(ExTraceGlobals.UCMATracer, this, "Ignoring invalid reporting uri = {0} {1}", new object[]
						{
							input,
							ex
						});
					}
				}
				return result;
			}

			private CallReceivedEventArgs<AudioVideoCall> callReceivedEventArgs;
		}

		private class OutboundCallInfoProvider : UcmaCallInfo.CallInfoProvider
		{
			public OutboundCallInfoProvider(CallMessageData d, Conversation conv, IPAddress remotePeer)
			{
				this.data = d;
				this.conv = conv;
				this.remotePeer = remotePeer;
			}

			public override ConversationParticipant LocalParticipant
			{
				get
				{
					return this.conv.LocalParticipant;
				}
			}

			public override ConversationParticipant RemoteParticipant
			{
				get
				{
					if (this.conv.RemoteParticipants != null)
					{
						return this.conv.RemoteParticipants[0];
					}
					return null;
				}
			}

			public override IEnumerable<SignalingHeader> SignalingHeaders
			{
				get
				{
					return this.data.MessageData.SignalingHeaders;
				}
			}

			public override FromToHeader FromHeader
			{
				get
				{
					return this.data.MessageData.FromHeader;
				}
			}

			public override FromToHeader ToHeader
			{
				get
				{
					return this.data.MessageData.ToHeader;
				}
			}

			public override string UserAgent
			{
				get
				{
					return this.data.MessageData.UserAgent;
				}
			}

			public override string RequestUri
			{
				get
				{
					return this.data.MessageData.RequestUri;
				}
			}

			public override string CallId
			{
				get
				{
					return this.data.DialogContext.CallID;
				}
			}

			public override List<PlatformDiversionInfo> DiversionInfo
			{
				get
				{
					return new List<PlatformDiversionInfo>();
				}
			}

			public override string RemoteMatchedFQDN
			{
				get
				{
					return string.Empty;
				}
			}

			public override X509Certificate RemoteCertificate
			{
				get
				{
					return null;
				}
			}

			public override IPAddress RemotePeer
			{
				get
				{
					return this.remotePeer;
				}
			}

			public override string QualityReportUri
			{
				get
				{
					throw new InvalidOperationException();
				}
			}

			public override string RemoteContactUri
			{
				get
				{
					throw new InvalidOperationException();
				}
			}

			public override string ApplicationAor
			{
				get
				{
					throw new InvalidOperationException();
				}
			}

			private CallMessageData data;

			private Conversation conv;

			private IPAddress remotePeer;
		}

		private class ServiceRequestProvider : UcmaCallInfo.CallInfoProvider
		{
			public ServiceRequestProvider(MessageReceivedEventArgs e, RealTimeConnection connection)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.UCMATracer, this, "ServiceRequestProvider: args={0} conn={1}", new object[]
				{
					e,
					connection
				});
				this.args = e;
				this.connection = connection;
			}

			public override IEnumerable<SignalingHeader> SignalingHeaders
			{
				get
				{
					return this.args.RequestData.SignalingHeaders;
				}
			}

			public override FromToHeader FromHeader
			{
				get
				{
					return this.args.RequestData.FromHeader;
				}
			}

			public override FromToHeader ToHeader
			{
				get
				{
					return this.args.RequestData.ToHeader;
				}
			}

			public override string UserAgent
			{
				get
				{
					return this.args.RequestData.UserAgent;
				}
			}

			public override string RequestUri
			{
				get
				{
					return this.args.RequestData.RequestUri;
				}
			}

			public override string CallId
			{
				get
				{
					SignalingHeader signalingHeader = this.SignalingHeaders.FirstOrDefault((SignalingHeader x) => x.Name.Equals("Call-ID", StringComparison.OrdinalIgnoreCase));
					if (signalingHeader == null)
					{
						return string.Empty;
					}
					return signalingHeader.GetValue();
				}
			}

			public override List<PlatformDiversionInfo> DiversionInfo
			{
				get
				{
					return new List<PlatformDiversionInfo>();
				}
			}

			public override string RemoteMatchedFQDN
			{
				get
				{
					return this.connection.MatchingDomainName;
				}
			}

			public override X509Certificate RemoteCertificate
			{
				get
				{
					return this.connection.RemoteCertificate;
				}
			}

			public override IPAddress RemotePeer
			{
				get
				{
					return this.connection.RemoteEndpoint.Address;
				}
			}

			public override ConversationParticipant LocalParticipant
			{
				get
				{
					throw new InvalidOperationException();
				}
			}

			public override ConversationParticipant RemoteParticipant
			{
				get
				{
					throw new InvalidOperationException();
				}
			}

			public override string QualityReportUri
			{
				get
				{
					throw new InvalidOperationException();
				}
			}

			public override string RemoteContactUri
			{
				get
				{
					throw new InvalidOperationException();
				}
			}

			public override string ApplicationAor
			{
				get
				{
					throw new InvalidOperationException();
				}
			}

			private MessageReceivedEventArgs args;

			private RealTimeConnection connection;
		}
	}
}
