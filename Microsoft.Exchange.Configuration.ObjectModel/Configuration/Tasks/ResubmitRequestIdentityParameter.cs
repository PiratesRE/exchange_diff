using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.Management;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public class ResubmitRequestIdentityParameter : IIdentityParameter
	{
		public ResubmitRequestIdentityParameter()
		{
		}

		public ResubmitRequestIdentityParameter(ResubmitRequestId identity)
		{
			this.identity = identity;
		}

		public IEnumerable<T> GetObjects<T>(ObjectId rootId, IConfigDataProvider session) where T : IConfigurable, new()
		{
			return session.FindPaged<T>(null, rootId, false, null, 0);
		}

		public IEnumerable<T> GetObjects<T>(ObjectId rootId, IConfigDataProvider session, OptionalIdentityData notUsed2, out LocalizedString? notFoundReason) where T : IConfigurable, new()
		{
			notFoundReason = null;
			T[] array = session.FindPaged<T>(null, rootId, false, null, 0).ToArray<T>();
			if (array == null || array.Length == 0)
			{
				notFoundReason = new LocalizedString?(Strings.ResubmitRequestDoesNotExist((this.identity == null) ? 0L : this.identity.ResubmitRequestRowId));
			}
			return array;
		}

		public void Initialize(ObjectId objectId)
		{
			this.identity = (ResubmitRequestId)objectId;
		}

		public string RawIdentity
		{
			get
			{
				if (this.identity == null)
				{
					return string.Empty;
				}
				return this.identity.ToString();
			}
		}

		public ResubmitRequestId Identity
		{
			get
			{
				return this.identity;
			}
		}

		public static ResubmitRequestIdentityParameter Parse(string identity)
		{
			return new ResubmitRequestIdentityParameter(ResubmitRequestId.Parse(identity));
		}

		private ResubmitRequestId identity;
	}
}
