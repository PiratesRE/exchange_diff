using System;
using System.Collections.Generic;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.UM.UMPhoneSession
{
	[Serializable]
	public class UMPhoneSessionIdentityParameter : IIdentityParameter
	{
		public UMPhoneSessionIdentityParameter()
		{
		}

		public UMPhoneSessionIdentityParameter(INamedIdentity namedIdentity) : this(namedIdentity.Identity)
		{
		}

		public UMPhoneSessionIdentityParameter(string identity)
		{
			this.identity = new UMPhoneSessionId(identity);
		}

		public UMPhoneSessionIdentityParameter(UMPhoneSessionId identity)
		{
			this.identity = identity;
		}

		public string RawIdentity
		{
			get
			{
				return this.identity.ToString();
			}
		}

		public static explicit operator string(UMPhoneSessionIdentityParameter identityParameter)
		{
			if (identityParameter != null)
			{
				return identityParameter.ToString();
			}
			return null;
		}

		public static UMPhoneSessionIdentityParameter Parse(string identity)
		{
			return new UMPhoneSessionIdentityParameter(identity);
		}

		public IEnumerable<T> GetObjects<T>(ObjectId rootId, IConfigDataProvider session) where T : IConfigurable, new()
		{
			LocalizedString? localizedString;
			return ((IIdentityParameter)this).GetObjects<T>(rootId, session, null, out localizedString);
		}

		public IEnumerable<T> GetObjects<T>(ObjectId rootId, IConfigDataProvider session, OptionalIdentityData optionalData, out LocalizedString? notFoundReason) where T : IConfigurable, new()
		{
			T t = (T)((object)session.Read<T>(this.identity));
			if (t == null)
			{
				notFoundReason = new LocalizedString?(Strings.ErrorManagementObjectNotFound(this.ToString()));
				return new T[0];
			}
			notFoundReason = null;
			return new T[]
			{
				t
			};
		}

		public void Initialize(ObjectId objectId)
		{
			this.identity = (UMPhoneSessionId)objectId;
		}

		public override string ToString()
		{
			return this.identity.ToString();
		}

		private UMPhoneSessionId identity;
	}
}
