using System;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public class OutlookProviderIdParameter : ADIdParameter
	{
		public OutlookProviderIdParameter(ADObjectId objectId) : base(objectId)
		{
		}

		public OutlookProviderIdParameter(string identity) : base(identity)
		{
		}

		public OutlookProviderIdParameter()
		{
		}

		public OutlookProviderIdParameter(INamedIdentity namedIdentity) : base(namedIdentity)
		{
		}

		public static OutlookProviderIdParameter Parse(string rawString)
		{
			return new OutlookProviderIdParameter(rawString);
		}
	}
}
