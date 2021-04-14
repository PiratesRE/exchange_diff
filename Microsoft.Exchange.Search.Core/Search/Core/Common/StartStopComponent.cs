using System;
using System.Threading;
using System.Xml.Linq;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Search;
using Microsoft.Exchange.Search.Core.Abstraction;

namespace Microsoft.Exchange.Search.Core.Common
{
	internal abstract class StartStopComponent : StatefulComponent, IStartStop, IDisposable, INotifyFailed
	{
		internal StartStopComponent() : base(2U)
		{
			base.DiagnosticsSession.ComponentName = "StartStopComponent";
			base.DiagnosticsSession.Tracer = ExTraceGlobals.CoreComponentTracer;
		}

		public event EventHandler<FailedEventArgs> Failed;

		internal ComponentFailedException LastFailedReason { get; private set; }

		public IAsyncResult BeginPrepareToStart(AsyncCallback callback, object context)
		{
			base.CheckDisposed();
			AsyncResult asyncResult = new AsyncResult(callback, context);
			this.BeginDispatchPrepareToStartSignal(asyncResult, this.EndDispatchSignalCallback(asyncResult), null);
			return asyncResult;
		}

		public void EndPrepareToStart(IAsyncResult asyncResult)
		{
			base.CheckDisposed();
			((AsyncResult)asyncResult).End();
		}

		public IAsyncResult BeginStart(AsyncCallback callback, object context)
		{
			base.CheckDisposed();
			AsyncResult asyncResult = new AsyncResult(callback, context);
			this.BeginDispatchStartSignal(asyncResult, this.EndDispatchSignalCallback(asyncResult), null);
			return asyncResult;
		}

		public void EndStart(IAsyncResult asyncResult)
		{
			base.CheckDisposed();
			((AsyncResult)asyncResult).End();
		}

		public IAsyncResult BeginStop(AsyncCallback callback, object context)
		{
			base.CheckDisposed();
			AsyncResult asyncResult = new AsyncResult(callback, context);
			this.BeginDispatchStopSignal(asyncResult, this.EndDispatchSignalCallback(asyncResult), null);
			return asyncResult;
		}

		public void EndStop(IAsyncResult asyncResult)
		{
			base.CheckDisposed();
			((AsyncResult)asyncResult).End();
		}

		protected override bool AtNoTransitionDefined(uint signal)
		{
			if (base.CurrentState == 9U || base.CurrentState == 8U)
			{
				this.LastFailedReason.RethrowNewInstance();
			}
			return base.AtNoTransitionDefined(signal);
		}

		protected virtual void AtPrepareToStart(AsyncResult asyncResult)
		{
			Util.ThrowOnNullArgument(asyncResult, "asyncResult");
			this.BeginDispatchDonePreparingToStartSignal(asyncResult, this.EndDispatchSignalCallback(asyncResult), null);
		}

		protected virtual void AtDonePreparingToStart(AsyncResult asyncResult)
		{
			Util.ThrowOnNullArgument(asyncResult, "asyncResult");
			asyncResult.SetAsCompleted();
		}

		protected virtual void AtStart(AsyncResult asyncResult)
		{
			Util.ThrowOnNullArgument(asyncResult, "asyncResult");
			this.BeginDispatchDoneStartingSignal(asyncResult, this.EndDispatchSignalCallback(asyncResult), null);
		}

		protected virtual void AtDoneStarting(AsyncResult asyncResult)
		{
			Util.ThrowOnNullArgument(asyncResult, "asyncResult");
			asyncResult.SetAsCompleted();
		}

		protected virtual void AtStop(AsyncResult asyncResult)
		{
			Util.ThrowOnNullArgument(asyncResult, "asyncResult");
			this.BeginDispatchDoneStoppingSignal(asyncResult, this.EndDispatchSignalCallback(asyncResult), null);
		}

		protected virtual void AtDoneStopping(AsyncResult asyncResult)
		{
			Util.ThrowOnNullArgument(asyncResult, "asyncResult");
			asyncResult.SetAsCompleted();
		}

