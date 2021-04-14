using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Office.CompliancePolicy.PolicyEvaluation;

namespace Microsoft.Office.CompliancePolicy.ComplianceData
{
	public abstract class ComplianceItem
	{
		public virtual string Id { get; protected set; }

		public virtual string Extension { get; protected set; }

		public virtual string DisplayName { get; protected set; }

		public virtual string WorkloadIdentifier { get; protected set; }

		public virtual DateTime WhenCreated { get; protected set; }

		public virtual DateTime WhenLastModified { get; protected set; }

		public virtual string Creator { get; protected set; }

		public virtual string LastModifier { get; protected set; }

		public virtual bool IsDirty { get; protected set; }

		public virtual DateTime ExpiryTime { get; protected set; }

		public virtual IList<Guid> ClassificationScanned { get; set; }

		public virtual IDictionary<Guid, ClassificationResult> ClassificationDiscovered { get; set; }

		public virtual ComplianceItemStatusFlag Status { get; protected set; }

		public virtual AuditState AuditState { get; set; }

		public virtual AccessScope AccessScope { get; set; }

		public virtual IDictionary<string, List<string>> Metadata { get; set; }

		public virtual Stream OpenBodyReadStream()
		{
			return null;
		}

		public virtual Stream OpenExtractedBodyTextReadStream()
		{
			return null;
		}

		public virtual Stream OpenExtractedAttachmentsTextReadStream()
		{
			return null;
		}

		public void Save()
		{
			if (!this.IsDirty)
			{
				throw new InvalidOperationException("Cannot call Save if the item is not dirty.");
			}
			this.InternalSave();
		}

		public virtual void InternalSave()
		{
		}

		public const string ExMessageExtensionName = "ExMessage";
	}
}
