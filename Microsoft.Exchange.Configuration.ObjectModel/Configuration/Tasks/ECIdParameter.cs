using System;
using System.Collections.Generic;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public class ECIdParameter : IIdentityParameter
	{
		public ECIdParameter()
		{
			this.Identity = null;
		}

		public ECIdParameter(ObjectId id)
		{
			this.Initialize(id);
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
				return this.ToString();
			}
		}

		public static ECIdParameter Parse(string input)
		{
			return new ECIdParameter(EventCategoryIdentity.Parse(input));
		}

		void IIdentityParameter.Initialize(ObjectId objectId)
		{
			this.Initialize(objectId);
		}

		public override string ToString()
		{
			if (this.Identity != null)
			{
				return this.Identity.ToString();
			}
			return null;
		}

		IEnumerable<T> IIdentityParameter.GetObjects<T>(ObjectId rootId, IConfigDataProvider session)
		{
			return this.GetObjects<T>(rootId, session);
		}

		IEnumerable<T> IIdentityParameter.GetObjects<T>(ObjectId rootId, IConfigDataProvider session, OptionalIdentityData optionalData, out LocalizedString? notFoundReason)
		{
			return this.GetObjects<T>(rootId, session, optionalData, out notFoundReason);
		}

		internal virtual void Initialize(ObjectId id)
		{
			this.Identity = (id as EventCategoryIdentity);
			if (this.Identity.EventSource != null && !this.Identity.EventSource.StartsWith("*") && !this.Identity.EventSource.EndsWith("*"))
			{
				bool flag = false;
				foreach (string strB in EventCategoryIdentity.EventSources)
				{
					if (string.Compare(this.Identity.EventSource, strB, StringComparison.OrdinalIgnoreCase) == 0)
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					throw new ArgumentException(Strings.ErrorInvalidIdentity(this.Identity.EventSource), "Identity.EventSource");
				}
			}
		}

		internal IEnumerable<T> GetObjects<T>(ObjectId rootId, IConfigDataProvider session, OptionalIdentityData optionalData, out LocalizedString? notFoundReason) where T : IConfigurable, new()
		{
			notFoundReason = new LocalizedString?(Strings.ErrorManagementObjectNotFound(this.ToString()));
			return session.FindPaged<T>((optionalData == null) ? null : optionalData.AdditionalFilter, this.Identity, false, null, 0);
		}

		internal IEnumerable<T> GetObjects<T>(ObjectId rootId, IConfigDataProvider session) where T : IConfigurable, new()
		{
			LocalizedString? localizedString;
			return this.GetObjects<T>(rootId, session, null, out localizedString);
		}

		internal bool IsUnique()
		{
			return this.Identity.Category != null && null != this.Identity.EventSource;
		}

		private EventCategoryIdentity Identity;
	}
}
