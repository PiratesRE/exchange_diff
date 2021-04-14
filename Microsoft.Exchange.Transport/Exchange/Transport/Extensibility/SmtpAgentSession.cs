using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Internal.MExRuntime;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Transport;
using Microsoft.Exchange.Protocols.Smtp;

namespace Microsoft.Exchange.Transport.Extensibility
{
	internal class SmtpAgentSession : ISmtpAgentSession
	{
		public SmtpAgentSession(IMExRuntime mexRuntime, ICloneableInternal smtpReceiveServer, ISmtpInSession smtpInSession, SmtpSession smtpSession) : this(mexRuntime, smtpReceiveServer, smtpSession)
		{
			ArgumentValidator.ThrowIfNull("smtpInSession", smtpInSession);
			this.smtpInSession = smtpInSession;
			this.smtpInSession.MexSession = this.mexSession;
		}

		public SmtpAgentSession(IMExRuntime mexRuntime, ICloneableInternal smtpReceiveServer, SmtpInSessionState sessionState, out IMExSession mexSession) : this(mexRuntime, smtpReceiveServer, sessionState)
		{
			ArgumentValidator.ThrowIfNull("sessionState", sessionState);
			mexSession = this.mexSession;
			this.sessionState = sessionState;
			this.sessionState.MexRuntimeSession = this.mexSession;
		}

		private SmtpAgentSession(IMExRuntime mexRuntime, ICloneableInternal smtpReceiveServer, SmtpSession smtpSession)
		{
			ArgumentValidator.ThrowIfNull("mexRuntime", mexRuntime);
			ArgumentValidator.ThrowIfNull("smtpReceiveServer", smtpReceiveServer);
			ArgumentValidator.ThrowIfNull("smtpSession", smtpSession);
			this.mexSession = this.CreaateMExSession(mexRuntime, smtpReceiveServer);
			this.smtpSession = smtpSession;
			this.smtpSession.ExecutionControl = this.mexSession;
			this.latencyTracker = new AgentLatencyTracker(this.mexSession);
		}

		public IAsyncResult BeginNoEvent(AsyncCallback callback, object state)
		{
			IAsyncResult asyncResult = new SmtpAgentSession.NoEventAsyncResult(state);
			callback(asyncResult);
			return asyncResult;
		}

		public IAsyncResult BeginRaiseEvent(string eventTopic, object eventSource, object eventArgs, AsyncCallback callback, object state)
		{
			return this.mexSession.BeginInvoke(eventTopic, eventSource, eventArgs, callback, state);
		}

		public SmtpResponse EndRaiseEvent(IAsyncResult ar)
		{
			if (ar == null || ar is SmtpAgentSession.NoEventAsyncResult)
			{
				return SmtpResponse.Empty;
			}
			try
			{
				this.mexSession.EndInvoke(ar);
			}
			catch (Exception e)
			{
				SmtpResponse result;
				if (!this.HandleEndInvokeException(e, out result))
				{
					throw;
				}
				return result;
			}
			return SmtpResponse.Empty;
		}

		public Task<SmtpResponse> RaiseEventAsync(string eventTopic, object eventSource, object eventArgs)
		{
			return Task.Factory.FromAsync<string, object, object, SmtpResponse>(new Func<string, object, object, AsyncCallback, object, IAsyncResult>(this.BeginRaiseEvent), new Func<IAsyncResult, SmtpResponse>(this.EndRaiseEvent), eventTopic, eventSource, eventArgs, null);
		}

		public SmtpSession SessionSource
		{
			get
			{
				return this.smtpSession;
			}
		}

		public AgentLatencyTracker LatencyTracker
		{
			get
			{
				return this.latencyTracker;
			}
		}

		public void Close()
		{
			this.latencyTracker.Dispose();
			this.latencyTracker = null;
			this.mexSession.Close();
		}

		protected virtual bool HandleEndInvokeException(Exception e, out SmtpResponse response)
		{
			return SmtpInExceptionHandler.ShouldHandleException(e, ExTraceGlobals.SmtpReceiveTracer, null, out response);
		}

		private void TrackAsyncMessage()
		{
			TransportMailItem transportMailItem = this.TransportMailItem;
			if (transportMailItem != null)
			{
				TransportMailItem.TrackAsyncMessage(transportMailItem.InternetMessageId);
			}
		}

		private void TrackAsyncMessageCompleted()
		{
			TransportMailItem transportMailItem = this.TransportMailItem;
			if (transportMailItem != null)
			{
				TransportMailItem.TrackAsyncMessageCompleted(transportMailItem.InternetMessageId);
			}
		}

		private IMExSession CreaateMExSession(IMExRuntime mexRuntime, ICloneableInternal smtpReceiveServer)
		{
			return mexRuntime.CreateSession(smtpReceiveServer, "SMTP", new Action(this.TrackAsyncMessage), new Action(this.TrackAsyncMessageCompleted), null);
		}

		private TransportMailItem TransportMailItem
		{
			get
			{
				if (this.smtpInSession != null)
				{
					return this.smtpInSession.TransportMailItem;
				}
				return this.sessionState.TransportMailItem;
			}
		}

		private AgentLatencyTracker latencyTracker;

		private readonly IMExSession mexSession;

		private readonly ISmtpInSession smtpInSession;

		private readonly SmtpInSessionState sessionState;

		private readonly SmtpSession smtpSession;

		private class NoEventAsyncResult : IAsyncResult
		{
			public NoEventAsyncResult(object state)
			{
				this.state = state;
			}

			public object AsyncState
			{
				get
				{
					return this.state;
				}
			}

			public bool CompletedSynchronously
			{
				get
				{
					return true;
				}
			}

			public WaitHandle AsyncWaitHandle
			{
				get
				{
					throw new NotImplementedException();
				}
			}

			public bool IsCompleted
			{
				get
				{
					return true;
				}
			}

			private readonly object state;
		}
	}
}
