using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Win32;

namespace Microsoft.Exchange.Cluster.ClusApi
{
	internal class AmClusterPropListDisposable : AmClusterPropList, IDisposeTrackable, IDisposable
	{
		public AmClusterPropListDisposable(SafeHGlobalHandle buffer, uint bufferSize) : base(buffer.DangerousGetHandle(), bufferSize)
		{
			this.m_disposeTracker = this.GetDisposeTracker();
			this.HBuffer = buffer;
		}

		private SafeHGlobalHandle HBuffer { get; set; }

		public void Dispose()
		{
			if (!this.m_isDisposed)
			{
				this.Dispose(true);
				GC.SuppressFinalize(this);
			}
		}

		public void Dispose(bool disposing)
		{
			lock (this)
			{
				if (this.HBuffer != null)
				{
					this.HBuffer.Dispose();
					this.HBuffer = null;
				}
				if (this.m_disposeTracker != null)
				{
					this.m_disposeTracker.Dispose();
					this.m_disposeTracker = null;
				}
				this.m_isDisposed = true;
			}
		}

		public DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<AmClusterPropListDisposable>(this);
		}

		public void SuppressDisposeTracker()
		{
			if (this.m_disposeTracker != null)
			{
				this.m_disposeTracker.Suppress();
			}
		}

		private DisposeTracker m_disposeTracker;

		private bool m_isDisposed;
	}
}
