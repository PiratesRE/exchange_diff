using System;
using System.Collections.Generic;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.InfoWorker.Common.MessageTracking;

namespace Microsoft.Exchange.Management.Tracking
{
	[Serializable]
	public class MessageTrackingReportId : IIdentityParameter
	{
		public MessageTrackingReportId()
		{
		}

		public static MessageTrackingReportId Parse(string identity)
		{
			MessageTrackingReportId messageTrackingReportId;
			if (!MessageTrackingReportId.TryParse(identity, out messageTrackingReportId))
			{
				return null;
			}
			return new MessageTrackingReportId(messageTrackingReportId);
		}

		public override string ToString()
		{
			return this.internalMessageTrackingReportId.ToString();
		}

		public string MessageId
		{
			get
			{
				return this.internalMessageTrackingReportId.MessageId;
			}
		}

		public long InternalMessageId
		{
			get
			{
				return this.internalMessageTrackingReportId.InternalMessageId;
			}
		}

		public string Server
		{
			get
			{
				return this.internalMessageTrackingReportId.Server;
			}
		}

		public SmtpAddress Mailbox
		{
			get
			{
				return this.internalMessageTrackingReportId.Mailbox;
			}
		}

		public Guid UserGuid
		{
			get
			{
				return this.internalMessageTrackingReportId.UserGuid;
			}
		}

		public bool IsSender
		{
			get
			{
				return this.internalMessageTrackingReportId.IsSender;
			}
		}

		public IEnumerable<T> GetObjects<T>(ObjectId rootId, IConfigDataProvider session) where T : IConfigurable, new()
		{
			LocalizedString? localizedString;
			return this.GetObjects<T>(rootId, session, null, out localizedString);
		}

		public IEnumerable<T> GetObjects<T>(ObjectId rootId, IConfigDataProvider session, OptionalIdentityData optionalData, out LocalizedString? notFoundReason) where T : IConfigurable, new()
		{
			throw new NotImplementedException();
		}

		public void Initialize(ObjectId objectId)
		{
			throw new NotImplementedException();
		}

		public string RawIdentity
		{
			get
			{
				return this.internalMessageTrackingReportId.ToString();
			}
		}

		internal MessageTrackingReportId InternalMessageTrackingReportId
		{
			get
			{
				return this.internalMessageTrackingReportId;
			}
		}

		internal MessageTrackingReportId(MessageTrackingReportId internalMessageTrackingReportId)
		{
			this.internalMessageTrackingReportId = internalMessageTrackingReportId;
		}

		private MessageTrackingReportId internalMessageTrackingReportId;
	}
}
