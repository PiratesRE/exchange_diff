using System;
using System.Collections.Generic;

namespace Microsoft.Office.CompliancePolicy.PolicyEvaluation
{
	public class RuleTag
	{
		public RuleTag(string name, string tagType)
		{
			this.Name = name;
			this.TagType = tagType;
			this.Data = new Dictionary<string, string>();
		}

		public int Size { get; protected set; }

		public string Name { get; protected set; }

		public string TagType { get; protected set; }

		public Dictionary<string, string> Data { get; protected set; }
	}
}
