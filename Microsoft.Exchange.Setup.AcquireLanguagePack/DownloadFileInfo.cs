using System;
using System.IO;
using System.Text.RegularExpressions;

namespace Microsoft.Exchange.Setup.AcquireLanguagePack
{
	internal sealed class DownloadFileInfo
	{
		public DownloadFileInfo(string uriLink) : this(uriLink, null, false)
		{
		}

		public DownloadFileInfo(string uriLink, string filePattern, bool ignoreInvalidFileName)
		{
			ValidationHelper.ThrowIfNull(uriLink, "uriLink");
			this.UriLink = new Uri(uriLink);
			this.FilePattern = filePattern;
			this.IgnoreInvalidFileName = ignoreInvalidFileName;
		}

		public Uri UriLink { get; private set; }

		public string FilePattern { get; private set; }

		public bool IgnoreInvalidFileName { get; private set; }

		public long FileSize { get; set; }

		public string FilePath { get; set; }

		public static bool IsFileNameValid(string filePath, string filePattern)
		{
			if (string.IsNullOrEmpty(filePath))
			{
				return false;
			}
			string fileName = Path.GetFileName(filePath);
			return !string.IsNullOrEmpty(fileName) && (string.IsNullOrEmpty(filePattern) || Regex.IsMatch(fileName.ToLower(), filePattern));
		}

		public bool IsFileNameValid()
		{
			return DownloadFileInfo.IsFileNameValid(this.FilePath, this.FilePattern);
		}
	}
}