		protected virtual void AtFail(ComponentFailedException reason)
		{
			Util.ThrowOnNullArgument(reason, "reason");
			this.BeginDispatchDoneFailingSignal(reason, new AsyncCallback(this.EndDispatchDoneFailingSignal), null);
		}

		protected virtual void AtDoneFailing(ComponentFailedException reason)
		{
			Util.ThrowOnNullArgument(reason, "reason");
			this.OnFailed(new FailedEventArgs(reason));
		}

		protected virtual void AtStopInFailed(AsyncResult asyncResult)
		{
			Util.ThrowOnNullArgument(asyncResult, "asyncResult");
			this.BeginDispatchDoneStoppingSignal(asyncResult, this.EndDispatchSignalCallback(asyncResult), null);
		}

		protected bool TryRunUnderExceptionHandler<TReturnValue>(Func<TReturnValue> action, out TReturnValue returnValue, LocalizedString message)
		{
			TReturnValue tempReturnValue = default(TReturnValue);
			bool result = this.TryRunUnderExceptionHandler(delegate()
			{
				tempReturnValue = action();
			}, message);
			returnValue = tempReturnValue;
			return result;
		}

		protected bool TryRunUnderExceptionHandler(Action action, LocalizedString message)
		{
			ComponentFailedException ex;
			return this.TryRunUnderExceptionHandler(action, message, out ex);
		}

		protected bool TryRunUnderExceptionHandler(Action action, LocalizedString message, out ComponentFailedException result)
		{
			result = null;
			try
			{
				action();
				return true;
			}
			catch (ComponentFailedPermanentException innerException)
			{
				result = new ComponentFailedPermanentException(message, innerException);
			}
			catch (ComponentFailedTransientException innerException2)
			{
				result = new ComponentFailedTransientException(message, innerException2);
			}
			catch (OperationFailedException innerException3)
			{
				result = new ComponentFailedTransientException(message, innerException3);
			}
			base.DiagnosticsSession.TraceError<ComponentFailedException>("Component failing: {0}", result);
			this.BeginDispatchFailSignal(result, delegate(IAsyncResult failResult)
			{
				try
				{
					this.EndDispatchFailSignal(failResult);
				}
				catch (ComponentFailedException arg)
				{
					base.DiagnosticsSession.TraceError<ComponentFailedException>("Got error from signal processing: {0}", arg);
				}
			}, null);
			return false;
		}

		protected override XElement InternalGetDiagnosticInfo(DiagnosableParameters parameters)
		{
			XElement xelement = base.InternalGetDiagnosticInfo(parameters);
			if (base.CurrentState == 8U)
			{
				xelement.Add(new XElement("Reason", this.LastFailedReason));
			}
			return xelement;
		}

		private void AtFailInternal(ComponentFailedException reason)
		{
			Util.ThrowOnNullArgument(reason, "reason");
			base.DiagnosticsSession.TraceError<StartStopComponent, ComponentFailedException>("Component failed: {0}. Exception: {1}", this, reason);
			this.LastFailedReason = reason;
			this.AtFail(reason);
		}

		private void OnFailed(FailedEventArgs e)
		{
			EventHandler<FailedEventArgs> failed = this.Failed;
			if (failed != null)
			{
				failed(this, e);
			}
		}

		private AsyncCallback EndDispatchSignalCallback(AsyncResult result)
		{
			return delegate(IAsyncResult ar)
			{
				try
				{
					if (!this.EndDispatchSignal(ar))
					{
						result.SetAsCompleted();
					}
				}
				catch (ComponentException asCompleted)
				{
					result.SetAsCompleted(asCompleted);
				}
			};
		}

