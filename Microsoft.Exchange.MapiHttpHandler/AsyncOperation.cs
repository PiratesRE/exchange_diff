using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.MapiHttpHandler;
using Microsoft.Exchange.Rpc;
using Microsoft.Exchange.RpcClientAccess;
using Microsoft.Exchange.RpcClientAccess.Parser;
using Microsoft.Exchange.Security.Authentication;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.MapiHttp
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class AsyncOperation : BaseObject
	{
		protected AsyncOperation(HttpContextBase context, string cookieVdirPath, AsyncOperationCookieFlags cookieFlags)
		{
			using (DisposeGuard disposeGuard = this.Guard())
			{
				this.context = context;
				this.cookieVdirPath = cookieVdirPath;
				this.cookieFlags = cookieFlags;
				string strA = context.GetContentType();
				if (string.Compare(strA, "application/mapi-http", true) == 0)
				{
					this.contentType = "application/mapi-http";
				}
				else
				{
					if (string.Compare(strA, "application/octet-stream", true) != 0)
					{
						throw ProtocolException.FromResponseCode((LID)46876, string.Format("Invalid Content-Type header; expecting {0}", "application/mapi-http"), ResponseCode.InvalidHeader, null);
					}
					this.contentType = "application/octet-stream";
				}
				AsyncOperation.ValidateAcceptTypes(context.Request, ref this.contentType);
				this.requestId = context.GetRequestId();
				disposeGuard.Success();
			}
		}

		public string ContentType
		{
			get
			{
				base.CheckDisposed();
				return this.contentType;
			}
		}

		public string RequestId
		{
			get
			{
				base.CheckDisposed();
				return this.requestId;
			}
		}

		public abstract string RequestType { get; }

		public virtual TimeSpan InitialFlushPeriod
		{
			get
			{
				return AsyncOperation.initialFlushPeriod;
			}
		}

		public AsyncOperationInfo AsyncOperationInfo
		{
			get
			{
				base.CheckDisposed();
				return this.asyncOperationInfo;
			}
		}

		public virtual object DroppedConnectionContextHandle
		{
			get
			{
				base.CheckDisposed();
				return null;
			}
		}

		public SessionContext SessionContext
		{
			get
			{
				base.CheckDisposed();
				if (this.sessionContextActivity == null)
				{
					return null;
				}
				return this.sessionContextActivity.SessionContext;
			}
		}

		protected HttpContextBase Context
		{
			get
			{
				return this.context;
			}
		}

		protected object ContextHandle
		{
			get
			{
				if (this.SessionContext == null)
				{
					return null;
				}
				return this.SessionContext.ContextHandle;
			}
			set
			{
				if (this.SessionContext == null)
				{
					if (value != null)
					{
						throw new InvalidOperationException("Unable to set context handle without a session context");
					}
				}
				else
				{
					this.SessionContext.ContextHandle = value;
				}
			}
		}

		protected string Cookie
		{
			get
			{
				if (this.SessionContext == null)
				{
					return null;
				}
				return this.SessionContext.Cookie;
			}
		}

		protected string SequenceCookie
		{
			get
			{
				return this.currentSequenceCookie;
			}
		}

		protected AsyncOperation.MapiHttpProtocolRequestInfo ProtocolRequestInfo
		{
			get
			{
				return this.protocolRequestInfo;
			}
		}

		public virtual string GetTraceBeginParameters(MapiHttpRequest mapiHttpRequest)
		{
			base.CheckDisposed();
			return string.Empty;
		}

		public virtual string GetTraceEndParameters(MapiHttpRequest mapiHttpRequest, MapiHttpResponse mapiHttpResponse)
		{
			base.CheckDisposed();
			return string.Empty;
		}

		public virtual void ParseRequest(WorkBuffer requestBuffer)
		{
			base.CheckDisposed();
			using (Reader reader = Reader.CreateBufferReader(requestBuffer.ArraySegment))
			{
				this.request = this.InternalParseRequest(reader);
			}
		}

		public virtual async Task ExecuteAsync()
		{
			base.CheckDisposed();
			this.TraceBegin();
			TimeSpan requestDelay;
			TimeSpan responseDelay;
			this.context.GetTimingOverrides(out requestDelay, out responseDelay);
			if (requestDelay != TimeSpan.Zero)
			{
				await Task.Delay(requestDelay);
			}
			this.response = await this.InternalExecuteAsync(this.request);
			if (responseDelay != TimeSpan.Zero)
			{
				await Task.Delay(responseDelay);
			}
			this.TraceEnd();
		}

		public virtual void SerializeResponse(out WorkBuffer[] responseBuffers)
		{
			base.CheckDisposed();
			int maxSizeOfBuffer = (int)this.response.GetSerializedSize();
			WorkBuffer workBuffer = new WorkBuffer(maxSizeOfBuffer);
			try
			{
				using (BufferWriter bufferWriter = new BufferWriter(workBuffer.ArraySegment))
				{
					this.response.Serialize(bufferWriter);
					workBuffer.Count = (int)bufferWriter.Position;
				}
				responseBuffers = new WorkBuffer[]
				{
					workBuffer
				};
				workBuffer = null;
			}
			finally
			{
				Util.DisposeIfPresent(workBuffer);
			}
		}

		public virtual void Prepare()
		{
			base.CheckDisposed();
			this.sessionContextActivity = null;
			string userAuthIdentifier;
			if (!this.context.TryGetUserAuthIdentifier(out userAuthIdentifier))
			{
				throw ProtocolException.FromResponseCode((LID)43100, "Unable to determine user authentication identifier.", ResponseCode.AnonymousNotAllowed, null);
			}
			string empty;
			if (!this.context.TryGetMailboxIdParameter(out empty))
			{
				empty = string.Empty;
			}
			TimeSpan sessionContextIdleTimeout;
			if (!this.context.TryGetExpirationInfo(out sessionContextIdleTimeout))
			{
				sessionContextIdleTimeout = Constants.SessionContextIdleTimeout;
			}
			string cookie;
			Exception ex2;
			if ((this.cookieFlags & AsyncOperationCookieFlags.RequireSession) == AsyncOperationCookieFlags.RequireSession)
			{
				string contextCookie = this.context.GetContextCookie();
				if ((this.cookieFlags & AsyncOperationCookieFlags.AllowInvalidSession) == AsyncOperationCookieFlags.AllowInvalidSession)
				{
					Exception ex;
					SessionContextManager.TryGetSessionContextActivity(contextCookie, userAuthIdentifier, sessionContextIdleTimeout, out this.sessionContextActivity, out ex);
				}
				else
				{
					this.sessionContextActivity = SessionContextManager.GetSessionContextActivity(contextCookie, userAuthIdentifier, sessionContextIdleTimeout);
				}
				if (this.SessionContext != null && (this.cookieFlags & AsyncOperationCookieFlags.RequireSequence) == AsyncOperationCookieFlags.RequireSequence)
				{
					string sequenceCookie = this.context.GetSequenceCookie();
					string sequenceCookie2 = this.SessionContext.Identifier.BeginSequenceOperation(sequenceCookie);
					this.context.SetSequenceCookie(sequenceCookie2, this.cookieVdirPath);
					this.currentSequenceCookie = sequenceCookie;
				}
			}
			else if ((this.cookieFlags & AsyncOperationCookieFlags.AllowSession) == AsyncOperationCookieFlags.AllowSession && this.context.TryGetContextCookie(out cookie) && !SessionContextManager.TryGetSessionContextActivity(cookie, userAuthIdentifier, sessionContextIdleTimeout, out this.sessionContextActivity, out ex2) && (this.cookieFlags & AsyncOperationCookieFlags.AllowInvalidSession) == AsyncOperationCookieFlags.None)
			{
				throw ex2;
			}
			if ((this.cookieFlags & AsyncOperationCookieFlags.CreateSession) == AsyncOperationCookieFlags.CreateSession)
			{
				if (this.SessionContext != null)
				{
					this.SessionContext.MarkForRundown(SessionRundownReason.ClientRecreate);
					Util.DisposeIfPresent(this.sessionContextActivity);
					this.sessionContextActivity = null;
				}
				string text;
				string text2;
				string text3;
				string text4;
				string text5;
				if (!this.context.TryGetUserIdentityInfo(out text, out text2, out text3, out text4, out text5))
				{
					throw ProtocolException.FromResponseCode((LID)57852, "Unable to determine user identity information.", ResponseCode.AnonymousNotAllowed, null);
				}
				if (string.IsNullOrEmpty(text3))
				{
					throw ProtocolException.FromResponseCode((LID)33276, "Unable to determine user security information.", ResponseCode.AnonymousNotAllowed, null);
				}
				this.sessionContextActivity = SessionContextManager.CreateSessionContextActivity(new SessionContextIdentifier(), sessionContextIdleTimeout, userAuthIdentifier, string.IsNullOrEmpty(text) ? string.Empty : text, string.IsNullOrEmpty(text2) ? string.Empty : text2, text3, string.IsNullOrEmpty(text5) ? string.Empty : text5, string.IsNullOrEmpty(empty) ? string.Empty : empty);
			}
			if (this.SessionContext != null)
			{
				this.context.SetContextCookie(this.SessionContext.Identifier.Cookie, this.cookieVdirPath);
				this.context.SetExpirationInfo(this.SessionContext.ExpirationInfo);
				if ((this.cookieFlags & AsyncOperationCookieFlags.CreateSession) == AsyncOperationCookieFlags.CreateSession)
				{
					this.context.SetSequenceCookie(this.SessionContext.Identifier.NextSequenceCookie, this.cookieVdirPath);
				}
			}
			string empty2;
			if (!this.context.TryGetCafeActivityId(out empty2))
			{
				empty2 = string.Empty;
			}
			string empty3;
			string serverAddress;
			if (!this.context.TryGetClientIPEndpoints(out empty3, out serverAddress))
			{
				empty3 = string.Empty;
			}
			string empty4 = string.Empty;
			this.context.TryGetSourceCafeServer(out empty4);
			this.protocolRequestInfo = new AsyncOperation.MapiHttpProtocolRequestInfo(this.requestId, empty2, empty4, this.Cookie, this.SequenceCookie, empty3, serverAddress, empty);
			if (this.SessionContext != null)
			{
				this.asyncOperationInfo = new AsyncOperationInfo(this.RequestType, this.RequestId, this.SequenceCookie, empty4, empty2, empty3);
				this.SessionContext.AsyncOperationTracker.Register(this.asyncOperationInfo);
			}
		}

		public void Cleanup()
		{
			base.CheckDisposed();
			if (this.SessionContext != null)
			{
				if ((this.cookieFlags & AsyncOperationCookieFlags.DestroySession) == AsyncOperationCookieFlags.DestroySession)
				{
					this.SessionContext.MarkForRundown(SessionRundownReason.ClientRundown);
				}
				if (this.currentSequenceCookie != null)
				{
					this.SessionContext.Identifier.EndSequenceOperation();
					this.currentSequenceCookie = null;
				}
			}
		}

		public void OnComplete(Exception failureException)
		{
			base.CheckDisposed();
			if (this.SessionContext != null)
			{
				if ((this.cookieFlags & AsyncOperationCookieFlags.CreateSession) != AsyncOperationCookieFlags.None)
				{
					if (failureException != null)
					{
						this.SessionContext.MarkForRundown(SessionRundownReason.ProtocolFault);
					}
					else if (this.ContextHandle == null)
					{
						this.SessionContext.MarkForRundown(SessionRundownReason.ContextHandleCleared);
					}
				}
				if (this.currentSequenceCookie != null)
				{
					this.SessionContext.Identifier.EndSequenceOperation();
					this.currentSequenceCookie = null;
				}
			}
			if (this.asyncOperationInfo != null)
			{
				this.asyncOperationInfo.OnComplete(failureException);
				if (this.SessionContext != null)
				{
					this.SessionContext.AsyncOperationTracker.Complete(this.asyncOperationInfo);
				}
				this.asyncOperationInfo = null;
			}
		}

		public void AppendLogString(StringBuilder stringBuilder)
		{
			if (this.response != null)
			{
				this.response.AppendLogString(stringBuilder);
			}
		}

		protected static string CombineTraceParameters(string trace1, string trace2)
		{
			if (string.IsNullOrEmpty(trace2))
			{
				return trace1;
			}
			if (string.IsNullOrEmpty(trace1))
			{
				return trace2;
			}
			return string.Format("{0}; {1}", trace1, trace2);
		}

		protected static string CreateCookieVdirPath(string vdirPath)
		{
			if (!string.IsNullOrEmpty(vdirPath))
			{
				return vdirPath.TrimEnd(AsyncOperation.slashDelimiter);
			}
			return "/";
		}

		protected virtual MapiHttpRequest InternalParseRequest(Reader reader)
		{
			throw new NotImplementedException();
		}

		protected virtual Task<MapiHttpResponse> InternalExecuteAsync(MapiHttpRequest mapiHttpRequest)
		{
			throw new NotImplementedException();
		}

		protected ArraySegment<byte> AllocateBuffer(int requestedSize, int maxAllowedSize)
		{
			if (requestedSize > maxAllowedSize)
			{
				throw ProtocolException.FromResponseCode((LID)33500, string.Format("Requested output buffer size({0}) bigger than the maximum allowed({1})", requestedSize, maxAllowedSize), ResponseCode.InvalidPayload, null);
			}
			WorkBuffer workBuffer = new WorkBuffer(requestedSize);
			this.allocatedBuffers.Add(workBuffer);
			return workBuffer.ArraySegment;
		}

		protected ArraySegment<byte> AllocateErrorBuffer()
		{
			return this.AllocateBuffer(4104, 4104);
		}

		protected MapiHttpClientBinding GetMapiHttpClientBinding(Func<ClientSecurityContext> clientSecurityContextGetter)
		{
			return new MapiHttpClientBinding(this.ProtocolRequestInfo.MailboxIdentifier, this.ProtocolRequestInfo.ClientAddress, this.ProtocolRequestInfo.ServerAddress, this.Context.Request.IsSecureConnection, this.Context.User.Identity, clientSecurityContextGetter);
		}

		protected ClientSecurityContext GetInitialCachedClientSecurityContext()
		{
			ClientSecurityContext result;
			try
			{
				result = this.context.User.Identity.CreateClientSecurityContext(false);
			}
			catch (AuthzException innerException)
			{
				throw ProtocolException.FromHttpStatusCode((LID)37900, "Could not create a client security context from a user identity.", string.Empty, HttpStatusCode.ServiceUnavailable, HttpStatusCode.ServiceUnavailable.ToString(), null, null, innerException);
			}
			return result;
		}

		protected override void InternalDispose()
		{
			Util.DisposeIfPresent(this.sessionContextActivity);
			this.DisposeAllocatedBuffers();
			base.InternalDispose();
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<AsyncOperation>(this);
		}

		private static void ValidateAcceptTypes(HttpRequestBase request, ref string contentType)
		{
			string[] acceptTypes = request.AcceptTypes;
			if (acceptTypes == null || acceptTypes.Length == 0)
			{
				return;
			}
			string[] array = acceptTypes;
			int i = 0;
			while (i < array.Length)
			{
				string text = array[i];
				string text2 = text;
				int num = text2.IndexOf(';');
				if (num == -1)
				{
					goto IL_3F;
				}
				if (num > 0)
				{
					text2 = text2.Substring(0, num).Trim();
					goto IL_3F;
				}
				IL_8B:
				i++;
				continue;
				IL_3F:
				if (string.Equals(text2, "application/mapi-http", StringComparison.InvariantCultureIgnoreCase))
				{
					contentType = "application/mapi-http";
				}
				else if (string.Equals(text2, "application/octet-stream", StringComparison.InvariantCultureIgnoreCase))
				{
					contentType = "application/octet-stream";
				}
				else if (!string.Equals(text2, "application/*", StringComparison.InvariantCultureIgnoreCase) && !string.Equals(text2, "*/*", StringComparison.InvariantCultureIgnoreCase))
				{
					goto IL_8B;
				}
				return;
			}
			throw ProtocolException.FromResponseCode((LID)62716, string.Format("Invalid Accept header; expecting {0}", "application/mapi-http"), ResponseCode.InvalidHeader, null);
		}

		private void TraceOperation(string action, string parameters)
		{
			ExTraceGlobals.AsyncOperationTracer.TraceInformation(34208, 0L, "[{0}] {1}: {2}{3}", new object[]
			{
				this.RequestId,
				this.RequestType,
				action,
				string.IsNullOrEmpty(parameters) ? string.Empty : string.Format("; {0}", parameters)
			});
		}

		private void TraceBegin()
		{
			if (ExTraceGlobals.AsyncOperationTracer.IsTraceEnabled(TraceType.InfoTrace))
			{
				this.TraceOperation("Begin", this.GetTraceBeginParameters(this.request));
			}
		}

		private void TraceEnd()
		{
			if (ExTraceGlobals.AsyncOperationTracer.IsTraceEnabled(TraceType.InfoTrace))
			{
				this.TraceOperation("End", this.GetTraceEndParameters(this.request, this.response));
			}
		}

		private void DisposeAllocatedBuffers()
		{
			foreach (WorkBuffer workBuffer in this.allocatedBuffers)
			{
				workBuffer.Dispose();
			}
			this.allocatedBuffers.Clear();
		}

		private static readonly char[] slashDelimiter = new char[]
		{
			'/'
		};

		private static readonly TimeSpan initialFlushPeriod = TimeSpan.FromSeconds(2.0);

		private readonly IList<WorkBuffer> allocatedBuffers = new List<WorkBuffer>();

		private readonly HttpContextBase context;

		private readonly string requestId;

		private readonly string cookieVdirPath;

		private readonly string contentType;

		private readonly AsyncOperationCookieFlags cookieFlags;

		private MapiHttpRequest request;

		private MapiHttpResponse response;

		private SessionContextActivity sessionContextActivity;

		private AsyncOperationInfo asyncOperationInfo;

		private string currentSequenceCookie;

		private AsyncOperation.MapiHttpProtocolRequestInfo protocolRequestInfo;

		protected class MapiHttpProtocolRequestInfo : ProtocolRequestInfo
		{
			public MapiHttpProtocolRequestInfo(string requestId, string cafeActivityId, string sourceCafeServer, string contextCookie, string sequenceCookie, string clientAddress, string serverAddress, string mailboxIdentifier)
			{
				List<string> list = new List<string>(3);
				if (!string.IsNullOrEmpty(requestId))
				{
					list.Add("R:" + requestId);
				}
				if (!string.IsNullOrEmpty(cafeActivityId))
				{
					list.Add("A:" + cafeActivityId);
				}
				if (!string.IsNullOrEmpty(sourceCafeServer))
				{
					list.Add("FE:" + sourceCafeServer);
				}
				if (list.Count > 0)
				{
					this.requestIds = list.ToArray();
				}
				list.Clear();
				if (!string.IsNullOrEmpty(contextCookie))
				{
					list.Add("C:" + contextCookie);
				}
				if (!string.IsNullOrEmpty(sequenceCookie))
				{
					list.Add("S:" + sequenceCookie);
				}
				if (list.Count > 0)
				{
					this.cookies = list.ToArray();
				}
				this.clientAddress = clientAddress;
				this.serverAddress = serverAddress;
				this.mailboxIdentifier = mailboxIdentifier;
			}

			public override string[] RequestIds
			{
				get
				{
					return this.requestIds;
				}
			}

			public override string[] Cookies
			{
				get
				{
					return this.cookies;
				}
			}

			public override string ClientAddress
			{
				get
				{
					return this.clientAddress;
				}
			}

			public string ServerAddress
			{
				get
				{
					return this.serverAddress;
				}
			}

			public string MailboxIdentifier
			{
				get
				{
					return this.mailboxIdentifier;
				}
			}

			private readonly string[] requestIds;

			private readonly string[] cookies;

			private readonly string clientAddress;

			private readonly string serverAddress;

			private readonly string mailboxIdentifier;
		}
	}
}
