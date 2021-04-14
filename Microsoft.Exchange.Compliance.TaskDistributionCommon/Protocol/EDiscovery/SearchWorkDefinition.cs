using System;
using Microsoft.Exchange.Compliance.TaskDistributionCommon.Extensibility;
using Microsoft.Exchange.Compliance.TaskDistributionCommon.Serialization;

namespace Microsoft.Exchange.Compliance.TaskDistributionCommon.Protocol.EDiscovery
{
	public class SearchWorkDefinition : WorkDefinition
	{
		static SearchWorkDefinition()
		{
			SearchWorkDefinition.description.ComplianceStructureId = 99;
			SearchWorkDefinition.description.RegisterStringPropertyGetterAndSetter(0, (SearchWorkDefinition item) => item.Query, delegate(SearchWorkDefinition item, string value)
			{
				item.Query = value;
			});
			SearchWorkDefinition.description.RegisterIntegerPropertyGetterAndSetter(0, (SearchWorkDefinition item) => (int)item.Parser, delegate(SearchWorkDefinition item, int value)
			{
				item.Parser = (SearchWorkDefinition.QueryParser)value;
			});
			SearchWorkDefinition.description.RegisterIntegerPropertyGetterAndSetter(1, (SearchWorkDefinition item) => item.DetailCount, delegate(SearchWorkDefinition item, int value)
			{
				item.DetailCount = value;
			});
		}

		public static ComplianceSerializationDescription<SearchWorkDefinition> Description
		{
			get
			{
				return SearchWorkDefinition.description;
			}
		}

		public string Query { get; set; }

		public SearchWorkDefinition.QueryParser Parser { get; set; }

		public int DetailCount { get; set; }

		private static ComplianceSerializationDescription<SearchWorkDefinition> description = new ComplianceSerializationDescription<SearchWorkDefinition>();

		public enum QueryParser
		{
			KQL,
			AQS,
			FQL
		}
	}
}
