using System;
using System.Collections;
using Microsoft.Exchange.UM.ClientAccess.Messages;
using Microsoft.Exchange.UM.Prompts.Provisioning;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class UMMailboxPromptRpcRequestHandler : UMPromptRpcRequestHandler
	{
		protected override ArrayList GetPrompts(RequestBase requestBase)
		{
			UMMailboxPromptRpcRequest ummailboxPromptRpcRequest = requestBase as UMMailboxPromptRpcRequest;
			ArrayList result;
			using (UMSubscriber umsubscriber = UMRecipient.Factory.FromADRecipient<UMSubscriber>(ummailboxPromptRpcRequest.User))
			{
				if (umsubscriber == null)
				{
					throw new InvalidOperationException("AD replication latency ?");
				}
				string initVal = string.IsNullOrEmpty(ummailboxPromptRpcRequest.DisplayName) ? this.GetUserName(umsubscriber) : ummailboxPromptRpcRequest.DisplayName;
				if (ummailboxPromptRpcRequest.AwayFlag)
				{
					result = (ummailboxPromptRpcRequest.CustomFlag ? this.GetMailboxCustomGreetingPrompt(umsubscriber, MailboxGreetingEnum.Away) : VariablePrompt<object>.GetPreview<MailboxAwayGreetingPrompt>(new MailboxAwayGreetingPrompt(), umsubscriber.TelephonyCulture, initVal));
				}
				else
				{
					result = (ummailboxPromptRpcRequest.CustomFlag ? this.GetMailboxCustomGreetingPrompt(umsubscriber, MailboxGreetingEnum.Voicemail) : VariablePrompt<object>.GetPreview<MailboxVoicemailGreetingPrompt>(new MailboxVoicemailGreetingPrompt(), umsubscriber.TelephonyCulture, initVal));
				}
			}
			return result;
		}

		private ArrayList GetMailboxCustomGreetingPrompt(UMSubscriber s, MailboxGreetingEnum gt)
		{
			using (GreetingBase greetingBase = s.ConfigFolder.OpenCustomMailboxGreeting(gt))
			{
				if (greetingBase != null)
				{
					ITempWavFile tempWavFile = greetingBase.Get();
					if (tempWavFile != null)
					{
						tempWavFile.ExtraInfo = greetingBase.Name;
						ArrayList arrayList = GlobCfg.DefaultPromptHelper.Build(null, s.TelephonyCulture, new PromptConfigBase[]
						{
							GlobCfg.DefaultPromptsForPreview.MbxCustomGreeting
						});
						VariablePrompt<ITempWavFile>.SetActualPromptValues(arrayList, "customGreeting", tempWavFile);
						return arrayList;
					}
				}
				throw new SourceFileNotFoundException("custom mailbox greeting");
			}
			ArrayList result;
			return result;
		}

		private string GetUserName(UMSubscriber s)
		{
			string text = s.ADUser.PhoneticDisplayName;
			if (string.IsNullOrEmpty(text))
			{
				text = s.DisplayName;
			}
			return text;
		}
	}
}
