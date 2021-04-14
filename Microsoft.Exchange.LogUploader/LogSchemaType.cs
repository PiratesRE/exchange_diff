using System;

namespace Microsoft.Exchange.LogUploader
{
	internal enum LogSchemaType
	{
		MsgTrackingLogSchema,
		TenantSettingSyncLogSchema,
		TransportQueueLogSchema,
		AsyncQueueLogSchema,
		SpamDigestLogSchema,
		SpamEngineOpticsLogSchema,
		CustomizedSchema
	}
}
