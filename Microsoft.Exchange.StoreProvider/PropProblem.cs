using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi.Unmanaged;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal struct PropProblem
	{
		public int Index
		{
			get
			{
				return this.index;
			}
		}

		public PropTag PropTag
		{
			get
			{
				return this.propTag;
			}
		}

		public int Scode
		{
			get
			{
				return this.scode;
			}
		}

		public PropType PropType
		{
			get
			{
				return (PropType)(this.propTag & (PropTag)65535U);
			}
		}

		public PropProblem(int index, PropTag propTag, int scode)
		{
			this.index = index;
			this.propTag = propTag;
			this.scode = scode;
		}

		internal unsafe PropProblem(SPropProblem* pspp)
		{
			this.index = pspp->ulIndex;
			this.propTag = (PropTag)pspp->ulPropTag;
			this.scode = pspp->scode;
		}

		private readonly int index;

		private readonly PropTag propTag;

		private readonly int scode;
	}
}
