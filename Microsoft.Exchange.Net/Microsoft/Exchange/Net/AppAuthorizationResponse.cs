using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Net
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class AppAuthorizationResponse
	{
		internal AppAuthorizationResponse()
		{
		}

		public string AppAuthorizationCode { get; internal set; }

		public string Error { get; internal set; }

		public string ErrorReason { get; internal set; }

		public string ErrorDescription { get; internal set; }
	}
}
