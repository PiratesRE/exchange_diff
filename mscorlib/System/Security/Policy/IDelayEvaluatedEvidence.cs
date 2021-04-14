using System;

namespace System.Security.Policy
{
	internal interface IDelayEvaluatedEvidence
	{
		bool IsVerified { [SecurityCritical] get; }

		bool WasUsed { get; }

		void MarkUsed();
	}
}
