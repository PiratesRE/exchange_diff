using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.InfoWorker.Common.MessageTracking;

namespace Microsoft.Exchange.Management.Tracking
{
	[Serializable]
	public class MessageTrackingSearchResult : MessageTrackingConfigurableObject
	{
		public MessageTrackingSearchResult()
		{
		}

		internal override ObjectSchema ObjectSchema
		{
			get
			{
				return MessageTrackingSearchResult.schema;
			}
		}

		public MessageTrackingReportId MessageTrackingReportId
		{
			get
			{
				return (MessageTrackingReportId)this[MessageTrackingSharedResultSchema.MessageTrackingReportId];
			}
		}

		public DateTime SubmittedDateTime
		{
			get
			{
				return (DateTime)this[MessageTrackingSharedResultSchema.SubmittedDateTime];
			}
			internal set
			{
				this[MessageTrackingSharedResultSchema.SubmittedDateTime] = value;
			}
		}

		public string Subject
		{
			get
			{
				return (string)this[MessageTrackingSharedResultSchema.Subject];
			}
			internal set
			{
				this[MessageTrackingSharedResultSchema.Subject] = value;
			}
		}

		public SmtpAddress FromAddress
		{
			get
			{
				return (SmtpAddress)this[MessageTrackingSharedResultSchema.FromAddress];
			}
			set
			{
				this[MessageTrackingSharedResultSchema.FromAddress] = value;
			}
		}

		public string FromDisplayName
		{
			get
			{
				return (string)this[MessageTrackingSharedResultSchema.FromDisplayName];
			}
			set
			{
				this[MessageTrackingSharedResultSchema.FromDisplayName] = value;
			}
		}

		public SmtpAddress[] RecipientAddresses
		{
			get
			{
				return (SmtpAddress[])this[MessageTrackingSharedResultSchema.RecipientAddresses];
			}
			internal set
			{
				this[MessageTrackingSharedResultSchema.RecipientAddresses] = value;
			}
		}

		public string[] RecipientDisplayNames
		{
			get
			{
				return (string[])this[MessageTrackingSharedResultSchema.RecipientDisplayNames];
			}
			set
			{
				this[MessageTrackingSharedResultSchema.RecipientDisplayNames] = value;
			}
		}

		internal MessageTrackingSearchResult(MessageTrackingSearchResult internalMessageTrackingSearchResult)
		{
			this[MessageTrackingSharedResultSchema.MessageTrackingReportId] = new MessageTrackingReportId(internalMessageTrackingSearchResult.MessageTrackingReportId);
			this[MessageTrackingSharedResultSchema.SubmittedDateTime] = internalMessageTrackingSearchResult.SubmittedDateTime;
			this[MessageTrackingSharedResultSchema.Subject] = internalMessageTrackingSearchResult.Subject;
			this[MessageTrackingSharedResultSchema.FromAddress] = internalMessageTrackingSearchResult.FromAddress;
			this[MessageTrackingSharedResultSchema.FromDisplayName] = internalMessageTrackingSearchResult.FromDisplayName;
			this[MessageTrackingSharedResultSchema.RecipientAddresses] = internalMessageTrackingSearchResult.RecipientAddresses;
			this[MessageTrackingSharedResultSchema.RecipientDisplayNames] = null;
		}

		internal static void FillDisplayNames(List<MessageTrackingSearchResult> results, IRecipientSession recipSession)
		{
			BulkRecipientLookupCache bulkRecipientLookupCache = new BulkRecipientLookupCache(100);
			foreach (MessageTrackingSearchResult messageTrackingSearchResult in results)
			{
				IEnumerable<string> addresses = from address in messageTrackingSearchResult.RecipientAddresses
				select address.ToString();
				messageTrackingSearchResult.RecipientDisplayNames = bulkRecipientLookupCache.Resolve(addresses, recipSession).ToArray<string>();
			}
		}

		private static MessageTrackingSharedResultSchema schema = ObjectSchema.GetInstance<MessageTrackingSharedResultSchema>();
	}
}
