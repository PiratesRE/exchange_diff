using System;

namespace Microsoft.Exchange.LogUploader
{
	internal interface IWatermarkFileHelper
	{
		string WatermarkFileDirectory { get; }

		string LogFileDirectory { get; }

		IWatermarkFile CreateWaterMarkFileObj(string logFileName, string logPrefix);

		string DeduceDoneFileFullNameFromLogFileName(string logFileName);

		string DeduceWatermarkFileFullNameFromLogFileName(string logFileName);

		string DeduceLogFullFileNameFromDoneOrWatermarkFileName(string fileFullName);
	}
}
