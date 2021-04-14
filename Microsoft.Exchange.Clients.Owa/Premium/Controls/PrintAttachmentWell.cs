using System;
using System.Collections;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Core.Controls;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	public sealed class PrintAttachmentWell
	{
		public static void RenderAttachmentWell(TextWriter output, ArrayList attachmentList, UserContext userContext)
		{
			if (output == null)
			{
				throw new ArgumentNullException("output");
			}
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			if (attachmentList == null)
			{
				return;
			}
			output.Write("<div class=\"awRO\">");
			PrintAttachmentWell.RenderAttachments(output, attachmentList, userContext);
			output.Write("</div>");
		}

		private static void RenderAttachments(TextWriter output, ArrayList attachmentList, UserContext userContext)
		{
			if (attachmentList == null)
			{
				return;
			}
			int count = attachmentList.Count;
			if (count <= 0)
			{
				return;
			}
			int num = 0;
			ArrayList previousAttachmentDisplayNames = new ArrayList();
			for (int i = 0; i < count; i++)
			{
				AttachmentWellInfo attachmentInfoObject = (AttachmentWellInfo)attachmentList[i];
				PrintAttachmentWell.RenderAttachment(output, attachmentInfoObject, userContext, previousAttachmentDisplayNames);
				if (num < count - 1)
				{
					output.Write(" ; ");
				}
				num++;
			}
		}

		private static void RenderAttachment(TextWriter output, AttachmentWellInfo attachmentInfoObject, UserContext userContext, ArrayList previousAttachmentDisplayNames)
		{
			ItemAttachment itemAttachment = null;
			Item item = null;
			string value = null;
			try
			{
				if (attachmentInfoObject.AttachmentType == AttachmentType.EmbeddedMessage)
				{
					itemAttachment = (ItemAttachment)attachmentInfoObject.OpenAttachment();
					item = itemAttachment.GetItemAsReadOnly(new PropertyDefinition[]
					{
						TaskSchema.TaskType
					});
				}
				if (item != null)
				{
					value = Utilities.HtmlEncode(AttachmentUtility.TrimAttachmentDisplayName(AttachmentUtility.GetEmbeddedAttachmentDisplayName(item), previousAttachmentDisplayNames, true));
				}
				else
				{
					value = Utilities.HtmlEncode(AttachmentUtility.TrimAttachmentDisplayName(attachmentInfoObject.AttachmentName, previousAttachmentDisplayNames, false));
				}
			}
			catch (ObjectNotFoundException)
			{
				return;
			}
			finally
			{
				if (item != null)
				{
					item.Dispose();
					item = null;
				}
				if (itemAttachment != null)
				{
					itemAttachment.Dispose();
					itemAttachment = null;
				}
			}
			output.Write("<span id=\"spnAtmt\" tabindex=\"-1\">");
			output.Write(value);
			long size = attachmentInfoObject.Size;
			if (size > 0L)
			{
				output.Write(userContext.DirectionMark);
				output.Write(" ");
				output.Write(LocalizedStrings.GetHtmlEncoded(6409762));
				Utilities.RenderSizeWithUnits(output, size, true);
				output.Write(userContext.DirectionMark);
				output.Write(LocalizedStrings.GetHtmlEncoded(-1023695022));
			}
			output.Write("</span>");
		}

		public static bool ShouldRenderAttachments(ArrayList attachmentList)
		{
			return attachmentList != null && attachmentList.Count > 0;
		}
	}
}
