using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public sealed class GetDlpPolicyTipsResponse
	{
		internal GetDlpPolicyTipsResponse(EvaluationResult result)
		{
			this.EvaluationResult = result;
			this.Matches = new DlpPolicyMatchDetail[0];
		}

		internal GetDlpPolicyTipsResponse(EvaluationResult evalResult, OptimizationResult optimizationResult) : this(evalResult)
		{
			this.OptimizationResult = optimizationResult;
		}

		public static GetDlpPolicyTipsResponse GetResponseToPingRequest()
		{
			if (GetDlpPolicyTipsResponse.responseToPingRequest == null)
			{
				GetDlpPolicyTipsResponse getDlpPolicyTipsResponse = new GetDlpPolicyTipsResponse(EvaluationResult.Success);
				getDlpPolicyTipsResponse.Matches = new DlpPolicyMatchDetail[2];
				getDlpPolicyTipsResponse.Matches[0] = new DlpPolicyMatchDetail();
				getDlpPolicyTipsResponse.Matches[0].Action = DlpPolicyTipAction.NotifyOnly;
				getDlpPolicyTipsResponse.Matches[0].AttachmentIds = new AttachmentIdType[]
				{
					new AttachmentIdType(Guid.NewGuid().ToString()),
					new AttachmentIdType(Guid.NewGuid().ToString())
				};
				getDlpPolicyTipsResponse.Matches[1] = new DlpPolicyMatchDetail();
				getDlpPolicyTipsResponse.Matches[1].Action = DlpPolicyTipAction.RejectUnlessSilentOverride;
				getDlpPolicyTipsResponse.Matches[1].AttachmentIds = new AttachmentIdType[]
				{
					new AttachmentIdType(Guid.NewGuid().ToString()),
					new AttachmentIdType(Guid.NewGuid().ToString())
				};
				GetDlpPolicyTipsResponse.responseToPingRequest = getDlpPolicyTipsResponse;
			}
			return GetDlpPolicyTipsResponse.responseToPingRequest;
		}

		[DataMember]
		public EvaluationResult EvaluationResult { get; set; }

		[DataMember]
		public OptimizationResult OptimizationResult { get; set; }

		[DataMember]
		public DlpPolicyMatchDetail[] Matches { get; set; }

		[DataMember]
		public string DiagnosticData { get; set; }

		[DataMember]
		public PolicyTipCustomizedStrings CustomizedStrings { get; set; }

		[DataMember]
		public string DetectedClassificationIds { get; set; }

		[DataMember]
		public string ScanResultData { get; set; }

		private static volatile GetDlpPolicyTipsResponse responseToPingRequest = null;

		public static readonly GetDlpPolicyTipsResponse NoRulesResponse = new GetDlpPolicyTipsResponse(EvaluationResult.Success, OptimizationResult.NoRules);

		public static readonly GetDlpPolicyTipsResponse NullOrganizationResponse = new GetDlpPolicyTipsResponse(EvaluationResult.NullOrganization);

		public static readonly GetDlpPolicyTipsResponse InvalidStoreItemIdResponse = new GetDlpPolicyTipsResponse(EvaluationResult.ClientErrorInvalidStoreItemId);

		public static readonly GetDlpPolicyTipsResponse AccessDeniedStoreItemIdResponse = new GetDlpPolicyTipsResponse(EvaluationResult.ClientErrorAccessDeniedStoreItemId);

		public static readonly GetDlpPolicyTipsResponse InvalidClientScanResultResponse = new GetDlpPolicyTipsResponse(EvaluationResult.ClientErrorInvalidClientScanResult);

		public static readonly GetDlpPolicyTipsResponse NoContentResponse = new GetDlpPolicyTipsResponse(EvaluationResult.ClientErrorNoContent);

		public static readonly GetDlpPolicyTipsResponse ItemAlreadyBeingProcessedResponse = new GetDlpPolicyTipsResponse(EvaluationResult.ClientErrorItemAlreadyBeingProcessed);

		public static readonly GetDlpPolicyTipsResponse TooManyPendingRequestResponse = new GetDlpPolicyTipsResponse(EvaluationResult.TooManyPendingRequests);
	}
}
