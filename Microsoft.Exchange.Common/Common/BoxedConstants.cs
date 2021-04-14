using System;

namespace Microsoft.Exchange.Common
{
	public sealed class BoxedConstants
	{
		public static object GetBool(bool value)
		{
			if (!value)
			{
				return BoxedConstants.False;
			}
			return BoxedConstants.True;
		}

		public static object GetBool(bool? value)
		{
			if (value == null)
			{
				return null;
			}
			if (!value.Value)
			{
				return BoxedConstants.False;
			}
			return BoxedConstants.True;
		}

		public static readonly object True = true;

		public static readonly object False = false;
	}
}
