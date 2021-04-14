using System;
using System.Collections;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	[AttributeUsage(AttributeTargets.Enum, AllowMultiple = false)]
	public sealed class OwaEventEnumAttribute : Attribute
	{
		public OwaEventEnumAttribute()
		{
			this.enumValueTable = new Hashtable();
		}

		internal void AddValueInfo(int intValue, object enumValue)
		{
			this.enumValueTable[intValue] = enumValue;
		}

		internal object FindValueInfo(int intValue)
		{
			return this.enumValueTable[intValue];
		}

		private Hashtable enumValueTable;
	}
}
