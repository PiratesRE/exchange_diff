using System;

namespace Microsoft.Exchange.AirSync
{
	internal class OptionsCommand : Command
	{
		internal OptionsCommand()
		{
			base.PerfCounter = AirSyncCounters.NumberOfOptions;
		}

		internal override bool RequiresPolicyCheck
		{
			get
			{
				return false;
			}
		}

		internal override int MinVersion
		{
			get
			{
				return 0;
			}
		}

		protected override bool ShouldOpenSyncState
		{
			get
			{
				return false;
			}
		}

		protected sealed override string RootNodeName
		{
			get
			{
				return "Invalid";
			}
		}

		internal override Command.ExecutionState ExecuteCommand()
		{
			this.AddHeaders();
			base.XmlResponse = null;
			return Command.ExecutionState.Complete;
		}

		protected override bool HandleQuarantinedState()
		{
			throw new InvalidOperationException("Options command should always be allowed!");
		}

		private void AddHeaders()
		{
			if (base.User.IsConsumerOrganizationUser)
			{
				base.AddHeadersForConsumerOrgUser();
			}
			else
			{
				base.AddHeadersForEnterpriseOrgUser();
			}
			base.Context.Response.AppendHeader("Public", "OPTIONS,POST");
			base.Context.Response.AppendHeader("Allow", "OPTIONS,POST");
		}
	}
}