		private static void RegisterComponent(ComponentInfo componentInfo)
		{
			componentInfo.RegisterSignal(StartStopComponent.Signal.PrepareToStart, SignalPriority.Medium);
			componentInfo.RegisterSignal(StartStopComponent.Signal.DonePreparingToStart, SignalPriority.Medium);
			componentInfo.RegisterSignal(StartStopComponent.Signal.Start, SignalPriority.Medium);
			componentInfo.RegisterSignal(StartStopComponent.Signal.DoneStarting, SignalPriority.Medium);
			componentInfo.RegisterSignal(StartStopComponent.Signal.Stop, SignalPriority.Medium);
			componentInfo.RegisterSignal(StartStopComponent.Signal.DoneStopping, SignalPriority.Medium);
			componentInfo.RegisterSignal(StartStopComponent.Signal.Fail, SignalPriority.High);
			componentInfo.RegisterSignal(StartStopComponent.Signal.DoneFailing, SignalPriority.Medium);
			componentInfo.RegisterState(StartStopComponent.State.Stopped);
			componentInfo.RegisterState(StartStopComponent.State.Stopping);
			componentInfo.RegisterState(StartStopComponent.State.PreparedToStart);
			componentInfo.RegisterState(StartStopComponent.State.PreparingToStart);
			componentInfo.RegisterState(StartStopComponent.State.Started);
			componentInfo.RegisterState(StartStopComponent.State.Starting);
			componentInfo.RegisterState(StartStopComponent.State.Failed);
			componentInfo.RegisterState(StartStopComponent.State.Failing);
			componentInfo.RegisterTransition(2U, 5U, 2U, null, null);
			componentInfo.RegisterTransition(2U, 3U, 7U, null, new ActionMethod(StartStopComponent.Transition_AtStart));
			componentInfo.RegisterTransition(2U, 1U, 5U, null, new ActionMethod(StartStopComponent.Transition_AtPrepareToStart));
			componentInfo.RegisterTransition(7U, 4U, 6U, null, new ActionMethod(StartStopComponent.Transition_AtDoneStarting));
			componentInfo.RegisterTransition(7U, 5U, 3U, null, new ActionMethod(StartStopComponent.Transition_AtStop));
			componentInfo.RegisterTransition(6U, 3U, 6U, null, null);
			componentInfo.RegisterTransition(6U, 5U, 3U, null, new ActionMethod(StartStopComponent.Transition_AtStop));
			componentInfo.RegisterTransition(5U, 2U, 4U, null, new ActionMethod(StartStopComponent.Transition_AtDonePreparingToStart));
			componentInfo.RegisterTransition(5U, 5U, 3U, null, new ActionMethod(StartStopComponent.Transition_AtStop));
			componentInfo.RegisterTransition(4U, 1U, 4U, null, null);
			componentInfo.RegisterTransition(4U, 3U, 7U, null, new ActionMethod(StartStopComponent.Transition_AtStart));
			componentInfo.RegisterTransition(4U, 5U, 3U, null, new ActionMethod(StartStopComponent.Transition_AtStop));
			componentInfo.RegisterTransition(3U, 6U, 2U, null, new ActionMethod(StartStopComponent.Transition_AtDoneStopping));
			componentInfo.RegisterTransition(uint.MaxValue, 7U, 9U, null, new ActionMethod(StartStopComponent.Transition_AtFailInternal));
			componentInfo.RegisterTransition(2U, 7U, 2U, null, null);
			componentInfo.RegisterTransition(8U, 7U, 8U, null, null);
			componentInfo.RegisterTransition(9U, 7U, 9U, null, null);
			componentInfo.RegisterTransition(3U, 7U, 3U, null, null);
			componentInfo.RegisterTransition(9U, 8U, 8U, null, new ActionMethod(StartStopComponent.Transition_AtDoneFailing));
			componentInfo.RegisterTransition(8U, 5U, 3U, null, new ActionMethod(StartStopComponent.Transition_AtStopInFailed));
		}

		internal static void Transition_AtStart(object component, params object[] args)
		{
			((StartStopComponent)component).AtStart((AsyncResult)args[0]);
		}

		internal static void Transition_AtPrepareToStart(object component, params object[] args)
		{
			((StartStopComponent)component).AtPrepareToStart((AsyncResult)args[0]);
		}

		internal static void Transition_AtDoneStarting(object component, params object[] args)
		{
			((StartStopComponent)component).AtDoneStarting((AsyncResult)args[0]);
		}

		internal static void Transition_AtStop(object component, params object[] args)
		{
			((StartStopComponent)component).AtStop((AsyncResult)args[0]);
		}

