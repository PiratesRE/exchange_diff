using System;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public class ShouldContinueException : TransientException, IExceptionDetails
	{
		public ShouldContinueException(string message) : this(new LocalizedString(message), null, null)
		{
		}

		public ShouldContinueException(string message, string cmdlet, string suppressConfirmParameterName) : base(new LocalizedString(message))
		{
			this.Details = new ShouldContinueExceptionDetails(cmdlet, suppressConfirmParameterName);
		}

		public ShouldContinueExceptionDetails Details { get; private set; }

		object IExceptionDetails.Details
		{
			get
			{
				return this.Details;
			}
		}
	}
}
