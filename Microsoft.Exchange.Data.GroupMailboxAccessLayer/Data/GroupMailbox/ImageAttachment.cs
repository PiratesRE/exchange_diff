using System;
using System.IO;
using System.Reflection;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.GroupMailbox;

namespace Microsoft.Exchange.Data.GroupMailbox
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class ImageAttachment
	{
		public ImageAttachment(string imageName, string imageId, string contentType, byte[] bytes = null)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("imageName", imageName);
			ArgumentValidator.ThrowIfNullOrEmpty("imageId", imageId);
			ArgumentValidator.ThrowIfNullOrEmpty("contentType", contentType);
			this.ImageName = imageName;
			this.ImageId = imageId;
			this.contentType = contentType;
			this.bytes = (bytes ?? ImageAttachment.ReadImageFromEmbeddedResources(imageName));
		}

		public string ImageName { get; private set; }

		public string ImageId { get; private set; }

		public void AddImageAsAttachment(MessageItem message)
		{
			ImageAttachment.Tracer.TraceDebug<string, string>((long)this.GetHashCode(), "ImageAttachment.AddImageAsAttachment: Image Name: {0}. Image Id: {1}", this.ImageName, this.ImageId);
			using (StreamAttachment streamAttachment = message.AttachmentCollection.Create(AttachmentType.Stream) as StreamAttachment)
			{
				using (Stream contentStream = streamAttachment.GetContentStream())
				{
					streamAttachment.FileName = this.ImageName;
					streamAttachment.ContentId = this.ImageId;
					streamAttachment.ContentType = this.contentType;
					contentStream.Write(this.bytes, 0, this.bytes.Length);
					streamAttachment.Save();
				}
			}
		}

		private static byte[] ReadImageFromEmbeddedResources(string imageName)
		{
			ArgumentValidator.ThrowIfNullOrWhiteSpace("imageName", imageName);
			byte[] array = null;
			Assembly executingAssembly = Assembly.GetExecutingAssembly();
			using (Stream manifestResourceStream = executingAssembly.GetManifestResourceStream(imageName))
			{
				if (manifestResourceStream != null)
				{
					ImageAttachment.Tracer.TraceDebug<string>(0L, "Found image {0} as embedded resource.", imageName);
					array = new byte[manifestResourceStream.Length];
					int arg = manifestResourceStream.Read(array, 0, array.Length);
					ImageAttachment.Tracer.TraceDebug<int, string>(0L, "Read {0} bytes for image {1}.", arg, imageName);
				}
				else
				{
					ImageAttachment.Tracer.TraceError<string>(0L, "Couldn't find image {0} as embedded resource. Returning null.", imageName);
				}
			}
			return array;
		}

		private static readonly Trace Tracer = ExTraceGlobals.GroupEmailNotificationHandlerTracer;

		private readonly string contentType;

		private readonly byte[] bytes;
	}
}