		internal static void Transition_AtDonePreparingToStart(object component, params object[] args)
		{
			((StartStopComponent)component).AtDonePreparingToStart((AsyncResult)args[0]);
		}

		internal static void Transition_AtDoneStopping(object component, params object[] args)
		{
			((StartStopComponent)component).AtDoneStopping((AsyncResult)args[0]);
		}

		internal static void Transition_AtFailInternal(object component, params object[] args)
		{
			((StartStopComponent)component).AtFailInternal((ComponentFailedException)args[0]);
		}

		internal static void Transition_AtDoneFailing(object component, params object[] args)
		{
			((StartStopComponent)component).AtDoneFailing((ComponentFailedException)args[0]);
		}

		internal static void Transition_AtStopInFailed(object component, params object[] args)
		{
			((StartStopComponent)component).AtStopInFailed((AsyncResult)args[0]);
		}

		internal IAsyncResult BeginDispatchPrepareToStartSignal(AsyncResult asyncResult, AsyncCallback callback, object context)
		{
			return base.InternalBeginDispatchSignal(null, 1U, callback, context, TimeSpan.Zero, new object[]
			{
				asyncResult
			});
		}

		internal IAsyncResult BeginDispatchPrepareToStartSignal(AsyncResult asyncResult, AsyncCallback callback, object context, WaitHandle waitHandle, TimeSpan delayInTimespan)
		{
			Util.ThrowOnNullArgument(waitHandle, "waitHandle");
			return base.InternalBeginDispatchSignal(waitHandle, 1U, callback, context, delayInTimespan, new object[]
			{
				asyncResult
			});
		}

		internal void EndDispatchPrepareToStartSignal(IAsyncResult asyncResult)
		{
			base.EndDispatchSignal(asyncResult);
		}

		internal IAsyncResult BeginDispatchDonePreparingToStartSignal(AsyncResult asyncResult, AsyncCallback callback, object context)
		{
			return base.InternalBeginDispatchSignal(null, 2U, callback, context, TimeSpan.Zero, new object[]
			{
				asyncResult
			});
		}

		internal IAsyncResult BeginDispatchDonePreparingToStartSignal(AsyncResult asyncResult, AsyncCallback callback, object context, WaitHandle waitHandle, TimeSpan delayInTimespan)
		{
			Util.ThrowOnNullArgument(waitHandle, "waitHandle");
			return base.InternalBeginDispatchSignal(waitHandle, 2U, callback, context, delayInTimespan, new object[]
			{
				asyncResult
			});
		}

		internal void EndDispatchDonePreparingToStartSignal(IAsyncResult asyncResult)
		{
			base.EndDispatchSignal(asyncResult);
		}

		internal IAsyncResult BeginDispatchStartSignal(AsyncResult asyncResult, AsyncCallback callback, object context)
		{
			return base.InternalBeginDispatchSignal(null, 3U, callback, context, TimeSpan.Zero, new object[]
			{
				asyncResult
			});
		}

		internal IAsyncResult BeginDispatchStartSignal(AsyncResult asyncResult, AsyncCallback callback, object context, WaitHandle waitHandle, TimeSpan delayInTimespan)
		{
			Util.ThrowOnNullArgument(waitHandle, "waitHandle");
			return base.InternalBeginDispatchSignal(waitHandle, 3U, callback, context, delayInTimespan, new object[]
			{
				asyncResult
			});
		}

		internal void EndDispatchStartSignal(IAsyncResult asyncResult)
		{
			base.EndDispatchSignal(asyncResult);
		}

		internal IAsyncResult BeginDispatchDoneStartingSignal(AsyncResult asyncResult, AsyncCallback callback, object context)
		{
			return base.InternalBeginDispatchSignal(null, 4U, callback, context, TimeSpan.Zero, new object[]
			{
				asyncResult
			});
		}

		internal IAsyncResult BeginDispatchDoneStartingSignal(AsyncResult asyncResult, AsyncCallback callback, object context, WaitHandle waitHandle, TimeSpan delayInTimespan)
		{
			Util.ThrowOnNullArgument(waitHandle, "waitHandle");
			return base.InternalBeginDispatchSignal(waitHandle, 4U, callback, context, delayInTimespan, new object[]
			{
				asyncResult
			});
		}

