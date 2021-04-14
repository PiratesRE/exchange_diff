using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.UM.UMCommon.Exceptions;

namespace Microsoft.Exchange.UM.ClientAccess.Messages
{
	[Serializable]
	public class UMAutoAttendantPromptRpcRequest : PromptPreviewRpcRequest
	{
		public UMAutoAttendantPromptRpcRequest(UMAutoAttendant aa)
		{
			this.AutoAttendant = aa;
		}

		public UMAutoAttendant AutoAttendant { get; private set; }

		internal override string GetFriendlyName()
		{
			return Strings.AutoAttendantPromptRequest;
		}
	}
}
