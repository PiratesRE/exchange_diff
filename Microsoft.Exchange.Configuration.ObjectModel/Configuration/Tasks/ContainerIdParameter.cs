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
	public class ContainerIdParameter : ADIdParameter, IIdentityParameter
	{
		public ContainerIdParameter(string rawString) : base(rawString)
		{
		}

		public ContainerIdParameter(INamedIdentity namedIdentity) : base(namedIdentity)
		{
		}

		public ContainerIdParameter(ADObjectId adObjectId) : base(adObjectId)
		{
		}

		public ContainerIdParameter()
		{
		}

		public static ContainerIdParameter Parse(string rawString)
		{
			return new ContainerIdParameter(rawString);
		}

		internal override IEnumerable<T> GetObjects<T>(ADObjectId rootId, IDirectorySession session, IDirectorySession subTreeSession, OptionalIdentityData optionalData, out LocalizedString? notFoundReason)
		{
			if (!typeof(Container).IsAssignableFrom(typeof(T)) && !typeof(ADContainer).IsAssignableFrom(typeof(T)))
			{
				throw new ArgumentException(Strings.ErrorInvalidType(typeof(T).Name), "type");
			}
			EnumerableWrapper<T> wrapper = EnumerableWrapper<T>.GetWrapper(base.GetObjects<T>(rootId, session, subTreeSession, optionalData, out notFoundReason));
			if (!wrapper.HasElements())
			{
				notFoundReason = null;
				QueryFilter filter = new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Name, base.RawIdentity);
				wrapper = EnumerableWrapper<T>.GetWrapper(base.PerformPrimarySearch<T>(filter, ((IConfigurationSession)session).GetOrgContainerId(), session, false, optionalData));
			}
			return wrapper;
		}
	}
}
