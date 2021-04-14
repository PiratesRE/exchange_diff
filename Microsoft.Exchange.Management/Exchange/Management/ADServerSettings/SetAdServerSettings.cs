using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.ADServerSettings
{
	[Cmdlet("Set", "AdServerSettings", DefaultParameterSetName = "FullParams", SupportsShouldProcess = true)]
	public sealed class SetAdServerSettings : Task
	{
		[Parameter(Mandatory = false, Position = 0, ParameterSetName = "SingleDC")]
		public Fqdn PreferredServer
		{
			get
			{
				return (Fqdn)base.Fields["PreferredServer"];
			}
			set
			{
				base.Fields["PreferredServer"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "FullParams")]
		public Fqdn ConfigurationDomainController
		{
			get
			{
				return (Fqdn)base.Fields["ConfigurationDomainController"];
			}
			set
			{
				base.Fields["ConfigurationDomainController"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "FullParams")]
		public Fqdn PreferredGlobalCatalog
		{
			get
			{
				return (Fqdn)base.Fields["PreferredGlobalCatalog"];
			}
			set
			{
				base.Fields["PreferredGlobalCatalog"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "FullParams")]
		public MultiValuedProperty<Fqdn> SetPreferredDomainControllers
		{
			get
			{
				return (MultiValuedProperty<Fqdn>)base.Fields["SetPreferredDomainControllers"];
			}
			set
			{
				base.Fields["SetPreferredDomainControllers"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "FullParams")]
		[Parameter(Mandatory = false, ParameterSetName = "SingleDC")]
		public string RecipientViewRoot
		{
			get
			{
				return (string)base.Fields["RecipientViewRoot"];
			}
			set
			{
				base.Fields["RecipientViewRoot"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "FullParams")]
		[Parameter(Mandatory = false, ParameterSetName = "SingleDC")]
		public bool ViewEntireForest
		{
			get
			{
				return (bool)(base.Fields["ViewEntireForest"] ?? false);
			}
			set
			{
				base.Fields["ViewEntireForest"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool WriteOriginatingChangeTimestamp
		{
			get
			{
				return (bool)(base.Fields["WriteOriginatingChangeTimestamp"] ?? false);
			}
			set
			{
				base.Fields["WriteOriginatingChangeTimestamp"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool WriteShadowProperties
		{
			get
			{
				return (bool)(base.Fields["WriteShadowProperties"] ?? false);
			}
			set
			{
				base.Fields["WriteShadowProperties"] = value;
			}
		}

		[Parameter(Mandatory = true, ValueFromPipeline = true, ParameterSetName = "Instance")]
		public RunspaceServerSettingsPresentationObject RunspaceServerSettings
		{
			get
			{
				return (RunspaceServerSettingsPresentationObject)base.Fields["Instance"];
			}
			set
			{
				base.Fields["Instance"] = value;
			}
		}

		[Parameter(Mandatory = true, ValueFromPipeline = false, ParameterSetName = "Gls")]
		public bool DisableGls
		{
			get
			{
				return (bool)base.Fields["DisableGls"];
			}
			set
			{
				base.Fields["DisableGls"] = value;
			}
		}

		[Parameter(Mandatory = true, ValueFromPipeline = false, ParameterSetName = "ForceADInTemplateScope")]
		public bool ForceADInTemplateScope
		{
			get
			{
				return (bool)base.Fields["ForceADInTemplateScope"];
			}
			set
			{
				base.Fields["ForceADInTemplateScope"] = value;
			}
		}

		[Parameter(Mandatory = true, ValueFromPipeline = false, ParameterSetName = "Reset")]
		public SwitchParameter ResetSettings
		{
			get
			{
				return (SwitchParameter)(base.Fields["Reset"] ?? false);
			}
			set
			{
				base.Fields["Reset"] = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				if (this.ResetSettings)
				{
					return Strings.ConfirmationMessageResetADServerSettings;
				}
				return Strings.ConfirmationMessageSetADServerSettings;
			}
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.InternalValidate();
			if (base.HasErrors)
			{
				return;
			}
			if (!(base.ServerSettings is RunspaceServerSettings))
			{
				this.WriteWarning(Strings.WarningSettingsNotModifiable);
				this.settingsNotModifiable = true;
				return;
			}
			if (base.ServerSettings == null && (this.RunspaceServerSettings == null || this.RunspaceServerSettings.RawServerSettings == null))
			{
				base.WriteError(new InvalidOperationException(Strings.ErrorRunspaceServerSettingsNotFound), ErrorCategory.InvalidOperation, this);
			}
			if (base.Fields.IsModified("Instance"))
			{
				this.modifiedServerSettings = ((this.RunspaceServerSettings.RawServerSettings != null) ? this.RunspaceServerSettings.RawServerSettings : ((RunspaceServerSettings)base.ServerSettings.Clone()));
				this.ConfigurationDomainController = this.RunspaceServerSettings.UserPreferredConfigurationDomainController;
				this.PreferredGlobalCatalog = this.RunspaceServerSettings.UserPreferredGlobalCatalog;
				this.RecipientViewRoot = this.RunspaceServerSettings.RecipientViewRoot;
				this.SetPreferredDomainControllers = this.RunspaceServerSettings.UserPreferredDomainControllers;
				this.ViewEntireForest = this.RunspaceServerSettings.ViewEntireForest;
				this.WriteOriginatingChangeTimestamp = this.RunspaceServerSettings.WriteOriginatingChangeTimestamp;
				this.WriteShadowProperties = this.RunspaceServerSettings.WriteShadowProperties;
			}
			else
			{
				this.modifiedServerSettings = (RunspaceServerSettings)base.ServerSettings.Clone();
			}
			if (base.Fields.IsModified("ViewEntireForest"))
			{
				if (base.IsVerboseOn)
				{
					base.WriteVerbose(Strings.VerboseSettingViewEntireForest(this.ViewEntireForest.ToString()));
				}
				this.modifiedServerSettings.ViewEntireForest = this.ViewEntireForest;
				if (this.ViewEntireForest)
				{
					if (!string.IsNullOrEmpty(this.RecipientViewRoot))
					{
						base.WriteError(new ArgumentException(Strings.ErrorSetRecipientViewRootAndViewEntireForestToTrue), ErrorCategory.InvalidArgument, this);
					}
				}
				else if (!string.IsNullOrEmpty(this.RecipientViewRoot))
				{
					this.VerifyAndSetRecipientViewRoot(this.RecipientViewRoot);
				}
				else if (base.Fields.IsModified("RecipientViewRoot"))
				{
					base.WriteError(new ArgumentException(Strings.ErrorRecipientViewRootEmptyAndViewEntireForestToFalse), ErrorCategory.InvalidArgument, this);
				}
				else if (base.ScopeSet.RecipientReadScope != null && base.ScopeSet.RecipientReadScope.Root != null)
				{
					this.modifiedServerSettings.RecipientViewRoot = base.ScopeSet.RecipientReadScope.Root;
				}
			}
			else if (base.Fields.IsModified("RecipientViewRoot"))
			{
				if (!string.IsNullOrEmpty(this.RecipientViewRoot))
				{
					if (base.IsVerboseOn)
					{
						base.WriteVerbose(Strings.VerboseSettingViewEntireForest(false.ToString()));
					}
					this.modifiedServerSettings.ViewEntireForest = false;
					this.VerifyAndSetRecipientViewRoot(this.RecipientViewRoot);
				}
				else
				{
					if (base.IsVerboseOn)
					{
						base.WriteVerbose(Strings.VerboseSettingViewEntireForest(true.ToString()));
					}
					this.modifiedServerSettings.ViewEntireForest = true;
				}
			}
			if (this.PreferredServer != null)
			{
				if (base.IsVerboseOn)
				{
					base.WriteVerbose(Strings.VerboseSettingPreferredServer("PreferredGlobalCatalog", this.PreferredServer));
				}
				this.modifiedServerSettings.SetUserPreferredGlobalCatalog(this.PreferredServer);
				if (base.IsVerboseOn)
				{
					base.WriteVerbose(Strings.VerboseSettingPreferredServer("ConfigurationDomainController", this.PreferredServer));
				}
				this.modifiedServerSettings.SetUserConfigurationDomainController(this.PreferredServer);
				if (base.IsVerboseOn)
				{
					base.WriteVerbose(Strings.VerboseSettingPreferredServer("SetPreferredDomainControllers", this.PreferredServer));
				}
				ADObjectId adobjectId;
				Fqdn fqdn;
				this.modifiedServerSettings.AddOrReplaceUserPreferredDC(this.PreferredServer, out adobjectId, out fqdn);
			}
			if (base.Fields.IsModified("PreferredGlobalCatalog"))
			{
				if (this.PreferredGlobalCatalog == null)
				{
					if (base.IsVerboseOn)
					{
						base.WriteVerbose(Strings.VerboseClearPreferredServer("PreferredGlobalCatalog"));
					}
					this.modifiedServerSettings.ClearUserPreferredGlobalCatalog();
				}
				else if (!string.Equals(this.PreferredGlobalCatalog, this.modifiedServerSettings.UserPreferredGlobalCatalog, StringComparison.OrdinalIgnoreCase))
				{
					if (base.IsVerboseOn)
					{
						base.WriteVerbose(Strings.VerboseSettingPreferredServer("PreferredGlobalCatalog", this.PreferredGlobalCatalog));
					}
					this.modifiedServerSettings.SetUserPreferredGlobalCatalog(this.PreferredGlobalCatalog);
				}
			}
			if (base.Fields.IsModified("ConfigurationDomainController"))
			{
				if (this.ConfigurationDomainController == null)
				{
					if (base.IsVerboseOn)
					{
						base.WriteVerbose(Strings.VerboseClearPreferredServer("ConfigurationDomainController"));
					}
					this.modifiedServerSettings.ClearUserConfigurationDomainController();
				}
				else if (!string.Equals(this.ConfigurationDomainController, this.modifiedServerSettings.UserConfigurationDomainController, StringComparison.OrdinalIgnoreCase))
				{
					if (base.IsVerboseOn)
					{
						base.WriteVerbose(Strings.VerboseSettingPreferredServer("ConfigurationDomainController", this.ConfigurationDomainController));
					}
					this.modifiedServerSettings.SetUserConfigurationDomainController(this.ConfigurationDomainController);
				}
			}
			if (base.Fields.IsModified("SetPreferredDomainControllers"))
			{
				if (this.SetPreferredDomainControllers == null || this.SetPreferredDomainControllers.Count == 0)
				{
					if (base.IsVerboseOn)
					{
						base.WriteVerbose(Strings.VerboseClearAllPreferredDC);
					}
					this.modifiedServerSettings.ClearAllUserPreferredDCs();
				}
				else
				{
					foreach (Fqdn fqdn2 in this.SetPreferredDomainControllers)
					{
						if (!this.modifiedServerSettings.UserPreferredDomainControllers.Contains(fqdn2))
						{
							if (base.IsVerboseOn)
							{
								base.WriteVerbose(Strings.VerboseSettingPreferredServer("SetPreferredDomainControllers", fqdn2));
							}
							ADObjectId adobjectId2 = null;
							Fqdn fqdn3 = null;
							this.modifiedServerSettings.AddOrReplaceUserPreferredDC(fqdn2, out adobjectId2, out fqdn3);
							if (fqdn3 != null)
							{
								this.WriteWarning(Strings.WarningPreferredServerReplaced(fqdn2, fqdn3, adobjectId2.ToCanonicalName()));
							}
						}
					}
				}
			}
			if (base.Fields.IsModified("WriteOriginatingChangeTimestamp"))
			{
				if (base.IsVerboseOn)
				{
					base.WriteVerbose(Strings.VerboseSettingWriteOriginatingChangeTimestamp(this.WriteOriginatingChangeTimestamp.ToString()));
				}
				this.modifiedServerSettings.WriteOriginatingChangeTimestamp = this.WriteOriginatingChangeTimestamp;
			}
			if (base.Fields.IsModified("WriteShadowProperties"))
			{
				if (base.IsVerboseOn)
				{
					base.WriteVerbose(Strings.VerboseSettingWriteShadowProperties(this.WriteOriginatingChangeTimestamp.ToString()));
				}
				this.modifiedServerSettings.WriteShadowProperties = this.WriteShadowProperties;
			}
			if (base.Fields.IsModified("DisableGls"))
			{
				if (base.IsVerboseOn)
				{
					base.WriteVerbose(Strings.VerboseSettingDisableGls(this.DisableGls.ToString()));
				}
				this.modifiedServerSettings.DisableGls = this.DisableGls;
			}
			if (base.Fields.IsModified("ForceADInTemplateScope"))
			{
				if (base.IsVerboseOn)
				{
					base.WriteVerbose(Strings.VerboseSettingDisableAggregation(this.ForceADInTemplateScope.ToString()));
				}
				this.modifiedServerSettings.ForceADInTemplateScope = this.ForceADInTemplateScope;
			}
			TaskLogger.LogExit();
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			base.InternalProcessRecord();
			if (!this.settingsNotModifiable)
			{
				bool flag = true;
				RunspaceServerSettings runspaceServerSettings = (RunspaceServerSettings)base.ServerSettings;
				if (runspaceServerSettings != null)
				{
					flag = !runspaceServerSettings.Equals(this.modifiedServerSettings);
				}
				if (flag)
				{
					if (base.IsVerboseOn)
					{
						base.WriteVerbose(Strings.VerboseSaveADServerSettings);
					}
					ExchangePropertyContainer.SetServerSettings(base.SessionState, this.modifiedServerSettings);
					if (base.IsVerboseOn)
					{
						base.WriteVerbose(Strings.VerboseSaveADServerSettingsSucceed);
					}
				}
				else
				{
					this.WriteWarning(Strings.WarningADServerSettingsUnchanged);
				}
			}
			if (this.ResetSettings)
			{
				if (ExchangePropertyContainer.IsContainerInitialized(base.SessionState))
				{
					ExchangePropertyContainer.SetServerSettings(base.SessionState, null);
				}
				base.SessionState.Variables[ExchangePropertyContainer.ADServerSettingsVarName] = null;
				if (base.IsVerboseOn)
				{
					base.WriteVerbose(Strings.VerboseResetADServerSettingsSucceed);
				}
			}
			TaskLogger.LogExit();
		}

		protected override bool IsKnownException(Exception e)
		{
			return base.IsKnownException(e) || DataAccessHelper.IsDataAccessKnownException(e);
		}

		private void VerifyAndSetRecipientViewRoot(string root)
		{
			if (object.Equals(root, this.modifiedServerSettings.RecipientViewRoot))
			{
				return;
			}
			OrganizationalUnitIdParameter organizationalUnitIdParameter = null;
			try
			{
				organizationalUnitIdParameter = OrganizationalUnitIdParameter.Parse(root);
			}
			catch (ArgumentException exception)
			{
				base.WriteError(exception, ErrorCategory.InvalidArgument, this);
			}
			if (this.configSession == null)
			{
				ADSessionSettings sessionSettings = ADSessionSettings.FromCustomScopeSet(base.ScopeSet, ADSystemConfigurationSession.GetRootOrgContainerIdForLocalForest(), base.CurrentOrganizationId, base.ExecutingUserOrganizationId, true);
				this.configSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(ConsistencyMode.PartiallyConsistent, sessionSettings, 588, "VerifyAndSetRecipientViewRoot", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\ADServerSettings\\SetADServerSettings.cs");
				this.configSession.UseGlobalCatalog = true;
				this.configSession.UseConfigNC = false;
			}
			ADObjectId adobjectId = null;
			if (base.ScopeSet.RecipientReadScope != null && base.ScopeSet.RecipientReadScope.Root != null)
			{
				adobjectId = base.ScopeSet.RecipientReadScope.Root;
			}
			ExchangeOrganizationalUnit exchangeOrganizationalUnit = null;
			if (base.IsVerboseOn)
			{
				base.WriteVerbose(Strings.VerboseVerifyingRecipientViewRoot(root));
			}
			IEnumerable<ExchangeOrganizationalUnit> objects = organizationalUnitIdParameter.GetObjects<ExchangeOrganizationalUnit>(adobjectId, this.configSession);
			using (IEnumerator<ExchangeOrganizationalUnit> enumerator = objects.GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					exchangeOrganizationalUnit = enumerator.Current;
					if (enumerator.MoveNext())
					{
						if (adobjectId != null)
						{
							base.WriteError(new ManagementObjectAmbiguousException(Strings.ErrorRecipientViewRootDuplicatedUnderScope(root, adobjectId.ToString())), ErrorCategory.InvalidArgument, this);
						}
						else
						{
							base.WriteError(new ManagementObjectAmbiguousException(Strings.ErrorRecipientViewRootDuplicated(root)), ErrorCategory.InvalidArgument, this);
						}
					}
				}
				else if (adobjectId != null)
				{
					base.WriteError(new ArgumentException(Strings.ErrorRecipientViewRootNotFoundUnderScope(root, adobjectId.ToString())), ErrorCategory.InvalidArgument, this);
				}
				else
				{
					base.WriteError(new ArgumentException(Strings.ErrorRecipientViewRootNotFound(root)), ErrorCategory.InvalidArgument, this);
				}
			}
			this.modifiedServerSettings.RecipientViewRoot = exchangeOrganizationalUnit.Id;
			if (base.IsVerboseOn)
			{
				base.WriteVerbose(Strings.VerboseVerifyRecipientViewRootSucceed);
			}
		}

		private const string ParameterSetSingleDC = "SingleDC";

		private const string ParameterSetFullParams = "FullParams";

		private const string ParameterSetInstance = "Instance";

		private const string ParameterSetGls = "Gls";

		private const string ParameterSetForceADInTemplateScope = "ForceADInTemplateScope";

		private const string ParameterSetReset = "Reset";

		private const string paramPreferredServer = "PreferredServer";

		private const string paramConfigurationDomainController = "ConfigurationDomainController";

		private const string paramPreferredGlobalCatalog = "PreferredGlobalCatalog";

		private const string paramSetPreferredDomainControllers = "SetPreferredDomainControllers";

		private const string paramRecipientViewRoot = "RecipientViewRoot";

		private const string paramViewEntireForest = "ViewEntireForest";

		private const string paramWriteOriginatingChangeTimestamp = "WriteOriginatingChangeTimestamp";

		private const string paramWriteShadowProperties = "WriteShadowProperties";

		private const string paramInstance = "Instance";

		private const string paramDisableGls = "DisableGls";

		private const string paramForceADInTemplateScope = "ForceADInTemplateScope";

		private const string paramReset = "Reset";

		private RunspaceServerSettings modifiedServerSettings;

		private IConfigurationSession configSession;

		private bool settingsNotModifiable;
	}
}
