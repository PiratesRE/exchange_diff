using System;
using System.Net;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ContentAggregation;
using Microsoft.Exchange.Rpc.SubscriptionSubmission;

namespace Microsoft.Exchange.Transport.Sync.Manager
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class ContentAggregationHubServer : DisposeTrackableBase
	{
		internal ContentAggregationHubServer()
		{
			this.localServerMachineName = Environment.MachineName;
		}

		internal string MachineName
		{
			get
			{
				base.CheckDisposed();
				return this.localServerMachineName;
			}
		}

		internal SubscriptionSubmissionRpcClient RpcClient
		{
			get
			{
				base.CheckDisposed();
				if (this.rpcClient == null)
				{
					lock (this.syncRoot)
					{
						if (this.rpcClient == null)
						{
							this.rpcClient = new SubscriptionSubmissionRpcClient(this.localServerMachineName, ContentAggregationHubServer.localSystemCredential);
						}
					}
				}
				return this.rpcClient;
			}
		}

		internal void IncrementRef()
		{
			base.CheckDisposed();
			lock (this.syncRoot)
			{
				this.refCount++;
			}
		}

		internal void DecrementRef()
		{
			base.CheckDisposed();
			lock (this.syncRoot)
			{
				this.refCount--;
				if (this.disposeRequested && this.refCount == 0)
				{
					this.Dispose();
				}
			}
		}

		internal void RequestDispose()
		{
			base.CheckDisposed();
			this.disposeRequested = true;
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				lock (this.syncRoot)
				{
					if (this.rpcClient != null)
					{
						this.rpcClient.Dispose();
						this.rpcClient = null;
					}
				}
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<ContentAggregationHubServer>(this);
		}

		private static readonly Trace Diag = ExTraceGlobals.SubscriptionSubmitTracer;

		private static readonly NetworkCredential localSystemCredential = new NetworkCredential(Environment.MachineName + "$", string.Empty, string.Empty);

		private readonly object syncRoot = new object();

		private readonly string localServerMachineName;

		private SubscriptionSubmissionRpcClient rpcClient;

		private int refCount;

		private bool disposeRequested;
	}
}
