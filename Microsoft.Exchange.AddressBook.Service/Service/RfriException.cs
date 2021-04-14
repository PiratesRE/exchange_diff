using System;
using Microsoft.Exchange.Nspi.Rfri;

namespace Microsoft.Exchange.AddressBook.Service
{
	internal class RfriException : Exception
	{
		internal RfriException(RfriStatus status, string message) : base(message)
		{
			base.HResult = (int)status;
		}

		internal RfriException(RfriStatus status, string message, Exception inner) : base(message, inner)
		{
			base.HResult = (int)status;
		}

		internal RfriStatus Status
		{
			get
			{
				return (RfriStatus)base.HResult;
			}
		}
	}
}
