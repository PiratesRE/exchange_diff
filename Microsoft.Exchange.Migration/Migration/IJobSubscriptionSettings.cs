using System;
using Microsoft.Exchange.Data.Storage.Management;

namespace Microsoft.Exchange.Migration
{
	internal interface IJobSubscriptionSettings : ISubscriptionSettings, IMigrationSerializable
	{
		void WriteToBatch(MigrationBatch batch);

		void WriteExtendedProperties(PersistableDictionary dictionary);

		bool ReadExtendedProperties(PersistableDictionary dictionary);
	}
}
