using System;

namespace Microsoft.Exchange.Hygiene.Data.MessageTrace
{
	internal class MessageClientInformationSchema
	{
		internal static readonly HygienePropertyDefinition ClientInformationIdProperty = new HygienePropertyDefinition("ClientInformationId", typeof(Guid));

		internal static readonly HygienePropertyDefinition DataClassificationIdProperty = new HygienePropertyDefinition("DataClassificationId", typeof(Guid));

		internal static readonly HygienePropertyDefinition ExMessageIdProperty = CommonMessageTraceSchema.ExMessageIdProperty;
	}
}
