using System;
using System.Collections.Generic;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;

namespace Microsoft.Exchange.UnifiedContent
{
	public class ContentManager : IDisposable
	{
		internal ContentManager(string path)
		{
			this.tempFilePath = this.CreateUnifiedContentTempPath(path);
		}

		internal List<IExtractedContent> ContentCollection { get; set; }

		internal string ShareName
		{
			get
			{
				this.GetSharedStream();
				return this.sharedStream.SharedName;
			}
		}

		internal string TempFilePath
		{
			get
			{
				return this.tempFilePath;
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		internal SharedStream GetSharedStream()
		{
			if (this.sharedStream == null)
			{
				FileSecurity fileSecurity = new FileSecurity();
				fileSecurity.AddAccessRule(new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.NetworkServiceSid, null), FileSystemRights.FullControl, AccessControlType.Allow));
				fileSecurity.AddAccessRule(new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.LocalServiceSid, null), FileSystemRights.FullControl, AccessControlType.Allow));
				fileSecurity.AddAccessRule(new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.LocalSystemSid, null), FileSystemRights.FullControl, AccessControlType.Allow));
				fileSecurity.AddAccessRule(new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.BuiltinAdministratorsSid, null), FileSystemRights.FullControl, AccessControlType.Allow));
				this.sharedStream = SharedStream.Create(this.TempFilePath, 1048576L, fileSecurity);
				byte[] array = new byte[]
				{
					0,
					1,
					2,
					3,
					4,
					5,
					6,
					7
				};
				this.sharedStream.Write(array, 0, array.Length);
			}
			return this.sharedStream;
		}

		internal void ClearContent()
		{
			if (this.sharedStream != null)
			{
				this.sharedStream.Dispose();
				this.sharedStream = null;
			}
			this.ContentCollection = null;
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.ThrowIfDisposed();
			}
			if (!this.disposed)
			{
				this.ClearContent();
				this.disposed = true;
			}
		}

		private string CreateUnifiedContentTempPath(string path)
		{
			string text = Path.Combine(path, "UnifiedContent");
			if (!Directory.Exists(text))
			{
				Directory.CreateDirectory(text);
			}
			return text;
		}

		private void ThrowIfDisposed()
		{
			if (this.disposed)
			{
				throw new InvalidOperationException("Object disposed.");
			}
		}

		private readonly string tempFilePath;

		private bool disposed;

		private SharedStream sharedStream;
	}
}
