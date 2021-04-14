using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using Microsoft.Exchange.Data.ApplicationLogic.UserPhotos;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.GroupMailbox.Common;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.GroupMailbox;
using Microsoft.Exchange.Diagnostics.Performance;

namespace Microsoft.Exchange.Data.GroupMailbox
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class GroupMailboxDefaultPhotoUploader
	{
		public GroupMailboxDefaultPhotoUploader(IRecipientSession adSession, IMailboxSession mailboxSession, ADUser group)
		{
			this.adSession = adSession;
			this.mailboxSession = mailboxSession;
			this.group = group;
		}

		public static bool IsFlightEnabled(MailboxSession mailboxSession)
		{
			return mailboxSession.MailboxOwner.GetConfiguration().MailboxAssistants.GenerateGroupPhoto.Enabled;
		}

		public static void ClearGroupPhoto(IRecipientSession adSession, IMailboxSession mailboxSession, ADUser group)
		{
			PhotoUploadPipeline photoUploadPipeline = new PhotoUploadPipeline(new PhotosConfiguration(ExchangeSetupContext.InstallPath), mailboxSession, adSession, GroupMailboxDefaultPhotoUploader.Tracer);
			PhotoRequest request = new PhotoRequest
			{
				TargetPrimarySmtpAddress = group.PrimarySmtpAddress.ToString(),
				UploadTo = group.ObjectId,
				Preview = true,
				UploadCommand = UploadCommand.Clear
			};
			photoUploadPipeline.Upload(request, Stream.Null);
			request = new PhotoRequest
			{
				TargetPrimarySmtpAddress = group.PrimarySmtpAddress.ToString(),
				UploadTo = group.ObjectId,
				Preview = false,
				UploadCommand = UploadCommand.Clear
			};
			photoUploadPipeline.Upload(request, Stream.Null);
		}

		public byte[] Upload()
		{
			return this.UploadAndUpdateVersion(1);
		}

		public void UploadIfOutdated()
		{
			int valueOrDefault = this.mailboxSession.Mailbox.GetValueOrDefault<int>(MailboxSchema.GroupMailboxGeneratedPhotoVersion, -1);
			byte[] array = this.ReadGroupMailboxPhoto();
			bool flag = false;
			if (valueOrDefault == -1 && array.Length == 0)
			{
				flag = true;
			}
			else if (valueOrDefault < 1 && array.Length > 0)
			{
				byte[] valueOrDefault2 = this.mailboxSession.Mailbox.GetValueOrDefault<byte[]>(MailboxSchema.GroupMailboxGeneratedPhotoSignature, null);
				byte[] photoSignature = GroupMailboxDefaultPhotoUploader.GetPhotoSignature(array);
				if (valueOrDefault2 != null && photoSignature.SequenceEqual(valueOrDefault2))
				{
					flag = true;
				}
			}
			if (flag)
			{
				this.UploadAndUpdateVersion(1);
				return;
			}
			this.mailboxSession.Mailbox[MailboxSchema.GroupMailboxGeneratedPhotoVersion] = int.MaxValue;
			this.mailboxSession.Mailbox.Save();
			this.mailboxSession.Mailbox.Load();
		}

		private static byte[] GetPhotoSignature(byte[] photo)
		{
			byte[] result;
			using (SHA256Cng sha256Cng = new SHA256Cng())
			{
				sha256Cng.Initialize();
				result = sha256Cng.ComputeHash(photo);
			}
			return result;
		}

		private byte[] UploadAndUpdateVersion(int version)
		{
			this.InternalUploadPhoto();
			byte[] array = this.ReadGroupMailboxPhoto();
			this.mailboxSession.Mailbox[MailboxSchema.GroupMailboxGeneratedPhotoSignature] = GroupMailboxDefaultPhotoUploader.GetPhotoSignature(array);
			this.mailboxSession.Mailbox[MailboxSchema.GroupMailboxGeneratedPhotoVersion] = version;
			return array;
		}

		private void InternalUploadPhoto()
		{
			PhotoUploadPipeline photoUploadPipeline = new PhotoUploadPipeline(new PhotosConfiguration(ExchangeSetupContext.InstallPath), this.mailboxSession, this.adSession, GroupMailboxDefaultPhotoUploader.Tracer);
			string text = this.group.DisplayName;
			if (string.IsNullOrWhiteSpace(text))
			{
				text = this.group.Name;
			}
			using (Stream stream = InitialsImageGenerator.GenerateAsStream(text, 1024))
			{
				PhotoRequest request = new PhotoRequest
				{
					TargetPrimarySmtpAddress = this.group.PrimarySmtpAddress.ToString(),
					UploadTo = this.group.ObjectId,
					Preview = true,
					RawUploadedPhoto = stream,
					UploadCommand = UploadCommand.Upload
				};
				photoUploadPipeline.Upload(request, Stream.Null);
				request = new PhotoRequest
				{
					TargetPrimarySmtpAddress = this.group.PrimarySmtpAddress.ToString(),
					UploadTo = this.group.ObjectId,
					Preview = false,
					UploadCommand = UploadCommand.Upload
				};
				photoUploadPipeline.Upload(request, Stream.Null);
			}
		}

		private byte[] ReadGroupMailboxPhoto()
		{
			MailboxPhotoReader mailboxPhotoReader = new MailboxPhotoReader(GroupMailboxDefaultPhotoUploader.Tracer);
			byte[] result;
			using (MemoryStream memoryStream = new MemoryStream(8192))
			{
				try
				{
					mailboxPhotoReader.Read(this.mailboxSession, UserPhotoSize.HR64x64, false, memoryStream, NullPerformanceDataLogger.Instance);
					result = memoryStream.ToArray();
				}
				catch (ObjectNotFoundException)
				{
					result = new byte[0];
				}
			}
			return result;
		}

		public const int PhotoVersion = 1;

		private const int GeneratedPhotoHeightWidth = 1024;

		private const int PhotoReadMemoryBufferSize = 8192;

		private static readonly Trace Tracer = ExTraceGlobals.GroupMailboxAccessLayerTracer;

		private readonly IRecipientSession adSession;

		private readonly IMailboxSession mailboxSession;

		private ADUser group;
	}
}
