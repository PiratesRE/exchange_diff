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
	public class AddressBookBaseIdParameter : IIdentityParameter
	{
		public AddressBookBaseIdParameter()
		{
		}

		public AddressBookBaseIdParameter(ADObjectId adObjectId)
		{
			this.addressListIdParameter = new AddressListIdParameter(adObjectId);
			this.globalAddressListIdParameter = new GlobalAddressListIdParameter(adObjectId);
		}

		public AddressBookBaseIdParameter(AddressList addressList)
		{
			this.addressListIdParameter = new AddressListIdParameter(addressList);
		}

		public AddressBookBaseIdParameter(GlobalAddressList globalAddressList)
		{
			this.globalAddressListIdParameter = new GlobalAddressListIdParameter(globalAddressList);
		}

		public AddressBookBaseIdParameter(INamedIdentity namedIdentity) : this(namedIdentity.Identity)
		{
			this.rawIdentity = namedIdentity.DisplayName;
		}

		protected AddressBookBaseIdParameter(string identity)
		{
			this.rawIdentity = identity;
			this.addressListIdParameter = AddressListIdParameter.Parse(identity);
			this.globalAddressListIdParameter = GlobalAddressListIdParameter.Parse(identity);
		}

		string IIdentityParameter.RawIdentity
		{
			get
			{
				return this.RawIdentity;
			}
		}

		internal string RawIdentity
		{
			get
			{
				return this.rawIdentity ?? this.ToString();
			}
		}

		public static AddressBookBaseIdParameter Parse(string identity)
		{
			return new AddressBookBaseIdParameter(identity);
		}

		void IIdentityParameter.Initialize(ObjectId objectId)
		{
			this.Initialize(objectId);
		}

		public override string ToString()
		{
			if (this.addressListIdParameter != null)
			{
				return this.addressListIdParameter.ToString();
			}
			if (this.globalAddressListIdParameter != null)
			{
				return this.globalAddressListIdParameter.ToString();
			}
			return string.Empty;
		}

		IEnumerable<T> IIdentityParameter.GetObjects<T>(ObjectId rootId, IConfigDataProvider session, OptionalIdentityData optionalData, out LocalizedString? notFoundReason)
		{
			return this.GetObjects<T>(rootId, session, optionalData, out notFoundReason);
		}

		IEnumerable<T> IIdentityParameter.GetObjects<T>(ObjectId rootId, IConfigDataProvider session)
		{
			LocalizedString? localizedString;
			return this.GetObjects<T>(rootId, session, null, out localizedString);
		}

		internal virtual void Initialize(ObjectId objectId)
		{
			if (objectId == null)
			{
				throw new ArgumentNullException("objectId");
			}
			ADObjectId adobjectId = objectId as ADObjectId;
			if (adobjectId == null)
			{
				throw new ArgumentException(Strings.ErrorInvalidParameterType("objectId", typeof(ADObjectId).Name), "objectId");
			}
			if (this.addressListIdParameter != null || this.globalAddressListIdParameter != null)
			{
				throw new InvalidOperationException(Strings.ErrorChangeImmutableType);
			}
			this.addressListIdParameter = new AddressListIdParameter();
			this.addressListIdParameter.Initialize(adobjectId);
			this.globalAddressListIdParameter = new GlobalAddressListIdParameter();
			this.globalAddressListIdParameter.Initialize(adobjectId);
		}

		internal virtual IEnumerable<T> GetObjects<T>(ObjectId rootId, IConfigDataProvider session, OptionalIdentityData optionalData, out LocalizedString? notFoundReason) where T : IConfigurable, new()
		{
			TaskLogger.LogEnter();
			notFoundReason = null;
			IEnumerable<AddressBookBase> enumerable = new List<AddressBookBase>();
			try
			{
				if (typeof(T) != typeof(AddressBookBase))
				{
					throw new ArgumentException(Strings.ErrorInvalidType(typeof(T).Name), "type");
				}
				if (session == null)
				{
					throw new ArgumentNullException("session");
				}
				IList<IEnumerable<AddressBookBase>> list = new List<IEnumerable<AddressBookBase>>();
				if (this.addressListIdParameter != null)
				{
					list.Add(this.addressListIdParameter.GetObjects<AddressBookBase>(rootId, session));
				}
				if (this.globalAddressListIdParameter != null)
				{
					list.Add(this.globalAddressListIdParameter.GetObjects<AddressBookBase>(rootId, session));
				}
				enumerable = EnumerableWrapper<AddressBookBase>.GetWrapper(list);
			}
			finally
			{
				TaskLogger.LogExit();
			}
			return (IEnumerable<T>)enumerable;
		}

		private AddressListIdParameter addressListIdParameter;

		private GlobalAddressListIdParameter globalAddressListIdParameter;

		private string rawIdentity;
	}
}
