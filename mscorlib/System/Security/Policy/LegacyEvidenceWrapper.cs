using System;
using System.Security.Permissions;

namespace System.Security.Policy
{
	[Serializable]
	internal sealed class LegacyEvidenceWrapper : EvidenceBase, ILegacyEvidenceAdapter
	{
		internal LegacyEvidenceWrapper(object legacyEvidence)
		{
			this.m_legacyEvidence = legacyEvidence;
		}

		public object EvidenceObject
		{
			get
			{
				return this.m_legacyEvidence;
			}
		}

		public Type EvidenceType
		{
			get
			{
				return this.m_legacyEvidence.GetType();
			}
		}

		public override bool Equals(object obj)
		{
			return this.m_legacyEvidence.Equals(obj);
		}

		public override int GetHashCode()
		{
			return this.m_legacyEvidence.GetHashCode();
		}

		[SecuritySafeCritical]
		[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
		public override EvidenceBase Clone()
		{
			return base.Clone();
		}

		private object m_legacyEvidence;
	}
}
