using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class TemporaryFile : DisposeTrackableBase
	{
		public TemporaryFile(string fileFullPath)
		{
			this.m_fileFullPath = fileFullPath;
		}

		public static implicit operator string(TemporaryFile tempFile)
		{
			return tempFile.m_fileFullPath;
		}

		public static implicit operator TemporaryFile(string fileFullPath)
		{
			return new TemporaryFile(fileFullPath);
		}

		public override string ToString()
		{
			return this.m_fileFullPath;
		}

		public override int GetHashCode()
		{
			return this.m_fileFullPath.GetHashCode();
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				FileCleanup.TryDelete(this.m_fileFullPath);
			}
			this.m_fileFullPath = null;
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<TemporaryFile>(this);
		}

		private string m_fileFullPath;
	}
}
