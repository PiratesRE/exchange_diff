using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Management
{
	internal class MigrationObjectLog<T, TSchema, TConfig> : ObjectLog<T> where TSchema : ObjectLogSchema, new() where TConfig : ObjectLogConfiguration, new()
	{
		internal MigrationObjectLog() : base(Activator.CreateInstance<TSchema>(), Activator.CreateInstance<TConfig>())
		{
		}

		protected static void Write(T migrationObject)
		{
			MigrationObjectLog<T, TSchema, TConfig>.instance.LogObject(migrationObject);
		}

		private static MigrationObjectLog<T, TSchema, TConfig> instance = new MigrationObjectLog<T, TSchema, TConfig>();
	}
}
