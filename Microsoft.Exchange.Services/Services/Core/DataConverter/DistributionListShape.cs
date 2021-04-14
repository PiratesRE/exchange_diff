using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal sealed class DistributionListShape : Shape
	{
		static DistributionListShape()
		{
			DistributionListShape.defaultProperties.Add(ItemSchema.ItemId);
			DistributionListShape.defaultProperties.Add(ItemSchema.Subject);
			DistributionListShape.defaultProperties.Add(ItemSchema.Attachments);
			DistributionListShape.defaultProperties.Add(DistributionListSchema.DisplayName);
			DistributionListShape.defaultProperties.Add(DistributionListSchema.FileAs);
			DistributionListShape.defaultProperties.Add(DistributionListSchema.Members);
		}

		private DistributionListShape() : base(Schema.DistributionList, DistributionListSchema.GetSchema(), ItemShape.CreateShape(), DistributionListShape.defaultProperties)
		{
		}

		internal static DistributionListShape CreateShape()
		{
			return new DistributionListShape();
		}

		private static List<PropertyInformation> defaultProperties = new List<PropertyInformation>();
	}
}
