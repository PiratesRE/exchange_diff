using System;
using Microsoft.Ceres.InteractionEngine.Services.Exchange;
using Microsoft.Ceres.SearchCore.Admin.Model;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Search.Core.Abstraction;
using Microsoft.Exchange.Search.Core.Common;
using Microsoft.Exchange.Search.OperatorSchema;

namespace Microsoft.Exchange.Search.Fast
{
	internal class FailureEntry : DocEntry, IFailureEntry, IDocEntry, IEquatable<IDocEntry>, IEquatable<IFailureEntry>
	{
		public FailureEntry()
		{
		}

		public FailureEntry(ISearchResultItem item) : base(item)
		{
		}

		IIdentity IFailureEntry.ItemId
		{
			get
			{
				return base.ItemId;
			}
		}

		public EvaluationErrors ErrorCode
		{
			get
			{
				if (this.errorCode == EvaluationErrors.None && this.IsPartiallyIndexed)
				{
					return EvaluationErrors.MarsWriterTruncation;
				}
				return this.errorCode;
			}
			set
			{
				this.errorCode = value;
			}
		}

		public LocalizedString ErrorDescription
		{
			get
			{
				return EvaluationErrorsHelper.GetErrorDescription(this.ErrorCode);
			}
		}

		public string AdditionalInfo { get; set; }

		public bool IsPartiallyIndexed { get; set; }

		public DateTime? LastAttemptTime { get; set; }

		public int AttemptCount { get; set; }

		public bool IsPermanentFailure { get; set; }

		internal new static IndexSystemField[] Schema
		{
			get
			{
				return FailureEntry.schema;
			}
		}

		public static IndexSystemField[] GetSchema(FieldSet fieldSet)
		{
			switch (fieldSet)
			{
			case FieldSet.None:
				return Array<IndexSystemField>.Empty;
			case FieldSet.Default:
				return FailureEntry.Schema;
			case FieldSet.RetryFeederProperties:
				return FailureEntry.RetryFeederFields;
			case FieldSet.IndexRepairAssistant:
				return FailureEntry.IndexRepairAssistantFields;
			default:
				throw new ArgumentException(string.Format("Field set value '{0}' is not valid in this context.", fieldSet), "fieldSet");
			}
		}

		public override string ToString()
		{
			return string.Format("ItemId: {0}, ErrorCode: {1}{2}, Attempt: {3}", new object[]
			{
				base.ItemId,
				this.ErrorCode,
				(this.ErrorCode == EvaluationErrors.None) ? string.Empty : (this.IsPermanentFailure ? "(permanent)" : "(retriable)"),
				this.AttemptCount
			});
		}

		public override bool Equals(object other)
		{
			return other is FailureEntry && base.Equals(other);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public bool Equals(IFailureEntry other)
		{
			return other != null && base.Equals(other);
		}

		protected override void SetProp(string name, object value)
		{
			if (name == FastIndexSystemSchema.ErrorCode.Name)
			{
				this.ErrorCode = EvaluationErrorsHelper.GetErrorCode((int)((long)value));
				this.IsPermanentFailure = EvaluationErrorsHelper.IsPermanentError((int)((long)value));
				return;
			}
			if (name == FastIndexSystemSchema.ErrorMessage.Name)
			{
				this.AdditionalInfo = (string)value;
				return;
			}
			if (name == FastIndexSystemSchema.IsPartiallyProcessed.Name)
			{
				this.IsPartiallyIndexed = (bool)value;
				return;
			}
			if (name == FastIndexSystemSchema.LastAttemptTime.Name)
			{
				this.LastAttemptTime = new DateTime?((DateTime)value);
				return;
			}
			if (name == FastIndexSystemSchema.AttemptCount.Name)
			{
				this.AttemptCount = (int)((long)value);
			}
		}

		private static readonly IndexSystemField[] schema = new IndexSystemField[]
		{
			FastIndexSystemSchema.ItemId.Definition,
			FastIndexSystemSchema.MailboxGuid.Definition,
			FastIndexSystemSchema.AttemptCount.Definition,
			FastIndexSystemSchema.ErrorCode.Definition,
			FastIndexSystemSchema.ErrorMessage.Definition,
			FastIndexSystemSchema.LastAttemptTime.Definition,
			FastIndexSystemSchema.IsPartiallyProcessed.Definition
		};

		private static readonly IndexSystemField[] RetryFeederFields = new IndexSystemField[]
		{
			FastIndexSystemSchema.ItemId.Definition,
			FastIndexSystemSchema.MailboxGuid.Definition,
			FastIndexSystemSchema.AttemptCount.Definition,
			FastIndexSystemSchema.ErrorCode.Definition,
			FastIndexSystemSchema.LastAttemptTime.Definition
		};

		private static readonly IndexSystemField[] IndexRepairAssistantFields = new IndexSystemField[]
		{
			FastIndexSystemSchema.ErrorCode.Definition
		};

		private EvaluationErrors errorCode;
	}
}
