using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Management.Automation;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Audio;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("Import", "RecipientDataProperty", DefaultParameterSetName = "ImportPicture", SupportsShouldProcess = true)]
	public sealed class ImportRecipientDataProperty : RecipientObjectActionTask<MailboxUserContactIdParameter, ADRecipient>
	{
		[Parameter(Mandatory = true, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true, Position = 0)]
		public override MailboxUserContactIdParameter Identity
		{
			get
			{
				return (MailboxUserContactIdParameter)base.Fields["Identity"];
			}
			set
			{
				base.Fields["Identity"] = value;
			}
		}

		[Parameter(Mandatory = true)]
		public byte[] FileData
		{
			get
			{
				return (byte[])base.Fields["FileData"];
			}
			set
			{
				base.Fields["FileData"] = value;
			}
		}

		[Parameter(ParameterSetName = "ImportPicture")]
		public SwitchParameter Picture
		{
			get
			{
				return (SwitchParameter)(base.Fields["Picture"] ?? false);
			}
			set
			{
				base.Fields["Picture"] = value;
			}
		}

		[Parameter(ParameterSetName = "ImportSpokenName")]
		public SwitchParameter SpokenName
		{
			get
			{
				return (SwitchParameter)(base.Fields["SpokenName"] ?? false);
			}
			set
			{
				base.Fields["SpokenName"] = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageImportRecipientDataProperty(this.Identity.ToString());
			}
		}

		protected override void InternalValidate()
		{
			if (this.Picture.IsPresent)
			{
				this.ValidateJPEGFile(this.FileData);
			}
			else if (this.SpokenName.IsPresent)
			{
				this.ValidateWMAFile(this.FileData);
			}
			else
			{
				base.WriteError(new LocalizedException(Strings.ErrorUseDataPropertyNameParameter), ErrorCategory.InvalidData, null);
			}
			base.InternalValidate();
		}

		protected override IConfigurable PrepareDataObject()
		{
			TaskLogger.LogEnter();
			ADRecipient adrecipient = (ADRecipient)base.PrepareDataObject();
			if (this.Picture.IsPresent)
			{
				adrecipient.ThumbnailPhoto = this.data;
			}
			else if (this.SpokenName.IsPresent)
			{
				adrecipient.UMSpokenName = this.data;
			}
			TaskLogger.LogExit();
			return adrecipient;
		}

		private void ValidateJPEGFile(byte[] jpegFileData)
		{
			try
			{
				using (Stream stream = new MemoryStream(jpegFileData))
				{
					if (stream.Length > (long)ImportRecipientDataProperty.MaxJpegSize.ToBytes())
					{
						base.WriteError(new ArgumentException(Strings.ErrorJPEGFileTooBig), ErrorCategory.InvalidData, null);
					}
					Image image = Image.FromStream(stream);
					if (image.RawFormat.Guid != ImageFormat.Jpeg.Guid)
					{
						throw new ArgumentException();
					}
					this.data = jpegFileData;
				}
			}
			catch (ArgumentException)
			{
				base.WriteError(new FormatException(Strings.ErrorInvalidJPEGFormat), ErrorCategory.InvalidData, null);
			}
		}

		private void ValidateWMAFile(byte[] wmaFileData)
		{
			bool flag = true;
			Exception innerException = null;
			string text = null;
			string text2 = null;
			string text3 = null;
			try
			{
				text = Path.GetTempFileName();
				text2 = Path.GetTempFileName();
				text3 = Path.GetTempFileName();
				using (FileStream fileStream = new FileStream(text3, FileMode.Create, FileAccess.ReadWrite, FileShare.None))
				{
					BinaryWriter binaryWriter = new BinaryWriter(fileStream);
					binaryWriter.Write(wmaFileData);
					fileStream.Flush();
				}
				using (WmaReader wmaReader = new WmaReader(text3))
				{
					using (PcmWriter pcmWriter = new PcmWriter(text, wmaReader.Format))
					{
						byte[] array = new byte[wmaReader.SampleSize * 2];
						int count;
						while ((count = wmaReader.Read(array, array.Length)) > 0)
						{
							pcmWriter.Write(array, count);
						}
					}
				}
				using (PcmReader pcmReader = new PcmReader(text))
				{
					using (WmaWriter wmaWriter = new Wma8Writer(text2, pcmReader.WaveFormat))
					{
						byte[] array2 = new byte[wmaWriter.BufferSize];
						double num = 0.0;
						int num2;
						while ((num2 = pcmReader.Read(array2, array2.Length)) > 0)
						{
							AudioNormalizer.ProcessBuffer(array2, num2, 9E-05, 0.088, ref num);
							wmaWriter.Write(array2, num2);
						}
					}
				}
				using (FileStream fileStream2 = new FileStream(text2, FileMode.Open, FileAccess.Read, FileShare.Read))
				{
					if (fileStream2.Length > (long)ImportRecipientDataProperty.MaxWmaSize.ToBytes())
					{
						base.WriteError(new ArgumentException(Strings.ErrorWMAFileTooBig), ErrorCategory.InvalidData, null);
					}
					BinaryReader binaryReader = new BinaryReader(fileStream2);
					this.data = binaryReader.ReadBytes((int)binaryReader.BaseStream.Length);
				}
			}
			catch (InvalidWmaFormatException ex)
			{
				flag = false;
				innerException = ex;
			}
			catch (UnsupportedAudioFormat unsupportedAudioFormat)
			{
				flag = false;
				innerException = unsupportedAudioFormat;
			}
			catch (COMException ex2)
			{
				flag = false;
				innerException = ex2;
			}
			catch (IOException exception)
			{
				base.WriteError(exception, ErrorCategory.InvalidOperation, null);
			}
			finally
			{
				if (File.Exists(text))
				{
					File.Delete(text);
				}
				if (File.Exists(text2))
				{
					File.Delete(text2);
				}
				if (File.Exists(text3))
				{
					File.Delete(text3);
				}
				if (!flag)
				{
					base.WriteError(new FormatException(Strings.ErrorInvalidWMAFormat, innerException), ErrorCategory.InvalidData, null);
				}
			}
		}

		private byte[] data;

		internal static readonly ByteQuantifiedSize MaxJpegSize = ByteQuantifiedSize.FromKB(10UL);

		internal static readonly ByteQuantifiedSize MaxWmaSize = ByteQuantifiedSize.FromKB(32UL);
	}
}
