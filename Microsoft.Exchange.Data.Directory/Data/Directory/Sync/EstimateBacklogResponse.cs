using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.ServiceModel;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	[MessageContract(WrapperName = "EstimateBacklogResponse", WrapperNamespace = "http://schemas.microsoft.com/online/directoryservices/sync/2008/11", IsWrapped = true)]
	[DebuggerStepThrough]
	[GeneratedCode("System.ServiceModel", "4.0.0.0")]
	public class EstimateBacklogResponse
	{
		public EstimateBacklogResponse()
		{
		}

		public EstimateBacklogResponse(BacklogEstimateBatch EstimateBacklogResult)
		{
			this.EstimateBacklogResult = EstimateBacklogResult;
		}

		[MessageBodyMember(Namespace = "http://schemas.microsoft.com/online/directoryservices/sync/2008/11", Order = 0)]
		public BacklogEstimateBatch EstimateBacklogResult;
	}
}
