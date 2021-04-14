using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class MatchContainerClassOrThrow : MatchContainerClass
	{
		internal MatchContainerClassOrThrow(string containerClass) : base(containerClass)
		{
		}

		public override bool Validate(DefaultFolderContext context, PropertyBag propertyBag)
		{
			if (!base.Validate(context, propertyBag))
			{
				throw new DefaultFolderPropertyValidationException(ServerStrings.MatchContainerClassValidationFailed);
			}
			return true;
		}
	}
}
