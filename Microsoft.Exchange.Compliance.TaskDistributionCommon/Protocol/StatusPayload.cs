using System;
using System.Collections.Generic;
using Microsoft.Exchange.Compliance.TaskDistributionCommon.Serialization;

namespace Microsoft.Exchange.Compliance.TaskDistributionCommon.Protocol
{
	internal class StatusPayload : Payload
	{
		static StatusPayload()
		{
			StatusPayload.description.ComplianceStructureId = 7;
			StatusPayload.description.RegisterStringPropertyGetterAndSetter(0, (StatusPayload item) => item.PayloadId, delegate(StatusPayload item, string value)
			{
				item.PayloadId = value;
			});
			StatusPayload.description.RegisterCollectionPropertyAccessors(0, () => CollectionItemType.String, (StatusPayload item) => item.QueuedMessages.Count, (StatusPayload item, int index) => item.QueuedMessages[index], delegate(StatusPayload item, object value, int index)
			{
				item.QueuedMessages.Add((string)value);
			});
		}

		public static ComplianceSerializationDescription<StatusPayload> Description
		{
			get
			{
				return StatusPayload.description;
			}
		}

		public List<string> QueuedMessages
		{
			get
			{
				return this.queuedMessages;
			}
		}

		private static ComplianceSerializationDescription<StatusPayload> description = new ComplianceSerializationDescription<StatusPayload>();

		private List<string> queuedMessages = new List<string>();
	}
}
