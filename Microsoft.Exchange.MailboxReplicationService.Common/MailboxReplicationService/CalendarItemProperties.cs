using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[DataContract]
	internal class CalendarItemProperties : ItemPropertiesBase
	{
		[DataMember(Name = "ICalContents")]
		public string ICalContents { get; set; }

		public CalendarItemProperties(string iCalContents)
		{
			this.ICalContents = iCalContents;
		}

		public override void Apply(MailboxSession session, Item item)
		{
			if (string.IsNullOrEmpty(this.ICalContents))
			{
				return;
			}
			InboundConversionOptions scopedInboundConversionOptions = MapiUtils.GetScopedInboundConversionOptions(session.GetADRecipientSession(true, ConsistencyMode.IgnoreInvalid));
			ItemConversion.ConvertICalToItem(item, scopedInboundConversionOptions, this.ICalContents);
		}
	}
}
