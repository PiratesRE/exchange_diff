using System;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;

namespace Microsoft.Exchange.Clients.Owa2.Server.Diagnostics
{
	public enum WacRequestHandlerMetadata
	{
		[DisplayName("WRH", "RT")]
		RequestType,
		[DisplayName("WRH", "ST")]
		SessionElapsedTime,
		[DisplayName("WRH", "LT")]
		LockWaitTime,
		[DisplayName("WRH", "CT")]
		CobaltTime,
		[DisplayName("WRH", "RQL")]
		CobaltRequestLength,
		[DisplayName("WRH", "RSL")]
		CobaltResponseLength,
		[DisplayName("WRH", "CH")]
		CacheHit,
		[DisplayName("WRH", "U")]
		Updated,
		[DisplayName("WRH", "CO")]
		CobaltOperations,
		[DisplayName("WRH", "CR")]
		CobaltReads,
		[DisplayName("WRH", "CBR")]
		CobaltBytesRead,
		[DisplayName("WRH", "CW")]
		CobaltWrites,
		[DisplayName("WRH", "CBW")]
		CobaltBytesWritten,
		[DisplayName("WRH", "DBC")]
		DiskBlobCount,
		[DisplayName("WRH", "DBS")]
		DiskBlobSize,
		[DisplayName("WRH", "WSN")]
		WopiServerName,
		[DisplayName("WRH", "WCV")]
		WopiClientVersion,
		[DisplayName("WRH", "WCID")]
		WopiCorrelationId,
		[DisplayName("WRH", "ESID")]
		ExchangeSessionId,
		[DisplayName("WRH", "UA")]
		UserAgent,
		[DisplayName("WRH", "URL")]
		RequestUrl,
		[DisplayName("WRH", "MCT")]
		MdbCacheReloadTime,
		[DisplayName("WRH", "MCS")]
		MdbCacheSize,
		[DisplayName("WRH", "ED")]
		ErrorDetails
	}
}
