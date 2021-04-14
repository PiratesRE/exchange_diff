using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Storage.Management;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class MailboxCalendarFolderRow : BaseRow
	{
		public MailboxCalendarFolderRow(MailboxCalendarFolder calendarFolder) : base(new Identity(calendarFolder.Identity.ToString(), calendarFolder.Identity.ToString()), calendarFolder)
		{
			this.MailboxCalendarFolder = calendarFolder;
		}

		public MailboxCalendarFolder MailboxCalendarFolder { get; private set; }

		[DataMember]
		public bool PublishEnabled
		{
			get
			{
				return this.MailboxCalendarFolder.PublishEnabled;
			}
			protected set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string DetailLevel
		{
			get
			{
				return this.MailboxCalendarFolder.DetailLevel.ToString();
			}
			protected set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string PublishDateRangeFrom
		{
			get
			{
				return this.MailboxCalendarFolder.PublishDateRangeFrom.ToString();
			}
			protected set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string PublishDateRangeTo
		{
			get
			{
				return this.MailboxCalendarFolder.PublishDateRangeTo.ToString();
			}
			protected set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string SearchableUrlEnabled
		{
			get
			{
				return this.MailboxCalendarFolder.SearchableUrlEnabled.ToString().ToLowerInvariant();
			}
			protected set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string PublishedICalUrl
		{
			get
			{
				return this.MailboxCalendarFolder.PublishedICalUrl ?? string.Empty;
			}
			protected set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string PublishedCalendarUrl
		{
			get
			{
				return this.MailboxCalendarFolder.PublishedCalendarUrl ?? string.Empty;
			}
			protected set
			{
				throw new NotSupportedException();
			}
		}
	}
}
