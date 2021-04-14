using System;
using System.Collections.Generic;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public class GlobalAddressListIdParameter : ADIdParameter
	{
		public GlobalAddressListIdParameter()
		{
		}

		public GlobalAddressListIdParameter(ADObjectId adObjectId) : base(adObjectId)
		{
		}

		public GlobalAddressListIdParameter(GlobalAddressList globalAddressList) : base(globalAddressList.Id)
		{
		}

		public GlobalAddressListIdParameter(INamedIdentity namedIdentity) : base(namedIdentity)
		{
		}

		protected GlobalAddressListIdParameter(string identity) : base(identity)
		{
		}

		public static GlobalAddressListIdParameter Parse(string identity)
		{
			return new GlobalAddressListIdParameter(identity);
		}

		internal static ADObjectId GetRootContainerId(IConfigurationSession scSession)
		{
			return GlobalAddressListIdParameter.GetRootContainerId(scSession, null);
		}

		internal static ADObjectId GetRootContainerId(IConfigurationSession scSession, OrganizationId currentOrg)
		{
			ADObjectId adobjectId;
			if (currentOrg == null || currentOrg.ConfigurationUnit == null)
			{
				adobjectId = scSession.GetOrgContainerId();
			}
			else
			{
				adobjectId = currentOrg.ConfigurationUnit;
			}
			return adobjectId.GetDescendantId(GlobalAddressList.RdnGalContainerToOrganization);
		}

		internal override IEnumerableFilter<T> GetEnumerableFilter<T>()
		{
			return GlobalAddressListIdParameter.GlobalAddressListFilter<T>.GetInstance();
		}

		internal override IEnumerable<T> GetObjects<T>(ADObjectId rootId, IDirectorySession session, IDirectorySession subTreeSession, OptionalIdentityData optionalData, out LocalizedString? notFoundReason)
		{
			if (typeof(T) != typeof(AddressBookBase))
			{
				throw new ArgumentException(Strings.ErrorInvalidType(typeof(T).Name), "type");
			}
			if (session == null)
			{
				throw new ArgumentNullException("session");
			}
			IConfigurationSession scSession = session as IConfigurationSession;
			if ("\\" == base.RawIdentity)
			{
				notFoundReason = null;
				ADObjectId rootContainerId = GlobalAddressListIdParameter.GetRootContainerId(scSession);
				return EnumerableWrapper<T>.GetWrapper(base.GetADObjectIdObjects<T>(rootContainerId, rootId, subTreeSession, optionalData), this.GetEnumerableFilter<T>());
			}
			return base.GetObjects<T>(rootId, session, subTreeSession, optionalData, out notFoundReason);
		}

		internal override IEnumerable<T> GetObjectsInOrganization<T>(string identityString, ADObjectId rootId, IDirectorySession session, OptionalIdentityData optionalData)
		{
			string[] commonNames = AddressListIdParameter.GetCommonNames(identityString);
			if (commonNames.Length == 1)
			{
				ADObjectId rootContainerId = GlobalAddressListIdParameter.GetRootContainerId((IConfigurationSession)session);
				ADObjectId childId = rootContainerId.GetChildId(commonNames[0]);
				EnumerableWrapper<T> wrapper = EnumerableWrapper<T>.GetWrapper(base.GetADObjectIdObjects<T>(childId, rootId, session, optionalData), this.GetEnumerableFilter<T>());
				if (wrapper.HasElements())
				{
					return wrapper;
				}
			}
			return base.GetObjectsInOrganization<T>(identityString, rootId, session, optionalData);
		}

		private class GlobalAddressListFilter<T> : IEnumerableFilter<T>
		{
			public static GlobalAddressListIdParameter.GlobalAddressListFilter<T> GetInstance()
			{
				return GlobalAddressListIdParameter.GlobalAddressListFilter<T>.globalAddressListFilter;
			}

			public bool AcceptElement(T element)
			{
				AddressBookBase addressBookBase = element as AddressBookBase;
				return addressBookBase != null && addressBookBase.IsGlobalAddressList && !addressBookBase.Id.DistinguishedName.StartsWith(GlobalAddressList.RdnGalContainerToOrganization.DistinguishedName, StringComparison.OrdinalIgnoreCase);
			}

			public override bool Equals(object obj)
			{
				return obj is GlobalAddressListIdParameter.GlobalAddressListFilter<T>;
			}

			public override int GetHashCode()
			{
				return typeof(GlobalAddressListIdParameter.GlobalAddressListFilter<T>).GetHashCode();
			}

			private static GlobalAddressListIdParameter.GlobalAddressListFilter<T> globalAddressListFilter = new GlobalAddressListIdParameter.GlobalAddressListFilter<T>();
		}
	}
}
