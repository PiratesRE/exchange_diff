using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.InfoWorker.RequestDispatch;

namespace Microsoft.Exchange.InfoWorker.Common.Availability
{
	internal sealed class GetFolderAndProxyRequest : AsyncRequestWithQueryList, IDisposable
	{
		public GetFolderAndProxyRequest(Application application, InternalClientContext clientContext, RequestType requestType, RequestLogger requestLogger, QueryList queryList, TargetServerVersion targetVersion, ProxyAuthenticator proxyAuthenticator, WebServiceUri webServiceUri) : base(application, clientContext, requestType, requestLogger, queryList)
		{
			this.targetVersion = targetVersion;
			this.proxyAuthenticator = proxyAuthenticator;
			this.webServiceUri = webServiceUri;
		}

		public override void Abort()
		{
			base.Abort();
			if (this.parallel != null)
			{
				this.parallel.Abort();
			}
			if (this.proxyWebRequest != null)
			{
				this.proxyWebRequest.Abort();
			}
		}

		public void Dispose()
		{
			if (this.getFolderRequests != null)
			{
				foreach (GetFolderRequest getFolderRequest in this.getFolderRequests)
				{
					getFolderRequest.Dispose();
				}
			}
			if (this.proxyWebRequest != null)
			{
				this.proxyWebRequest.Dispose();
			}
		}

		public override void BeginInvoke(TaskCompleteCallback callback)
		{
			base.BeginInvoke(callback);
			GetFolderAndProxyRequest.GetFolderRequestTracer.TraceDebug<object, int, Uri>((long)this.GetHashCode(), "{0}: Dispatching GetFolder request for {1} mailboxes to Url {2}.", TraceContext.Get(), base.QueryList.Count, this.webServiceUri.Uri);
			this.getFolderRequests = new GetFolderRequest[base.QueryList.Count];
			for (int i = 0; i < base.QueryList.Count; i++)
			{
				BaseQuery query = base.QueryList[i];
				this.getFolderRequests[i] = new GetFolderRequest(base.Application, (InternalClientContext)base.ClientContext, base.RequestType, base.RequestLogger, query, this.webServiceUri.Uri);
			}
			this.parallel = new AsyncTaskParallel(this.getFolderRequests);
			this.parallel.BeginInvoke(new TaskCompleteCallback(this.Complete1));
		}

		private void Complete1(AsyncTask task)
		{
			GetFolderAndProxyRequest.GetFolderRequestTracer.TraceDebug((long)this.GetHashCode(), "{0}: Completed GetFolder requests.", new object[]
			{
				TraceContext.Get()
			});
			this.CheckResultsOfGetFolder();
			QueryList queryList = new QueryList(base.QueryList.Count);
			foreach (BaseQuery baseQuery in ((IEnumerable<BaseQuery>)base.QueryList))
			{
				if (baseQuery.Result == null || baseQuery.Result.ExceptionInfo == null)
				{
					queryList.Add(baseQuery);
				}
			}
			if (queryList.Count == 0)
			{
				GetFolderAndProxyRequest.GetFolderRequestTracer.TraceDebug((long)this.GetHashCode(), "{0}: Completed and no ProxyWebRequest is necessary.", new object[]
				{
					TraceContext.Get()
				});
				base.Complete();
				return;
			}
			this.proxyWebRequest = new ProxyWebRequest(base.Application, base.ClientContext, base.RequestType, base.RequestLogger, base.QueryList, this.targetVersion, this.proxyAuthenticator, this.webServiceUri, this.webServiceUri.Source);
			this.proxyWebRequest.BeginInvoke(new TaskCompleteCallback(this.Complete2));
		}

		private void Complete2(AsyncTask task)
		{
			GetFolderAndProxyRequest.GetFolderRequestTracer.TraceDebug((long)this.GetHashCode(), "{0}: Completed ProxyWebRequest.", new object[]
			{
				TraceContext.Get()
			});
			base.Complete();
		}

		private void CheckResultsOfGetFolder()
		{
			foreach (GetFolderRequest getFolderRequest in this.getFolderRequests)
			{
				BaseQuery query = getFolderRequest.Query;
				if (string.IsNullOrEmpty(getFolderRequest.ResultFolderId))
				{
					GetFolderAndProxyRequest.GetFolderRequestTracer.TraceDebug<object, EmailAddress>((long)this.GetHashCode(), "{0}: No ResultFolderId returned for mailbox {1}.", TraceContext.Get(), query.Email);
				}
				else
				{
					StoreObjectId storeObjectId = StoreId.EwsIdToStoreObjectId(getFolderRequest.ResultFolderId);
					StoreObjectId associatedFolderId = query.RecipientData.AssociatedFolderId;
					if (!associatedFolderId.Equals(storeObjectId))
					{
						GetFolderAndProxyRequest.GetFolderRequestTracer.TraceDebug((long)this.GetHashCode(), "{0}: Requested folder id {2} doesn't match default folder id {3} for mailbox {1}.", new object[]
						{
							TraceContext.Get(),
							query.Email,
							associatedFolderId,
							storeObjectId
						});
						query.SetResultOnFirstCall(base.Application.CreateQueryResult(new NotDefaultCalendarException()));
					}
				}
			}
		}

		public override string ToString()
		{
			return "GetFolderAndProxyRequest for " + base.QueryList.Count + " mailboxes";
		}

		private AsyncTaskParallel parallel;

		private GetFolderRequest[] getFolderRequests;

		private ProxyWebRequest proxyWebRequest;

		private TargetServerVersion targetVersion;

		private ProxyAuthenticator proxyAuthenticator;

		private WebServiceUri webServiceUri;

		private static readonly Trace GetFolderRequestTracer = ExTraceGlobals.GetFolderRequestTracer;
	}
}
