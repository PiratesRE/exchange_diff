using System;
using System.Collections.Generic;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Management.Transport;
using Microsoft.Office.CompliancePolicy.PolicyConfiguration;

namespace Microsoft.Office.CompliancePolicy.Tasks
{
	[Serializable]
	public class PolicyDistributionErrorDetails
	{
		public PolicyDistributionErrorDetails(string endpoint, Guid objectId, ConfigurationObjectType objectType, Workload workload, UnifiedPolicyErrorCode errorCode, string errorMessage, DateTime lastErrorTime, string additionalDiagnostics)
		{
			this.Endpoint = endpoint;
			this.ObjectId = objectId;
			this.ObjectType = objectType;
			this.Workload = workload;
			this.ResultCode = errorCode;
			this.ResultMessage = errorMessage;
			this.LastResultTime = new DateTime?(lastErrorTime);
			this.AdditionalDiagnostics = additionalDiagnostics;
			this.Severity = PolicyDistributionResultSeverity.Error;
			if (string.IsNullOrEmpty(this.ResultMessage))
			{
				string resultMessage;
				if (!PolicyDistributionErrorDetails.errorCodeToStringMap.TryGetValue(this.ResultCode, out resultMessage))
				{
					resultMessage = Strings.UnknownErrorMsg;
				}
				this.ResultMessage = resultMessage;
			}
		}

		public string Endpoint { get; private set; }

		public Guid ObjectId { get; private set; }

		public ConfigurationObjectType ObjectType { get; private set; }

		public Workload Workload { get; private set; }

		public UnifiedPolicyErrorCode ResultCode { get; private set; }

		public string ResultMessage { get; internal set; }

		public DateTime? LastResultTime { get; private set; }

		public string AdditionalDiagnostics { get; private set; }

		public PolicyDistributionResultSeverity Severity { get; set; }

		public override string ToString()
		{
			return string.Format("[{0}]{1}:{2}", this.Workload, this.Endpoint, this.ResultMessage);
		}

		public void Redact()
		{
			this.Endpoint = SuppressingPiiData.Redact(this.Endpoint);
		}

		internal void AppendAdditionalDiagnosticsInfo(string diagnosticsInfo)
		{
			if (!string.IsNullOrWhiteSpace(diagnosticsInfo))
			{
				this.AdditionalDiagnostics = (string.IsNullOrWhiteSpace(this.AdditionalDiagnostics) ? diagnosticsInfo : string.Format("{0}, {1}", this.AdditionalDiagnostics, diagnosticsInfo));
			}
		}

		private static readonly Dictionary<UnifiedPolicyErrorCode, string> errorCodeToStringMap = new Dictionary<UnifiedPolicyErrorCode, string>
		{
			{
				UnifiedPolicyErrorCode.PolicyNotifyError,
				Strings.PolicyNotifyErrorErrorMsg
			},
			{
				UnifiedPolicyErrorCode.PolicySyncTimeout,
				Strings.PolicySyncTimeoutErrorMsg
			},
			{
				UnifiedPolicyErrorCode.FailedToOpenContainer,
				Strings.FailedToOpenContainerErrorMsg
			},
			{
				UnifiedPolicyErrorCode.SiteInReadonlyOrNotAccessible,
				Strings.SiteInReadonlyOrNotAccessibleErrorMsg
			},
			{
				UnifiedPolicyErrorCode.SiteOutOfQuota,
				Strings.SiteOutOfQuotaErrorMsg
			}
		};
	}
}
