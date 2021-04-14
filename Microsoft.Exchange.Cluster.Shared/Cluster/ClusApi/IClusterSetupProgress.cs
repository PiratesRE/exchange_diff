using System;

namespace Microsoft.Exchange.Cluster.ClusApi
{
	internal interface IClusterSetupProgress
	{
		int MaxPercentageDuringCallback { get; set; }

		Exception LastException { get; set; }

		int ClusterSetupProgressCallback(IntPtr pvCallbackArg, ClusapiMethods.CLUSTER_SETUP_PHASE eSetupPhase, ClusapiMethods.CLUSTER_SETUP_PHASE_TYPE ePhaseType, ClusapiMethods.CLUSTER_SETUP_PHASE_SEVERITY ePhaseSeverity, uint dwPercentComplete, string lpszObjectName, uint dwStatus);
	}
}
