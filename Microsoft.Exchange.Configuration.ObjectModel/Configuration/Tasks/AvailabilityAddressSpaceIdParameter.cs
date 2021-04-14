using System;
using System.Collections.Generic;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public class AvailabilityAddressSpaceIdParameter : ADIdParameter
	{
		public AvailabilityAddressSpaceIdParameter()
		{
		}

		public AvailabilityAddressSpaceIdParameter(ADObjectId adObjectId) : base(adObjectId)
		{
		}

		public AvailabilityAddressSpaceIdParameter(INamedIdentity namedIdentity) : base(namedIdentity)
		{
		}

		protected AvailabilityAddressSpaceIdParameter(string identity) : base(identity)
		{
		}

		public static AvailabilityAddressSpaceIdParameter Parse(string identity)
		{
			return new AvailabilityAddressSpaceIdParameter(identity);
		}

		internal override IEnumerable<T> GetObjects<T>(ADObjectId rootId, IDirectorySession session, IDirectorySession subTreeSession, OptionalIdentityData optionalData, out LocalizedString? notFoundReason)
		{
			TaskLogger.LogEnter();
			EnumerableWrapper<T> wrapper;
			try
			{
				if (typeof(T) != typeof(AvailabilityAddressSpace))
				{
					throw new ArgumentException(Strings.ErrorInvalidType(typeof(T).Name), "type");
				}
				if (string.IsNullOrEmpty(base.RawIdentity))
				{
					throw new InvalidOperationException(Strings.ErrorOperationOnInvalidObject);
				}
				wrapper = EnumerableWrapper<T>.GetWrapper(base.GetObjects<T>(rootId, session, subTreeSession, optionalData, out notFoundReason));
				if (!wrapper.HasElements())
				{
					notFoundReason = null;
					wrapper = EnumerableWrapper<T>.GetWrapper(base.PerformPrimarySearch<T>(base.CreateWildcardOrEqualFilter(AvailabilityAddressSpaceSchema.ForestName, base.RawIdentity), rootId, session, true, optionalData));
				}
			}
			finally
			{
				TaskLogger.LogExit();
			}
			return wrapper;
		}
	}
}
