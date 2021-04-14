using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Enable", "AntispamUpdates", SupportsShouldProcess = true)]
	public class EnableAntispamUpdates : Task
	{
		[Parameter(Mandatory = false, Position = 0, ValueFromPipeline = true)]
		public virtual ServerIdParameter Identity
		{
			get
			{
				return (ServerIdParameter)base.Fields["Identity"];
			}
			set
			{
				base.Fields["Identity"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool SpamSignatureUpdatesEnabled
		{
			get
			{
				return (bool)(base.Fields["SpamSignatureUpdatesEnabled"] ?? false);
			}
			set
			{
				base.Fields["SpamSignatureUpdatesEnabled"] = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageEnableAntiSpamUpdates;
			}
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			this.WriteWarning(Strings.EnableAntiSpamUpdatesDeprecated);
			TaskLogger.LogExit();
		}

		private const string CmdletNoun = "AntispamUpdates";

		private const string ParamSpamSignatureUpdatesEnabled = "SpamSignatureUpdatesEnabled";
	}
}
