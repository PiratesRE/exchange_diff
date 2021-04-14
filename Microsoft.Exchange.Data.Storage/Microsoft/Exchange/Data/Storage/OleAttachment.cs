using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class OleAttachment : StreamAttachmentBase
	{
		internal OleAttachment(CoreAttachment coreAttachment) : base(coreAttachment)
		{
		}

		public override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<OleAttachment>(this);
		}

		public bool TryConvertToImage(Stream outStream, ImageFormat format)
		{
			bool result;
			try
			{
				this.ConvertToImage(outStream, format);
				result = true;
			}
			catch (OleConversionFailedException)
			{
				result = false;
			}
			return result;
		}

		public Stream TryConvertToImage(ImageFormat format)
		{
			Stream result;
			try
			{
				result = this.ConvertToImage(format);
			}
			catch (OleConversionFailedException)
			{
				result = null;
			}
			return result;
		}

		public Stream ConvertToImage(ImageFormat format)
		{
			MemoryStream memoryStream = new MemoryStream();
			bool flag = true;
			Stream result;
			try
			{
				this.ConvertToImage(memoryStream, format);
				flag = false;
				memoryStream.Position = 0L;
				result = memoryStream;
			}
			finally
			{
				if (flag)
				{
					memoryStream.Dispose();
				}
			}
			return result;
		}

		internal override Attachment CreateCopy(AttachmentCollection collection, BodyFormat? targetBodyFormat)
		{
			bool flag = targetBodyFormat != null && targetBodyFormat != BodyFormat.ApplicationRtf;
			if (flag)
			{
				return this.ConvertToImageAttachment(collection.CoreAttachmentCollection, ImageFormat.Jpeg);
			}
			return (Attachment)Attachment.CreateCopy(this, collection, new AttachmentType?(AttachmentType.Ole));
		}

		public StreamAttachment ConvertToImageAttachment(CoreAttachmentCollection collection, ImageFormat format)
		{
			base.CheckDisposed("ConvertToImageAttachment");
			Util.ThrowOnNullArgument(collection, "collection");
			EnumValidator.ThrowIfInvalid<ImageFormat>(format);
			StreamAttachment result;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				CoreAttachment coreAttachment = collection.InternalCreateCopy(new AttachmentType?(AttachmentType.Stream), base.CoreAttachment);
				disposeGuard.Add<CoreAttachment>(coreAttachment);
				StreamAttachment streamAttachment = (StreamAttachment)AttachmentCollection.CreateTypedAttachment(coreAttachment, new AttachmentType?(AttachmentType.Stream));
				disposeGuard.Add<StreamAttachment>(streamAttachment);
				string text = streamAttachment.FileName;
				if (string.IsNullOrEmpty(text))
				{
					text = Attachment.GenerateFilename();
				}
				string str = null;
				switch (format)
				{
				case ImageFormat.Jpeg:
					str = ".jpg";
					break;
				case ImageFormat.Png:
					str = ".png";
					break;
				}
				streamAttachment.FileName = text + str;
				streamAttachment.ContentType = "image/jpeg";
				streamAttachment.IsInline = true;
				using (Stream contentStream = streamAttachment.GetContentStream(PropertyOpenMode.Create))
				{
					if (!this.TryConvertToImage(contentStream, ImageFormat.Jpeg))
					{
						ConvertUtils.SaveDefaultImage(contentStream);
					}
				}
				disposeGuard.Success();
				result = streamAttachment;
			}
			return result;
		}

		protected override void OnBeforeSave()
		{
			base.OnBeforeSave();
			base.PropertyBag[InternalSchema.AttachMethod] = 6;
		}

		protected override PropertyTagPropertyDefinition ContentStreamProperty
		{
			get
			{
				return InternalSchema.AttachDataObj;
			}
		}

		public override AttachmentType AttachmentType
		{
			get
			{
				base.CheckDisposed("AttachmentType::get");
				return AttachmentType.Ole;
			}
		}

		public void ConvertToImage(Stream outStream, ImageFormat format)
		{
			EnumValidator.ThrowIfInvalid<ImageFormat>(format, "format");
			using (Stream stream = this.ConvertToBitmap())
			{
				try
				{
					using (Image image = Image.FromStream(stream))
					{
						try
						{
							switch (format)
							{
							case ImageFormat.Jpeg:
								image.Save(outStream, ImageFormat.Jpeg);
								break;
							case ImageFormat.Png:
								image.Save(outStream, ImageFormat.Png);
								break;
							}
						}
						catch (ExternalException innerException)
						{
							StorageGlobals.ContextTraceError(ExTraceGlobals.CcOleTracer, "OleAttachment::ConvertToImage: result stream is corrupt.");
							throw new OleConversionFailedException(ServerStrings.OleConversionFailed, innerException);
						}
					}
				}
				catch (ArgumentException innerException2)
				{
					StorageGlobals.ContextTraceError(ExTraceGlobals.CcOleTracer, "OleAttachment::ConvertToImage: result stream is corrupt.");
					throw new OleConversionFailedException(ServerStrings.OleConversionFailed, innerException2);
				}
			}
		}

		private Stream ConvertToBitmap()
		{
			Stream stream = null;
			using (StorageGlobals.SetTraceContext(this))
			{
				if (StandaloneFuzzing.IsEnabled)
				{
					using (Bitmap bitmap = new Bitmap(1, 1))
					{
						stream = new MemoryStream();
						bitmap.Save(stream, ImageFormat.Jpeg);
						goto IL_5C;
					}
				}
				OleConverter instance = OleConverter.Instance;
				using (Stream contentStream = this.GetContentStream(PropertyOpenMode.ReadOnly))
				{
					stream = instance.ConvertToBitmap(contentStream);
				}
				IL_5C:;
			}
			return stream;
		}

		internal const string JpegExtension = ".jpg";

		internal const string PngExtension = ".png";

		internal const int AttachMethod = 6;
	}
}
