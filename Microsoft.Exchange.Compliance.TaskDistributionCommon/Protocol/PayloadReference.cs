using System;
using Microsoft.Exchange.Compliance.TaskDistributionCommon.Serialization;

namespace Microsoft.Exchange.Compliance.TaskDistributionCommon.Protocol
{
	public class PayloadReference
	{
		static PayloadReference()
		{
			PayloadReference.description.ComplianceStructureId = 3;
			PayloadReference.description.RegisterIntegerPropertyGetterAndSetter(0, (PayloadReference item) => item.Count, delegate(PayloadReference item, int value)
			{
				item.Count = value;
			});
			PayloadReference.description.RegisterStringPropertyGetterAndSetter(0, (PayloadReference item) => item.PayloadId, delegate(PayloadReference item, string value)
			{
				item.PayloadId = value;
			});
			PayloadReference.description.RegisterStringPropertyGetterAndSetter(1, (PayloadReference item) => item.Bookmark, delegate(PayloadReference item, string value)
			{
				item.Bookmark = value;
			});
		}

		public static ComplianceSerializationDescription<PayloadReference> Description
		{
			get
			{
				return PayloadReference.description;
			}
		}

		public int Count { get; set; }

		public string Bookmark { get; set; }

		public string PayloadId { get; set; }

		public static implicit operator byte[](PayloadReference payload)
		{
			return ComplianceSerializer.Serialize<PayloadReference>(PayloadReference.Description, payload);
		}

		private static ComplianceSerializationDescription<PayloadReference> description = new ComplianceSerializationDescription<PayloadReference>();
	}
}
