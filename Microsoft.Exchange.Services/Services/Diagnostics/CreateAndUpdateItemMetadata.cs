using System;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;

namespace Microsoft.Exchange.Services.Diagnostics
{
	public enum CreateAndUpdateItemMetadata
	{
		[DisplayName("CUI", "MD")]
		MessageDisposition,
		[DisplayName("CUI", "TNR")]
		TotalNbRecipients,
		[DisplayName("CUI", "TBS")]
		TotalBodySize,
		[DisplayName("CUI", "TNM")]
		TotalNbMessages,
		[DisplayName("CUI", "GetRI")]
		GetMailboxItemResponseObjectInformation,
		[DisplayName("CUI", "PI")]
		PrepareItem,
		[DisplayName("CUI", "GCIB.T")]
		GetCalendarItemBaseTime,
		[DisplayName("CUI", "GCIBRPC.L")]
		GetCalendarItemBaseRpcLatency,
		[DisplayName("CUI", "GCIBRPC.C")]
		GetCalendarItemBaseRpcCount,
		[DisplayName("CUI", "SAV")]
		Save,
		[DisplayName("CUI", "TM")]
		TotalMeetings,
		[DisplayName("CUI", "RM.T")]
		RespondToMeetingRequestTime,
		[DisplayName("CUI", "RMRPC.C")]
		RespondToMeetingRequestRpcCount,
		[DisplayName("CUI", "RMRPC.L")]
		RespondToMeetingRequestRpcLatency,
		[DisplayName("CUI", "RCB.T")]
		RespondToCalendarItemBaseTime,
		[DisplayName("CUI", "RCBRPC.C")]
		RespondToCalendarItemBaseRpcCount,
		[DisplayName("CUI", "RCBRPC.L")]
		RespondToCalendarItemBaseRpcLatency,
		[DisplayName("CUI", "UPD.T")]
		UpdateMeetingTime,
		[DisplayName("CUI", "UPDRPC.C")]
		UpdateMeetingRpcCount,
		[DisplayName("CUI", "UPDRPC.L")]
		UpdateMeetingRpcLatency,
		[DisplayName("CUI", "LDI.T")]
		LoadAndDeleteItemTime,
		[DisplayName("CUI", "LDIRPC.C")]
		LoadAndDeleteItemRpcCount,
		[DisplayName("CUI", "LDIRPC.L")]
		LoadAndDeleteItemRpcLatency,
		[DisplayName("CUI", "MI.T")]
		MoveItemTime,
		[DisplayName("CUI", "MIRPC.C")]
		MoveItemRpcCount,
		[DisplayName("CUI", "MIRPC.L")]
		MoveItemRpcLatency,
		[DisplayName("CUI", "ACT")]
		ActionType,
		[DisplayName("CUI", "ST")]
		SessionType,
		[DisplayName("CUI", "PRIP")]
		Principal,
		[DisplayName("CUI", "CO")]
		ComposeOperation
	}
}
