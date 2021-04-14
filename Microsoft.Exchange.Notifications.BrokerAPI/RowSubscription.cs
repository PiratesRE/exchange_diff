using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Conversion;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Services.Core.Search;

namespace Microsoft.Exchange.Notifications.Broker
{
	[XmlInclude(typeof(MessageItemSubscription))]
	[XmlInclude(typeof(CalendarItemSubscription))]
	[XmlInclude(typeof(PeopleIKnowSubscription))]
	[KnownType(typeof(PeopleIKnowSubscription))]
	[XmlInclude(typeof(ClutterFilter))]
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[KnownType(typeof(ConversationSubscription))]
	[XmlInclude(typeof(ConversationSubscription))]
	[KnownType(typeof(MessageItemSubscription))]
	[KnownType(typeof(CalendarItemSubscription))]
	[KnownType(typeof(SortResults))]
	[XmlInclude(typeof(SortResults))]
	[KnownType(typeof(ViewFilter))]
	[XmlInclude(typeof(ViewFilter))]
	[KnownType(typeof(ClutterFilter))]
	public abstract class RowSubscription : BaseSubscription
	{
		protected RowSubscription(NotificationType notificationType) : base(notificationType)
		{
		}

		[DataMember(EmitDefaultValue = false)]
		public string FolderId { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public SortResults[] SortBy { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public ViewFilter Filter { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public ClutterFilter ClutterFilter { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public string FromFilter { get; set; }

		[IgnoreDataMember]
		[XmlIgnore]
		public override IEnumerable<Tuple<string, object>> Differentiators
		{
			get
			{
				return base.Differentiators.Concat(new Tuple<string, object>[]
				{
					new Tuple<string, object>("FId", this.FolderId),
					new Tuple<string, object>("F", this.Filter),
					new Tuple<string, object>("CF", this.ClutterFilter),
					new Tuple<string, object>("FF", this.FromFilter),
					new Tuple<string, object>("SB", JsonConverter.Serialize<SortResults[]>(this.SortBy, null))
				});
			}
		}

		protected override bool Validate()
		{
			return base.Validate() && !string.IsNullOrEmpty(this.FolderId) && Enum.IsDefined(typeof(ViewFilter), this.Filter) && Enum.IsDefined(typeof(ClutterFilter), this.ClutterFilter);
		}
	}
}
