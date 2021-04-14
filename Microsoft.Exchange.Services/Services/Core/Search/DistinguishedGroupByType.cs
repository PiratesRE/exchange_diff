using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.Search
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType(TypeName = "DistinguishedGroupByType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public class DistinguishedGroupByType : GroupByType
	{
		[IgnoreDataMember]
		[XmlElement(ElementName = "StandardGroupBy", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
		public StandardGroupBys StandardGroupBy
		{
			get
			{
				return this.standardGroupBy;
			}
			set
			{
				this.standardGroupBy = value;
				this.ConfigureGroupByProperties();
			}
		}

		[XmlIgnore]
		[DataMember(Name = "StandardGroupBy")]
		public string StandardGroupByString
		{
			get
			{
				return EnumUtilities.ToString<StandardGroupBys>(this.StandardGroupBy);
			}
			set
			{
				this.StandardGroupBy = EnumUtilities.Parse<StandardGroupBys>(value);
			}
		}

		private void ConfigureGroupByProperties()
		{
			StandardGroupBys standardGroupBys = this.standardGroupBy;
			if (standardGroupBys != StandardGroupBys.ConversationTopic)
			{
				return;
			}
			base.AggregateOn = new AggregateOnType(DistinguishedGroupByType.conversationTopicAggregationProperty, AggregateType.Maximum);
			base.GroupByProperty = DistinguishedGroupByType.conversationTopicGroupByProperty;
		}

		private static PropertyPath conversationTopicAggregationProperty = new PropertyUri(PropertyUriEnum.DateTimeReceived);

		private static PropertyPath conversationTopicGroupByProperty = new PropertyUri(PropertyUriEnum.ConversationTopic);

		private StandardGroupBys standardGroupBy;
	}
}
