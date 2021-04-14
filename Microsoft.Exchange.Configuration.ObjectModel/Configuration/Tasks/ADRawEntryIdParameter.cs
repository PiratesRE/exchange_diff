using System;
using System.Collections.Generic;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public class ADRawEntryIdParameter : ADIdParameter
	{
		public ADRawEntryIdParameter()
		{
		}

		public ADRawEntryIdParameter(ADObjectId adObjectId) : base(adObjectId)
		{
		}

		public ADRawEntryIdParameter(INamedIdentity namedIdentity) : base(namedIdentity)
		{
		}

		protected ADRawEntryIdParameter(string identity) : base(identity)
		{
		}

		public static ADRawEntryIdParameter Parse(string identity)
		{
			return new ADRawEntryIdParameter(identity);
		}

		public static explicit operator string(ADRawEntryIdParameter rawEntryIdParameter)
		{
			if (rawEntryIdParameter != null)
			{
				return rawEntryIdParameter.ToString();
			}
			return null;
		}

		internal override IEnumerable<T> PerformSearch<T>(QueryFilter filter, ADObjectId rootId, IDirectorySession session, bool deepSearch)
		{
			if (session == null)
			{
				throw new ArgumentNullException("session");
			}
			if (typeof(T) != typeof(ADRawEntry))
			{
				throw new ArgumentException(Strings.ErrorInvalidType(typeof(T).Name), "type");
			}
			return (IEnumerable<T>)session.FindPagedADRawEntry(rootId, deepSearch ? QueryScope.SubTree : QueryScope.OneLevel, filter, null, 0, new PropertyDefinition[]
			{
				ADObjectSchema.Name,
				ADObjectSchema.Id,
				ADObjectSchema.ObjectClass
			});
		}

		internal override IEnumerable<T> GetObjects<T>(ADObjectId rootId, IDirectorySession session, IDirectorySession subTreeSession, OptionalIdentityData optionalData, out LocalizedString? notFoundReason)
		{
			EnumerableWrapper<T> wrapper = EnumerableWrapper<T>.GetWrapper(base.GetObjects<T>(rootId, session, subTreeSession, optionalData, out notFoundReason));
			if (notFoundReason == null && !wrapper.HasElements() && ADRawEntryIdParameter.IsSingleOrDoubleAsterisks(base.RawIdentity))
			{
				notFoundReason = new LocalizedString?(Strings.ErrorNotSupportSingletonWildcard);
			}
			return wrapper;
		}

		protected override bool IsWildcardDefined(string name)
		{
			return name != null && !ADRawEntryIdParameter.IsSingleOrDoubleAsterisks(name) && (name.StartsWith("*") || name.EndsWith("*"));
		}

		private static bool IsSingleOrDoubleAsterisks(string str)
		{
			return "*" == str || "**" == str;
		}
	}
}
