using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal interface IRequestIndexEntry : IConfigurable
	{
		Guid RequestGuid { get; set; }

		string Name { get; set; }

		RequestStatus Status { get; set; }

		RequestFlags Flags { get; set; }

		string RemoteHostName { get; set; }

		string BatchName { get; set; }

		ADObjectId SourceMDB { get; set; }

		ADObjectId TargetMDB { get; set; }

		ADObjectId StorageMDB { get; set; }

		string FilePath { get; set; }

		MRSRequestType Type { get; set; }

		ADObjectId TargetUserId { get; set; }

		ADObjectId SourceUserId { get; set; }

		OrganizationId OrganizationId { get; }

		DateTime? WhenChanged { get; }

		DateTime? WhenCreated { get; }

		DateTime? WhenChangedUTC { get; }

		DateTime? WhenCreatedUTC { get; }

		RequestIndexId RequestIndexId { get; }

		RequestJobObjectId GetRequestJobId();

		RequestIndexEntryObjectId GetRequestIndexEntryId(RequestBase owner);
	}
}
