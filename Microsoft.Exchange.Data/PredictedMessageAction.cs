using System;
using System.ComponentModel;

namespace Microsoft.Exchange.Data
{
	public enum PredictedMessageAction : short
	{
		[Description("ReplyOrForward")]
		ReplyOrForward,
		[Description("Flag")]
		Flag,
		[Description("Delete")]
		Delete,
		[Description("MarkUnread")]
		MarkUnread,
		[Description("Ignore")]
		Ignore,
		[Description("Clutter")]
		Clutter,
		[Description("Max")]
		Max
	}
}
