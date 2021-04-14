using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	public class RuleTag
	{
		public int Size { get; protected set; }

		public string Name { get; protected set; }

		public string TagType { get; protected set; }

		public Dictionary<string, string> Data { get; protected set; }

		public RuleTag(string name, string tagType)
		{
			this.Name = name;
			this.TagType = tagType;
			this.Data = new Dictionary<string, string>();
			if (this.Name != null)
			{
				this.Size += this.Name.Length * 2;
				this.Size += 18;
			}
			if (this.TagType != null)
			{
				this.Size += this.TagType.Length * 2;
				this.Size += 18;
			}
			if (this.Data != null)
			{
				foreach (KeyValuePair<string, string> keyValuePair in this.Data)
				{
					if (keyValuePair.Key != null)
					{
						this.Size += keyValuePair.Key.Length * 2;
						this.Size += 18;
					}
					if (keyValuePair.Value != null)
					{
						this.Size += keyValuePair.Value.Length * 2;
						this.Size += 18;
					}
				}
			}
		}
	}
}
