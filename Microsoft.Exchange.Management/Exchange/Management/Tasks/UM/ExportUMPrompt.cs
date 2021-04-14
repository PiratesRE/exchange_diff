using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Rpc;
using Microsoft.Exchange.UM.ClientAccess;
using Microsoft.Exchange.UM.ClientAccess.Messages;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.Management.Tasks.UM
{
	[Cmdlet("Export", "UMPrompt", SupportsShouldProcess = true, DefaultParameterSetName = "BusinessHoursWelcomeGreeting")]
	public sealed class ExportUMPrompt : UMPromptTaskBase<UMDialPlanIdParameter>
	{
		private new UMDialPlanIdParameter Identity
		{
			get
			{
				return base.Identity;
			}
			set
			{
				base.Identity = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "AfterHoursWelcomeGreeting")]
		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = true, ParameterSetName = "AACustomGreeting")]
		[Parameter(Mandatory = true, ParameterSetName = "AfterHoursWelcomeGreetingAndMenu")]
		[Parameter(Mandatory = true, ParameterSetName = "BusinessHours")]
		[Parameter(Mandatory = true, ParameterSetName = "BusinessLocation")]
		[Parameter(Mandatory = true, ParameterSetName = "BusinessHoursWelcomeGreeting")]
		[Parameter(Mandatory = true, ParameterSetName = "BusinessHoursWelcomeGreetingAndMenu")]
		public override UMAutoAttendantIdParameter UMAutoAttendant
		{
			get
			{
				return (UMAutoAttendantIdParameter)base.Fields["UMAutoAttendant"];
			}
			set
			{
				base.Fields["UMAutoAttendant"] = value;
			}
		}

		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = true, ParameterSetName = "DPCustomGreeting")]
		public override UMDialPlanIdParameter UMDialPlan
		{
			get
			{
				return this.Identity;
			}
			set
			{
				this.Identity = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "BusinessHours")]
		public SwitchParameter BusinessHours
		{
			get
			{
				return (SwitchParameter)(base.Fields["BusinessHours"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["BusinessHours"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "BusinessLocation")]
		public SwitchParameter BusinessLocation
		{
			get
			{
				return (SwitchParameter)(base.Fields["BusinessLocation"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["BusinessLocation"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "BusinessHoursWelcomeGreeting")]
		public SwitchParameter BusinessHoursWelcomeGreeting
		{
			get
			{
				return (SwitchParameter)(base.Fields["BusinessHoursWelcomeGreeting"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["BusinessHoursWelcomeGreeting"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "BusinessHoursWelcomeGreetingAndMenu")]
		public SwitchParameter BusinessHoursWelcomeGreetingAndMenu
		{
			get
			{
				return (SwitchParameter)(base.Fields["BusinessHoursWelcomeGreetingAndMenu"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["BusinessHoursWelcomeGreetingAndMenu"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "AfterHoursWelcomeGreeting")]
		public SwitchParameter AfterHoursWelcomeGreeting
		{
			get
			{
				return (SwitchParameter)(base.Fields["AfterHoursWelcomeGreeting"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["AfterHoursWelcomeGreeting"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "AfterHoursWelcomeGreetingAndMenu")]
		public SwitchParameter AfterHoursWelcomeGreetingAndMenu
		{
			get
			{
				return (SwitchParameter)(base.Fields["AfterHoursWelcomeGreetingAndMenu"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["AfterHoursWelcomeGreetingAndMenu"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "AACustomGreeting")]
		[Parameter(Mandatory = true, ParameterSetName = "DPCustomGreeting")]
		public string PromptFileName
		{
			get
			{
				return (string)base.Fields["PromptFileName"];
			}
			set
			{
				base.Fields["PromptFileName"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "AfterHoursWelcomeGreeting")]
		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = false, ParameterSetName = "BusinessHoursWelcomeGreeting")]
		public string TestBusinessName
		{
			get
			{
				return (string)base.Fields["TestBusinessName"];
			}
			set
			{
				base.Fields["TestBusinessName"] = value;
			}
		}

		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = false, ParameterSetName = "AfterHoursWelcomeGreetingAndMenu")]
		[Parameter(Mandatory = false, ParameterSetName = "BusinessHoursWelcomeGreetingAndMenu")]
		public CustomMenuKeyMapping[] TestMenuKeyMapping
		{
			get
			{
				return (CustomMenuKeyMapping[])base.Fields["TestMenuKeyMapping"];
			}
			set
			{
				base.Fields["TestMenuKeyMapping"] = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				string parameterSetName;
				switch (parameterSetName = base.ParameterSetName)
				{
				case "AACustomGreeting":
				case "AfterHoursWelcomeGreeting":
				case "AfterHoursWelcomeGreetingAndMenu":
				case "BusinessHours":
				case "BusinessLocation":
				case "BusinessHoursWelcomeGreeting":
				case "BusinessHoursWelcomeGreetingAndMenu":
					return Strings.ConfirmationMessageExportUMPromptAutoAttendantPrompts(this.PromptFileName, this.UMAutoAttendant.ToString());
				case "DPCustomGreeting":
					return Strings.ConfirmationMessageExportUMPromptDialPlanPrompts(this.PromptFileName, this.UMDialPlan.ToString());
				}
				ExAssert.RetailAssert(false, "Invalid parameter set {0}", new object[]
				{
					base.ParameterSetName
				});
				return new LocalizedString(string.Empty);
			}
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			PromptPreviewRpcRequest request = null;
			ObjectId identity = null;
			string parameterSetName;
			switch (parameterSetName = base.ParameterSetName)
			{
			case "AACustomGreeting":
				request = new UMAACustomPromptRpcRequest(base.AutoAttendant, this.PromptFileName);
				identity = base.AutoAttendant.Identity;
				goto IL_1ED;
			case "AfterHoursWelcomeGreeting":
				request = UMAAWelcomePromptRpcRequest.AfterHoursWithCustomBusinessName(base.AutoAttendant, this.TestBusinessName);
				identity = base.AutoAttendant.Identity;
				goto IL_1ED;
			case "AfterHoursWelcomeGreetingAndMenu":
				request = UMAAWelcomePromptRpcRequest.AfterHoursWithCustomKeyMapping(base.AutoAttendant, this.TestMenuKeyMapping);
				identity = base.AutoAttendant.Identity;
				goto IL_1ED;
			case "BusinessHours":
				request = new UMAABusinessHoursPromptRpcRequest(base.AutoAttendant);
				identity = base.AutoAttendant.Identity;
				goto IL_1ED;
			case "BusinessLocation":
				request = new UMAABusinessLocationPromptRpcRequest(base.AutoAttendant);
				identity = base.AutoAttendant.Identity;
				goto IL_1ED;
			case "BusinessHoursWelcomeGreeting":
				request = UMAAWelcomePromptRpcRequest.BusinessHoursWithCustomBusinessName(base.AutoAttendant, this.TestBusinessName);
				identity = base.AutoAttendant.Identity;
				goto IL_1ED;
			case "BusinessHoursWelcomeGreetingAndMenu":
				request = UMAAWelcomePromptRpcRequest.BusinessHoursWithCustomKeyMapping(base.AutoAttendant, this.TestMenuKeyMapping);
				identity = base.AutoAttendant.Identity;
				goto IL_1ED;
			case "DPCustomGreeting":
				request = new UMDPCustomPromptRpcRequest(this.DataObject, this.PromptFileName);
				identity = this.DataObject.Identity;
				goto IL_1ED;
			}
			ExAssert.RetailAssert(false, "Invalid parameter set {0}", new object[]
			{
				base.ParameterSetName
			});
			try
			{
				IL_1ED:
				ADObjectId adobjectId = (base.AutoAttendant == null) ? ((ADObjectId)this.DataObject.Identity) : base.AutoAttendant.UMDialPlan;
				UMPrompt umprompt = new UMPrompt(identity);
				umprompt.AudioData = this.GetUMPromptPreview(request, adobjectId.ObjectGuid);
				if (base.ParameterSetName == "DPCustomGreeting" || base.ParameterSetName == "AACustomGreeting")
				{
					umprompt.Name = this.PromptFileName;
				}
				else
				{
					umprompt.Name = base.ParameterSetName;
				}
				base.WriteObject(umprompt);
			}
			catch (LocalizedException exception)
			{
				base.WriteError(exception, ErrorCategory.NotSpecified, null);
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
			internal const string AfterHoursWelcomeGreeting = "AfterHoursWelcomeGreeting";

			internal const string AfterHoursWelcomeGreetingAndMenu = "AfterHoursWelcomeGreetingAndMenu";

			internal const string BusinessHours = "BusinessHours";

			internal const string BusinessLocation = "BusinessLocation";

			internal const string BusinessHoursWelcomeGreeting = "BusinessHoursWelcomeGreeting";

			internal const string BusinessHoursWelcomeGreetingAndMenu = "BusinessHoursWelcomeGreetingAndMenu";

			internal const string AACustomGreeting = "AACustomGreeting";

			internal const string DPCustomGreeting = "DPCustomGreeting";
		}
	}
}
