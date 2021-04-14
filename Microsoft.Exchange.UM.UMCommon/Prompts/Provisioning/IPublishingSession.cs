using System;
using System.IO;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.Prompts.Provisioning
{
	internal interface IPublishingSession : IDisposeTrackable, IDisposable
	{
		TimeSpan TestHookKeepOrphanFilesInterval { get; set; }

		void Upload(string source, string destinationName);

		void Download(string sourceName, string destination);

		ITempWavFile DownloadAsWav(string sourceName);

		void DownloadAllAsWma(DirectoryInfo directory);

		void Delete();
	}
}
