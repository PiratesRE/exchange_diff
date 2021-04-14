using System;
using System.Collections.Generic;
using System.Reflection;

namespace System.Security.Policy
{
	internal sealed class AppDomainEvidenceFactory : IRuntimeEvidenceFactory
	{
		internal AppDomainEvidenceFactory(AppDomain target)
		{
			this.m_targetDomain = target;
		}

		public IEvidenceFactory Target
		{
			get
			{
				return this.m_targetDomain;
			}
		}

		public IEnumerable<EvidenceBase> GetFactorySuppliedEvidence()
		{
			return new EvidenceBase[0];
		}

		[SecuritySafeCritical]
		public EvidenceBase GenerateEvidence(Type evidenceType)
		{
			if (!this.m_targetDomain.IsDefaultAppDomain())
			{
				AppDomain defaultDomain = AppDomain.GetDefaultDomain();
				return defaultDomain.GetHostEvidence(evidenceType);
			}
			if (this.m_entryPointEvidence == null)
			{
				Assembly entryAssembly = Assembly.GetEntryAssembly();
				RuntimeAssembly runtimeAssembly = entryAssembly as RuntimeAssembly;
				if (runtimeAssembly != null)
				{
					this.m_entryPointEvidence = runtimeAssembly.EvidenceNoDemand.Clone();
				}
				else if (entryAssembly != null)
				{
					this.m_entryPointEvidence = entryAssembly.Evidence;
				}
			}
			if (this.m_entryPointEvidence != null)
			{
				return this.m_entryPointEvidence.GetHostEvidence(evidenceType);
			}
			return null;
		}

		private AppDomain m_targetDomain;

		private Evidence m_entryPointEvidence;
	}
}
