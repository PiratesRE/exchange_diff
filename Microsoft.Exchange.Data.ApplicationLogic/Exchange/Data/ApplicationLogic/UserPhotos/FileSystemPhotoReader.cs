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
	internal sealed class FileSystemPhotoReader : IFileSystemPhotoReader
	{
		public FileSystemPhotoReader(ITracer upstreamTracer)
		{
			if (upstreamTracer == null)
			{
				throw new ArgumentNullException("upstreamTracer");
			}
			this.tracer = ExTraceGlobals.UserPhotosTracer.Compose(upstreamTracer);
		}

		public PhotoMetadata Read(string photoFullPath, Stream output)
		{
			if (string.IsNullOrEmpty(photoFullPath))
			{
				throw new ArgumentNullException("photoFullPath");
			}
			if (output == null)
			{
				throw new ArgumentNullException("output");
			}
			this.tracer.TraceDebug<string>((long)this.GetHashCode(), "File system photo reader: reading photo file {0}", photoFullPath);
			PhotoMetadata result;
			using (FileStream fileStream = new FileStream(photoFullPath, FileMode.Open, FileAccess.Read, FileShare.Read | FileShare.Delete))
			{
				this.tracer.TraceDebug((long)this.GetHashCode(), "File system photo reader: writing photo to output stream.");
				fileStream.CopyTo(output);
				result = new PhotoMetadata
				{
					Length = fileStream.Length,
					ContentType = "image/jpeg"
				};
			}
			return result;
		}

		public int ReadThumbprint(string photoFullPath)
		{
			if (string.IsNullOrEmpty(photoFullPath))
			{
				throw new ArgumentNullException("photoFullPath");
			}
			string thumbprintFullPath = FileSystemPhotoReader.GetThumbprintFullPath(photoFullPath);
			this.tracer.TraceDebug<string>((long)this.GetHashCode(), "File system photo reader: reading thumbprint from file {0}", thumbprintFullPath);
			NativeMethods.SECURITY_ATTRIBUTES security_ATTRIBUTES = new NativeMethods.SECURITY_ATTRIBUTES(SafeHGlobalHandle.InvalidHandle);
			int result;
			using (SafeFileHandle safeFileHandle = NativeMethods.CreateFile(thumbprintFullPath, (NativeMethods.CreateFileAccess)2147483648U, NativeMethods.CreateFileShare.Read | NativeMethods.CreateFileShare.Delete, ref security_ATTRIBUTES, FileMode.Open, NativeMethods.CreateFileFileAttributes.None, IntPtr.Zero))
			{
				if (safeFileHandle.IsInvalid)
				{
					int lastWin32Error = Marshal.GetLastWin32Error();
					this.tracer.TraceDebug<int>((long)this.GetHashCode(), "File system photo reader: failed to open file.  Win32 error: {0}", lastWin32Error);
					throw new Win32Exception(lastWin32Error);
				}
				using (FileStream fileStream = new FileStream(safeFileHandle, FileAccess.Read))
				{
					using (BinaryReader binaryReader = new BinaryReader(fileStream))
					{
						int num = binaryReader.ReadInt32();
						this.tracer.TraceDebug<int>((long)this.GetHashCode(), "File system photo reader: thumbprint read: {0:X8}", num);
						result = num;
					}
				}
			}
			return result;
		}

		public DateTime GetLastModificationTimeUtc(string photoFullPath)
		{
			return File.GetLastWriteTimeUtc(photoFullPath);
		}

		private static string GetThumbprintFullPath(string photoFullPath)
		{
			return photoFullPath + ":thumbprint";
		}

		private const string ThumbprintStream = ":thumbprint";

		private const string PhotoContentType = "image/jpeg";

		private ITracer tracer = ExTraceGlobals.UserPhotosTracer;
	}
}
