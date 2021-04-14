using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(TypeName = "RulePredicateSizeRangeType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	public class RulePredicateSizeRange
	{
		[XmlElement(Order = 0)]
		public int MinimumSize { get; set; }

		[XmlIgnore]
		public bool MinimumSizeSpecified { get; set; }

		[XmlElement(Order = 1)]
		public int MaximumSize { get; set; }

		[XmlIgnore]
		public bool MaximumSizeSpecified { get; set; }

		public RulePredicateSizeRange()
		{
		}

		public RulePredicateSizeRange(int? minimumSize, int? maximumSize)
		{
			if (minimumSize != null)
			{
				this.MinimumSize = minimumSize.Value;
				this.MinimumSizeSpecified = true;
			}
			if (maximumSize != null)
			{
				this.MaximumSize = maximumSize.Value;
				this.MaximumSizeSpecified = true;
			}
		}
	}
}
