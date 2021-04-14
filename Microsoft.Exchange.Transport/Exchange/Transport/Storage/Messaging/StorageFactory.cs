using System;
using Microsoft.Exchange.Transport.Storage.Messaging.Null;
using Microsoft.Exchange.Transport.Storage.Messaging.Utah;

namespace Microsoft.Exchange.Transport.Storage.Messaging
{
	internal class StorageFactory
	{
		public static IMessagingDatabase GetNewDatabaseInstance()
		{
			switch (StorageFactory.SchemaToUse)
			{
			case StorageFactory.Schema.NullSchema:
				return new Microsoft.Exchange.Transport.Storage.Messaging.Null.MessagingDatabase();
			case StorageFactory.Schema.UtahSchema:
				return new Microsoft.Exchange.Transport.Storage.Messaging.Utah.MessagingDatabase();
			}
			return null;
		}

		public static IBootLoader CreateBootScanner()
		{
			StorageFactory.Schema schemaToUse = StorageFactory.SchemaToUse;
			if (schemaToUse == StorageFactory.Schema.UtahSchema)
			{
				return new BootScanner();
			}
			return null;
		}

		public static readonly StorageFactory.Schema DefaultSchema = TransportAppConfig.GetConfigEnum<StorageFactory.Schema>("QueueDatabaseSchema", StorageFactory.Schema.UtahSchema);

		public static StorageFactory.Schema SchemaToUse = StorageFactory.DefaultSchema;

		public enum Schema
		{
			NullSchema,
			UtahSchema = 2
		}
	}
}
