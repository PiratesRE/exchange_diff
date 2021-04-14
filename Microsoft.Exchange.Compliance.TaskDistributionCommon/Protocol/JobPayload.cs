using System;
using System.Collections.Generic;
using Microsoft.Exchange.Compliance.TaskDistributionCommon.Serialization;

namespace Microsoft.Exchange.Compliance.TaskDistributionCommon.Protocol
{
	public class JobPayload : Payload
	{
		static JobPayload()
		{
			JobPayload.description.ComplianceStructureId = 5;
			JobPayload.description.RegisterStringPropertyGetterAndSetter(0, (JobPayload item) => item.PayloadId, delegate(JobPayload item, string value)
			{
				item.PayloadId = value;
			});
			JobPayload.description.RegisterStringPropertyGetterAndSetter(1, (JobPayload item) => item.JobId, delegate(JobPayload item, string value)
			{
				item.JobId = value;
			});
			JobPayload.description.RegisterBlobPropertyGetterAndSetter(0, (JobPayload item) => item.Payload, delegate(JobPayload item, byte[] value)
			{
				item.Payload = value;
			});
			JobPayload.description.RegisterComplexPropertyAsBlobGetterAndSetter<Target>(1, (JobPayload item) => item.Target, delegate(JobPayload item, Target value)
			{
				item.Target = value;
			}, Target.Description);
			JobPayload.description.RegisterCollectionPropertyAccessors(0, () => CollectionItemType.Blob, (JobPayload item) => item.Children.Count, (JobPayload item, int index) => item.Children[index], delegate(JobPayload item, object value, int index)
			{
				item.Children.Add((byte[])value);
			});
		}

		public static ComplianceSerializationDescription<JobPayload> Description
		{
			get
			{
				return JobPayload.description;
			}
		}

		public byte[] Payload { get; set; }

		public Target Target { get; set; }

		public string JobId { get; set; }

		public List<byte[]> Children
		{
			get
			{
				return this.children;
			}
		}

		public static implicit operator byte[](JobPayload payload)
		{
			return ComplianceSerializer.Serialize<JobPayload>(JobPayload.Description, payload);
		}

		private static ComplianceSerializationDescription<JobPayload> description = new ComplianceSerializationDescription<JobPayload>();

		private List<byte[]> children = new List<byte[]>();
	}
}
