using System;
using System.Runtime.Serialization;
using System.ServiceModel;
using Microsoft.Exchange.Data.Storage.Infoworker.MailboxSearch;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class SetMailboxSearchParameters : BaseMailboxSearchParameters
	{
		[OnDeserialized]
		private void OnDeserialized(StreamingContext context)
		{
			if (this.SearchAllDates != null)
			{
				if (this.SearchAllDates.Value)
				{
					base[SearchObjectSchema.StartDate] = null;
					base[SearchObjectSchema.EndDate] = null;
				}
				else if (string.IsNullOrEmpty(this.SearchStartDate) && string.IsNullOrEmpty(this.SearchEndDate))
				{
					throw new FaultException(Strings.MailboxSearchSpecifySearchDate);
				}
			}
			if (this.SearchAllMailboxes != null && this.SearchAllMailboxes.Value)
			{
				base[SearchObjectSchema.SourceMailboxes] = null;
			}
			else if (base.ParameterIsSpecified(SearchObjectSchema.SourceMailboxes.ToString()) && this.SourceMailboxes.IsNullOrEmpty())
			{
				throw new FaultException(Strings.MailboxSearchSpecifySourceMailboxesError);
			}
			if (base.EstimateOnly)
			{
				base.ExcludeDuplicateMessages = false;
				base[SearchObjectSchema.LogLevel] = LoggingLevel.Suppress;
			}
		}

		[DataMember]
		public bool? SearchAllMailboxes { get; set; }

		[DataMember]
		public bool? SearchAllDates { get; set; }

		[DataMember]
		public string SearchStartDate
		{
			get
			{
				return base[SearchObjectSchema.StartDate].ToStringWithNull();
			}
			set
			{
				base[SearchObjectSchema.StartDate] = value.ToEcpExDateTime();
			}
		}

		[DataMember]
		public string SearchEndDate
		{
			get
			{
				return base[SearchObjectSchema.EndDate].ToStringWithNull();
			}
			set
			{
				base[SearchObjectSchema.EndDate] = value.ToEcpExDateTime();
			}
		}

		[DataMember]
		public Identity[] SourceMailboxes
		{
			get
			{
				return Identity.FromIdParameters(base[SearchObjectSchema.SourceMailboxes]);
			}
			set
			{
				base[SearchObjectSchema.SourceMailboxes] = value.ToIdParameters();
			}
		}

		[DataMember]
		public bool IncludeKeywordStatistics
		{
			get
			{
				return (bool)(base[SearchObjectSchema.IncludeKeywordStatistics] ?? false);
			}
			set
			{
				base[SearchObjectSchema.IncludeKeywordStatistics] = value;
			}
		}

		public override string AssociatedCmdlet
		{
			get
			{
				return "Set-MailboxSearch";
			}
		}

		public override string RbacScope
		{
			get
			{
				return string.Empty;
			}
		}
	}
}
