using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IUserConfiguration : IReadableUserConfiguration, IDisposable
	{
		void Save();

		ConflictResolutionResult Save(SaveMode saveMode);

		ConfigurationDictionary GetConfigurationDictionary();
	}
}
