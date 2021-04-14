using System;
using System.Collections.Generic;
using Microsoft.Exchange.Connections.Common;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Pop
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	public sealed class Pop3Connection : DisposeTrackableBase, IPop3Connection, IConnection<IPop3Connection>, IDisposable
	{
		private Pop3Connection(ConnectionParameters connectionParameters)
		{
			this.connectionParameters = connectionParameters;
		}

		public Pop3ConnectionContext ConnectionContext
		{
			get
			{
				base.CheckDisposed();
				return this.connectionContext;
			}
		}

		private ConnectionParameters ConnectionParameters
		{
			get
			{
				base.CheckDisposed();
				return this.connectionParameters;
			}
		}

		public static IPop3Connection CreateInstance(ConnectionParameters connectionParameters)
		{
			return Pop3Connection.hookableFactory.Value(connectionParameters).Initialize();
		}

		public IPop3Connection Initialize()
		{
			this.connectionContext = new Pop3ConnectionContext(this.ConnectionParameters, null);
			return this;
		}

		public void ConnectAndAuthenticate(Pop3ServerParameters serverParameters, Pop3AuthenticationParameters authenticationParameters)
		{
			base.CheckDisposed();
			Pop3ConnectionContext pop3ConnectionContext = this.ConnectionContext;
			pop3ConnectionContext.AuthenticationParameters = authenticationParameters;
			pop3ConnectionContext.ServerParameters = serverParameters;
			pop3ConnectionContext.Client = new Pop3Client(serverParameters, authenticationParameters, this.connectionParameters, null, null);
		}

		public Pop3ResultData GetUniqueIds()
		{
			base.CheckDisposed();
			AsyncOperationResult<Pop3ResultData> uniqueIds = Pop3ConnectionCore.GetUniqueIds(this.ConnectionContext, null, null);
			this.ThrowIfExceptionNotNull(uniqueIds.Exception);
			return uniqueIds.Data;
		}

		public Pop3ResultData DeleteEmails(List<int> messageIds)
		{
			base.CheckDisposed();
			AsyncOperationResult<Pop3ResultData> asyncOperationResult = Pop3ConnectionCore.DeleteEmails(this.ConnectionContext, messageIds, null, null);
			this.ThrowIfExceptionNotNull(asyncOperationResult.Exception);
			return asyncOperationResult.Data;
		}

		public Pop3ResultData GetEmail(int messageId)
		{
			base.CheckDisposed();
			AsyncOperationResult<Pop3ResultData> email = Pop3ConnectionCore.GetEmail(this.ConnectionContext, messageId, null, null);
			this.ThrowIfExceptionNotNull(email.Exception);
			return email.Data;
		}

		public Pop3ResultData Quit()
		{
			base.CheckDisposed();
			AsyncOperationResult<Pop3ResultData> asyncOperationResult = Pop3ConnectionCore.Quit(this.ConnectionContext, null, null);
			this.ThrowIfExceptionNotNull(asyncOperationResult.Exception);
			return asyncOperationResult.Data;
		}

		public Pop3ResultData VerifyAccount()
		{
			base.CheckDisposed();
			AsyncOperationResult<Pop3ResultData> asyncOperationResult = Pop3ConnectionCore.VerifyAccount(this.ConnectionContext, null, null);
			this.ThrowIfExceptionNotNull(asyncOperationResult.Exception);
			return asyncOperationResult.Data;
		}

		internal static IDisposable SetTestHook(Func<ConnectionParameters, IPop3Connection> newFactory)
		{
			return Pop3Connection.hookableFactory.SetTestHook(newFactory);
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing && this.ConnectionContext != null)
			{
				this.ConnectionContext.Dispose();
				this.connectionContext = null;
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<Pop3Connection>(this);
		}

		private static IPop3Connection Factory(ConnectionParameters connectionParameters)
		{
			return new Pop3Connection(connectionParameters).Initialize();
		}

		private void ThrowIfExceptionNotNull(Exception exceptionOrNull)
		{
			if (exceptionOrNull == null)
			{
				return;
			}
			if (exceptionOrNull is LocalizedException)
			{
				throw exceptionOrNull;
			}
			string fullName = exceptionOrNull.GetType().FullName;
			throw new UnhandledException(fullName, exceptionOrNull);
		}

		private static Hookable<Func<ConnectionParameters, IPop3Connection>> hookableFactory = Hookable<Func<ConnectionParameters, IPop3Connection>>.Create(true, new Func<ConnectionParameters, IPop3Connection>(Pop3Connection.Factory));

		private Pop3ConnectionContext connectionContext;

		private ConnectionParameters connectionParameters;
	}
}
