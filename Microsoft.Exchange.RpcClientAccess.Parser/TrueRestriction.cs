using System;

namespace Microsoft.Exchange.RpcClientAccess
{
	internal sealed class TrueRestriction : Restriction
	{
		internal TrueRestriction()
		{
		}

		internal override RestrictionType RestrictionType
		{
			get
			{
				return RestrictionType.True;
			}
		}

		internal new static TrueRestriction InternalParse(Reader reader, WireFormatStyle wireFormatStyle, uint depth)
		{
			return new TrueRestriction();
		}
	}
}
