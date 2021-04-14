using System;
using System.Collections.Generic;
using Microsoft.Exchange.Compliance.TaskDistributionCommon.ObjectModel;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Office.ComplianceJob.Tasks
{
	[Serializable]
	public class ComplianceJobIdParameter : IIdentityParameter
	{
		public ComplianceJobIdParameter()
		{
		}

		public ComplianceJobIdParameter(ComplianceJob job) : this(((ComplianceJobId)job.Identity).Guid)
		{
		}

		public ComplianceJobIdParameter(Guid identity)
		{
			this.jobId = identity;
			this.rawIdentity = identity.ToString();
		}

		public ComplianceJobIdParameter(string identity)
		{
			if (string.IsNullOrEmpty(identity))
			{
				throw new ArgumentNullException("ComplianceJobIdParameter(string identity)");
			}
			this.rawIdentity = identity;
			Guid guid;
			if (Guid.TryParse(identity, out guid))
			{
				this.jobId = guid;
				return;
			}
			this.displayName = identity;
		}

		public ComplianceJobIdParameter(INamedIdentity namedIdentity) : this(namedIdentity.Identity)
		{
			this.displayName = namedIdentity.DisplayName;
		}

		public static ComplianceJobIdParameter Parse(string identity)
		{
			return new ComplianceJobIdParameter(identity);
		}

		public override string ToString()
		{
			return this.displayName ?? this.rawIdentity;
		}

		public IEnumerable<T> GetObjects<T>(ObjectId rootId, IConfigDataProvider session) where T : IConfigurable, new()
		{
			LocalizedString? localizedString;
			return this.GetObjects<T>(rootId, session, null, out localizedString);
		}

		public IEnumerable<T> GetObjects<T>(ObjectId rootId, IConfigDataProvider session, OptionalIdentityData optionalData, out LocalizedString? notFoundReason) where T : IConfigurable, new()
		{
			if (session == null)
			{
				throw new ArgumentNullException("session");
			}
			if (!(session is ComplianceJobProvider))
			{
				throw new NotSupportedException("session: " + session.GetType().FullName);
			}
			if (optionalData != null && optionalData.AdditionalFilter != null)
			{
				throw new NotSupportedException("Supplying Additional Filters is not currently supported by this IdParameter.");
			}
			T t;
			if (string.IsNullOrEmpty(this.displayName))
			{
				t = (T)((object)((ComplianceJobProvider)session).Read<ComplianceSearch>(new ComplianceJobId(this.jobId)));
			}
			else
			{
				t = (T)((object)((ComplianceJobProvider)session).FindJobsByName<T>(this.displayName));
			}
			if (t == null)
			{
				notFoundReason = new LocalizedString?(Strings.ErrorManagementObjectNotFound(this.rawIdentity));
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
			throw new NotImplementedException();
		}

		public string RawIdentity
		{
			get
			{
				return this.rawIdentity;
			}
		}

		private readonly string displayName;

		private readonly string rawIdentity;

		[NonSerialized]
		private readonly Guid jobId;
	}
}
