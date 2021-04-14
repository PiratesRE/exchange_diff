using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class InvalidTimeProposalException : InvalidOperationException
	{
		public InvalidTimeProposalException(string message) : base(message)
		{
		}
	}
}