		internal void EndDispatchDoneStartingSignal(IAsyncResult asyncResult)
		{
			base.EndDispatchSignal(asyncResult);
		}

		internal IAsyncResult BeginDispatchStopSignal(AsyncResult asyncResult, AsyncCallback callback, object context)
		{
			return base.InternalBeginDispatchSignal(null, 5U, callback, context, TimeSpan.Zero, new object[]
			{
				asyncResult
			});
		}

		internal IAsyncResult BeginDispatchStopSignal(AsyncResult asyncResult, AsyncCallback callback, object context, WaitHandle waitHandle, TimeSpan delayInTimespan)
		{
			Util.ThrowOnNullArgument(waitHandle, "waitHandle");
			return base.InternalBeginDispatchSignal(waitHandle, 5U, callback, context, delayInTimespan, new object[]
			{
				asyncResult
			});
		}

		internal void EndDispatchStopSignal(IAsyncResult asyncResult)
		{
			base.EndDispatchSignal(asyncResult);
		}

		internal IAsyncResult BeginDispatchDoneStoppingSignal(AsyncResult asyncResult, AsyncCallback callback, object context)
		{
			return base.InternalBeginDispatchSignal(null, 6U, callback, context, TimeSpan.Zero, new object[]
			{
				asyncResult
			});
		}

		internal IAsyncResult BeginDispatchDoneStoppingSignal(AsyncResult asyncResult, AsyncCallback callback, object context, WaitHandle waitHandle, TimeSpan delayInTimespan)
		{
			Util.ThrowOnNullArgument(waitHandle, "waitHandle");
			return base.InternalBeginDispatchSignal(waitHandle, 6U, callback, context, delayInTimespan, new object[]
			{
				asyncResult
			});
		}

		internal void EndDispatchDoneStoppingSignal(IAsyncResult asyncResult)
		{
			base.EndDispatchSignal(asyncResult);
		}

		internal IAsyncResult BeginDispatchFailSignal(ComponentFailedException reason, AsyncCallback callback, object context)
		{
			return base.InternalBeginDispatchSignal(null, 7U, callback, context, TimeSpan.Zero, new object[]
			{
				reason
			});
		}

		internal IAsyncResult BeginDispatchFailSignal(ComponentFailedException reason, AsyncCallback callback, object context, WaitHandle waitHandle, TimeSpan delayInTimespan)
		{
			Util.ThrowOnNullArgument(waitHandle, "waitHandle");
			return base.InternalBeginDispatchSignal(waitHandle, 7U, callback, context, delayInTimespan, new object[]
			{
				reason
			});
		}

		internal void EndDispatchFailSignal(IAsyncResult asyncResult)
		{
			base.EndDispatchSignal(asyncResult);
		}

		internal IAsyncResult BeginDispatchDoneFailingSignal(ComponentFailedException reason, AsyncCallback callback, object context)
		{
			return base.InternalBeginDispatchSignal(null, 8U, callback, context, TimeSpan.Zero, new object[]
			{
				reason
			});
		}

		internal IAsyncResult BeginDispatchDoneFailingSignal(ComponentFailedException reason, AsyncCallback callback, object context, WaitHandle waitHandle, TimeSpan delayInTimespan)
		{
			Util.ThrowOnNullArgument(waitHandle, "waitHandle");
			return base.InternalBeginDispatchSignal(waitHandle, 8U, callback, context, delayInTimespan, new object[]
			{
				reason
			});
		}

		internal void EndDispatchDoneFailingSignal(IAsyncResult asyncResult)
		{
			base.EndDispatchSignal(asyncResult);
		}

		internal new enum Signal : uint
		{
			PrepareToStart = 1U,
			DonePreparingToStart,
			Start,
			DoneStarting,
			Stop,
			DoneStopping,
			Fail,
			DoneFailing,
			Max
		}

		internal new enum State : uint
		{
			Stopped = 2U,
			Stopping,
			PreparedToStart,
			PreparingToStart,
			Started,
			Starting,
			Failed,
			Failing,
			Max
		}
	}
}
