using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Search.Core.Abstraction;
using Microsoft.Exchange.Search.Core.Common;

namespace Microsoft.Exchange.Management.Search
{
	[Serializable]
	public class FailedDocument : ConfigurableObject
	{
		internal FailedDocument(IFailureEntry failure, string subject, string databaseName, ADUser user) : base(new SimpleProviderPropertyBag())
		{
			this[FailedDocumentSchema.DocID] = failure.DocumentId;
			this[FailedDocumentSchema.EntryID] = failure.EntryId;
			this[FailedDocumentSchema.Database] = databaseName;
			this[FailedDocumentSchema.MailboxGuid] = failure.MailboxGuid;
			this[FailedDocumentSchema.ErrorCode] = (int)EvaluationErrorsHelper.GetErrorCode(failure.ErrorCode);
			this[FailedDocumentSchema.Description] = failure.ErrorDescription;
			this[FailedDocumentSchema.FailedTime] = failure.LastAttemptTime;
			this[FailedDocumentSchema.IsPartialIndexed] = failure.IsPartiallyIndexed;
			this[FailedDocumentSchema.AdditionalInfo] = failure.AdditionalInfo;
			this[FailedDocumentSchema.Subject] = subject;
			this[FailedDocumentSchema.FailureMode] = (failure.IsPermanentFailure ? FailureMode.Permanent : FailureMode.Transient);
			this[FailedDocumentSchema.AttemptCount] = failure.AttemptCount;
			if (user != null)
			{
				this[FailedDocumentSchema.Mailbox] = user.Name;
				this[FailedDocumentSchema.SmtpAddress] = user.PrimarySmtpAddress.ToString();
				return;
			}
			this[FailedDocumentSchema.Mailbox] = string.Empty;
			this[FailedDocumentSchema.SmtpAddress] = string.Empty;
		}

		internal override ObjectSchema ObjectSchema
		{
			get
			{
				return FailedDocument.schema;
			}
		}

		public int DocID
		{
			get
			{
				return (int)this[FailedDocumentSchema.DocID];
			}
		}

		public string Database
		{
			get
			{
				return (string)this[FailedDocumentSchema.Database];
			}
		}

		public Guid MailboxGuid
		{
			get
			{
				return (Guid)this[FailedDocumentSchema.MailboxGuid];
			}
		}

		public string Mailbox
		{
			get
			{
				return (string)this[FailedDocumentSchema.Mailbox];
			}
		}

		public string SmtpAddress
		{
			get
			{
				return (string)this[FailedDocumentSchema.SmtpAddress];
			}
		}

		public string EntryID
		{
			get
			{
				return (string)this[FailedDocumentSchema.EntryID];
			}
		}

		public string Subject
		{
			get
			{
				return (string)this[FailedDocumentSchema.Subject];
			}
		}

		public int ErrorCode
		{
			get
			{
				return (int)this[FailedDocumentSchema.ErrorCode];
			}
		}

		public LocalizedString Description
		{
			get
			{
				return (LocalizedString)this[FailedDocumentSchema.Description];
			}
		}

		public string AdditionalInfo
		{
			get
			{
				return (string)this[FailedDocumentSchema.AdditionalInfo];
			}
		}

		public bool IsPartialIndexed
		{
			get
			{
				return (bool)this[FailedDocumentSchema.IsPartialIndexed];
			}
		}

		public DateTime? FailedTime
		{
			get
			{
				return (DateTime?)this[FailedDocumentSchema.FailedTime];
			}
		}

		public FailureMode FailureMode
		{
			get
			{
				return (FailureMode)this[FailedDocumentSchema.FailureMode];
			}
		}

		public int AttemptCount
		{
			get
			{
				return (int)this[FailedDocumentSchema.AttemptCount];
			}
		}

		private static ObjectSchema schema = ObjectSchema.GetInstance<FailedDocumentSchema>();
	}
}
