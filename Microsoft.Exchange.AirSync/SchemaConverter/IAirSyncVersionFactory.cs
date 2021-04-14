using System;
using System.Collections.Generic;
using Microsoft.Exchange.AirSync.SchemaConverter.AirSync;

namespace Microsoft.Exchange.AirSync.SchemaConverter
{
	internal interface IAirSyncVersionFactory
	{
		string VersionString { get; }

		AirSyncSchemaState CreateCalendarSchema();

		AirSyncSchemaState CreateEmailSchema(IdMapping idMapping);

		AirSyncSchemaState CreateContactsSchema();

		AirSyncSchemaState CreateNotesSchema();

		AirSyncSchemaState CreateSmsSchema();

		AirSyncSchemaState CreateConsumerSmsAndMmsSchema();

		AirSyncSchemaState CreateTasksSchema();

		AirSyncSchemaState CreateRecipientInfoCacheSchema();

		IAirSyncMissingPropertyStrategy CreateMissingPropertyStrategy(Dictionary<string, bool> supportedTags);

		IAirSyncMissingPropertyStrategy CreateReadFlagMissingPropertyStrategy();

		string GetClassFromMessageClass(string messageClass);
	}
}
