using System;
using System.Collections.Generic;
using System.Threading;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Search.Core.Abstraction;

namespace Microsoft.Exchange.Search.Core.Common
{
	internal abstract class ContainerComponent : StartStopComponent, IDiagnosable
	{
		XElement IDiagnosable.GetDiagnosticInfo(DiagnosableParameters parameters)
		{
			XElement xelement = this.InternalGetDiagnosticInfo(parameters);
			lock (this.diagnosableComponents)
			{
				foreach (IDiagnosable diagnosable in this.diagnosableComponents)
				{
					xelement.Add(diagnosable.GetDiagnosticInfo(parameters));
				}
			}
			return xelement;
		}

		protected abstract void CreateChildren();

		protected abstract void DisposeChildren();

		protected abstract void PrepareToStartChildrenAsync();

		protected abstract void StartChildrenAsync();

		protected abstract void StopChildrenAsync();

		protected void AddComponent(IDiagnosable component)
		{
			lock (this.diagnosableComponents)
			{
				this.diagnosableComponents.Add(component);
			}
		}

		protected void RemoveComponent(IDiagnosable component)
		{
			lock (this.diagnosableComponents)
			{
				this.diagnosableComponents.Remove(component);
			}
		}

		protected sealed override void AtPrepareToStart(AsyncResult asyncResult)
		{
			base.DiagnosticsSession.TraceDebug<ContainerComponent>("{0} is preparing to start.", this);
			if (Interlocked.CompareExchange<AsyncResult>(ref this.pendingAsyncResult, asyncResult, null) != null)
			{
				throw new InvalidOperationException("There is another pending async result not completed yet.");
			}
			try
			{
				this.CreateChildren();
				this.PrepareToStartChildrenAsync();
			}
			catch (ComponentException exception)
			{
				this.CompletePrepareToStart(exception);
			}
		}

		protected sealed override void AtStart(AsyncResult asyncResult)
		{
			base.DiagnosticsSession.TraceDebug<ContainerComponent>("{0} is starting.", this);
			if (Interlocked.CompareExchange<AsyncResult>(ref this.pendingAsyncResult, asyncResult, null) != null)
			{
				throw new InvalidOperationException("There is another pending async result not completed yet.");
			}
			try
			{
				this.StartChildrenAsync();
			}
			catch (ComponentException exception)
			{
				this.CompleteStart(exception);
			}
		}

		protected sealed override void AtStop(AsyncResult asyncResult)
		{
			base.DiagnosticsSession.TraceDebug<ContainerComponent>("{0} is stopping.", this);
			if (Interlocked.CompareExchange<AsyncResult>(ref this.pendingAsyncResult, asyncResult, null) != null)
			{
				throw new InvalidOperationException("There is another pending async result not completed yet.");
			}
			try
			{
				this.StopChildrenAsync();
			}
			catch (ComponentException exception)
			{
				this.CompleteStop(exception);
			}
		}

		protected override void AtDoneStopping(AsyncResult asyncResult)
		{
			this.DisposeChildren();
			base.AtDoneStopping(asyncResult);
		}

		protected sealed override void AtStopInFailed(AsyncResult asyncResult)
		{
			base.DiagnosticsSession.TraceDebug<ContainerComponent>("{0} is stopping from failed state.", this);
			if (Interlocked.CompareExchange<AsyncResult>(ref this.pendingAsyncResult, asyncResult, null) != null)
			{
				throw new InvalidOperationException("There is another pending async result not completed yet.");
			}
			this.StopChildrenAsync();
		}

		protected override void InternalDispose(bool calledFromDispose)
		{
			if (calledFromDispose)
			{
				this.DisposeChildren();
				base.DiagnosticsSession.Assert(this.diagnosableComponents.Count == 0, "There must be no diagnosable components after children are disposed", new object[0]);
			}
			base.InternalDispose(calledFromDispose);
		}

		protected void CompletePrepareToStart(ComponentException exception)
		{
			base.DiagnosticsSession.TraceDebug<ContainerComponent>("{0} is completing prepare to start.", this);
			AsyncResult asyncResult = Interlocked.Exchange<AsyncResult>(ref this.pendingAsyncResult, null);
			base.DiagnosticsSession.Assert(asyncResult != null, "The pendingAsyncResult must not be null", new object[0]);
			if (exception != null)
			{
				asyncResult.SetAsCompleted(exception);
				return;
			}
			base.BeginDispatchDonePreparingToStartSignal(asyncResult, new AsyncCallback(base.EndDispatchDonePreparingToStartSignal), null);
		}

		protected void CompleteStart(ComponentException exception)
		{
			base.DiagnosticsSession.TraceDebug<ContainerComponent>("{0} is completing start.", this);
			AsyncResult asyncResult = Interlocked.Exchange<AsyncResult>(ref this.pendingAsyncResult, null);
			base.DiagnosticsSession.Assert(asyncResult != null, "The pendingAsyncResult must not be null", new object[0]);
			if (exception != null)
			{
				asyncResult.SetAsCompleted(exception);
				return;
			}
			base.BeginDispatchDoneStartingSignal(asyncResult, new AsyncCallback(base.EndDispatchDoneStartingSignal), null);
		}

		protected void CompleteStop(ComponentException exception)
		{
			base.DiagnosticsSession.TraceDebug<ContainerComponent>("{0} is completing stop.", this);
			AsyncResult asyncResult = Interlocked.Exchange<AsyncResult>(ref this.pendingAsyncResult, null);
			base.DiagnosticsSession.Assert(asyncResult != null, "The pendingAsyncResult must not be null", new object[0]);
			if (exception != null)
			{
				asyncResult.SetAsCompleted(exception);
				return;
			}
			base.BeginDispatchDoneStoppingSignal(asyncResult, new AsyncCallback(base.EndDispatchDoneStoppingSignal), null);
		}

		private readonly IList<IDiagnosable> diagnosableComponents = new List<IDiagnosable>();

		private AsyncResult pendingAsyncResult;
	}
}
