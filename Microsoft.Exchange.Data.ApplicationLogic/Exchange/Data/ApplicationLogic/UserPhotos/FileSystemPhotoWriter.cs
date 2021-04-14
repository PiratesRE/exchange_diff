using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.Win32;
using Microsoft.Win32.SafeHandles;

namespace Microsoft.Exchange.Data.ApplicationLogic.UserPhotos
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class FileSystemPhotoWriter : IFileSystemPhotoWriter
	{
		public FileSystemPhotoWriter(ITracer upstreamTracer)
		{
			if (upstreamTracer == null)
			{
				throw new ArgumentNullException("upstreamTracer");
			}
			this.tracer = ExTraceGlobals.UserPhotosTracer.Compose(upstreamTracer);
		}

		public void Write(string photoFullPath, int thumbprint, Stream photo)
		{
			FileSystemPhotoWriter.ThrowIfInvalidPhotoFullPath(photoFullPath);
			if (photoFullPath == null)
			{
				throw new ArgumentNullException("photoFullPath");
			}
			this.CreatePhotosDirectory(photoFullPath);
			this.tracer.TraceDebug<string, int>((long)this.GetHashCode(), "File system photo writer: writing photo file {0} with thumbprint {1:X8}", photoFullPath, thumbprint);
			NativeMethods.SECURITY_ATTRIBUTES security_ATTRIBUTES = new NativeMethods.SECURITY_ATTRIBUTES(SafeHGlobalHandle.InvalidHandle);
			using (FileStream fileStream = new FileStream(photoFullPath, FileMode.Create, FileAccess.Write, FileShare.None))
			{
				using (SafeFileHandle safeFileHandle = NativeMethods.CreateFile(photoFullPath + ":thumbprint", NativeMethods.CreateFileAccess.GenericWrite, NativeMethods.CreateFileShare.None, ref security_ATTRIBUTES, FileMode.Create, NativeMethods.CreateFileFileAttributes.None, IntPtr.Zero))
				{
					if (safeFileHandle.IsInvalid)
					{
						int lastWin32Error = Marshal.GetLastWin32Error();
						this.tracer.TraceDebug<int>((long)this.GetHashCode(), "File system photo writer: failed to create file.  Win32 error: {0}", lastWin32Error);
						throw new Win32Exception(lastWin32Error);
					}
					photo.CopyTo(fileStream);
					using (FileStream fileStream2 = new FileStream(safeFileHandle, FileAccess.Write))
					{
						using (BinaryWriter binaryWriter = new BinaryWriter(fileStream2))
						{
							binaryWriter.Write(thumbprint);
						}
					}
				}
			}
		}

		private void CreatePhotosDirectory(string photoFullPath)
		{
			string directoryName = Path.GetDirectoryName(photoFullPath);
			this.tracer.TraceDebug<string>((long)this.GetHashCode(), "File system photo writer: ensuring photos directory {0} exists.", directoryName);
			Directory.CreateDirectory(directoryName);
		}

		private static void ThrowIfInvalidPhotoFullPath(string photoFullPath)
		{
			if (string.IsNullOrEmpty(photoFullPath))
			{
				throw new ArgumentNullException("photoFullPath");
			}
			if (!Path.HasExtension(photoFullPath))
			{
				throw new ArgumentException("photoFullPath");
			}
		}

		internal const string ThumbprintStream = ":thumbprint";

		private ITracer tracer = ExTraceGlobals.UserPhotosTracer;
	}
}
