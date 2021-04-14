using System;

namespace Microsoft.Exchange.EDiscovery.Export
{
	public class ExportRecord
	{
		public string OriginalPath { get; internal set; }

		public string TargetPath { get; internal set; }

		public string Id { get; internal set; }

		public int DocumentId { get; internal set; }

		public string InternetMessageId { get; internal set; }

		public string PrimaryIdOfDuplicates { get; internal set; }

		public ExportFile ExportFile { get; internal set; }

		public ExportRecord Parent { get; internal set; }

		public string SourceId { get; internal set; }

		public string Title { get; internal set; }

		public string Sender { get; internal set; }

		public string SenderSmtpAddress { get; internal set; }

		public uint Size { get; internal set; }

		public DateTime SentTime { get; internal set; }

		public DateTime ReceivedTime { get; internal set; }

		public string BodyPreview { get; internal set; }

		public string Importance { get; internal set; }

		public bool IsRead { get; internal set; }

		public bool HasAttachment { get; internal set; }

		public string ToRecipients { get; internal set; }

		public string CcRecipients { get; internal set; }

		public string BccRecipients { get; internal set; }

		public string ToGroupExpansionRecipients { get; set; }

		public string CcGroupExpansionRecipients { get; set; }

		public string BccGroupExpansionRecipients { get; set; }

		public string DGGroupExpansionError { get; set; }

		public string DocumentType { get; internal set; }

		public string MimeType
		{
			get
			{
				return "application/vnd.ms-outlook";
			}
		}

		public string RelationshipType { get; internal set; }

		public bool IsUnsearchable { get; internal set; }

		public const string DocumentTypeFile = "File";

		public const string DocumentTypeMessage = "Message";

		public const string RelationshipTypeNone = "None";

		public const string RelationshipTypeContainer = "Container";
	}
}
