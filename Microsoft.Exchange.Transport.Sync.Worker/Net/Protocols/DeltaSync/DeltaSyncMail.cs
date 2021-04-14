using System;
using System.IO;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class DeltaSyncMail : DeltaSyncObject
	{
		internal DeltaSyncMail(Guid serverId) : base(serverId)
		{
			this.Initialize();
		}

		internal DeltaSyncMail(string clientId) : base(clientId)
		{
			this.Initialize();
		}

		internal string From
		{
			get
			{
				return this.from;
			}
			set
			{
				this.from = value;
			}
		}

		internal DeltaSyncMail.ImportanceLevel Importance
		{
			get
			{
				return this.importance;
			}
			set
			{
				this.importance = value;
			}
		}

		internal ExDateTime DateReceived
		{
			get
			{
				return this.dateReceived;
			}
			set
			{
				this.dateReceived = value;
			}
		}

		internal string DateReceivedUniversalTimeString
		{
			get
			{
				return this.dateReceived.UniversalTime.ToString(DeltaSyncCommon.DateTimeFormatString);
			}
		}

		internal string MessageClass
		{
			get
			{
				return this.messageClass;
			}
			set
			{
				this.messageClass = value;
			}
		}

		internal string Subject
		{
			get
			{
				return this.subject;
			}
			set
			{
				this.subject = value;
			}
		}

		internal bool Read
		{
			get
			{
				return this.read;
			}
			set
			{
				this.read = value;
			}
		}

		internal string ConversationTopic
		{
			get
			{
				return this.conversationTopic;
			}
			set
			{
				this.conversationTopic = value;
			}
		}

		internal string ConversationIndex
		{
			get
			{
				return this.conversationIndex;
			}
			set
			{
				this.conversationIndex = value;
			}
		}

		internal DeltaSyncMail.SensitivityLevel Sensitivity
		{
			get
			{
				return this.sensitivity;
			}
			set
			{
				this.sensitivity = value;
			}
		}

		internal int Size
		{
			get
			{
				return this.size;
			}
			set
			{
				this.size = value;
			}
		}

		internal bool HasAttachments
		{
			get
			{
				return this.hasAttachments;
			}
			set
			{
				this.hasAttachments = value;
			}
		}

		internal Stream EmailMessage
		{
			get
			{
				return this.emailMessage;
			}
			set
			{
				this.emailMessage = value;
			}
		}

		internal DeltaSyncMail.ReplyToOrForwardState? ReplyToOrForward
		{
			get
			{
				return this.replyToOrForward;
			}
			set
			{
				this.replyToOrForward = value;
			}
		}

		internal bool IsDraft
		{
			get
			{
				return this.messageClass != null && this.messageClass.Equals(DeltaSyncCommon.DraftMessageClass, StringComparison.OrdinalIgnoreCase);
			}
		}

		internal static bool IsSupportedMessageClass(string messageClass)
		{
			if (messageClass == null)
			{
				return false;
			}
			foreach (string value in DeltaSyncCommon.SupportedMessageClasses)
			{
				if (messageClass.Equals(value, StringComparison.OrdinalIgnoreCase))
				{
					return true;
				}
			}
			return false;
		}

		internal string MessageIncludeContentId
		{
			get
			{
				return this.messageIncludeContentId;
			}
		}

		private void Initialize()
		{
			this.importance = DeltaSyncMail.ImportanceLevel.Normal;
			this.dateReceived = ExDateTime.UtcNow;
			this.messageClass = DeltaSyncCommon.NormalMessageClass;
			this.read = false;
			this.sensitivity = DeltaSyncMail.SensitivityLevel.Normal;
			this.hasAttachments = false;
			this.replyToOrForward = null;
			this.messageIncludeContentId = Guid.NewGuid().ToString();
		}

		private string from;

		private DeltaSyncMail.ImportanceLevel importance;

		private ExDateTime dateReceived;

		private string messageClass;

		private string subject;

		private bool read;

		private string conversationTopic;

		private string conversationIndex;

		private DeltaSyncMail.SensitivityLevel sensitivity;

		private int size;

		private bool hasAttachments;

		private Stream emailMessage;

		private DeltaSyncMail.ReplyToOrForwardState? replyToOrForward;

		private string messageIncludeContentId;

		internal enum ImportanceLevel : byte
		{
			Low,
			Normal,
			High
		}

		internal enum ReplyToOrForwardState : byte
		{
			None,
			RepliedTo,
			Forwarded
		}

		internal enum SensitivityLevel : byte
		{
			Normal,
			Personal,
			Private,
			Confidential
		}
	}
}
