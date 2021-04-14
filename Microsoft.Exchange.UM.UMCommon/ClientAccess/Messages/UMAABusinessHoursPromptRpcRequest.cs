using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.UM.UMCommon.Exceptions;

namespace Microsoft.Exchange.UM.ClientAccess.Messages
{
	[Serializable]
	public class UMAABusinessHoursPromptRpcRequest : UMAutoAttendantPromptRpcRequest
	{
		public UMAABusinessHoursPromptRpcRequest(UMAutoAttendant aa) : base(aa)
		{
		}

		internal override string GetFriendlyName()
		{
			return Strings.AutoAttendantBusinessHoursPromptRequest;
		}
	}
}
