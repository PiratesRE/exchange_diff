using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Infoworker.MailboxSearch
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IDiscoverySearchDataProvider
	{
		OrganizationId OrganizationId { get; }

		string DisplayName { get; }

		string DistinguishedName { get; }

		string PrimarySmtpAddress { get; }

		Guid ObjectGuid { get; }

		IEnumerable<T> GetAll<T>() where T : DiscoverySearchBase, new();

		T Find<T>(string searchId) where T : DiscoverySearchBase, new();

		void CreateOrUpdate<T>(T discoverySearch) where T : DiscoverySearchBase;

		void Delete<T>(string searchId) where T : DiscoverySearchBase, new();
	}
}
