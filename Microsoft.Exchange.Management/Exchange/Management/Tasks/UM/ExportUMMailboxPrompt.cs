using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Rpc;
using Microsoft.Exchange.UM.ClientAccess;
using Microsoft.Exchange.UM.ClientAccess.Messages;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.Management.Tasks.UM
{
	[Cmdlet("Export", "UMMailboxPrompt", SupportsShouldProcess = true, DefaultParameterSetName = "DefaultVoicemailGreeting")]
	public sealed class ExportUMMailboxPrompt : RecipientObjectActionTask<MailboxIdParameter, ADUser>
	{
		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = true, ParameterSetName = "CustomAwayGreeting", Position = 0, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
		[Parameter(Mandatory = true, ParameterSetName = "DefaultVoicemailGreeting", Position = 0, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
		[Parameter(Mandatory = true, ParameterSetName = "DefaultAwayGreeting", Position = 0, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
		[Parameter(Mandatory = true, ParameterSetName = "CustomVoicemailGreeting", Position = 0, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
		public override MailboxIdParameter Identity
		{
			get
			{
				return (MailboxIdParameter)base.Fields["Identity"];
			}
			set
			{
				base.Fields["Identity"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "DefaultVoicemailGreeting")]
		public SwitchParameter DefaultVoicemailGreeting
		{
			get
			{
				return (SwitchParameter)(base.Fields["DefaultVoicemailGreeting"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["DefaultVoicemailGreeting"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "DefaultAwayGreeting")]
		public SwitchParameter DefaultAwayGreeting
		{
			get
			{
				return (SwitchParameter)(base.Fields["DefaultAwayGreeting"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["DefaultAwayGreeting"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "CustomVoicemailGreeting")]
		public SwitchParameter CustomVoicemailGreeting
		{
			get
			{
				return (SwitchParameter)(base.Fields["CustomVoicemailGreeting"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["CustomVoicemailGreeting"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "CustomAwayGreeting")]
		public SwitchParameter CustomAwayGreeting
		{
			get
			{
				return (SwitchParameter)(base.Fields["CustomAwayGreeting"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["CustomAwayGreeting"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "DefaultAwayGreeting")]
		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = false, ParameterSetName = "DefaultVoicemailGreeting")]
		public string TestPhoneticDisplayName
		{
			get
			{
				return (string)base.Fields["TestPhoneticDisplayName"];
			}
			set
			{
				base.Fields["TestPhoneticDisplayName"] = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageExportUMMailboxPrompt(base.ParameterSetName);
			}
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			base.InternalProcessRecord();
			using (UMSubscriber umsubscriber = UMRecipient.Factory.FromADRecipient<UMSubscriber>(this.DataObject))
			{
				if (umsubscriber != null)
				{
					UMMailboxPromptRpcRequest request = null;
					string empty = string.Empty;
					string parameterSetName;
					if ((parameterSetName = base.ParameterSetName) != null)
					{
						if (parameterSetName == "DefaultVoicemailGreeting")
						{
							request = UMMailboxPromptRpcRequest.CreateVoicemailPromptRequest(this.DataObject, this.TestPhoneticDisplayName);
							goto IL_CD;
						}
						if (parameterSetName == "DefaultAwayGreeting")
						{
							request = UMMailboxPromptRpcRequest.CreateAwayPromptRequest(this.DataObject, this.TestPhoneticDisplayName);
							goto IL_CD;
						}
						if (parameterSetName == "CustomVoicemailGreeting")
						{
							request = UMMailboxPromptRpcRequest.CreateCustomVoicemailPromptRequest(this.DataObject);
							goto IL_CD;
						}
						if (parameterSetName == "CustomAwayGreeting")
						{
							request = UMMailboxPromptRpcRequest.CreateCustomAwayPromptRequest(this.DataObject);
							goto IL_CD;
						}
					}
					ExAssert.RetailAssert(false, "Invalid parameter set {0}", new object[]
					{
						base.ParameterSetName
					});
					try
					{
						IL_CD:
						base.WriteObject(new UMPrompt(this.DataObject.Identity)
						{
							AudioData = this.GetUMPromptPreview(request, ((ADObjectId)umsubscriber.DialPlan.Identity).ObjectGuid),
							Name = base.ParameterSetName
						});
						goto IL_13D;
					}
					catch (LocalizedException exception)
					{
						base.WriteError(exception, ErrorCategory.NotSpecified, null);
						goto IL_13D;
					}
				}
				base.WriteError(new UserNotUmEnabledException(this.Identity.ToString()), (ErrorCategory)1000, null);
				IL_13D:;
			}
			TaskLogger.LogExit();
		}

		private byte[] GetUMPromptPreview(PromptPreviewRpcRequest request, Guid dialPlanGuid)
		{
			UMPromptPreviewRpcTarget umpromptPreviewRpcTarget = (UMPromptPreviewRpcTarget)UMPromptPreviewRpcTargetPicker.Instance.PickNextServer(dialPlanGuid);
			if (umpromptPreviewRpcTarget == null)
			{
				throw new RpcUMServerNotFoundException();
			}
			byte[] audioData;
			try
			{
				ResponseBase responseBase = umpromptPreviewRpcTarget.ExecuteRequest(request) as ResponseBase;
				if (responseBase == null)
				{
					throw new InvalidResponseException(umpromptPreviewRpcTarget.Name, string.Empty);
				}
				ErrorResponse errorResponse = responseBase as ErrorResponse;
				if (errorResponse != null)
				{
					throw errorResponse.GetException();
				}
				PromptPreviewRpcResponse promptPreviewRpcResponse = responseBase as PromptPreviewRpcResponse;
				if (promptPreviewRpcResponse == null)
				{
					throw new InvalidResponseException(umpromptPreviewRpcTarget.Name, string.Empty);
				}
				audioData = promptPreviewRpcResponse.AudioData;
			}
			catch (RpcException ex)
			{
				throw new InvalidResponseException(umpromptPreviewRpcTarget.Name, ex.Message, ex);
			}
			return audioData;
		}

		internal abstract class ParameterSet
		{
			internal const string DefaultVoicemailGreeting = "DefaultVoicemailGreeting";

			internal const string DefaultAwayGreeting = "DefaultAwayGreeting";

			internal const string CustomVoicemailGreeting = "CustomVoicemailGreeting";

			internal const string CustomAwayGreeting = "CustomAwayGreeting";
		}
	}
}
