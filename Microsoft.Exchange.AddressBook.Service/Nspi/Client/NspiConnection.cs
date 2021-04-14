using System;
using System.Threading;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.AddressBook.Service;
using Microsoft.Exchange.Nspi;
using Microsoft.Exchange.Nspi.Client;
using Microsoft.Exchange.Rpc;

namespace Microsoft.Exchange.AddressBook.Nspi.Client
{
	internal class NspiConnection : IDisposable
	{
		internal NspiConnection(NspiConnectionPool owningPool)
		{
			this.owningPool = owningPool;
		}

		public NspiClient Client
		{
			get
			{
				return this.client;
			}
		}

		public string Server
		{
			get
			{
				return this.owningPool.Server;
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
		}

		public NspiStatus Connect()
		{
			NspiConnection.NspiConnectionTracer.TraceDebug<string>(0L, "NspiConnection.Connect: owningPool.Server={0}", this.owningPool.Server ?? "(null)");
			NspiClient nspiClient = null;
			for (int i = 0; i < 3; i++)
			{
				try
				{
					if (i != 0)
					{
						Thread.Sleep(NspiConnection.RetryInterval);
					}
					nspiClient = new NspiClient(this.owningPool.Server);
					NspiStatus nspiStatus = nspiClient.Bind(NspiBindFlags.None);
					if (nspiStatus == NspiStatus.Success)
					{
						NspiConnection.NspiConnectionTracer.TraceDebug<string>(0L, "Bind to {0} succeeded", this.owningPool.Server ?? "(null)");
						this.client = nspiClient;
						nspiClient = null;
						return NspiStatus.Success;
					}
					NspiConnection.NspiConnectionTracer.TraceDebug<string, NspiStatus>(0L, "Bind to {0} failed with status {1}", this.owningPool.Server ?? "(null)", nspiStatus);
				}
				catch (RpcException innerException)
				{
					throw new ADTransientException(DirectoryStrings.ExceptionServerUnavailable(this.owningPool.Server), innerException);
				}
				finally
				{
					if (nspiClient != null)
					{
						nspiClient.Dispose();
						nspiClient = null;
					}
				}
			}
			NspiConnection.NspiConnectionTracer.TraceDebug<int, string>(0L, "All {0} attempts to connect to {1} failed", 3, this.owningPool.Server ?? "(null)");
			return NspiStatus.GeneralFailure;
		}

		public void ReturnToPool()
		{
			this.returningToPool = true;
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (this.returningToPool)
				{
					this.returningToPool = false;
					this.owningPool.ReturnToPool(this);
					return;
				}
				if (this.client != null)
				{
					this.client.Dispose();
					this.client = null;
				}
				GC.SuppressFinalize(this);
			}
		}

		private const int BindRetries = 3;

		internal static readonly Trace NspiConnectionTracer = ExTraceGlobals.NspiConnectionTracer;

		private static readonly TimeSpan RetryInterval = TimeSpan.FromSeconds(1.0);

		private readonly NspiConnectionPool owningPool;

		private NspiClient client;

		private bool returningToPool;
	}
}
