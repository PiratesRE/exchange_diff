using System;
using System.IO;
using Microsoft.Exchange.Win32;

namespace Microsoft.Exchange.Transport.Pickup
{
	internal sealed class FileList : IDisposable
	{
		internal FileList(string path, string filter)
		{
			this.path = path;
			this.filter = filter;
		}

		public void Dispose()
		{
			this.StopSearch();
		}

		internal bool GetNextFile(out string fileName, out ulong fileSize)
		{
			bool flag = false;
			NativeMethods.WIN32_FIND_DATA win32_FIND_DATA;
			win32_FIND_DATA.FileName = string.Empty;
			win32_FIND_DATA.FileSizeHigh = 0U;
			win32_FIND_DATA.FileSizeLow = 0U;
			bool flag2;
			do
			{
				if (this.safeFindHandle == null)
				{
					string fileName2 = Path.Combine(this.path, this.filter);
					this.safeFindHandle = NativeMethods.FindFirstFile(fileName2, out win32_FIND_DATA);
					flag2 = !this.safeFindHandle.IsInvalid;
				}
				else
				{
					flag2 = NativeMethods.FindNextFile(this.safeFindHandle, out win32_FIND_DATA);
				}
				if (flag2 && (win32_FIND_DATA.FileAttributes & NativeMethods.FileAttributes.Directory) == NativeMethods.FileAttributes.None)
				{
					flag = true;
				}
			}
			while (flag2 && !flag);
			if (flag)
			{
				fileName = Path.Combine(this.path, win32_FIND_DATA.FileName);
				fileSize = (ulong)(win32_FIND_DATA.FileSizeHigh + win32_FIND_DATA.FileSizeLow);
			}
			else
			{
				fileName = string.Empty;
				fileSize = 0UL;
				this.StopSearch();
			}
			return flag;
		}

		internal void StopSearch()
		{
			if (this.safeFindHandle != null)
			{
				this.safeFindHandle.Close();
				this.safeFindHandle = null;
			}
		}

		private string path;

		private string filter;

		private SafeFindHandle safeFindHandle;
	}
}
