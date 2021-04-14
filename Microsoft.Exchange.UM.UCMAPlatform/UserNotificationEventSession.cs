using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.UM.TroubleshootingTool.Shared;
using Microsoft.Exchange.UM.UMCommon;
using Microsoft.Exchange.UM.UMCore;
using Microsoft.Exchange.UM.UMCore.Exceptions;
using Microsoft.Exchange.VariantConfiguration;
using Microsoft.Rtc.Signaling;

namespace Microsoft.Exchange.UM.UcmaPlatform
{
	internal class UserNotificationEventSession : IOfferAnswer
	{
		internal UserNotificationEventSession(DiagnosticHelper diagnostics, SignalingSession session, UserNotificationEventHandler handler)
		{
			this.diagnostics = diagnostics;
			this.session = session;
			this.handler = handler;
			this.sessionId = session.CallId;
			this.session.OfferAnswerNegotiation = this;
			this.session.StateChanged += this.HandleStateChanged;
			this.session.MessageReceived += this.HandleMessageReceived;
		}

		internal TimeSpan IdleTime
		{
			get
			{
				return ExDateTime.UtcNow.Subtract(this.lastInfoReceivedDate);
			}
		}

		internal string Id
		{
			get
			{
				return this.sessionId;
			}
		}

		internal bool IsActive
		{
			get
			{
				return this.session != null;
			}
		}

		internal void Start(SessionReceivedEventArgs args)
		{
			this.TraceDebug("Start()", new object[0]);
			UserNotificationEventSession.SIPStatus status = UserNotificationEventSession.SIPStatus.Forbidden;
			lock (this)
			{
				Exception ex = UcmaUtils.CatchRealtimeErrors(delegate
				{
					if (this.ValidateSession(args))
					{
						status = this.ProcessRemoteSdp(args.MediaDescription);
					}
					if (status.Success)
					{
						this.mediaDescription = this.GenerateSessionDescription();
						this.BeginParticipate();
						return;
					}
					object obj;
					if (this.session != null)
					{
						obj = this.session.Endpoint;
						this.RejectSession(status, UserNotificationEventSession.SIPStatus.InvalidSDPWarning);
					}
					else
					{
						obj = string.Empty;
					}
					UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_EventNotifSessionInvalidFormat, null, new object[]
					{
						obj,
						this.Id
					});
				}, this.diagnostics);
				if (ex != null)
				{
					this.TraceError("UserNotificationEventSession.Start: {0}", new object[]
					{
						ex
					});
					this.LogSignalingErrorEvent(ex);
				}
			}
		}

		internal void Terminate()
		{
			lock (this)
			{
				if (this.session != null)
				{
					try
					{
						if (this.session.State != null && this.session.State != 4)
						{
							this.session.BeginTerminate(null, null);
						}
						else
						{
							this.TraceDebug("Terminate():Session({0}).State={1} - waiting for StateChange event", new object[]
							{
								this.Id,
								this.session.State
							});
						}
						goto IL_C2;
					}
					catch (RealTimeException ex)
					{
						this.TraceError("Terminate: {0}", new object[]
						{
							ex
						});
						goto IL_C2;
					}
					catch (InvalidOperationException ex2)
					{
						this.TraceError("Terminate: {0}", new object[]
						{
							ex2
						});
						goto IL_C2;
					}
				}
				this.TraceDebug("Terminate: Session was already terminated", new object[0]);
				IL_C2:;
			}
		}

		private static string GetHeaderValue(IEnumerable<SignalingHeader> headerList, string name)
		{
			foreach (SignalingHeader signalingHeader in headerList)
			{
				if (string.Compare(signalingHeader.Name, name, StringComparison.OrdinalIgnoreCase) == 0)
				{
					return signalingHeader.GetValue();
				}
			}
			return string.Empty;
		}

		private static bool IsTLSCall(SessionReceivedEventArgs args)
		{
			string protocolName = args.Session.Connection.ProtocolName;
			return 0 == string.Compare(protocolName, "TLS", StringComparison.InvariantCultureIgnoreCase);
		}

