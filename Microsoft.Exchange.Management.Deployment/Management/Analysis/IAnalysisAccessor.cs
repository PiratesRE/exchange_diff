using System;

namespace Microsoft.Exchange.Management.Analysis
{
	internal interface IAnalysisAccessor
	{
		void UpdateProgress(Rule completedRule);

		void CallOnAnalysisMemberStart(AnalysisMember member);

		void CallOnAnalysisMemberStop(AnalysisMember member);

		void CallOnAnalysisMemberEvaluate(AnalysisMember member, Result result);
	}
}
