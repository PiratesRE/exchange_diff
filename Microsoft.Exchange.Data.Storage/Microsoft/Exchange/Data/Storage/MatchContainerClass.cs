using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class MatchContainerClass : IValidator
	{
		internal MatchContainerClass(string containerClass)
		{
			this.containerClass = containerClass;
		}

		public virtual bool Validate(DefaultFolderContext context, PropertyBag propertyBag)
		{
			return propertyBag.TryGetProperty(InternalSchema.ContainerClass) as string == this.containerClass;
		}

		public void SetProperties(DefaultFolderContext context, Folder folder)
		{
			folder[InternalSchema.ContainerClass] = this.containerClass;
		}

		private string containerClass;
	}
}
