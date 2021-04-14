using System;

namespace Microsoft.Exchange.Search.Core.Abstraction
{
	internal interface IFastDocument
	{
		int AttemptCount { get; set; }

		string CompositeItemId { get; set; }

		Guid CorrelationId { get; }

		long DocumentId { get; set; }

		int ErrorCode { get; set; }

		string ErrorMessage { get; set; }

		int FeedingVersion { get; set; }

		string FlowOperation { get; set; }

		string FolderId { get; set; }

		long IndexId { get; set; }

		string IndexSystemName { get; set; }

		string InstanceName { get; set; }

		bool IsLocalMdb { get; set; }

		bool IsMoveDestination { get; set; }

		Guid MailboxGuid { get; set; }

		int Port { get; set; }

		int MessageFlags { get; set; }

		Guid TenantId { get; set; }

		string TransportContextId { get; set; }

		long Watermark { get; set; }
	}
}
