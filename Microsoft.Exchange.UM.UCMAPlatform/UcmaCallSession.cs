using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Audio;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.UM.TroubleshootingTool.Shared;
using Microsoft.Exchange.UM.UMCommon;
using Microsoft.Exchange.UM.UMCore;
using Microsoft.Rtc.Collaboration;
using Microsoft.Rtc.Collaboration.AudioVideo;
using Microsoft.Rtc.Signaling;
using Microsoft.Speech.AudioFormat;
using Microsoft.Speech.Recognition;
using Microsoft.Speech.Synthesis;

namespace Microsoft.Exchange.UM.UcmaPlatform
{
	internal class UcmaCallSession : BaseUMCallSession, IUMSpeechRecognizer, ISerializationGuard
	{
		public UcmaCallSession(ISessionSerializer serializer, ApplicationEndpoint localEndpoint, CallContext cc) : this(serializer, localEndpoint, null, cc, null)
		{
		}

		public UcmaCallSession(ISessionSerializer serializer, ApplicationEndpoint localEndpoint, UcmaCallInfo callInfo, CallContext cc, AudioVideoCall call)
		{
			this.Diag = new DiagnosticHelper(this, ExTraceGlobals.UCMATracer);
			this.CallInfo = callInfo;
			this.ConsecutiveErrors = new List<Exception>();
			this.Serializer = serializer;
			base.CurrentCallContext = cc;
			this.loggingManager = new UcmaLoggingManager(this.Diag);
			this.LocalEndpoint = localEndpoint;
			this.ToneAccumulator = new ToneAccumulator();
			this.TestInfo = new UcmaCallSession.TestInfoManager(this);
			this.CurrentState = new UcmaCallSession.IdleSessionState(this);
			this.CurrentState.Start();
			base.RemoteCertificate = ((callInfo != null) ? callInfo.RemoteCertificate : null);
			base.RemoteMatchedFQDN = cc.RemoteFQDN;
			this.CommandAndControlLoggingCounter = 0;
			this.CommandAndControlLoggingEnabled = UcmaAudioLogging.CmdAndControlAudioLoggingEnabled;
			if (call != null)
			{
				this.AssociateCall(call);
			}
			if (this.Subscriber == null)
			{
				this.Subscriber = new UcmaCallSession.SubscriptionHelper(this);
			}
		}

		public event UMCallSessionHandler<UMSpeechEventArgs> OnSpeech;

		internal override event UMCallSessionHandler<EventArgs> OnCallConnected;

		internal override event UMCallSessionHandler<UMCallSessionEventArgs> OnCancelled;

		internal override event UMCallSessionHandler<UMCallSessionEventArgs> OnComplete;

		internal override event UMCallSessionHandler<EventArgs> OnDispose;

		internal override event UMCallSessionHandler<UMCallSessionEventArgs> OnDtmf;

		internal override event UMCallSessionHandler<UMCallSessionEventArgs> OnError;

		internal override event UMCallSessionHandler<UMCallSessionEventArgs> OnFaxRequestReceive;

		internal override event UMCallSessionHandler<UMCallSessionEventArgs> OnHangup;

		internal override event UMCallSessionHandler<HeavyBlockingOperationEventArgs> OnHeavyBlockingOperation;

		internal override event UMCallSessionHandler<InfoMessage.MessageReceivedEventArgs> OnMessageReceived;

		internal override event UMCallSessionHandler<EventArgs> OnMessageSent;

		internal override event UMCallSessionHandler<OutboundCallDetailsEventArgs> OnOutboundCallRequestCompleted;

		internal override event UMCallSessionHandler<UMCallSessionEventArgs> OnStateInfoSent;

		internal override event UMCallSessionHandler<UMCallSessionEventArgs> OnTimeout;

		internal override event UMCallSessionHandler<UMCallSessionEventArgs> OnTransferComplete;

		internal override event UMCallSessionHandler<UMCallSessionEventArgs> OnHold;

		internal override event UMCallSessionHandler<UMCallSessionEventArgs> OnResume;

		public MediaEncryption MediaEncryptionPolicy
		{
			get
			{
				MediaEncryption mediaEncryption;
				if (base.CurrentCallContext.IsLocalDiagnosticCall)
				{
					mediaEncryption = 2;
				}
				else if (base.CurrentCallContext.DialPlan.VoIPSecurity == UMVoIPSecurityType.Secured)
				{
					mediaEncryption = 3;
				}
				else
				{
					mediaEncryption = 1;
				}
				this.Diag.Trace("get_MediaEncryptionPolicy '{0}'", new object[]
				{
					mediaEncryption
				});
				return mediaEncryption;
			}
		}

		public bool StopSerializedEvents { get; private set; }

		public object SerializationLocker
		{
			get
			{
				return this;
			}
		}

		public ApplicationEndpoint LocalEndpoint { get; private set; }

		public ISessionSerializer Serializer { get; private set; }

		public int CommandAndControlLoggingCounter { get; set; }

		public bool CommandAndControlLoggingEnabled { get; private set; }

		internal override IUMSpeechRecognizer SpeechRecognizer
		{
			get
			{
				return this;
			}
		}

		internal override bool IsClosing
		{
			get
			{
				return this.IsSignalingTerminatingOrTerminated;
			}
		}

		internal override UMCallState State
		{
			get
			{
				UMCallState result = UMCallState.Disconnected;
				switch (this.SnapshotCallState())
				{
				case 0:
					result = UMCallState.Idle;
					break;
				case 1:
					result = UMCallState.Incoming;
					break;
				case 2:
					result = UMCallState.Connecting;
					break;
				case 3:
					result = UMCallState.Connected;
					break;
				case 5:
					result = UMCallState.Transferring;
					break;
				case 7:
					result = UMCallState.Disconnected;
					break;
				case 8:
					result = UMCallState.Disconnected;
					break;
				}
				return result;
			}
		}

		internal override UMLoggingManager LoggingManager
		{
			get
			{
				return this.loggingManager;
			}
		}

		internal override string CallLegId
		{
			get
			{
				return this.Ucma.Call.CallId;
			}
		}

		internal bool IsInvalidOperationExplainable
		{
			get
			{
				return this.IsSignalingTerminatingOrTerminated || this.IsMediaTerminatingOrTerminated;
			}
		}

		protected override bool IsDependentSession
		{
			get
			{
				return false;
			}
		}

		private UcmaCallSession.UcmaObjects Ucma { get; set; }

		private UcmaCallSession.SubscriptionHelper Subscriber { get; set; }

		private UcmaCallSession.TestInfoManager TestInfo { get; set; }

		private DiagnosticHelper Diag { get; set; }

		private UcmaCallInfo CallInfo { get; set; }

		private List<Exception> ConsecutiveErrors { get; set; }

		private UcmaCallSession.SessionState CurrentState { get; set; }

		private ToneAccumulator ToneAccumulator { get; set; }

		private CallStateTransitionReason CallStateTransitionReason { get; set; }

		private CallState LastCallStateChangeProcessed { get; set; }

		private bool IsSignalingTerminatingOrTerminated
		{
			get
			{
				CallState callState = this.SnapshotCallState();
				return callState == 7 || callState == 8;
			}
		}

		private bool IsMediaTerminatingOrTerminated
		{
			get
			{
				MediaFlowState mediaFlowState = this.SnapshotFlowState();
				return mediaFlowState == 2;
			}
		}

		public static string FormatStateChange<T>(string reference, T previous, T next)
		{
			return string.Format("{0}_StateChanged from {1} to {2}", reference, previous, next);
		}

		public static byte ToneToByte(DtmfTone tone)
		{
			byte result = 0;
			switch (tone)
			{
			case 0:
				result = 48;
				break;
			case 1:
				result = 49;
				break;
			case 2:
				result = 50;
				break;
			case 3:
				result = 51;
				break;
			case 4:
				result = 52;
				break;
			case 5:
				result = 53;
				break;
			case 6:
				result = 54;
				break;
			case 7:
				result = 55;
				break;
			case 8:
				result = 56;
				break;
			case 9:
				result = 57;
				break;
			case 10:
				result = 42;
				break;
			case 11:
				result = 35;
				break;
			case 12:
				result = 65;
				break;
			case 13:
				result = 66;
				break;
			case 14:
				result = 67;
				break;
			case 15:
				result = 68;
				break;
			}
			return result;
		}

		public static ToneId CharToToneId(char c)
		{
			ToneId result = 0;
			switch (c)
			{
			case '#':
				result = 11;
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
				result = 10;
				break;
			case '0':
				result = 0;
				break;
			case '1':
				result = 1;
				break;
			case '2':
				result = 2;
				break;
			case '3':
				result = 3;
				break;
			case '4':
				result = 4;
				break;
			case '5':
				result = 5;
				break;
			case '6':
				result = 6;
				break;
			case '7':
				result = 7;
				break;
			case '8':
				result = 8;
				break;
			case '9':
				result = 9;
				break;
			default:
				switch (c)
				{
				case 'A':
					result = 12;
					break;
				case 'B':
					result = 13;
					break;
				case 'C':
					result = 14;
					break;
				case 'D':
					result = 15;
					break;
				}
				break;
			}
			return result;
		}

		public override void RebufferDigits(byte[] dtmfDigits)
		{
			this.ToneAccumulator.RebufferDigits(dtmfDigits);
		}

		public void AssociateCall(AudioVideoCall call)
		{
			this.TeardownUcmaObjects();
			this.TeardownSubscriber();
			base.CallId = call.CallId;
			this.Ucma = new UcmaCallSession.UcmaObjects(this, call);
			this.Subscriber = new UcmaCallSession.SubscriptionHelper(this);
			this.Subscriber.SubscribeTo(call);
		}

		public void PlayPrompts(ArrayList prompts, int minDigits, int maxDigits, int timeout, string stopTones, int interDigitTimeout, StopPatterns stopPatterns, int startIdx, TimeSpan offset, List<UMGrammar> grammars, bool expetingSpeechInput, int babbleTimeout, bool stopPromptOnBargeIn, string turnName, int initialSilenceTimeout)
		{
			UcmaCallSession.PlaybackInfo promptInfo = new UcmaCallSession.PlaybackInfo
			{
				Prompts = prompts,
				MinDigits = minDigits,
				MaxDigits = maxDigits,
				Timeout = TimeSpan.FromSeconds((double)timeout),
				StopTones = stopTones,
				InterDigitTimeout = TimeSpan.FromSeconds((double)interDigitTimeout),
				StopPatterns = stopPatterns,
				Offset = offset,
				Grammars = grammars,
				BabbleTimeout = TimeSpan.FromSeconds((double)babbleTimeout),
				UnconditionalBargeIn = stopPromptOnBargeIn,
				TurnName = this.activityState,
				InitialSilenceTimeout = TimeSpan.FromSeconds((double)initialSilenceTimeout)
			};
			if (this.TestInfo.IsFeatureEnabled(UcmaCallSession.TestInfoFeatures.PlayAudio) || this.TestInfo.IsFeatureEnabled(UcmaCallSession.TestInfoFeatures.RecoEmulate))
			{
				this.ChangeState(new UcmaCallSession.PlayPromptsAndRecoEmulateSessionState(this, promptInfo));
				return;
			}
			if (this.CommandAndControlLoggingEnabled)
			{
				this.ChangeState(new UcmaCallSession.CommandAndControlLoggingSessionState(this, promptInfo));
				return;
			}
			this.ChangeState(new UcmaCallSession.PlayPromptsAndRecoSpeechSessionState(this, promptInfo));
		}

		internal override void PlayPrompts(ArrayList prompts, int minDigits, int maxDigits, int timeout, string stopTones, int interDigitTimeout, StopPatterns stopPatterns, int startIdx, TimeSpan offset, bool stopPromptOnBargeIn, string turnName, int initialSilenceTimeout)
		{
			UcmaCallSession.PlaybackInfo promptInfo = new UcmaCallSession.PlaybackInfo
			{
				Prompts = prompts,
				MinDigits = minDigits,
				MaxDigits = maxDigits,
				Timeout = TimeSpan.FromSeconds((double)timeout),
				InterDigitTimeout = TimeSpan.FromSeconds((double)interDigitTimeout),
				StopTones = stopTones,
				StopPatterns = stopPatterns,
				UnconditionalBargeIn = stopPromptOnBargeIn,
				Offset = offset,
				Uninterruptable = false,
				TurnName = this.activityState,
				InitialSilenceTimeout = TimeSpan.FromSeconds((double)initialSilenceTimeout)
			};
			if (maxDigits == 0)
			{
				this.ChangeState(new UcmaCallSession.PlayPromptsSessionState(this, promptInfo));
				return;
			}
			this.ChangeState(new UcmaCallSession.PlayPromptsAndRecoDtmfSessionState(this, promptInfo));
		}

		internal override void PlayUninterruptiblePrompts(ArrayList prompts)
		{
			UcmaCallSession.PlaybackInfo promptInfo = new UcmaCallSession.PlaybackInfo
			{
				UnconditionalBargeIn = true,
				Prompts = prompts,
				Uninterruptable = true,
				TurnName = this.activityState
			};
			this.ChangeState(new UcmaCallSession.PlayPromptsSessionState(this, promptInfo));
		}

		internal override BaseUMAsyncTimer StartTimer(BaseUMAsyncTimer.UMTimerCallback callback, int dueTime)
		{
			SerializableCallback<BaseUMCallSession> serializeableCallbackWithErrorHandling = delegate(BaseUMCallSession o)
			{
				this.CatchAndFireOnError(delegate
				{
					callback(this);
				});
			};
			BaseUMAsyncTimer baseUMAsyncTimer = new UcmaAsyncTimer(this, delegate(BaseUMCallSession o)
			{
				this.Serializer.SerializeCallback<BaseUMCallSession>(this, serializeableCallbackWithErrorHandling, this, false, "StartTimer");
			}, dueTime);
			base.Timers.Add(baseUMAsyncTimer);
			return baseUMAsyncTimer;
		}

		internal override void RecordFile(string fileName, int maxSilence, int maxSeconds, string stopTones, bool append, ArrayList comfortPrompts)
		{
			UcmaCallSession.RecordInfo r = new UcmaCallSession.RecordInfo
			{
				FileName = fileName,
				EndSilenceTimeout = TimeSpan.FromSeconds((double)maxSilence),
				MaxDuration = TimeSpan.FromSeconds((double)maxSeconds),
				StopTones = stopTones,
				Append = append
			};
			this.ChangeState(new UcmaCallSession.RecordFileSessionState(this, r));
		}

		internal override void CancelPendingOperations()
		{
			this.CurrentState.Cancel(true);
		}

		internal override void DisconnectCall(PlatformSignalingHeader diagnosticHeader)
		{
			this.Diag.Trace("Teardown due to DisconnectCall", new object[0]);
			this.ChangeState(new UcmaCallSession.TeardownSessionState(this, UcmaCallSession.DisconnectType.Local, diagnosticHeader));
		}

		internal override void CloseSession()
		{
			UcmaCallSession.EstablishCallSessionState establishCallSessionState = this.CurrentState as UcmaCallSession.EstablishCallSessionState;
			if (establishCallSessionState != null)
			{
				establishCallSessionState.CancelOutboundCall();
				return;
			}
			this.Diag.Trace("Teardown due to CloseSession", new object[0]);
			this.ChangeState(new UcmaCallSession.TeardownSessionState(this, UcmaCallSession.DisconnectType.Remote, null));
		}

		internal override void RunHeavyBlockingOperation(IUMHeavyBlockingOperation operation, ArrayList prompts)
		{
			this.ChangeState(new UcmaCallSession.HeavyBlockingOperationSessionState(this, operation, prompts));
		}

		internal override void SendMessage(InfoMessage m)
		{
			lock (this)
			{
				this.Diag.Trace("Sending info message within state {0}", new object[]
				{
					this.CurrentState.Name
				});
				this.CurrentState.SendMessage(m);
			}
		}

		internal override void SendDtmf(string dtmfSequence, TimeSpan initialSilence)
		{
			this.ChangeState(new UcmaCallSession.SendDtmfSessionState(this, dtmfSequence, initialSilence));
		}

		internal override void TransferAsync()
		{
			this.ChangeState(new UcmaCallSession.SupervisedTransferSessionState(this, null, null));
		}

		internal override void TransferAsync(PlatformSipUri target)
		{
			this.TransferAsync(target, null);
		}

		internal override void TransferAsync(PlatformSipUri target, IList<PlatformSignalingHeader> headers)
		{
			bool flag = false;
			if (base.DependentSessionDetails != null)
			{
				UcmaCallSession ucmaCallSession = base.DependentSessionDetails.DependentUMCallSession as UcmaCallSession;
				if (ucmaCallSession.Ucma != null)
				{
					flag = (null != ucmaCallSession.Ucma.Call);
				}
			}
			this.Diag.Trace("TransferAsync: supervised={0}", new object[]
			{
				flag
			});
			if (flag)
			{
				this.ChangeState(new UcmaCallSession.SupervisedTransferSessionState(this, target, headers));
				return;
			}
			this.ChangeState(new UcmaCallSession.BlindTransferSessionState(this, target, headers));
		}

		internal override void TransferAsync(string phoneNumber)
		{
			this.ChangeState(new UcmaCallSession.BlindTransferSessionState(this, phoneNumber));
		}

		internal override void Redirect(string host, int port, int code)
		{
			this.ChangeState(new UcmaCallSession.RedirectSessionState(this, host, port, code));
		}

		internal override void SendStateInfo(string callId, string state)
		{
			this.activityState = state;
			this.ChangeState(new UcmaCallSession.SendFsmInfoSessionState(this, callId, state));
		}

		internal override void ClearDigits(int sleepMsec)
		{
			this.ToneAccumulator.Clear();
		}

		internal override void AcceptFax()
		{
			this.ChangeState(new UcmaCallSession.AcceptFaxSessionState(this));
		}

		internal override bool IsDuringPlayback()
		{
			bool result = false;
			UcmaCallSession.PlayPromptsSessionState playPromptsSessionState = this.CurrentState as UcmaCallSession.PlayPromptsSessionState;
			if (playPromptsSessionState != null)
			{
				result = playPromptsSessionState.IsSpeaking;
			}
			return result;
		}

		internal override void StopPlayback()
		{
			UcmaCallSession.PlayPromptsSessionState playPromptsSessionState = this.CurrentState as UcmaCallSession.PlayPromptsSessionState;
			this.Diag.Assert(null != playPromptsSessionState);
			playPromptsSessionState.HandleApplicationRequestToStopPlayback();
		}

		internal override void StopPlaybackAndCancelRecognition()
		{
			UcmaCallSession.PlayPromptsSessionState playPromptsSessionState = this.CurrentState as UcmaCallSession.PlayPromptsSessionState;
			if (playPromptsSessionState != null && !playPromptsSessionState.IsIdle)
			{
				playPromptsSessionState.Cancel(false);
			}
		}

		internal override void Skip(TimeSpan timeToSkip)
		{
			UcmaCallSession.PlayPromptsSessionState playPromptsSessionState = this.CurrentState as UcmaCallSession.PlayPromptsSessionState;
			if (playPromptsSessionState != null && !playPromptsSessionState.IsIdle)
			{
				playPromptsSessionState.Skip(timeToSkip);
			}
		}

		internal override void InitializeConnectedCall(OutboundCallDetailsEventArgs args)
		{
			this.Diag.Assert(this.CallInfo != null, "CallInfo has not be set, but InitializeConnectedCall is invoked");
			this.Diag.Assert(base.CurrentCallContext != null, "CallContext has not be set, but InitializeConnectedCall is invoked");
			base.CurrentCallContext.ConnectionTime = new ExDateTime?(ExDateTime.UtcNow);
			UcmaVoipPlatform.InitializeOutboundCallContext(this.CallInfo, base.CurrentCallContext);
			this.Fire<EventArgs>(this.OnCallConnected, null);
		}

		protected virtual void Teardown()
		{
			this.Dispose();
		}

		protected override void InternalAcceptCall()
		{
			this.ChangeState(new UcmaCallSession.AcceptCallSessionState(this));
		}

		protected override void SetContactUriForAccept()
		{
		}

		protected override void DeclineCall(PlatformSignalingHeader diagnosticHeader)
		{
			this.ChangeState(new UcmaCallSession.DeclineSessionState(this, diagnosticHeader));
		}

		protected override BaseUMCallSession InternalCreateDependentSession(DependentSessionDetails details, CallContext context)
		{
			return new UcmaDependentCallSession(details, this.Serializer, this.LocalEndpoint, context);
		}

		protected override void InternalOpenAsync(BaseUMCallSession.OutboundCallInfo info, IList<PlatformSignalingHeader> headers)
		{
			this.ChangeState(new UcmaCallSession.EstablishCallSessionState(this, info, headers));
		}

		protected override void InternalDispose(bool disposing)
		{
			try
			{
				if (disposing)
				{
					this.StopSerializedEvents = true;
					this.DisposeLoggingManager();
				}
			}
			finally
			{
				base.InternalDispose(disposing);
			}
		}

