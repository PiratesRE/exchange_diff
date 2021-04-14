using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.Management.Tasks.UM
{
	internal class UMPromptPreviewRpcTargetPicker : UMServerRpcTargetPickerBase<IVersionedRpcTarget>
	{
		protected override IVersionedRpcTarget CreateTarget(Server server)
		{
			return new UMPromptPreviewRpcTarget(server);
		}

		public static readonly UMPromptPreviewRpcTargetPicker Instance = new UMPromptPreviewRpcTargetPicker();
	}
}
