using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Globalization;
using Microsoft.Exchange.Data.Mime;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal abstract class IImapMimeProvider : DisposableObject
	{
		internal static IImapMimeProvider CreateInstance(ItemToMimeConverter itemConverter)
		{
			return new ImapItemMimeProvider(itemConverter);
		}

		internal static IImapMimeProvider CreateInstance(MimePart smimePart, MimeDocument smimeDoc)
		{
			return new ImapSmimeMimeProvider(smimePart, smimeDoc);
		}

		internal abstract void WriteMimePart(MimeStreamWriter mimeWriter, ConversionLimitsTracker limits, MimePartInfo part, ItemToMimeConverter.MimeFlags mimeFlags);

		internal abstract MimePartInfo CalculateMimeStructure(Charset itemCharset);

		internal abstract List<MimeDocument> ExtractSkeletons();
	}
}
