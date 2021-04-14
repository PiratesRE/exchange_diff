using System;

namespace Microsoft.Exchange.RpcClientAccess
{
	internal sealed class AndRestriction : CompositeRestriction
	{
		internal AndRestriction(Restriction[] childRestrictions) : base(childRestrictions)
		{
		}

		internal override RestrictionType RestrictionType
		{
			get
			{
				return RestrictionType.And;
			}
		}

		internal new static AndRestriction InternalParse(Reader reader, WireFormatStyle wireFormatStyle, uint depth)
		{
			return CompositeRestriction.InternalParse<AndRestriction>(reader, (Restriction[] childRestrictions) => new AndRestriction(childRestrictions), wireFormatStyle, depth);
		}
	}
}
