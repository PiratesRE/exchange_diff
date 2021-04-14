using System;

namespace Microsoft.Exchange.RpcClientAccess
{
	internal sealed class OrRestriction : CompositeRestriction
	{
		internal OrRestriction(Restriction[] childRestrictions) : base(childRestrictions)
		{
		}

		internal override RestrictionType RestrictionType
		{
			get
			{
				return RestrictionType.Or;
			}
		}

		internal new static OrRestriction InternalParse(Reader reader, WireFormatStyle wireFormatStyle, uint depth)
		{
			return CompositeRestriction.InternalParse<OrRestriction>(reader, (Restriction[] childRestrictions) => new OrRestriction(childRestrictions), wireFormatStyle, depth);
		}
	}
}
