using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public class PartnerApplicationIdParameter : ADIdParameter
	{
		public PartnerApplicationIdParameter()
		{
		}

		public PartnerApplicationIdParameter(string identity) : base(identity)
		{
		}

		public PartnerApplicationIdParameter(ADObjectId adObjectId) : base(adObjectId)
		{
		}

		public PartnerApplicationIdParameter(PartnerApplication app) : base(app.Id)
		{
		}

		public PartnerApplicationIdParameter(INamedIdentity namedIdentity) : base(namedIdentity)
		{
		}

		public static PartnerApplicationIdParameter Parse(string identity)
		{
			return new PartnerApplicationIdParameter(identity);
		}

		internal override IEnumerable<T> GetObjectsInOrganization<T>(string identityString, ADObjectId rootId, IDirectorySession session, OptionalIdentityData optionalData)
		{
			bool isRedirectedToSharedConfig = session.SessionSettings.IsRedirectedToSharedConfig;
			IEnumerable<T> objectsInOrganization;
			try
			{
				session.SessionSettings.IsRedirectedToSharedConfig = false;
				objectsInOrganization = base.GetObjectsInOrganization<T>(identityString, rootId, session, optionalData);
			}
			finally
			{
				session.SessionSettings.IsRedirectedToSharedConfig = isRedirectedToSharedConfig;
			}
			return objectsInOrganization;
		}

		internal override ADPropertyDefinition[] AdditionalMatchingProperties
		{
			get
			{
				return new ADPropertyDefinition[]
				{
					PartnerApplicationSchema.ApplicationIdentifier
				};
			}
		}
	}
}
