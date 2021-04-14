using System;

namespace Microsoft.Exchange.Hygiene.Data.Spam
{
	internal class KesSpamSchema
	{
		public static readonly HygienePropertyDefinition ActiveOnlyProperty = new HygienePropertyDefinition("f_ActiveOnly", typeof(bool?));

		public static readonly HygienePropertyDefinition CommentProperty = new HygienePropertyDefinition("Comment", typeof(string));

		public static readonly HygienePropertyDefinition ApprovalStatusIDProperty = new HygienePropertyDefinition("ApprovalStatusId", typeof(int?));

		public static readonly HygienePropertyDefinition ApprovedByProperty = new HygienePropertyDefinition("ApprovedBy", typeof(string));

		public static readonly HygienePropertyDefinition DeletedByProperty = new HygienePropertyDefinition("DeletedBy", typeof(string));

		public static readonly HygienePropertyDefinition ModifiedByProperty = new HygienePropertyDefinition("ModifiedBy", typeof(string));

		public static readonly HygienePropertyDefinition DeactivatedByProperty = new HygienePropertyDefinition("DeactivatedBy", typeof(string));

		public static readonly HygienePropertyDefinition ApprovedDatetimeProperty = new HygienePropertyDefinition("ApprovedDatetime", typeof(DateTime?));

		public static readonly HygienePropertyDefinition IsUnifiedQueryProperty = new HygienePropertyDefinition("IsUnified", typeof(bool));
	}
}
