using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Microsoft.Office.Outlook;
using Microsoft.Office.Outlook.Network;
using Microsoft.Office.Outlook.V1.Mail;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.OutlookService
{
	public class SocketClient
	{
		public string ExtraInfo { get; set; }

		public ServiceApiClient Client { get; set; }

		public ManualResetEvent DoneEvent { get; set; }

		public Semaphore HandlingResponseSemaphore { get; set; }

		public SocketResponseType EventResponseType { get; set; }

		public string UnexpectedEventResponseMessage { get; set; }

		public SocketClient(ServiceApiClient client)
		{
			this.Client = client;
			this.DoneEvent = new ManualResetEvent(false);
			this.HandlingResponseSemaphore = new Semaphore(1, 1);
			this.ResetDefaultCallbacks();
		}

		public TResponse PostSynchronous<TRequest, TResponse>(TRequest request, TimeSpan timeout) where TRequest : RequestBase where TResponse : ResponseBase
		{
			this.DoneEvent.Reset();
			this.Client.PostAsync<TRequest>(request, 0UL);
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			do
			{
				try
				{
					this.fSignaled = this.DoneEvent.WaitOne(timeout);
					this.DoneEvent.Reset();
					if (!this.IsEventExpected())
					{
						return default(TResponse);
					}
					TResponse tresponse = (TResponse)((object)this.EventResponse);
					if (tresponse != null || !this.fSignaled)
					{
						return tresponse;
					}
				}
				finally
				{
					this.HandlingResponseSemaphore.Release();
				}
			}
			while (stopwatch.Elapsed < timeout);
			return default(TResponse);
		}

		protected void SetDefaultCallback<TResponse>() where TResponse : ResponseBase, new()
		{
			this.Client.OnAsyncResponse<TResponse>(delegate(TResponse response)
			{
				this.AppendExtraInfo("Received a response");
				this.HandlingResponseSemaphore.WaitOne();
				this.EventResponse = response;
				this.EventResponseType = 0;
				this.UnexpectedEventResponseMessage = "";
				this.DoneEvent.Set();
			});
		}

		protected void ResetDefaultCallbacksBase()
		{
			this.Client.OnAsyncDisconnect(delegate(SocketResponse response)
			{
				this.AppendExtraInfo("Disconnecting");
				if (response.StatusMessage != "Goodbye: Expired" && response.StatusMessage != "OK")
				{
					this.HandlingResponseSemaphore.WaitOne();
					this.AppendExtraInfo("DisconnectStatus:" + response.StatusMessage);
					this.EventResponseType = response.ResponseType;
					this.UnexpectedEventResponseMessage = response.StatusMessage;
					this.DoneEvent.Set();
				}
			});
			this.Client.OnUnknownVersionError(delegate(List<uint> knownVersions)
			{
				this.HandlingResponseSemaphore.WaitOne();
				this.AppendExtraInfo("UnknownVersionError=" + this.UnexpectedEventResponseMessage);
				this.EventResponseType = 3;
				this.UnexpectedEventResponseMessage = "Received OnUnknownVersionError SocketResponse";
				this.DoneEvent.Set();
			});
			this.Client.OnError(delegate(SocketResponse response)
			{
				this.HandlingResponseSemaphore.WaitOne();
				this.AppendExtraInfo("ResponseType=" + response.ResponseType.ToString());
				this.AppendExtraInfo("Error=" + response.StatusCode.ToString());
				this.AppendExtraInfo("Message=" + response.StatusMessage);
				this.EventResponseType = response.ResponseType;
				this.UnexpectedEventResponseMessage = response.StatusMessage;
				this.DoneEvent.Set();
			});
		}

		protected bool IsEventExpected()
		{
			bool result = true;
			switch (this.EventResponseType)
			{
			case 0:
			case 1:
			case 2:
			case 5:
				break;
			case 3:
				this.AppendExtraInfo("UnexpectedError=\r\n[start]\r\n" + this.UnexpectedEventResponseMessage + "\r\n[end]");
				result = false;
				break;
			case 4:
				this.AppendExtraInfo("UnexpectedDisconnect=\r\n[start]\r\n" + this.UnexpectedEventResponseMessage + "\r\n[end]");
				result = false;
				break;
			default:
				result = false;
				this.AppendExtraInfo("UnexpectedEvent=Unknown SocketResponseType in SocketResponse");
				break;
			}
			return result;
		}

		protected void CheckResponseErrors(string command, ResponseBase response)
		{
			if (!this.fSignaled)
			{
				this.AppendExtraInfo("ProbeFailure=Probe Timed Out");
				this.ExecutionSuccess = false;
				return;
			}
			if (response == null)
			{
				this.AppendExtraInfo("ProbeFailure:Probe Failed Empty Response Received");
				this.ExecutionSuccess = false;
				return;
			}
			if (command == null || command == "PingCommand" || !(command == "BeginSyncCommand"))
			{
				PingResponse pingResponse = (PingResponse)response;
				if (string.IsNullOrWhiteSpace(pingResponse.BuildNumber))
				{
					this.AppendExtraInfo("ProbeFailure= Unexpected Ping response failed to get service build version");
					this.ExecutionSuccess = false;
					return;
				}
			}
			else
			{
				BeginSyncResponse beginSyncResponse = (BeginSyncResponse)response;
				ResponseCode errorCode = beginSyncResponse.ErrorCode;
				if (errorCode != null)
				{
					this.AppendExtraInfo(string.Format("ProbeFailure= Server returned {0} response", beginSyncResponse.ErrorCode.ToString()));
					this.ExecutionSuccess = false;
				}
			}
		}

		protected void ResetDefaultCallbacks()
		{
			this.ResetDefaultCallbacksBase();
			this.SetDefaultCallback<PingResponse>();
			this.SetDefaultCallback<BeginSyncResponse>();
		}

		public PingResponse Ping(PingRequest pingRequest, TimeSpan timeout)
		{
			PingResponse pingResponse = this.PostSynchronous<PingRequest, PingResponse>(pingRequest, timeout);
			this.CheckResponseErrors("PingCommand", pingResponse);
			return pingResponse;
		}

		public BeginSyncResponse Sync(BeginSyncRequest syncRequest, TimeSpan timeout)
		{
			BeginSyncResponse beginSyncResponse = this.PostSynchronous<BeginSyncRequest, BeginSyncResponse>(syncRequest, timeout);
			this.CheckResponseErrors("BeginSyncCommand", beginSyncResponse);
			return beginSyncResponse;
		}

		private void AppendExtraInfo(string str)
		{
			if (string.IsNullOrWhiteSpace(this.ExtraInfo))
			{
				this.ExtraInfo = str;
				return;
			}
			this.ExtraInfo = string.Format("{0};{1}", this.ExtraInfo, str);
		}

		public bool ExecutionSuccess = true;

		public ResponseBase EventResponse;

		private bool fSignaled = true;
	}
}
