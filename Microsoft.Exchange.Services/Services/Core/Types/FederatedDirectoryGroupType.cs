using System;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Services.Core.DataConverter;

namespace Microsoft.Exchange.Services.Core.Types
{
	public class FederatedDirectoryGroupType
	{
		public string Alias { get; set; }

		public string CalendarUrl { get; set; }

		public string DisplayName { get; set; }

		public string ExternalDirectoryObjectId { get; set; }

		public string InboxUrl { get; set; }

		public bool IsMember { get; set; }

		public bool IsPinned { get; set; }

		public string JoinDate { get; set; }

		public string LegacyDn { get; set; }

		public string PeopleUrl { get; set; }

		public string PhotoUrl { get; set; }

		public string SmtpAddress { get; set; }

		public FederatedDirectoryGroupTypeType GroupType { get; set; }

		public ExDateTime JoinDateTime
		{
			get
			{
				return this.joinDateTime;
			}
			set
			{
				this.joinDateTime = value;
				this.JoinDate = ExDateTimeConverter.ToSoapHeaderTimeZoneRelatedXsdDateTime(value);
			}
		}

		private ExDateTime joinDateTime;
	}
}
