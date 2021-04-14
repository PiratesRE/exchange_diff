using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Transport;
using Microsoft.Exchange.Extensibility.Internal;
using Microsoft.Exchange.Protocols.Smtp;

namespace Microsoft.Exchange.Transport.ShadowRedundancy
{
	internal class CommitCoordinator
	{
		internal IAsyncResult LocalCommitResult
		{
			get
			{
				return this.localCommitResult;
			}
		}

		internal IAsyncResult ShadowCommitResult
		{
			get
			{
				return this.shadowCommitResult;
			}
		}

		public CommitCoordinator(ISmtpInMailItemStorage storage, IShadowSession shadowSession, ProcessTransportRole transportRole)
		{
			ArgumentValidator.ThrowIfNull("storage", storage);
			ArgumentValidator.ThrowIfNull("shadowSession", shadowSession);
			this.storage = storage;
			this.shadowSession = shadowSession;
			this.processTransportRole = transportRole;
		}

		public Task<SmtpResponse> CommitMailItemAsync(TransportMailItem transportMailItem)
		{
			TaskCompletionSource<SmtpResponse> taskCompletionSource = new TaskCompletionSource<SmtpResponse>();
			this.BeginCommitMailItem(transportMailItem, new AsyncCallback(this.EndCommitMailItem), taskCompletionSource);
			return taskCompletionSource.Task;
		}

		public IAsyncResult BeginCommitMailItem(TransportMailItem transportMailItem, AsyncCallback asyncCallback, object state)
		{
			ArgumentValidator.ThrowIfNull("transportMailItem", transportMailItem);
			ArgumentValidator.ThrowIfNull("asyncCallback", asyncCallback);
			ExTraceGlobals.SmtpReceiveTracer.TraceDebug((long)this.GetHashCode(), "CommitCoordinator.BeginCommitMailItem");
			if (this.finalCommitResult != null || this.mailItem != null)
			{
				throw new InvalidOperationException("BeginCommitMailItem already called");
			}
			SystemProbeHelper.ShadowRedundancyTracer.TracePass(this.mailItem, (long)this.GetHashCode(), "Message received by hub server");
			this.mailItem = transportMailItem;
			this.finalCommitResult = new AsyncResult(asyncCallback, state);
			this.StartCommit();
			return this.finalCommitResult;
		}

		public bool EndCommitMailItem(IAsyncResult asyncResult, out SmtpResponse smtpResponse, out Exception commitException)
		{
			ArgumentValidator.ThrowIfNull("asyncResult", asyncResult);
			ExTraceGlobals.SmtpReceiveTracer.TraceDebug((long)this.GetHashCode(), "CommitCoordinator.EndCommitMailItem");
			if (asyncResult != this.finalCommitResult)
			{
				throw new InvalidOperationException("Mismatched async results");
			}
			return this.CompleteCommit(out smtpResponse, out commitException);
		}

		private void EndCommitMailItem(IAsyncResult asyncResult)
		{
			TaskCompletionSource<SmtpResponse> taskCompletionSource = (TaskCompletionSource<SmtpResponse>)asyncResult.AsyncState;
			try
			{
				SmtpResponse result;
				Exception ex;
				if (this.EndCommitMailItem(asyncResult, out result, out ex))
				{
					taskCompletionSource.SetResult(SmtpResponse.Empty);
				}
				else if (ex != null)
				{
					taskCompletionSource.SetException(ex);
				}
				else
				{
					taskCompletionSource.SetResult(result);
				}
			}
			catch (Exception exception)
			{
				taskCompletionSource.SetException(exception);
			}
		}

		private void StartCommit()
		{
			ExTraceGlobals.SmtpReceiveTracer.TraceDebug((long)this.GetHashCode(), "CommitCoordinator.StartCommit");
			LatencyTracker.BeginTrackLatency(LatencyComponent.SmtpReceiveCommitLocal, this.mailItem.LatencyTracker);
			this.localCommitResult = this.storage.BeginCommitMailItem(this.mailItem, new AsyncCallback(this.LocalCommitCallback), this);
			LatencyTracker.BeginTrackLatency(LatencyComponent.SmtpReceiveCommitRemote, this.mailItem.LatencyTracker);
			this.shadowCommitResult = this.shadowSession.BeginComplete(new AsyncCallback(this.ShadowCommitCallback), this);
		}

		private bool CompleteCommit(out SmtpResponse smtpResponse, out Exception commitException)
		{
			ExTraceGlobals.SmtpReceiveTracer.TraceDebug((long)this.GetHashCode(), "CommitCoordinator.CompleteCommit");
			smtpResponse = SmtpResponse.Empty;
			bool flag = this.storage.EndCommitMailItem(this.mailItem, this.localCommitResult, out commitException);
			bool flag2 = this.shadowSession.EndComplete(this.shadowCommitResult);
			if (!flag)
			{
				this.shadowSession.NotifyLocalMessageDiscarded(this.mailItem);
				return false;
			}
			if (this.processTransportRole == ProcessTransportRole.Hub && ShadowRedundancyManager.PerfCounters != null)
			{
				ShadowRedundancyManager.PerfCounters.TrackMessageMadeRedundant(flag2);
			}
			if (!flag2 && this.shadowSession.MailItemRequiresShadowCopy(this.mailItem))
			{
				this.shadowSession.NotifyMessageRejected(this.mailItem);
				smtpResponse = SmtpResponse.ShadowRedundancyFailed;
				commitException = null;
				return false;
			}
			this.shadowSession.NotifyMessageComplete(this.mailItem);
			return true;
		}

		private void LocalCommitCallback(IAsyncResult asyncResult)
		{
			ExTraceGlobals.SmtpReceiveTracer.TraceDebug((long)this.GetHashCode(), "CommitCoordinator.LocalCommitCallback");
			LatencyTracker.EndTrackLatency(LatencyComponent.SmtpReceiveCommitLocal, this.mailItem.LatencyTracker);
			if (Interlocked.Increment(ref this.localComplete) == 1)
			{
				this.localCommitResult = asyncResult;
				this.JoinAllAsyncCommits();
			}
		}

		private void ShadowCommitCallback(IAsyncResult asyncResult)
		{
			ExTraceGlobals.SmtpReceiveTracer.TraceDebug((long)this.GetHashCode(), "CommitCoordinator.ShadowCommitCallback");
			LatencyTracker.EndTrackLatency(LatencyComponent.SmtpReceiveCommitRemote, this.mailItem.LatencyTracker);
			if (Interlocked.Increment(ref this.shadowComplete) == 1)
			{
				this.shadowCommitResult = asyncResult;
				this.JoinAllAsyncCommits();
			}
		}

		private void JoinAllAsyncCommits()
		{
			ExTraceGlobals.SmtpReceiveTracer.TraceDebug((long)this.GetHashCode(), "CommitCoordinator.JoinAllAsyncCommits");
			if (Interlocked.Decrement(ref this.commitsRemaining) == 0)
			{
				ExTraceGlobals.SmtpReceiveTracer.TraceDebug((long)this.GetHashCode(), "CommitCoordinator triggering final callback");
				this.finalCommitResult.IsCompleted = true;
			}
		}

		private ISmtpInMailItemStorage storage;

		private IShadowSession shadowSession;

		private TransportMailItem mailItem;

		private AsyncResult finalCommitResult;

		private IAsyncResult localCommitResult;

		private IAsyncResult shadowCommitResult;

		private int localComplete;

		private int shadowComplete;

		private int commitsRemaining = 2;

		private readonly ProcessTransportRole processTransportRole;
	}
}
