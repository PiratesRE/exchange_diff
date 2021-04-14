using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Globalization;
using Microsoft.Exchange.Data.Mime;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class ImapSmimeMimeProvider : IImapMimeProvider
	{
		internal ImapSmimeMimeProvider(MimePart smimePart, MimeDocument smimeDoc)
		{
			this.smimePart = smimePart;
			this.smimeDoc = smimeDoc;
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<ImapSmimeMimeProvider>(this);
		}

		protected override void InternalDispose(bool disposing)
		{
			base.InternalDispose(disposing);
		}

		internal override void WriteMimePart(MimeStreamWriter mimeWriter, ConversionLimitsTracker limits, MimePartInfo part, ItemToMimeConverter.MimeFlags mimeFlags)
		{
			if ((mimeFlags & ItemToMimeConverter.MimeFlags.SkipContent) != ItemToMimeConverter.MimeFlags.SkipContent)
			{
				mimeWriter.WritePartWithHeaders(part.SmimePart, false);
				return;
			}
			mimeWriter.WriteHeadersFromPart(part.SmimePart);
		}

		internal override MimePartInfo CalculateMimeStructure(Charset itemCharset)
		{
			int num = 0;
			return this.CalculateMimeSubStructure(this.smimePart, itemCharset, ref num);
		}

		internal override List<MimeDocument> ExtractSkeletons()
		{
			return new List<MimeDocument>(0);
		}

		private MimePartInfo CalculateMimeSubStructure(MimePart part, Charset itemCharset, ref int partIndex)
		{
			MimePartInfo mimePartInfo = null;
			if (part.IsEmbeddedMessage)
			{
				mimePartInfo = new MimePartInfo(itemCharset, null, MimePartContentType.ItemAttachment, null, null, null, part, this.smimeDoc, ref partIndex);
				int num = 0;
				mimePartInfo.AttachedItemStructure = this.CalculateMimeSubStructure((MimePart)part.FirstChild, itemCharset, ref num);
			}
			else
			{
				mimePartInfo = new MimePartInfo(itemCharset, null, MimePartInfo.GetContentType(part.ContentType), null, null, null, part, this.smimeDoc, ref partIndex);
				foreach (MimePart part2 in part)
				{
					mimePartInfo.AddChild(this.CalculateMimeSubStructure(part2, itemCharset, ref partIndex));
				}
			}
			foreach (Header header in part.Headers)
			{
				mimePartInfo.AddHeader(header);
			}
			MimeStreamWriter.CalculateBodySize(mimePartInfo, part);
			return mimePartInfo;
		}

		private MimePart smimePart;

		private MimeDocument smimeDoc;
	}
}
