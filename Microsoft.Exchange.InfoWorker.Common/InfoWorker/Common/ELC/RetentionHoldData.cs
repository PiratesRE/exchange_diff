using System;

namespace Microsoft.Exchange.InfoWorker.Common.ELC
{
	internal struct RetentionHoldData
	{
		internal RetentionHoldData(bool holdEnabled, string comment, string url)
		{
			this.HoldEnabled = holdEnabled;
			this.Comment = comment;
			this.Url = url;
		}

		internal bool HoldEnabled;

		internal string Comment;

		internal string Url;
	}
}
