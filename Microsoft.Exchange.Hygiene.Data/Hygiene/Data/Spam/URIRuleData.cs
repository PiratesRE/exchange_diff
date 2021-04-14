using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Hygiene.Data.Spam
{
	[Serializable]
	public class URIRuleData : RuleDataBase
	{
		public string Uri { get; set; }

		public int UriType { get; set; }

		public int Score { get; set; }

		public bool Overridable { get; set; }

		public URIRuleData()
		{
		}

		public URIRuleData(SerializationInfo info, StreamingContext context)
		{
			this.Uri = (string)info.GetValue("Uri", typeof(string));
			this.UriType = (int)info.GetValue("UriType", typeof(int));
			this.Score = (int)info.GetValue("Score", typeof(int));
			this.Overridable = (bool)info.GetValue("Overridable", typeof(bool));
			base.RuleID = (long)info.GetValue("RuleID", typeof(long));
			base.GroupID = (long)info.GetValue("GroupID", typeof(long));
			base.IsActive = (bool)info.GetValue("IsActive", typeof(bool));
			base.IsPersistent = (bool)info.GetValue("IsPersistent", typeof(bool));
			base.Comment = (string)info.GetValue("Comment", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("Uri", this.Uri);
			info.AddValue("UriType", this.UriType);
			info.AddValue("Score", this.Score);
			info.AddValue("Overridable", this.Overridable);
			info.AddValue("RuleID", base.RuleID);
			info.AddValue("GroupID", base.GroupID);
			info.AddValue("IsActive", base.IsActive);
			info.AddValue("IsPersistent", base.IsPersistent);
			info.AddValue("Comment", base.Comment);
		}

		internal static class URIRulesDataConstants
		{
			internal const string UriSerializationName = "Uri";

			internal const string UriTypeSerializationName = "UriType";

			internal const string ScoreSerializationName = "Score";

			internal const string OverridableSerializationName = "Overridable";

			internal const string RuleIDSerializationName = "RuleID";

			internal const string GroupIDSerializationName = "GroupID";

			internal const string IsActiveSerializationName = "IsActive";

			internal const string IsPersistentSerializationName = "IsPersistent";

			internal const string CommentSerializationName = "Comment";
		}
	}
}
