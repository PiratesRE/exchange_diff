using System;

namespace Microsoft.Exchange.RpcClientAccess
{
	internal sealed class NullRestriction : Restriction
	{
		private NullRestriction()
		{
		}

		internal override RestrictionType RestrictionType
		{
			get
			{
				return RestrictionType.Null;
			}
		}

		internal new static NullRestriction InternalParse(Reader reader, WireFormatStyle wireFormatStyle, uint depth)
		{
			return NullRestriction.Instance;
		}

		public static NullRestriction Instance
		{
			get
			{
				return NullRestriction.instance;
			}
		}

		private static readonly NullRestriction instance = new NullRestriction();
	}
}
