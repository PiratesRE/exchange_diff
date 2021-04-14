using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Globalization;
using Microsoft.Exchange.Data.Mime;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class ImapItemMimeProvider : IImapMimeProvider
	{
		internal ImapItemMimeProvider(ItemToMimeConverter itemConverter)
		{
			this.itemConverter = itemConverter;
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<ImapItemMimeProvider>(this);
		}

		protected override void InternalDispose(bool disposing)
		{
			base.InternalDispose(disposing);
			if (disposing && this.itemConverter != null)
			{
				this.itemConverter.Dispose();
				this.itemConverter = null;
			}
		}

		internal override void WriteMimePart(MimeStreamWriter mimeWriter, ConversionLimitsTracker limits, MimePartInfo part, ItemToMimeConverter.MimeFlags mimeFlags)
		{
			this.itemConverter.WriteMimePart(mimeWriter, limits, part, mimeFlags);
		}

		internal override MimePartInfo CalculateMimeStructure(Charset itemCharset)
		{
			return this.itemConverter.CalculateMimeStructure(itemCharset);
		}

		internal override List<MimeDocument> ExtractSkeletons()
		{
			return this.itemConverter.ExtractSkeletons();
		}

		private ItemToMimeConverter itemConverter;
	}
}
