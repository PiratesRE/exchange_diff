using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.UM.UMCommon.Exceptions;

namespace Microsoft.Exchange.UM.ClientAccess.Messages
{
	[Serializable]
	public class UMAACustomPromptRpcRequest : UMAutoAttendantPromptRpcRequest
	{
		public UMAACustomPromptRpcRequest(UMAutoAttendant aa, string promptFileName) : base(aa)
		{
			this.PromptFileName = promptFileName;
		}

		public string PromptFileName { get; private set; }

		internal override string GetFriendlyName()
		{
			return Strings.AutoAttendantCustomPromptRequest;
		}
	}
}
