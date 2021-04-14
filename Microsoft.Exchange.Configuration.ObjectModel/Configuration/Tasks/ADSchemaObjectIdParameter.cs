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
	public class ADSchemaObjectIdParameter : ADIdParameter
	{
		public ADSchemaObjectIdParameter()
		{
		}

		public ADSchemaObjectIdParameter(ADObjectId adObjectId) : base(adObjectId)
		{
		}

		public ADSchemaObjectIdParameter(INamedIdentity namedIdentity) : base(namedIdentity)
		{
		}

		protected ADSchemaObjectIdParameter(string identity) : base(identity)
		{
		}

		public static ADSchemaObjectIdParameter Parse(string identity)
		{
			return new ADSchemaObjectIdParameter(identity);
		}

		internal override IEnumerable<T> GetObjects<T>(ADObjectId rootId, IDirectorySession session, IDirectorySession subTreeSession, OptionalIdentityData optionalData, out LocalizedString? notFoundReason)
		{
			if (typeof(T) != typeof(ADSchemaClassObject) && typeof(T) != typeof(ADSchemaAttributeObject))
			{
				throw new ArgumentException(Strings.ErrorInvalidType(typeof(T).Name), "type");
			}
			if (session == null)
			{
				throw new ArgumentNullException("session");
			}
			IEnumerable<T> enumerable = base.GetObjects<T>(session.GetSchemaNamingContext(), session, subTreeSession, optionalData, out notFoundReason);
			EnumerableWrapper<T> wrapper = EnumerableWrapper<T>.GetWrapper(enumerable);
			if (!wrapper.HasElements() && base.InternalADObjectId != null && base.InternalADObjectId.ObjectGuid != Guid.Empty)
			{
				QueryFilter filter = new ComparisonFilter(ComparisonOperator.Equal, ADSchemaObjectSchema.SchemaIDGuid, base.InternalADObjectId.ObjectGuid);
				enumerable = base.PerformPrimarySearch<T>(filter, ADSession.GetSchemaNamingContextForLocalForest(), session, true, optionalData);
				wrapper = EnumerableWrapper<T>.GetWrapper(enumerable);
			}
			if (!wrapper.HasElements() && !string.IsNullOrEmpty(base.RawIdentity))
			{
				QueryFilter filter2 = new ComparisonFilter(ComparisonOperator.Equal, ADSchemaObjectSchema.DisplayName, base.RawIdentity);
				enumerable = base.PerformPrimarySearch<T>(filter2, ADSession.GetSchemaNamingContextForLocalForest(), session, true, optionalData);
				wrapper = EnumerableWrapper<T>.GetWrapper(enumerable);
			}
			return wrapper;
		}
	}
}
