using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.HA.DirectoryServices
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IADDatabase : IADObjectCommon
	{
		ReplicationType ReplicationType { get; }

		EdbFilePath EdbFilePath { get; }

		NonRootLocalLongFullPath LogFolderPath { get; }

		NonRootLocalLongFullPath SystemFolderPath { get; }

		bool Recovery { get; }

		bool AutoDagExcludeFromMonitoring { get; }

		ADObjectId HostServerForPreference1 { get; }

		IADDatabaseCopy[] DatabaseCopies { get; }

		IADDatabaseCopy[] AllDatabaseCopies { get; }

		IADDatabaseCopy GetDatabaseCopy(string serverShortName);

		void ExcludeDatabaseCopyFromProperties(string hostServerToExclude);

		ADObjectId Server { get; }

		ADObjectId[] Servers { get; }

		bool MountAtStartup { get; }

		bool DatabaseCreated { get; }

		bool AllowFileRestore { get; }

		string DistinguishedName { get; }

		string LogFilePrefix { get; }

		bool IsPublicFolderDatabase { get; }

		bool IsMailboxDatabase { get; }

		bool CircularLoggingEnabled { get; }

		string ExchangeLegacyDN { get; }

		string RpcClientAccessServerLegacyDN { get; }

		ADObjectId MailboxPublicFolderDatabase { get; }

		bool IsExchange2009OrLater { get; }

		ADObjectId MasterServerOrAvailabilityGroup { get; }

		string DatabaseGroup { get; }
	}
}
