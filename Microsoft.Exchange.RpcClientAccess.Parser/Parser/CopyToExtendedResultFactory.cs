using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class CopyToExtendedResultFactory : ResultFactory
	{
		internal CopyToExtendedResultFactory(uint destinationObjectHandleIndex)
		{
			this.destinationObjectHandleIndex = destinationObjectHandleIndex;
		}

		public override RopResult CreateStandardFailedResult(ErrorCode errorCode)
		{
			return this.CreateFailedResult(errorCode);
		}

		public RopResult CreateFailedResult(ErrorCode errorCode)
		{
			return new FailedCopyToExtendedResult(errorCode, this.destinationObjectHandleIndex);
		}

		public RopResult CreateSuccessfulResult(PropertyProblem[] propertyProblems)
		{
			return new SuccessfulCopyToExtendedResult(propertyProblems);
		}

		private readonly uint destinationObjectHandleIndex;
	}
}
