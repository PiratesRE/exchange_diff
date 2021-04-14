using System;

namespace System.Security.Policy
{
	internal interface ILegacyEvidenceAdapter
	{
		object EvidenceObject { get; }

		Type EvidenceType { get; }
	}
}
