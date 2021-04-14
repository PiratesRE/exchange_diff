using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Net.Sockets;
using System.Security;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Common.IL;
using Microsoft.Exchange.Connections.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ContentAggregation;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Security;
using Microsoft.Exchange.Security.Authentication;

namespace Microsoft.Exchange.Connections.Pop
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class Pop3Client : DisposeTrackableBase
	{
		internal Pop3Client(ServerParameters serverParameters, Pop3AuthenticationParameters authenticationParameters, ConnectionParameters connectionParameters, EventHandler<DownloadCompleteEventArgs> downloadCompleted, EventHandler<RoundtripCompleteEventArgs> roundtripComplete)
		{
			this.host = serverParameters.Server;
			this.port = serverParameters.Port;
			this.username = authenticationParameters.NetworkCredential.UserName;
			this.logSession = connectionParameters.Log;
			this.connectionTimeout = connectionParameters.Timeout;
			this.state = Pop3ClientState.ProcessConnection;
			this.emailDropCount = -1;
			this.authType = Pop3Client.GetAuthType(authenticationParameters.Pop3AuthenticationMechanism, authenticationParameters.Pop3SecurityMechanism);
			if (authenticationParameters.NetworkCredential.SecurePassword == null)
			{
				this.password = new SecureString();
			}
			else
			{
				this.password = authenticationParameters.NetworkCredential.SecurePassword;
			}
			if (downloadCompleted != null)
			{
				this.RetrieveMessageCompleted += downloadCompleted;
			}
			if (roundtripComplete != null)
			{
				this.RoundtripComplete += roundtripComplete;
			}
		}

		internal event EventHandler<DownloadCompleteEventArgs> RetrieveMessageCompleted;

		private event EventHandler<RoundtripCompleteEventArgs> RoundtripComplete;

		internal AsyncResult<Pop3Client, Pop3ResultData> CurrentAsyncResult
		{
			get
			{
				base.CheckDisposed();
				return this.currentAsyncResult;
			}
			set
			{
				base.CheckDisposed();
				this.currentAsyncResult = value;
			}
		}

		internal MessageIdTracker MessageIdTracker
		{
			get
			{
				base.CheckDisposed();
				return this.messageIdTracker;
			}
			set
			{
				base.CheckDisposed();
				this.messageIdTracker = value;
			}
		}

		internal ExDateTime TimeSent
		{
			get
			{
				base.CheckDisposed();
				return this.timeSent;
			}
		}

		internal int EmailDropCount
		{
			get
			{
				base.CheckDisposed();
				return this.emailDropCount;
			}
		}

		private bool IsFirstLineBeingProcessed
		{
			get
			{
				return this.response == null;
			}
		}

		public override string ToString()
		{
			if (this.nc != null)
			{
				return string.Format(CultureInfo.InvariantCulture, "Connection {0} from {1} to {2}", new object[]
				{
					this.nc.ConnectionId,
					this.nc.LocalEndPoint,
					this.nc.RemoteEndPoint
				});
			}
			return string.Format(CultureInfo.InvariantCulture, "Disconnected connection", new object[0]);
		}

		internal static IAsyncResult BeginGetUniqueIds(Pop3Client thisPtr, AsyncCallback callback, object callbackState, object syncPoisonContext)
		{
			if (thisPtr == null)
			{
				throw new ArgumentNullException("thisPtr");
			}
			thisPtr.CheckDisposed();
			thisPtr.logSession.Debug("Pop3Client.BeginGetUniqueIds", new object[0]);
			thisPtr.ThrowIfAsyncIOPending();
			thisPtr.CloseConnectionIfExists();
			thisPtr.resultData = new AsyncOperationResult<Pop3ResultData>(new Pop3ResultData());
			thisPtr.currentAsyncResult = new AsyncResult<Pop3Client, Pop3ResultData>(thisPtr, thisPtr, callback, callbackState);
			thisPtr.seeServerCapabilities = true;
			Pop3AuthType pop3AuthType = thisPtr.authType;
			switch (pop3AuthType)
			{
			case Pop3AuthType.Basic:
				goto IL_123;
			case Pop3AuthType.Ntlm:
				break;
			default:
				switch (pop3AuthType)
				{
				case Pop3AuthType.SSL:
					goto IL_123;
				case Pop3AuthType.NtlmOverSSL:
					break;
				default:
					switch (pop3AuthType)
					{
					case Pop3AuthType.TLS:
						thisPtr.ProcessStates(new Pop3ClientState[]
						{
							Pop3ClientState.ProcessConnection,
							Pop3ClientState.ProcessStlsCommand,
							Pop3ClientState.ProcessUserCommand,
							Pop3ClientState.ProcessPassCommand,
							Pop3ClientState.ProcessCapaCommand,
							Pop3ClientState.ProcessStatCommand,
							Pop3ClientState.ProcessUidlCommand,
							Pop3ClientState.ProcessListCommand,
							Pop3ClientState.ProcessTopCommand
						});
						goto IL_15F;
					case Pop3AuthType.NtlmOverTLS:
						thisPtr.ProcessStates(new Pop3ClientState[]
						{
							Pop3ClientState.ProcessConnection,
							Pop3ClientState.ProcessStlsCommand,
							Pop3ClientState.ProcessAuthNtlmCommand,
							Pop3ClientState.ProcessCapaCommand,
							Pop3ClientState.ProcessStatCommand,
							Pop3ClientState.ProcessUidlCommand,
							Pop3ClientState.ProcessListCommand,
							Pop3ClientState.ProcessTopCommand
						});
						goto IL_15F;
					default:
						throw new NotSupportedException();
					}
					break;
				}
				break;
			}
			thisPtr.ProcessStates(new Pop3ClientState[]
			{
				Pop3ClientState.ProcessConnection,
				Pop3ClientState.ProcessAuthNtlmCommand,
				Pop3ClientState.ProcessCapaCommand,
				Pop3ClientState.ProcessStatCommand,
				Pop3ClientState.ProcessUidlCommand,
				Pop3ClientState.ProcessListCommand,
				Pop3ClientState.ProcessTopCommand
			});
			goto IL_15F;
			IL_123:
			thisPtr.ProcessStates(new Pop3ClientState[]
			{
				Pop3ClientState.ProcessConnection,
				Pop3ClientState.ProcessUserCommand,
				Pop3ClientState.ProcessPassCommand,
				Pop3ClientState.ProcessCapaCommand,
				Pop3ClientState.ProcessStatCommand,
				Pop3ClientState.ProcessUidlCommand,
				Pop3ClientState.ProcessListCommand,
				Pop3ClientState.ProcessTopCommand
			});
			IL_15F:
			return thisPtr.currentAsyncResult;
		}

		internal static AsyncOperationResult<Pop3ResultData> EndGetUniqueIds(IAsyncResult asyncResult)
		{
			AsyncResult<Pop3Client, Pop3ResultData> asyncResult2 = (AsyncResult<Pop3Client, Pop3ResultData>)asyncResult;
			asyncResult2.State.CheckDisposed();
			return asyncResult2.WaitForCompletion();
		}

		internal static AsyncResult<Pop3Client, Pop3ResultData> BeginVerifyAccount(Pop3Client thisPtr, AsyncCallback callback, object callbackState, object syncPoisonContext)
		{
			if (thisPtr == null)
			{
				throw new ArgumentNullException("thisPtr");
			}
			thisPtr.CheckDisposed();
			thisPtr.ThrowIfAsyncIOPending();
			thisPtr.CloseConnectionIfExists();
			thisPtr.resultData = new AsyncOperationResult<Pop3ResultData>(new Pop3ResultData());
			thisPtr.tempAsyncResult = new AsyncResult<Pop3Client, Pop3ResultData>(thisPtr, thisPtr, callback, callbackState);
			thisPtr.currentAsyncResult = new AsyncResult<Pop3Client, Pop3ResultData>(thisPtr, thisPtr, new AsyncCallback(Pop3Client.CapaListComplete), thisPtr);
			thisPtr.ensureQuit = true;
			Pop3AuthType pop3AuthType = thisPtr.authType;
			switch (pop3AuthType)
			{
			case Pop3AuthType.Basic:
				goto IL_ED;
			case Pop3AuthType.Ntlm:
				break;
			default:
				switch (pop3AuthType)
				{
				case Pop3AuthType.SSL:
					goto IL_ED;
				case Pop3AuthType.NtlmOverSSL:
					break;
				default:
					switch (pop3AuthType)
					{
					case Pop3AuthType.TLS:
						thisPtr.ProcessStates(new Pop3ClientState[]
						{
							Pop3ClientState.ProcessConnection,
							Pop3ClientState.ProcessStlsCommand,
							Pop3ClientState.ProcessUserCommand,
							Pop3ClientState.ProcessPassCommand,
							Pop3ClientState.ProcessCapaCommand
						});
						goto IL_124;
					case Pop3AuthType.NtlmOverTLS:
						thisPtr.ProcessStates(new Pop3ClientState[]
						{
							Pop3ClientState.ProcessConnection,
							Pop3ClientState.ProcessStlsCommand,
							Pop3ClientState.ProcessAuthNtlmCommand,
							Pop3ClientState.ProcessCapaCommand
						});
						goto IL_124;
					default:
						throw new NotSupportedException(thisPtr.authType.ToString());
					}
					break;
				}
				break;
			}
			thisPtr.ProcessStates(new Pop3ClientState[]
			{
				Pop3ClientState.ProcessConnection,
				Pop3ClientState.ProcessAuthNtlmCommand,
				Pop3ClientState.ProcessCapaCommand
			});
			goto IL_124;
			IL_ED:
			thisPtr.ProcessStates(new Pop3ClientState[]
			{
				Pop3ClientState.ProcessConnection,
				Pop3ClientState.ProcessUserCommand,
				Pop3ClientState.ProcessPassCommand,
				Pop3ClientState.ProcessCapaCommand
			});
			IL_124:
			return thisPtr.tempAsyncResult;
		}

		internal static AsyncOperationResult<Pop3ResultData> EndVerifyAccount(AsyncResult<Pop3Client, Pop3ResultData> asyncResult)
		{
			asyncResult.State.CheckDisposed();
			return asyncResult.WaitForCompletion();
		}

		internal static AsyncResult<Pop3Client, Pop3ResultData> BeginGetEmail(Pop3Client thisPtr, int messageId, AsyncCallback callback, object callbackState, object syncPoisonContext)
		{
			if (thisPtr == null)
			{
				throw new ArgumentNullException("thisPtr");
			}
			thisPtr.logSession.Debug("Pop3Client.BeginGetEmail {0}", new object[]
			{
				messageId
			});
			thisPtr.CheckDisposed();
			thisPtr.currentAsyncResult = new AsyncResult<Pop3Client, Pop3ResultData>(thisPtr, thisPtr, callback, callbackState);
			if (!thisPtr.ProcessCompletedOnConnectionError(true))
			{
				thisPtr.messageIdTracker = new MessageIdTracker(messageId, messageId, false);
				thisPtr.ProcessStates(new Pop3ClientState[]
				{
					Pop3ClientState.ProcessRetrCommand
				});
			}
			return thisPtr.currentAsyncResult;
		}

		internal static AsyncOperationResult<Pop3ResultData> EndGetEmail(IAsyncResult asyncResult)
		{
			AsyncResult<Pop3Client, Pop3ResultData> asyncResult2 = (AsyncResult<Pop3Client, Pop3ResultData>)asyncResult;
			asyncResult2.State.CheckDisposed();
			return asyncResult2.WaitForCompletion();
		}

		internal static AsyncResult<Pop3Client, Pop3ResultData> BeginDeleteEmails(Pop3Client thisPtr, List<int> messageIds, AsyncCallback callback, object callbackState, object syncPoisonContext)
		{
			if (thisPtr == null)
			{
				throw new ArgumentNullException("thisPtr");
			}
			thisPtr.logSession.Debug("Pop3Client.BeginDeleteEmails", new object[0]);
			if (messageIds == null)
			{
				throw new ArgumentNullException("messageId");
			}
			thisPtr.CheckDisposed();
			thisPtr.currentAsyncResult = new AsyncResult<Pop3Client, Pop3ResultData>(thisPtr, thisPtr, callback, callbackState);
			if (!thisPtr.ProcessCompletedOnConnectionError())
			{
				thisPtr.messageIdTracker = new MessageIdTracker(messageIds);
				thisPtr.ProcessStates(new Pop3ClientState[]
				{
					Pop3ClientState.ProcessDeleCommand
				});
			}
			return thisPtr.currentAsyncResult;
		}

		internal static AsyncOperationResult<Pop3ResultData> EndDeleteEmails(IAsyncResult asyncResult)
		{
			AsyncResult<Pop3Client, Pop3ResultData> asyncResult2 = (AsyncResult<Pop3Client, Pop3ResultData>)asyncResult;
			asyncResult2.State.CheckDisposed();
			return asyncResult2.WaitForCompletion();
		}

		internal static AsyncResult<Pop3Client, Pop3ResultData> BeginQuit(Pop3Client thisPtr, AsyncCallback callback, object callbackState, object syncPoisonContext)
		{
			if (thisPtr == null)
			{
				throw new ArgumentNullException("thisPtr");
			}
			thisPtr.logSession.Debug("Pop3Client.BeginQuit", new object[0]);
			thisPtr.CheckDisposed();
			thisPtr.currentAsyncResult = new AsyncResult<Pop3Client, Pop3ResultData>(thisPtr, thisPtr, callback, callbackState);
			if (!thisPtr.ProcessCompletedOnConnectionError())
			{
				thisPtr.ProcessStates(new Pop3ClientState[]
				{
					Pop3ClientState.ProcessQuitCommand
				});
			}
			return thisPtr.currentAsyncResult;
		}

		internal static AsyncOperationResult<Pop3ResultData> EndQuit(IAsyncResult asyncResult)
		{
			AsyncResult<Pop3Client, Pop3ResultData> asyncResult2 = (AsyncResult<Pop3Client, Pop3ResultData>)asyncResult;
			asyncResult2.State.CheckDisposed();
			return asyncResult2.WaitForCompletion();
		}

		internal void Cancel(IAsyncResult asyncResult)
		{
			base.CheckDisposed();
			this.logSession.Debug("Pop3Client.Cancel", new object[0]);
			AsyncResult<Pop3Client, Pop3ResultData> asyncResult2 = (AsyncResult<Pop3Client, Pop3ResultData>)asyncResult;
			if (asyncResult2.PendingAsyncResult != null)
			{
				asyncResult2.PendingAsyncResult.AsyncWaitHandle.WaitOne();
			}
		}

		internal void NotifyRoundtripComplete(object sender, RoundtripCompleteEventArgs roundtripCompleteEventArgs)
		{
			base.CheckDisposed();
			if (this.RoundtripComplete != null)
			{
				this.RoundtripComplete(sender, roundtripCompleteEventArgs);
			}
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				this.CloseConnectionIfExists();
				if (this.enumerator != null)
				{
					this.enumerator.Dispose();
					this.enumerator = null;
				}
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<Pop3Client>(this);
		}

		private static void CapaListComplete(IAsyncResult asyncResult)
		{
			Pop3Client pop3Client = asyncResult.AsyncState as Pop3Client;
			if (pop3Client.currentAsyncResult.Result.Exception != null && pop3Client.currentAsyncResult.Result.Exception.InnerException != null && pop3Client.currentAsyncResult.Result.Exception.InnerException is Pop3CapabilitiesNotSupportedException)
			{
				pop3Client.currentAsyncResult = pop3Client.tempAsyncResult;
				pop3Client.checkUniqueIdSupport = true;
				pop3Client.ProcessStates(new Pop3ClientState[]
				{
					Pop3ClientState.ProcessUidlCommand
				});
				return;
			}
			pop3Client.tempAsyncResult.ProcessCompleted(pop3Client.currentAsyncResult.Result.Data, pop3Client.currentAsyncResult.Result.Exception);
		}

		private static Pop3AuthType GetAuthType(Pop3AuthenticationMechanism authMechanism, Pop3SecurityMechanism securityMechanism)
		{
			Pop3AuthType result;
			if (authMechanism != Pop3AuthenticationMechanism.Basic)
			{
				if (authMechanism != Pop3AuthenticationMechanism.Spa)
				{
					throw new InvalidOperationException("PopSubscription has an invalid PopAuthentication and PopSecurity set.");
				}
				switch (securityMechanism)
				{
				case Pop3SecurityMechanism.None:
					result = Pop3AuthType.Ntlm;
					break;
				case Pop3SecurityMechanism.Ssl:
					result = Pop3AuthType.NtlmOverSSL;
					break;
				case Pop3SecurityMechanism.Tls:
					result = Pop3AuthType.NtlmOverTLS;
					break;
				default:
					throw new InvalidOperationException("PopSubscription has an invalid PopAuthentication and PopSecurity set.");
				}
			}
			else
			{
				switch (securityMechanism)
				{
				case Pop3SecurityMechanism.None:
					result = Pop3AuthType.Basic;
					break;
				case Pop3SecurityMechanism.Ssl:
					result = Pop3AuthType.SSL;
					break;
				case Pop3SecurityMechanism.Tls:
					result = Pop3AuthType.TLS;
					break;
				default:
					throw new InvalidOperationException("PopSubscription has an invalid PopAuthentication and PopSecurity set.");
				}
			}
			return result;
		}

		private static void ProcessState(Pop3Client thisPtr)
		{
			Pop3Client.<>c__DisplayClass6 CS$<>8__locals1 = new Pop3Client.<>c__DisplayClass6();
			CS$<>8__locals1.thisPtr = thisPtr;
			CS$<>8__locals1.thisPtr.logSession.Debug("Pop3Client.ProcessState {0}", new object[]
			{
				CS$<>8__locals1.thisPtr.state
			});
			bool flag = true;
			if ((CS$<>8__locals1.thisPtr.state == Pop3ClientState.ProcessDeleCommand || CS$<>8__locals1.thisPtr.state == Pop3ClientState.ProcessTopCommand || CS$<>8__locals1.thisPtr.state == Pop3ClientState.ProcessRetrCommand) && CS$<>8__locals1.thisPtr.messageIdTracker.MoveNext())
			{
				flag = false;
			}
			if (flag)
			{
				CS$<>8__locals1.thisPtr.MoveToNextState();
				if (CS$<>8__locals1.thisPtr.state != Pop3ClientState.ProcessUidlCommand && CS$<>8__locals1.thisPtr.messageIdTracker != null)
				{
					CS$<>8__locals1.thisPtr.messageIdTracker.Reset();
					if (!CS$<>8__locals1.thisPtr.messageIdTracker.MoveNext())
					{
						CS$<>8__locals1.thisPtr.MoveToNextState();
					}
				}
			}
			CS$<>8__locals1.thisPtr.logSession.Debug("Processing state {0}", new object[]
			{
				CS$<>8__locals1.thisPtr.state
			});
			switch (CS$<>8__locals1.thisPtr.state)
			{
			case Pop3ClientState.ProcessConnection:
				CS$<>8__locals1.thisPtr.connectionError = null;
				ILUtil.DoTryFilterCatch(new TryDelegate(CS$<>8__locals1, (UIntPtr)ldftn(<ProcessState>b__0)), new FilterDelegate(CS$<>8__locals1, (UIntPtr)ldftn(<ProcessState>b__1)), new CatchDelegate(null, (UIntPtr)ldftn(<ProcessState>b__2)));
				CS$<>8__locals1.thisPtr.command = null;
				CS$<>8__locals1.thisPtr.response = null;
				try
				{
					AsyncCallback requestCallback = Pop3Client.connectComplete;
					CS$<>8__locals1.thisPtr.currentAsyncResult.PendingAsyncResult = CS$<>8__locals1.thisPtr.socket.BeginConnect(CS$<>8__locals1.thisPtr.host, CS$<>8__locals1.thisPtr.port, requestCallback, CS$<>8__locals1.thisPtr);
					return;
				}
				catch (ArgumentException exception)
				{
					CS$<>8__locals1.thisPtr.socket = null;
					CS$<>8__locals1.thisPtr.HandleResult(exception);
					return;
				}
				catch (SocketException exception2)
				{
					CS$<>8__locals1.thisPtr.socket = null;
					CS$<>8__locals1.thisPtr.HandleResult(exception2);
					return;
				}
				break;
			case Pop3ClientState.ProcessCapaCommand:
				Pop3Client.SendRequestThenReadLine(CS$<>8__locals1.thisPtr, Pop3Command.Capa);
				return;
			case Pop3ClientState.ProcessTopCommand:
				Pop3Client.SendRequestThenReadLine(CS$<>8__locals1.thisPtr, Pop3Command.Top(CS$<>8__locals1.thisPtr.messageIdTracker.Current, 0));
				return;
			case Pop3ClientState.ProcessStlsCommand:
				break;
			case Pop3ClientState.ProcessAuthNtlmCommand:
				CS$<>8__locals1.thisPtr.authNtlmResponseCount = 0;
				Pop3Client.SendRequestThenReadLine(CS$<>8__locals1.thisPtr, Pop3Command.AuthNtlm);
				return;
			case Pop3ClientState.ProcessUserCommand:
			case Pop3ClientState.ProcessPassCommand:
			{
				Pop3Command pop3Command = null;
				try
				{
					if (CS$<>8__locals1.thisPtr.state == Pop3ClientState.ProcessUserCommand)
					{
						pop3Command = Pop3Command.User(CS$<>8__locals1.thisPtr.username);
					}
					else
					{
						pop3Command = Pop3Command.Pass(CS$<>8__locals1.thisPtr.password);
					}
				}
				catch (Pop3InvalidCommandException exception3)
				{
					CS$<>8__locals1.thisPtr.logSession.Error("The 'USER' or 'PASS' POP3 command is invalid (username = {0}).", new object[]
					{
						CS$<>8__locals1.thisPtr.username
					});
					CS$<>8__locals1.thisPtr.HandleResult(exception3);
					return;
				}
				Pop3Client.SendRequestThenReadLine(CS$<>8__locals1.thisPtr, pop3Command);
				return;
			}
			case Pop3ClientState.ProcessStatCommand:
				Pop3Client.SendRequestThenReadLine(CS$<>8__locals1.thisPtr, Pop3Command.Stat);
				return;
			case Pop3ClientState.ProcessUidlCommand:
				Pop3Client.SendRequestThenReadLine(CS$<>8__locals1.thisPtr, Pop3Command.Uidl());
				return;
			case Pop3ClientState.ProcessListCommand:
				Pop3Client.SendRequestThenReadLine(CS$<>8__locals1.thisPtr, Pop3Command.List());
				return;
			case Pop3ClientState.ProcessRetrCommand:
				Pop3Client.SendRequestThenReadLine(CS$<>8__locals1.thisPtr, Pop3Command.Retr(CS$<>8__locals1.thisPtr.messageIdTracker.Current));
				return;
			case Pop3ClientState.ProcessDeleCommand:
				Pop3Client.SendRequestThenReadLine(CS$<>8__locals1.thisPtr, Pop3Command.Dele(CS$<>8__locals1.thisPtr.messageIdTracker.Current));
				return;
			case Pop3ClientState.ProcessQuitCommand:
				Pop3Client.SendRequestThenReadLine(CS$<>8__locals1.thisPtr, Pop3Command.Quit);
				return;
			case Pop3ClientState.ProcessEnd:
				if (CS$<>8__locals1.thisPtr.ensureQuit)
				{
					CS$<>8__locals1.thisPtr.ensureQuit = false;
					Pop3Client.SendRequestThenReadLine(CS$<>8__locals1.thisPtr, Pop3Command.Quit);
					return;
				}
				CS$<>8__locals1.thisPtr.currentAsyncResult.ProcessCompleted(CS$<>8__locals1.thisPtr.resultData.Data, CS$<>8__locals1.thisPtr.resultData.Exception);
				return;
			default:
				return;
			}
			Pop3Client.SendRequestThenReadLine(CS$<>8__locals1.thisPtr, Pop3Command.Stls);
		}

		private static void ConnectComplete(IAsyncResult asyncResult)
		{
			Pop3Client pop3Client = (Pop3Client)asyncResult.AsyncState;
			pop3Client.logSession.Debug("Pop3Client.ConnectComplete", new object[0]);
			try
			{
				pop3Client.currentAsyncResult.PendingAsyncResult = null;
				pop3Client.socket.EndConnect(asyncResult);
				if (pop3Client.currentAsyncResult.IsCanceled)
				{
					pop3Client.socket = null;
					pop3Client.HandleCancelation();
					return;
				}
				pop3Client.nc = new NetworkConnection(pop3Client.socket, 4096);
				pop3Client.socket = null;
				pop3Client.nc.Timeout = (pop3Client.connectionTimeout + 999) / 1000;
				pop3Client.logSession.Debug("Connection Completed. Connection ID : {0}, Remote End Point {1}", new object[]
				{
					pop3Client.nc.ConnectionId,
					pop3Client.nc.RemoteEndPoint
				});
			}
			catch (SocketException exception)
			{
				pop3Client.socket = null;
				pop3Client.HandleResult(exception);
				return;
			}
			switch (pop3Client.authType)
			{
			case Pop3AuthType.SSL:
			case Pop3AuthType.NtlmOverSSL:
				pop3Client.currentAsyncResult.PendingAsyncResult = pop3Client.nc.BeginNegotiateTlsAsClient(null, pop3Client.nc.RemoteEndPoint.Address.ToString(), Pop3Client.negotiateTlsCompleteReadLine, pop3Client);
				return;
			default:
				Pop3Client.StartReadLine(pop3Client);
				break;
			}
		}

		private static void NegotiateTlsCompleteReadLine(IAsyncResult asyncResult)
		{
			Pop3Client pop3Client = (Pop3Client)asyncResult.AsyncState;
			pop3Client.logSession.Debug("Pop3Client.NegotiateTlsCompleteReadline", new object[0]);
			pop3Client.currentAsyncResult.PendingAsyncResult = null;
			object obj;
			pop3Client.nc.EndNegotiateTlsAsClient(asyncResult, out obj);
			if (obj != null)
			{
				pop3Client.HandleError(obj);
				return;
			}
			if (pop3Client.currentAsyncResult.IsCanceled)
			{
				pop3Client.HandleCancelation();
				return;
			}
			if (pop3Client.nc.RemoteCertificate == null)
			{
				pop3Client.logSession.Error("No remote certificate present", new object[0]);
				return;
			}
			pop3Client.logSession.Debug("TLS negotiation completed for connection.", new object[0]);
			Pop3AuthType pop3AuthType = pop3Client.authType;
			switch (pop3AuthType)
			{
			case Pop3AuthType.SSL:
			case Pop3AuthType.NtlmOverSSL:
				Pop3Client.StartReadLine(pop3Client);
				return;
			default:
				switch (pop3AuthType)
				{
				case Pop3AuthType.TLS:
				case Pop3AuthType.NtlmOverTLS:
					Pop3Client.ProcessState(pop3Client);
					return;
				default:
					throw new NotSupportedException();
				}
				break;
			}
		}

		private static void StartReadLine(Pop3Client thisPtr)
		{
			if (thisPtr.recursionLevel < 20)
			{
				try
				{
					Interlocked.Increment(ref thisPtr.recursionLevel);
					thisPtr.currentAsyncResult.PendingAsyncResult = thisPtr.nc.BeginReadLine(Pop3Client.readLineComplete, thisPtr);
					return;
				}
				finally
				{
					Interlocked.Decrement(ref thisPtr.recursionLevel);
				}
			}
			ThreadPool.QueueUserWorkItem(delegate(object callstate)
			{
				Pop3Client pop3Client = (Pop3Client)callstate;
				pop3Client.currentAsyncResult.PendingAsyncResult = pop3Client.nc.BeginReadLine(Pop3Client.readLineComplete, pop3Client);
			}, thisPtr);
		}

		private static void ReadLineComplete(IAsyncResult asyncResult)
		{
			Pop3Client pop3Client = (Pop3Client)asyncResult.AsyncState;
			pop3Client.currentAsyncResult.PendingAsyncResult = null;
			byte[] buffer;
			int offset;
			int size;
			object obj;
			pop3Client.nc.EndReadLine(asyncResult, out buffer, out offset, out size, out obj);
			bool moreResponseDataAvailable = false;
			if (obj != null)
			{
				if (!(obj is SocketError) || (SocketError)obj != SocketError.MessageSize)
				{
					pop3Client.HandleError(obj);
					return;
				}
				moreResponseDataAvailable = true;
			}
			if (pop3Client.currentAsyncResult.IsCanceled)
			{
				pop3Client.logSession.Error("Response received from the network connection, but we are already disconnected", new object[0]);
				pop3Client.HandleCancelation();
				return;
			}
			Pop3Client.StartProcessingResponse(pop3Client, buffer, offset, size, moreResponseDataAvailable);
		}

		private static void StartProcessingResponse(Pop3Client thisPtr, byte[] buffer, int offset, int size, bool moreResponseDataAvailable)
		{
			thisPtr.logSession.Debug("Pop3Client.StartProcessingResponse", new object[0]);
			BufferBuilder bufferBuilder = thisPtr.responseBuffer ?? new BufferBuilder(size);
			thisPtr.responseBuffer = null;
			bool isFirstLineBeingProcessed = thisPtr.IsFirstLineBeingProcessed;
			int num = isFirstLineBeingProcessed ? 510 : 32768;
			if (bufferBuilder.Length + size > num)
			{
				string text = string.Format(CultureInfo.InvariantCulture, "The response line has got {0} bytes, which exceeds the maximum length: {1}.", new object[]
				{
					bufferBuilder.Length + size,
					num
				});
				thisPtr.logSession.Error(text, new object[0]);
				thisPtr.HandleError(new Pop3NonCompliantServerException(new FormatException(text)));
				return;
			}
			bufferBuilder.Append(buffer, offset, size);
			if (moreResponseDataAvailable)
			{
				thisPtr.responseBuffer = bufferBuilder;
				Pop3Client.StartReadLine(thisPtr);
				return;
			}
			byte[] buffer2 = bufferBuilder.GetBuffer();
			bool flag = BufferParser.CompareArg(Pop3Client.DotBytes, buffer2, 0, Pop3Client.DotBytes.Length);
			if (isFirstLineBeingProcessed)
			{
				Pop3Response pop3Response = new Pop3Response(bufferBuilder.ToString());
				if (thisPtr.state == Pop3ClientState.ProcessAuthNtlmCommand)
				{
					thisPtr.authNtlmResponseCount++;
					if (thisPtr.authNtlmResponseCount == 2 && pop3Response.Type != Pop3ResponseType.sendMore)
					{
						string text2 = string.Format(CultureInfo.InvariantCulture, "The response line was type {0} but expected was {1}.", new object[]
						{
							pop3Response.Type,
							Pop3ResponseType.sendMore
						});
						thisPtr.logSession.Error(text2, new object[0]);
						thisPtr.HandleError(new Pop3NonCompliantServerException(new InvalidDataException(text2)));
						return;
					}
				}
				thisPtr.response = pop3Response;
				if (thisPtr.response.Type == Pop3ResponseType.unknown && thisPtr.response.Headline.StartsWith(" "))
				{
					string.Format(CultureInfo.InvariantCulture, "Pop server {0} returned a response that started with leading space for user name: {1} while processing state: {2} Response was: [{3}]", new object[]
					{
						thisPtr.host,
						thisPtr.username,
						thisPtr.state.ToString(),
						thisPtr.response.Headline
					});
				}
			}
			else if (!flag && (thisPtr.command.Type == Pop3CommandType.Capa || thisPtr.command.Type == Pop3CommandType.List || thisPtr.command.Type == Pop3CommandType.Uidl))
			{
				int num2;
				if (thisPtr.resultData.Data.EmailDropCount > 0)
				{
					num2 = Math.Min(thisPtr.resultData.Data.EmailDropCount, 50000);
				}
				else
				{
					num2 = 50000;
				}
				if (thisPtr.response.ListingCount == num2)
				{
					thisPtr.logSession.Error("'{0}' has already got {1} response lines that is the maximum line count or the email drop count.", new object[]
					{
						thisPtr.command,
						thisPtr.response.ListingCount
					});
					Pop3Client.CountCommand(thisPtr.currentAsyncResult, false);
					thisPtr.HandleResult(new Pop3BrokenResponseException(thisPtr.command.ToString(), thisPtr.response.ToString()));
					return;
				}
				thisPtr.response.AppendListing(bufferBuilder.ToString());
			}
			bool flag2 = thisPtr.command != null && thisPtr.command.Listings && thisPtr.response.Type == Pop3ResponseType.ok;
			bool flag3 = !flag2 || (buffer2.Length == 1 && flag);
			if (isFirstLineBeingProcessed || (thisPtr.command != null && (thisPtr.command.Type == Pop3CommandType.Uidl || thisPtr.command.Type == Pop3CommandType.List || thisPtr.command.Type == Pop3CommandType.Capa)))
			{
				bufferBuilder.RemoveUnusedBufferSpace();
			}
			if (flag3)
			{
				Pop3Client.ProcessResponse(thisPtr);
				return;
			}
			if (thisPtr.response.ListingCount == 0 && (thisPtr.command.Type == Pop3CommandType.Capa || thisPtr.command.Type == Pop3CommandType.List || thisPtr.command.Type == Pop3CommandType.Uidl))
			{
				thisPtr.response.ListingCapacity = thisPtr.resultData.Data.EmailDropCount;
			}
			if (thisPtr.command.Type == Pop3CommandType.Retr || thisPtr.command.Type == Pop3CommandType.Top)
			{
				if (thisPtr.dotStuffedParser == null)
				{
					thisPtr.dotStuffedParser = new SmtpInDataParser();
				}
				else
				{
					thisPtr.dotStuffedParser.Reset();
				}
				thisPtr.dotStuffedParser.BodyStream = thisPtr.response.MessageStream;
				thisPtr.currentAsyncResult.PendingAsyncResult = thisPtr.nc.BeginRead(Pop3Client.readCompleteConsumeData, thisPtr);
				return;
			}
			Pop3Client.StartReadLine(thisPtr);
		}

		private static void ReadCompleteConsumeData(IAsyncResult asyncResult)
		{
			Pop3Client pop3Client = (Pop3Client)asyncResult.AsyncState;
			pop3Client.currentAsyncResult.PendingAsyncResult = null;
			byte[] data;
			int offset;
			int num;
			object obj;
			pop3Client.nc.EndRead(asyncResult, out data, out offset, out num, out obj);
			if (pop3Client.currentAsyncResult.IsCanceled)
			{
				pop3Client.HandleCancelation();
				return;
			}
			if (obj != null)
			{
				pop3Client.HandleError(obj);
				return;
			}
			int num2;
			bool flag = pop3Client.dotStuffedParser.ParseAndWrite(data, offset, num, out num2);
			int num3 = num - num2;
			if (flag)
			{
				pop3Client.logSession.Assert(num3 >= 0, "BUG: The unconsumed bytes must be non-negative.", new object[0]);
				pop3Client.nc.PutBackReceivedBytes(num3);
				pop3Client.response.MessageStream.Seek(0L, SeekOrigin.Begin);
				Pop3Client.ProcessResponse(pop3Client);
				return;
			}
			pop3Client.logSession.Assert(num3 < 5, "BUG: The unconsumed bytes must be less than '5'.", new object[0]);
			pop3Client.nc.PutBackReceivedBytes(num3);
			pop3Client.currentAsyncResult.PendingAsyncResult = pop3Client.nc.BeginRead(Pop3Client.readCompleteConsumeData, pop3Client);
		}

		private static void SendAuthBlobThenReadLine(Pop3Client thisPtr)
		{
			if (thisPtr.authContext == null)
			{
				AuthenticationMechanism mechanism = AuthenticationMechanism.Ntlm;
				string spn = "POP/" + thisPtr.host;
				thisPtr.authContext = new AuthenticationContext();
				SecurityStatus securityStatus = thisPtr.authContext.InitializeForOutboundNegotiate(mechanism, spn, thisPtr.username, thisPtr.password);
				if (securityStatus != SecurityStatus.OK)
				{
					thisPtr.HandleError(securityStatus);
					return;
				}
			}
			thisPtr.logSession.Assert(thisPtr.response.Headline.Length >= 2, "Server challenge response is too short.  Actual Length: {0}", new object[]
			{
				thisPtr.response.Headline.Length
			});
			string s = thisPtr.response.Headline.Substring(2);
			if (thisPtr.authNtlmResponseCount == 1)
			{
				thisPtr.logSession.Assert(thisPtr.state == Pop3ClientState.ProcessAuthNtlmCommand, "state was different from ProcessAuthNtlmCommand", new object[0]);
				s = string.Empty;
			}
			byte[] bytes = Encoding.ASCII.GetBytes(s);
			byte[] blob = null;
			SecurityStatus securityStatus2 = thisPtr.authContext.NegotiateSecurityContext(bytes, out blob);
			SecurityStatus securityStatus3 = securityStatus2;
			if (securityStatus3 != SecurityStatus.OK && securityStatus3 != SecurityStatus.ContinueNeeded)
			{
				thisPtr.HandleError(securityStatus2);
				return;
			}
			Pop3Client.SendRequestThenReadLine(thisPtr, Pop3Command.Blob(blob));
		}

		private static void ProcessResponse(Pop3Client thisPtr)
		{
			thisPtr.logSession.Debug("Pop3Client.ProcessResponse {0}", new object[]
			{
				thisPtr.response.Type
			});
			if (thisPtr.command != null)
			{
				Pop3Client.CountCommand(thisPtr.currentAsyncResult, true);
			}
			switch (thisPtr.response.Type)
			{
			case Pop3ResponseType.ok:
				if (thisPtr.state == Pop3ClientState.ProcessAuthNtlmCommand && thisPtr.authNtlmResponseCount == 1)
				{
					Pop3Client.SendAuthBlobThenReadLine(thisPtr);
					return;
				}
				switch (thisPtr.state)
				{
				case Pop3ClientState.ProcessCapaCommand:
				{
					bool uidlCommandSupported = false;
					int? retentionDays = null;
					Exception ex = thisPtr.response.TryParseCapaResponse(out uidlCommandSupported, out retentionDays);
					if (ex != null)
					{
						thisPtr.HandleResult(new Pop3BrokenResponseException(thisPtr.command.ToString(), thisPtr.response.ToString(), ex));
						return;
					}
					thisPtr.resultData.Data.RetentionDays = retentionDays;
					thisPtr.resultData.Data.UidlCommandSupported = uidlCommandSupported;
					break;
				}
				case Pop3ClientState.ProcessTopCommand:
				{
					ExDateTime exDateTime = thisPtr.response.ParseReceivedDate(false);
					if (thisPtr.messageIdTracker.Current == 1)
					{
						thisPtr.resultData.Data.FirstReceivedDate = exDateTime;
					}
					else
					{
						if (thisPtr.messageIdTracker.Current != thisPtr.resultData.Data.EmailDropCount)
						{
							throw new InvalidOperationException("The message id must be 1 or the number of messages in the maildrop.");
						}
						thisPtr.resultData.Data.LastReceivedDate = exDateTime;
					}
					break;
				}
				case Pop3ClientState.ProcessStlsCommand:
					thisPtr.currentAsyncResult.PendingAsyncResult = thisPtr.nc.BeginNegotiateTlsAsClient(null, thisPtr.nc.RemoteEndPoint.Address.ToString(), Pop3Client.negotiateTlsCompleteReadLine, thisPtr);
					return;
				case Pop3ClientState.ProcessStatCommand:
				{
					int num;
					Exception ex2 = thisPtr.response.TryParseSTATResponse(out num);
					if (ex2 != null)
					{
						thisPtr.HandleResult(new Pop3BrokenResponseException(thisPtr.command.ToString(), thisPtr.response.ToString(), ex2));
						return;
					}
					thisPtr.emailDropCount = num;
					if (num > 50000)
					{
						thisPtr.logSession.Error("'STAT' indicates that there are {0} response lines coming, and that is above the maximum email drop count.  Stopping download.", new object[]
						{
							num
						});
						thisPtr.HandleResult(new Pop3BrokenResponseException(thisPtr.command.ToString(), thisPtr.response.ToString()));
					}
					else
					{
						thisPtr.resultData.Data.EmailDropCount = num;
						if (num == 0)
						{
							thisPtr.MoveToEndState();
						}
					}
					break;
				}
				case Pop3ClientState.ProcessUidlCommand:
					if (thisPtr.checkUniqueIdSupport)
					{
						thisPtr.checkUniqueIdSupport = false;
					}
					else
					{
						Exception ex3 = thisPtr.response.TryParseUIDLResponse(thisPtr.resultData.Data);
						if (ex3 != null)
						{
							thisPtr.HandleResult(new Pop3BrokenResponseException(thisPtr.command.ToString(), thisPtr.response.ToString(), ex3));
							return;
						}
					}
					break;
				case Pop3ClientState.ProcessListCommand:
				{
					Exception ex4 = thisPtr.response.TryParseLISTResponse(thisPtr.resultData.Data);
					if (ex4 != null)
					{
						thisPtr.HandleResult(new Pop3BrokenResponseException(thisPtr.command.ToString(), thisPtr.response.ToString(), ex4));
						return;
					}
					bool flag = thisPtr.resultData.Data.EmailDropCount > 2;
					if (flag)
					{
						thisPtr.messageIdTracker = new MessageIdTracker(new int[]
						{
							1,
							thisPtr.resultData.Data.EmailDropCount
						});
					}
					else
					{
						thisPtr.MoveToEndState();
					}
					break;
				}
				case Pop3ClientState.ProcessRetrCommand:
				{
					ExDateTime exDateTime2 = thisPtr.response.ParseReceivedDate(true);
					if (exDateTime2 == ExDateTime.MinValue)
					{
						exDateTime2 = ExDateTime.UtcNow;
					}
					thisPtr.resultData.Data.Email = new Pop3Email(thisPtr.logSession, exDateTime2, thisPtr.response.MessageStream);
					thisPtr.submittedMessagesCount += 1UL;
					if (thisPtr.RetrieveMessageCompleted != null)
					{
						thisPtr.RetrieveMessageCompleted(thisPtr, new DownloadCompleteEventArgs(thisPtr.response.MessageStream.Length));
					}
					break;
				}
				case Pop3ClientState.ProcessDeleCommand:
					thisPtr.resultData.Data.AddDeletedMessageId(thisPtr.messageIdTracker.Current);
					break;
				}
				Pop3Client.ProcessState(thisPtr);
				return;
			case Pop3ResponseType.err:
				if (thisPtr.state != Pop3ClientState.ProcessTopCommand && thisPtr.state != Pop3ClientState.ProcessCapaCommand && thisPtr.state != Pop3ClientState.ProcessUidlCommand)
				{
					thisPtr.logSession.Error("Error response returned: {0}.", new object[]
					{
						thisPtr.response.ToString()
					});
				}
				if (!thisPtr.response.HasPermanentError)
				{
					switch (thisPtr.state)
					{
					case Pop3ClientState.ProcessCapaCommand:
						if (!thisPtr.seeServerCapabilities)
						{
							thisPtr.HandleResult(new Pop3CapabilitiesNotSupportedException());
							return;
						}
						Pop3Client.ProcessState(thisPtr);
						return;
					case Pop3ClientState.ProcessTopCommand:
						thisPtr.MoveToEndState();
						Pop3Client.ProcessState(thisPtr);
						return;
					case Pop3ClientState.ProcessUserCommand:
					case Pop3ClientState.ProcessPassCommand:
					{
						Exception exception;
						if (thisPtr.response.HasInUseAuthenticationError)
						{
							exception = new Pop3TransientInUseAuthErrorException();
						}
						else if (thisPtr.response.HasLogInDelayAuthenticationError)
						{
							exception = new Pop3TransientLoginDelayedAuthErrorException();
						}
						else if (thisPtr.response.HasSystemTemporaryError)
						{
							exception = new Pop3TransientSystemAuthErrorException();
						}
						else
						{
							exception = new Pop3AuthErrorException();
						}
						thisPtr.HandleResult(exception);
						return;
					}
					case Pop3ClientState.ProcessUidlCommand:
						thisPtr.resultData.Data.UidlCommandSupported = false;
						Pop3Client.ProcessState(thisPtr);
						return;
					}
					thisPtr.HandleResult(new Pop3ErrorResponseException((thisPtr.command == null) ? string.Empty : thisPtr.command.ToString(), (thisPtr.response == null) ? string.Empty : thisPtr.response.ToString()));
					return;
				}
				if (thisPtr.state == Pop3ClientState.ProcessUserCommand || thisPtr.state == Pop3ClientState.ProcessPassCommand)
				{
					thisPtr.HandleResult(new Pop3DisabledResponseException());
					return;
				}
				thisPtr.HandleResult(new Pop3PermErrorResponseException((thisPtr.command == null) ? string.Empty : thisPtr.command.ToString(), (thisPtr.response == null) ? string.Empty : thisPtr.response.ToString()));
				return;
			case Pop3ResponseType.unknown:
				break;
			case Pop3ResponseType.sendMore:
				if (thisPtr.state == Pop3ClientState.ProcessAuthNtlmCommand)
				{
					Pop3Client.SendAuthBlobThenReadLine(thisPtr);
					return;
				}
				break;
			default:
				throw new InvalidOperationException("The Pop3 response type is invalid.");
			}
			thisPtr.logSession.Error("Unknown response returned: {0}.", new object[]
			{
				thisPtr.response.ToString()
			});
			thisPtr.HandleResult(new Pop3UnknownResponseException((thisPtr.command == null) ? string.Empty : thisPtr.command.ToString(), (thisPtr.response == null) ? string.Empty : thisPtr.response.ToString()));
		}

		private static void ReadCompleteSendRequest(IAsyncResult asyncResult)
		{
			Pop3Client pop3Client = (Pop3Client)asyncResult.AsyncState;
			pop3Client.currentAsyncResult.PendingAsyncResult = null;
			byte[] value;
			int offset;
			int num;
			object obj;
			pop3Client.nc.EndRead(asyncResult, out value, out offset, out num, out obj);
			if (pop3Client.currentAsyncResult.IsCanceled)
			{
				pop3Client.HandleCancelation();
				return;
			}
			if (obj != null)
			{
				pop3Client.HandleError(obj);
				return;
			}
			BufferBuilder bufferBuilder = new BufferBuilder(num);
			bufferBuilder.Append(value, offset, num);
			bufferBuilder.RemoveUnusedBufferSpace();
			Pop3Client.SendRequestThenReadLine(pop3Client, pop3Client.command);
		}

		private static void SendRequestThenReadLine(Pop3Client thisPtr, Pop3Command pop3Command)
		{
			thisPtr.command = pop3Command;
			if (thisPtr.nc.IsDataAvailable)
			{
				thisPtr.currentAsyncResult.PendingAsyncResult = thisPtr.nc.BeginRead(Pop3Client.readCompleteSendRequest, thisPtr);
				return;
			}
			byte[] buffer = thisPtr.command.Buffer;
			thisPtr.response = null;
			thisPtr.timeSent = ExDateTime.UtcNow;
			thisPtr.currentAsyncResult.PendingAsyncResult = thisPtr.nc.BeginWrite(buffer, 0, buffer.Length, Pop3Client.sendCompleteReadLine, thisPtr);
		}

		private static void SendCompleteReadLine(IAsyncResult asyncResult)
		{
			Pop3Client pop3Client = (Pop3Client)asyncResult.AsyncState;
			pop3Client.command.ClearBuffer();
			pop3Client.currentAsyncResult.PendingAsyncResult = null;
			object obj;
			pop3Client.nc.EndWrite(asyncResult, out obj);
			if (obj != null)
			{
				pop3Client.HandleError(obj);
				return;
			}
			if (pop3Client.currentAsyncResult.IsCanceled)
			{
				pop3Client.HandleCancelation();
				return;
			}
			Pop3Client.StartReadLine(pop3Client);
		}

		private static void CountCommand(AsyncResult<Pop3Client, Pop3ResultData> curOp, bool successful)
		{
			curOp.State.NotifyRoundtripComplete(null, new RoundtripCompleteEventArgs(ExDateTime.UtcNow - curOp.State.TimeSent, successful, curOp.State.host));
		}

		private void HandleError(object errorCode)
		{
			Pop3Client.CountCommand(this.currentAsyncResult, false);
			Exception ex;
			if (errorCode is SocketError)
			{
				this.logSession.Debug("Pop3Client.HandleError (SocketError={0})", new object[]
				{
					errorCode
				});
				this.connectionError = new SocketException((int)errorCode);
				ex = this.connectionError;
			}
			else if (errorCode is SecurityStatus)
			{
				this.logSession.Debug("Pop3Client.HandleError (SecurityStatus={0})", new object[]
				{
					errorCode
				});
				ex = this.connectionError;
			}
			else if (errorCode is Pop3NonCompliantServerException)
			{
				Pop3NonCompliantServerException ex2 = (Pop3NonCompliantServerException)errorCode;
				this.connectionError = ex2;
				ex = ex2;
			}
			else if (errorCode is MessageSizeLimitExceededException)
			{
				Exception ex3 = (Exception)errorCode;
				this.connectionError = ex3;
				ex = ex3;
			}
			else
			{
				this.logSession.Debug("Pop3Client.HandleError (error={0})", new object[]
				{
					errorCode
				});
				this.connectionError = new Win32Exception((int)errorCode);
				ex = this.connectionError;
			}
			this.logSession.Assert(ex != null, "Pop3Client.LogError: resultException not set", new object[0]);
			this.CloseConnectionIfExists();
			this.HandleResult(ex);
		}

		private void HandleResult(Exception exception)
		{
			this.logSession.Error("Pop3Client.HandleResult (exception={0})", new object[]
			{
				exception
			});
			this.MoveToEndState();
			this.currentAsyncResult.ProcessCompleted(new Pop3ResultData(), exception);
		}

		private void HandleCancelation()
		{
			Pop3Client.CountCommand(this.currentAsyncResult, false);
			this.HandleResult(AsyncOperationResult<Pop3ResultData>.CanceledException);
		}

		private void CloseConnectionIfExists()
		{
			if (this.authContext != null)
			{
				this.authContext.Dispose();
				this.authContext = null;
			}
			if (this.nc != null)
			{
				this.nc.Dispose();
				this.nc = null;
			}
		}

		private bool ProcessCompletedOnConnectionError()
		{
			return this.ProcessCompletedOnConnectionError(false);
		}

		private bool ProcessCompletedOnConnectionError(bool isEmailRetrieve)
		{
			if (this.connectionError == null)
			{
				return false;
			}
			Exception exception = isEmailRetrieve ? new ConnectionClosedException(this.connectionError) : this.connectionError;
			this.currentAsyncResult.ProcessCompleted(new Pop3ResultData(), exception);
			return true;
		}

		private void ThrowIfAsyncIOPending()
		{
			if (this.currentAsyncResult != null && this.currentAsyncResult.PendingAsyncResult != null && !this.currentAsyncResult.PendingAsyncResult.IsCompleted)
			{
				throw new InvalidOperationException("The Pop3 client already has pending async I/O(s).");
			}
		}

		private void ProcessStates(params Pop3ClientState[] states)
		{
			if (states == null)
			{
				throw new ArgumentNullException("states");
			}
			if (states.Length == 0)
			{
				throw new ArgumentOutOfRangeException("states");
			}
			if (this.enumerator != null && this.enumerator.Current != Pop3ClientState.ProcessEnd)
			{
				throw new InvalidOperationException("The previous async operation request hasnt completed yet.");
			}
			List<Pop3ClientState> list = new List<Pop3ClientState>(states.Length);
			for (int i = 0; i < states.Length; i++)
			{
				list.Add(states[i]);
			}
			this.enumerator = list.GetEnumerator();
			Pop3Client.ProcessState(this);
		}

		private void MoveToNextState()
		{
			if (this.enumerator != null)
			{
				if (this.enumerator.MoveNext())
				{
					this.state = this.enumerator.Current;
					return;
				}
				this.enumerator.Dispose();
				this.enumerator = null;
			}
			this.state = Pop3ClientState.ProcessEnd;
		}

		private void MoveToEndState()
		{
			if (this.enumerator != null)
			{
				this.enumerator.Dispose();
				this.enumerator = null;
			}
			this.state = Pop3ClientState.ProcessEnd;
		}

		private const int NumberOfBreadcrumbs = 64;

		private const string PopComponentId = "POP";

		private const string PopSpnPrefix = "POP/";

		private const int MaxResponseLineLength = 32768;

		private const int MaxHeaderLineLength = 510;

		private const int MaxResponseLineCount = 50000;

		private const int MaxRecursionLevel = 20;

		private static readonly Trace Tracer = ExTraceGlobals.Pop3ClientTracer;

		private static readonly AsyncCallback connectComplete = new AsyncCallback(Pop3Client.ConnectComplete);

		private static readonly AsyncCallback negotiateTlsCompleteReadLine = new AsyncCallback(Pop3Client.NegotiateTlsCompleteReadLine);

		private static readonly AsyncCallback readLineComplete = new AsyncCallback(Pop3Client.ReadLineComplete);

		private static readonly AsyncCallback readCompleteSendRequest = new AsyncCallback(Pop3Client.ReadCompleteSendRequest);

		private static readonly AsyncCallback readCompleteConsumeData = new AsyncCallback(Pop3Client.ReadCompleteConsumeData);

		private static readonly AsyncCallback sendCompleteReadLine = new AsyncCallback(Pop3Client.SendCompleteReadLine);

		private static readonly byte[] PassBytes = Encoding.ASCII.GetBytes("PASS *****");

		private static readonly byte[] CrlfBytes = Encoding.ASCII.GetBytes("\r\n");

		private static readonly byte[] DotBytes = Encoding.ASCII.GetBytes(".");

		private readonly string host;

		private readonly int port;

		private readonly int connectionTimeout;

		private readonly string username;

		private readonly SecureString password;

		private AuthenticationContext authContext;

		private Pop3AuthType authType;

		private Pop3ClientState state;

		private int recursionLevel;

		private ILog logSession;

		private Socket socket;

		private NetworkConnection nc;

		private Pop3Command command;

		private ExDateTime timeSent;

		private Pop3Response response;

		private BufferBuilder responseBuffer;

		private AsyncOperationResult<Pop3ResultData> resultData;

		private AsyncResult<Pop3Client, Pop3ResultData> currentAsyncResult;

		private AsyncResult<Pop3Client, Pop3ResultData> tempAsyncResult;

		private MessageIdTracker messageIdTracker;

		private IEnumerator<Pop3ClientState> enumerator;

		private ulong submittedMessagesCount;

		private bool checkUniqueIdSupport;

		private bool seeServerCapabilities;

		private bool ensureQuit;

		private Exception connectionError;

		private int emailDropCount;

		private SmtpInDataParser dotStuffedParser;

		private int authNtlmResponseCount;
	}
}
