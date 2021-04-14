using System;
using System.Collections.Generic;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.MessagingPolicies.AddressRewrite
{
	[Serializable]
	public class AddressRewriteEntryIdParameter : ADIdParameter
	{
		public AddressRewriteEntryIdParameter()
		{
		}

		public AddressRewriteEntryIdParameter(string identity) : base(identity)
		{
		}

		public AddressRewriteEntryIdParameter(ADObjectId adObjectId) : base(adObjectId)
		{
		}

		public AddressRewriteEntryIdParameter(INamedIdentity namedIdentity) : base(namedIdentity)
		{
		}

		public ADObjectId AdIdentity
		{
			get
			{
				return base.InternalADObjectId;
			}
		}

		public string Name
		{
			get
			{
				return base.RawIdentity;
			}
		}

		public static explicit operator string(AddressRewriteEntryIdParameter addressRewriteEntryIdParameter)
		{
			return addressRewriteEntryIdParameter.ToString();
		}

		public static AddressRewriteEntryIdParameter Parse(string identity)
		{
			return new AddressRewriteEntryIdParameter(identity);
		}

		internal override IEnumerable<T> GetObjects<T>(ADObjectId rootId, IDirectorySession session, IDirectorySession subTreeSession, OptionalIdentityData optionalData, out LocalizedString? notFoundReason)
		{
			TaskLogger.LogEnter();
			IEnumerable<T> result = null;
			try
			{
				if (typeof(T) != typeof(AddressRewriteEntry))
				{
					throw new InvalidOperationException();
				}
				if (session == null)
				{
					throw new ArgumentException(Strings.AddressRewriteSessionNull, "Session");
				}
				if (string.IsNullOrEmpty(base.RawIdentity) && base.InternalADObjectId == null)
				{
					throw new ArgumentException(Strings.AddressRewriteInvalidIdentity, "Identity");
				}
				notFoundReason = null;
				if (base.InternalADObjectId == null)
				{
					IConfigurationSession configurationSession = (IConfigurationSession)session;
					QueryFilter filter = new TextFilter(ADObjectSchema.Name, this.Name, MatchOptions.FullString, MatchFlags.IgnoreCase);
					result = base.PerformPrimarySearch<T>(filter, null, session, true, optionalData);
				}
				else
				{
					result = base.GetObjects<T>(rootId, session, subTreeSession, optionalData, out notFoundReason);
				}
			}
			finally
			{
				TaskLogger.LogExit();
			}
			return result;
		}

		public override string ToString()
		{
			if (this.AdIdentity != null)
			{
				return this.AdIdentity.ObjectGuid.ToString();
			}
			return this.Name;
		}
	}
}
