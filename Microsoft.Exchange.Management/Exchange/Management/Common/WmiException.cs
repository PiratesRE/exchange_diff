using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Common
{
	[Serializable]
	public class WmiException : LocalizedException
	{
		public WmiException(Exception e, string computerName) : base((e == null) ? Strings.ErrorWMIException(computerName) : Strings.ErrorWMIException2(computerName, e.Message), e)
		{
		}
	}
}
