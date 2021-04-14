using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess
{
	internal abstract class SingleRestriction : Restriction
	{
		internal SingleRestriction(Restriction childRestriction)
		{
			this.childRestriction = childRestriction;
		}

		public Restriction ChildRestriction
		{
			get
			{
				return this.childRestriction;
			}
		}

		internal override void ResolveString8Values(Encoding string8Encoding)
		{
			base.ResolveString8Values(string8Encoding);
			if (this.childRestriction != null)
			{
				this.childRestriction.ResolveString8Values(string8Encoding);
			}
		}

		private readonly Restriction childRestriction;
	}
}
