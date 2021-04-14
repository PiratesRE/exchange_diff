using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage.Configuration;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IAggregatedUserConfigurationReader
	{
		IReadableUserConfiguration Read(IMailboxSession session, UserConfigurationDescriptor descriptor);

		bool TryRead<T>(string key, out T result) where T : SerializableDataBase;

		void Validate(IUserConfigurationManager manager, IXSOFactory xsoFactory, IAggregationReValidator validator, Action<IEnumerable<UserConfigurationDescriptor.MementoClass>, IEnumerable<string>> callback);
	}
}
