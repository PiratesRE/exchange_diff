using System;
using System.Security;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxTransport.ContentAggregation;
using Microsoft.Exchange.Transport.Sync.Common;
using Microsoft.Exchange.Transport.Sync.Common.Logging;
using Microsoft.Exchange.Transport.Sync.Common.Subscription.Pim;
using Microsoft.Exchange.Transport.Sync.Worker.Framework.Provider.IMAP.Client;

namespace Microsoft.Exchange.Transport.Sync.Worker.Framework.Provider.IMAP
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class IMAPAutoProvisionClient : DisposeTrackableBase
	{
		public IMAPAutoProvisionClient(string host, int port, string username, SecureString password, IMAPAuthenticationMechanism authMechanism, IMAPSecurityMechanism secMechanism, AggregationType aggregationType, int connectionTimeout, int maxDownload, SyncLogSession logSession)
		{
			this.clientState = new IMAPClientState(new Fqdn(host), port, username, password, null, logSession, SyncUtilities.GetNextSessionId(), Guid.NewGuid(), authMechanism, secMechanism, aggregationType, (long)maxDownload, connectionTimeout, null, null, null, null);
		}

		public IAsyncResult BeginVerifyAccount(AsyncCallback callback, object callbackState, object syncPoisonContext)
		{
			base.CheckDisposed();
			AsyncResult<IMAPClientState, DBNull> asyncResult = new AsyncResult<IMAPClientState, DBNull>(this, this.clientState, callback, callbackState, syncPoisonContext);
			asyncResult.PendingAsyncResult = IMAPClient.BeginConnectAndAuthenticate(this.clientState, new AsyncCallback(IMAPAutoProvisionClient.OnEndConnectAndAuthenticate), asyncResult, syncPoisonContext);
			return asyncResult;
		}

		public AsyncOperationResult<DBNull> EndVerifyAccount(IAsyncResult asyncResult)
		{
			base.CheckDisposed();
			if (asyncResult == null)
			{
				throw new ArgumentNullException("asyncResult");
			}
			AsyncResult<IMAPClientState, DBNull> asyncResult2 = (AsyncResult<IMAPClientState, DBNull>)asyncResult;
			return asyncResult2.WaitForCompletion();
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing && this.clientState != null)
			{
				this.clientState.Dispose();
				this.clientState = null;
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<IMAPAutoProvisionClient>(this);
		}

		private static void OnEndConnectAndAuthenticate(IAsyncResult asyncResult)
		{
			AsyncResult<IMAPClientState, DBNull> asyncResult2 = (AsyncResult<IMAPClientState, DBNull>)asyncResult.AsyncState;
			IMAPClientState state = asyncResult2.State;
			AsyncOperationResult<DBNull> asyncOperationResult = IMAPClient.EndConnectAndAuthenticate(asyncResult);
			if (asyncResult.CompletedSynchronously)
			{
				asyncResult2.SetCompletedSynchronously();
			}
			if (!asyncOperationResult.IsSucceeded)
			{
				state.Log.LogError((TSLID)676UL, "Failed to authenticate and login.", new object[0]);
				asyncResult2.ProcessCompleted(null, asyncOperationResult.Exception);
				return;
			}
			asyncResult2.PendingAsyncResult = IMAPClient.BeginLogOff(state, new AsyncCallback(IMAPAutoProvisionClient.OnEndLogOff), asyncResult2, asyncResult2.SyncPoisonContext);
			asyncResult2.ProcessCompleted();
		}

		private static void OnEndLogOff(IAsyncResult asyncResult)
		{
		}

		private IMAPClientState clientState;
	}
}
