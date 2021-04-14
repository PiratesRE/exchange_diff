using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Management.Automation;
using System.Text;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Search.AqsParser;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage.Infoworker.MailboxSearch
{
	[Serializable]
	public sealed class SearchObject : SearchObjectBase
	{
		internal static object AqsQueryGetter(IPropertyBag propertyBag)
		{
			StringBuilder stringBuilder = new StringBuilder(64);
			int num = 0;
			foreach (string text in ((MultiValuedProperty<string>)propertyBag[SearchObjectSchema.Senders]))
			{
				stringBuilder.Append("\"");
				stringBuilder.Append(text.Replace("\\", "\\\\"));
				stringBuilder.Append("\"*");
				num = stringBuilder.Length;
				stringBuilder.Append(" OR ");
			}
			if (num > 0)
			{
				stringBuilder.Length = num;
			}
			StringBuilder stringBuilder2 = new StringBuilder(64);
			num = 0;
			foreach (string text2 in ((MultiValuedProperty<string>)propertyBag[SearchObjectSchema.Recipients]))
			{
				stringBuilder2.Append("\"");
				stringBuilder2.Append(text2.Replace("\\", "\\\\"));
				stringBuilder2.Append("\"*");
				num = stringBuilder2.Length;
				stringBuilder2.Append(" OR ");
			}
			if (num > 0)
			{
				stringBuilder2.Length = num;
			}
			string text3 = string.Empty;
			ExDateTime? exDateTime = (ExDateTime?)propertyBag[SearchObjectSchema.StartDate];
			ExDateTime? exDateTime2 = SearchObject.RoundEndDate((ExDateTime?)propertyBag[SearchObjectSchema.EndDate]);
			if (exDateTime != null && exDateTime != null)
			{
				text3 = text3 + " >=" + exDateTime.Value.ToString();
			}
			if (exDateTime2 != null && exDateTime2 != null)
			{
				text3 = text3 + " <=" + exDateTime2.Value.ToString();
			}
			StringBuilder stringBuilder3 = new StringBuilder(64);
			num = 0;
			foreach (KindKeyword key in ((MultiValuedProperty<KindKeyword>)propertyBag[SearchObjectSchema.MessageTypes]))
			{
				stringBuilder3.Append(AqsParser.CanonicalKindKeys[key]);
				num = stringBuilder3.Length;
				stringBuilder3.Append(" OR ");
			}
			if (num > 0)
			{
				stringBuilder3.Length = num;
			}
			string text4 = stringBuilder.ToString();
			string text5 = stringBuilder2.ToString();
			string text6 = stringBuilder3.ToString();
			string str = string.Empty;
			text4 = (string.IsNullOrEmpty(text4) ? string.Empty : string.Format("{0}:({1}) ", AqsParser.CanonicalKeywords[PropertyKeyword.From], text4));
			text5 = (string.IsNullOrEmpty(text5) ? string.Empty : string.Format("{0}:({1}) ", AqsParser.CanonicalKeywords[PropertyKeyword.Participants], text5));
			if (!string.IsNullOrEmpty(text4) && !string.IsNullOrEmpty(text5))
			{
				str = string.Format("({0} OR {1}) ", text4, text5);
			}
			else if (!string.IsNullOrEmpty(text4))
			{
				str = text4;
			}
			else if (!string.IsNullOrEmpty(text5))
			{
				str = text5;
			}
			return str + (string.IsNullOrEmpty(text3) ? string.Empty : (AqsParser.CanonicalKeywords[PropertyKeyword.Received] + ":(" + text3 + ") ")) + (string.IsNullOrEmpty(text6) ? string.Empty : (AqsParser.CanonicalKeywords[PropertyKeyword.Kind] + ":(" + text6 + ") ")) + (string)propertyBag[SearchObjectSchema.SearchQuery];
		}

		public ADObjectId CreatedBy
		{
			get
			{
				return (ADObjectId)this[SearchObjectSchema.CreatedBy];
			}
			set
			{
				this[SearchObjectSchema.CreatedBy] = value;
			}
		}

		public string CreatedByEx
		{
			get
			{
				return (string)this[SearchObjectSchema.CreatedByEx];
			}
			set
			{
				this[SearchObjectSchema.CreatedByEx] = value;
			}
		}

		public MultiValuedProperty<ADObjectId> SourceMailboxes
		{
			get
			{
				return (MultiValuedProperty<ADObjectId>)this[SearchObjectSchema.SourceMailboxes];
			}
			set
			{
				this[SearchObjectSchema.SourceMailboxes] = value;
			}
		}

		public ADObjectId TargetMailbox
		{
			get
			{
				return (ADObjectId)this[SearchObjectSchema.TargetMailbox];
			}
			set
			{
				this[SearchObjectSchema.TargetMailbox] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string SearchQuery
		{
			get
			{
				return (string)this[SearchObjectSchema.SearchQuery];
			}
			set
			{
				this[SearchObjectSchema.SearchQuery] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public CultureInfo Language
		{
			get
			{
				return (CultureInfo)this[SearchObjectSchema.Language];
			}
			set
			{
				this[SearchObjectSchema.Language] = value;
			}
		}

		public MultiValuedProperty<string> Senders
		{
			get
			{
				return (MultiValuedProperty<string>)this[SearchObjectSchema.Senders];
			}
			set
			{
				this[SearchObjectSchema.Senders] = value;
			}
		}

		public MultiValuedProperty<string> Recipients
		{
			get
			{
				return (MultiValuedProperty<string>)this[SearchObjectSchema.Recipients];
			}
			set
			{
				this[SearchObjectSchema.Recipients] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ExDateTime? StartDate
		{
			get
			{
				return (ExDateTime?)this[SearchObjectSchema.StartDate];
			}
			set
			{
				this[SearchObjectSchema.StartDate] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ExDateTime? EndDate
		{
			get
			{
				return SearchObject.RoundEndDate((ExDateTime?)this[SearchObjectSchema.EndDate]);
			}
			set
			{
				this[SearchObjectSchema.EndDate] = value;
			}
		}

		public MultiValuedProperty<KindKeyword> MessageTypes
		{
			get
			{
				return (MultiValuedProperty<KindKeyword>)this[SearchObjectSchema.MessageTypes];
			}
			set
			{
				this[SearchObjectSchema.MessageTypes] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool SearchDumpster
		{
			get
			{
				return (bool)this[SearchObjectSchema.SearchDumpster];
			}
			set
			{
				this[SearchObjectSchema.SearchDumpster] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public LoggingLevel LogLevel
		{
			get
			{
				return (LoggingLevel)this[SearchObjectSchema.LogLevel];
			}
			set
			{
				this[SearchObjectSchema.LogLevel] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool IncludeUnsearchableItems
		{
			get
			{
				return (bool)this[SearchObjectSchema.IncludeUnsearchableItems];
			}
			set
			{
				this[SearchObjectSchema.IncludeUnsearchableItems] = value;
			}
		}

		public bool IncludePersonalArchive
		{
			get
			{
				return (bool)this[SearchObjectSchema.IncludePersonalArchive];
			}
			set
			{
				this[SearchObjectSchema.IncludePersonalArchive] = value;
			}
		}

		internal bool IncludeRemoteAccounts
		{
			get
			{
				return (bool)this[SearchObjectSchema.IncludeRemoteAccounts];
			}
			set
			{
				this[SearchObjectSchema.IncludeRemoteAccounts] = value;
			}
		}

		public MultiValuedProperty<ADObjectId> StatusMailRecipients
		{
			get
			{
				return (MultiValuedProperty<ADObjectId>)this[SearchObjectSchema.StatusMailRecipients];
			}
			set
			{
				this[SearchObjectSchema.StatusMailRecipients] = value;
			}
		}

		public MultiValuedProperty<ADObjectId> ManagedBy
		{
			get
			{
				return (MultiValuedProperty<ADObjectId>)this[SearchObjectSchema.ManagedBy];
			}
			set
			{
				this[SearchObjectSchema.ManagedBy] = value;
			}
		}

		public bool EstimateOnly
		{
			get
			{
				return (bool)this[SearchObjectSchema.EstimateOnly];
			}
			set
			{
				this[SearchObjectSchema.EstimateOnly] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool ExcludeDuplicateMessages
		{
			get
			{
				return (bool)this[SearchObjectSchema.ExcludeDuplicateMessages];
			}
			set
			{
				this[SearchObjectSchema.ExcludeDuplicateMessages] = value;
			}
		}

		public bool Resume
		{
			get
			{
				return (bool)this[SearchObjectSchema.Resume];
			}
			set
			{
				this[SearchObjectSchema.Resume] = value;
			}
		}

		public bool IncludeKeywordStatistics
		{
			get
			{
				return (bool)this[SearchObjectSchema.IncludeKeywordStatistics];
			}
			set
			{
				this[SearchObjectSchema.IncludeKeywordStatistics] = value;
			}
		}

		public bool KeywordStatisticsDisabled
		{
			get
			{
				return (bool)this[SearchObjectSchema.KeywordStatisticsDisabled];
			}
			set
			{
				this[SearchObjectSchema.KeywordStatisticsDisabled] = value;
			}
		}

		public MultiValuedProperty<string> Information
		{
			get
			{
				return (MultiValuedProperty<string>)this[SearchObjectSchema.Information];
			}
			set
			{
				this[SearchObjectSchema.Information] = value;
			}
		}

		public string AqsQuery
		{
			get
			{
				return (string)this[SearchObjectSchema.AqsQuery];
			}
		}

		internal override SearchObjectBaseSchema Schema
		{
			get
			{
				return SearchObject.schema;
			}
		}

		internal override ObjectType ObjectType
		{
			get
			{
				return ObjectType.SearchObject;
			}
		}

		internal SearchStatus SearchStatus { get; set; }

		protected override void ValidateWrite(List<ValidationError> errors)
		{
			base.ValidateWrite(errors);
			if (this.SourceMailboxes.Contains(base.Id.MailboxOwnerId))
			{
				errors.Add(new PropertyValidationError(ServerStrings.DiscoveryMailboxCannotBeSourceOrTarget(base.Id.MailboxOwnerId.DistinguishedName), SearchObjectSchema.SourceMailboxes, this.SourceMailboxes));
			}
			if (this.TargetMailbox != null)
			{
				if (this.SourceMailboxes.Contains(this.TargetMailbox))
				{
					errors.Add(new PropertyValidationError(ServerStrings.SearchTargetInSource, SearchObjectSchema.TargetMailbox, this.TargetMailbox));
				}
				if (this.TargetMailbox.Equals(base.Id.MailboxOwnerId))
				{
					errors.Add(new PropertyValidationError(ServerStrings.DiscoveryMailboxCannotBeSourceOrTarget(base.Id.MailboxOwnerId.DistinguishedName), SearchObjectSchema.TargetMailbox, this.TargetMailbox));
				}
			}
			if (this.StartDate != null && this.EndDate != null && this.StartDate > this.EndDate)
			{
				errors.Add(new PropertyValidationError(ServerStrings.InvalidateDateRange, SearchObjectSchema.StartDate, this.StartDate));
			}
			KindKeyword[] second = new KindKeyword[]
			{
				KindKeyword.faxes,
				KindKeyword.voicemail,
				KindKeyword.rssfeeds,
				KindKeyword.posts
			};
			if (this.MessageTypes.Intersect(second).Count<KindKeyword>() > 0)
			{
				errors.Add(new PropertyValidationError(ServerStrings.UnsupportedKindKeywords, SearchObjectSchema.MessageTypes, this.MessageTypes));
			}
			if (!string.IsNullOrEmpty(this.SearchQuery))
			{
				try
				{
					AqsParser aqsParser = new AqsParser();
					aqsParser.Parse(this.SearchQuery, AqsParser.ParseOption.None, this.Language).Dispose();
				}
				catch (ParserException ex)
				{
					errors.Add(new PropertyValidationError(ex.LocalizedString, SearchObjectSchema.SearchQuery, this.SearchQuery));
				}
			}
		}

		public static ExDateTime? RoundEndDate(ExDateTime? value)
		{
			ExDateTime? result = null;
			if (value != null)
			{
				ExDateTime value2 = value.Value;
				if (value2.Hour == 0 && value2.Minute == 0 && value2.Second == 0)
				{
					result = new ExDateTime?(new ExDateTime(value2.TimeZone, value2.Year, value2.Month, value2.Day, DateTime.MaxValue.Hour, DateTime.MaxValue.Minute, DateTime.MaxValue.Second));
				}
				else if (value2.Second == 0)
				{
					result = new ExDateTime?(new ExDateTime(value2.TimeZone, value2.Year, value2.Month, value2.Day, value2.Hour, value2.Minute, DateTime.MaxValue.Second));
				}
				else
				{
					result = new ExDateTime?(value2);
				}
			}
			return result;
		}

		private static SearchObjectSchema schema = ObjectSchema.GetInstance<SearchObjectSchema>();
	}
}
