using System;

namespace Microsoft.Exchange.Server.Storage.LogicalDataModel
{
	public enum SubmissionResponsibility : long
	{
		None,
		SpoolerDone,
		MdbDone = 16L,
		PreProcessingDone = 256L,
		LocalDeliveryDone = 4096L
	}
}
