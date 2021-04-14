using System;
using System.Collections.Generic;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Configuration.MonadDataProvider;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Setup.Common
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class InstallOrgCfgDataHandler : OrgCfgDataHandler
	{
		public InstallOrgCfgDataHandler(ISetupContext context, MonadConnection connection) : base(context, "Install-ExchangeOrganization", connection)
		{
			SetupLogger.TraceEnter(new object[0]);
			base.WorkUnit.Text = Strings.OrganizationInstallText;
			this.setPrepareSchema = false;
			this.setPrepareOrganization = false;
			this.setPrepareDomain = false;
			this.setPrepareSCT = false;
			this.setPrepareAllDomains = false;
			this.setDomain = null;
			this.setOrganizationName = null;
			SetupLogger.TraceExit();
		}

		public IOrganizationName OrganizationName
		{
			get
			{
				return base.SetupContext.OrganizationName;
			}
			set
			{
				base.SetupContext.OrganizationName = value;
			}
		}

		public bool? ActiveDirectorySplitPermissions
		{
			get
			{
				return base.SetupContext.ActiveDirectorySplitPermissions;
			}
			set
			{
				base.SetupContext.ActiveDirectorySplitPermissions = value;
			}
		}

		private void DetermineParameters()
		{
			SetupLogger.TraceEnter(new object[0]);
			this.setPrepareSchema = false;
			this.setPrepareOrganization = false;
			this.setPrepareDomain = false;
			this.setPrepareSCT = false;
			this.setPrepareAllDomains = false;
			this.setDomain = null;
			this.setOrganizationName = null;
			SetupLogger.Log(Strings.DeterminingOrgPrepParameters);
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = false;
			bool flag5 = false;
			if (base.SetupContext.ParsedArguments.ContainsKey("prepareschema"))
			{
				flag = true;
				SetupLogger.Log(Strings.CommandLineParameterSpecified("prepareschema"));
			}
			if (base.SetupContext.ParsedArguments.ContainsKey("preparead"))
			{
				flag2 = true;
				SetupLogger.Log(Strings.CommandLineParameterSpecified("preparead"));
			}
			if (base.SetupContext.ParsedArguments.ContainsKey("preparesct"))
			{
				flag3 = true;
				SetupLogger.Log(Strings.CommandLineParameterSpecified("preparesct"));
			}
			if (base.SetupContext.ParsedArguments.ContainsKey("preparedomain"))
			{
				flag4 = true;
				SetupLogger.Log(Strings.CommandLineParameterSpecified("preparedomain"));
			}
			if (base.SetupContext.ParsedArguments.ContainsKey("preparealldomains"))
			{
				flag5 = true;
				SetupLogger.Log(Strings.CommandLineParameterSpecified("preparealldomains"));
			}
			if (flag || flag2 || flag3 || flag4 || flag5)
			{
				if (flag)
				{
					this.setPrepareSchema = true;
					SetupLogger.Log(Strings.SettingArgumentBecauseOfCommandLineParameter("prepareschema", "PrepareSchema"));
				}
				if (flag2)
				{
					if (base.SetupContext.IsSchemaUpdateRequired)
					{
						this.setPrepareSchema = true;
						SetupLogger.Log(Strings.SettingArgumentBecauseItIsRequired("PrepareSchema"));
					}
					this.setPrepareOrganization = true;
					SetupLogger.Log(Strings.SettingArgumentBecauseOfCommandLineParameter("preparead", "PrepareOrganization"));
					this.setPrepareDomain = true;
					SetupLogger.Log(Strings.SettingArgumentBecauseOfCommandLineParameter("preparead", "PrepareDomain"));
				}
				if (flag3 && Datacenter.IsMicrosoftHostedOnly(true))
				{
					this.setPrepareSCT = true;
					SetupLogger.Log(Strings.SettingArgumentBecauseOfCommandLineParameter("preparesct", "PrepareSCT"));
					if (base.SetupContext.IsSchemaUpdateRequired)
					{
						this.setPrepareSchema = true;
						SetupLogger.Log(Strings.SettingArgumentBecauseItIsRequired("PrepareSchema"));
					}
					if (base.SetupContext.IsOrgConfigUpdateRequired)
					{
						this.setPrepareOrganization = true;
						SetupLogger.Log(Strings.SettingArgumentBecauseItIsRequired("PrepareOrganization"));
					}
					if (base.SetupContext.IsDomainConfigUpdateRequired)
					{
						this.setPrepareDomain = true;
						SetupLogger.Log(Strings.SettingArgumentBecauseItIsRequired("PrepareDomain"));
					}
				}
				if (flag5)
				{
					this.setPrepareAllDomains = true;
					SetupLogger.Log(Strings.SettingArgumentBecauseOfCommandLineParameter("preparealldomains", "PrepareAllDomains"));
				}
				if (flag4)
				{
					this.setPrepareDomain = true;
					SetupLogger.Log(Strings.SettingArgumentBecauseOfCommandLineParameter("preparedomain", "PrepareDomain"));
				}
			}
			else
			{
				if (base.SetupContext.IsSchemaUpdateRequired)
				{
					this.setPrepareSchema = true;
					SetupLogger.Log(Strings.SettingArgumentBecauseItIsRequired("PrepareSchema"));
				}
				if (base.SetupContext.IsOrgConfigUpdateRequired)
				{
					this.setPrepareOrganization = true;
					SetupLogger.Log(Strings.SettingArgumentBecauseItIsRequired("PrepareOrganization"));
					if (Datacenter.IsMicrosoftHostedOnly(true))
					{
						this.setPrepareSCT = true;
						SetupLogger.Log(Strings.SettingArgumentBecauseItIsRequired("PrepareSCT"));
					}
				}
				if (base.SetupContext.IsDomainConfigUpdateRequired)
				{
					this.setPrepareDomain = true;
					SetupLogger.Log(Strings.SettingArgumentBecauseItIsRequired("PrepareDomain"));
				}
			}
			if (this.OrganizationName != null)
			{
				this.setOrganizationName = this.OrganizationName.EscapedName;
				SetupLogger.Log(Strings.SettingArgumentToValue("OrganizationName", this.setOrganizationName));
			}
			if (base.SetupContext.ParsedArguments.ContainsKey("preparedomain"))
			{
				object obj = base.SetupContext.ParsedArguments["preparedomain"];
				this.setDomain = ((obj != null) ? obj.ToString() : null);
				SetupLogger.Log(Strings.SettingArgumentToValue("Domain", this.setDomain));
			}
			SetupLogger.TraceExit();
		}

		protected override void AddParameters()
		{
			SetupLogger.TraceEnter(new object[0]);
			base.AddParameters();
			this.DetermineParameters();
			if (this.setOrganizationName != null)
			{
				base.Parameters.AddWithValue("OrganizationName", this.setOrganizationName);
			}
			if (this.setPrepareSchema)
			{
				base.Parameters.AddWithValue("PrepareSchema", true);
			}
			if (this.setPrepareOrganization)
			{
				this.AddParametersForPrepareOrganization();
			}
			if (this.setPrepareAllDomains)
			{
				base.Parameters.AddWithValue("PrepareAllDomains", true);
			}
			if (this.setPrepareDomain)
			{
				base.Parameters.AddWithValue("PrepareDomain", true);
				if (this.setDomain != null)
				{
					base.Parameters.AddWithValue("Domain", this.setDomain);
				}
			}
			if (this.setPrepareSCT)
			{
				base.Parameters.AddWithValue("PrepareSCT", true);
			}
			SetupLogger.TraceExit();
		}

		private void AddParametersForPrepareOrganization()
		{
			base.Parameters.AddWithValue("PrepareOrganization", true);
			if (base.SetupContext.GlobalCustomerFeedbackEnabled != base.SetupContext.OriginalGlobalCustomerFeedbackEnabled)
			{
				base.Parameters.AddWithValue("CustomerFeedbackEnabled", base.SetupContext.GlobalCustomerFeedbackEnabled);
			}
			base.Parameters.AddWithValue("Industry", base.SetupContext.Industry);
			base.Parameters.AddWithValue("ActiveDirectorySplitPermissions", this.ActiveDirectorySplitPermissions);
		}

		public override bool WillDataHandlerDoAnyWork()
		{
			bool result = false;
			this.DetermineParameters();
			if (this.setPrepareSchema || this.setPrepareOrganization || this.setPrepareAllDomains || this.setPrepareDomain || this.setPrepareSCT)
			{
				result = true;
			}
			return result;
		}

		public override void UpdatePreCheckTaskDataHandler()
		{
			SetupLogger.TraceEnter(new object[0]);
			PrerequisiteAnalysisTaskDataHandler instance = PrerequisiteAnalysisTaskDataHandler.GetInstance(base.SetupContext, base.Connection);
			instance.AddRole("Global");
			this.DetermineParameters();
			if (this.setPrepareAllDomains)
			{
				instance.PrepareAllDomains = true;
			}
			if (this.setPrepareOrganization)
			{
				instance.PrepareOrganization = true;
				instance.ActiveDirectorySplitPermissions = this.ActiveDirectorySplitPermissions;
			}
			if (this.setPrepareSchema)
			{
				instance.PrepareSchema = true;
			}
			if (this.setPrepareDomain)
			{
				instance.PrepareDomain = true;
				if (this.setDomain != null)
				{
					instance.PrepareDomainTarget = this.setDomain;
				}
			}
			if (this.setPrepareSCT)
			{
				instance.PrepareSCT = true;
			}
			instance.CustomerFeedbackEnabled = base.SetupContext.GlobalCustomerFeedbackEnabled;
			SetupLogger.TraceExit();
		}

		public override ValidationError[] ValidateConfiguration()
		{
			List<ValidationError> list = new List<ValidationError>(base.ValidateConfiguration());
			if (!base.SetupContext.ExchangeOrganizationExists && (base.SetupContext.ParsedArguments.ContainsKey("preparead") || base.SetupContext.HasRolesToInstall))
			{
				if (this.OrganizationName == null)
				{
					list.Add(new SetupValidationError(Strings.SpecifyExchangeOrganizationName));
				}
				else
				{
					string escapedName = this.OrganizationName.EscapedName;
					try
					{
						new NewOrganizationName(escapedName);
					}
					catch (FormatException)
					{
						list.Add(new SetupValidationError(Strings.InvalidOrganizationName(escapedName)));
					}
				}
			}
			return list.ToArray();
		}

		private const string prepareSchemaArgument = "PrepareSchema";

		private const string prepareOrganizationArgument = "PrepareOrganization";

		private const string customerFeedbackEnabledArgument = "CustomerFeedbackEnabled";

		private const string industryArgument = "Industry";

		private const string prepareDomainArgument = "PrepareDomain";

		private const string prepareSCTArgument = "PrepareSCT";

		private const string prepareAllDomainsArgument = "PrepareAllDomains";

		private const string domainArgument = "Domain";

		private const string organizationNameArgument = "OrganizationName";

		private const string activeDirectorySplitPermissionsArgument = "ActiveDirectorySplitPermissions";

		private bool setPrepareSchema;

		private bool setPrepareOrganization;

		private bool setPrepareDomain;

		private bool setPrepareSCT;

		private bool setPrepareAllDomains;

		private string setDomain;

		private string setOrganizationName;
	}
}
