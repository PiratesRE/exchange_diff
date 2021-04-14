using System;

namespace Microsoft.Exchange.RpcClientAccess
{
	internal sealed class FalseRestriction : Restriction
	{
		internal FalseRestriction()
		{
		}

		internal override RestrictionType RestrictionType
		{
			get
			{
				return RestrictionType.False;
			}
		}

		internal new static FalseRestriction InternalParse(Reader reader, WireFormatStyle wireFormatStyle, uint depth)
		{
			return new FalseRestriction();
		}
	}
}
