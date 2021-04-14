using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.HA.DirectoryServices
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IADObjectCommon
	{
		ADObjectId Id { get; }

		ObjectId Identity { get; }

		Guid Guid { get; }

		string Name { get; }

		bool IsValid { get; }

		void Minimize();
	}
}
