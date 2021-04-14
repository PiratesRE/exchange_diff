using System;
using System.Collections.Generic;
using Microsoft.Exchange.Compliance.TaskDistributionCommon.Serialization;

namespace Microsoft.Exchange.Compliance.TaskDistributionCommon.Protocol
{
	public class RetrievedPayload : Payload
	{
		static RetrievedPayload()
		{
			RetrievedPayload.description.ComplianceStructureId = 5;
			RetrievedPayload.description.RegisterStringPropertyGetterAndSetter(0, (RetrievedPayload item) => item.PayloadId, delegate(RetrievedPayload item, string value)
			{
				item.PayloadId = value;
			});
			RetrievedPayload.description.RegisterStringPropertyGetterAndSetter(1, (RetrievedPayload item) => item.Bookmark, delegate(RetrievedPayload item, string value)
			{
				item.Bookmark = value;
			});
			RetrievedPayload.description.RegisterBytePropertyGetterAndSetter(0, (RetrievedPayload item) => item.IsComplete ? 1 : 0, delegate(RetrievedPayload item, byte value)
			{
				item.IsComplete = (value == 1);
			});
			RetrievedPayload.description.RegisterCollectionPropertyAccessors(0, () => CollectionItemType.Blob, (RetrievedPayload item) => item.Children.Count, (RetrievedPayload item, int index) => item.Children[index], delegate(RetrievedPayload item, object value, int index)
			{
				item.Children.Add((byte[])value);
			});
		}

		public static ComplianceSerializationDescription<RetrievedPayload> Description
		{
			get
			{
				return RetrievedPayload.description;
			}
		}

		public bool IsComplete { get; set; }

		public string Bookmark { get; set; }

		public List<byte[]> Children
		{
			get
			{
				return this.children;
			}
		}

		public static implicit operator byte[](RetrievedPayload payload)
		{
			return ComplianceSerializer.Serialize<RetrievedPayload>(RetrievedPayload.Description, payload);
		}

		private static ComplianceSerializationDescription<RetrievedPayload> description = new ComplianceSerializationDescription<RetrievedPayload>();

		private List<byte[]> children = new List<byte[]>();
	}
}
