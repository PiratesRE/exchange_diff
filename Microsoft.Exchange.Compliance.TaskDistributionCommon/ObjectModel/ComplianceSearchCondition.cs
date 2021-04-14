using System;
using Microsoft.Exchange.Compliance.TaskDistributionCommon.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Compliance.TaskDistributionCommon.ObjectModel
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class ComplianceSearchCondition
	{
		static ComplianceSearchCondition()
		{
			ComplianceSearchCondition.description.ComplianceStructureId = 8;
			ComplianceSearchCondition.description.RegisterShortPropertyGetterAndSetter(0, (ComplianceSearchCondition item) => (short)item.Name, delegate(ComplianceSearchCondition item, short value)
			{
				item.Name = (ComplianceSearchCondition.ConditionName)value;
			});
			ComplianceSearchCondition.description.RegisterStringPropertyGetterAndSetter(0, (ComplianceSearchCondition item) => item.Content, delegate(ComplianceSearchCondition item, string value)
			{
				item.Content = value;
			});
		}

		public ComplianceSearchCondition()
		{
		}

		public ComplianceSearchCondition(ComplianceSearchCondition.ConditionName name, string content)
		{
			this.Name = name;
			this.Content = content;
		}

		public static ComplianceSerializationDescription<ComplianceSearchCondition> Description
		{
			get
			{
				return ComplianceSearchCondition.description;
			}
		}

		public ComplianceSearchCondition.ConditionName Name { get; set; }

		public string Content { get; set; }

		internal byte[] ToBlob()
		{
			return ComplianceSerializer.Serialize<ComplianceSearchCondition>(ComplianceSearchCondition.Description, this);
		}

		private static ComplianceSerializationDescription<ComplianceSearchCondition> description = new ComplianceSerializationDescription<ComplianceSearchCondition>();

		internal enum ConditionName : short
		{
			UnknownCondition,
			StartDate,
			EndDate
		}
	}
}
