using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Exchange.Common.Sniff;
using Microsoft.Exchange.Data.Transport.Email;
using Microsoft.Exchange.Diagnostics.Components.MessagingPolicies;

namespace Microsoft.Exchange.MessagingPolicies.AttachFilter
{
	internal class AttachmentInfo
	{
		internal string Name
		{
			get
			{
				return this.name;
			}
		}

		internal List<string> ContentTypes
		{
			get
			{
				return this.contentTypes;
			}
		}

		internal Attachment Attachment
		{
			get
			{
				return this.attachment;
			}
		}

		internal static AttachmentInfo BuildInfo(Attachment attachment)
		{
			AttachmentInfo attachmentInfo = new AttachmentInfo();
			attachmentInfo.attachment = attachment;
			attachmentInfo.name = attachment.FileName;
			ExTraceGlobals.AttachmentFilteringTracer.TraceDebug<string>((long)attachmentInfo.GetHashCode(), "Attachment name: {0}", attachmentInfo.name);
			if (attachmentInfo.name.Length > AttachmentInfo.MaxPath)
			{
				ExTraceGlobals.AttachmentFilteringTracer.TraceDebug((long)attachmentInfo.GetHashCode(), "The attachment name is too long");
				return null;
			}
			string contentType = attachment.ContentType;
			if (!string.IsNullOrEmpty(contentType))
			{
				ExTraceGlobals.AttachmentFilteringTracer.TraceDebug<string>((long)attachmentInfo.GetHashCode(), "Content-Type from MIME header: {0}", contentType);
				attachmentInfo.contentTypes.Add(contentType);
			}
			Stream file;
			if (!attachment.TryGetContentReadStream(out file))
			{
				return null;
			}
			string text = attachmentInfo.sniffer.FindMimeFromData(file);
			if (!string.IsNullOrEmpty(text))
			{
				ExTraceGlobals.AttachmentFilteringTracer.TraceDebug<string>((long)attachmentInfo.GetHashCode(), "Sniffed Content-Type: {0}", text);
				if (!text.Equals(contentType, StringComparison.OrdinalIgnoreCase))
				{
					attachmentInfo.contentTypes.Add(text);
				}
			}
			return attachmentInfo;
		}

		private const int SnifferSampleSize = 64;

		private static readonly int MaxPath = 260;

		private Attachment attachment;

		private string name;

		private List<string> contentTypes = new List<string>();

		private DataSniff sniffer = new DataSniff(64);
	}
}
