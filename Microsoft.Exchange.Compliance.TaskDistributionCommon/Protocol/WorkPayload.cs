using System;
using Microsoft.Exchange.Compliance.TaskDistributionCommon.Serialization;

namespace Microsoft.Exchange.Compliance.TaskDistributionCommon.Protocol
{
	public class WorkPayload : Payload
	{
		static WorkPayload()
		{
			WorkPayload.description.ComplianceStructureId = 4;
			WorkPayload.description.RegisterStringPropertyGetterAndSetter(0, (WorkPayload item) => item.PayloadId, delegate(WorkPayload item, string value)
			{
				item.PayloadId = value;
			});
			WorkPayload.description.RegisterIntegerPropertyGetterAndSetter(0, (WorkPayload item) => EnumConverter.ConvertEnumToInteger<WorkDefinitionType>(item.WorkDefinitionType), delegate(WorkPayload item, int value)
			{
				item.WorkDefinitionType = EnumConverter.ConvertIntegerToEnum<WorkDefinitionType>(value);
			});
			WorkPayload.description.RegisterIntegerPropertyGetterAndSetter(1, (WorkPayload item) => item.PayloadSerializationVersion, delegate(WorkPayload item, int value)
			{
				item.PayloadSerializationVersion = value;
			});
			WorkPayload.description.RegisterBlobPropertyGetterAndSetter(0, (WorkPayload item) => item.WorkDefinition, delegate(WorkPayload item, byte[] value)
			{
				item.WorkDefinition = value;
			});
		}

		public static ComplianceSerializationDescription<WorkPayload> Description
		{
			get
			{
				return WorkPayload.description;
			}
		}

		public WorkDefinitionType WorkDefinitionType { get; set; }

		public int PayloadSerializationVersion { get; set; }

		public byte[] WorkDefinition { get; set; }

		public static implicit operator byte[](WorkPayload payload)
		{
			return ComplianceSerializer.Serialize<WorkPayload>(WorkPayload.Description, payload);
		}

		private static ComplianceSerializationDescription<WorkPayload> description = new ComplianceSerializationDescription<WorkPayload>();
	}
}
