using System;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal sealed class PendingGetPayload
	{
		public PendingGetPayload(int? emailCount, bool hasVoiceMail = false)
		{
			this.EmailCount = emailCount;
			this.HasVoiceMail = hasVoiceMail;
		}

		public int? EmailCount { get; private set; }

		public bool HasVoiceMail { get; private set; }

		public override string ToString()
		{
			if (this.toStringCache == null)
			{
				this.toStringCache = string.Format("{{emailCount:{0}; hasVoicemail:{1}}}", this.EmailCount, this.HasVoiceMail);
			}
			return this.toStringCache;
		}

		private string toStringCache;
	}
}
