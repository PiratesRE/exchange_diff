using System;
using System.Collections;
using System.Globalization;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.UM.ClientAccess.Messages;
using Microsoft.Exchange.UM.Prompts.Provisioning;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class UMDPCustomPromptRpcRequestHandler : UMPromptRpcRequestHandler
	{
		protected override ArrayList GetPrompts(RequestBase requestBase)
		{
			UMDPCustomPromptRpcRequest umdpcustomPromptRpcRequest = requestBase as UMDPCustomPromptRpcRequest;
			ArrayList customPrompts;
			using (IPublishingSession publishingSession = PublishingPoint.GetPublishingSession("UM", umdpcustomPromptRpcRequest.DialPlan))
			{
				this.promptFile = publishingSession.DownloadAsWav(umdpcustomPromptRpcRequest.PromptFileName);
				if (Utils.RunningInTestMode)
				{
					string path;
					this.promptFile = UMPromptRpcRequestHandler.ChangeFileNameForTests(this.promptFile, umdpcustomPromptRpcRequest.PromptFileName, out path);
					customPrompts = this.GetCustomPrompts(path, umdpcustomPromptRpcRequest.DialPlan.DefaultLanguage.Culture);
				}
				else
				{
					customPrompts = this.GetCustomPrompts(this.promptFile.FilePath, umdpcustomPromptRpcRequest.DialPlan.DefaultLanguage.Culture);
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
			return DisposeTracker.Get<UMDPCustomPromptRpcRequestHandler>(this);
		}

		private ITempFile promptFile;
	}
}
