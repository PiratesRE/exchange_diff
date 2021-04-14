using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.AnchorService.Storage
{
	internal static class AnchorMessageHelper
	{
		internal static AnchorAttachment CreateAttachment(AnchorContext context, MessageItem message, string name)
		{
			AnchorUtil.ThrowOnNullArgument(message, "message");
			AnchorUtil.ThrowOnNullOrEmptyArgument(name, "name");
			AttachmentType type = AttachmentType.Stream;
			AttachmentId attachmentId = null;
			foreach (AttachmentHandle handle in message.AttachmentCollection)
			{
				using (StreamAttachment streamAttachment = (StreamAttachment)message.AttachmentCollection.Open(handle, type))
				{
					if (streamAttachment.FileName.Equals(name, StringComparison.OrdinalIgnoreCase))
					{
						attachmentId = streamAttachment.Id;
						break;
					}
				}
			}
			context.Logger.Log(MigrationEventType.Information, "Creating a new attachment with name {0}", new object[]
			{
				name
			});
			StreamAttachment streamAttachment2 = (StreamAttachment)message.AttachmentCollection.Create(type);
			streamAttachment2.FileName = name;
			if (attachmentId != null)
			{
				context.Logger.Log(MigrationEventType.Information, "Found an existing attachment with name {0} and id {1}, removing old one", new object[]
				{
					name,
					attachmentId.ToBase64String()
				});
				message.AttachmentCollection.Remove(attachmentId);
			}
			return new AnchorAttachment(context, streamAttachment2, PropertyOpenMode.Create);
		}

		internal static AnchorAttachment GetAttachment(AnchorContext context, MessageItem message, string name, PropertyOpenMode openMode)
		{
			AnchorUtil.ThrowOnNullArgument(message, "message");
			AnchorUtil.ThrowOnNullOrEmptyArgument(name, "name");
			if (openMode != PropertyOpenMode.ReadOnly && openMode != PropertyOpenMode.Modify)
			{
				throw new ArgumentException("Invalid OpenMode for GetAttachment", "openMode");
			}
			AnchorAttachment anchorAttachment = null;
			foreach (AttachmentHandle handle in message.AttachmentCollection)
			{
				StreamAttachment streamAttachment = null;
				try
				{
					AttachmentType type = AttachmentType.Stream;
					streamAttachment = (StreamAttachment)message.AttachmentCollection.Open(handle, type);
					if (streamAttachment.FileName.Equals(name, StringComparison.OrdinalIgnoreCase))
					{
						anchorAttachment = new AnchorAttachment(context, streamAttachment, openMode);
						streamAttachment = null;
						break;
					}
				}
				finally
				{
					if (streamAttachment != null)
					{
						streamAttachment.Dispose();
						streamAttachment = null;
					}
				}
			}
			if (anchorAttachment == null)
			{
				throw new MigrationAttachmentNotFoundException(name);
			}
			return anchorAttachment;
		}

		internal static void DeleteAttachment(MessageItem message, string name)
		{
			AnchorUtil.ThrowOnNullArgument(message, "message");
			AnchorUtil.ThrowOnNullOrEmptyArgument(name, "name");
			List<AttachmentHandle> list = new List<AttachmentHandle>(message.AttachmentCollection.Count);
			foreach (AttachmentHandle attachmentHandle in message.AttachmentCollection)
			{
				AttachmentType type = AttachmentType.Stream;
				using (StreamAttachment streamAttachment = (StreamAttachment)message.AttachmentCollection.Open(attachmentHandle, type))
				{
					if (streamAttachment.FileName.Equals(name, StringComparison.OrdinalIgnoreCase))
					{
						list.Add(attachmentHandle);
					}
				}
			}
			foreach (AttachmentHandle handle in list)
			{
				message.AttachmentCollection.Remove(handle);
			}
		}
	}
}
