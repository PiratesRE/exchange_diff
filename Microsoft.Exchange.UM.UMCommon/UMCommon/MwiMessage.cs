using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.UM.UMCommon
{
	internal class MwiMessage
	{
		internal MwiMessage(Guid mailboxGuid, Guid dialPlanGuid, string userName, string userExtension, int unreadVoicemailCount, int totalVoicemailCount, TimeSpan timeToLive, ExDateTime eventTimeUtc, Guid tenantGuid)
		{
			this.mailboxGuid = mailboxGuid;
			this.dialPlanGuid = dialPlanGuid;
			this.userName = (string.IsNullOrEmpty(userName) ? string.Empty : userName);
			this.userExtension = (string.IsNullOrEmpty(userExtension) ? string.Empty : userExtension);
			this.unreadVoicemailCount = unreadVoicemailCount;
			this.totalVoicemailCount = totalVoicemailCount;
			this.expirationTimeUtc = ExDateTime.UtcNow.Add(timeToLive);
			this.eventTimeUtc = eventTimeUtc;
			this.sentTimeUtc = eventTimeUtc;
			this.deliveryErrors = new List<MwiDeliveryException>();
			this.tenantGuid = tenantGuid;
		}

		internal Guid MailboxGuid
		{
			get
			{
				return this.mailboxGuid;
			}
		}

		internal Guid DialPlanGuid
		{
			get
			{
				return this.dialPlanGuid;
			}
		}

		internal string UserName
		{
			get
			{
				return this.userName;
			}
		}

		internal string UserExtension
		{
			get
			{
				return this.userExtension;
			}
		}

		internal int UnreadVoicemailCount
		{
			get
			{
				return this.unreadVoicemailCount;
			}
		}

		internal int TotalVoicemailCount
		{
			get
			{
				return this.totalVoicemailCount;
			}
		}

		internal int NumberOfTargetsAttempted
		{
			get
			{
				return this.numberOfTargetsAttempted;
			}
		}

		internal bool Expired
		{
			get
			{
				return ExDateTime.Compare(ExDateTime.UtcNow, this.expirationTimeUtc) >= 0;
			}
		}

		internal ExDateTime EventTimeUtc
		{
			get
			{
				return this.eventTimeUtc;
			}
		}

		internal ExDateTime SentTimeUtc
		{
			get
			{
				return this.sentTimeUtc;
			}
		}

		internal string MailboxDisplayName
		{
			get
			{
				return string.Format(CultureInfo.InvariantCulture, "{0}({1})", new object[]
				{
					this.UserName,
					this.MailboxGuid
				});
			}
		}

		internal List<MwiDeliveryException> DeliveryErrors
		{
			get
			{
				return this.deliveryErrors;
			}
		}

		internal SendMessageCompletedDelegate CompletionCallback
		{
			get
			{
				return this.completionCallback;
			}
		}

		internal IMwiTarget CurrentTarget
		{
			get
			{
				return this.currentTarget;
			}
		}

		internal Guid TenantGuid
		{
			get
			{
				return this.tenantGuid;
			}
		}

		public override string ToString()
		{
			string text = "MbxGuid:{0}, DPGuid:{1}, Name:{2}, Ext:{3}, UnreadVM:{4}, ";
			text += "TotalVM:{5}, Target:{6}, Expires:{7}, Expired:{8}, EventTime:{9}, SentTime:{10}, TenantGuid: {11}";
			return string.Format(CultureInfo.InvariantCulture, text, new object[]
			{
				this.MailboxGuid,
				this.DialPlanGuid,
				this.UserName,
				this.UserExtension,
				this.UnreadVoicemailCount,
				this.TotalVoicemailCount,
				(this.CurrentTarget != null) ? this.CurrentTarget.Name : "null",
				this.expirationTimeUtc,
				this.Expired,
				this.EventTimeUtc,
				this.SentTimeUtc,
				this.TenantGuid
			});
		}

		internal void SendAsync(IMwiTarget target, SendMessageCompletedDelegate completionCallback)
		{
			this.currentTarget = target;
			this.completionCallback = completionCallback;
			CallIdTracer.TraceDebug(ExTraceGlobals.MWITracer, this.GetHashCode(), "MwiMessage.SendAsync: Message={0}", new object[]
			{
				this
			});
			this.sentTimeUtc = ExDateTime.UtcNow;
			this.numberOfTargetsAttempted++;
			target.SendMessageAsync(this);
		}

		private readonly Guid tenantGuid;

		private Guid mailboxGuid;

		private Guid dialPlanGuid;

		private string userName;

		private string userExtension;

		private int unreadVoicemailCount;

		private int totalVoicemailCount;

		private int numberOfTargetsAttempted;

		private List<MwiDeliveryException> deliveryErrors;

		private SendMessageCompletedDelegate completionCallback;

		private IMwiTarget currentTarget;

		private ExDateTime expirationTimeUtc;

		private ExDateTime eventTimeUtc;

		private ExDateTime sentTimeUtc;
	}
}
