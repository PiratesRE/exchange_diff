using System;
using Microsoft.Exchange.Extensibility.Internal;

namespace Microsoft.Exchange.Transport.Logging.MessageTracking
{
	internal class MsgTrackDSNInfo
	{
		public MsgTrackDSNInfo(string origMessageId, DsnFlags dsnType) : this(origMessageId, dsnType, string.Empty)
		{
		}

		public MsgTrackDSNInfo(string origMessageId, DsnFlags dsnType, string originalDsnSender)
		{
			this.origMessageId = origMessageId;
			this.dsnType = dsnType;
			this.originalDsnSender = originalDsnSender;
		}

		internal string OrigMessageID
		{
			get
			{
				return this.origMessageId;
			}
		}

		internal DsnFlags DsnType
		{
			get
			{
				return this.dsnType;
			}
		}

		internal string OriginalDsnSender
		{
			get
			{
				return this.originalDsnSender;
			}
		}

		private readonly string origMessageId;

		private readonly DsnFlags dsnType;

		private readonly string originalDsnSender;
	}
}
