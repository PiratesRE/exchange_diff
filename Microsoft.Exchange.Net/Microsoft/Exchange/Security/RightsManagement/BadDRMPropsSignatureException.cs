using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Security.RightsManagement
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class BadDRMPropsSignatureException : RightsManagementException
	{
		public BadDRMPropsSignatureException() : this(DrmStrings.BadDRMPropsSignature)
		{
		}

		public BadDRMPropsSignatureException(LocalizedString message) : this(message, null)
		{
		}

		public BadDRMPropsSignatureException(LocalizedString message, Exception innerException) : base(RightsManagementFailureCode.BadDRMPropsSignature, message, innerException)
		{
		}
	}
}
