using System;
using Microsoft.Exchange.Compliance.TaskDistributionCommon.Instrumentation;
using Microsoft.Exchange.Compliance.TaskDistributionCommon.Serialization;

namespace Microsoft.Exchange.Compliance.TaskDistributionCommon.Protocol
{
	public class ComplianceMessage
	{
		static ComplianceMessage()
		{
			ComplianceMessage.description.ComplianceStructureId = 1;
			ComplianceMessage.description.RegisterBytePropertyGetterAndSetter(0, (ComplianceMessage item) => (byte)item.ComplianceMessageType, delegate(ComplianceMessage item, byte value)
			{
				item.ComplianceMessageType = (ComplianceMessageType)value;
			});
			ComplianceMessage.description.RegisterIntegerPropertyGetterAndSetter(0, (ComplianceMessage item) => (int)item.WorkDefinitionType, delegate(ComplianceMessage item, int value)
			{
				item.WorkDefinitionType = (WorkDefinitionType)value;
			});
			ComplianceMessage.description.RegisterGuidPropertyGetterAndSetter(0, (ComplianceMessage item) => item.CorrelationId, delegate(ComplianceMessage item, Guid value)
			{
				item.CorrelationId = value;
			});
			ComplianceMessage.description.RegisterStringPropertyGetterAndSetter(0, (ComplianceMessage item) => item.MessageId, delegate(ComplianceMessage item, string value)
			{
				item.MessageId = value;
			});
			ComplianceMessage.description.RegisterStringPropertyGetterAndSetter(1, (ComplianceMessage item) => item.MessageSourceId, delegate(ComplianceMessage item, string value)
			{
				item.MessageSourceId = value;
			});
			ComplianceMessage.description.RegisterStringPropertyGetterAndSetter(2, (ComplianceMessage item) => item.Culture, delegate(ComplianceMessage item, string value)
			{
				item.Culture = value;
			});
			ComplianceMessage.description.RegisterComplexPropertyAsBlobGetterAndSetter<Target>(0, (ComplianceMessage item) => item.MessageTarget, delegate(ComplianceMessage item, Target value)
			{
				item.MessageTarget = value;
			}, Target.Description);
			ComplianceMessage.description.RegisterComplexPropertyAsBlobGetterAndSetter<Target>(1, (ComplianceMessage item) => item.MessageSource, delegate(ComplianceMessage item, Target value)
			{
				item.MessageSource = value;
			}, Target.Description);
			ComplianceMessage.description.RegisterBlobPropertyGetterAndSetter(2, (ComplianceMessage item) => item.Payload, delegate(ComplianceMessage item, byte[] value)
			{
				item.Payload = value;
			});
			ComplianceMessage.description.RegisterBlobPropertyGetterAndSetter(3, (ComplianceMessage item) => item.TenantId, delegate(ComplianceMessage item, byte[] value)
			{
				item.TenantId = value;
			});
		}

		public static ComplianceSerializationDescription<ComplianceMessage> Description
		{
			get
			{
				return ComplianceMessage.description;
			}
		}

		public ComplianceMessageType ComplianceMessageType { get; set; }

		public WorkDefinitionType WorkDefinitionType { get; set; }

		public Guid CorrelationId { get; set; }

		public string MessageId { get; set; }

		public string MessageSourceId { get; set; }

		public byte[] TenantId { get; set; }

		public Target MessageTarget { get; set; }

		public Target MessageSource { get; set; }

		public string Culture { get; set; }

		public byte[] Payload { get; set; }

		internal ProtocolContext ProtocolContext
		{
			get
			{
				return this.protocolContext;
			}
		}

		public ComplianceMessage Clone()
		{
			ComplianceMessage complianceMessage = (ComplianceMessage)base.MemberwiseClone();
			complianceMessage.protocolContext = new ProtocolContext();
			return complianceMessage;
		}

		private static ComplianceSerializationDescription<ComplianceMessage> description = new ComplianceSerializationDescription<ComplianceMessage>();

		private ProtocolContext protocolContext = new ProtocolContext();
	}
}
