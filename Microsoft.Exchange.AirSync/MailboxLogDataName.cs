using System;

namespace Microsoft.Exchange.AirSync
{
	internal enum MailboxLogDataName
	{
		AdditionalData,
		RequestHeader,
		RequestBody,
		RequestTime,
		ResponseHeader,
		ResponseBody,
		ResponseTime,
		Command_WorkerThread_Exception,
		SyncCommand_ConvertRequestsAndApply_Add_Exception,
		SyncCommand_ConvertRequestsAndApply_Change_RejectClientChange_Exception,
		SyncCommand_ConvertRequestsAndApply_Delete_Exception,
		SyncCommand_GenerateResponsesXmlNode_Fetch_Exception,
		SyncCommand_GenerateResponsesXmlNode_AddChange_ConvertServerToClientObject_Exception,
		SyncCommand_GenerateResponsesXmlNode_AddChange_Exception,
		SyncCommand_OnExecute_Exception,
		MailboxSyncCommand_HasSchemaPropertyChanged_Exception,
		MeetingResponseCommand_OnExecute_Exception,
		GetItemEstimateCommand_OnExecute_Exception,
		PingCommand_Consume_Exception,
		PingCommand__ItemChangesSinceLastSync_Exception,
		SyncCommand_ConvertRequestsAndApply_Change_AcceptClientChange_Exception,
		ItemOperationsCommand_Execute_Fetch_Exception,
		SearchCommand_Execute_Exception,
		Identifier,
		LogicalRequest,
		WasPending,
		ServerName,
		AssemblyVersion,
		ValidateCertCommand_ProcessCommand_Per_Cert_Exception,
		ValidateCertCommand_ProcessCommand_Exception,
		AccessState,
		AccessStateReason,
		DeviceAccessControlRule,
		SyncCollection_VerifySyncKey_Exception,
		IRM_FailureCode,
		IRM_Exception,
		CalendarSync_Exception
	}
}
