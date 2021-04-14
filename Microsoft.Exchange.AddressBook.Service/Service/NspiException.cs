using System;
using Microsoft.Exchange.Nspi;

namespace Microsoft.Exchange.AddressBook.Service
{
	internal class NspiException : Exception
	{
		internal NspiException(NspiStatus status, string message) : base(message)
		{
			base.HResult = (int)status;
		}

		internal NspiException(NspiStatus status, string message, Exception inner) : base(message, inner)
		{
			base.HResult = (int)status;
		}

		internal NspiStatus Status
		{
			get
			{
				return (NspiStatus)base.HResult;
			}
		}
	}
}
