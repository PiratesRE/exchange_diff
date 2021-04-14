using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal interface IValidator
	{
		bool Validate(DefaultFolderContext context, PropertyBag propertyBag);

		void SetProperties(DefaultFolderContext context, Folder folder);
	}
}
