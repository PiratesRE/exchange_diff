using System;
using Microsoft.Exchange.Data.ContentTypes.Tnef;

namespace Microsoft.Exchange.Data.Transport.Email
{
	internal struct TnefNameTag
	{
		public TnefNameTag(TnefNameId id, TnefPropertyType type)
		{
			this.id = id;
			this.type = type;
		}

		public TnefNameId Id
		{
			get
			{
				return this.id;
			}
		}

		public TnefPropertyType Type
		{
			get
			{
				return this.type;
			}
		}

		private TnefNameId id;

		private TnefPropertyType type;
	}
}
