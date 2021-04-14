using System;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Threading;
using Microsoft.Exchange.Data.Storage.Infoworker.MailboxSearch;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class NewMailboxSearchParameters : BaseMailboxSearchParameters
	{
		[OnDeserialized]
		private void OnDeserialized(StreamingContext context)
		{
			if (this.SearchAllDates)
			{
				base[SearchObjectSchema.StartDate] = null;
				base[SearchObjectSchema.EndDate] = null;
			}
			else
			{
				if (string.IsNullOrEmpty(this.SearchStartDate) && string.IsNullOrEmpty(this.SearchEndDate))
				{
					throw new FaultException(Strings.MailboxSearchSpecifySearchDate);
				}
				base[SearchObjectSchema.StartDate] = this.SearchStartDate.ToEcpExDateTime();
				base[SearchObjectSchema.EndDate] = this.SearchEndDate.ToEcpExDateTime();
			}
			if (this.SearchAllMailboxes)
			{
				base[SearchObjectSchema.SourceMailboxes] = null;
			}
			else
			{
				if (this.SourceMailboxes == null || this.SourceMailboxes.Length <= 0)
				{
					throw new FaultException(Strings.MailboxSearchSpecifySourceMailboxesError);
				}
				base[SearchObjectSchema.SourceMailboxes] = this.SourceMailboxes.ToIdParameters();
			}
			if (base.EstimateOnly)
			{
				base.ExcludeDuplicateMessages = false;
				base[SearchObjectSchema.LogLevel] = LoggingLevel.Suppress;
			}
			base[SearchObjectSchema.Language] = Thread.CurrentThread.CurrentUICulture.ToString();
		}

		[DataMember]
		public bool SearchAllMailboxes { get; set; }

		[DataMember]
		public bool SearchAllDates { get; set; }

		[DataMember]
		public string SearchStartDate { get; set; }

		[DataMember]
		public string SearchEndDate { get; set; }

		[DataMember]
		public Identity[] SourceMailboxes { get; set; }

		public override string AssociatedCmdlet
		{
			get
			{
				return "New-MailboxSearch";
			}
		}

		public override string RbacScope
		{
			get
			{
				return string.Empty;
			}
		}

		public const string RbacParameters = "?StartDate&EndDate&SourceMailboxes&Language";
	}
}
