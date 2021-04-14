using System;

namespace Microsoft.Exchange.Data.Directory.Management
{
	internal abstract class ADPresentationSchema : ADObjectSchema
	{
		internal ADObjectSchema ParentSchema { get; private set; }

		internal abstract ADObjectSchema GetParentSchema();

		public ADPresentationSchema()
		{
			this.ParentSchema = this.GetParentSchema();
		}
	}
}