		protected override void TeardownCurrentCall()
		{
			this.Diag.Trace("TeardownCurrentCall", new object[0]);
			try
			{
				this.Fire<EventArgs>(this.OnDispose, null);
				this.TeardownSubscriber();
				this.TeardownUcmaObjects();
				this.TeardownEvents();
				this.TeardownCallState();
			}
			finally
			{
				base.TeardownCurrentCall();
			}
		}

		private static bool IsGrayException(Exception e)
		{
			bool result = false;
			if (e is WatsoningDueToRecycling)
			{
				result = true;
			}
			return result;
		}

		private void ChangeState(UcmaCallSession.SessionState nextState)
		{
			this.Diag.Assert(this.CurrentState == null || this.CurrentState.IsIdle || nextState is UcmaCallSession.TeardownSessionState);
			lock (this)
			{
				if (this.IgnoreStateChange(nextState))
				{
					this.Diag.Trace("Disposing nextState '{0}' because it's being ignored", new object[]
					{
						nextState.Name
					});
					nextState.Dispose();
				}
				else
				{
					this.Diag.Trace("Changing session state from '{0}' to '{1}'", new object[]
					{
						this.CurrentState.Name,
						nextState.Name
					});
					if (this.CurrentState != null)
					{
						this.CurrentState.Dispose();
					}
					this.CurrentState = nextState;
					this.CurrentState.Start();
					this.TeardownIdleSession();
				}
			}
		}

		private void TeardownIdleSession()
		{
			if (!base.IsDisposed && this.CurrentState != null && !this.CurrentState.IsDisposed && this.CurrentState.IsIdle)
			{
				this.Diag.Assert(this.IsSignalingTerminatingOrTerminated, "The call session is idle while the call is active!");
				this.Diag.Trace("Tearing down an idle call session.", new object[0]);
				this.ChangeState(new UcmaCallSession.TeardownSessionState(this, UcmaCallSession.DisconnectType.Local, null));
			}
		}

		private bool IgnoreStateChange(UcmaCallSession.SessionState nextState)
		{
			bool flag = this.CurrentState is UcmaCallSession.IdleSessionState && nextState is UcmaCallSession.IdleSessionState;
			bool flag2 = this.CurrentState is UcmaCallSession.TeardownSessionState;
			return flag || flag2;
		}

		private CallState SnapshotCallState()
		{
			CallState result = 0;
			if (this.Ucma != null && this.Ucma.Call != null)
			{
				result = this.Ucma.Call.State;
			}
			return result;
		}

		private MediaFlowState SnapshotFlowState()
		{
			MediaFlowState result = 0;
			if (this.Ucma != null)
			{
				if (this.Ucma.Flow != null)
				{
					result = this.Ucma.Flow.State;
				}
				if (this.Ucma.MediaDropped)
				{
					this.Diag.Trace("Assuming media flow is terminated because a connector went inactive", new object[0]);
					result = 2;
				}
			}
			return result;
		}

		private void Fire<TArgs>(UMCallSessionHandler<TArgs> handler, TArgs args)
		{
			if (handler != null)
			{
				this.Diag.Trace("Firing {0}", new object[]
				{
					handler.Method.Name
				});
				if (handler.Equals(this.OnHangup))
				{
					this.OnHangup = null;
				}
				else if (handler.Equals(this.OnDispose))
				{
					this.OnDispose = null;
				}
				handler(this, args);
			}
		}

		private void CatchAndFireOnError(GrayException.UserCodeDelegate function)
		{
			Exception error = null;
			try
			{
				ExceptionHandling.MapAndReportGrayExceptions(delegate()
				{
					try
					{
						function();
					}
					catch (StorageTransientException error2)
					{
						error = error2;
					}
					catch (StoragePermanentException error3)
					{
						error = error3;
					}
					catch (ADTransientException ex)
					{
						this.CurrentCallContext.TrackDirectoryAccessFailures(ex);
						error = ex;
					}
					catch (DataValidationException ex2)
					{
						this.CurrentCallContext.TrackDirectoryAccessFailures(ex2);
						error = ex2;
					}
					catch (InvalidOperationException ex3)
					{
						bool isInvalidOperationExplainable = this.IsInvalidOperationExplainable;
						this.Diag.Trace("Caught IOP='{0}'.  IsInvalidOperationExplainable='{1}'", new object[]
						{
							ex3,
							isInvalidOperationExplainable
						});
						if (!isInvalidOperationExplainable)
						{
							throw;
						}
						error = (this.CurrentState.ConditionalWaitForTerminatedOrFax() ? null : ex3);
					}
					catch (RealTimeException error4)
					{
						error = error4;
					}
					catch (LocalizedException error5)
					{
						error = error5;
					}
				}, new GrayException.IsGrayExceptionDelegate(UcmaCallSession.IsGrayException));
			}
			catch (UMGrayException error)
			{
				UMGrayException error6;
				error = error6;
			}
			catch (Exception e)
			{
				ExceptionHandling.SendWatsonWithExtraData(e, true);
				Utils.KillThisProcess();
			}
			if (error != null)
			{
				this.CatchAndFireOnError(delegate
				{
					this.LogException(error);
					this.CurrentState.FireError(error);
				});
			}
		}

		private void LogException(Exception e)
		{
			this.Diag.Trace("LogException: {0}", new object[]
			{
				e
			});
			if (this.ShouldLog(e))
			{
				string text = CommonUtil.ToEventLogString(e);
				UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_PlatformException, null, new object[]
				{
					text,
					base.CallId
				});
				this.LogTlsException(e);
			}
		}

