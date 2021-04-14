using System;
using System.ComponentModel;
using System.Security.Principal;
using Microsoft.Exchange.Cluster.ClusApi;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal static class HaTaskCallbackHelper
	{
		static HaTaskCallbackHelper()
		{
			HaTaskCallbackHelper.ClusterCallbackLookupTableEntry[] array = new HaTaskCallbackHelper.ClusterCallbackLookupTableEntry[4];
			array[0] = new HaTaskCallbackHelper.ClusterCallbackLookupTableEntry(ClusapiMethods.CLUSTER_SETUP_PHASE.ClusterSetupPhaseValidateClusterNameAccount, ClusapiMethods.CLUSTER_SETUP_PHASE_TYPE.ClusterSetupPhaseEnd, ClusapiMethods.CLUSTER_SETUP_PHASE_SEVERITY.ClusterSetupPhaseFatal, delegate(string objectName, uint status)
			{
				Exception result;
				using (WindowsIdentity current = WindowsIdentity.GetCurrent())
				{
					if (status == 5U)
					{
						result = new DagTaskComputerAccountCouldNotBeValidatedAccessDeniedException(objectName, current.Name);
					}
					else
					{
						result = new DagTaskComputerAccountCouldNotBeValidatedException(objectName, current.Name, new Win32Exception((int)status).Message);
					}
				}
				return result;
			});
			array[1] = new HaTaskCallbackHelper.ClusterCallbackLookupTableEntry(ClusapiMethods.CLUSTER_SETUP_PHASE.ClusterSetupPhaseCreateClusterAccount, ClusapiMethods.CLUSTER_SETUP_PHASE_TYPE.ClusterSetupPhaseEnd, ClusapiMethods.CLUSTER_SETUP_PHASE_SEVERITY.ClusterSetupPhaseFatal, delegate(string objectName, uint status)
			{
				Exception result;
				using (WindowsIdentity current = WindowsIdentity.GetCurrent())
				{
					if (status == 5U)
					{
						result = new DagTaskComputerAccountCouldNotBeValidatedAccessDeniedException(objectName, current.Name);
					}
					else
					{
						result = new DagTaskComputerAccountCouldNotBeValidatedException(objectName, current.Name, new Win32Exception((int)status).Message);
					}
				}
				return result;
			});
			array[2] = new HaTaskCallbackHelper.ClusterCallbackLookupTableEntry(ClusapiMethods.CLUSTER_SETUP_PHASE.ClusterSetupPhaseValidateNetft, ClusapiMethods.CLUSTER_SETUP_PHASE_TYPE.ClusterSetupPhaseEnd, ClusapiMethods.CLUSTER_SETUP_PHASE_SEVERITY.ClusterSetupPhaseFatal, (string objectName, uint status) => new DagTaskNetFtProblemException((int)status));
			array[3] = new HaTaskCallbackHelper.ClusterCallbackLookupTableEntry(ClusapiMethods.CLUSTER_SETUP_PHASE.ClusterSetupPhaseValidateNodeState, ClusapiMethods.CLUSTER_SETUP_PHASE_TYPE.ClusterSetupPhaseEnd, ClusapiMethods.CLUSTER_SETUP_PHASE_SEVERITY.ClusterSetupPhaseFatal, delegate(string objectName, uint status)
			{
				if (status == 2147746132U)
				{
					return new DagTaskClusteringMustBeInstalledException(objectName);
				}
				if (status == 2147947451U)
				{
					return new DagTaskValidateNodeTimedOutException(objectName);
				}
				return null;
			});
			HaTaskCallbackHelper.m_lookupTable = array;
		}

		internal static Exception LookUpStatus(ClusapiMethods.CLUSTER_SETUP_PHASE eSetupPhase, ClusapiMethods.CLUSTER_SETUP_PHASE_TYPE ePhaseType, ClusapiMethods.CLUSTER_SETUP_PHASE_SEVERITY ePhaseSeverity, uint dwPercentComplete, string objectName, uint status)
		{
			foreach (HaTaskCallbackHelper.ClusterCallbackLookupTableEntry clusterCallbackLookupTableEntry in HaTaskCallbackHelper.m_lookupTable)
			{
				if (eSetupPhase == clusterCallbackLookupTableEntry.m_ESetupPhase && ePhaseType == clusterCallbackLookupTableEntry.m_EPhaseType && ePhaseSeverity == clusterCallbackLookupTableEntry.m_EPhaseSeverity)
				{
					Exception ex = clusterCallbackLookupTableEntry.m_ExceptionFactory(objectName, status);
					if (ex != null)
					{
						return ex;
					}
				}
			}
			return null;
		}

		private static HaTaskCallbackHelper.ClusterCallbackLookupTableEntry[] m_lookupTable;

		internal class ClusterCallbackLookupTableEntry
		{
			internal ClusterCallbackLookupTableEntry(ClusapiMethods.CLUSTER_SETUP_PHASE eSetupPhase, ClusapiMethods.CLUSTER_SETUP_PHASE_TYPE ePhaseType, ClusapiMethods.CLUSTER_SETUP_PHASE_SEVERITY ePhaseSeverity, HaTaskCallbackHelper.ClusterCallbackLookupTableEntry.ExceptionFactory exceptionFactory)
			{
				this.m_ESetupPhase = eSetupPhase;
				this.m_EPhaseType = ePhaseType;
				this.m_EPhaseSeverity = ePhaseSeverity;
				this.m_ExceptionFactory = exceptionFactory;
			}

			internal ClusapiMethods.CLUSTER_SETUP_PHASE m_ESetupPhase;

			internal ClusapiMethods.CLUSTER_SETUP_PHASE_TYPE m_EPhaseType;

			internal ClusapiMethods.CLUSTER_SETUP_PHASE_SEVERITY m_EPhaseSeverity;

			internal HaTaskCallbackHelper.ClusterCallbackLookupTableEntry.ExceptionFactory m_ExceptionFactory;

			internal delegate Exception ExceptionFactory(string objectName, uint status);
		}
	}
}