		private void LogSignalingErrorEvent(Exception error)
		{
			RealTimeEndpoint realTimeEndpoint = (this.session != null) ? this.session.Endpoint : null;
			UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_EventNotifSessionSignalingError, null, new object[]
			{
				realTimeEndpoint,
				this.Id,
				error.Message
			});
		}

		private void BeginParticipate()
		{
			this.TraceDebug("BeginParticipate()", new object[0]);
			this.session.BeginParticipate(null, null);
		}

		private void RejectSession(UserNotificationEventSession.SIPStatus status, UserNotificationEventSession.SIPStatus warning)
		{
			this.TraceDebug("RejectSession - Status={0}, Warning={1}", new object[]
			{
				status,
				warning
			});
			PlatformSignalingHeader platformSignalingHeader = CallRejectedException.RenderDiagnosticHeader(CallEndingReason.TransientError, status.Text, new object[0]);
			List<SignalingHeader> list = new List<SignalingHeader>();
			string text = string.Format(CultureInfo.InvariantCulture, "{0} {1} \"{2}\"", new object[]
			{
				warning.Code,
				Utils.GetLocalHostName(),
				warning.Text
			});
			list.Add(new SignalingHeader("Warning", text));
			list.Add(new SignalingHeader(platformSignalingHeader.Name, platformSignalingHeader.Value));
			this.session.TerminateWithRejection(status.Code, status.Text, list);
		}

		private void HandleStateChanged(object sender, SignalingStateChangedEventArgs e)
		{
			this.TraceDebug("HandleStateChanged: PreviousState={0}, NewState={1}, Reason={2}", new object[]
			{
				e.PreviousState,
				e.State,
				e.Reason
			});
			if (e.State == 4 || e.State == null)
			{
				this.HandleSessionTerminated();
			}
		}

		private void HandleMessageReceived(object sender, MessageReceivedEventArgs e)
		{
			this.TraceDebug("HandleMessageReceived - {0}", new object[]
			{
				e.MessageType
			});
			RealTimeConnection connection = e.Connection;
			if (connection == null)
			{
				this.TraceDebug("Connection is null. Ignoring request.", new object[0]);
				return;
			}
			lock (this)
			{
				this.lastInfoReceivedDate = ExDateTime.UtcNow;
			}
			int? responseCode = null;
			PlatformSignalingHeader diagnosticHeader = null;
			UserNotificationEventContext context = new UserNotificationEventContext();
			using (new CallId(this.Id))
			{
				Exception innerError = null;
				Exception ex = UcmaUtils.CatchRealtimeErrors(delegate
				{
					if (VariantConfiguration.InvariantNoFlightingSnapshot.UM.SipInfoNotifications.Enabled)
					{
						innerError = UcmaUtils.ProcessPlatformRequestAndReportErrors(delegate
						{
							this.handler(new UcmaCallInfo(e, connection), e.GetBody(), context);
						}, this.Id, out diagnosticHeader);
						if (innerError == null)
						{
							diagnosticHeader = null;
							return;
						}
					}
					else
					{
						this.TraceError("User notification events via SIP INFO are not supported in hosted exchange", new object[0]);
						responseCode = new int?(UserNotificationEventSession.SIPStatus.Forbidden.Code);
						innerError = CallRejectedException.Create(Strings.InvalidRequest, CallEndingReason.UnsupportedRequest, "User notification events via SIP INFO are not supported in hosted Exchange", new object[0]);
					}
				}, this.diagnostics);
				Exception ex2 = innerError ?? ex;
				bool flag2 = false;
				CallRejectedException ex3 = ex2 as CallRejectedException;
				if (ex3 != null)
				{
					flag2 = (ex3.Reason == CallEndingReason.NotificationNotSupportedForLegacyUser || ex3.Reason == CallEndingReason.MailboxIsNotUMEnabled);
				}
				if (ex2 != null && !flag2)
				{
					this.TraceError("HandleMessageReceived - Encountered error {0}", new object[]
					{
						ex2
					});
					if (ex2 is UMGrayException && (ex2 as UMGrayException).InnerException is ArgumentException)
					{
						responseCode = new int?(UserNotificationEventSession.SIPStatus.BadRequest.Code);
					}
					responseCode = new int?(responseCode ?? UserNotificationEventSession.SIPStatus.InternalServerError.Code);
					diagnosticHeader = (diagnosticHeader ?? CallRejectedException.RenderDiagnosticHeader(CallEndingReason.TransientError, null, null));
				}
				else
				{
					responseCode = new int?(UserNotificationEventSession.SIPStatus.OK.Code);
				}
				List<SignalingHeader> headers = new List<SignalingHeader>(1);
				if (diagnosticHeader != null)
				{
					this.TraceError("HandleMessageReceived - diagnostic header = {0}", new object[]
					{
						diagnosticHeader
					});
					headers.Add(new SignalingHeader(diagnosticHeader.Name, diagnosticHeader.Value));
				}
				UcmaUtils.CatchRealtimeErrors(delegate
				{
					e.SendResponse(responseCode.Value, null, null, headers);
				}, this.diagnostics);
				if (!flag2)
				{
					bool flag3 = responseCode == UserNotificationEventSession.SIPStatus.OK.Code;
					if (!flag3)
					{
						UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_UserNotificationFailed, null, new object[]
						{
							CommonUtil.ToEventLogString(e.RequestData.FromHeader.Uri),
							CommonUtil.ToEventLogString(e.RequestData.ToHeader.Uri),
							CommonUtil.ToEventLogString(responseCode),
							CommonUtil.ToEventLogString(diagnosticHeader),
							CommonUtil.ToEventLogString(context.User),
							CommonUtil.ToEventLogString((context.Backend != null) ? context.Backend.Fqdn : string.Empty)
						});
					}
					Util.SetCounter(CallRouterAvailabilityCounters.RecentMissedCallNotificationProxyFailed, (long)UserNotificationEventSession.recentMissedCallNotificationsProxyFailed.Update(flag3));
				}
			}
			this.TraceDebug("HandleMessageReceived - responseCode = {0}", new object[]
			{
				responseCode
			});
		}

		private void HandleSessionTerminated()
		{
			this.TraceDebug("HandleSessionTerminated()", new object[0]);
			lock (this)
			{
				if (this.session != null)
				{
					try
					{
						try
						{
							this.TraceDebug("HandleSessionTerminated - unwiring event handlers", new object[0]);
							this.session.OfferAnswerNegotiation = null;
							this.session.StateChanged -= this.HandleStateChanged;
							this.session.MessageReceived -= this.HandleMessageReceived;
						}
						catch (RealTimeException ex)
						{
							this.TraceError("HandleSessionTerminated: {0}", new object[]
							{
								ex
							});
						}
						catch (InvalidOperationException ex2)
						{
							this.TraceError("HandleSessionTerminated: {0}", new object[]
							{
								ex2
							});
						}
						goto IL_CA;
					}
					finally
					{
						this.session = null;
					}
				}
				this.TraceDebug("HandleSessionTerminated: Session was already terminated", new object[0]);
				IL_CA:;
			}
		}

		private bool ValidateSession(SessionReceivedEventArgs args)
		{
			if (args.Session.Connection == null)
			{
				this.TraceError("ValidateSession: Connection is unavailable.", new object[0]);
				return false;
			}
			if (!UserNotificationEventSession.IsTLSCall(args))
			{
				this.TraceError("ValidateSession: Session is not a TLS call!", new object[0]);
				return false;
			}
			string headerValue = UserNotificationEventSession.GetHeaderValue(args.RequestData.SignalingHeaders, "FROM");
			return 0 < headerValue.IndexOf("sip:A410AA79-D874-4e56-9B46-709BDD0EB850", StringComparison.OrdinalIgnoreCase);
		}

		private ContentDescription GenerateSessionDescription()
		{
			Sdp<SdpGlobalDescription, SdpMediaDescription> sdp = new Sdp<SdpGlobalDescription, SdpMediaDescription>();
			sdp.GlobalDescription.Origin.Version = 0L;
			sdp.GlobalDescription.Origin.Connection.HostName = this.session.Endpoint.ConnectionManager.LocalHostName;
			sdp.GlobalDescription.Origin.SessionId = "0";
			SdpMediaDescription sdpMediaDescription = new SdpMediaDescription("application");
			sdpMediaDescription.Port = 9;
			sdpMediaDescription.TransportProtocol = "SIP";
			sdpMediaDescription.Formats = "*";
			sdpMediaDescription.Attributes.Add(new SdpAttribute("recvonly", null));
			sdpMediaDescription.Attributes.Add(new SdpAttribute("accept-types", "application/ms-rtc-usernotification+xml"));
			sdpMediaDescription.Attributes.Add(new SdpAttribute("ms-rtc-accept-eventtemplates", "RtcDefault"));
			sdp.MediaDescriptions.Add(sdpMediaDescription);
			return new ContentDescription(Constants.OCS.SDPContentType, sdp.GetBytes());
		}

		private UserNotificationEventSession.SIPStatus ProcessRemoteSdp(ContentDescription description)
		{
			if (description == null)
			{
				return UserNotificationEventSession.SIPStatus.BadRequest;
			}
			if (!description.ContentType.Equals(Constants.OCS.SDPContentType))
			{
				return UserNotificationEventSession.SIPStatus.UnsupportedMediaType;
			}
			Sdp<SdpGlobalDescription, SdpMediaDescription> sdp = new Sdp<SdpGlobalDescription, SdpMediaDescription>();
			this.TraceDebug("ProcessRemoteSdp():\r\n{0}", new object[]
			{
				Encoding.ASCII.GetString(description.GetBody())
			});
			if (!sdp.TryParse(description.GetBody()))
			{
				return UserNotificationEventSession.SIPStatus.NotAcceptableHere;
			}
			if (sdp.MediaDescriptions.Count != 1)
			{
				return UserNotificationEventSession.SIPStatus.NotAcceptableHere;
			}
			SdpMediaDescription sdpMediaDescription = sdp.MediaDescriptions[0];
			if (string.Compare(sdpMediaDescription.MediaName, "application", StringComparison.InvariantCultureIgnoreCase) != 0)
			{
				return UserNotificationEventSession.SIPStatus.NotAcceptableHere;
			}
			if (!sdpMediaDescription.TransportProtocol.Equals("SIP", StringComparison.InvariantCultureIgnoreCase))
			{
				return UserNotificationEventSession.SIPStatus.NotAcceptableHere;
			}
			foreach (SdpAttribute sdpAttribute in sdpMediaDescription.Attributes)
			{
				if (string.Compare(sdpAttribute.Name, "accept-types", StringComparison.InvariantCultureIgnoreCase) == 0 && sdpAttribute.Value != null && sdpAttribute.Value.IndexOf("application/ms-rtc-usernotification+xml", StringComparison.InvariantCultureIgnoreCase) >= 0)
				{
					return UserNotificationEventSession.SIPStatus.OK;
				}
			}
			return UserNotificationEventSession.SIPStatus.NotAcceptableHere;
		}

		public ContentDescription GetAnswer(object sender, ContentDescription remoteSdp)
		{
			string text = (this.mediaDescription != null) ? Encoding.ASCII.GetString(this.mediaDescription.GetBody()) : null;
			this.TraceDebug("GetMediaAnswer called -> returning:\r\n{0}", new object[]
			{
				text
			});
			return this.mediaDescription;
		}

		public ContentDescription GetOffer(object sender)
		{
			this.TraceDebug("GetMediaOffer called -> returning null", new object[0]);
			return null;
		}

		public void SetAnswer(object sender, ContentDescription descriptionResponse)
		{
			this.TraceDebug("SetMediaAnswer called -> ignore", new object[0]);
		}

		public void HandleOfferInReInvite(object sender, OfferInReInviteEventArgs e)
		{
			this.TraceDebug("HandleOfferInReInvite called -> sending original SDP", new object[0]);
			try
			{
				e.BeginAccept(null, null, null);
			}
			catch (RealTimeException ex)
			{
				this.TraceError("HandleRenegotiationReceived: {0}", new object[]
				{
					ex
				});
			}
			catch (InvalidOperationException ex2)
			{
				this.TraceError("HandleRenegotiationReceived: {0}", new object[]
				{
					ex2
				});
			}
		}

		public void HandleOfferInInviteResponse(object sender, OfferInInviteResponseEventArgs e)
		{
			this.TraceDebug("HandleOfferInInviteResponse called -> ignoring", new object[0]);
		}

		private void TraceDebug(string format, params object[] args)
		{
			this.diagnostics.Trace(format, args);
		}

		private void TraceError(string format, params object[] args)
		{
			this.diagnostics.TraceError(format, args);
		}

		private static PercentageBooleanSlidingCounter recentMissedCallNotificationsProxyFailed = PercentageBooleanSlidingCounter.CreateFailureCounter(1000, TimeSpan.FromHours(1.0));

		private readonly string sessionId = string.Empty;

		private readonly UserNotificationEventHandler handler;

		private readonly DiagnosticHelper diagnostics;

		private SignalingSession session;

		private ExDateTime lastInfoReceivedDate = ExDateTime.UtcNow;

		private ContentDescription mediaDescription;

		internal class SIPStatus
		{
			private SIPStatus(int code, string text, bool success)
			{
				this.Code = code;
				this.Text = text;
				this.Success = success;
			}

			public override string ToString()
			{
				return string.Format(CultureInfo.InvariantCulture, "{0} {1}", new object[]
				{
					this.Code,
					this.Text
				});
			}

			internal static readonly UserNotificationEventSession.SIPStatus OK = new UserNotificationEventSession.SIPStatus(200, "OK", true);

			internal static readonly UserNotificationEventSession.SIPStatus Forbidden = new UserNotificationEventSession.SIPStatus(403, "Forbidden", false);

			internal static readonly UserNotificationEventSession.SIPStatus BadRequest = new UserNotificationEventSession.SIPStatus(400, "Bad Request", false);

			internal static readonly UserNotificationEventSession.SIPStatus InvalidSDPWarning = new UserNotificationEventSession.SIPStatus(305, "Incompatible Media Format", false);

			internal static readonly UserNotificationEventSession.SIPStatus NotAcceptableHere = new UserNotificationEventSession.SIPStatus(488, "Not Acceptable Here", false);

			internal static readonly UserNotificationEventSession.SIPStatus InternalServerError = new UserNotificationEventSession.SIPStatus(500, "Internal Server Error", false);

			internal static readonly UserNotificationEventSession.SIPStatus UnsupportedMediaType = new UserNotificationEventSession.SIPStatus(415, "Unsupported Media Type", false);

			internal readonly int Code;

			internal readonly bool Success;

			internal readonly string Text;
		}
	}
}
