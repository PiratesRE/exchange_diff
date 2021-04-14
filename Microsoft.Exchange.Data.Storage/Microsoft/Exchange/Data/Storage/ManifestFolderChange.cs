using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class ManifestFolderChange : ManifestChangeBase
	{
		internal ManifestFolderChange(PropValue[] propertyValues)
		{
			this.propertyValues = propertyValues;
		}

		public PropValue[] PropertyValues
		{
			get
			{
				return this.propertyValues;
			}
		}

		private readonly PropValue[] propertyValues;
	}
}
