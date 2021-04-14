using System;
using System.Collections;
using System.Globalization;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.UM.ClientAccess.Messages;
using Microsoft.Exchange.UM.Prompts.Provisioning;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class UMAACustomPromptRpcRequestHandler : UMPromptRpcRequestHandler
	{
		protected override ArrayList GetPrompts(RequestBase requestBase)
		{
			UMAACustomPromptRpcRequest umaacustomPromptRpcRequest = requestBase as UMAACustomPromptRpcRequest;
			ArrayList customPrompts;
			using (IPublishingSession publishingSession = PublishingPoint.GetPublishingSession("UM", umaacustomPromptRpcRequest.AutoAttendant))
			{
				this.promptFile = publishingSession.DownloadAsWav(umaacustomPromptRpcRequest.PromptFileName);
				if (Utils.RunningInTestMode)
				{
					string path;
					this.promptFile = UMPromptRpcRequestHandler.ChangeFileNameForTests(this.promptFile, umaacustomPromptRpcRequest.PromptFileName, out path);
					customPrompts = this.GetCustomPrompts(path, umaacustomPromptRpcRequest.AutoAttendant.Language.Culture);
				}
				else
				{
					customPrompts = this.GetCustomPrompts(this.promptFile.FilePath, umaacustomPromptRpcRequest.AutoAttendant.Language.Culture);
				}
			}
			return customPrompts;
		}

		private ArrayList GetCustomPrompts(string path, CultureInfo c)
		{
			ArrayList arrayList = GlobCfg.DefaultPromptHelper.Build(null, c, new PromptConfigBase[]
			{
				GlobCfg.DefaultPromptsForPreview.AACustomPrompt
			});
			VariablePrompt<string>.SetActualPromptValues(arrayList, "customPrompt", path);
			return arrayList;
		}

		protected override void InternalDispose(bool disposing)
		{
			try
			{
				if (disposing && this.promptFile != null)
				{
					this.promptFile.Dispose();
				}
			}
			finally
			{
				base.InternalDispose(disposing);
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<UMAACustomPromptRpcRequestHandler>(this);
		}

		private ITempFile promptFile;
	}
}
