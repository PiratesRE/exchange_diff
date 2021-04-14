using System;
using System.IO;
using Microsoft.Exchange.LogUploaderProxy;

namespace Microsoft.Exchange.LogUploader
{
	internal class WatermarkFileHelper : IWatermarkFileHelper
	{
		public WatermarkFileHelper(string logDirectory, string wmkDirectory = null)
		{
			if (string.IsNullOrEmpty(logDirectory))
			{
				throw new ArgumentException(string.Format("{0} is not a valid directory name", logDirectory));
			}
			this.logfileDirectory = logDirectory.TrimEnd(new char[]
			{
				'\\'
			});
			if (string.IsNullOrEmpty(wmkDirectory))
			{
				this.watermarkFileDirectory = this.logfileDirectory;
				return;
			}
			this.watermarkFileDirectory = wmkDirectory.TrimEnd(new char[]
			{
				'\\'
			});
			if (!Directory.Exists(this.watermarkFileDirectory))
			{
				Directory.CreateDirectory(this.watermarkFileDirectory);
			}
		}

		public string WatermarkFileDirectory
		{
			get
			{
				return this.watermarkFileDirectory;
			}
		}

		public string LogFileDirectory
		{
			get
			{
				return this.logfileDirectory;
			}
		}

		public string DeduceLogFullFileNameFromDoneOrWatermarkFileName(string fileFullName)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("fileFullName", fileFullName);
			string text = Path.GetFileName(fileFullName);
			ArgumentValidator.ThrowIfNullOrEmpty("fileName", text);
			text = Path.ChangeExtension(text, "log");
			return Path.Combine(this.logfileDirectory, text);
		}

		public string DeduceDoneFileFullNameFromLogFileName(string logFileName)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("logFileName", logFileName);
			logFileName = Path.GetFileName(logFileName);
			ArgumentValidator.ThrowIfNullOrEmpty("logFileName", logFileName);
			return Path.Combine(this.watermarkFileDirectory, Path.ChangeExtension(logFileName, "done"));
		}

		public string DeduceWatermarkFileFullNameFromLogFileName(string logFileName)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("logFileName", logFileName);
			logFileName = Path.GetFileName(logFileName);
			ArgumentValidator.ThrowIfNullOrEmpty("logFileName", logFileName);
			return Path.Combine(this.watermarkFileDirectory, Path.ChangeExtension(logFileName, "wmk"));
		}

		public IWatermarkFile CreateWaterMarkFileObj(string logFileName, string logPrefix)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("logFileName", logFileName);
			ArgumentValidator.ThrowIfNullOrEmpty("logPrefix", logPrefix);
			string fileName = Path.GetFileName(logFileName);
			ArgumentValidator.ThrowIfNullOrEmpty("fileName", fileName);
			string logFileName2 = Path.Combine(this.logfileDirectory, fileName);
			return new WatermarkFile(logFileName2, this.DeduceWatermarkFileFullNameFromLogFileName(fileName), logPrefix);
		}

		private readonly string watermarkFileDirectory;

		private readonly string logfileDirectory;
	}
}
