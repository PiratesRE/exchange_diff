using System;
using System.Collections.Generic;

namespace System.Security.Policy
{
	internal interface IRuntimeEvidenceFactory
	{
		IEvidenceFactory Target { get; }

		IEnumerable<EvidenceBase> GetFactorySuppliedEvidence();

		EvidenceBase GenerateEvidence(Type evidenceType);
	}
}
