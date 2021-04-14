using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class MatchIsHidden : IValidator
	{
		internal MatchIsHidden(bool isHidden)
		{
			this.isHidden = isHidden;
		}

		public bool Validate(DefaultFolderContext context, PropertyBag propertyBag)
		{
			bool? valueAsNullable = propertyBag.GetValueAsNullable<bool>(InternalSchema.IsHidden);
			return valueAsNullable != null && valueAsNullable.Value == this.isHidden;
		}

		public void SetProperties(DefaultFolderContext context, Folder folder)
		{
			folder[InternalSchema.IsHidden] = this.isHidden;
		}

		private bool isHidden;
	}
}
