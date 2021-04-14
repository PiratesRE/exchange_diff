using System;

namespace Microsoft.Exchange.Server.Storage.Common
{
	public class ValueReference
	{
		public bool IsZero
		{
			get
			{
				return object.ReferenceEquals(this, ValueReference.Zero);
			}
		}

		public static readonly ValueReference Zero = new ValueReference();
	}
}
