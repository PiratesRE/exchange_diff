using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Permissions;

namespace System.Security.Policy
{
	[Serializable]
	internal sealed class LegacyEvidenceList : EvidenceBase, IEnumerable<EvidenceBase>, IEnumerable, ILegacyEvidenceAdapter
	{
		public object EvidenceObject
		{
			get
			{
				if (this.m_legacyEvidenceList.Count <= 0)
				{
					return null;
				}
				return this.m_legacyEvidenceList[0];
			}
		}

		public Type EvidenceType
		{
			get
			{
				ILegacyEvidenceAdapter legacyEvidenceAdapter = this.m_legacyEvidenceList[0] as ILegacyEvidenceAdapter;
				if (legacyEvidenceAdapter != null)
				{
					return legacyEvidenceAdapter.EvidenceType;
				}
				return this.m_legacyEvidenceList[0].GetType();
			}
		}

		public void Add(EvidenceBase evidence)
		{
			this.m_legacyEvidenceList.Add(evidence);
		}

		public IEnumerator<EvidenceBase> GetEnumerator()
		{
			return this.m_legacyEvidenceList.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.m_legacyEvidenceList.GetEnumerator();
		}

		[SecuritySafeCritical]
		[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
		public override EvidenceBase Clone()
		{
			return base.Clone();
		}

		private List<EvidenceBase> m_legacyEvidenceList = new List<EvidenceBase>();
	}
}
