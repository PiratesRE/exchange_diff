using System;
using System.Globalization;
using System.Text;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal abstract class ActivityBase : DisposableBase
	{
		internal ActivityBase(ActivityManager manager, ActivityConfig config)
		{
			this.manager = manager;
			this.config = config;
		}

		private ActivityBase()
		{
		}

		internal string UniqueId
		{
			get
			{
				return this.config.UniqueId;
			}
		}

		internal string ActivityId
		{
			get
			{
				return this.config.ActivityId;
			}
		}

		protected internal int FailureCount
		{
			get
			{
				return this.failureCount;
			}
			protected set
			{
				this.failureCount = value;
			}
		}

		protected internal ActivityManager Manager
		{
			get
			{
				return this.manager;
			}
			protected set
			{
				this.manager = value;
			}
		}

		protected ActivityConfig Config
		{
			get
			{
				return this.config;
			}
			set
			{
				this.config = value;
			}
		}

		protected int NumFailures
		{
			get
			{
				return this.failureCount;
			}
			set
			{
				this.failureCount = value;
			}
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "Type: {0}, ID: {1}", new object[]
			{
				base.GetType().ToString(),
				this.UniqueId
			});
		}

		internal virtual void Start(BaseUMCallSession vo, string refInfo)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "Starting {0} with refInfo {1}.", new object[]
			{
				this,
				refInfo
			});
			UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_FsmActivityStart, null, new object[]
			{
				this.ToString(),
				vo.CallId
			});
		}

		internal virtual void OnSpeech(object sender, UMSpeechEventArgs args)
		{
			throw new InvalidOperationException();
		}

		internal virtual void OnInput(BaseUMCallSession vo, UMCallSessionEventArgs callSessionEventArgs)
		{
			PIIMessage data = PIIMessage.Create(PIIType._PII, Encoding.ASCII.GetString(callSessionEventArgs.DtmfDigits));
			CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, data, "ActivityBase OnDTMFInput. Received _PII after {0} seconds.", new object[]
			{
				callSessionEventArgs.PlayTime
			});
		}

		internal virtual void OnUserHangup(BaseUMCallSession vo, UMCallSessionEventArgs callSessionEventArgs)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "ActivityBase OnUserHangup.", new object[0]);
			vo.DisconnectCall();
		}

		internal virtual void OnCancelled(BaseUMCallSession vo, UMCallSessionEventArgs callSessionEventArgs)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "ActivityBase OnCancelled.", new object[0]);
		}

		internal virtual void OnComplete(BaseUMCallSession vo, UMCallSessionEventArgs callSessionEventArgs)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "ActivityBase OnComplete Elapsed Seconds: {0}.", new object[]
			{
				callSessionEventArgs.PlayTime
			});
		}

		internal virtual void OnTimeout(BaseUMCallSession vo, UMCallSessionEventArgs callSessionEventArgs)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "ActivityBase OnTimeout called.", new object[0]);
		}

		internal virtual void OnStateInfoSent(BaseUMCallSession vo, UMCallSessionEventArgs callSessionEventArgs)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "State info send complete.", new object[0]);
		}

		internal virtual void OnOutBoundCallRequestCompleted(BaseUMCallSession vo, OutboundCallDetailsEventArgs callSessionEventArgs)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "OnOutBoundCallRequestCompleted", new object[0]);
		}

		internal virtual void OnError(BaseUMCallSession vo, UMCallSessionEventArgs callSessionEventArgs)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "OnError(), Error={0}.", new object[]
			{
				callSessionEventArgs.Error
			});
			if (callSessionEventArgs.Error != null)
			{
				string exceptionCategory = this.GetExceptionCategory(callSessionEventArgs);
				TransitionBase transition = this.GetTransition(exceptionCategory);
				if (transition == null)
				{
					return;
				}
				callSessionEventArgs.Error = null;
				transition.Execute(this.manager, vo);
			}
		}

		internal virtual void OnTransferComplete(BaseUMCallSession vo, UMCallSessionEventArgs callSessionEventArgs)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "OnTransferComplete(), Error={0}.", new object[]
			{
				callSessionEventArgs.Error
			});
		}

		internal virtual void OnMessageReceived(BaseUMCallSession vo, InfoMessage.MessageReceivedEventArgs e)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "Received message on trunk {0}:{1}.", new object[]
			{
				vo.SessionGuid,
				e.Message
			});
		}

		internal virtual void OnMessageSent(BaseUMCallSession vo, EventArgs e)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "Message sent on trunk {0}.", new object[]
			{
				vo.SessionGuid
			});
		}

		internal virtual void OnHeavyBlockingOperation(BaseUMCallSession vo, HeavyBlockingOperationEventArgs hboea)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "OnHeavyBlockingOperation(), completion type {0}.", new object[]
			{
				hboea.CompletionType
			});
			if (hboea.CompletionType == HeavyBlockingOperationCompletionType.Timeout)
			{
				return;
			}
			HeavyBlockingOperation heavyBlockingOperation = (HeavyBlockingOperation)hboea.Operation;
			heavyBlockingOperation.CompleteHeavyBlockingOperation();
		}

		internal virtual void OnDispose(BaseUMCallSession vo, EventArgs e)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "OnDispose.", new object[0]);
		}

		internal virtual void OnHold(BaseUMCallSession vo, UMCallSessionEventArgs callSessionEventArgs)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "Received a Hold event from the SIP peer.", new object[0]);
		}

		internal virtual void OnResume(BaseUMCallSession vo, UMCallSessionEventArgs callSessionEventArgs)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "Received a Hold event from the SIP peer.", new object[0]);
		}

		internal virtual TransitionBase GetTransition(string input)
		{
			return this.config.GetTransition(input, this.manager);
		}

		protected override void InternalDispose(bool disposing)
		{
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<ActivityBase>(this);
		}

		protected virtual string GetExceptionCategory(UMCallSessionEventArgs callSessionEventArgs)
		{
			if (callSessionEventArgs.Error is QuotaExceededException)
			{
				return "quotaExceeded";
			}
			if (callSessionEventArgs.Error != null)
			{
				return "xsoError";
			}
			return null;
		}

		private ActivityManager manager;

		private ActivityConfig config;

		private int failureCount;
	}
}
