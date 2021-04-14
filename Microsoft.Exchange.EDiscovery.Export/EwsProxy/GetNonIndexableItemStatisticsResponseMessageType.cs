using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[Serializable]
	public class GetNonIndexableItemStatisticsResponseMessageType : ResponseMessageType
	{
		[XmlArrayItem("NonIndexableItemStatistic", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		public NonIndexableItemStatisticType[] NonIndexableItemStatistics
		{
			get
			{
				return this.nonIndexableItemStatisticsField;
			}
			set
			{
				this.nonIndexableItemStatisticsField = value;
			}
		}

		private NonIndexableItemStatisticType[] nonIndexableItemStatisticsField;
	}
}
