﻿using System;

namespace Microsoft.Exchange.Data.Mime
{
	internal enum HeaderNameIndex : byte
	{
		Unknown,
		ReturnPath,
		ResentDate,
		MessageId,
		Subject,
		Summary = 6,
		XPriority,
		Precedence,
		ListUnsubscribe,
		RR,
		Comments,
		ContentLocation,
		ResentFrom,
		ContentLanguage,
		ContentMD5,
		NewsGroups,
		Importance,
		ResentCc,
		ResentBcc,
		ResentSender,
		ContentClass,
		ListHelp,
		AdHoc,
		ContentBase,
		From,
		ReturnReceiptTo,
		NntpPostingHost,
		Lines,
		ContentDisposition,
		Bytes = 31,
		ContentId,
		Cc,
		FollowUpTo,
		ReplyBy,
		Bcc,
		ResentMessageId,
		XMSMailPriority = 39,
		XExchangeBcc,
		DeferredDelivery,
		Expires,
		Organization = 44,
		ContentTransferEncoding,
		Encoding,
		XExchangeCrossPremisesBcc,
		ContentType,
		ApparentlyTo,
		Approved,
		Received = 52,
		Path,
		References,
		Sender,
		ResentTo,
		Supercedes = 58,
		ResentReplyTo,
		Keywords,
		Control,
		InReplyTo,
		ListSubscribe,
		ContentDescription,
		Encrypted,
		MimeVersion = 67,
		Article,
		DispositionNotificationTo,
		Distribution,
		Date,
		XRef,
		Sensitivity = 74,
		To,
		ReplyTo,
		ExpiryDate,
		Priority
	}
}
