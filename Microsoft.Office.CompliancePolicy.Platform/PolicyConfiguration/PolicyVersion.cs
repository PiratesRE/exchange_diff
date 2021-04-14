using System;
using System.Data.SqlTypes;
using System.Runtime.Serialization;

namespace Microsoft.Office.CompliancePolicy.PolicyConfiguration
{
	[DataContract]
	[Serializable]
	public sealed class PolicyVersion : IComparable
	{
		private PolicyVersion()
		{
		}

		internal Guid InternalStorage
		{
			get
			{
				return this.internalStorage;
			}
		}

		public static bool operator ==(PolicyVersion lhs, PolicyVersion rhs)
		{
			return object.ReferenceEquals(lhs, rhs) || (lhs != null && rhs != null && lhs.Equals(rhs));
		}

		public static bool operator !=(PolicyVersion lhs, PolicyVersion rhs)
		{
			return !(lhs == rhs);
		}

		public static implicit operator Guid(PolicyVersion version)
		{
			if (version == null)
			{
				return Guid.Empty;
			}
			return version.InternalStorage;
		}

		public static implicit operator PolicyVersion(Guid version)
		{
			return PolicyVersion.Create(version);
		}

		public int CompareTo(object obj)
		{
			PolicyVersion policyVersion = obj as PolicyVersion;
			if (policyVersion == null)
			{
				throw new ArgumentNullException("obj");
			}
			return new SqlGuid(this.internalStorage).CompareTo(new SqlGuid(policyVersion.InternalStorage));
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			PolicyVersion policyVersion = obj as PolicyVersion;
			return !(policyVersion == null) && this.internalStorage.Equals(policyVersion.internalStorage);
		}

		public override int GetHashCode()
		{
			return this.internalStorage.GetHashCode();
		}

		internal static PolicyVersion Create(Guid combGuid)
		{
			return new PolicyVersion
			{
				internalStorage = combGuid
			};
		}

		public static readonly PolicyVersion Empty = PolicyVersion.Create(Guid.Empty);

		[DataMember]
		private Guid internalStorage;
	}
}
