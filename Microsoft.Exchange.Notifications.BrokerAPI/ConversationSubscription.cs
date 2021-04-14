using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Conversion;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Notifications.Broker
{
	[KnownType(typeof(ConversationResponseShape))]
	[XmlInclude(typeof(ConversationResponseShape))]
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public sealed class ConversationSubscription : RowSubscription
	{
		public ConversationSubscription() : base(NotificationType.Conversation)
		{
		}

		[DataMember(EmitDefaultValue = false)]
		public ConversationResponseShape ConversationShape { get; set; }

		[IgnoreDataMember]
		[XmlIgnore]
		public override IEnumerable<Tuple<string, object>> Differentiators
		{
			get
			{
				return base.Differentiators.Concat(new Tuple<string, object>[]
				{
					new Tuple<string, object>("CS", JsonConverter.Serialize<ConversationResponseShape>(this.ConversationShape, null))
				});
			}
		}

		protected override bool Validate()
		{
			return base.Validate() && this.ConversationShape != null;
		}
	}
}
