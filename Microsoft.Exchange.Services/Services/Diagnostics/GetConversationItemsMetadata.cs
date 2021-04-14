using System;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;

namespace Microsoft.Exchange.Services.Diagnostics
{
	internal enum GetConversationItemsMetadata
	{
		[DisplayName("GCI", "ID")]
		ConversationId,
		[DisplayName("GCI", "NC")]
		TotalNodeCount,
		[DisplayName("GCI", "LC")]
		LeafNodeCount,
		[DisplayName("GCI", "IE")]
		ItemsExtracted,
		[DisplayName("GCI", "IO")]
		ItemsOpened,
		[DisplayName("GCI", "S")]
		SummariesConstructed,
		[DisplayName("GCI", "TC")]
		BodyTagMatchingAttemptsCount,
		[DisplayName("GCI", "TI")]
		BodyTagMatchingIssuesCount,
		[DisplayName("GCI", "TNP")]
		BodyTagNotPresentCount,
		[DisplayName("GCI", "TM")]
		BodyTagMismatchedCount,
		[DisplayName("GCI", "FM")]
		BodyFormatMismatchedCount,
		[DisplayName("GCI", "NMSH")]
		NonMSHeaderCount,
		[DisplayName("GCI", "EPN")]
		ExtraPropertiesNeededCount,
		[DisplayName("GCI", "PNF")]
		ParticipantNotFoundCount,
		[DisplayName("GCI", "A")]
		AttachmentPresentCount,
		[DisplayName("GCI", "AM")]
		MapiAttachmentPresentCount,
		[DisplayName("GCI", "IL")]
		PossibleInlinesCount,
		[DisplayName("GCI", "IRM")]
		IrmProtectedCount,
		[DisplayName("GCI", "SS")]
		SyncState
	}
}
