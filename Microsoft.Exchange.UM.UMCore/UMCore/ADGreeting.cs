using System;
using System.IO;
using Microsoft.Exchange.Audio;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class ADGreeting : GreetingBase
	{
		internal ADGreeting(ADRecipient aduser, string name)
		{
			this.aduser = aduser;
			this.name = name;
		}

		internal override string Name
		{
			get
			{
				return "Um.CustomGreetings." + this.name;
			}
		}

		internal override void Put(string sourceFileName)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.XsoTracer, this, "ADGreeting::Put().", new object[0]);
			using (ITempFile tempFile = TempFileFactory.CreateTempWmaFile())
			{
				using (PcmReader pcmReader = new PcmReader(sourceFileName))
				{
					using (WmaWriter wmaWriter = new Wma8Writer(tempFile.FilePath, pcmReader.WaveFormat))
					{
						MediaMethods.ConvertWavToWma(pcmReader, wmaWriter);
					}
				}
				FileInfo fileInfo = new FileInfo(tempFile.FilePath);
				int num = Convert.ToInt32(fileInfo.Length);
				if (num > 32768)
				{
					throw new MaxGreetingLengthExceededException(num);
				}
				byte[] array = new byte[num];
				using (FileStream fileStream = new FileStream(tempFile.FilePath, FileMode.Open, FileAccess.Read))
				{
					int num2 = 0;
					int num3;
					do
					{
						num3 = fileStream.Read(array, num2, array.Length - num2);
						num2 += num3;
					}
					while (0 < num3);
				}
				this.aduser.UMSpokenName = array;
				this.aduser.Session.Save(this.aduser);
			}
		}

		internal override void Delete()
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.XsoTracer, this, "ADGreeting::Delete().", new object[0]);
			this.aduser.UMSpokenName = null;
			this.aduser.Session.Save(this.aduser);
		}

		internal override ITempWavFile Get()
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.XsoTracer, this, "ADGreeting::Get().", new object[0]);
			if (!this.Exists())
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.XsoTracer, this, "ADGreeting::Get No Spoken Name.", new object[0]);
				return null;
			}
			byte[] umspokenName = this.aduser.UMSpokenName;
			ITempFile tempFile = TempFileFactory.CreateTempWmaFile();
			ITempWavFile tempWavFile = TempFileFactory.CreateTempWavFile();
			MemoryStream memoryStream = null;
			FileStream fileStream = null;
			ITempWavFile result;
			try
			{
				memoryStream = new MemoryStream(umspokenName);
				fileStream = new FileStream(tempFile.FilePath, FileMode.OpenOrCreate, FileAccess.Write);
				CommonUtil.CopyStream(memoryStream, fileStream);
				fileStream.Close();
				MediaMethods.ConvertWmaToWav(tempFile.FilePath, tempWavFile.FilePath);
				result = tempWavFile;
			}
			catch (WmaToWavConversionException ex)
			{
				CallIdTracer.TraceError(ExTraceGlobals.XsoTracer, this, "ADGreeting::Get WmaToWav conversion failed with e={0}.", new object[]
				{
					ex
				});
				result = null;
			}
			finally
			{
				if (memoryStream != null)
				{
					memoryStream.Close();
				}
				if (fileStream != null)
				{
					fileStream.Close();
				}
			}
			return result;
		}

		internal override bool Exists()
		{
			return this.aduser.UMSpokenName != null && 0 < this.aduser.UMSpokenName.Length;
		}

		protected override void InternalDispose(bool disposing)
		{
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<ADGreeting>(this);
		}

		private ADRecipient aduser;

		private string name;
	}
}
