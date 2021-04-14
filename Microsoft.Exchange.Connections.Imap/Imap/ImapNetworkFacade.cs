using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.Sockets;
using Microsoft.Exchange.Connections.Common;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ContentAggregation;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Security;

namespace Microsoft.Exchange.Connections.Imap
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class ImapNetworkFacade : DisposeTrackableBase, INetworkFacade, IDisposeTrackable, IDisposable
	{
		internal ImapNetworkFacade(ConnectionParameters connectionParameters, ImapServerParameters serverParameters)
		{
			this.connectionParameters = connectionParameters;
			this.serverParameters = serverParameters;
			this.Log = connectionParameters.Log;
			this.currentResponse = new ImapResponse(this.Log);
		}

		public long TotalBytesSent
		{
			get
			{
				base.CheckDisposed();
				return this.totalBytesSent;
			}
		}

		public long TotalBytesReceived
		{
			get
			{
				base.CheckDisposed();
				return this.totalBytesReceived;
			}
		}

		public ulong TotalMessagesReceived
		{
			get
			{
				base.CheckDisposed();
				return this.totalMessagesReceived;
			}
		}

		public bool IsConnected
		{
			get
			{
				base.CheckDisposed();
				return this.networkConnection != null && !this.isNetworkConnectionShutdown;
			}
		}

		public string Server
		{
			get
			{
				base.CheckDisposed();
				return this.serverParameters.Server;
			}
		}

		private ILog Log { get; set; }

		public IAsyncResult BeginConnect(ImapConnectionContext imapConnectionContext, AsyncCallback callback, object callbackState)
		{
			base.CheckDisposed();
			this.ResetStateForConnectIfNecessary();
			this.socket = SocketFactory.CreateTcpStreamSocket();
			AsyncResult<ImapConnectionContext, ImapResultData> asyncResult = new AsyncResult<ImapConnectionContext, ImapResultData>(this, imapConnectionContext, callback, callbackState);
			try
			{
				asyncResult.PendingAsyncResult = this.socket.BeginConnect(this.serverParameters.Server, this.serverParameters.Port, new AsyncCallback(this.OnEndConnectInternalReadResponse), asyncResult);
			}
			catch (SocketException errorCode)
			{
				this.socket = null;
				ImapNetworkFacade.HandleError(errorCode, asyncResult);
			}
			return asyncResult;
		}

		public AsyncOperationResult<ImapResultData> EndConnect(IAsyncResult asyncResult)
		{
			base.CheckDisposed();
			AsyncResult<ImapConnectionContext, ImapResultData> asyncResult2 = (AsyncResult<ImapConnectionContext, ImapResultData>)asyncResult;
			return asyncResult2.WaitForCompletion();
		}

		public IAsyncResult BeginNegotiateTlsAsClient(ImapConnectionContext imapConnectionContext, AsyncCallback callback, object callbackState)
		{
			base.CheckDisposed();
			AsyncResult<ImapConnectionContext, ImapResultData> asyncResult = new AsyncResult<ImapConnectionContext, ImapResultData>(this, imapConnectionContext, callback, callbackState);
			asyncResult.PendingAsyncResult = this.networkConnection.BeginNegotiateTlsAsClient(null, this.networkConnection.RemoteEndPoint.Address.ToString(), new AsyncCallback(this.OnEndConnectNegotiateTlsAsClientInternalReadResponse), asyncResult);
			return asyncResult;
		}

		public AsyncOperationResult<ImapResultData> EndNegotiateTlsAsClient(IAsyncResult asyncResult)
		{
			base.CheckDisposed();
			AsyncResult<ImapConnectionContext, ImapResultData> asyncResult2 = (AsyncResult<ImapConnectionContext, ImapResultData>)asyncResult;
			return asyncResult2.WaitForCompletion();
		}

		public IAsyncResult BeginCommand(ImapCommand command, ImapConnectionContext imapConnectionContext, AsyncCallback callback, object callbackState)
		{
			return this.BeginCommand(command, true, imapConnectionContext, callback, callbackState);
		}

		public IAsyncResult BeginCommand(ImapCommand command, bool processResponse, ImapConnectionContext imapConnectionContext, AsyncCallback callback, object callbackState)
		{
			base.CheckDisposed();
			byte[] array = command.ToBytes();
			this.currentResponse.Reset(command);
			this.currentCommand = command;
			AsyncResult<ImapConnectionContext, ImapResultData> asyncResult = new AsyncResult<ImapConnectionContext, ImapResultData>(this, imapConnectionContext, callback, callbackState);
			asyncResult.State.TimeSent = ExDateTime.MinValue;
			if (this.cancellationRequested)
			{
				asyncResult.SetCompletedSynchronously();
				this.HandleCancellation(asyncResult);
				return asyncResult;
			}
			if (this.isNetworkConnectionShutdown)
			{
				asyncResult.SetCompletedSynchronously();
				ImapNetworkFacade.HandleError(ImapNetworkFacade.GetConnectionClosedException(), asyncResult);
				return asyncResult;
			}
			this.totalBytesSent += (long)array.Length;
			if (this.totalBytesSent > this.connectionParameters.MaxBytesToTransfer)
			{
				asyncResult.SetCompletedSynchronously();
				ImapNetworkFacade.HandleError(ImapNetworkFacade.MaxBytesSentExceeded(), asyncResult);
				return asyncResult;
			}
			if (processResponse && this.totalBytesReceived > this.connectionParameters.MaxBytesToTransfer)
			{
				asyncResult.State.Log.Debug("Not sending {0}, since we've exceeded our received-bytes threshold.", new object[]
				{
					this.currentCommand.ToPiiCleanString()
				});
				asyncResult.SetCompletedSynchronously();
				ImapNetworkFacade.HandleError(ImapNetworkFacade.MaxBytesReceivedExceeded(), asyncResult);
				return asyncResult;
			}
			asyncResult.State.Log.Info("IMAP Send command: [{0}]", new object[]
			{
				this.currentCommand.ToPiiCleanString()
			});
			DownloadCompleteEventArgs eventArgs = new DownloadCompleteEventArgs(0L, (long)array.Length);
			imapConnectionContext.ActivatePerfDownloadEvent(imapConnectionContext, eventArgs);
			if (processResponse)
			{
				asyncResult.PendingAsyncResult = this.networkConnection.BeginWrite(array, 0, array.Length, new AsyncCallback(this.OnEndWriteCommandOrLiteralBeginReadResponse), asyncResult);
			}
			else
			{
				asyncResult.PendingAsyncResult = this.networkConnection.BeginWrite(array, 0, array.Length, new AsyncCallback(this.OnEndSendCommandIgnoreResponse), asyncResult);
				asyncResult.SetCompletedSynchronously();
				this.currentResultData.Clear();
				asyncResult.ProcessCompleted(this.currentResultData);
			}
			return asyncResult;
		}

		public AsyncOperationResult<ImapResultData> EndCommand(IAsyncResult asyncResult)
		{
			base.CheckDisposed();
			AsyncResult<ImapConnectionContext, ImapResultData> asyncResult2 = (AsyncResult<ImapConnectionContext, ImapResultData>)asyncResult;
			return asyncResult2.WaitForCompletion();
		}

		public void Cancel()
		{
			lock (this.syncRoot)
			{
				base.CheckDisposed();
				if (!this.cancellationRequested)
				{
					this.cancellationRequested = true;
					if (this.networkConnection != null)
					{
						this.networkConnection.Shutdown();
					}
				}
			}
		}

		public override void Dispose()
		{
			lock (this.syncRoot)
			{
				base.Dispose();
			}
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				this.CloseConnections();
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<ImapNetworkFacade>(this);
		}

		private static Exception BuildTimeoutException()
		{
			Exception result;
			if ((result = ImapNetworkFacade.timeoutException) == null)
			{
				result = (ImapNetworkFacade.timeoutException = new ImapConnectionException(CXStrings.ImapServerTimeout, RetryPolicy.Backoff));
			}
			return result;
		}

		private static Exception BuildConnectionShutdownException()
		{
			Exception result;
			if ((result = ImapNetworkFacade.connectionShutdownException) == null)
			{
				result = (ImapNetworkFacade.connectionShutdownException = new ImapConnectionException(CXStrings.ImapServerShutdown, RetryPolicy.Backoff));
			}
			return result;
		}

		private static Exception BuildUnknownNetworkException()
		{
			Exception result;
			if ((result = ImapNetworkFacade.unknownFailureException) == null)
			{
				result = (ImapNetworkFacade.unknownFailureException = new ImapCommunicationException(CXStrings.ImapServerNetworkError, RetryPolicy.Backoff));
			}
			return result;
		}

		private static Exception GetConnectionClosedException()
		{
			Exception result;
			if ((result = ImapNetworkFacade.connectionClosedException) == null)
			{
				result = (ImapNetworkFacade.connectionClosedException = new ImapConnectionClosedException(CXStrings.ImapServerConnectionClosed));
			}
			return result;
		}

		private static Exception MaxBytesSentExceeded()
		{
			Exception result;
			if ((result = ImapNetworkFacade.maxBytesSentException) == null)
			{
				result = (ImapNetworkFacade.maxBytesSentException = new ItemLimitExceededException(CXStrings.ImapMaxBytesSentExceeded));
			}
			return result;
		}

		private static Exception MaxBytesReceivedExceeded()
		{
			Exception result;
			if ((result = ImapNetworkFacade.maxBytesReceivedException) == null)
			{
				result = (ImapNetworkFacade.maxBytesReceivedException = new ItemLimitExceededException(CXStrings.ImapMaxBytesReceivedExceeded));
			}
			return result;
		}

		private static void CountCommand(AsyncResult<ImapConnectionContext, ImapResultData> curOp, bool successful)
		{
			string server = ((ImapNetworkFacade)curOp.State.NetworkFacade).serverParameters.Server;
			if (curOp.State.TimeSent != ExDateTime.MinValue)
			{
				curOp.State.NotifyRoundtripComplete(null, new RoundtripCompleteEventArgs(ExDateTime.UtcNow - curOp.State.TimeSent, successful, server));
				curOp.State.TimeSent = ExDateTime.MinValue;
			}
		}

		private static void HandleError(object errorCode, AsyncResult<ImapConnectionContext, ImapResultData> curOp)
		{
			ImapNetworkFacade.CountCommand(curOp, false);
			if (errorCode is ConnectionsPermanentException || errorCode is ConnectionsTransientException)
			{
				curOp.State.Log.Error("Exception while communicating: {0}.", new object[]
				{
					errorCode
				});
				curOp.ProcessCompleted((Exception)errorCode);
				return;
			}
			ImapNetworkFacade imapNetworkFacade = (ImapNetworkFacade)curOp.State.NetworkFacade;
			if (imapNetworkFacade.networkConnection != null)
			{
				imapNetworkFacade.networkConnection.Shutdown();
				imapNetworkFacade.isNetworkConnectionShutdown = true;
			}
			if (errorCode is SocketError)
			{
				switch ((SocketError)errorCode)
				{
				case SocketError.Shutdown:
					curOp.State.Log.Error("Network connection shut down: {0}.", new object[]
					{
						errorCode
					});
					curOp.ProcessCompleted(ImapNetworkFacade.BuildConnectionShutdownException());
					return;
				case SocketError.TimedOut:
					curOp.State.Log.Error("Operation timed out, shutting down network connection: {0}.", new object[]
					{
						errorCode
					});
					curOp.ProcessCompleted(ImapNetworkFacade.BuildTimeoutException());
					return;
				}
				curOp.State.Log.Error("HandleError unhandled SocketError={0}, shutting down network connection.", new object[]
				{
					((SocketError)errorCode).ToString()
				});
				curOp.ProcessCompleted(ImapNetworkFacade.BuildUnknownNetworkException());
				return;
			}
			if (errorCode is SecurityStatus)
			{
				curOp.State.Log.Error("ImapNetworkFacade.HandleError (SecurityStatus={0}), shutting down network connection.", new object[]
				{
					errorCode
				});
				LocalizedString value = CXStrings.TlsFailureOccurredError(((SecurityStatus)errorCode).ToString());
				TlsFailureException innerException = new TlsFailureException(value);
				curOp.ProcessCompleted(new ImapConnectionException(CXStrings.ImapSecurityStatusError, RetryPolicy.Backoff, innerException));
				return;
			}
			curOp.State.Log.Error("HandleError unknown error type (error={0}), shutting down network connection.", new object[]
			{
				errorCode
			});
			curOp.ProcessCompleted(ImapNetworkFacade.BuildUnknownNetworkException());
		}

		private IAsyncResult BeginNetworkRead(AsyncResult<ImapConnectionContext, ImapResultData> curOp, AsyncCallback asyncCallback)
		{
			IAsyncResult result;
			try
			{
				result = this.networkConnection.BeginRead(asyncCallback, curOp);
			}
			catch (InvalidOperationException ex)
			{
				string text = string.Format("BUG: BeginNetworkRead : BeginRead should never throw InvalidOperationException.  Happened during {0}.", this.currentCommand.ToPiiCleanString());
				this.Log.Fatal(ex, text);
				curOp.ProcessCompleted(new ImapCommunicationException(text, RetryPolicy.Backoff, ex));
				result = null;
			}
			return result;
		}

		private void CloseConnections()
		{
			if (this.networkConnection != null)
			{
				this.networkConnection.Dispose();
				this.networkConnection = null;
			}
			if (this.socket != null && this.socket.Connected)
			{
				this.socket.Close();
			}
			this.socket = null;
		}

		private void OnEndConnectInternalReadResponse(IAsyncResult asyncResult)
		{
			AsyncResult<ImapConnectionContext, ImapResultData> asyncResult2 = null;
			ImapConnectionContext imapConnectionContext = null;
			lock (this.syncRoot)
			{
				if (!this.ShouldCancelCallback())
				{
					base.CheckDisposed();
					asyncResult2 = (AsyncResult<ImapConnectionContext, ImapResultData>)asyncResult.AsyncState;
					imapConnectionContext = asyncResult2.State;
					try
					{
						Socket socket = this.socket;
						socket.EndConnect(asyncResult);
						if (this.cancellationRequested)
						{
							this.HandleCancellation(asyncResult2);
							this.socket = null;
							return;
						}
						this.networkConnection = new NetworkConnection(this.socket, 4096);
						this.socket = null;
						this.networkConnection.Timeout = (this.connectionParameters.Timeout + 999) / 1000;
						this.Log.Debug("Connection Completed. Connection ID : {0}, Remote End Point {1}", new object[]
						{
							this.networkConnection.ConnectionId,
							this.networkConnection.RemoteEndPoint
						});
					}
					catch (SocketException ex)
					{
						ImapUtilities.LogExceptionDetails(this.Log, "Failed to connect, SocketException", ex);
						this.socket = null;
						asyncResult2.ProcessCompleted(new ImapConnectionException(CXStrings.ImapSocketException, RetryPolicy.Backoff, ex));
						return;
					}
					this.Log.Info(string.Format(CultureInfo.InvariantCulture, "Connect, from:{0} to:{1}", new object[]
					{
						this.serverParameters.Server,
						this.networkConnection.RemoteEndPoint
					}), new object[0]);
					switch (imapConnectionContext.ImapSecurityMechanism)
					{
					case ImapSecurityMechanism.None:
					case ImapSecurityMechanism.Tls:
						asyncResult2.PendingAsyncResult = this.BeginNetworkRead(asyncResult2, new AsyncCallback(this.OnReadAndDiscardLine));
						break;
					case ImapSecurityMechanism.Ssl:
						asyncResult2.PendingAsyncResult = this.networkConnection.BeginNegotiateTlsAsClient(null, this.networkConnection.RemoteEndPoint.Address.ToString(), new AsyncCallback(this.OnEndConnectNegotiateTlsAsClientInternalReadResponse), asyncResult2);
						break;
					default:
						throw new InvalidOperationException("Unexpected security mechanism " + imapConnectionContext.ImapSecurityMechanism);
					}
				}
			}
		}

		private void OnEndSendCommandIgnoreResponse(IAsyncResult asyncResult)
		{
			NetworkConnection networkConnection = this.ExtractNetworkConnectionFrom(asyncResult);
			object obj;
			networkConnection.EndWrite(asyncResult, out obj);
		}

		private void OnEndConnectNegotiateTlsAsClientInternalReadResponse(IAsyncResult asyncResult)
		{
			lock (this.syncRoot)
			{
				if (this.ShouldCancelCallback())
				{
					NetworkConnection networkConnection = this.ExtractNetworkConnectionFrom(asyncResult);
					object obj2;
					networkConnection.EndNegotiateTlsAsClient(asyncResult, out obj2);
				}
				else
				{
					base.CheckDisposed();
					AsyncResult<ImapConnectionContext, ImapResultData> asyncResult2 = (AsyncResult<ImapConnectionContext, ImapResultData>)asyncResult.AsyncState;
					ImapConnectionContext state = asyncResult2.State;
					object obj2;
					this.networkConnection.EndNegotiateTlsAsClient(asyncResult, out obj2);
					if (obj2 != null)
					{
						ImapNetworkFacade.HandleError(obj2, asyncResult2);
					}
					else if (this.cancellationRequested)
					{
						this.HandleCancellation(asyncResult2);
					}
					else
					{
						ExTraceGlobals.FaultInjectionTracer.TraceTest(3211144509U);
						switch (state.ImapSecurityMechanism)
						{
						case ImapSecurityMechanism.Ssl:
							this.currentResponse.Reset(null);
							asyncResult2.PendingAsyncResult = this.BeginNetworkRead(asyncResult2, new AsyncCallback(this.OnReadAndDiscardLine));
							break;
						case ImapSecurityMechanism.Tls:
							asyncResult2.ProcessCompleted();
							break;
						default:
							throw new InvalidOperationException("Invalid security mechanism");
						}
					}
				}
			}
		}

		private void OnReadAndDiscardLine(IAsyncResult asyncResult)
		{
			lock (this.syncRoot)
			{
				if (this.ShouldCancelCallback())
				{
					NetworkConnection networkConnection = this.ExtractNetworkConnectionFrom(asyncResult);
					byte[] data;
					int offset;
					int num;
					object obj2;
					networkConnection.EndRead(asyncResult, out data, out offset, out num, out obj2);
				}
				else
				{
					base.CheckDisposed();
					AsyncResult<ImapConnectionContext, ImapResultData> asyncResult2 = (AsyncResult<ImapConnectionContext, ImapResultData>)asyncResult.AsyncState;
					ImapResponse imapResponse = this.currentResponse;
					byte[] data;
					int offset;
					int num;
					object obj2;
					this.networkConnection.EndRead(asyncResult, out data, out offset, out num, out obj2);
					if (obj2 != null)
					{
						ImapNetworkFacade.HandleError(obj2, asyncResult2);
					}
					else if (this.cancellationRequested)
					{
						this.HandleCancellation(asyncResult2);
					}
					else
					{
						ExTraceGlobals.FaultInjectionTracer.TraceTest(4284886333U);
						this.totalBytesReceived += (long)num;
						DownloadCompleteEventArgs eventArgs = new DownloadCompleteEventArgs((long)num, 0L);
						asyncResult2.State.ActivatePerfDownloadEvent(asyncResult2.State, eventArgs);
						if (this.totalBytesReceived > this.connectionParameters.MaxBytesToTransfer)
						{
							ImapNetworkFacade.HandleError(ImapNetworkFacade.MaxBytesReceivedExceeded(), asyncResult2);
						}
						else
						{
							int num2 = imapResponse.AddData(data, offset, num);
							int num3 = num - num2;
							this.Log.Assert(num3 >= 0, "The unconsumed bytes must be non-negative.", new object[0]);
							if (num3 > 0)
							{
								this.networkConnection.PutBackReceivedBytes(num3);
							}
							if (imapResponse.IsComplete)
							{
								this.currentResultData.Clear();
								this.currentResultData.Status = imapResponse.Status;
								if (asyncResult.CompletedSynchronously)
								{
									asyncResult2.SetCompletedSynchronously();
								}
								asyncResult2.State.Log.Assert(this.currentCommand == null, "this.currentCommand is expected to be null", new object[0]);
								if (imapResponse.Status == ImapStatus.No || imapResponse.Status == ImapStatus.Bad || imapResponse.Status == ImapStatus.Bye)
								{
									this.LogFailureDetails("Connecting", imapResponse);
									asyncResult2.ProcessCompleted(this.currentResultData);
								}
								else
								{
									asyncResult2.State.Log.Assert(imapResponse.ResponseLines != null && imapResponse.ResponseLines.Count > 0, "ResponseLines was null or had no content", new object[0]);
									if (imapResponse.Status != ImapStatus.Ok)
									{
										asyncResult2.ProcessCompleted(this.BuildAndLogUnknownCommandFailureException(asyncResult2.State, "Connecting"));
									}
									else
									{
										asyncResult2.ProcessCompleted(this.currentResultData);
									}
								}
							}
							else
							{
								asyncResult2.PendingAsyncResult = this.BeginNetworkRead(asyncResult2, new AsyncCallback(this.OnReadAndDiscardLine));
							}
						}
					}
				}
			}
		}

		private void OnEndWriteLiteral(IAsyncResult asyncResult)
		{
			lock (this.syncRoot)
			{
				if (this.ShouldCancelCallback())
				{
					NetworkConnection networkConnection = this.ExtractNetworkConnectionFrom(asyncResult);
					object obj2;
					networkConnection.EndWrite(asyncResult, out obj2);
				}
				else
				{
					base.CheckDisposed();
					AsyncResult<ImapConnectionContext, ImapResultData> asyncResult2 = (AsyncResult<ImapConnectionContext, ImapResultData>)asyncResult.AsyncState;
					object obj2;
					this.networkConnection.EndWrite(asyncResult, out obj2);
					if (obj2 != null)
					{
						ImapNetworkFacade.HandleError(obj2, asyncResult2);
					}
					else if (this.cancellationRequested)
					{
						this.HandleCancellation(asyncResult2);
					}
					else
					{
						ExTraceGlobals.FaultInjectionTracer.TraceTest(3748015421U);
						this.totalBytesSent += (long)ImapNetworkFacade.BytesCrLf.Length;
						DownloadCompleteEventArgs eventArgs = new DownloadCompleteEventArgs(0L, (long)ImapNetworkFacade.BytesCrLf.Length);
						asyncResult2.State.ActivatePerfDownloadEvent(asyncResult2.State, eventArgs);
						asyncResult2.State.Log.Debug("Literal sent, sending CRLF to complete it.", new object[0]);
						asyncResult2.PendingAsyncResult = this.networkConnection.BeginWrite(ImapNetworkFacade.BytesCrLf, 0, ImapNetworkFacade.BytesCrLf.Length, new AsyncCallback(this.OnEndWriteCommandOrLiteralBeginReadResponse), asyncResult2);
					}
				}
			}
		}

		private void OnEndWriteCommandOrLiteralBeginReadResponse(IAsyncResult asyncResult)
		{
			lock (this.syncRoot)
			{
				if (this.ShouldCancelCallback())
				{
					NetworkConnection networkConnection = this.ExtractNetworkConnectionFrom(asyncResult);
					object obj2;
					networkConnection.EndWrite(asyncResult, out obj2);
				}
				else
				{
					base.CheckDisposed();
					AsyncResult<ImapConnectionContext, ImapResultData> asyncResult2 = (AsyncResult<ImapConnectionContext, ImapResultData>)asyncResult.AsyncState;
					object obj2;
					this.networkConnection.EndWrite(asyncResult, out obj2);
					if (obj2 != null)
					{
						ImapNetworkFacade.HandleError(obj2, asyncResult2);
					}
					else if (this.cancellationRequested)
					{
						this.HandleCancellation(asyncResult2);
					}
					else
					{
						ExTraceGlobals.FaultInjectionTracer.TraceTest(2942709053U);
						asyncResult2.State.Log.Debug("Command/literal sent, begin reading response.", new object[0]);
						this.currentResponse.Reset(this.currentCommand);
						this.currentResultData.Clear();
						asyncResult2.State.TimeSent = ExDateTime.UtcNow;
						asyncResult2.PendingAsyncResult = this.BeginNetworkRead(asyncResult2, new AsyncCallback(this.OnReadMoreResponse));
					}
				}
			}
		}

		private void OnReadMoreResponse(IAsyncResult asyncResult)
		{
			lock (this.syncRoot)
			{
				if (this.ShouldCancelCallback())
				{
					NetworkConnection networkConnection = this.ExtractNetworkConnectionFrom(asyncResult);
					byte[] data;
					int offset;
					int num;
					object obj2;
					networkConnection.EndRead(asyncResult, out data, out offset, out num, out obj2);
				}
				else
				{
					base.CheckDisposed();
					AsyncResult<ImapConnectionContext, ImapResultData> asyncResult2 = (AsyncResult<ImapConnectionContext, ImapResultData>)asyncResult.AsyncState;
					ImapResponse imapResponse = this.currentResponse;
					byte[] data;
					int offset;
					int num;
					object obj2;
					this.networkConnection.EndRead(asyncResult, out data, out offset, out num, out obj2);
					if (obj2 != null)
					{
						ImapNetworkFacade.HandleError(obj2, asyncResult2);
					}
					else if (this.cancellationRequested)
					{
						this.HandleCancellation(asyncResult2);
					}
					else
					{
						ExTraceGlobals.FaultInjectionTracer.TraceTest(2674273597U);
						this.totalBytesReceived += (long)num;
						DownloadCompleteEventArgs eventArgs = new DownloadCompleteEventArgs((long)num, 0L);
						asyncResult2.State.ActivatePerfDownloadEvent(asyncResult2.State, eventArgs);
						if (this.totalBytesReceived > this.connectionParameters.MaxBytesToTransfer)
						{
							ImapNetworkFacade.HandleError(ImapNetworkFacade.MaxBytesReceivedExceeded(), asyncResult2);
						}
						else
						{
							int num2 = imapResponse.AddData(data, offset, num);
							int num3 = num - num2;
							this.Log.Assert(num3 >= 0, "The unconsumed bytes must be non-negative.", new object[0]);
							if (num3 > 0)
							{
								this.networkConnection.PutBackReceivedBytes(num3);
							}
							if (imapResponse.IsComplete)
							{
								ImapNetworkFacade.CountCommand(asyncResult2, true);
								asyncResult2.State.Log.Debug("Command complete: [{0}].  Status = {1}", new object[]
								{
									this.currentCommand.ToPiiCleanString(),
									imapResponse.Status
								});
								this.currentResultData.Status = imapResponse.Status;
								if (asyncResult.CompletedSynchronously)
								{
									asyncResult2.SetCompletedSynchronously();
								}
								if (imapResponse.Status == ImapStatus.No || imapResponse.Status == ImapStatus.Bad || imapResponse.Status == ImapStatus.Bye)
								{
									this.LogFailureDetails(this.currentCommand, imapResponse);
									asyncResult2.ProcessCompleted(this.currentResultData);
								}
								else if (imapResponse.Status != ImapStatus.Ok)
								{
									this.LogFailureDetails(this.currentCommand, imapResponse);
									asyncResult2.ProcessCompleted(this.BuildAndLogUnknownCommandFailureException(asyncResult2.State));
								}
								else
								{
									if (!imapResponse.TryParseIntoResult(this.currentCommand, ref this.currentResultData))
									{
										if (this.currentResultData.FailureException == null)
										{
											if (this.currentResultData.MessageStream != null)
											{
												this.totalMessagesReceived += 1UL;
											}
											asyncResult2.ProcessCompleted(this.currentResultData, this.BuildAndLogUnknownParseFailureException(asyncResult2.State));
											return;
										}
										ImapUtilities.LogExceptionDetails(asyncResult2.State.Log, this.currentCommand, this.currentResultData.FailureException);
										this.LogFailureDetails(this.currentCommand, this.currentResponse);
									}
									else
									{
										asyncResult2.State.Log.Debug("Parsed server response succesfully.", new object[0]);
									}
									asyncResult2.ProcessCompleted(this.currentResultData);
								}
							}
							else if (imapResponse.IsWaitingForLiteral)
							{
								Exception ex = null;
								Stream commandParameterStream = this.currentCommand.GetCommandParameterStream(this.serverParameters.Server, imapResponse.GetLastResponseLine(), out ex);
								if (ex != null)
								{
									if (commandParameterStream != null)
									{
										commandParameterStream.Close();
									}
									ImapNetworkFacade.CountCommand(asyncResult2, false);
									asyncResult2.ProcessCompleted(ex);
								}
								else if (commandParameterStream == null)
								{
									ImapNetworkFacade.CountCommand(asyncResult2, false);
									asyncResult2.ProcessCompleted(this.BuildAndLogUnexpectedLiteralRequestException(asyncResult2.State));
								}
								else
								{
									this.totalBytesSent += commandParameterStream.Length;
									if (this.totalBytesSent > this.connectionParameters.MaxBytesToTransfer)
									{
										ImapNetworkFacade.HandleError(ImapNetworkFacade.MaxBytesSentExceeded(), asyncResult2);
									}
									else
									{
										eventArgs = new DownloadCompleteEventArgs(0L, commandParameterStream.Length);
										asyncResult2.State.ActivatePerfDownloadEvent(asyncResult2.State, eventArgs);
										asyncResult2.State.Log.Debug("Begin writing literal stream.", new object[0]);
										asyncResult2.PendingAsyncResult = this.networkConnection.BeginWrite(commandParameterStream, new AsyncCallback(this.OnEndWriteLiteral), asyncResult2);
									}
								}
							}
							else if (imapResponse.TotalLiteralBytesExpected > 0 && this.totalBytesReceived + (long)imapResponse.LiteralBytesRemaining > this.connectionParameters.MaxBytesToTransfer)
							{
								this.totalBytesReceived += (long)imapResponse.LiteralBytesRemaining;
								ImapNetworkFacade.HandleError(ImapNetworkFacade.MaxBytesReceivedExceeded(), asyncResult2);
							}
							else
							{
								asyncResult2.PendingAsyncResult = this.BeginNetworkRead(asyncResult2, new AsyncCallback(this.OnReadMoreResponse));
							}
						}
					}
				}
			}
		}

		private Exception BuildAndLogUnknownParseFailureException(ImapConnectionContext context)
		{
			string text = string.Format(CultureInfo.InvariantCulture, "Unknown parse error in response.  Command = {0}", new object[]
			{
				this.currentCommand.ToPiiCleanString()
			});
			context.Log.Error(text, new object[0]);
			this.LogFailureDetails(this.currentCommand, this.currentResponse);
			return new ImapCommunicationException(text, RetryPolicy.Backoff);
		}

		private Exception BuildAndLogUnknownCommandFailureException(ImapConnectionContext context)
		{
			return this.BuildAndLogUnknownCommandFailureException(context, this.currentCommand.ToPiiCleanString());
		}

		private Exception BuildAndLogUnknownCommandFailureException(ImapConnectionContext context, string commandCleanString)
		{
			string text = string.Format(CultureInfo.InvariantCulture, "Unknown command failure, retry.  Command = {0}.", new object[]
			{
				commandCleanString
			});
			context.Log.Error(text, new object[0]);
			this.LogFailureDetails(commandCleanString, this.currentResponse);
			return new ImapCommunicationException(text, RetryPolicy.Immediate);
		}

		private Exception BuildAndLogUnexpectedLiteralRequestException(ImapConnectionContext context)
		{
			string text = string.Format(CultureInfo.InvariantCulture, "Server waiting for literal, but none given with command. Command = {0}.", new object[]
			{
				this.currentCommand.ToPiiCleanString()
			});
			context.Log.Error(text, new object[0]);
			this.LogFailureDetails(this.currentCommand, this.currentResponse);
			return new ImapCommunicationException(text, RetryPolicy.Backoff);
		}

		private void HandleCancellation(AsyncResult<ImapConnectionContext, ImapResultData> curOp)
		{
			ImapNetworkFacade.CountCommand(curOp, false);
			this.Log.Error("networkFacade operation cancelled.  Dropped connection.", new object[0]);
			Exception canceledException = AsyncOperationResult<ImapResultData>.CanceledException;
			curOp.ProcessCompleted(new ImapConnectionException(string.Empty, RetryPolicy.Backoff, canceledException));
		}

		private void LogFailureDetails(ImapCommand command, ImapResponse response)
		{
			this.LogFailureDetails(command.ToPiiCleanString(), response);
		}

		private void LogFailureDetails(string command, ImapResponse response)
		{
			this.Log.Error("Error while executing [{0}]", new object[]
			{
				command
			});
			IList<string> responseLines = response.ResponseLines;
			if (responseLines == null || responseLines.Count <= 0)
			{
				return;
			}
			int num = Math.Max(0, responseLines.Count - 10);
			for (int i = num; i < responseLines.Count; i++)
			{
				this.Log.Error("Response line [{0}]: {1}", new object[]
				{
					i,
					responseLines[i]
				});
			}
		}

		private NetworkConnection ExtractNetworkConnectionFrom(IAsyncResult asyncResult)
		{
			return ((LazyAsyncResult)asyncResult).AsyncObject as NetworkConnection;
		}

		private bool ShouldCancelCallback()
		{
			return base.IsDisposed && this.cancellationRequested;
		}

		private void ResetStateForConnectIfNecessary()
		{
			this.CloseConnections();
			this.currentCommand = null;
		}

		private static readonly byte[] BytesCrLf = new byte[]
		{
			Convert.ToByte('\r'),
			Convert.ToByte('\n')
		};

		private static Exception timeoutException;

		private static Exception connectionShutdownException;

		private static Exception unknownFailureException;

		private static Exception maxBytesSentException;

		private static Exception maxBytesReceivedException;

		private static Exception connectionClosedException;

		private readonly object syncRoot = new object();

		private readonly ConnectionParameters connectionParameters;

		private readonly ImapServerParameters serverParameters;

		private readonly ImapResponse currentResponse;

		private Socket socket;

		private NetworkConnection networkConnection;

		private long totalBytesSent;

		private long totalBytesReceived;

		private ulong totalMessagesReceived;

		private ImapCommand currentCommand;

		private ImapResultData currentResultData = new ImapResultData();

		private bool cancellationRequested;

		private bool isNetworkConnectionShutdown;
	}
}
