using System;

namespace Microsoft.Exchange.Hygiene.Cache.Data
{
	internal class CacheFileName
	{
		internal static string CookieFileNameFormat
		{
			get
			{
				return "Cookie_{0}_{1}.bin";
			}
		}

		internal static string ContentFileNameFormat
		{
			get
			{
				return "Content_{0}_{1}.bin";
			}
		}

		internal static string FileNameFormatWithTSVersion
		{
			get
			{
				return "{0}_{1}_{2}_{3}.bin";
			}
		}

		internal static string CacheFileFolderNameFormat
		{
			get
			{
				return "{0}_{1}";
			}
		}

		internal static string CacheZipFileNameFormat
		{
			get
			{
				return "{0}_{1}_{2}_{3}_{4}.zip";
			}
		}

		internal static string FileNameDateTimeFormat
		{
			get
			{
				return "yyyy-MM-dd-HH-mm-ss";
			}
		}

		internal static string CookieFileNameSearchFormat
		{
			get
			{
				return "Cookie_{0}_{1}*.bin";
			}
		}

		internal static string ContentFileNameSearchFormat
		{
			get
			{
				return "Content_{0}_{1}*.bin";
			}
		}

		internal static string CacheZipFileNameSearchFormat
		{
			get
			{
				return "{0}_{1}*.zip";
			}
		}

		internal static string CookieFileNamePrefix
		{
			get
			{
				return "Cookie_";
			}
		}

		internal static string ContentFileNamePrefix
		{
			get
			{
				return "Content_";
			}
		}

		internal static string BaselineBloomFileFormat
		{
			get
			{
				return "{0}.bff";
			}
		}

		internal static string BaselineBloomFileTempZipFormat
		{
			get
			{
				return "{0}-{1}.zip.tmp";
			}
		}

		internal static string BloomFilterZipFileNameSearchFormat
		{
			get
			{
				return "{0}*.zip";
			}
		}

		internal static string ZipFileFolder
		{
			get
			{
				return "ZipFile";
			}
		}

		internal static string CurrentFileFolder1
		{
			get
			{
				return "Current1";
			}
		}

		internal static string CurrentFileFolder2
		{
			get
			{
				return "Current2";
			}
		}

		internal static string WorkInProgress
		{
			get
			{
				return "WorkInProgress";
			}
		}

		internal static string WatermarkFileName
		{
			get
			{
				return "Watermark.txt";
			}
		}

		internal static string GetBaselineBloomFilterFileName(string entityName)
		{
			return string.Format(CacheFileName.BaselineBloomFileFormat, entityName);
		}
	}
}
