using System;
using System.Security.Principal;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net.XropService;
using Microsoft.Exchange.Rpc;

namespace Microsoft.Exchange.RpcClientAccess.Server
{
	internal sealed class WebServiceServerSession : DisposeTrackableBase, IServerSession
	{
		public WebServiceServerSession(WindowsIdentity userIdentity, WebServiceUserInformation userInformation, IExchangeAsyncDispatch asyncDispatch)
		{
			this.asyncDispatch = asyncDispatch;
			this.userIdentity = userIdentity;
			this.userInformation = userInformation;
			this.contextHandle = IntPtr.Zero;
		}

		public IAsyncResult BeginConnect(ConnectRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			WebServiceCallStateConnect webServiceCallStateConnect = new WebServiceCallStateConnect(this.userInformation, this.asyncDispatch, asyncCallback, asyncState, this.contextHandle, this.userIdentity);
			return webServiceCallStateConnect.Begin(request);
		}

		public ConnectResponse EndConnect(IAsyncResult asyncResult)
		{
			WebServiceCallStateConnect webServiceCallStateConnect = (WebServiceCallStateConnect)asyncResult;
			ConnectResponse result;
			try
			{
				result = webServiceCallStateConnect.End();
			}
			finally
			{
				this.contextHandle = webServiceCallStateConnect.ContextHandle;
			}
			return result;
		}

		public IAsyncResult BeginExecute(ExecuteRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			WebServiceCallStateExecute webServiceCallStateExecute = new WebServiceCallStateExecute(this.userInformation, this.asyncDispatch, asyncCallback, asyncState, this.contextHandle);
			return webServiceCallStateExecute.Begin(request);
		}

		public ExecuteResponse EndExecute(IAsyncResult asyncResult)
		{
			WebServiceCallStateExecute webServiceCallStateExecute = (WebServiceCallStateExecute)asyncResult;
			ExecuteResponse result;
			try
			{
				result = webServiceCallStateExecute.End();
			}
			finally
			{
				this.contextHandle = webServiceCallStateExecute.ContextHandle;
			}
			return result;
		}

		public IAsyncResult BeginDisconnect(DisconnectRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			WebServiceCallStateDisconnect webServiceCallStateDisconnect = new WebServiceCallStateDisconnect(this.userInformation, this.asyncDispatch, asyncCallback, asyncState, this.contextHandle);
			return webServiceCallStateDisconnect.Begin(request);
		}

		public DisconnectResponse EndDisconnect(IAsyncResult asyncResult)
		{
			WebServiceCallStateDisconnect webServiceCallStateDisconnect = (WebServiceCallStateDisconnect)asyncResult;
			DisconnectResponse result;
			try
			{
				result = webServiceCallStateDisconnect.End();
			}
			finally
			{
				this.contextHandle = webServiceCallStateDisconnect.ContextHandle;
			}
			return result;
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing && this.asyncDispatch != null && this.contextHandle != IntPtr.Zero)
			{
				try
				{
					this.asyncDispatch.ContextHandleRundown(this.contextHandle);
				}
				catch (RpcServiceException)
				{
				}
				catch (OutOfMemoryException)
				{
				}
				this.contextHandle = IntPtr.Zero;
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<WebServiceServerSession>(this);
		}

		private readonly IExchangeAsyncDispatch asyncDispatch;

		private readonly WindowsIdentity userIdentity;

		private readonly WebServiceUserInformation userInformation;

		private IntPtr contextHandle;
	}
}
