using System;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public class UMAutoAttendantIdParameter : ADIdParameter
	{
		public UMAutoAttendantIdParameter()
		{
		}

		public UMAutoAttendantIdParameter(string identity) : base(identity)
		{
		}

		public UMAutoAttendantIdParameter(INamedIdentity namedIdentity) : base(namedIdentity)
		{
		}

		public UMAutoAttendantIdParameter(ADObjectId adObjectId) : base(adObjectId)
		{
		}

		public static UMAutoAttendantIdParameter Parse(string identity)
		{
			return new UMAutoAttendantIdParameter(identity);
		}
	}
}