		private void LogTlsException(Exception e)
		{
			TlsFailureException ex = e as TlsFailureException;
			int num = 100;
			while (e != null && ex == null && --num > 0)
			{
				ex = (e as TlsFailureException);
				e = e.InnerException;
			}
			this.Diag.Assert(num > 0);
			if (ex != null)
			{
				this.Diag.Trace("LogTlsError: {0}", new object[]
				{
					ex
				});
				string text = CommonUtil.ToEventLogString(UcmaUtils.GetTlsError(ex));
				UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_PlatformTlsException, null, new object[]
				{
					base.CallId,
					text
				});
			}
		}

		private bool ShouldLog(Exception e)
		{
			bool flag = true;
			if ((e is InvalidOperationException || e is OperationFailureException) && this.IsInvalidOperationExplainable)
			{
				flag = false;
			}
			if (!flag)
			{
				this.Diag.Trace("Not logging {0}", new object[]
				{
					e
				});
			}
			return flag;
		}

		private void TeardownUcmaObjects()
		{
			this.Diag.Trace("TeardownUcmaObjects", new object[0]);
			if (this.Ucma != null)
			{
				this.Ucma.Dispose();
				this.Ucma = null;
			}
		}

		private void TeardownSubscriber()
		{
			this.Diag.Trace("TeardownSubscriber", new object[0]);
			if (this.Subscriber != null)
			{
				this.Subscriber.Dispose();
				this.Subscriber = null;
			}
		}

		private void TeardownEvents()
		{
			this.Diag.Trace("TeardownEvents", new object[0]);
			this.OnCallConnected = null;
			this.OnCancelled = null;
			this.OnComplete = null;
			this.OnDispose = null;
			this.OnDtmf = null;
			this.OnError = null;
			this.OnFaxRequestReceive = null;
			this.OnHangup = null;
			this.OnHeavyBlockingOperation = null;
			this.OnMessageReceived = null;
			this.OnMessageSent = null;
			this.OnOutboundCallRequestCompleted = null;
			this.OnSpeech = null;
			this.OnStateInfoSent = null;
			this.OnTimeout = null;
			this.OnTransferComplete = null;
			this.OnHold = null;
			this.OnResume = null;
		}

		private void TeardownCallState()
		{
			this.Diag.Trace("TeardownCallState", new object[0]);
			if (this.CurrentState != null)
			{
				this.CurrentState.Dispose();
			}
			this.CallInfo = null;
			this.ConsecutiveErrors.Clear();
			this.ToneAccumulator.Clear();
		}

		private void DisposeLoggingManager()
		{
			this.Diag.Trace("DisposeLoggingManager", new object[0]);
			if (this.loggingManager != null)
			{
				this.loggingManager.Dispose();
			}
		}

		private const int MaxConsecutiveErrors = 5;

		public static readonly SpeechAudioFormatInfo SpeechAudioFormatInfo = new SpeechAudioFormatInfo(8000, 16, 1);

		private UMLoggingManager loggingManager;

		private string activityState;

		internal enum TestInfoHandling
		{
			HandleNow,
			StateNotMatch,
			FeatureDisabled
		}

		internal abstract class TestInfoEvent
		{
			public abstract UcmaCallSession.TestInfoHandling CanHandle(UcmaCallSession session);

			public abstract void ProcessEvent(UcmaCallSession session);
		}

		internal abstract class BaseTestInfoEvent : UcmaCallSession.TestInfoEvent
		{
			public override UcmaCallSession.TestInfoHandling CanHandle(UcmaCallSession session)
			{
				if (!this.IsRequiredFeatureEnabled(session))
				{
					session.Diag.Trace("TestInfo feature {0} is not enabled", new object[]
					{
						this.requiredFeatures
					});
					return UcmaCallSession.TestInfoHandling.FeatureDisabled;
				}
				bool flag = false;
				if (this.requiredStateType.Length == 0)
				{
					flag = true;
				}
				foreach (Type left in this.requiredStateType)
				{
					if (left == session.CurrentState.GetType())
					{
						flag = true;
					}
				}
				if (flag)
				{
					return UcmaCallSession.TestInfoHandling.HandleNow;
				}
				session.Diag.Trace("TestInfo state {0} is not in correct state", new object[]
				{
					session.CurrentState
				});
				return UcmaCallSession.TestInfoHandling.StateNotMatch;
			}

			protected T ForceCreate<T>(params object[] args)
			{
				object obj = typeof(T).InvokeMember(".ctor", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.CreateInstance, null, null, args);
				return (T)((object)obj);
			}

			protected bool IsRequiredFeatureEnabled(UcmaCallSession session)
			{
				return session.TestInfo.IsFeatureEnabled(this.requiredFeatures);
			}

			protected Type[] requiredStateType = new Type[0];

			protected UcmaCallSession.TestInfoFeatures requiredFeatures;
		}

		internal static class TestInfoEventFactory
		{
			public static UcmaCallSession.TestInfoEvent CreateFromSipInfo(InfoMessage message)
			{
				if (message == null || message.Body == null)
				{
					return null;
				}
				string @string = Encoding.UTF8.GetString(message.Body);
				if (string.IsNullOrEmpty(@string))
				{
					return null;
				}
				string[] array = @string.Split("\r\n".ToCharArray(), 2);
				if (array.Length != 2 || array[0] != "TEST-INFO")
				{
					return null;
				}
				string[] array2 = array[1].Split("\r\n".ToCharArray(), 2);
				if (array2.Length != 2)
				{
					return null;
				}
				string text = array2[0];
				string text2 = array2[1];
				string a;
				if ((a = text) != null)
				{
					if (a == "Initialize")
					{
						return new UcmaCallSession.TestInfoEventFactory.InitializeTestInfoEvent(text2);
					}
					if (a == "PlayDtmf")
					{
						return new UcmaCallSession.TestInfoEventFactory.PlayDtmfTestInfoEvent(text2);
					}
					if (a == "PlayAudio")
					{
						return new UcmaCallSession.TestInfoEventFactory.PlayAudioTestInfoEvent(text2);
					}
					if (a == "RecoEmulate")
					{
						return new UcmaCallSession.TestInfoEventFactory.RecoEmulateTestInfoEvent(text2);
					}
				}
				return null;
			}

			private class InitializeTestInfoEvent : UcmaCallSession.BaseTestInfoEvent
			{
				public InitializeTestInfoEvent(string features)
				{
					this.requiredFeatures = UcmaCallSession.TestInfoFeatures.None;
					this.requiredStateType = new Type[0];
					this.featureList = features.Split(new char[]
					{
						','
					});
				}

				public override void ProcessEvent(UcmaCallSession session)
				{
					string[] array = this.featureList;
					int i = 0;
					while (i < array.Length)
					{
						string text = array[i];
						string a;
						if ((a = text.Trim()) == null)
						{
							goto IL_8A;
						}
						if (!(a == "PlayDtmf"))
						{
							if (!(a == "PlayAudio"))
							{
								if (!(a == "RecoEmulate"))
								{
									if (!(a == "SkipPrompt"))
									{
										goto IL_8A;
									}
									session.TestInfo.EnableFeature(UcmaCallSession.TestInfoFeatures.SkipPrompt);
								}
								else
								{
									session.TestInfo.EnableFeature(UcmaCallSession.TestInfoFeatures.RecoEmulate);
								}
							}
							else
							{
								session.TestInfo.EnableFeature(UcmaCallSession.TestInfoFeatures.PlayAudio);
							}
						}
						else
						{
							session.TestInfo.EnableFeature(UcmaCallSession.TestInfoFeatures.PlayDtmf);
						}
						IL_A9:
						i++;
						continue;
						IL_8A:
						session.Diag.Trace("Unknown Test Info Feature requested {0}", new object[]
						{
							text
						});
						goto IL_A9;
					}
				}

				private string[] featureList;
			}

			private class PlayDtmfTestInfoEvent : UcmaCallSession.BaseTestInfoEvent
			{
				public PlayDtmfTestInfoEvent(string dtmf)
				{
					this.requiredFeatures = UcmaCallSession.TestInfoFeatures.PlayDtmf;
					this.requiredStateType = new Type[]
					{
						typeof(UcmaCallSession.PlayPromptsAndRecoEmulateSessionState),
						typeof(UcmaCallSession.PlayPromptsAndRecoDtmfSessionState),
						typeof(UcmaCallSession.RecordFileSessionState)
					};
					this.dtmf = dtmf;
				}

				public override void ProcessEvent(UcmaCallSession session)
				{
					foreach (char c in this.dtmf)
					{
						ToneControllerEventArgs e = base.ForceCreate<ToneControllerEventArgs>(new object[]
						{
							UcmaCallSession.CharToToneId(c),
							100f
						});
						session.CurrentState.ToneController_ToneReceived(this, e);
					}
				}

				private readonly string dtmf;
			}

			private class PlayAudioTestInfoEvent : UcmaCallSession.BaseTestInfoEvent
			{
				public PlayAudioTestInfoEvent(string wave)
				{
					this.requiredFeatures = UcmaCallSession.TestInfoFeatures.PlayAudio;
					this.requiredStateType = new Type[]
					{
						typeof(UcmaCallSession.PlayPromptsAndRecoEmulateSessionState),
						typeof(UcmaCallSession.RecordFileSessionState)
					};
					this.wave = wave;
				}

				public override void ProcessEvent(UcmaCallSession session)
				{
					if (session.CurrentState is UcmaCallSession.PlayPromptsAndRecoEmulateSessionState)
					{
						UcmaCallSession.PlayPromptsAndRecoEmulateSessionState playPromptsAndRecoEmulateSessionState = session.CurrentState as UcmaCallSession.PlayPromptsAndRecoEmulateSessionState;
						playPromptsAndRecoEmulateSessionState.RecognizeWave(this.wave);
						return;
					}
					if (session.CurrentState is UcmaCallSession.RecordFileSessionState)
					{
						UcmaCallSession.RecordFileSessionState recordFileSessionState = session.CurrentState as UcmaCallSession.RecordFileSessionState;
						recordFileSessionState.TestInfoRecordedFileName = this.wave;
						return;
					}
					session.Diag.Assert(false, "Unexpected state {0} in {1}", new object[]
					{
						session.CurrentState.Name,
						"PlayAudioTestInfoEvent.ProcessEvent"
					});
				}

				private readonly string wave;
			}

			private class RecoEmulateTestInfoEvent : UcmaCallSession.BaseTestInfoEvent
			{
				public RecoEmulateTestInfoEvent(string text)
				{
					this.requiredFeatures = UcmaCallSession.TestInfoFeatures.RecoEmulate;
					this.requiredStateType = new Type[]
					{
						typeof(UcmaCallSession.PlayPromptsAndRecoEmulateSessionState)
					};
					this.text = text;
				}

				public override void ProcessEvent(UcmaCallSession session)
				{
					if (session.CurrentState is UcmaCallSession.PlayPromptsAndRecoEmulateSessionState)
					{
						UcmaCallSession.PlayPromptsAndRecoEmulateSessionState playPromptsAndRecoEmulateSessionState = session.CurrentState as UcmaCallSession.PlayPromptsAndRecoEmulateSessionState;
						playPromptsAndRecoEmulateSessionState.EmulateWave(this.text);
						return;
					}
					session.Diag.Assert(false, "Unexpected state {0} in {1}", new object[]
					{
						session.CurrentState.Name,
						"RecoSimTestInfoEvent.ProcessEvent"
					});
				}

				private readonly string text;
			}
		}

		[Flags]
		internal enum TestInfoFeatures
		{
			None = 0,
			PlayDtmf = 1,
			PlayAudio = 2,
			RecoEmulate = 4,
			SkipPrompt = 8,
			All = 1023
		}

		internal class TestInfoManager
		{
			public bool TestInfoEnabled
			{
				get
				{
					return Utils.RunningInTestMode && (this.features & UcmaCallSession.TestInfoFeatures.All) != UcmaCallSession.TestInfoFeatures.None;
				}
			}

			public TestInfoManager(UcmaCallSession session)
			{
				this.session = session;
				this.features = UcmaCallSession.TestInfoFeatures.None;
			}

			public void EnableFeature(UcmaCallSession.TestInfoFeatures feature)
			{
				this.session.Diag.Trace("Feature {0} Enabled", new object[]
				{
					this.features.ToString()
				});
				this.features |= feature;
			}

			public void DisableFeature(UcmaCallSession.TestInfoFeatures feature)
			{
				this.session.Diag.Trace("Feature {0} Disabled", new object[]
				{
					this.features.ToString()
				});
				this.features &= ~feature;
			}

			public bool IsFeatureEnabled(UcmaCallSession.TestInfoFeatures feature)
			{
				return Utils.RunningInTestMode && (this.features & feature) == feature;
			}

			public void QueueEvent(UcmaCallSession.TestInfoEvent testInfoEvent)
			{
				lock (this.lockObject)
				{
					this.testInfoQueue.Enqueue(testInfoEvent);
				}
			}

			public void ProcessQueue()
			{
				lock (this.lockObject)
				{
					while (this.testInfoQueue.Count > 0)
					{
						UcmaCallSession.TestInfoHandling testInfoHandling = this.testInfoQueue.Peek().CanHandle(this.session);
						if (testInfoHandling == UcmaCallSession.TestInfoHandling.HandleNow)
						{
							this.testInfoQueue.Dequeue().ProcessEvent(this.session);
						}
						else if (testInfoHandling == UcmaCallSession.TestInfoHandling.FeatureDisabled)
						{
							this.testInfoQueue.Dequeue();
						}
						else if (testInfoHandling == UcmaCallSession.TestInfoHandling.StateNotMatch)
						{
							break;
						}
					}
				}
			}

			private Queue<UcmaCallSession.TestInfoEvent> testInfoQueue = new Queue<UcmaCallSession.TestInfoEvent>();

			private object lockObject = new object();

			private UcmaCallSession session;

			private UcmaCallSession.TestInfoFeatures features;
		}

		internal class UcmaObjects : DisposableBase
		{
			public UcmaObjects(UcmaCallSession session, AudioVideoCall call)
			{
				ValidateArgument.NotNull(session, "session");
				ValidateArgument.NotNull(call, "call");
				this.session = session;
				this.call = call;
			}

			public AudioVideoCall Call
			{
				get
				{
					return this.call;
				}
			}

			public AudioVideoFlow Flow
			{
				get
				{
					return this.call.Flow;
				}
			}

			public PromptPlayer Player
			{
				get
				{
					return this.player;
				}
			}

			public Recorder MediaRecorder
			{
				get
				{
					return this.mediaRecorder;
				}
			}

			public ToneController ToneController
			{
				get
				{
					return this.toneController;
				}
			}

			public SpeechRecognitionEngine SpeechReco
			{
				get
				{
					return this.speechReco;
				}
			}

			public bool MediaDropped
			{
				get
				{
					bool flag = this.connectorShouldBeActive && !this.speechRecoConnector.IsActive;
					bool flag2 = this.player != null && this.player.MediaDropped;
					bool flag3 = flag || flag2;
					if (flag3)
					{
						this.session.Diag.Trace("Media was dropped!  reco='{0}', player='{1}'", new object[]
						{
							flag,
							flag2
						});
					}
					return flag3;
				}
			}

			public void ActivateFlow()
			{
				this.session.Diag.Assert(!this.flowActivated, "Flow has already been activated!");
				this.flowActivated = true;
				this.ActivatePlayer();
				this.ActivateRecorder();
				this.ActivateToneController();
			}

			public void EnsureRecognition(CultureInfo culture)
			{
				this.EnsureSpeechRecognizer(culture);
				this.EnsureSpeechRecognitionConnector();
				this.EnsureSpeechRecognitionStream();
			}

			public void HaltRecognition()
			{
				this.connectorShouldBeActive = false;
				if (this.speechRecoConnector != null)
				{
					this.speechRecoConnector.Stop();
				}
				if (this.speechReco != null)
				{
					this.speechReco.RecognizeAsyncCancel();
				}
			}

			public void Pause()
			{
				if (this.Player != null)
				{
					this.Player.Pause();
				}
				if (this.MediaRecorder != null && this.MediaRecorder.State == null)
				{
					this.MediaRecorder.Pause();
				}
			}

			public void Resume()
			{
				if (this.Player != null)
				{
					this.Player.Resume();
				}
				if (this.MediaRecorder != null && this.MediaRecorder.State == 2)
				{
					this.MediaRecorder.Start();
				}
			}

			protected override DisposeTracker InternalGetDisposeTracker()
			{
				return DisposeTracker.Get<UcmaCallSession.UcmaObjects>(this);
			}

			protected override void InternalDispose(bool disposing)
			{
				if (disposing)
				{
					if (this.speechRecoConnector != null)
					{
						this.speechRecoConnector.Dispose();
					}
					if (this.speechReco != null)
					{
						this.speechReco.Dispose();
					}
					if (this.speechRecoStream != null)
					{
						this.speechRecoStream.Dispose();
					}
					if (this.player != null)
					{
						this.player.Dispose();
					}
					this.call = null;
					this.player = null;
					this.mediaRecorder = null;
					this.speechReco = null;
					this.speechRecoConnector = null;
					this.speechRecoStream = null;
					this.toneController = null;
				}
			}

			private void ActivatePlayer()
			{
				this.player = new PromptPlayer(this.session);
				this.player.AttachFlow(this.Flow);
				this.session.Subscriber.SubscribeTo(this.player);
			}

			private void ActivateRecorder()
			{
				this.mediaRecorder = new Recorder();
				this.mediaRecorder.AttachFlow(this.Flow);
				this.session.Subscriber.SubscribeTo(this.mediaRecorder);
			}

			private void ActivateToneController()
			{
				this.toneController = new ToneController();
				this.toneController.AttachFlow(this.Flow);
				this.session.Subscriber.SubscribeTo(this.toneController);
			}

			private void EnsureSpeechRecognitionConnector()
			{
				if (this.speechRecoConnector == null)
				{
					this.speechRecoConnector = new SpeechRecognitionConnector();
					this.speechRecoConnector.AttachFlow(this.Flow);
				}
			}

			private void EnsureSpeechRecognizer(CultureInfo culture)
			{
				if (this.SpeechReco != null && this.speechReco.RecognizerInfo.Culture.LCID != culture.LCID)
				{
					this.session.Diag.Trace("Language changed from {0} to {1}, discarding old engine", new object[]
					{
						this.speechReco.RecognizerInfo.Culture.Name,
						culture.Name
					});
					this.speechReco.Dispose();
					this.speechReco = null;
				}
				if (this.speechReco == null)
				{
					this.session.Diag.Trace("Changing culture to {0}, creating new engine", new object[]
					{
						culture.Name
					});
					this.speechReco = new SpeechRecognitionEngine(UcmaInstalledRecognizers.GetRecognizerId(SpeechRecognitionEngineType.CmdAndControl, culture));
					this.speechReco.UpdateRecognizerSetting("AssumeCFGFromTrustedSource", 1);
					this.session.Subscriber.SubscribeTo(this.speechReco);
					this.session.Diag.Assert(culture.LCID == this.speechReco.RecognizerInfo.Culture.LCID, "Reco culture cannot be changed requested {0}, existing {1}", new object[]
					{
						culture.LCID,
						this.speechReco.RecognizerInfo.Culture.LCID
					});
				}
			}

			private void EnsureSpeechRecognitionStream()
			{
				this.connectorShouldBeActive = true;
				if (!this.speechRecoConnector.IsActive)
				{
					if (this.speechRecoStream != null)
					{
						this.speechRecoStream.Dispose();
					}
					this.speechRecoStream = this.speechRecoConnector.Start();
					this.speechReco.SetInputToAudioStream(this.speechRecoStream, UcmaCallSession.SpeechAudioFormatInfo);
				}
			}

			private AudioVideoCall call;

			private PromptPlayer player;

			private Recorder mediaRecorder;

			private SpeechRecognitionEngine speechReco;

			private SpeechRecognitionConnector speechRecoConnector;

			private ToneController toneController;

			private UcmaCallSession session;

			private Stream speechRecoStream;

			private bool connectorShouldBeActive;

			private bool flowActivated;
		}

		private abstract class SessionState : DisposableBase
		{
			public SessionState(UcmaCallSession session)
			{
				this.Session = session;
				this.Args = new UMCallSessionEventArgs();
			}

			public bool IsWaitingForAsyncCompletion
			{
				get
				{
					return this.waiting != UcmaCallSession.SessionState.AsyncOperation.None;
				}
			}

			public abstract string Name { get; }

			public DiagnosticHelper Diag
			{
				get
				{
					return this.Session.Diag;
				}
			}

			public virtual bool IsIdle
			{
				get
				{
					return !this.IsWaitingForAsyncCompletion;
				}
			}

			private protected UcmaCallSession Session { protected get; private set; }

			private protected UMCallSessionEventArgs Args { protected get; private set; }

			protected UcmaCallSession.SubscriptionHelper EventSubscriber
			{
				get
				{
					return this.Session.Subscriber;
				}
			}

			public void Start()
			{
				this.Diag.Trace("Starting session state {0}", new object[]
				{
					this.Name
				});
				this.InternalStart();
				this.Session.TestInfo.ProcessQueue();
			}

			public void Cancel(bool fireOnCancelled)
			{
				if (fireOnCancelled)
				{
					this.completion = UcmaCallSession.SessionState.CompletionReason.Cancel;
				}
				if (this.IsIdle)
				{
					this.CompleteState();
					return;
				}
				this.InternalCancel();
			}

			public void CompleteAsyncCallback()
			{
				if (!this.IsWaitingForAsyncCompletion)
				{
					this.CompleteState();
				}
			}

			public void FireError(Exception e)
			{
				this.Args.Error = e;
				this.Session.ConsecutiveErrors.Add(this.Args.Error);
				try
				{
					if (!this.IsIdle)
					{
						this.Cancel(false);
					}
					else
					{
						this.Diag.Assert(!this.IsWaitingForAsyncCompletion, "Idle, yet waiting!");
						this.Session.ChangeState(new UcmaCallSession.ErrorSessionState(this.Session, e));
						this.Session.ConsecutiveErrors.Clear();
						this.Diag.Trace("Successfully processed OnError", new object[0]);
					}
				}
				finally
				{
					if (this.Session.ConsecutiveErrors.Count > 5)
					{
						this.DropCallDueToConsecutiveErrors();
					}
				}
			}

			public virtual void Call_EstablishCompleted(IAsyncResult r)
			{
				this.Diag.Assert(false, "Call_EstablishCompleted unhandled");
			}

			public virtual void Call_AudioVideoFlowConfigurationRequested(object sender, AudioVideoFlowConfigurationRequestedEventArgs e)
			{
				this.Diag.Trace("Call_AudioVideoFlowConfigurationRequested unhandled", new object[0]);
			}

			public virtual void Call_MediaTroubleshootingDataReported(object sender, MediaTroubleshootingDataReportedEventArgs e)
			{
				this.Diag.Trace("MediaTroubleshootingDataReportedEventArgs:  QualityOfExperienceContent: {0}", new object[]
				{
					e.QualityOfExperienceContent
				});
				if (e.QualityOfExperienceContent != null)
				{
					AudioQuality audioQuality;
					string text;
					if (AudioQuality.TryParseAudioQuality(e.QualityOfExperienceContent.GetBody(), ref audioQuality, ref text))
					{
						this.Diag.Trace("AudioQuality metrics: {0}", new object[]
						{
							audioQuality
						});
						this.Diag.Assert(this.Session.CurrentCallContext != null, "Call context already disposed of");
						this.Session.CurrentCallContext.CallLoggingHelper.AudioMetrics = audioQuality;
					}
					else
					{
						this.Diag.Trace("AudioQuality Validation Errors: {0}", new object[]
						{
							text
						});
					}
				}
				if (e.MediaChannelEstablishmentDataCollection != null && e.MediaChannelEstablishmentDataCollection.Count > 0 && this.Session.CurrentCallContext.DialPlan != null && this.Session.CurrentCallContext.DialPlan.URIType == UMUriType.SipName)
				{
					UcmaVoipPlatform.HandlePossibleMRASIssues(e.MediaChannelEstablishmentDataCollection[0]);
				}
			}

			public virtual void Call_StateChanged(object sender, CallStateChangedEventArgs e)
			{
				this.Diag.Trace(UcmaCallSession.FormatStateChange<CallState>("Call", e.PreviousState, e.State), new object[0]);
				this.Diag.Trace("Reason: {0}", new object[]
				{
					e.TransitionReason
				});
				this.Session.CallStateTransitionReason = e.TransitionReason;
				this.Session.LastCallStateChangeProcessed = e.State;
				if ((!(this.Session is UcmaDependentCallSession) || !((UcmaDependentCallSession)this.Session).IgnoreBye) && this.Session.IsSignalingTerminatingOrTerminated)
				{
					if (e.State == 8 && this.IsWaitingFor(UcmaCallSession.SessionState.AsyncOperation.TerminatedOrFax))
					{
						this.completion = UcmaCallSession.SessionState.CompletionReason.Teardown;
						this.StopWaitFor(UcmaCallSession.SessionState.AsyncOperation.TerminatedOrFax);
					}
					else if (e.TransitionReason == 8 || e.TransitionReason == 9)
					{
						this.completion = UcmaCallSession.SessionState.CompletionReason.Hangup;
					}
					this.Cancel(false);
				}
			}

			public virtual void Call_InfoReceived(object sender, MessageReceivedEventArgs e)
			{
				int num = 500;
				string text = null;
				try
				{
					InfoMessage infoMessage = new InfoMessage
					{
						ContentType = e.ContentType,
						Body = e.GetBody()
					};
					foreach (SignalingHeader signalingHeader in e.RequestData.SignalingHeaders)
					{
						text = signalingHeader.Name;
						string value = signalingHeader.GetValue();
						if (!infoMessage.Headers.ContainsKey(text))
						{
							infoMessage.Headers.Add(text, value);
						}
					}
					UcmaCallSession.TestInfoEvent testInfoEvent = UcmaCallSession.TestInfoEventFactory.CreateFromSipInfo(infoMessage);
					if (testInfoEvent != null)
					{
						this.Session.TestInfo.QueueEvent(testInfoEvent);
					}
					else
					{
						InfoMessage.MessageReceivedEventArgs args = new InfoMessage.MessageReceivedEventArgs(infoMessage);
						this.Session.ConditionalUpdateCallerId(args);
						this.Session.Fire<InfoMessage.MessageReceivedEventArgs>(this.Session.OnMessageReceived, args);
					}
					this.Session.TestInfo.ProcessQueue();
					num = 200;
				}
				catch (MessageParsingException ex)
				{
					this.Diag.Trace("Ignoring invalid SIP INFO message header: {0}.  {1}", new object[]
					{
						text,
						ex
					});
					num = 400;
				}
				finally
				{
					e.SendResponse(num);
				}
			}

			public virtual void Call_RemoteParticipantChanged(object sender, RemoteParticipantChangedEventArgs e)
			{
				if (this.Session.CallInfo != null && this.Session.CallInfo.IsInbound)
				{
					PlatformTelephonyAddress callerAddress;
					try
					{
						callerAddress = new PlatformTelephonyAddress(e.NewParticipant.DisplayName, new UcmaSipUri(e.NewParticipant.Uri));
					}
					catch (ArgumentException ex)
					{
						this.Diag.Trace("Ignoring SIP UPDATE for invalid remote participant uri: {0}. error ={1}", new object[]
						{
							e.NewParticipant.Uri,
							ex
						});
						return;
					}
					this.Session.UpdateCallerId(callerAddress);
				}
			}

			public virtual void Call_AcceptCompleted(IAsyncResult r)
			{
				this.Diag.Assert(false, "Call_AcceptCompleted Unhandled");
			}

			public virtual void Call_SendInfoCompleted(IAsyncResult r)
			{
				this.StopWaitFor(UcmaCallSession.SessionState.AsyncOperation.SendMessageComplete);
				this.Session.Ucma.Call.EndSendMessage(r);
				this.Session.Fire<EventArgs>(this.Session.OnMessageSent, null);
			}

			public virtual void Call_SendPromptInfoCompleted(IAsyncResult r)
			{
				this.Diag.Assert(false, "Call_SendPromptInfoCompleted Unhandled");
			}

			public virtual void Call_TerminateCompleted(IAsyncResult r)
			{
				this.Diag.Assert(false, "Call_TerminateCompleted Unhandled");
			}

			public virtual void Call_TransferCompleted(IAsyncResult r)
			{
				this.Diag.Assert(false, "Call_TransferCompleted Unhandled");
			}

			public virtual void Flow_StateChanged(object sender, MediaFlowStateChangedEventArgs e)
			{
				this.Diag.Trace(UcmaCallSession.FormatStateChange<MediaFlowState>("Flow_State", e.PreviousState, e.State), new object[0]);
			}

			public virtual void Flow_ConfigurationChanged(object sender, AudioVideoFlowConfigurationChangedEventArgs e)
			{
				this.Diag.Trace("Flow_ConfigurationChanged called", new object[0]);
				bool flag = this.callOnHold;
				AudioVideoFlow audioVideoFlow = (AudioVideoFlow)sender;
				if (audioVideoFlow.State != 1)
				{
					this.Diag.Trace("Flow_ConfigurationChanged called when state is {0}", new object[]
					{
						audioVideoFlow.State
					});
					return;
				}
				AudioChannel audioChannel = null;
				if (audioVideoFlow.Audio.GetChannels().TryGetValue(0, out audioChannel))
				{
					this.Diag.Trace("Flow_ConfigurationChanged called: ChannelLabel.AudioMono present", new object[0]);
					if (audioChannel.Direction == 2 || audioChannel.Direction == null)
					{
						if (!flag)
						{
							this.callOnHold = true;
							this.Session.Ucma.Pause();
							this.OnHoldNotify();
							this.Session.Fire<UMCallSessionEventArgs>(this.Session.OnHold, this.Args);
							return;
						}
					}
					else if (flag)
					{
						this.callOnHold = false;
						this.Session.Ucma.Resume();
						this.OnResumeNotify();
						this.Session.Fire<UMCallSessionEventArgs>(this.Session.OnResume, this.Args);
						return;
					}
				}
				else
				{
					this.Diag.Trace("Flow_ConfigurationChanged called: ChannelLabel.AudioMono not present", new object[0]);
				}
			}

			public virtual void Player_BookmarkReached(object sender, BookmarkReachedEventArgs e)
			{
				this.Diag.Assert(false, "Player_BookmarkReached Unhandled");
			}

			public virtual void Player_SpeakCompleted(object sender, PromptPlayer.PlayerCompletedEventArgs e)
			{
				this.Diag.Assert(false, "Player_SpeakCompleted Unhandled");
			}

			public virtual void SpeechReco_RecognizeCompleted(object sender, RecognizeCompletedEventArgs e)
			{
				this.Diag.Assert(false, "SpeechReco_RecognizeCompleted Unhandled in {0}", new object[]
				{
					this.Name
				});
			}

			public virtual void SpeechReco_SpeechDetected(object sender, SpeechDetectedEventArgs e)
			{
				this.Diag.Trace("Ignoring SpeechReco_SpeechDetected", new object[0]);
			}

			public virtual void SpeechReco_SpeechHypothesized(object sender, SpeechHypothesizedEventArgs e)
			{
				this.Diag.Assert(false, "SpeechReco_SpeechHypothesized Unhandled in {0}", new object[]
				{
					this.Name
				});
			}

			public virtual void SpeechReco_EmulateRecognizeCompleted(object sender, EmulateRecognizeCompletedEventArgs e)
			{
				this.Diag.Assert(false, "SpeechReco_EmulateRecognizeCompleted Unhandled in {0}", new object[]
				{
					this.Name
				});
			}

			public virtual void MediaPlayer_StateChanged(object sender, PlayerStateChangedEventArgs e)
			{
				this.Diag.Assert(false, "MediaPlayer_StateChanged Unhandled");
			}

			public virtual void MediaRecorder_StateChanged(object sender, RecorderStateChangedEventArgs e)
			{
				if (this.Session.CommandAndControlLoggingEnabled)
				{
					this.Diag.Trace("Ignoring MediaRecorder_StateChanged", new object[0]);
					return;
				}
				this.Diag.Assert(false, "MediaRecorder_StateChanged Unhandled");
			}

			public virtual void MediaRecorder_VoiceActivityChanged(object sender, VoiceActivityChangedEventArgs e)
			{
				this.Diag.Trace("Ignoring VoiceActivityChanged event.", new object[0]);
			}

			public virtual void ToneController_ToneReceived(object sender, ToneControllerEventArgs e)
			{
				this.Session.ToneAccumulator.Add(UcmaCallSession.ToneToByte((byte)e.Tone));
			}

			public virtual void ToneController_IncomingFaxDetected(object sender, IncomingFaxDetectedEventArgs e)
			{
				this.StopWaitFor(UcmaCallSession.SessionState.AsyncOperation.TerminatedOrFax);
				this.completion = UcmaCallSession.SessionState.CompletionReason.Fax;
				this.Session.CurrentCallContext.FaxToneReceived = true;
				this.Args.DtmfDigits = Encoding.ASCII.GetBytes("faxtone");
				this.Cancel(false);
			}

			public virtual void SendMessage(InfoMessage message)
			{
				this.Diag.Assert(!this.IsWaitingFor(UcmaCallSession.SessionState.AsyncOperation.SendMessageComplete), "Cannot send a new INFO message until the previous INFO callback completes.");
				List<SignalingHeader> list = null;
				if (message.Headers != null && message.Headers.Count > 0)
				{
					list = message.Headers.ConvertAll((KeyValuePair<string, string> kvp) => new SignalingHeader(kvp.Key, kvp.Value));
				}
				AsyncCallback asyncCallback = this.EventSubscriber.CreateSerializedAsyncCallback(delegate(IAsyncResult r)
				{
					this.Session.CurrentState.Call_SendInfoCompleted(r);
				}, "Call_SendInfoCompleted");
				CallSendMessageRequestOptions callSendMessageRequestOptions = new CallSendMessageRequestOptions();
				if (list != null)
				{
					CollectionExtensions.AddRange<SignalingHeader>(callSendMessageRequestOptions.Headers, list);
				}
				ContentDescription contentDescription = new ContentDescription(message.ContentType, message.Body);
				this.Session.Ucma.Call.BeginSendMessage(1, contentDescription, callSendMessageRequestOptions, asyncCallback, this);
				this.StartWaitFor(UcmaCallSession.SessionState.AsyncOperation.SendMessageComplete);
			}

			public bool ConditionalWaitForTerminatedOrFax()
			{
				if (this.Session.IsMediaTerminatingOrTerminated && this.Session.LastCallStateChangeProcessed != 8 && !this.Session.CurrentCallContext.FaxToneReceived)
				{
					this.StartWaitFor(UcmaCallSession.SessionState.AsyncOperation.TerminatedOrFax);
				}
				return this.IsWaitingFor(UcmaCallSession.SessionState.AsyncOperation.TerminatedOrFax);
			}

			protected static TimeSpan CalculateMinTimerDueTime(TimeSpan t1, TimeSpan t2)
			{
				if (t1 < TimeSpan.Zero || t2 < TimeSpan.Zero)
				{
					return TimeSpan.Zero;
				}
				if (t1 < t2)
				{
					return t1;
				}
				return t2;
			}

			protected virtual void InternalCancel()
			{
				this.ForceAsyncWaitingCompletions();
			}

			protected override void InternalDispose(bool disposing)
			{
			}

			protected void StartWaitFor(UcmaCallSession.SessionState.AsyncOperation op)
			{
				if (!this.IsWaitingFor(op))
				{
					this.Diag.Trace("Now waiting for {0}", new object[]
					{
						op
					});
					this.waiting |= op;
				}
			}

			protected void StopWaitFor(UcmaCallSession.SessionState.AsyncOperation op)
			{
				if (this.IsWaitingFor(op))
				{
					this.Diag.Trace("Done waiting for {0}", new object[]
					{
						op
					});
					this.waiting &= ~op;
					this.Diag.Trace("Still waiting for {0}", new object[]
					{
						this.waiting
					});
				}
			}

			protected bool IsWaitingFor(UcmaCallSession.SessionState.AsyncOperation op)
			{
				return (this.waiting & op) == op;
			}

			protected bool RequiresSigningAsTransferor(bool isSigningEnabled, PlatformSignalingHeader h, out string transferor)
			{
				transferor = null;
				if (isSigningEnabled && string.Equals(h.Name, "Referred-By", StringComparison.OrdinalIgnoreCase))
				{
					transferor = UcmaCallSession.SessionState.GetTransferorFromReferredBy(h);
					this.Diag.Trace("RequiresSigningAsTransferor: Signing required. Transferor: {0}", new object[]
					{
						transferor
					});
				}
				return null != transferor;
			}

			protected abstract void InternalStart();

			protected abstract void ForceAsyncWaitingCompletions();

			protected abstract void CompleteFinalAsyncCallback();

			protected virtual void OnHoldNotify()
			{
				this.Diag.Trace("On Hold notification received", new object[0]);
			}

			protected virtual void OnResumeNotify()
			{
				this.Diag.Trace("On Resume notification received", new object[0]);
			}

			private static string GetTransferorFromReferredBy(PlatformSignalingHeader referredBy)
			{
				string text = referredBy.Value;
				if (text.StartsWith("<", StringComparison.OrdinalIgnoreCase) && text.EndsWith(">", StringComparison.OrdinalIgnoreCase))
				{
					text = text.Substring(1, text.Length - 2);
				}
				return text.ToLowerInvariant();
			}

			private void CompleteState()
			{
				this.Diag.Assert(!this.IsWaitingForAsyncCompletion, "Waiting for Async completion, but completing state!");
				if (!base.IsDisposed)
				{
					if (this is UcmaCallSession.TeardownSessionState)
					{
						this.CompleteFinalAsyncCallback();
						return;
					}
					this.CompleteNonTeardownState();
				}
			}

			private void CompleteNonTeardownState()
			{
				if (this.Args.Error != null)
				{
					this.Diag.Trace("CompleteAsyncCallback with error='{0}'", new object[]
					{
						this.Args.Error
					});
					this.FireError(this.Args.Error);
				}
				else if (this.completion == UcmaCallSession.SessionState.CompletionReason.Cancel)
				{
					this.Diag.Trace("CompleteAsyncCallback with OnCancelled", new object[0]);
					this.Session.Fire<UMCallSessionEventArgs>(this.Session.OnCancelled, this.Args);
				}
				else if (this.completion == UcmaCallSession.SessionState.CompletionReason.Hangup)
				{
					this.Diag.Trace("CompleteAsyncCallback with OnHangup", new object[0]);
					this.Session.Fire<UMCallSessionEventArgs>(this.Session.OnHangup, this.Args);
				}
				else if (this.completion == UcmaCallSession.SessionState.CompletionReason.Fax)
				{
					this.Diag.Trace("CompleteAsyncCallback with OnFax (OnDtmf)", new object[0]);
					this.Session.Fire<UMCallSessionEventArgs>(this.Session.OnDtmf, this.Args);
				}
				else if (this.completion == UcmaCallSession.SessionState.CompletionReason.Teardown)
				{
					this.Diag.Trace("CompleteAsyncCallback with Teardown", new object[0]);
					this.Session.ChangeState(new UcmaCallSession.TeardownSessionState(this.Session, UcmaCallSession.DisconnectType.Remote, null));
				}
				else
				{
					this.Diag.Trace("CompleteAsyncCallback using derived class '{0}'", new object[]
					{
						this.Name
					});
					this.CompleteFinalAsyncCallback();
				}
				this.Session.TeardownIdleSession();
			}

			private void DropCallDueToConsecutiveErrors()
			{
				this.Diag.Trace("Droppring call because there have been too many consecutive unhandled errors", new object[0]);
				this.TraceConsecutiveErrors();
				ExceptionHandling.SendWatsonWithExtraData(this.Args.Error, false);
				this.Session.ChangeState(new UcmaCallSession.TeardownSessionState(this.Session, UcmaCallSession.DisconnectType.Remote, null));
			}

			private void TraceConsecutiveErrors()
			{
				foreach (Exception ex in this.Session.ConsecutiveErrors)
				{
					this.Diag.Trace("=> {0} followed by:", new object[]
					{
						ex
					});
				}
				this.Diag.Trace("=> Drop Call", new object[0]);
			}

			private UcmaCallSession.SessionState.AsyncOperation waiting;

			private UcmaCallSession.SessionState.CompletionReason completion;

			private bool callOnHold;

			[Flags]
			protected enum AsyncOperation
			{
				None = 0,
				CallAccepted = 1,
				CallEstablished = 2,
				FlowEstablished = 4,
				TransferComplete = 8,
				DtmfInputTimer = 16,
				HeavyBlockingComplete = 32,
				PromptsComplete = 64,
				SpeechRecoComplete = 256,
				BeepComplete = 512,
				RecordComplete = 1024,
				SendMessageComplete = 2048,
				TerminateComplete = 4096,
				MaxRecordingTimer = 8192,
				VoiceActivityTimer = 16384,
				SendDtmfComplete = 32768,
				SendPromptInfoComplete = 65536,
				CallStateTerminated = 131072,
				TerminatedOrFax = 262144,
				SpeechEmulateComplete = 524288
			}

			private enum CompletionReason
			{
				None,
				Cancel,
				Hangup,
				Fax,
				Teardown
			}
		}

		private abstract class InitializeFlowSessionState : UcmaCallSession.SessionState
		{
			public InitializeFlowSessionState(UcmaCallSession session) : base(session)
			{
			}

			private protected bool FlowActivated { protected get; private set; }

			public override void Call_AudioVideoFlowConfigurationRequested(object sender, AudioVideoFlowConfigurationRequestedEventArgs e)
			{
				AudioVideoFlowTemplate audioVideoFlowTemplate = new AudioVideoFlowTemplate(e.Flow);
				audioVideoFlowTemplate.EncryptionPolicy = base.Session.MediaEncryptionPolicy;
				AudioChannelTemplate audioChannelTemplate = audioVideoFlowTemplate.Audio.GetChannels()[0];
				audioChannelTemplate.UseHighPerformance = !AppConfig.Instance.Service.EnableRTAudio;
				e.Flow.Initialize(audioVideoFlowTemplate);
				base.Session.Subscriber.SubscribeTo(e.Flow);
			}

			public override void Flow_StateChanged(object sender, MediaFlowStateChangedEventArgs e)
			{
				base.Diag.Trace(UcmaCallSession.FormatStateChange<MediaFlowState>("Flow", e.PreviousState, e.State), new object[0]);
				if (base.IsWaitingFor(UcmaCallSession.SessionState.AsyncOperation.FlowEstablished) && e.State == 1)
				{
					base.StopWaitFor(UcmaCallSession.SessionState.AsyncOperation.FlowEstablished);
					base.Session.Ucma.ActivateFlow();
					this.FlowActivated = true;
				}
			}

			protected override void InternalStart()
			{
				base.StartWaitFor(UcmaCallSession.SessionState.AsyncOperation.FlowEstablished);
			}

			protected override void ForceAsyncWaitingCompletions()
			{
				base.StopWaitFor(UcmaCallSession.SessionState.AsyncOperation.FlowEstablished);
			}
		}

		private class AcceptCallSessionState : UcmaCallSession.InitializeFlowSessionState
		{
			public AcceptCallSessionState(UcmaCallSession session) : base(session)
			{
			}

			public override string Name
			{
				get
				{
					return "AcceptCallSessionState";
				}
			}

			public override void Call_AcceptCompleted(IAsyncResult r)
			{
				base.StopWaitFor(UcmaCallSession.SessionState.AsyncOperation.CallAccepted);
				base.Session.Ucma.Call.EndAccept(r);
			}

			protected override void InternalStart()
			{
				AsyncCallback asyncCallback = base.EventSubscriber.CreateSerializedAsyncCallback(delegate(IAsyncResult r)
				{
					base.Session.CurrentState.Call_AcceptCompleted(r);
				}, "Call_AcceptCompleted");
				CallAcceptOptions callAcceptOptions = new CallAcceptOptions();
				callAcceptOptions.RedirectDueToBandwidthPolicyEnabled = false;
				base.Session.Ucma.Call.BeginAccept(callAcceptOptions, asyncCallback, this);
				base.StartWaitFor(UcmaCallSession.SessionState.AsyncOperation.CallAccepted);
				base.InternalStart();
			}

			protected override void ForceAsyncWaitingCompletions()
			{
				base.ForceAsyncWaitingCompletions();
			}

			protected override void CompleteFinalAsyncCallback()
			{
				if (!base.FlowActivated)
				{
					base.Diag.Trace("Flow was never established.  Tearing down", new object[0]);
					base.Session.ChangeState(new UcmaCallSession.TeardownSessionState(base.Session, UcmaCallSession.DisconnectType.Remote, null));
					return;
				}
				base.Session.CurrentCallContext.ConnectionTime = new ExDateTime?(ExDateTime.UtcNow);
				base.Diag.Trace("Flow established.  Pausing, then firing OnCallConnected.", new object[0]);
				Thread.Sleep(250);
				base.Session.Fire<EventArgs>(base.Session.OnCallConnected, null);
			}

			protected override DisposeTracker InternalGetDisposeTracker()
			{
				return DisposeTracker.Get<UcmaCallSession.AcceptCallSessionState>(this);
			}
		}

		private class AcceptFaxSessionState : UcmaCallSession.SessionState
		{
			public AcceptFaxSessionState(UcmaCallSession session) : base(session)
			{
			}

			public override string Name
			{
				get
				{
					return "AcceptFaxSessionState";
				}
			}

			protected override void InternalStart()
			{
				base.Session.CurrentCallContext.CallType = 6;
				base.Session.TaskCallType = CommonConstants.TaskCallType.Fax;
				base.Session.Fire<UMCallSessionEventArgs>(base.Session.OnFaxRequestReceive, base.Args);
			}

			protected override void ForceAsyncWaitingCompletions()
			{
			}

			protected override void CompleteFinalAsyncCallback()
			{
			}

			protected override DisposeTracker InternalGetDisposeTracker()
			{
				return DisposeTracker.Get<UcmaCallSession.AcceptFaxSessionState>(this);
			}
		}

		private class BlindTransferSessionState : UcmaCallSession.SessionState
		{
			public BlindTransferSessionState(UcmaCallSession session, string phoneNumber) : base(session)
			{
				bool flag = false;
				try
				{
					this.Initialize(this.UriFromPhoneNumber(phoneNumber), null);
					flag = true;
				}
				finally
				{
					if (!flag)
					{
						this.Dispose();
					}
				}
			}

			public BlindTransferSessionState(UcmaCallSession session, PlatformSipUri uri, IEnumerable<PlatformSignalingHeader> headers) : base(session)
			{
				this.Initialize(uri, headers);
			}

			public override string Name
			{
				get
				{
					return "BlindTransferSessionState";
				}
			}

			public override void Call_TransferCompleted(IAsyncResult r)
			{
				base.StopWaitFor(UcmaCallSession.SessionState.AsyncOperation.TransferComplete);
				base.Session.Ucma.Call.EndTransfer(r);
			}

			protected override void InternalStart()
			{
				UcmaPlatform.ValidateRealTimeUri(this.uri.ToString());
				AsyncCallback asyncCallback = base.EventSubscriber.CreateSerializedAsyncCallback(delegate(IAsyncResult r)
				{
					base.Session.CurrentState.Call_TransferCompleted(r);
				}, "Call_TransferCompleted");
				base.Session.Ucma.Call.BeginTransfer(this.uri.ToString(), this.CreateCallTransferOptions(), asyncCallback, this);
				base.StartWaitFor(UcmaCallSession.SessionState.AsyncOperation.TransferComplete);
			}

			protected override void ForceAsyncWaitingCompletions()
			{
			}

			protected override void CompleteFinalAsyncCallback()
			{
				base.Session.Fire<UMCallSessionEventArgs>(base.Session.OnTransferComplete, base.Args);
			}

			protected override DisposeTracker InternalGetDisposeTracker()
			{
				return DisposeTracker.Get<UcmaCallSession.BlindTransferSessionState>(this);
			}

			private void Initialize(PlatformSipUri uri, IEnumerable<PlatformSignalingHeader> headers)
			{
				this.uri = uri;
				this.headers = headers;
			}

			private PlatformSipUri UriFromPhoneNumber(string phoneNumber)
			{
				UcmaCallInfo callInfo = base.Session.CallInfo;
				base.Diag.Assert(null != callInfo, "no call info in blind transfer!");
				base.Diag.Assert(null != callInfo.RemoteContactUri, "remote contact uri is null in blind transfer!");
				return new UcmaSipUri(callInfo.RemoteContactUri.ToString())
				{
					User = phoneNumber,
					UserParameter = UserParameter.Phone,
					TransportParameter = ((base.Session.LocalEndpoint.Platform.Transport == 1) ? TransportParameter.Tcp : TransportParameter.Tls)
				};
			}

			private CallTransferOptions CreateCallTransferOptions()
			{
				CallTransferOptions callTransferOptions = new CallTransferOptions(0);
				bool supportsMsOrganizationRouting = base.Session.CurrentCallContext.RoutingHelper.SupportsMsOrganizationRouting;
				if (this.headers != null)
				{
					foreach (PlatformSignalingHeader platformSignalingHeader in this.headers)
					{
						string transferor;
						if (base.RequiresSigningAsTransferor(supportsMsOrganizationRouting, platformSignalingHeader, out transferor))
						{
							callTransferOptions.Transferor = transferor;
						}
						else
						{
							callTransferOptions.Headers.Add(new SignalingHeader(platformSignalingHeader.Name, platformSignalingHeader.Value));
						}
					}
				}
				return callTransferOptions;
			}

			private PlatformSipUri uri;

			private IEnumerable<PlatformSignalingHeader> headers;
		}

		private class PlayPromptsSessionState : UcmaCallSession.SessionState
		{
			public PlayPromptsSessionState(UcmaCallSession session, UcmaCallSession.PlaybackInfo promptInfo) : base(session)
			{
				this.PlaybackInfo = promptInfo;
			}

			public bool IsSpeaking { get; private set; }

			public override bool IsIdle
			{
				get
				{
					return !base.IsWaitingForAsyncCompletion && !this.IsSpeaking;
				}
			}

			public override string Name
			{
				get
				{
					return "PlayPromptsSessionState";
				}
			}

			private protected UcmaCallSession.PlaybackInfo PlaybackInfo { protected get; private set; }

			public override void Player_SpeakCompleted(object sender, PromptPlayer.PlayerCompletedEventArgs e)
			{
				base.StopWaitFor(UcmaCallSession.SessionState.AsyncOperation.PromptsComplete);
				this.IsSpeaking = false;
				base.Args.PlayTime = this.PlaybackInfo.Offset + (DateTime.UtcNow - this.promptStartTimeUtc);
			}

			public override void ToneController_ToneReceived(object sender, ToneControllerEventArgs e)
			{
				if (!this.PlaybackInfo.Uninterruptable)
				{
					this.ForceAsyncWaitingCompletions();
					base.ToneController_ToneReceived(sender, e);
				}
			}

			public override void Call_SendPromptInfoCompleted(IAsyncResult r)
			{
				base.StopWaitFor(UcmaCallSession.SessionState.AsyncOperation.SendPromptInfoComplete);
				try
				{
					base.Session.Ucma.Call.EndSendMessage(r);
				}
				catch (FailureResponseException ex)
				{
					base.Diag.Trace("An exception is hit in sending SIP Info for prompts: {0}", new object[]
					{
						ex
					});
				}
			}

			public virtual void HandleApplicationRequestToStopPlayback()
			{
				base.Args.WasPlaybackStopped = true;
				base.Session.Ucma.Player.Cancel();
			}

			public void Skip(TimeSpan timeToSkip)
			{
				base.Session.Ucma.Player.Skip(timeToSkip);
			}

			protected override void InternalStart()
			{
				this.ConditionalSendPromptInfoMessage();
				this.ConditionalStartPrompts();
			}

			protected override void InternalCancel()
			{
				this.ForceAsyncWaitingCompletions();
				base.Session.Ucma.Player.Cancel();
			}

			protected override DisposeTracker InternalGetDisposeTracker()
			{
				return DisposeTracker.Get<UcmaCallSession.PlayPromptsSessionState>(this);
			}

			protected override void ForceAsyncWaitingCompletions()
			{
				if (base.IsWaitingFor(UcmaCallSession.SessionState.AsyncOperation.PromptsComplete))
				{
					base.Session.Ucma.Player.Cancel();
				}
			}

			protected override void CompleteFinalAsyncCallback()
			{
				base.Diag.Assert(!this.IsSpeaking || !this.PlaybackInfo.UnconditionalBargeIn);
				if (!this.IsSpeaking)
				{
					base.Session.Fire<UMCallSessionEventArgs>(base.Session.OnComplete, base.Args);
				}
			}

			protected void ConditionalSendPromptInfoMessage()
			{
				if (this.ShouldSendTestInfo())
				{
					string text = base.Session.BuildPromptInfoMessageString(this.PlaybackInfo.TurnName, this.PlaybackInfo.Prompts);
					AsyncCallback asyncCallback = base.EventSubscriber.CreateSerializedAsyncCallback(delegate(IAsyncResult r)
					{
						base.Session.CurrentState.Call_SendPromptInfoCompleted(r);
					}, "Call_SendPromptInfoCompleted");
					ContentDescription contentDescription = new ContentDescription(CommonConstants.ContentTypeTextPlain, text);
					base.Session.Ucma.Call.BeginSendMessage(1, contentDescription, new CallSendMessageRequestOptions(), asyncCallback, this);
					base.StartWaitFor(UcmaCallSession.SessionState.AsyncOperation.SendPromptInfoComplete);
				}
			}

			private void ConditionalStartPrompts()
			{
				if (this.IsTypeAheadAvailable())
				{
					this.ForceAsyncWaitingCompletions();
					base.CompleteAsyncCallback();
					return;
				}
				if (this.PlaybackInfo.Prompts.Count == 0)
				{
					this.SimulateSpeakCompleted();
					return;
				}
				this.StartPrompts();
			}

			private void SimulateSpeakCompleted()
			{
				PromptPlayer.PlayerCompletedEventArgs e = new PromptPlayer.PlayerCompletedEventArgs();
				this.Player_SpeakCompleted(this, e);
				base.CompleteAsyncCallback();
			}

			private void StartPrompts()
			{
				this.promptStartTimeUtc = DateTime.UtcNow;
				ArrayList prompts = base.Session.TestInfo.IsFeatureEnabled(UcmaCallSession.TestInfoFeatures.SkipPrompt) ? new ArrayList() : this.PlaybackInfo.Prompts;
				base.Session.Ucma.Player.Play(prompts, base.Session.CurrentCallContext.Culture, this.PlaybackInfo.Offset);
				this.IsSpeaking = true;
				if (this.PlaybackInfo.UnconditionalBargeIn)
				{
					base.StartWaitFor(UcmaCallSession.SessionState.AsyncOperation.PromptsComplete);
				}
			}

			private bool ShouldSendTestInfo()
			{
				bool result = false;
				if (this.PlaybackInfo.Prompts.Count > 0 && (base.Session.CurrentCallContext.IsTestCall || base.Session.CurrentCallContext.IsTUIDiagnosticCall))
				{
					result = true;
				}
				return result;
			}

			private bool IsTypeAheadAvailable()
			{
				return !base.Session.ToneAccumulator.IsEmpty && !this.PlaybackInfo.Uninterruptable;
			}

			private DateTime promptStartTimeUtc;
		}

		private class PlayPromptsAndRecoDtmfSessionState : UcmaCallSession.PlayPromptsSessionState
		{
			public PlayPromptsAndRecoDtmfSessionState(UcmaCallSession session, UcmaCallSession.PlaybackInfo promptInfo) : base(session, promptInfo)
			{
				this.finalTimeoutUtc = DateTime.MaxValue;
				this.Completion = UcmaCallSession.PlayPromptsAndRecoDtmfSessionState.CompletionReason.None;
			}

			public override string Name
			{
				get
				{
					return "PlayPromptsAndRecoDtmfSessionState";
				}
			}

			private protected UcmaCallSession.PlayPromptsAndRecoDtmfSessionState.CompletionReason Completion { protected get; private set; }

			public override void Player_SpeakCompleted(object sender, PromptPlayer.PlayerCompletedEventArgs e)
			{
				base.Player_SpeakCompleted(sender, e);
				if (!e.Cancelled)
				{
					this.Completion = UcmaCallSession.PlayPromptsAndRecoDtmfSessionState.CompletionReason.None;
					if (this.ExpectingInput())
					{
						this.finalTimeoutUtc = DateTime.UtcNow + base.PlaybackInfo.Timeout;
						this.EnsureTimer(base.PlaybackInfo.InitialSilenceTimeout);
						return;
					}
					this.ForceAsyncWaitingCompletions();
				}
			}

			public override void ToneController_ToneReceived(object sender, ToneControllerEventArgs e)
			{
				this.SetCompletionReasonIfCurrentlyUnset(UcmaCallSession.PlayPromptsAndRecoDtmfSessionState.CompletionReason.Tone);
				base.Session.ToneAccumulator.Add(UcmaCallSession.ToneToByte((byte)e.Tone));
				InputState inputState = this.ComputeInputState();
				if (base.IsSpeaking && (base.PlaybackInfo.UnconditionalBargeIn || base.Session.ToneAccumulator.ContainsBargeInPattern(base.PlaybackInfo.StopPatterns)))
				{
					base.StartWaitFor(UcmaCallSession.SessionState.AsyncOperation.PromptsComplete);
				}
				base.ForceAsyncWaitingCompletions();
				if (InputStateHelper.IsUnambiguous(inputState))
				{
					this.ForceAsyncWaitingCompletions();
					return;
				}
				this.ContinueDtmfRecognition(inputState);
			}

			protected override void OnHoldNotify()
			{
				if (this.timer != null && !this.isTimerDisposed)
				{
					this.timer.Change(-1, 0);
					base.StopWaitFor(UcmaCallSession.SessionState.AsyncOperation.DtmfInputTimer);
				}
			}

			protected override void OnResumeNotify()
			{
				if (this.timer != null && !this.isTimerDisposed)
				{
					this.finalTimeoutUtc = DateTime.UtcNow + base.PlaybackInfo.Timeout;
					this.timer.Change(base.PlaybackInfo.InitialSilenceTimeout, TimeSpan.Zero);
					base.StartWaitFor(UcmaCallSession.SessionState.AsyncOperation.DtmfInputTimer);
				}
			}

			protected override void InternalStart()
			{
				InputState inputState = this.ComputeInputState();
				base.Diag.Assert(InputStateHelper.IsAllowed(inputState));
				if (!InputStateHelper.IsStarted(inputState))
				{
					base.InternalStart();
					return;
				}
				base.ConditionalSendPromptInfoMessage();
				if (InputStateHelper.IsUnambiguous(inputState))
				{
					this.SetCompletionReasonIfCurrentlyUnset(UcmaCallSession.PlayPromptsAndRecoDtmfSessionState.CompletionReason.Tone);
					this.ForceAsyncWaitingCompletions();
					base.CompleteAsyncCallback();
					return;
				}
				this.ContinueDtmfRecognition(inputState);
			}

			protected virtual bool ExpectingInput()
			{
				return base.PlaybackInfo.MinDigits > 0;
			}

			protected override void ForceAsyncWaitingCompletions()
			{
				base.StopWaitFor(UcmaCallSession.SessionState.AsyncOperation.DtmfInputTimer);
				base.ForceAsyncWaitingCompletions();
			}

			protected override void CompleteFinalAsyncCallback()
			{
				switch (this.Completion)
				{
				case UcmaCallSession.PlayPromptsAndRecoDtmfSessionState.CompletionReason.Tone:
					this.SetDigitsForCompletion();
					base.Session.Fire<UMCallSessionEventArgs>(base.Session.OnDtmf, base.Args);
					this.RestartRecognitionIfAppropriate();
					return;
				case UcmaCallSession.PlayPromptsAndRecoDtmfSessionState.CompletionReason.Timeout:
					base.Session.Fire<UMCallSessionEventArgs>(base.Session.OnTimeout, base.Args);
					return;
				default:
					base.CompleteFinalAsyncCallback();
					return;
				}
			}

			protected override void InternalDispose(bool disposing)
			{
				try
				{
					if (this.timer != null)
					{
						this.timer.Dispose();
						this.isTimerDisposed = true;
					}
				}
				finally
				{
					base.InternalDispose(disposing);
				}
			}

			private void SetCompletionReasonIfCurrentlyUnset(UcmaCallSession.PlayPromptsAndRecoDtmfSessionState.CompletionReason reason)
			{
				if (this.Completion == UcmaCallSession.PlayPromptsAndRecoDtmfSessionState.CompletionReason.None)
				{
					this.Completion = reason;
				}
			}

			private void SetDigitsForCompletion()
			{
				if (!this.digitsConsumed)
				{
					base.Args.DtmfDigits = base.Session.ToneAccumulator.ConsumeAccumulatedDigits(base.PlaybackInfo.MinDigits, base.PlaybackInfo.MaxDigits, base.PlaybackInfo.StopPatterns);
					this.digitsConsumed = true;
				}
			}

			private void DtmfInputTimer_Expired(object state)
			{
				if (base.IsWaitingFor(UcmaCallSession.SessionState.AsyncOperation.DtmfInputTimer))
				{
					base.StopWaitFor(UcmaCallSession.SessionState.AsyncOperation.DtmfInputTimer);
					if (base.Session.ToneAccumulator.IsEmpty)
					{
						this.SetCompletionReasonIfCurrentlyUnset(UcmaCallSession.PlayPromptsAndRecoDtmfSessionState.CompletionReason.Timeout);
						this.ForceAsyncWaitingCompletions();
						return;
					}
					this.SetCompletionReasonIfCurrentlyUnset(UcmaCallSession.PlayPromptsAndRecoDtmfSessionState.CompletionReason.Tone);
					this.ForceAsyncWaitingCompletions();
				}
			}

			private void ContinueDtmfRecognition(InputState inputState)
			{
				base.Diag.Assert(InputStateHelper.IsStarted(inputState), "Continuing DTMF recognition but it's not started!");
				TimeSpan t = base.PlaybackInfo.InterDigitTimeout;
				if (inputState == InputState.StartedCompleteAmbiguous)
				{
					t = TimeSpan.FromSeconds(1.0);
				}
				TimeSpan nextTimeout = UcmaCallSession.SessionState.CalculateMinTimerDueTime(this.finalTimeoutUtc - DateTime.UtcNow, t);
				this.EnsureTimer(nextTimeout);
			}

			protected void EnsureTimer(TimeSpan nextTimeout)
			{
				if (this.timer != null)
				{
					if (!this.isTimerDisposed)
					{
						this.timer.Change(nextTimeout, TimeSpan.Zero);
					}
				}
				else
				{
					TimerCallback callback = base.EventSubscriber.CreateSerializedTimerCallback(delegate(object r)
					{
						this.DtmfInputTimer_Expired(r);
					}, "DtmfInputTimer");
					this.timer = new Timer(callback, this, nextTimeout, TimeSpan.Zero);
					this.isTimerDisposed = false;
				}
				base.StartWaitFor(UcmaCallSession.SessionState.AsyncOperation.DtmfInputTimer);
			}

			private InputState ComputeInputState()
			{
				return base.Session.ToneAccumulator.ComputeInputState(base.PlaybackInfo.MinDigits, base.PlaybackInfo.MaxDigits, base.PlaybackInfo.StopPatterns, base.PlaybackInfo.StopTones);
			}

			private void RestartRecognitionIfAppropriate()
			{
				if (!base.Args.WasPlaybackStopped)
				{
					this.digitsConsumed = false;
					this.Completion = UcmaCallSession.PlayPromptsAndRecoDtmfSessionState.CompletionReason.None;
					base.Args.Reset();
				}
			}

			private DateTime finalTimeoutUtc;

			private Timer timer;

			private bool digitsConsumed;

			private bool isTimerDisposed;

			protected enum CompletionReason
			{
				None,
				Tone,
				Timeout
			}
		}

		private class PlayPromptsAndRecoSpeechSessionState : UcmaCallSession.PlayPromptsAndRecoDtmfSessionState
		{
			public PlayPromptsAndRecoSpeechSessionState(UcmaCallSession session, UcmaCallSession.PlaybackInfo promptInfo) : base(session, promptInfo)
			{
			}

			public override string Name
			{
				get
				{
					return "PlayPromptsAndRecoSpeechSessionState";
				}
			}

			public override void HandleApplicationRequestToStopPlayback()
			{
				this.speechArgs.WasPlaybackStopped = true;
				base.HandleApplicationRequestToStopPlayback();
			}

			public override void SpeechReco_SpeechHypothesized(object sender, SpeechHypothesizedEventArgs e)
			{
				if (base.IsWaitingFor(UcmaCallSession.SessionState.AsyncOperation.PromptsComplete))
				{
					base.Session.Ucma.Player.Cancel();
				}
			}

			public override void SpeechReco_SpeechDetected(object sender, SpeechDetectedEventArgs e)
			{
				base.EnsureTimer(base.PlaybackInfo.Timeout);
			}

			public override void SpeechReco_RecognizeCompleted(object sender, RecognizeCompletedEventArgs e)
			{
				base.StopWaitFor(UcmaCallSession.SessionState.AsyncOperation.SpeechRecoComplete);
				this.ForceAsyncWaitingCompletions();
				if (e.Error != null)
				{
					base.Args.Error = e.Error;
					return;
				}
				if (!e.Cancelled)
				{
					base.Diag.Trace("RecognizedCompleted.  babble='{0}', silence='{1}', inputEnded='{2}', result='{3}'", new object[]
					{
						e.BabbleTimeout,
						e.InitialSilenceTimeout,
						e.InputStreamEnded,
						e.Result
					});
					this.fireOnSpeech = true;
					if (e.Result != null && UcmaCallSession.PlayPromptsAndRecoSpeechSessionState.TestSemanticAccess(e.Result))
					{
						this.speechArgs.Result = new UcmaRecognitionResult(e.Result);
					}
				}
			}

			public override void Player_BookmarkReached(object sender, BookmarkReachedEventArgs e)
			{
				this.speechArgs.BookmarkReached = e.Bookmark;
			}

			public override void Player_SpeakCompleted(object sender, PromptPlayer.PlayerCompletedEventArgs e)
			{
				this.SetRecognizerSensitivity(0);
				base.Player_SpeakCompleted(sender, e);
			}

			protected static bool TestSemanticAccess(RecognitionResult result)
			{
				ValidateArgument.NotNull(result, "result");
				bool result2 = false;
				try
				{
					SemanticValue semantics = result.Semantics;
					result2 = true;
				}
				catch (InvalidOperationException ex)
				{
					CallIdTracer.TraceError(ExTraceGlobals.UCMATracer, null, "Failed to access semantic items.  {0}", new object[]
					{
						ex
					});
				}
				return result2;
			}

			protected override void CompleteFinalAsyncCallback()
			{
				this.speechArgs.PlayTime = base.Args.PlayTime;
				if (this.fireOnSpeech)
				{
					base.Session.Fire<UMSpeechEventArgs>(base.Session.OnSpeech, this.speechArgs);
					this.RestartRecognitionIfAppropriate();
					return;
				}
				base.CompleteFinalAsyncCallback();
			}

			protected override void ForceAsyncWaitingCompletions()
			{
				base.Session.Ucma.HaltRecognition();
				base.ForceAsyncWaitingCompletions();
			}

			protected override void InternalStart()
			{
				base.Session.Ucma.EnsureRecognition(base.Session.CurrentCallContext.Culture);
				this.SetRecognizerSensitivity(100);
				this.LoadGrammars();
				this.StartRecognizer();
				base.InternalStart();
			}

			protected override bool ExpectingInput()
			{
				return true;
			}

			protected virtual void StartRecognizer()
			{
				base.Session.Ucma.EnsureRecognition(base.Session.CurrentCallContext.Culture);
				base.Session.Ucma.SpeechReco.InitialSilenceTimeout = TimeSpan.MaxValue;
				base.Session.Ucma.SpeechReco.BabbleTimeout = base.PlaybackInfo.BabbleTimeout;
				base.Session.Ucma.SpeechReco.RecognizeAsync();
				base.StartWaitFor(UcmaCallSession.SessionState.AsyncOperation.SpeechRecoComplete);
			}

			private void LoadGrammars()
			{
				base.Session.Ucma.SpeechReco.UnloadAllGrammars();
				foreach (UMGrammar grammar in base.PlaybackInfo.Grammars)
				{
					this.LoadGrammar(grammar);
				}
			}

			private void LoadGrammar(UMGrammar grammar)
			{
				base.Diag.Trace("Loading grammar '{0}'", new object[]
				{
					grammar.ToString()
				});
				Grammar grammar2 = UcmaUtils.CreateGrammar(grammar);
				try
				{
					base.Session.Ucma.SpeechReco.LoadGrammar(grammar2);
				}
				catch (IOException ex)
				{
					base.Diag.Trace("Could not load grammar '{0}'. Error: {1}", new object[]
					{
						grammar,
						ex
					});
					throw new UMGrayException(ex);
				}
				catch (UnauthorizedAccessException ex2)
				{
					base.Diag.Trace("Could not load grammar '{0}'. Error: {1}", new object[]
					{
						grammar,
						ex2
					});
					throw new UMGrayException(ex2);
				}
			}

			private void RestartRecognitionIfAppropriate()
			{
				if (this.ShouldRestartRecognition())
				{
					this.RestartRecognition();
				}
			}

			private bool ShouldRestartRecognition()
			{
				return base.Session.CurrentState.Equals(this) && !this.speechArgs.WasPlaybackStopped;
			}

			private void RestartRecognition()
			{
				this.speechArgs = new UMSpeechEventArgs();
				this.fireOnSpeech = false;
				this.StartRecognizer();
			}

			private void SetRecognizerSensitivity(int setting)
			{
				base.Session.Ucma.SpeechReco.UpdateRecognizerSetting("BackgroundSpeechSensitivity", setting);
			}

			private const string BackgroundSpeechSensitivity = "BackgroundSpeechSensitivity";

			private const int Normal = 0;

			private const int EchoCancel = 100;

			protected UMSpeechEventArgs speechArgs = new UMSpeechEventArgs();

			protected bool fireOnSpeech;
		}

		private class CommandAndControlLoggingSessionState : UcmaCallSession.PlayPromptsAndRecoSpeechSessionState
		{
			public CommandAndControlLoggingSessionState(UcmaCallSession session, UcmaCallSession.PlaybackInfo promptInfo) : base(session, promptInfo)
			{
				base.Diag.Trace("Command And Control Logging Enabled", new object[0]);
				this.diagnosticRecorder = session.Ucma.MediaRecorder;
				string path = Path.Combine(Utils.TempPath, "CommandAndControlDiagnosticRecording");
				this.diagnosticCallIdFolder = Path.Combine(path, base.Session.CurrentCallContext.CallId);
				Directory.CreateDirectory(this.diagnosticCallIdFolder);
			}

			public override string Name
			{
				get
				{
					return "CommandAndControlLoggingSessionState";
				}
			}

			public override void SpeechReco_RecognizeCompleted(object sender, RecognizeCompletedEventArgs e)
			{
				this.StopCommandAndControlDiagnosticRecording(e);
				base.SpeechReco_RecognizeCompleted(sender, e);
			}

			protected override void StartRecognizer()
			{
				this.StartCommandAndControlDiagnosticRecording();
				base.StartRecognizer();
			}

			private void StartCommandAndControlDiagnosticRecording()
			{
				this.diagnosticTmpWma = TempFileFactory.CreateTempWmaFile();
				WmaFileSink wmaFileSink = new WmaFileSink(this.diagnosticTmpWma.FilePath);
				wmaFileSink.EncodingFormat = 2;
				this.diagnosticRecorder.SetSink(wmaFileSink);
				this.diagnosticRecorder.Start();
			}

			private void StopCommandAndControlDiagnosticRecording(RecognizeCompletedEventArgs result)
			{
				this.diagnosticRecorder.Stop();
				FileInfo fileInfo = new FileInfo(this.diagnosticTmpWma.FilePath);
				if (fileInfo.Exists && fileInfo.Length > 0L)
				{
					using (ITempFile tempFile = MediaMethods.ToPcm(this.diagnosticTmpWma))
					{
						this.SetFinalFileAndLogRecoResults(tempFile, result);
					}
				}
			}

			private void SetFinalFileAndLogRecoResults(ITempFile wav, RecognizeCompletedEventArgs recoEvent)
			{
				string arg = string.Empty;
				try
				{
					arg = Path.Combine(this.diagnosticCallIdFolder, string.Format("{0}_{1}", base.Session.CommandAndControlLoggingCounter++, base.PlaybackInfo.TurnName));
				}
				catch (ArgumentException)
				{
					arg = Path.Combine(this.diagnosticCallIdFolder, string.Format("{0}_{1}", base.Session.CommandAndControlLoggingCounter++, Guid.NewGuid().ToString()));
				}
				string text = string.Format("{0}.wma", arg);
				string path = string.Format("{0}.txt", arg);
				try
				{
					File.Delete(text);
					File.Move(wav.FilePath, text);
					StringBuilder stringBuilder = new StringBuilder();
					if (recoEvent.Error != null)
					{
						stringBuilder.AppendLine(string.Format("Recognized Error: {0}", recoEvent.Error.Message)).AppendLine();
					}
					RecognitionResult result = recoEvent.Result;
					if (result == null)
					{
						stringBuilder.AppendLine("Recognition Result is null").AppendLine();
					}
					else
					{
						stringBuilder.AppendLine(string.Format("Recognized Phrase:{0}", result.Text));
						stringBuilder.AppendLine(string.Format("Confidence:{0}", result.Confidence)).AppendLine();
						stringBuilder.Append("Recognized WordUnit[Word,Confidence]:");
						foreach (RecognizedWordUnit recognizedWordUnit in result.Words)
						{
							stringBuilder.Append(string.Format("[{0},{1}] ", recognizedWordUnit.Text, recognizedWordUnit.Confidence));
						}
						stringBuilder.AppendLine().AppendLine();
						stringBuilder.Append("Alternates[Word,Confidence]:");
						foreach (RecognizedPhrase recognizedPhrase in result.Alternates)
						{
							stringBuilder.Append(string.Format("[{0},{1}]", recognizedPhrase.Text, recognizedPhrase.Confidence));
						}
						stringBuilder.AppendLine().AppendLine();
						stringBuilder.AppendLine(string.Format("Recognized Grammar:[RuleName:{0}, Weight:{1} , Loaded:{2}, Priority:{3}]", new object[]
						{
							result.Grammar.RuleName,
							result.Grammar.Weight,
							result.Grammar.Loaded,
							result.Grammar.Priority
						})).AppendLine();
					}
					stringBuilder.AppendLine("Loaded Grammars:");
					foreach (UMGrammar umgrammar in base.PlaybackInfo.Grammars)
					{
						stringBuilder.AppendLine(umgrammar.ToString());
					}
					using (StreamWriter streamWriter = new StreamWriter(path))
					{
						streamWriter.Write(stringBuilder.ToString());
					}
				}
				catch (Exception ex)
				{
					base.Diag.Trace("Ignoring Exception since these generated files are for diagnostic purposes, e ='{0}'", new object[]
					{
						ex
					});
				}
			}

			private readonly string diagnosticCallIdFolder;

			private readonly Recorder diagnosticRecorder;

			private ITempFile diagnosticTmpWma;
		}

		private class DeclineSessionState : UcmaCallSession.SessionState
		{
			public DeclineSessionState(UcmaCallSession session, PlatformSignalingHeader diagnosticHeader) : base(session)
			{
				this.diagnosticHeader = diagnosticHeader;
			}

			public override string Name
			{
				get
				{
					return "DeclineSessionState";
				}
			}

			protected override void InternalStart()
			{
				CallDeclineOptions callDeclineOptions = new CallDeclineOptions(403);
				callDeclineOptions.Headers.Add(new SignalingHeader(this.diagnosticHeader.Name, this.diagnosticHeader.Value));
				base.Diag.Trace("DeclineSessionState.InternalStart: DiagnosticHeader:{0}", new object[]
				{
					this.diagnosticHeader.Value
				});
				base.Session.Ucma.Call.Decline(callDeclineOptions);
				base.Session.DisconnectCall(this.diagnosticHeader);
			}

			protected override void ForceAsyncWaitingCompletions()
			{
			}

			protected override void CompleteFinalAsyncCallback()
			{
			}

			protected override DisposeTracker InternalGetDisposeTracker()
			{
				return DisposeTracker.Get<UcmaCallSession.DeclineSessionState>(this);
			}

			private PlatformSignalingHeader diagnosticHeader;
		}

		private class EstablishCallSessionState : UcmaCallSession.InitializeFlowSessionState
		{
			public EstablishCallSessionState(UcmaCallSession session, BaseUMCallSession.OutboundCallInfo info, IList<PlatformSignalingHeader> headers) : base(session)
			{
				this.info = info;
				this.headers = headers;
				Conversation conversation = new Conversation(base.Session.LocalEndpoint);
				this.SetCallingLineId(conversation);
				this.outboundCall = new AudioVideoCall(conversation);
				base.Session.AssociateCall(this.outboundCall);
			}

			public override string Name
			{
				get
				{
					return "EstablishCallSessionState";
				}
			}

			public override void Call_EstablishCompleted(IAsyncResult r)
			{
				try
				{
					base.StopWaitFor(UcmaCallSession.SessionState.AsyncOperation.CallEstablished);
					this.InitializeOutboundCallContext(this.outboundCall.EndEstablish(r));
				}
				catch (InvalidOperationException ex)
				{
					this.outboundCallError = ex;
				}
				catch (RealTimeException ex2)
				{
					this.outboundCallError = ex2;
				}
				finally
				{
					if (this.outboundCallError != null)
					{
						base.Diag.Trace("Call.EndEstablish failed. e='{0}'", new object[]
						{
							this.outboundCallError
						});
						this.ForceAsyncWaitingCompletions();
					}
				}
			}

			public override void Call_TerminateCompleted(IAsyncResult r)
			{
				base.StopWaitFor(UcmaCallSession.SessionState.AsyncOperation.TerminateComplete);
			}

			public void CancelOutboundCall()
			{
				this.outboundCallError = new OutboundCallCancelledException();
				this.ForceAsyncWaitingCompletions();
				AsyncCallback asyncCallback = base.EventSubscriber.CreateSerializedAsyncCallback(delegate(IAsyncResult r)
				{
					base.Session.CurrentState.Call_TerminateCompleted(r);
				}, "Call_TerminateCompleted");
				this.outboundCall.BeginTerminate(asyncCallback, this);
				base.StartWaitFor(UcmaCallSession.SessionState.AsyncOperation.TerminateComplete);
			}

			protected override void InternalStart()
			{
				UcmaPlatform.ValidateRealTimeUri(this.info.CalledParty);
				AsyncCallback asyncCallback = base.EventSubscriber.CreateSerializedAsyncCallback(delegate(IAsyncResult r)
				{
					base.Session.CurrentState.Call_EstablishCompleted(r);
				}, "Call_EstablishCompleted");
				CallEstablishOptions callEstablishOptions = this.CreateCallEstablishOptions();
				this.outboundCall.BeginEstablish(this.info.CalledParty, callEstablishOptions, asyncCallback, this);
				base.StartWaitFor(UcmaCallSession.SessionState.AsyncOperation.CallEstablished);
				base.InternalStart();
			}

			protected override void ForceAsyncWaitingCompletions()
			{
				base.ForceAsyncWaitingCompletions();
			}

			protected override void CompleteFinalAsyncCallback()
			{
				this.SetOutboundCallArgs();
				base.Session.Fire<OutboundCallDetailsEventArgs>(base.Session.OnOutboundCallRequestCompleted, this.outboundEventArgs);
			}

			protected override DisposeTracker InternalGetDisposeTracker()
			{
				return DisposeTracker.Get<UcmaCallSession.EstablishCallSessionState>(this);
			}

			private void SetCallingLineId(Conversation c)
			{
				try
				{
					if (!string.IsNullOrEmpty(this.info.CallingParty))
					{
						c.Impersonate(this.info.CallingParty, null, null);
						base.Diag.Trace("Outbound CLID set to '{0}'", new object[]
						{
							this.info.CallingParty
						});
					}
				}
				catch (ArgumentException ex)
				{
					base.Diag.Trace("Exception in SetCallingLineId '{0}'", new object[]
					{
						ex
					});
				}
			}

			private CallEstablishOptions CreateCallEstablishOptions()
			{
				CallEstablishOptions callEstablishOptions = new CallEstablishOptions();
				callEstablishOptions.ConnectionContext = UcmaUtils.CreateConnectionContext(this.info.Gateway.NextHopForOutboundRouting.Address.ToString(), this.info.Gateway.NextHopForOutboundRouting.Port);
				callEstablishOptions.ConnectionContext.AddressFamilyHint = new AddressFamilyHint?(UcmaUtils.MapIPAddressFamilyToHint(this.info.Gateway.IPAddressFamily));
				base.Diag.Trace("CreateCallEstablishOptions: using IPAddressFamily {0}", new object[]
				{
					this.info.Gateway.IPAddressFamily
				});
				if (this.headers != null)
				{
					foreach (PlatformSignalingHeader platformSignalingHeader in this.headers)
					{
						string transferor;
						if (base.RequiresSigningAsTransferor(true, platformSignalingHeader, out transferor))
						{
							callEstablishOptions.Transferor = transferor;
						}
						else
						{
							callEstablishOptions.Headers.Add(new SignalingHeader(platformSignalingHeader.Name, platformSignalingHeader.Value));
						}
					}
				}
				return callEstablishOptions;
			}

			private void InitializeOutboundCallContext(CallMessageData data)
			{
				ValidateArgument.NotNull(data, "data");
				UcmaCallInfo callInfo = new UcmaCallInfo(data, this.outboundCall.Conversation, this.info.Gateway.Address.Address);
				base.Session.CallInfo = callInfo;
			}

			private void SetOutboundCallArgs()
			{
				this.SetErrorIfNotConnected();
				if (this.outboundCallError == null && base.FlowActivated)
				{
					this.outboundEventArgs = this.CreateOutboundArgsForSuccess();
					return;
				}
				this.outboundEventArgs = this.CreateOutboundArgsForFailure(this.outboundCallError);
			}

			private void SetErrorIfNotConnected()
			{
				if (this.outboundCallError == null)
				{
					UMCallState state = base.Session.State;
					if (state != UMCallState.Connected)
					{
						base.Diag.Trace("CallState is not connected.  State={0}", new object[]
						{
							state
						});
						this.outboundCallError = new EstablishCallFailureException(string.Empty);
					}
				}
			}

			private OutboundCallDetailsEventArgs CreateOutboundArgsForSuccess()
			{
				UMCallInfoEx exInfo = new UMCallInfoEx
				{
					CallState = UMCallState.Connected,
					EndResult = UMOperationResult.Success
				};
				return new OutboundCallDetailsEventArgs(null, exInfo, null);
			}

			private OutboundCallDetailsEventArgs CreateOutboundArgsForFailure(Exception e)
			{
				if (this.outboundCallError != null)
				{
					base.Session.LogOutboundCallFailed(this.info, e.Message, e.Message);
				}
				return new OutboundCallDetailsEventArgs(e, this.CreateCallInfoEx(e), null);
			}

			private UMCallInfoEx CreateCallInfoEx(Exception e)
			{
				FailureResponseException ex = e as FailureResponseException;
				int num = 100;
				while (e != null && ex == null && --num > 0)
				{
					ex = (e as FailureResponseException);
					e = e.InnerException;
				}
				base.Diag.Assert(num > 0);
				UMEventCause eventCause = UMEventCause.None;
				int num2 = 0;
				string responseText = string.Empty;
				if (ex != null)
				{
					num2 = ex.ResponseData.ResponseCode;
					responseText = ex.ResponseData.ResponseText;
					eventCause = BaseUMCallSession.GetUMEventCause(num2);
				}
				return new UMCallInfoEx
				{
					CallState = UMCallState.Disconnected,
					EndResult = UMOperationResult.Failure,
					EventCause = eventCause,
					ResponseCode = num2,
					ResponseText = responseText
				};
			}

			private AudioVideoCall outboundCall;

			private BaseUMCallSession.OutboundCallInfo info;

			private IList<PlatformSignalingHeader> headers;

			private Exception outboundCallError;

			private OutboundCallDetailsEventArgs outboundEventArgs;
		}

		private class ErrorSessionState : UcmaCallSession.SessionState
		{
			public ErrorSessionState(UcmaCallSession session, Exception e) : base(session)
			{
				base.Args.Error = e;
			}

			public override string Name
			{
				get
				{
					return "ErrorSessionState";
				}
			}

			protected override void InternalStart()
			{
				if (base.Session.IsInvalidOperationExplainable && base.Args.Error is InvalidOperationException)
				{
					base.Diag.Trace("Ignoring IOP with disconnected call.  Tearing down.  e='{0}'", new object[]
					{
						base.Args.Error
					});
					base.Session.ChangeState(new UcmaCallSession.TeardownSessionState(base.Session, UcmaCallSession.DisconnectType.Remote, null));
					return;
				}
				if (base.Session.OnError == null)
				{
					base.Diag.Trace("Tearing down the call session because the application has not yet subscribed to the error event handler.  e={0}", new object[]
					{
						base.Args.Error
					});
					base.Session.ChangeState(new UcmaCallSession.TeardownSessionState(base.Session, UcmaCallSession.DisconnectType.Remote, null));
					return;
				}
				base.Session.Fire<UMCallSessionEventArgs>(base.Session.OnError, base.Args);
			}

			protected override void ForceAsyncWaitingCompletions()
			{
			}

			protected override void CompleteFinalAsyncCallback()
			{
			}

			protected override DisposeTracker InternalGetDisposeTracker()
			{
				return DisposeTracker.Get<UcmaCallSession.ErrorSessionState>(this);
			}
		}

		private class HeavyBlockingOperationSessionState : UcmaCallSession.SessionState
		{
			public HeavyBlockingOperationSessionState(UcmaCallSession session, IUMHeavyBlockingOperation operation, ArrayList prompts) : base(session)
			{
				UcmaCallSession.HeavyBlockingOperationSessionState <>4__this = this;
				this.prompts = prompts;
				this.hboEventArgs = new HeavyBlockingOperationEventArgs(operation);
				this.hbo = delegate()
				{
					using (new CallId(<>4__this.Session.CallId))
					{
						operation.Execute();
					}
				};
			}

			public override string Name
			{
				get
				{
					return "HeavingBlockingOperationSessionState";
				}
			}

			public override void Player_SpeakCompleted(object sender, PromptPlayer.PlayerCompletedEventArgs e)
			{
				base.StopWaitFor(UcmaCallSession.SessionState.AsyncOperation.PromptsComplete);
				if (!e.Cancelled)
				{
					this.hboEventArgs.CompletionType = HeavyBlockingOperationCompletionType.Timeout;
				}
			}

			protected override void ForceAsyncWaitingCompletions()
			{
				base.Session.Ucma.Player.Cancel();
			}

			protected override void CompleteFinalAsyncCallback()
			{
				base.Diag.Trace("HBO Completion: {0}", new object[]
				{
					this.hboEventArgs.CompletionType
				});
				if (this.hboEventArgs.CompletionType == HeavyBlockingOperationCompletionType.Success)
				{
					base.Session.Fire<HeavyBlockingOperationEventArgs>(base.Session.OnHeavyBlockingOperation, this.hboEventArgs);
					return;
				}
				base.Session.ChangeState(new UcmaCallSession.TeardownSessionState(base.Session, UcmaCallSession.DisconnectType.Local, null));
			}

			protected override DisposeTracker InternalGetDisposeTracker()
			{
				return DisposeTracker.Get<UcmaCallSession.HeavyBlockingOperationSessionState>(this);
			}

			protected override void InternalStart()
			{
				this.StartAudioHourglass();
				this.StartOperation();
			}

			private void EndOperation(IAsyncResult r)
			{
				try
				{
					base.StopWaitFor(UcmaCallSession.SessionState.AsyncOperation.HeavyBlockingComplete);
					this.ForceAsyncWaitingCompletions();
					this.hboEventArgs.Latency = DateTime.UtcNow - this.operationStartTimeUtc;
					if (this.hboEventArgs.CompletionType == HeavyBlockingOperationCompletionType.None)
					{
						this.hboEventArgs.CompletionType = HeavyBlockingOperationCompletionType.Success;
					}
					this.UpdatePerformanceCounters();
					this.hbo.EndInvoke(r);
				}
				catch (Exception error)
				{
					this.hboEventArgs.Error = error;
					throw;
				}
			}

			private void StartAudioHourglass()
			{
				if (!base.Session.IsMediaTerminatingOrTerminated)
				{
					base.Session.Ucma.Player.Play(this.prompts, base.Session.CurrentCallContext.Culture, TimeSpan.Zero);
					base.StartWaitFor(UcmaCallSession.SessionState.AsyncOperation.PromptsComplete);
				}
			}

			private void StartOperation()
			{
				this.operationStartTimeUtc = DateTime.UtcNow;
				AsyncCallback callback = base.EventSubscriber.CreateSerializedAsyncCallback(delegate(IAsyncResult r)
				{
					this.EndOperation(r);
				}, "EndOperation", true);
				this.hbo.BeginInvoke(callback, this);
				base.StartWaitFor(UcmaCallSession.SessionState.AsyncOperation.HeavyBlockingComplete);
			}

			private void UpdatePerformanceCounters()
			{
				if (this.hboEventArgs.Latency > Constants.HeavyBlockingOperationDelay)
				{
					if (base.Session.IsSignalingTerminatingOrTerminated)
					{
						base.Session.CurrentCallContext.IncrementHungUpAfterDelayCounter();
					}
					if (!base.Session.CurrentCallContext.IsDelayedCallsCounterIncremented)
					{
						base.Session.IncrementCounter(GeneralCounters.DelayedCalls);
					}
				}
				base.Session.SetCounter(GeneralCounters.UserResponseLatency, BaseUMCallSession.AverageUserResponseLatency.Update(this.hboEventArgs.Latency.TotalMilliseconds));
			}

			private UcmaCallSession.HeavyBlockingOperationSessionState.HboDelegate hbo;

			private HeavyBlockingOperationEventArgs hboEventArgs;

			private DateTime operationStartTimeUtc;

			private ArrayList prompts;

			private delegate void HboDelegate();
		}

		private class IdleSessionState : UcmaCallSession.SessionState
		{
			public IdleSessionState(UcmaCallSession session) : base(session)
			{
			}

			public override string Name
			{
				get
				{
					return "IdleCallState";
				}
			}

			protected override void InternalStart()
			{
				base.Diag.Trace("Session is now idle.", new object[0]);
			}

			protected override void ForceAsyncWaitingCompletions()
			{
			}

			protected override void CompleteFinalAsyncCallback()
			{
			}

			protected override DisposeTracker InternalGetDisposeTracker()
			{
				return DisposeTracker.Get<UcmaCallSession.IdleSessionState>(this);
			}
		}

		private class PlayPromptsAndRecoEmulateSessionState : UcmaCallSession.PlayPromptsAndRecoSpeechSessionState
		{
			public PlayPromptsAndRecoEmulateSessionState(UcmaCallSession session, UcmaCallSession.PlaybackInfo promptInfo) : base(session, promptInfo)
			{
			}

			public override string Name
			{
				get
				{
					return "PlayPromptsAndRecoEmulateSessionState";
				}
			}

			public override void SpeechReco_RecognizeCompleted(object sender, RecognizeCompletedEventArgs e)
			{
				base.StopWaitFor(UcmaCallSession.SessionState.AsyncOperation.SpeechEmulateComplete);
				base.SpeechReco_RecognizeCompleted(sender, e);
			}

			public override void SpeechReco_EmulateRecognizeCompleted(object sender, EmulateRecognizeCompletedEventArgs e)
			{
				base.StopWaitFor(UcmaCallSession.SessionState.AsyncOperation.SpeechEmulateComplete);
				this.ForceAsyncWaitingCompletions();
				base.Session.Ucma.HaltRecognition();
				if (e.Error != null)
				{
					base.Args.Error = e.Error;
					return;
				}
				if (!e.Cancelled)
				{
					base.Diag.Trace("EmulateRecognizedCompleted.  result='{0}'", new object[]
					{
						e.Result
					});
					this.fireOnSpeech = true;
					if (e.Result != null && UcmaCallSession.PlayPromptsAndRecoSpeechSessionState.TestSemanticAccess(e.Result))
					{
						this.speechArgs.Result = new UcmaRecognitionResult(e.Result);
					}
				}
			}

			protected override void StartRecognizer()
			{
				base.Session.Ucma.EnsureRecognition(base.Session.CurrentCallContext.Culture);
			}

			public void RecognizeWave(string file)
			{
				base.StartWaitFor(UcmaCallSession.SessionState.AsyncOperation.SpeechEmulateComplete);
				base.Session.Ucma.SpeechReco.SetInputToWaveFile(file);
				base.Session.Ucma.SpeechReco.RecognizeAsync();
			}

			public void EmulateWave(string text)
			{
				base.StartWaitFor(UcmaCallSession.SessionState.AsyncOperation.SpeechEmulateComplete);
				base.Session.Ucma.SpeechReco.EmulateRecognizeAsync(text);
			}
		}

		internal class PlaybackInfo
		{
			public ArrayList Prompts { get; set; }

			public TimeSpan InitialSilenceTimeout { get; set; }

			public int MinDigits { get; set; }

			public int MaxDigits { get; set; }

			public TimeSpan Timeout { get; set; }

			public TimeSpan Offset { get; set; }

			public TimeSpan InterDigitTimeout { get; set; }

			public string StopTones { get; set; }

			public StopPatterns StopPatterns { get; set; }

			public bool UnconditionalBargeIn { get; set; }

			public bool Uninterruptable { get; set; }

			public string TurnName { get; set; }

			public List<UMGrammar> Grammars { get; set; }

			public TimeSpan BabbleTimeout { get; set; }
		}

		internal class RecordInfo
		{
			public string FileName { get; set; }

			public TimeSpan EndSilenceTimeout { get; set; }

			public TimeSpan MaxDuration { get; set; }

			public string StopTones { get; set; }

			public bool Append { get; set; }
		}

		private class RecordFileSessionState : UcmaCallSession.SessionState
		{
			static RecordFileSessionState()
			{
				FilePrompt value = new FilePrompt("Beep.wav", CommonConstants.DefaultCulture);
				UcmaCallSession.RecordFileSessionState.beepFiles.Add(value);
			}

			public RecordFileSessionState(UcmaCallSession session, UcmaCallSession.RecordInfo r) : base(session)
			{
				this.recordInfo = r;
				this.tmpWma = TempFileFactory.CreateTempWmaFile();
				this.isSilenceTimerAllowed = true;
				this.InitializeEventArgs();
				this.completionReason = UcmaCallSession.RecordFileSessionState.CompletionReason.None;
				this.SetRecordingFormat();
				this.recordInfo.MaxDuration = UcmaCallSession.SessionState.CalculateMinTimerDueTime(this.recordInfo.MaxDuration - base.Args.TotalRecordTime, TimeSpan.MaxValue);
			}

			internal string TestInfoRecordedFileName { get; set; }

			public override string Name
			{
				get
				{
					return "RecordFileSessionState";
				}
			}

			public override void Player_SpeakCompleted(object sender, PromptPlayer.PlayerCompletedEventArgs e)
			{
				base.StopWaitFor(UcmaCallSession.SessionState.AsyncOperation.BeepComplete);
			}

			public override void MediaRecorder_StateChanged(object sender, RecorderStateChangedEventArgs e)
			{
				base.Diag.Trace(UcmaCallSession.FormatStateChange<RecorderState>("MediaRecorder", e.PreviousState, e.State), new object[0]);
				if (e.State == 1)
				{
					base.StopWaitFor(UcmaCallSession.SessionState.AsyncOperation.RecordComplete);
					this.UpdateCompletionReason(UcmaCallSession.RecordFileSessionState.CompletionReason.MediaEnded);
					this.FinalizeFile();
				}
			}

			public override void MediaRecorder_VoiceActivityChanged(object sender, VoiceActivityChangedEventArgs e)
			{
				base.Diag.Trace("MediaRecorder_VoiceActivityChanged Voice={0} Time={1} Silence Timer Allowed={2}", new object[]
				{
					e.IsVoice,
					e.TimeStamp,
					this.isSilenceTimerAllowed
				});
				this.lastVoiceActivity = e;
				if (e.IsVoice)
				{
					base.StopWaitFor(UcmaCallSession.SessionState.AsyncOperation.VoiceActivityTimer);
					this.voiceActivityTimer.Change(-1, 0);
					this.userSpoke = true;
					return;
				}
				if (this.isSilenceTimerAllowed)
				{
					this.voiceActivityTimer.Change(this.recordInfo.EndSilenceTimeout, TimeSpan.Zero);
					base.StartWaitFor(UcmaCallSession.SessionState.AsyncOperation.VoiceActivityTimer);
				}
			}

			public override void ToneController_ToneReceived(object sender, ToneControllerEventArgs e)
			{
				base.Args.DtmfDigits = new byte[]
				{
					UcmaCallSession.ToneToByte((byte)e.Tone)
				};
				this.UpdateCompletionReason(UcmaCallSession.RecordFileSessionState.CompletionReason.Tone);
				this.ForceAsyncWaitingCompletions();
			}

			protected override void OnHoldNotify()
			{
				if (this.voiceActivityTimer != null)
				{
					base.StopWaitFor(UcmaCallSession.SessionState.AsyncOperation.VoiceActivityTimer);
					this.voiceActivityTimer.Change(-1, 0);
				}
			}

			protected override void OnResumeNotify()
			{
				if (this.voiceActivityTimer != null && this.isSilenceTimerAllowed)
				{
					this.voiceActivityTimer.Change(this.recordInfo.EndSilenceTimeout, TimeSpan.Zero);
					base.StartWaitFor(UcmaCallSession.SessionState.AsyncOperation.VoiceActivityTimer);
				}
			}

			protected override void InternalStart()
			{
				base.Session.ToneAccumulator.Clear();
				this.StartPlayBeep();
				this.StartRecording();
				this.StartTimers();
			}

			protected override DisposeTracker InternalGetDisposeTracker()
			{
				return DisposeTracker.Get<UcmaCallSession.RecordFileSessionState>(this);
			}

			protected override void ForceAsyncWaitingCompletions()
			{
				base.StopWaitFor(UcmaCallSession.SessionState.AsyncOperation.MaxRecordingTimer);
				base.StopWaitFor(UcmaCallSession.SessionState.AsyncOperation.VoiceActivityTimer);
				if (base.IsWaitingFor(UcmaCallSession.SessionState.AsyncOperation.BeepComplete))
				{
					base.Session.Ucma.Player.Cancel();
				}
				if (base.IsWaitingFor(UcmaCallSession.SessionState.AsyncOperation.RecordComplete))
				{
					base.Session.Ucma.MediaRecorder.Stop();
				}
				this.isSilenceTimerAllowed = false;
			}

			protected override void CompleteFinalAsyncCallback()
			{
				switch (this.completionReason)
				{
				case UcmaCallSession.RecordFileSessionState.CompletionReason.Tone:
					base.Session.Fire<UMCallSessionEventArgs>(base.Session.OnDtmf, base.Args);
					return;
				case UcmaCallSession.RecordFileSessionState.CompletionReason.Timeout:
					base.Session.Fire<UMCallSessionEventArgs>(base.Session.OnComplete, base.Args);
					return;
				case UcmaCallSession.RecordFileSessionState.CompletionReason.Silence:
					base.Session.Fire<UMCallSessionEventArgs>(base.Session.OnComplete, base.Args);
					return;
				case UcmaCallSession.RecordFileSessionState.CompletionReason.MediaEnded:
					base.Session.Fire<UMCallSessionEventArgs>(base.Session.OnHangup, base.Args);
					return;
				default:
					return;
				}
			}

			protected override void InternalDispose(bool disposing)
			{
				try
				{
					if (this.maxRecordingtimer != null)
					{
						this.maxRecordingtimer.Dispose();
					}
					if (this.voiceActivityTimer != null)
					{
						this.voiceActivityTimer.Dispose();
					}
					if (this.tmpWma != null)
					{
						this.tmpWma.Dispose();
					}
				}
				finally
				{
					base.InternalDispose(disposing);
				}
			}

			private void MaxRecordingTimer_Expired(object state)
			{
				if (base.IsWaitingFor(UcmaCallSession.SessionState.AsyncOperation.MaxRecordingTimer))
				{
					base.StopWaitFor(UcmaCallSession.SessionState.AsyncOperation.MaxRecordingTimer);
					this.UpdateCompletionReason(UcmaCallSession.RecordFileSessionState.CompletionReason.Timeout);
					this.ForceAsyncWaitingCompletions();
				}
			}

			private void VoiceActivityTimer_Expired(object state)
			{
				if (base.IsWaitingFor(UcmaCallSession.SessionState.AsyncOperation.VoiceActivityTimer))
				{
					base.StopWaitFor(UcmaCallSession.SessionState.AsyncOperation.VoiceActivityTimer);
					this.UpdateCompletionReason(UcmaCallSession.RecordFileSessionState.CompletionReason.Silence);
					this.ForceAsyncWaitingCompletions();
				}
			}

			private void StartPlayBeep()
			{
				base.Session.Ucma.Player.Play(UcmaCallSession.RecordFileSessionState.beepFiles, CommonConstants.DefaultCulture, TimeSpan.Zero);
				base.StartWaitFor(UcmaCallSession.SessionState.AsyncOperation.BeepComplete);
			}

			private void StartRecording()
			{
				WmaFileSink wmaFileSink = new WmaFileSink(this.tmpWma.FilePath);
				wmaFileSink.EncodingFormat = this.format;
				base.Session.Ucma.MediaRecorder.SetSink(wmaFileSink);
				base.Session.Ucma.MediaRecorder.Start();
				base.StartWaitFor(UcmaCallSession.SessionState.AsyncOperation.RecordComplete);
			}

			private void StartTimers()
			{
				this.StartMaxRecordingTimer();
				this.StartVoiceActivityTimer();
			}

			private void StartMaxRecordingTimer()
			{
				this.maxRecordingtimer = new Timer(base.EventSubscriber.CreateSerializedTimerCallback(delegate(object r)
				{
					this.MaxRecordingTimer_Expired(r);
				}, "MaxRecordingTimer_Expired"), this, this.recordInfo.MaxDuration, TimeSpan.Zero);
				base.StartWaitFor(UcmaCallSession.SessionState.AsyncOperation.MaxRecordingTimer);
			}

			private void StartVoiceActivityTimer()
			{
				this.voiceActivityTimer = new Timer(base.EventSubscriber.CreateSerializedTimerCallback(delegate(object r)
				{
					this.VoiceActivityTimer_Expired(r);
				}, "VoiceActivityTimer_Expired"), this, this.recordInfo.EndSilenceTimeout, TimeSpan.Zero);
				base.StartWaitFor(UcmaCallSession.SessionState.AsyncOperation.VoiceActivityTimer);
			}

			private void UpdateCompletionReason(UcmaCallSession.RecordFileSessionState.CompletionReason reason)
			{
				if (this.completionReason == UcmaCallSession.RecordFileSessionState.CompletionReason.None)
				{
					this.completionReason = reason;
				}
			}

			private void FinalizeFile()
			{
				if (!base.Session.TestInfo.IsFeatureEnabled(UcmaCallSession.TestInfoFeatures.PlayAudio) || string.IsNullOrEmpty(this.TestInfoRecordedFileName))
				{
					if (this.userSpoke)
					{
						FileInfo fileInfo = new FileInfo(this.tmpWma.FilePath);
						if (!fileInfo.Exists || fileInfo.Length <= 0L)
						{
							return;
						}
						using (ITempFile tempFile = MediaMethods.ToPcm(this.tmpWma))
						{
							using (ITempFile tempFile2 = this.CreateFinalFile(tempFile))
							{
								this.SetFinalFile(tempFile2);
							}
							return;
						}
					}
					Util.TryDeleteFile(this.tmpWma.FilePath);
					return;
				}
				File.Delete(this.recordInfo.FileName);
				File.Copy(this.TestInfoRecordedFileName, this.recordInfo.FileName, true);
				base.Args.RecordTime = this.CalculateRecordingTime(this.recordInfo.FileName);
				base.Args.TotalRecordTime += base.Args.RecordTime;
			}

			private void InitializeEventArgs()
			{
				base.Args.TotalRecordTime = TimeSpan.Zero;
				if (this.recordInfo.Append)
				{
					base.Args.TotalRecordTime = this.CalculateRecordingTime(this.recordInfo.FileName);
				}
			}

			private TimeSpan CalculateRecordingTime(string filename)
			{
				TimeSpan result = TimeSpan.Zero;
				using (DisposeGuard disposeGuard = default(DisposeGuard))
				{
					if (File.Exists(filename))
					{
						SoundReader soundReader = null;
						if (PcmReader.TryCreate(filename, out soundReader))
						{
							disposeGuard.Add<SoundReader>(soundReader);
							double num = (double)soundReader.WaveDataLength;
							double num2 = (double)((int)soundReader.WaveFormat.BitsPerSample * soundReader.WaveFormat.SamplesPerSec / 8);
							result = TimeSpan.FromSeconds(num / num2);
						}
					}
				}
				return result;
			}

			private ITempFile CreateFinalFile(ITempFile newRecording)
			{
				ITempFile tempFile = newRecording;
				this.TrimEndSilence(tempFile);
				base.Args.RecordTime = this.CalculateRecordingTime(tempFile.FilePath);
				base.Args.TotalRecordTime += base.Args.RecordTime;
				if (this.recordInfo.Append)
				{
					tempFile = this.AppendToExistingRecording(tempFile);
				}
				return tempFile;
			}

			private void TrimEndSilence(ITempFile file)
			{
				if (this.lastVoiceActivity != null && !this.lastVoiceActivity.IsVoice)
				{
					TimeSpan t = this.CalculateRecordingTime(file.FilePath);
					TimeSpan timeSpan = t - this.lastVoiceActivity.TimeStamp;
					if (timeSpan > UcmaCallSession.RecordFileSessionState.VoiceActivityDetectionLatency)
					{
						base.Diag.Trace("Trimming {0} ms of end silence from file {1} whose original length was {2} ms", new object[]
						{
							timeSpan.TotalMilliseconds,
							file.FilePath,
							t.TotalMilliseconds
						});
						MediaMethods.RemoveAudioFromEnd(file.FilePath, timeSpan);
					}
				}
			}

			private ITempFile AppendToExistingRecording(ITempFile newRecording)
			{
				SoundReader soundReader = null;
				ITempFile tempFile = newRecording;
				using (DisposeGuard disposeGuard = default(DisposeGuard))
				{
					if (!string.IsNullOrEmpty(this.recordInfo.FileName) && PcmReader.TryCreate(this.recordInfo.FileName, out soundReader))
					{
						disposeGuard.Add<SoundReader>(soundReader);
						SoundReader soundReader2 = new PcmReader(newRecording.FilePath);
						disposeGuard.Add<SoundReader>(soundReader2);
						tempFile = TempFileFactory.CreateTempWavFile();
						PcmWriter pcmWriter = new PcmWriter(tempFile.FilePath, soundReader.WaveFormat);
						disposeGuard.Add<PcmWriter>(pcmWriter);
						MediaMethods.Append(soundReader, soundReader2, pcmWriter);
					}
				}
				return tempFile;
			}

			private void SetFinalFile(ITempFile wav)
			{
				File.Delete(this.recordInfo.FileName);
				File.Move(wav.FilePath, this.recordInfo.FileName);
			}

			private void SetRecordingFormat()
			{
				this.format = 2;
				AudioChannel audioChannel = null;
				if (base.Session.Ucma.Flow.Audio.GetChannels().TryGetValue(0, out audioChannel))
				{
					base.Diag.Trace("AudioChannel receive sampling rate is: {0}.", new object[]
					{
						audioChannel.ReceiveDirectionSamplingRate
					});
					if ((audioChannel.ReceiveDirectionSamplingRate & 2) != null)
					{
						this.format = 3;
					}
				}
				base.Diag.Trace("Setting the recording format to: {0}.", new object[]
				{
					this.format
				});
			}

			private static readonly TimeSpan VoiceActivityDetectionLatency = TimeSpan.FromSeconds(2.0);

			private static ArrayList beepFiles = new ArrayList();

			private UcmaCallSession.RecordInfo recordInfo;

			private UcmaCallSession.RecordFileSessionState.CompletionReason completionReason;

			private Timer maxRecordingtimer;

			private Timer voiceActivityTimer;

			private ITempFile tmpWma;

			private WmaEncodingFormat format;

			private bool userSpoke;

			private bool isSilenceTimerAllowed;

			private VoiceActivityChangedEventArgs lastVoiceActivity;

			private enum CompletionReason
			{
				None,
				Tone,
				Timeout,
				Silence,
				MediaEnded
			}
		}

		private class RedirectSessionState : UcmaCallSession.SessionState
		{
			public RedirectSessionState(UcmaCallSession session, string host, int port, int code) : base(session)
			{
				this.host = host;
				this.port = port;
				this.code = code;
			}

			public override string Name
			{
				get
				{
					return "RedirectSessionState";
				}
			}

			protected override void InternalStart()
			{
				PlatformSipUri redirectContactUri = base.Session.GetRedirectContactUri(this.host, this.port);
				CallForwardOptions callForwardOptions = new CallForwardOptions(this.code);
				base.Session.Ucma.Call.Forward(redirectContactUri.ToString(), callForwardOptions);
				base.Session.DisconnectCall(null);
			}

			protected override void ForceAsyncWaitingCompletions()
			{
			}

			protected override void CompleteFinalAsyncCallback()
			{
			}

			protected override DisposeTracker InternalGetDisposeTracker()
			{
				return DisposeTracker.Get<UcmaCallSession.RedirectSessionState>(this);
			}

			private readonly string host;

			private readonly int port;

			private readonly int code;
		}

		private class SendDtmfSessionState : UcmaCallSession.SessionState
		{
			public SendDtmfSessionState(UcmaCallSession session, string dtmfSequence, TimeSpan initialSilence) : base(session)
			{
				this.dtmfSequence = dtmfSequence;
				this.initialSilence = initialSilence;
			}

			public override string Name
			{
				get
				{
					return "SendDtmfSessionState";
				}
			}

			protected override void InternalStart()
			{
				Thread.Sleep(this.initialSilence);
				foreach (char c in this.dtmfSequence)
				{
					base.Session.Ucma.ToneController.Send(UcmaCallSession.CharToToneId(c));
				}
				base.StartWaitFor(UcmaCallSession.SessionState.AsyncOperation.SendDtmfComplete);
				WaitCallback callBack = base.EventSubscriber.CreateSerializedWaitCallback(delegate(object r)
				{
					this.FinishSendingDtmf(r);
				}, "SendDtmfSessionState_FinishSendingDtmf", true);
				base.Diag.Assert(ThreadPool.QueueUserWorkItem(callBack, this));
			}

			protected override void ForceAsyncWaitingCompletions()
			{
			}

			protected override void CompleteFinalAsyncCallback()
			{
				base.Session.Fire<UMCallSessionEventArgs>(base.Session.OnComplete, base.Args);
			}

			protected override DisposeTracker InternalGetDisposeTracker()
			{
				return DisposeTracker.Get<UcmaCallSession.SendDtmfSessionState>(this);
			}

			private void FinishSendingDtmf(object o)
			{
				base.StopWaitFor(UcmaCallSession.SessionState.AsyncOperation.SendDtmfComplete);
				base.Args.SendDtmfCompleted = true;
			}

			private readonly string dtmfSequence;

			private readonly TimeSpan initialSilence;
		}

		private class SendFsmInfoSessionState : UcmaCallSession.SessionState
		{
			public SendFsmInfoSessionState(UcmaCallSession session, string callId, string state) : base(session)
			{
				this.message = string.Format(CultureInfo.InvariantCulture, "Call-Id: {0}\r\nCall-State: {1}\r\n", new object[]
				{
					callId,
					state
				});
			}

			public override string Name
			{
				get
				{
					return "SendFsmInfoSessionState";
				}
			}

			public override void Call_SendInfoCompleted(IAsyncResult r)
			{
				base.StopWaitFor(UcmaCallSession.SessionState.AsyncOperation.SendMessageComplete);
				try
				{
					base.Session.Ucma.Call.EndSendMessage(r);
				}
				catch (FailureResponseException ex)
				{
					base.Diag.Trace("An exception is hit in sending SIP info for state change: {0}", new object[]
					{
						ex
					});
				}
			}

			public override void SendMessage(InfoMessage message)
			{
				this.infoReason = UcmaCallSession.SendFsmInfoSessionState.INFOPurpose.ProductInfo;
				base.SendMessage(message);
			}

			protected override void InternalStart()
			{
				AsyncCallback asyncCallback = base.EventSubscriber.CreateSerializedAsyncCallback(delegate(IAsyncResult r)
				{
					base.Session.CurrentState.Call_SendInfoCompleted(r);
				}, "Call_SendInfoCompleted");
				this.infoReason = UcmaCallSession.SendFsmInfoSessionState.INFOPurpose.TestStateChange;
				ContentDescription contentDescription = new ContentDescription(CommonConstants.ContentTypeTextPlain, this.message);
				base.Session.Ucma.Call.BeginSendMessage(1, contentDescription, new CallSendMessageRequestOptions(), asyncCallback, this);
				base.StartWaitFor(UcmaCallSession.SessionState.AsyncOperation.SendMessageComplete);
			}

			protected override void ForceAsyncWaitingCompletions()
			{
			}

			protected override void CompleteFinalAsyncCallback()
			{
				if (this.infoReason == UcmaCallSession.SendFsmInfoSessionState.INFOPurpose.TestStateChange)
				{
					base.Session.Fire<UMCallSessionEventArgs>(base.Session.OnStateInfoSent, base.Args);
					return;
				}
				base.Session.Fire<EventArgs>(base.Session.OnMessageSent, null);
			}

			protected override DisposeTracker InternalGetDisposeTracker()
			{
				return DisposeTracker.Get<UcmaCallSession.SendFsmInfoSessionState>(this);
			}

			private readonly string message;

			private UcmaCallSession.SendFsmInfoSessionState.INFOPurpose infoReason;

			private enum INFOPurpose
			{
				TestStateChange,
				ProductInfo
			}
		}

		private class SupervisedTransferSessionState : UcmaCallSession.SessionState
		{
			public SupervisedTransferSessionState(UcmaCallSession session, PlatformSipUri uri, IList<PlatformSignalingHeader> headers) : base(session)
			{
				this.uri = uri;
				List<SignalingHeader> list;
				if (headers != null)
				{
					list = headers.ConvertAll((PlatformSignalingHeader x) => new SignalingHeader(x.Name, x.Value));
				}
				else
				{
					list = null;
				}
				this.headers = list;
			}

			public override string Name
			{
				get
				{
					return "SupervisedTransferSessionState";
				}
			}

			public override void Call_TransferCompleted(IAsyncResult r)
			{
				base.StopWaitFor(UcmaCallSession.SessionState.AsyncOperation.TransferComplete);
				base.Session.Ucma.Call.EndTransfer(r);
			}

			protected override void InternalStart()
			{
				base.Diag.Assert(null != base.Session.DependentSessionDetails);
				base.Diag.Assert(null != base.Session.DependentSessionDetails.DependentUMCallSession);
				CallTransferOptions callTransferOptions = new CallTransferOptions(0);
				if (this.headers != null)
				{
					foreach (SignalingHeader item in this.headers)
					{
						callTransferOptions.Headers.Add(item);
					}
				}
				UcmaDependentCallSession ucmaDependentCallSession = (UcmaDependentCallSession)base.Session.DependentSessionDetails.DependentUMCallSession;
				AsyncCallback asyncCallback = base.EventSubscriber.CreateSerializedAsyncCallback(delegate(IAsyncResult r)
				{
					base.Session.CurrentState.Call_TransferCompleted(r);
				}, "Call_TransferCompleted");
				try
				{
					if (this.uri == null)
					{
						SipUriParser sipUriParser = new SipUriParser(ucmaDependentCallSession.Ucma.Call.RemoteEndpoint.Uri);
						if (sipUriParser.User != null)
						{
							base.Session.Ucma.Call.BeginTransfer(ucmaDependentCallSession.Ucma.Call, callTransferOptions, asyncCallback, this);
						}
						else
						{
							base.Session.Ucma.Call.BeginTransfer(this.GetCompleteReferTargetUri(ucmaDependentCallSession), callTransferOptions, asyncCallback, this);
						}
					}
					else
					{
						base.Session.Ucma.Call.BeginTransfer(this.GetCompleteReferTargetUri(ucmaDependentCallSession), callTransferOptions, asyncCallback, this);
						ucmaDependentCallSession.IgnoreBye = true;
					}
					base.StartWaitFor(UcmaCallSession.SessionState.AsyncOperation.TransferComplete);
				}
				catch (InvalidOperationException ex)
				{
					throw new RealTimeException(ex.Message, ex);
				}
			}

			protected override void ForceAsyncWaitingCompletions()
			{
			}

			protected override void CompleteFinalAsyncCallback()
			{
				base.Session.Fire<UMCallSessionEventArgs>(base.Session.OnTransferComplete, base.Args);
			}

			protected override DisposeTracker InternalGetDisposeTracker()
			{
				return DisposeTracker.Get<UcmaCallSession.SupervisedTransferSessionState>(this);
			}

			private string GetCompleteReferTargetUri(UcmaDependentCallSession cs)
			{
				StringBuilder stringBuilder = new StringBuilder(cs.Ucma.Call.CallId);
				stringBuilder.Append(";from-tag=");
				stringBuilder.Append(cs.Ucma.Call.LocalTag);
				stringBuilder.Append(";to-tag=");
				stringBuilder.Append(cs.Ucma.Call.RemoteTag);
				SignalingHeader signalingHeader = new SignalingHeader("Replaces", stringBuilder.ToString());
				base.Diag.Trace("GetCompleteReferTargetUri: Going to use REPLACES header : {0}", new object[]
				{
					stringBuilder
				});
				SipUriParser sipUriParser;
				if (this.uri != null)
				{
					sipUriParser = new SipUriParser(this.uri.ToString());
				}
				else
				{
					base.Diag.Trace("GetCompleteReferTargetUri: get uri from remote endpoint uri", new object[0]);
					sipUriParser = new SipUriParser(cs.Ucma.Call.RemoteEndpoint.Uri);
					if (sipUriParser.User == null)
					{
						base.Diag.Trace("GetCompleteReferTargetUri: uri user part is null - get user from OriginalDestinationUri", new object[0]);
						SipUriParser sipUriParser2 = new SipUriParser(cs.Ucma.Call.OriginalDestinationUri);
						if (sipUriParser2.User != null)
						{
							sipUriParser.User = sipUriParser2.User;
						}
						else
						{
							base.Diag.Trace("GetCompleteReferTargetUri: user part of OriginalDestinationUri is null", new object[0]);
						}
					}
				}
				sipUriParser.AddHeader(signalingHeader);
				base.Diag.Trace("GetCompleteReferTargetUri: target uri : {0}", new object[]
				{
					sipUriParser
				});
				return sipUriParser.ToString();
			}

			private PlatformSipUri uri;

			private List<SignalingHeader> headers;
		}

		private enum DisconnectType
		{
			Local,
			Remote
		}

		private class TeardownSessionState : UcmaCallSession.SessionState
		{
			public TeardownSessionState(UcmaCallSession session, UcmaCallSession.DisconnectType disconnectType, PlatformSignalingHeader diagnosticHeader) : this(session, null, disconnectType, diagnosticHeader)
			{
			}

			public TeardownSessionState(UcmaCallSession session, Exception e, UcmaCallSession.DisconnectType assumedDisconnectType, PlatformSignalingHeader diagnosticHeader) : base(session)
			{
				this.SetDisconnectType(assumedDisconnectType);
				this.SetDiagnosticHeader(diagnosticHeader);
			}

			public override string Name
			{
				get
				{
					return "TeardownSessionState";
				}
			}

			private bool CallExists
			{
				get
				{
					return base.Session.Ucma != null && null != base.Session.Ucma.Call;
				}
			}

			public override void Call_TerminateCompleted(IAsyncResult r)
			{
				base.StopWaitFor(UcmaCallSession.SessionState.AsyncOperation.TerminateComplete);
				base.Session.Ucma.Call.EndTerminate(r);
			}

			public override void Call_StateChanged(object sender, CallStateChangedEventArgs e)
			{
				if (e.State == 8)
				{
					base.StopWaitFor(UcmaCallSession.SessionState.AsyncOperation.CallStateTerminated);
				}
				base.Call_StateChanged(sender, e);
			}

			protected override void InternalStart()
			{
				base.Session.StopSerializedEvents = true;
				this.ConditionalLogDisconnectEvent();
				this.ConditionalFireOnHangup();
				this.ConditionalBeginSessionTeardown();
			}

			protected override void CompleteFinalAsyncCallback()
			{
				this.TeardownSession();
			}

			protected override void ForceAsyncWaitingCompletions()
			{
			}

			protected override DisposeTracker InternalGetDisposeTracker()
			{
				return DisposeTracker.Get<UcmaCallSession.TeardownSessionState>(this);
			}

			private void ConditionalLogDisconnectEvent()
			{
				if (this.CallExists)
				{
					if (this.disconnectType == UcmaCallSession.DisconnectType.Remote)
					{
						UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_CallEndedByUser, null, new object[]
						{
							base.Session.CallId
						});
						return;
					}
					UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_CallEndedByApplication, null, new object[]
					{
						base.Session.CallId
					});
				}
			}

			private void ConditionalFireOnHangup()
			{
				if (this.CallExists && this.disconnectType == UcmaCallSession.DisconnectType.Remote)
				{
					base.Session.Fire<UMCallSessionEventArgs>(base.Session.OnHangup, base.Args);
				}
			}

			private void TerminateCall()
			{
				base.Diag.Assert(this.CallExists, "Call must exist in TerminateCall!");
				List<SignalingHeader> list = new List<SignalingHeader>();
				list.Add(new SignalingHeader(this.diagnosticHeader.Name, this.diagnosticHeader.Value));
				AsyncCallback asyncCallback = base.EventSubscriber.CreateSerializedAsyncCallback(delegate(IAsyncResult r)
				{
					base.Session.CurrentState.Call_TerminateCompleted(r);
				}, "Call_TerminateCompleted", true, true);
				Thread.Sleep(250);
				base.Session.Ucma.Call.BeginTerminate(list, asyncCallback, this);
				base.StartWaitFor(UcmaCallSession.SessionState.AsyncOperation.TerminateComplete);
			}

			private void ConditionalBeginSessionTeardown()
			{
				if (this.CallExists)
				{
					this.TerminateCall();
					if (base.Session.LastCallStateChangeProcessed != 8)
					{
						base.StartWaitFor(UcmaCallSession.SessionState.AsyncOperation.CallStateTerminated);
						return;
					}
				}
				else
				{
					this.TeardownSession();
				}
			}

			private void TeardownSession()
			{
				if (!this.teardownComplete)
				{
					base.Session.Teardown();
					this.teardownComplete = true;
				}
			}

			private void SetDisconnectType(UcmaCallSession.DisconnectType assumedDisconnectType)
			{
				if (base.Session.CallStateTransitionReason == 7)
				{
					this.disconnectType = UcmaCallSession.DisconnectType.Local;
					return;
				}
				if (base.Session.CallStateTransitionReason == 8)
				{
					this.disconnectType = UcmaCallSession.DisconnectType.Remote;
					return;
				}
				this.disconnectType = assumedDisconnectType;
			}

			private void SetDiagnosticHeader(PlatformSignalingHeader diagnosticHeader)
			{
				if (diagnosticHeader != null)
				{
					this.diagnosticHeader = diagnosticHeader;
					return;
				}
				this.diagnosticHeader = CallRejectedException.RenderDiagnosticHeader(CallEndingReason.GraceFulTermination, null, new object[0]);
			}

			private UcmaCallSession.DisconnectType disconnectType;

			private bool teardownComplete;

			private PlatformSignalingHeader diagnosticHeader;
		}

		internal class SubscriptionHelper : DisposableBase
		{
			public SubscriptionHelper(UcmaCallSession session)
			{
				this.session = session;
				this.registry = new UcmaCallSession.SubscriptionHelper.EventRegistry();
			}

			private DiagnosticHelper Diag
			{
				get
				{
					return this.session.Diag;
				}
			}

			public void SubscribeTo(AudioVideoCall call)
			{
				call.StateChanged += this.CreateSerializedEventHandler<CallStateChangedEventArgs>(delegate(object sender, CallStateChangedEventArgs e)
				{
					this.session.CurrentState.Call_StateChanged(sender, e);
				}, delegate(EventHandler<CallStateChangedEventArgs> x)
				{
					call.StateChanged -= x;
				}, true, "Call_StateChanged");
				call.MediaTroubleshootingDataReported += this.CreateSerializedEventHandler<MediaTroubleshootingDataReportedEventArgs>(delegate(object sender, MediaTroubleshootingDataReportedEventArgs e)
				{
					this.session.CurrentState.Call_MediaTroubleshootingDataReported(sender, e);
				}, delegate(EventHandler<MediaTroubleshootingDataReportedEventArgs> x)
				{
					call.MediaTroubleshootingDataReported -= x;
				}, true, "Call_QoeDataReported");
				call.AudioVideoFlowConfigurationRequested += this.CreateSerializedEventHandler<AudioVideoFlowConfigurationRequestedEventArgs>(delegate(object sender, AudioVideoFlowConfigurationRequestedEventArgs e)
				{
					this.session.CurrentState.Call_AudioVideoFlowConfigurationRequested(sender, e);
				}, delegate(EventHandler<AudioVideoFlowConfigurationRequestedEventArgs> x)
				{
					call.AudioVideoFlowConfigurationRequested -= x;
				}, "Call_AudioVideoFlowConfigurationRequested");
				call.InfoReceived += this.CreateSerializedEventHandler<MessageReceivedEventArgs>(delegate(object sender, MessageReceivedEventArgs e)
				{
					this.session.CurrentState.Call_InfoReceived(sender, e);
				}, delegate(EventHandler<MessageReceivedEventArgs> x)
				{
					call.InfoReceived -= x;
				}, "Call_InfoReceived");
				call.RemoteParticipantChanged += this.CreateSerializedEventHandler<RemoteParticipantChangedEventArgs>(delegate(object sender, RemoteParticipantChangedEventArgs e)
				{
					this.session.CurrentState.Call_RemoteParticipantChanged(sender, e);
				}, delegate(EventHandler<RemoteParticipantChangedEventArgs> x)
				{
					call.RemoteParticipantChanged -= x;
				}, "Call_RemoteParticipantChanged");
			}

			public void SubscribeTo(AudioVideoFlow flow)
			{
				flow.StateChanged += this.CreateSerializedEventHandler<MediaFlowStateChangedEventArgs>(delegate(object xsender, MediaFlowStateChangedEventArgs xe)
				{
					this.session.CurrentState.Flow_StateChanged(xsender, xe);
				}, delegate(EventHandler<MediaFlowStateChangedEventArgs> x)
				{
					flow.StateChanged -= x;
				}, "Flow_StateChanged");
				flow.ConfigurationChanged += this.CreateSerializedEventHandler<AudioVideoFlowConfigurationChangedEventArgs>(delegate(object xsender, AudioVideoFlowConfigurationChangedEventArgs xe)
				{
					this.session.CurrentState.Flow_ConfigurationChanged(xsender, xe);
				}, delegate(EventHandler<AudioVideoFlowConfigurationChangedEventArgs> x)
				{
					flow.ConfigurationChanged -= x;
				}, "Flow_ConfigurationChanged");
			}

			public void SubscribeTo(PromptPlayer player)
			{
				player.BookmarkReached += this.CreateSerializedEventHandler<BookmarkReachedEventArgs>(delegate(object sender, BookmarkReachedEventArgs e)
				{
					this.session.CurrentState.Player_BookmarkReached(sender, e);
				}, delegate(EventHandler<BookmarkReachedEventArgs> x)
				{
					player.BookmarkReached -= x;
				}, "Player_BookmarkReached");
				player.SpeakCompleted += this.CreateSerializedEventHandler<PromptPlayer.PlayerCompletedEventArgs>(delegate(object sender, PromptPlayer.PlayerCompletedEventArgs e)
				{
					this.session.CurrentState.Player_SpeakCompleted(sender, e);
				}, delegate(EventHandler<PromptPlayer.PlayerCompletedEventArgs> x)
				{
					player.SpeakCompleted -= x;
				}, "Player_SpeakCompleted");
			}

			public void SubscribeTo(Recorder mediaRecorder)
			{
				mediaRecorder.StateChanged += this.CreateSerializedEventHandler<RecorderStateChangedEventArgs>(delegate(object sender, RecorderStateChangedEventArgs args)
				{
					this.session.CurrentState.MediaRecorder_StateChanged(sender, args);
				}, delegate(EventHandler<RecorderStateChangedEventArgs> x)
				{
					mediaRecorder.StateChanged -= x;
				}, "MediaRecorder_StateChanged");
				mediaRecorder.VoiceActivityChanged += this.CreateSerializedEventHandler<VoiceActivityChangedEventArgs>(delegate(object sender, VoiceActivityChangedEventArgs args)
				{
					this.session.CurrentState.MediaRecorder_VoiceActivityChanged(sender, args);
				}, delegate(EventHandler<VoiceActivityChangedEventArgs> x)
				{
					mediaRecorder.VoiceActivityChanged -= x;
				}, "MediaRecorder_VoiceActivityChanged");
			}

			public void SubscribeTo(ToneController toneController)
			{
				toneController.ToneReceived += this.CreateSerializedEventHandler<ToneControllerEventArgs>(delegate(object sender, ToneControllerEventArgs args)
				{
					this.session.CurrentState.ToneController_ToneReceived(sender, args);
				}, delegate(EventHandler<ToneControllerEventArgs> x)
				{
					toneController.ToneReceived -= x;
				}, "ToneController_ToneReceived");
				toneController.IncomingFaxDetected += this.CreateSerializedEventHandler<IncomingFaxDetectedEventArgs>(delegate(object sender, IncomingFaxDetectedEventArgs args)
				{
					this.session.CurrentState.ToneController_IncomingFaxDetected(sender, args);
				}, delegate(EventHandler<IncomingFaxDetectedEventArgs> x)
				{
					toneController.IncomingFaxDetected -= x;
				}, "ToneController_IncomingFaxDetected");
			}

			public void SubscribeTo(SpeechRecognitionEngine speechReco)
			{
				speechReco.RecognizeCompleted += this.CreateSerializedEventHandler<RecognizeCompletedEventArgs>(delegate(object sender, RecognizeCompletedEventArgs args)
				{
					this.session.CurrentState.SpeechReco_RecognizeCompleted(sender, args);
				}, delegate(EventHandler<RecognizeCompletedEventArgs> x)
				{
					speechReco.RecognizeCompleted -= x;
				}, "SpeechReco_RecognizeCompleted");
				speechReco.SpeechHypothesized += this.CreateSerializedEventHandler<SpeechHypothesizedEventArgs>(delegate(object sender, SpeechHypothesizedEventArgs args)
				{
					this.session.CurrentState.SpeechReco_SpeechHypothesized(sender, args);
				}, delegate(EventHandler<SpeechHypothesizedEventArgs> x)
				{
					speechReco.SpeechHypothesized -= x;
				}, "SpeechReco_SpeechHypothesized");
				speechReco.EmulateRecognizeCompleted += this.CreateSerializedEventHandler<EmulateRecognizeCompletedEventArgs>(delegate(object sender, EmulateRecognizeCompletedEventArgs args)
				{
					this.session.CurrentState.SpeechReco_EmulateRecognizeCompleted(sender, args);
				}, delegate(EventHandler<EmulateRecognizeCompletedEventArgs> x)
				{
					speechReco.EmulateRecognizeCompleted -= x;
				}, "SpeechReco_EmulateRecognizeCompleted");
				speechReco.SpeechDetected += this.CreateSerializedEventHandler<SpeechDetectedEventArgs>(delegate(object sender, SpeechDetectedEventArgs args)
				{
					this.session.CurrentState.SpeechReco_SpeechDetected(sender, args);
				}, delegate(EventHandler<SpeechDetectedEventArgs> x)
				{
					speechReco.SpeechDetected -= x;
				}, "SpeechReco_SpeechDetected");
			}

			public WaitCallback CreateSerializedWaitCallback(SerializableCallback<object> callback, string callbackTraceName, bool assertState)
			{
				return delegate(object s)
				{
					SerializableCallback<object> callback2 = assertState ? this.WrapCallbackAndAssertState<object>(callback, (UcmaCallSession.SessionState)s, callbackTraceName) : this.WrapCallbackAndIgnoreState<object>(callback, (UcmaCallSession.SessionState)s, callbackTraceName);
					this.session.Serializer.SerializeCallback<object>(s, callback2, this.session, false, callbackTraceName);
				};
			}

			public TimerCallback CreateSerializedTimerCallback(SerializableCallback<object> callback, string callbackTraceName)
			{
				return delegate(object s)
				{
					SerializableCallback<object> callback2 = this.WrapCallbackAndIgnoreState<object>(callback, (UcmaCallSession.SessionState)s, callbackTraceName);
					this.session.Serializer.SerializeCallback<object>(s, callback2, this.session, false, callbackTraceName);
				};
			}

			public AsyncCallback CreateSerializedAsyncCallback(SerializableCallback<IAsyncResult> callback, string callbackTraceName)
			{
				return this.CreateSerializedAsyncCallback(callback, callbackTraceName, true);
			}

			public AsyncCallback CreateSerializedAsyncCallback(SerializableCallback<IAsyncResult> callback, string callbackTraceName, bool assertState)
			{
				return this.CreateSerializedAsyncCallback(callback, callbackTraceName, assertState, false);
			}

			public AsyncCallback CreateSerializedAsyncCallback(SerializableCallback<IAsyncResult> callback, string callbackTraceName, bool assertState, bool forceCallback)
			{
				return delegate(IAsyncResult s)
				{
					SerializableCallback<IAsyncResult> callback2 = assertState ? this.WrapCallbackAndAssertState<IAsyncResult>(callback, (UcmaCallSession.SessionState)s.AsyncState, callbackTraceName) : this.WrapCallbackAndIgnoreState<IAsyncResult>(callback, (UcmaCallSession.SessionState)s.AsyncState, callbackTraceName);
					this.session.Serializer.SerializeCallback<IAsyncResult>(s, callback2, this.session, forceCallback, callbackTraceName);
				};
			}

			protected override DisposeTracker InternalGetDisposeTracker()
			{
				return DisposeTracker.Get<UcmaCallSession.SubscriptionHelper>(this);
			}

			protected override void InternalDispose(bool disposing)
			{
				this.registry.Dispose();
			}

			private SerializableCallback<TState> WrapCallbackAndAssertState<TState>(SerializableCallback<TState> callback, UcmaCallSession.SessionState callbackState, string callbackTraceName)
			{
				return this.WrapCallback<TState>(callback, callbackState, callbackTraceName, true);
			}

			private SerializableCallback<TState> WrapCallbackAndIgnoreState<TState>(SerializableCallback<TState> callback, UcmaCallSession.SessionState callbackState, string callbackTraceName)
			{
				return this.WrapCallback<TState>(callback, callbackState, callbackTraceName, false);
			}

			private SerializableCallback<TState> WrapCallback<TState>(SerializableCallback<TState> callback, UcmaCallSession.SessionState callbackState, string callbackTraceName, bool assertState)
			{
				return delegate(TState o)
				{
					using (new CallId(this.session.CallId))
					{
						this.session.CatchAndFireOnError(delegate
						{
							this.Diag.Trace("Event: {0}::{1}", new object[]
							{
								this.session.CurrentState.Name,
								callbackTraceName
							});
							if (this.IsDisposed)
							{
								this.Diag.Trace("Ignoring callback for state {0} because the subscription manager has been disposed", new object[]
								{
									callbackState.Name
								});
								return;
							}
							if (callbackState.Equals(this.session.CurrentState))
							{
								callback(o);
								this.session.CurrentState.CompleteAsyncCallback();
								return;
							}
							if (assertState)
							{
								this.Diag.Assert(false, "callback state {0} is not equal to current state {1} !", new object[]
								{
									callbackState.Name,
									this.session.CurrentState.Name
								});
								return;
							}
							this.Diag.Trace("Ignoring callback because current state {0} does not equal callback state {1}", new object[]
							{
								callbackState.Name,
								this.session.CurrentState.Name
							});
						});
					}
				};
			}

			private EventHandler<TArgs> CreateSerializedEventHandler<TArgs>(SerializableEventHandler<TArgs> callback, UcmaCallSession.SubscriptionHelper.EventRegistry.UnregisterEventHandler<TArgs> unregister, string eventTraceName) where TArgs : EventArgs
			{
				return this.CreateSerializedEventHandler<TArgs>(callback, unregister, false, eventTraceName);
			}

			private EventHandler<TArgs> CreateSerializedEventHandler<TArgs>(SerializableEventHandler<TArgs> callback, UcmaCallSession.SubscriptionHelper.EventRegistry.UnregisterEventHandler<TArgs> unregister, bool forceEvent, string eventTraceName) where TArgs : EventArgs
			{
				SerializableEventHandler<TArgs> wrappedHandler = delegate(object sender, TArgs args)
				{
					using (new CallId(this.session.CallId))
					{
						this.session.CatchAndFireOnError(delegate
						{
							this.Diag.Trace("Event: {0}::{1}", new object[]
							{
								this.session.CurrentState.Name,
								eventTraceName
							});
							if (this.IsDisposed)
							{
								this.Diag.Trace("Ignoring event handler because the subscription manager has been disposed", new object[0]);
								return;
							}
							callback(sender, args);
							this.session.CurrentState.CompleteAsyncCallback();
						});
					}
				};
				EventHandler<TArgs> eventHandler = delegate(object sender, TArgs args)
				{
					this.session.Serializer.SerializeEvent<TArgs>(sender, args, wrappedHandler, this.session, forceEvent, eventTraceName);
				};
				this.registry.Register(unregister, eventHandler);
				return eventHandler;
			}

			private UcmaCallSession.SubscriptionHelper.EventRegistry registry;

			private UcmaCallSession session;

			private class EventRegistry : DisposableBase
			{
				public void Register(Delegate unsubscribeDelegate, Delegate eventHandler)
				{
					this.registrations.Add(new KeyValuePair<Delegate, Delegate>(unsubscribeDelegate, eventHandler));
				}

				protected override void InternalDispose(bool disposing)
				{
					if (disposing)
					{
						foreach (KeyValuePair<Delegate, Delegate> keyValuePair in this.registrations)
						{
							keyValuePair.Key.DynamicInvoke(new object[]
							{
								keyValuePair.Value
							});
						}
					}
				}

				protected override DisposeTracker InternalGetDisposeTracker()
				{
					return DisposeTracker.Get<UcmaCallSession.SubscriptionHelper.EventRegistry>(this);
				}

				private List<KeyValuePair<Delegate, Delegate>> registrations = new List<KeyValuePair<Delegate, Delegate>>(32);

				public delegate void UnregisterEventHandler<TArgs>(EventHandler<TArgs> handler) where TArgs : EventArgs;
			}
		}
	}
}
