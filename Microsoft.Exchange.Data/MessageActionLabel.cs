using System;
using System.ComponentModel;

namespace Microsoft.Exchange.Data
{
	public enum MessageActionLabel
	{
		[Description("ReplyForwardFlag")]
		ReplyForwardFlag,
		[Description("DeleteFromClutter")]
		DeleteFromClutter,
		[Description("DeleteFromInboxWithoutReading")]
		DeleteFromInboxWithoutReading,
		[Description("DeleteFromInboxAfterReading")]
		DeleteFromInboxAfterReading,
		[Description("MarkAsClutter")]
		MarkAsClutter,
		[Description("MarkAsNotClutter")]
		MarkAsNotClutter,
		[Description("ReadInClutter")]
		ReadInClutter,
		[Description("ReadInInbox")]
		ReadInInbox,
		[Description("IgnoreInClutter")]
		IgnoreInClutter,
		[Description("IgnoreInInbox")]
		IgnoreInInbox,
		[Description("MarkAsUnreadInClutter")]
		MarkAsUnreadInClutter,
		[Description("MarkAsUnreadInInbox")]
		MarkAsUnreadInInbox,
		[Description("ReadMultipleTimesInClutter")]
		ReadMultipleTimesInClutter,
		[Description("ReadMultipleTimesInInbox")]
		ReadMultipleTimesInInbox,
		[Description("Moved")]
		Moved,
		[Description("CachedClutter")]
		CachedClutter,
		[Description("CachedNotClutter")]
		CachedNotClutter,
		[Description("OnVacation")]
		OnVacation
	}
}
