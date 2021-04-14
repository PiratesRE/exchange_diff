using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Mapi
{
	internal class ConnectionPool : IDisposeTrackable, IDisposable
	{
		private ConnectionPool()
		{
			this.disposeTracker = this.GetDisposeTracker();
		}

		public DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<ConnectionPool>(this);
		}

		public void SuppressDisposeTracker()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Suppress();
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		public ExRpcAdmin GetAdministration(string server)
		{
			return ExRpcAdmin.Create("Client=Management", server, null, null, null);
		}

		public void ReturnBack(ExRpcAdmin connection)
		{
			connection.Dispose();
		}

		private void Dispose(bool disposing)
		{
			if (disposing && this.disposeTracker != null)
			{
				this.disposeTracker.Dispose();
			}
		}

		internal const ConnectFlag ConnectionFlags = ConnectFlag.UseAdminPrivilege;

		private const int MaximumConnectionCacheSize = 10;

		private const bool DisallowConnectionCacheOverflow = false;

		internal static readonly ConnectionPool Instance = new ConnectionPool();

		private DisposeTracker disposeTracker;
	}
}
