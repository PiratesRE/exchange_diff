using System;
using Microsoft.Exchange.Services.Wcf.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	internal class CmdletException : Exception
	{
		public OptionsActionError ErrorCode { get; set; }

		public CmdletException(OptionsActionError errorCode, string errorMessage) : base(errorMessage)
		{
			this.ErrorCode = errorCode;
		}
	}
}
