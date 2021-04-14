using System;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data
{
	internal class KqlFilterSchema : FilterSchema
	{
		public override string And
		{
			get
			{
				return "AND";
			}
		}

		public override string Or
		{
			get
			{
				return "OR";
			}
		}

		public override string Not
		{
			get
			{
				return "NOT";
			}
		}

		public override string Like
		{
			get
			{
				return ":";
			}
		}

		public override string QuotationMark
		{
			get
			{
				return "\"";
			}
		}

		public override bool SupportQuotedPrefix
		{
			get
			{
				return false;
			}
		}

		public override string GetRelationalOperator(ComparisonOperator op)
		{
			switch (op)
			{
			case ComparisonOperator.Equal:
				return "=";
			case ComparisonOperator.NotEqual:
				return "<>";
			case ComparisonOperator.LessThan:
				return "<";
			case ComparisonOperator.LessThanOrEqual:
				return "<=";
			case ComparisonOperator.GreaterThan:
				return ">";
			case ComparisonOperator.GreaterThanOrEqual:
				return ">=";
			case ComparisonOperator.Like:
				return ":";
			default:
				return null;
			}
		}

		public override string EscapeStringValue(object o)
		{
			if (o == null)
			{
				return null;
			}
			string text;
			if (o is ExDateTime)
			{
				text = ((ExDateTime)o).ToString();
			}
			else if (o is string)
			{
				text = (string)o;
				if (text.IndexOf('"') >= 0)
				{
					throw new ArgumentOutOfRangeException(DataStrings.ErrorQuotionMarkNotSupportedInKql);
				}
			}
			else
			{
				text = o.ToString();
			}
			return text.Replace("\"", "\"\"");
		}

		public override string GetExistsFilter(ExistsFilter filter)
		{
			throw new NotSupportedException("KQL doesn't support exists operator.");
		}

		public override string GetFalseFilter()
		{
			return "false";
		}

		public override string GetPropertyName(string propertyName)
		{
			switch (propertyName)
			{
			case "SearchSender":
				return "from";
			case "SearchRecipientsTo":
				return "to";
			case "SearchRecipientsCc":
				return "cc";
			case "SearchRecipientsBcc":
				return "bcc";
			case "SearchRecipients":
				return "participants";
			case "TextBody":
				return "body";
			case "SubjectProperty":
				return "subject";
			case "AttachmentContent":
				return "attachment";
			case "SentTime":
				return "sent";
			case "ReceivedTime":
				return "received";
			case "ItemClass":
				return "kind";
			case "Categories":
				return "category";
			case "Importance":
				return "importance";
			case "Size":
				return "size";
			case "SearchAllIndexedProps":
				return null;
			}
			return propertyName;
		}

		public const string QueryFilterPropertySearchSender = "SearchSender";

		public const string QueryFilterPropertySearchRecipientsTo = "SearchRecipientsTo";

		public const string QueryFilterPropertySearchRecipientsCc = "SearchRecipientsCc";

		public const string QueryFilterPropertySearchRecipientsBcc = "SearchRecipientsBcc";

		public const string QueryFilterPropertySearchRecipients = "SearchRecipients";

		public const string QueryFilterPropertyTextBody = "TextBody";

		public const string QueryFilterPropertySubjectProperty = "SubjectProperty";

		public const string QueryFilterPropertyAttachmentContent = "AttachmentContent";

		public const string QueryFilterPropertySentTime = "SentTime";

		public const string QueryFilterPropertyReceivedTime = "ReceivedTime";

		public const string QueryFilterPropertyItemClass = "ItemClass";

		public const string QueryFilterPropertyCategories = "Categories";

		public const string QueryFilterPropertyImportance = "Importance";

		public const string QueryFilterPropertySize = "Size";

		public const string QueryFilterPropertySearchAllIndexedProps = "SearchAllIndexedProps";

		public const string KqlPropertyFrom = "from";

		public const string KqlPropertyTo = "to";

		public const string KqlPropertyCc = "cc";

		public const string KqlPropertyBcc = "bcc";

		public const string KqlPropertyParticipants = "participants";

		public const string KqlPropertyBody = "body";

		public const string KqlPropertySubject = "subject";

		public const string KqlPropertyAttachment = "attachment";

		public const string KqlPropertySent = "sent";

		public const string KqlPropertyReceived = "received";

		public const string KqlPropertyKind = "kind";

		public const string KqlPropertyCategory = "category";

		public const string KqlPropertyImportance = "importance";

		public const string KqlPropertySize = "size";
	}
}
