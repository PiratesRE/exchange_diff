using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Exchange.Cluster.Common;
using Microsoft.Exchange.Cluster.Replay.MountPoint;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Win32;

namespace Microsoft.Exchange.Cluster.Replay.IO
{
	public class DirectorySpaceEnumerator
	{
		public static IEnumerable<DirectorySpaceEnumerator.DirectorySize> GetTopNDirectories(string rootDirectory, int maxDirectories)
		{
			DirectorySpaceEnumerator directorySpaceEnumerator = new DirectorySpaceEnumerator(rootDirectory, maxDirectories);
			return directorySpaceEnumerator.GetTopNDirectories();
		}

		private DirectorySpaceEnumerator(string rootDirectory, int maxDirectories)
		{
			this.m_rootDirectory = rootDirectory;
			this.m_maxDirectories = maxDirectories;
		}

		private IEnumerable<DirectorySpaceEnumerator.DirectorySize> GetTopNDirectories()
		{
			TopNList<DirectorySpaceEnumerator.DirectorySize> topDirsBySize = new TopNList<DirectorySpaceEnumerator.DirectorySize>(this.m_maxDirectories, Dependencies.Assert);
			MountPointUtil.HandleIOExceptions(delegate
			{
				DirectoryInfo path = new DirectoryInfo(this.m_rootDirectory);
				using (DirectoryEnumerator directoryEnumerator = new DirectoryEnumerator(path, true, true))
				{
					Predicate<NativeMethods.WIN32_FIND_DATA> extendedFilter = (NativeMethods.WIN32_FIND_DATA findData) => (findData.FileAttributes & NativeMethods.FileAttributes.ReparsePoint) == NativeMethods.FileAttributes.None;
					foreach (string dir in directoryEnumerator.EnumerateDirectories("*", extendedFilter))
					{
						this.EnumerateFilesForDirectory(topDirsBySize, dir);
					}
					this.EnumerateFilesForDirectory(topDirsBySize, this.m_rootDirectory);
				}
			});
			return topDirsBySize;
		}

		private void EnumerateFilesForDirectory(TopNList<DirectorySpaceEnumerator.DirectorySize> topDirsBySize, string dir)
		{
			MountPointUtil.HandleIOExceptions(delegate
			{
				DirectoryInfo path = new DirectoryInfo(dir);
				using (DirectoryEnumerator directoryEnumerator = new DirectoryEnumerator(path, false, false))
				{
					ulong num = 0UL;
					string text;
					NativeMethods.WIN32_FIND_DATA win32_FIND_DATA;
					while (directoryEnumerator.GetNextFileExtendedInfo("*", out text, out win32_FIND_DATA))
					{
						ulong longFileSize = this.GetLongFileSize(win32_FIND_DATA.FileSizeLow, win32_FIND_DATA.FileSizeHigh);
						num += longFileSize;
					}
					this.InsertIfNecessary(topDirsBySize, dir, num);
				}
			});
		}

		private ulong GetLongFileSize(uint low, uint high)
		{
			return (ulong)high << 32 | (ulong)low;
		}

		private bool InsertIfNecessary(TopNList<DirectorySpaceEnumerator.DirectorySize> topDirsBySize, string dir, ulong dirSize)
		{
			DirectorySpaceEnumerator.DirectorySize directorySize = new DirectorySpaceEnumerator.DirectorySize(dir, dirSize);
			return topDirsBySize.InsertIfNecessary(directorySize);
		}

		private readonly int m_maxDirectories;

		private readonly string m_rootDirectory;

		public struct DirectorySize : IComparable<DirectorySpaceEnumerator.DirectorySize>
		{
			public DirectorySize(string dirName, ulong size)
			{
				this.Name = dirName;
				this.Size = ByteQuantifiedSize.FromBytes(size);
			}

			public int CompareTo(DirectorySpaceEnumerator.DirectorySize other)
			{
				return this.Size.CompareTo(other.Size);
			}

			public string Name;

			public ByteQuantifiedSize Size;
		}
	}
}
