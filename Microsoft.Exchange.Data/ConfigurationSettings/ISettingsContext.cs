using System;

namespace Microsoft.Exchange.Data.ConfigurationSettings
{
	public interface ISettingsContext
	{
		Guid? ServerGuid { get; }

		string ServerName { get; }

		ServerVersion ServerVersion { get; }

		string ServerRole { get; }

		string ProcessName { get; }

		Guid? DagOrServerGuid { get; }

		Guid? DatabaseGuid { get; }

		string DatabaseName { get; }

		ServerVersion DatabaseVersion { get; }

		string OrganizationName { get; }

		ExchangeObjectVersion OrganizationVersion { get; }

		Guid? MailboxGuid { get; }

		string GetGenericProperty(string propertyName);
	}
}
