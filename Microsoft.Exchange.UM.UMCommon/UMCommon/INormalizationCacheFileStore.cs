using System;
using System.Globalization;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.UM.UMCommon
{
	internal interface INormalizationCacheFileStore
	{
		bool UploadCache(string filePath, string fileNamePrefix, CultureInfo culture, string version, MailboxSession mbxSession);

		bool DownloadCache(string destinationFilePath, string fileNamePrefix, CultureInfo culture, string version);
	}
}
