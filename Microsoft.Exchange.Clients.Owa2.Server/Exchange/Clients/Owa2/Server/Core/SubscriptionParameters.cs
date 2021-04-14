using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Services.Core.Search;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class SubscriptionParameters
	{
		public NotificationType NotificationType { get; set; }

		[DataMember(Name = "NotificationType")]
		public string NotificationTypeString
		{
			get
			{
				return this.NotificationType.ToString();
			}
			set
			{
				this.NotificationType = (NotificationType)Enum.Parse(typeof(NotificationType), value);
			}
		}

		public ViewFilter Filter { get; set; }

		[DataMember(Name = "Filter", IsRequired = false)]
		public string FilterString
		{
			get
			{
				return EnumUtilities.ToString<ViewFilter>(this.Filter);
			}
			set
			{
				this.Filter = EnumUtilities.Parse<ViewFilter>(value);
			}
		}

		[DataMember]
		public string CallId { get; set; }

		[DataMember]
		public bool IsConversation { get; set; }

		[DataMember]
		public string FolderId { get; set; }

		[DataMember]
		public SortResults[] SortBy { get; set; }

		[DataMember]
		public string ChannelId { get; set; }

		[DataMember]
		public bool InferenceEnabled { get; set; }

		[DataMember]
		public string FromFilter { get; set; }

		[DataMember]
		public string ConversationShapeName { get; set; }

		public ClutterFilter ClutterFilter { get; set; }

		[DataMember(Name = "ClutterFilter", IsRequired = false)]
		public string ClutterFilterString
		{
			get
			{
				return EnumUtilities.ToString<ClutterFilter>(this.ClutterFilter);
			}
			set
			{
				this.ClutterFilter = EnumUtilities.Parse<ClutterFilter>(value);
			}
		}

		[DataMember]
		public string MailboxId { get; set; }
	}
}
