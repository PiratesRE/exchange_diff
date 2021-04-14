using System;
using System.IO;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal static class SharingMessageAttachment
	{
		internal static void SetSharingMessage(MessageItem item, SharingMessage sharingMessage)
		{
			using (StreamAttachment orCreateSharingMessageAttachment = SharingMessageAttachment.GetOrCreateSharingMessageAttachment(item))
			{
				orCreateSharingMessageAttachment.PropertyBag[AttachmentSchema.AttachMimeTag] = "application/x-sharing-metadata-xml";
				orCreateSharingMessageAttachment.PropertyBag[AttachmentSchema.AttachLongFileName] = "sharing_metadata.xml";
				using (Stream contentStream = orCreateSharingMessageAttachment.GetContentStream(PropertyOpenMode.Create))
				{
					sharingMessage.SerializeToStream(contentStream);
					contentStream.Flush();
				}
				orCreateSharingMessageAttachment.Save();
			}
		}

		internal static SharingMessage GetSharingMessage(MessageItem item)
		{
			StreamAttachment sharingMessageAttachment = SharingMessageAttachment.GetSharingMessageAttachment(item);
			if (sharingMessageAttachment != null)
			{
				using (sharingMessageAttachment)
				{
					using (Stream stream = sharingMessageAttachment.TryGetContentStream(PropertyOpenMode.ReadOnly))
					{
						SharingMessage sharingMessage = null;
						try
						{
							sharingMessage = SharingMessage.DeserializeFromStream(stream);
						}
						catch (InvalidOperationException)
						{
						}
						if (sharingMessage != null)
						{
							ValidationResults validationResults = sharingMessage.Validate();
							if (validationResults.Result == ValidationResult.Success)
							{
								return sharingMessage;
							}
						}
					}
				}
			}
			return null;
		}

		private static StreamAttachment GetSharingMessageAttachment(MessageItem item)
		{
			bool flag = true;
			foreach (AttachmentHandle handle in item.AttachmentCollection)
			{
				Attachment attachment = item.AttachmentCollection.Open(handle, null);
				try
				{
					string valueOrDefault = attachment.GetValueOrDefault<string>(AttachmentSchema.AttachMimeTag, string.Empty);
					string valueOrDefault2 = attachment.GetValueOrDefault<string>(AttachmentSchema.AttachLongFileName, string.Empty);
					if (StringComparer.InvariantCultureIgnoreCase.Equals(valueOrDefault, "application/x-sharing-metadata-xml") && StringComparer.InvariantCultureIgnoreCase.Equals(valueOrDefault2, "sharing_metadata.xml") && attachment is StreamAttachment)
					{
						flag = false;
						return (StreamAttachment)attachment;
					}
				}
				finally
				{
					if (flag)
					{
						attachment.Dispose();
					}
				}
			}
			return null;
		}

		private static StreamAttachment GetOrCreateSharingMessageAttachment(MessageItem item)
		{
			StreamAttachment streamAttachment = SharingMessageAttachment.GetSharingMessageAttachment(item);
			if (streamAttachment == null)
			{
				streamAttachment = (StreamAttachment)item.AttachmentCollection.Create(AttachmentType.Stream);
			}
			return streamAttachment;
		}
	}
}
