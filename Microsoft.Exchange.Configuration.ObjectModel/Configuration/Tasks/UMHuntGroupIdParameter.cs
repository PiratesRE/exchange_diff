using System;
using System.Collections.Generic;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public class UMHuntGroupIdParameter : ADIdParameter
	{
		public UMHuntGroupIdParameter()
		{
		}

		public UMHuntGroupIdParameter(string identity) : base(identity)
		{
			this.Initialize(identity);
		}

		public UMHuntGroupIdParameter(INamedIdentity namedIdentity) : base(namedIdentity)
		{
		}

		public UMHuntGroupIdParameter(ADObjectId adObjectId) : base(adObjectId)
		{
		}

		public static UMHuntGroupIdParameter Parse(string identity)
		{
			return new UMHuntGroupIdParameter(identity);
		}

		internal override IEnumerable<T> GetObjects<T>(ADObjectId rootId, IDirectorySession session, IDirectorySession subTreeSession, OptionalIdentityData optionalData, out LocalizedString? notFoundReason)
		{
			if (base.InternalADObjectId != null)
			{
				return base.GetObjects<T>(rootId, session, subTreeSession, optionalData, out notFoundReason);
			}
			List<T> list = new List<T>(5);
			notFoundReason = null;
			if (this.gatewayIdentity != null)
			{
				UMIPGatewayIdParameter umipgatewayIdParameter = new UMIPGatewayIdParameter(this.gatewayIdentity);
				IEnumerable<UMIPGateway> objects = umipgatewayIdParameter.GetObjects<UMIPGateway>(rootId, session, subTreeSession, optionalData, out notFoundReason);
				if (objects != null)
				{
					foreach (UMIPGateway umipgateway in objects)
					{
						IEnumerable<T> objectsInOrganization = base.GetObjectsInOrganization<T>(this.huntGroupId, umipgateway.Id, subTreeSession, optionalData);
						if (objectsInOrganization != null)
						{
							foreach (T item in objectsInOrganization)
							{
								list.Add(item);
							}
						}
					}
				}
			}
			return list;
		}

		private void Initialize(string identity)
		{
			if (base.InternalADObjectId != null)
			{
				if (!(base.InternalADObjectId.Rdn != null))
				{
					Guid objectGuid = base.InternalADObjectId.ObjectGuid;
				}
				return;
			}
			int num = identity.LastIndexOf('\\');
			if (num == -1 || num == 0 || num == identity.Length - 1)
			{
				throw new ArgumentException(Strings.ErrorInvalidUMHuntGroupIdentity(identity));
			}
			this.gatewayIdentity = identity.Substring(0, num);
			this.huntGroupId = identity.Substring(num + 1, identity.Length - num - 1);
		}

		private string gatewayIdentity;

		private string huntGroupId;
	}
}
