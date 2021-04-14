using System;
using System.Text;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Management.ClassificationDefinitions
{
	[Serializable]
	public sealed class DataClassificationObjectId : ObjectId
	{
		internal DataClassificationObjectId(string ruleId)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("ruleId", ruleId);
			this.ruleId = ruleId;
			this.organizationHierarchy = null;
		}

		internal DataClassificationObjectId(string organizationHierarchy, string ruleId) : this(ruleId)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("organizationHierarchy", organizationHierarchy);
			this.organizationHierarchy = organizationHierarchy;
		}

		public string Name
		{
			get
			{
				return this.ruleId;
			}
		}

		public override string ToString()
		{
			if (!string.IsNullOrEmpty(this.organizationHierarchy))
			{
				return ClassificationDefinitionUtils.CreateHierarchicalIdentityString(this.organizationHierarchy, this.ruleId);
			}
			return this.ruleId;
		}

		public static implicit operator string(DataClassificationObjectId dataClassificationObjectId)
		{
			return dataClassificationObjectId.ToString();
		}

		public static implicit operator DataClassificationIdParameter(DataClassificationObjectId dataClassificationObjectId)
		{
			return DataClassificationIdParameter.Parse(dataClassificationObjectId.ToString());
		}

		public override byte[] GetBytes()
		{
			return Encoding.Unicode.GetBytes(this);
		}

		private readonly string organizationHierarchy;

		private readonly string ruleId;
	}
}
