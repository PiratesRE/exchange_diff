using System;
using System.IO;

namespace Microsoft.Exchange.UM.UMCommon.CrossServerMailboxAccess
{
	internal class UMSubscriberCallAnsweringData
	{
		public ITempWavFile Greeting { get; set; }

		public byte[] RawGreeting
		{
			get
			{
				byte[] array = null;
				if (this.Greeting != null)
				{
					using (FileStream fileStream = new FileStream(this.Greeting.FilePath, FileMode.Open))
					{
						array = new byte[fileStream.Length];
						fileStream.Read(array, 0, array.Length);
					}
				}
				return array;
			}
		}

		public bool IsOOF { get; set; }

		public TranscriptionEnabledSetting IsTranscriptionEnabledInMailboxConfig { get; set; }

		public bool IsMailboxQuotaExceeded { get; set; }

		public bool TaskTimedOut { get; set; }

		public UMSubscriberCallAnsweringData()
		{
		}

		public UMSubscriberCallAnsweringData(byte[] greetingBytes, string greetingName, bool isOOF, TranscriptionEnabledSetting isTranscriptionEnabledInMailboxConfig, bool isMailboxQuotaExceeded, bool taskTimedOut) : this(UMSubscriberCallAnsweringData.CreateTemporaryGreetingFile(greetingBytes, greetingName), isOOF, isTranscriptionEnabledInMailboxConfig, isMailboxQuotaExceeded, taskTimedOut)
		{
		}

		public UMSubscriberCallAnsweringData(ITempWavFile greeting, bool isOOF, TranscriptionEnabledSetting isTranscriptionEnabledInMailboxConfig, bool isMailboxQuotaExceeded, bool taskTimedOut)
		{
			this.IsOOF = isOOF;
			this.IsTranscriptionEnabledInMailboxConfig = isTranscriptionEnabledInMailboxConfig;
			this.IsMailboxQuotaExceeded = isMailboxQuotaExceeded;
			this.TaskTimedOut = taskTimedOut;
			this.Greeting = greeting;
		}

		private static ITempWavFile CreateTemporaryGreetingFile(byte[] greetingBytes, string extraInfo)
		{
			ITempWavFile tempWavFile = null;
			if (greetingBytes != null && greetingBytes.Length > 0)
			{
				tempWavFile = TempFileFactory.CreateTempWavFile();
				using (FileStream fileStream = new FileStream(tempWavFile.FilePath, FileMode.Create))
				{
					fileStream.Write(greetingBytes, 0, greetingBytes.Length);
				}
				tempWavFile.ExtraInfo = extraInfo;
			}
			return tempWavFile;
		}
	}
}
