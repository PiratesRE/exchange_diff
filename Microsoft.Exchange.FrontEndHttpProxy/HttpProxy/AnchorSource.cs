using System;

namespace Microsoft.Exchange.HttpProxy
{
	internal enum AnchorSource
	{
		Smtp,
		Sid,
		Domain,
		DomainAndVersion,
		OrganizationId,
		MailboxGuid,
		DatabaseName,
		DatabaseGuid,
		UserADRawEntry,
		ServerInfo,
		ServerVersion,
		Url,
		Anonymous,
		GenericAnchorHint,
		Puid,
		ExternalDirectoryObjectId,
		OAuthActAsUser,
		LiveIdMemberName
	}
}
