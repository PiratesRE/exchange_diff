using System;
using System.Management.Automation;
using System.Security.Permissions;
using System.ServiceModel.Activation;
using Microsoft.Exchange.Configuration.ObjectModel;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks.UM;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	public sealed class UMMailboxService : DataSourceService, IUMMailboxService, IEditObjectService<SetUMMailboxConfiguration, SetUMMailboxParameters>, IGetObjectService<SetUMMailboxConfiguration>, INewGetObjectService<UMMailboxFeatureInfo, NewUMMailboxParameters, RecipientRow>, INewObjectService<UMMailboxFeatureInfo, NewUMMailboxParameters>, IRemoveObjectsService, IRemoveObjectsService<BaseWebServiceParameters>
	{
		[PrincipalPermission(SecurityAction.Demand, Role = "Get-Recipient?Identity@R:Organization")]
		public PowerShellResults<RecipientRow> GetObjectForNew(Identity identity)
		{
			PSCommand pscommand = new PSCommand().AddCommand("Get-Recipient");
			pscommand.AddParameter("Identity", identity);
			return base.GetObject<RecipientRow>(pscommand);
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "Enable-UMMailbox?Identity&UMMailboxPolicy&ValidateOnly&Extensions@W:Organization+Get-UMMailboxPolicy?Identity@R:Organization")]
		public PowerShellResults<NewUMMailboxConfiguration> GetConfigurationForNewUMMailbox(Identity identity, UMEnableSelectedPolicyParameters properties)
		{
			properties.FaultIfNull();
			PSCommand pscommand = new PSCommand().AddCommand("Enable-UMMailbox");
			pscommand.AddParameter("Identity", identity);
			pscommand.AddParameter("ValidateOnly");
			if (properties.UMMailboxPolicy != null)
			{
				pscommand.AddParameter("UMMailboxPolicy", properties.UMMailboxPolicy);
			}
			PowerShellResults<NewUMMailboxConfiguration> powerShellResults = base.Invoke<NewUMMailboxConfiguration>(pscommand);
			if (!powerShellResults.Succeeded)
			{
				powerShellResults.ErrorRecords = Array.FindAll<ErrorRecord>(powerShellResults.ErrorRecords, (ErrorRecord x) => !(x.Exception is CouldNotGenerateExtensionException) && !(x.Exception is SipResourceIdAndExtensionsNeededException) && !(x.Exception is E164ResourceIdNeededException));
			}
			if (powerShellResults.SucceededWithValue)
			{
				PowerShellResults<UMMailboxPolicy> powerShellResults2 = powerShellResults.MergeErrors<UMMailboxPolicy>(base.GetObject<UMMailboxPolicy>("Get-UMMailboxPolicy", properties.UMMailboxPolicy));
				if (powerShellResults2.SucceededWithValue)
				{
					powerShellResults.Value.Policy = powerShellResults2.Value;
				}
			}
			return powerShellResults;
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "Enable-UMMailbox?Identity&UMMailboxPolicy&ValidateOnly&Extensions@W:Organization")]
		public PowerShellResults<UMMailboxFeatureInfo> NewObject(NewUMMailboxParameters properties)
		{
			PowerShellResults<UMMailboxFeatureInfo> powerShellResults = base.NewObject<UMMailboxFeatureInfo, NewUMMailboxParameters>("Enable-UMMailbox", properties);
			PowerShellResults<SetUMMailboxConfiguration> @object = this.GetObject(properties.Identity);
			if (powerShellResults.SucceededWithValue && @object.SucceededWithValue)
			{
				powerShellResults.Output[0].WhenChanged = @object.Output[0].WhenChanged;
			}
			return powerShellResults;
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "Get-UMMailbox?Identity@R:Organization+Get-UMMailboxPin?Identity@R:Organization")]
		public PowerShellResults<SetUMMailboxConfiguration> GetObject(Identity identity)
		{
			PowerShellResults<SetUMMailboxConfiguration> @object = base.GetObject<SetUMMailboxConfiguration>("Get-UMMailbox", identity);
			if (@object.SucceededWithValue)
			{
				PowerShellResults<UMMailboxPin> powerShellResults = @object.MergeErrors<UMMailboxPin>(base.GetObject<UMMailboxPin>("Get-UMMailboxPin", identity));
				if (powerShellResults.SucceededWithValue)
				{
					@object.Value.UMMailboxPin = powerShellResults.Value;
				}
			}
			return @object;
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "Get-UMMailbox?Identity@R:Organization+Get-UMMailboxPin?Identity@R:Organization+Set-Mailbox?Identity&EmailAddresses@W:Organization")]
		[PrincipalPermission(SecurityAction.Demand, Role = "Get-UMMailbox?Identity@R:Organization+Get-UMMailboxPin?Identity@R:Organization+Set-UMMailbox?Identity@W:Organization")]
		public PowerShellResults<SetUMMailboxConfiguration> SetObject(Identity identity, SetUMMailboxParameters properties)
		{
			PowerShellResults<SetUMMailboxConfiguration> powerShellResults = new PowerShellResults<SetUMMailboxConfiguration>();
			properties.FaultIfNull();
			properties.ReturnObjectType = ReturnObjectTypes.Full;
			if (properties.SetUMExtensionParameteres.SecondaryExtensions != null)
			{
				PowerShellResults<SetUMMailboxConfiguration> @object = this.GetObject(identity);
				powerShellResults.MergeErrors<SetUMMailboxConfiguration>(@object);
				if (@object.HasValue)
				{
					try
					{
						properties.SetUMExtensionParameteres.UpdateSecondaryExtensions(@object.Output[0].UMMailbox);
					}
					catch (ProxyAddressExistsException exception)
					{
						powerShellResults.ErrorRecords = new ErrorRecord[]
						{
							new ErrorRecord(exception)
						};
						return powerShellResults;
					}
				}
				PSCommand pscommand = new PSCommand().AddCommand("Set-Mailbox");
				pscommand.AddParameter("Identity", identity);
				pscommand.AddParameters(properties.SetUMExtensionParameteres);
				powerShellResults.MergeErrors(base.Invoke(pscommand));
				if (powerShellResults.Failed)
				{
					return powerShellResults;
				}
			}
			return base.SetObject<SetUMMailboxConfiguration, SetUMMailboxParameters>("Set-UMMailbox", identity, properties);
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "Disable-UMMailbox?Identity&Confirm@W:Organization")]
		public PowerShellResults RemoveObjects(Identity[] identities, BaseWebServiceParameters parameters)
		{
			return base.RemoveObjects("Disable-UMMailbox", identities, parameters);
		}

		private const string GetDataForNewUMMailboxRole = "Get-Recipient?Identity@R:Organization";

		private const string NewUMMailboxRole = "Enable-UMMailbox?Identity&UMMailboxPolicy&ValidateOnly&Extensions@W:Organization";

		private const string GetConfigurationForNewUMMailboxRole = "Enable-UMMailbox?Identity&UMMailboxPolicy&ValidateOnly&Extensions@W:Organization+Get-UMMailboxPolicy?Identity@R:Organization";

		private const string GetUMMailboxRole = "Get-UMMailbox?Identity@R:Organization+Get-UMMailboxPin?Identity@R:Organization";

		private const string SetUMMailboxRole = "Get-UMMailbox?Identity@R:Organization+Get-UMMailboxPin?Identity@R:Organization+Set-UMMailbox?Identity@W:Organization";

		private const string SetMailboxRole = "Get-UMMailbox?Identity@R:Organization+Get-UMMailboxPin?Identity@R:Organization+Set-Mailbox?Identity&EmailAddresses@W:Organization";

		internal const string RemoveUMMailboxRole = "Disable-UMMailbox?Identity&Confirm@W:Organization";
	}
}
