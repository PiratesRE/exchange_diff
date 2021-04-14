using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Hygiene.Data.Spam
{
	[Serializable]
	public class RuleDataBase : ISerializable
	{
		public long RuleID { get; set; }

		public long GroupID { get; set; }

		public decimal Sequence { get; set; }

		public RuleScopeType ScopeID { get; set; }

		public bool IsActive { get; set; }

		public bool IsPersistent { get; set; }

		public RuleStatusType? RuleStatus { get; set; }

		public string Comment { get; set; }

		public int? ApprovalStatusID { get; set; }

		public string ApprovedBy { get; set; }

		public string ModifiedBy { get; set; }

		public string DeletedBy { get; set; }

		public DateTime? ApprovedDate { get; set; }

		public long? AddedVersion { get; set; }

		public long? RemovedVersion { get; set; }

		public DateTime? CreatedDatetime { get; internal set; }

		public DateTime? ChangeDatetime { get; internal set; }

		public RuleDataBase()
		{
		}

		public RuleDataBase(SerializationInfo info, StreamingContext context)
		{
			throw new NotImplementedException("RuleDataBaseSerializer");
		}

		public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			throw new NotImplementedException("RuleDataBaseGetObjectData");
		}
	}
}
