using System;
using System.IO;
using System.IO.Compression;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal class ApnsFeedbackFileIO
	{
		public virtual bool Exists(string path)
		{
			return Directory.Exists(path);
		}

		public virtual void ExtractFileToDirectory(string sourceArchiveFileName, string destinationDirectoryName)
		{
			ZipFile.ExtractToDirectory(sourceArchiveFileName, destinationDirectoryName);
		}

		public virtual string[] GetFiles(string path, string searchPattern, SearchOption searchOption)
		{
			return Directory.GetFiles(path, searchPattern, searchOption);
		}

		public virtual StreamReader GetFileReader(string path)
		{
			FileStream stream = new FileStream(path, FileMode.Open);
			return new StreamReader(stream);
		}

		public virtual void DeleteFile(string path)
		{
			File.Delete(path);
		}

		public virtual void DeleteFolder(string path)
		{
			Directory.Delete(path, true);
		}

		public static readonly ApnsFeedbackFileIO DefaultFileIO = new ApnsFeedbackFileIO();
	}
}
