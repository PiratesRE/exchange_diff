using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Threading;
using Microsoft.Exchange.CtsResources;
using Microsoft.Win32.SafeHandles;

namespace Microsoft.Exchange.Data.Internal
{
	internal class TempFileStream : FileStream
	{
		internal static string Path
		{
			get
			{
				return TempFileStream.GetTempPath();
			}
		}

		public string FilePath
		{
			get
			{
				return this.filePath;
			}
		}

		private TempFileStream(SafeFileHandle handle) : base(handle, FileAccess.ReadWrite)
		{
		}

		public static TempFileStream CreateInstance()
		{
			return TempFileStream.CreateInstance("Cts");
		}

		public static TempFileStream CreateInstance(string prefix)
		{
			return TempFileStream.CreateInstance(prefix, true);
		}

		public static TempFileStream CreateInstance(string prefix, bool deleteOnClose)
		{
			NativeMethods.SecurityAttributes securityAttributes = new NativeMethods.SecurityAttributes(false);
			string path = TempFileStream.Path;
			new FileIOPermission(FileIOPermissionAccess.Write, path).Demand();
			int num = 0;
			int num2 = 10;
			string filename;
			SafeFileHandle safeFileHandle;
			for (;;)
			{
				filename = System.IO.Path.Combine(path, prefix + ((uint)Interlocked.Increment(ref TempFileStream.nextId)).ToString("X5") + ".tmp");
				uint num3 = deleteOnClose ? 67108864U : 0U;
				safeFileHandle = NativeMethods.CreateFile(filename, 1180063U, 0U, ref securityAttributes, 1U, 256U | num3 | 8192U, IntPtr.Zero);
				num2--;
				if (safeFileHandle.IsInvalid)
				{
					num = Marshal.GetLastWin32Error();
					if (num == 80)
					{
						num2++;
					}
					using (Process currentProcess = Process.GetCurrentProcess())
					{
						Interlocked.Add(ref TempFileStream.nextId, currentProcess.Id);
						goto IL_CB;
					}
					goto IL_C8;
				}
				goto IL_C8;
				IL_CB:
				if (num2 <= 0)
				{
					break;
				}
				continue;
				IL_C8:
				num2 = 0;
				goto IL_CB;
			}
			if (safeFileHandle.IsInvalid)
			{
				string message = SharedStrings.CreateFileFailed(filename);
				throw new IOException(message, new Win32Exception(num, message));
			}
			return new TempFileStream(safeFileHandle)
			{
				filePath = filename
			};
		}

		internal static void SetTemporaryPath(string path)
		{
			TempFileStream.tempPath = path;
		}

		private static string GetTempPath()
		{
			if (TempFileStream.tempPath == null)
			{
				TempFileStream.tempPath = System.IO.Path.GetTempPath();
			}
			return TempFileStream.tempPath;
		}

		protected override void Dispose(bool disposing)
		{
			try
			{
				base.Dispose(disposing);
			}
			catch (IOException)
			{
			}
		}

		private static string tempPath;

		private static int nextId = Environment.TickCount ^ Process.GetCurrentProcess().Id;

		private string filePath;
	}
}
