using System;

namespace Microsoft.Exchange.InfoWorker.Common.MessageTracking
{
	internal class TrackingContext
	{
		public TrackingContext(LogCache cache, DirectoryContext directoryContext, MessageTrackingReportId startingEventId)
		{
			this.tree = new EventTree();
			this.cache = cache;
			this.directoryContext = directoryContext;
			this.startingEventId = startingEventId;
		}

		public string SelectedRecipient
		{
			get
			{
				return this.selectedRecipient;
			}
			set
			{
				this.selectedRecipient = value;
			}
		}

		public ReportTemplate ReportTemplate
		{
			get
			{
				return this.reportTemplate;
			}
			set
			{
				this.reportTemplate = value;
			}
		}

		public MessageTrackingDetailLevel DetailLevel
		{
			get
			{
				return this.detailLevel;
			}
			set
			{
				this.detailLevel = value;
			}
		}

		public EventTree Tree
		{
			get
			{
				return this.tree;
			}
			set
			{
				this.tree = value;
			}
		}

		public LogCache Cache
		{
			get
			{
				return this.cache;
			}
		}

		public DirectoryContext DirectoryContext
		{
			get
			{
				return this.directoryContext;
			}
		}

		public MessageTrackingReportId StartingEventId
		{
			get
			{
				return this.startingEventId;
			}
		}

		private MessageTrackingReportId startingEventId;

		private DirectoryContext directoryContext;

		private LogCache cache;

		private EventTree tree;

		private MessageTrackingDetailLevel detailLevel;

		private ReportTemplate reportTemplate;

		private string selectedRecipient;
	}
}
