using System;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Assistants
{
	internal sealed class FatalException : Exception
	{
		public FatalException(LocalizedString description) : base(description)
		{
		}
	}
}
