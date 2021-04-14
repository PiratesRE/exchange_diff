using System;

namespace Microsoft.Exchange.Data.ContentTypes.Tnef
{
	public enum TnefAttributeTag
	{
		Null,
		From = 32768,
		Subject = 98308,
		DateSent = 229381,
		DateReceived,
		MessageStatus = 425991,
		MessageClass = 491528,
		MessageId = 98313,
		ParentId,
		ConversationId,
		Body = 163852,
		Priority = 294925,
		AttachData = 425999,
		AttachTitle = 98320,
		AttachMetaFile = 426001,
		AttachCreateDate = 229394,
		AttachModifyDate,
		DateModified = 229408,
		AttachTransportFilename = 430081,
		AttachRenderData,
		MapiProperties,
		RecipientTable,
		Attachment,
		TnefVersion = 561158,
		OemCodepage = 430087,
		OriginalMessageClass = 458758,
		Owner = 393216,
		SentFor,
		Delegate,
		DateStart = 196614,
		DateEnd,
		AidOwner = 327688,
		RequestResponse = 262153
	}
}
