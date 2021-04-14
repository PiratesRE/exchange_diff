using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.Exchange.Configuration.Tasks
{
	internal class FileSearchUtil
	{
		public FileSearchUtil(string dirRoot, string searchPattern)
		{
			if (string.IsNullOrEmpty(dirRoot))
			{
				throw new ArgumentException("Argument 'path' was null or emtpy.");
			}
			if (string.IsNullOrEmpty(searchPattern))
			{
				throw new ArgumentException("Argument 'searchPattern' was null or emtpy.");
			}
			this.path = dirRoot;
			this.searchPattern = searchPattern;
		}

		public List<string> GetFilesRecurse()
		{
			this.fileList = new List<string>();
			this.TraverseDirectoryTree(this.path, 10);
			return this.fileList;
		}

		private void TraverseDirectoryTree(string root, int maxDirectoryDepth)
		{
			KeyValuePair<string, int> item = new KeyValuePair<string, int>(root, 0);
			Stack<KeyValuePair<string, int>> stack = new Stack<KeyValuePair<string, int>>();
			if (!Directory.Exists(root))
			{
				throw new ArgumentException("Directory 'root' does not exist", "root");
			}
			stack.Push(item);
			while (stack.Count > 0)
			{
				KeyValuePair<string, int> keyValuePair = stack.Pop();
				string[] array = null;
				try
				{
					array = Directory.GetDirectories(keyValuePair.Key);
				}
				catch (UnauthorizedAccessException)
				{
					continue;
				}
				catch (DirectoryNotFoundException)
				{
					continue;
				}
				if (keyValuePair.Value < maxDirectoryDepth)
				{
					foreach (string key in array)
					{
						stack.Push(new KeyValuePair<string, int>(key, keyValuePair.Value + 1));
					}
				}
				FileInfo[] array3 = null;
				try
				{
					DirectoryInfo directoryInfo = new DirectoryInfo(keyValuePair.Key);
					array3 = directoryInfo.GetFiles(this.searchPattern);
				}
				catch (UnauthorizedAccessException)
				{
					continue;
				}
				catch (DirectoryNotFoundException)
				{
					continue;
				}
				if (array3 != null)
				{
					foreach (FileInfo fileInfo in array3)
					{
						this.fileList.Add(fileInfo.FullName);
					}
				}
			}
		}

		private const int recurseDepthLimit = 10;

		private string path;

		private string searchPattern;

		private List<string> fileList;
	}
}
