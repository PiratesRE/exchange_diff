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
	public class ExtendedRightIdParameter : ADIdParameter
	{
		public ExtendedRightIdParameter()
		{
		}

		public ExtendedRightIdParameter(ADObjectId adObjectId) : base(adObjectId)
		{
		}

		public ExtendedRightIdParameter(INamedIdentity namedIdentity) : base(namedIdentity)
		{
		}

		protected ExtendedRightIdParameter(string identity) : base(identity)
		{
		}

		public static ExtendedRight[] AllExtendedRights
		{
			get
			{
				return ExtendedRightIdParameter.allExtendedRights;
			}
		}

		public static ExtendedRightIdParameter Parse(string identity)
		{
			return new ExtendedRightIdParameter(identity);
		}

		internal override IEnumerable<T> GetObjects<T>(ADObjectId rootId, IDirectorySession session, IDirectorySession subTreeSession, OptionalIdentityData optionalData, out LocalizedString? notFoundReason)
		{
			if (typeof(T) != typeof(ExtendedRight))
			{
				throw new ArgumentException(Strings.ErrorInvalidType(typeof(T).Name), "type");
			}
			if (session == null)
			{
				throw new ArgumentNullException("session");
			}
			if (ExtendedRightIdParameter.AllExtendedRights == null)
			{
				ADPagedReader<ExtendedRight> adpagedReader = ((ITopologyConfigurationSession)session).GetAllExtendedRights();
				ExtendedRightIdParameter.allExtendedRights = adpagedReader.ReadAllPages();
			}
			base.GetObjects<T>(rootId, session, subTreeSession, optionalData, out notFoundReason);
			List<T> list = new List<T>();
			foreach (ExtendedRight extendedRight in ExtendedRightIdParameter.AllExtendedRights)
			{
				if (ADObjectId.Equals(extendedRight.Id, base.InternalADObjectId) || (base.InternalADObjectId != null && base.InternalADObjectId.ObjectGuid == extendedRight.RightsGuid) || (base.RawIdentity != null && (string.Compare(extendedRight.DisplayName, base.RawIdentity, StringComparison.OrdinalIgnoreCase) == 0 || string.Compare(extendedRight.Name, base.RawIdentity, StringComparison.OrdinalIgnoreCase) == 0)))
				{
					list.Add((T)((object)extendedRight));
					break;
				}
			}
			return list;
		}

		internal override IEnumerable<T> PerformSearch<T>(QueryFilter filter, ADObjectId rootId, IDirectorySession session, bool deepSearch)
		{
			return new T[0];
		}

		private static ExtendedRight[] allExtendedRights;
	}
}
