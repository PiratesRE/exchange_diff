using System;

namespace Microsoft.Exchange.Management.Deployment.Analysis
{
	internal interface IAnalysisDependantSetter
	{
		void SetAnalysis(Analysis analysis);
	}
}
