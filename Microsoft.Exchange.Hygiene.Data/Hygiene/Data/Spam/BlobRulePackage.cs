using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Hygiene.Data.Spam
{
	[Serializable]
	public class BlobRulePackage : ISerializable
	{
		public BlobRulePackage()
		{
		}

		public BlobRulePackage(SerializationInfo info, StreamingContext context)
		{
			this.RuleIDs = (long[])info.GetValue("RuleID", typeof(long[]));
			this.Uris = (string[])info.GetValue("RuleUri", typeof(string[]));
			this.Scores = (int[])info.GetValue("RuleScore", typeof(int[]));
			this.IsActiveRules = (bool[])info.GetValue("IsActive", typeof(bool[]));
			this.MajorVerison = (int)info.GetValue("MajorVersion", typeof(int));
			this.MinorVerison = (int)info.GetValue("MinorVersion", typeof(int));
		}

		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("RuleID", this.RuleIDs);
			info.AddValue("IsActive", this.IsActiveRules);
			info.AddValue("RuleScore", this.Scores);
			info.AddValue("RuleUri", this.Uris);
			info.AddValue("MajorVersion", this.MajorVerison);
			info.AddValue("MinorVersion", this.MinorVerison);
		}

		public int[] Scores { get; set; }

		public long[] RuleIDs { get; set; }

		public string[] Uris { get; set; }

		public bool[] IsActiveRules { get; set; }

		public int MajorVerison { get; set; }

		public int MinorVerison { get; set; }

		internal static class UriRulePackageDataConstants
		{
			internal const string RuleIDSerializationName = "RuleID";

			internal const string RuleScoreSerializationName = "RuleScore";

			internal const string RuleUriSerializationName = "RuleUri";

			internal const string RuleIsActiveSerializationName = "IsActive";

			internal const string MajorVersionSerializationName = "MajorVersion";

			internal const string MinorVersionSerializationName = "MinorVersion";
		}
	}
}
