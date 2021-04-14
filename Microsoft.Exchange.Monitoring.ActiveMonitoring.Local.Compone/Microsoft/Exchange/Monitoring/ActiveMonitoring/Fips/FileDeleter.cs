using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Fips
{
	internal static class FileDeleter
	{
		public static void Delete(string directoryPath, IFileDeletionPolicy policy)
		{
			if (!string.IsNullOrEmpty(directoryPath) && policy != null)
			{
				IEnumerable<string> enumerable = from o in Directory.GetFiles(directoryPath, "*.*", SearchOption.TopDirectoryOnly)
				let filePath = Path.Combine(directoryPath, o)
				where policy.ShouldDelete(filePath)
				select o;
				foreach (string path in enumerable)
				{
					try
					{
						File.Delete(path);
					}
					catch (IOException)
					{
					}
				}
			}
		}

		private const string SearchPattern = "*.*";
	}
}
