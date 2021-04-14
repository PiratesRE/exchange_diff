using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal enum DataMessageOpcode
	{
		None,
		MapiFxConfig,
		MapiFxTransferBuffer,
		MapiFxIsInterfaceOk,
		MapiFxTellPartnerVersion,
		MapiFxStartMdbEventsImport = 11,
		MapiFxFinishMdbEventsImport,
		MapiFxAddMdbEvents,
		MapiFxSetWatermarks,
		MapiFxSetReceiveFolder,
		MapiFxSetPerUser,
		MapiFxSetProps,
		FxProxyPoolOpenFolder = 100,
		FxProxyPoolCloseEntry,
		FxProxyPoolOpenItem,
		FxProxyPoolCreateItem,
		FxProxyPoolCreateFAIItem,
		FxProxyPoolSetProps,
		FxProxyPoolSaveChanges,
		FxProxyPoolDeleteItem,
		FxProxyPoolWriteToMime,
		FxProxyPoolCreateFolder,
		FxProxyPoolSetExtendedAcl,
		FxProxyPoolSetItemProperties,
		BufferBatch = 200,
		BufferBatchWithFlush,
		PagedDataChunk = 210,
		PagedLastDataChunk,
		MessageExportResults = 220,
		Flush = 1000,
		FxProxyGetObjectDataRequest,
		FxProxyGetObjectDataResponse,
		FxProxyPoolGetFolderDataRequest,
		FxProxyPoolGetFolderDataResponse,
		FxProxyPoolGetUploadedIDsRequest,
		FxProxyPoolGetUploadedIDsResponse
	}
}
