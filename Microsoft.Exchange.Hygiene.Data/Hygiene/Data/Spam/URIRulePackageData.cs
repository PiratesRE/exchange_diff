using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Hygiene.Data.Spam
{
	[Serializable]
	public class URIRulePackageData : ISerializable
	{
		public URIRulePackageData()
		{
		}

		public URIRulePackageData(SerializationInfo info, StreamingContext context)
		{
			this.Rules = (URIRuleData[])info.GetValue("Rule", typeof(URIRuleData[]));
		}

		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("Rule", this.Rules);
		}

		public URIRuleData[] Rules { get; set; }

		internal static class URIRulePackageDataConstants
		{
			internal const string RuleSerializationName = "Rule";
		}
	}
}
