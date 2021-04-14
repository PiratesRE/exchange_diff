using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class RightsManagedMessageItemSchema : MessageItemSchema
	{
		private RightsManagedMessageItemSchema()
		{
		}

		public new static RightsManagedMessageItemSchema Instance
		{
			get
			{
				if (RightsManagedMessageItemSchema.instance == null)
				{
					RightsManagedMessageItemSchema.instance = new RightsManagedMessageItemSchema();
				}
				return RightsManagedMessageItemSchema.instance;
			}
		}

		private static RightsManagedMessageItemSchema instance;
	}
}
