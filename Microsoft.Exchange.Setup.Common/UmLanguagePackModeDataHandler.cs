using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Configuration.MonadDataProvider;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.Deployment;

namespace Microsoft.Exchange.Setup.Common
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class UmLanguagePackModeDataHandler : ModeDataHandler
	{
		public UmLanguagePackModeDataHandler(ISetupContext setupContext, MonadConnection connection) : base(setupContext, connection)
		{
			this.SelectedCultures = new List<CultureInfo>();
			if (base.SetupContext.SelectedCultures != null)
			{
				this.SelectedCultures.AddRange(setupContext.SelectedCultures);
			}
			InstallableUnitConfigurationInfoManager.InitializeUmLanguagePacksConfigurationInfo(this.SelectedCultures.ToArray());
		}

		public List<CultureInfo> SelectedCultures
		{
			get
			{
				return this.selectedCultures;
			}
			set
			{
				this.selectedCultures = value;
			}
		}

		public List<CultureInfo> InstalledUMLanguagePacks
		{
			get
			{
				return base.SetupContext.InstalledUMLanguagePacks;
			}
		}

		public override List<string> SelectedInstallableUnits
		{
			get
			{
				base.SelectedInstallableUnits.Clear();
				foreach (CultureInfo umlang in this.SelectedCultures)
				{
					base.SelectedInstallableUnits.Add(UmLanguagePackConfigurationInfo.GetUmLanguagePackNameForCultureInfo(umlang));
				}
				return base.SelectedInstallableUnits;
			}
		}

		protected override bool NeedPrePostSetupDataHandlers
		{
			get
			{
				return false;
			}
		}

		protected override bool NeedFileDataHandler()
		{
			return false;
		}

		public override ValidationError[] Validate()
		{
			SetupLogger.TraceEnter(new object[0]);
			List<ValidationError> list = new List<ValidationError>();
			if (!RoleManager.GetRoleByName("UnifiedMessagingRole").IsInstalled)
			{
				list.Add(new SetupValidationError(Strings.UnifiedMessagingRoleIsRequiredForLanguagePackInstalls));
			}
			if (!base.IgnoreValidatingRoleChanges && (this.SelectedCultures == null || this.SelectedCultures.Count == 0))
			{
				list.Add(new SetupValidationError(Strings.NoUmLanguagePackSpecified));
			}
			list.AddRange(base.Validate());
			SetupLogger.TraceExit();
			return list.ToArray();
		}

		public override bool CanChangeSourceDir
		{
			get
			{
				return !base.SetupContext.IsCleanMachine;
			}
		}

		public override string RoleSelectionDescription
		{
			get
			{
				return Strings.AddServerRoleText;
			}
		}

		public override LocalizedString ConfigurationSummary
		{
			get
			{
				return Strings.AddRolesToInstall;
			}
		}

		public override string CompletionDescription
		{
			get
			{
				return base.WorkUnits.HasFailures ? Strings.AddFailText : Strings.AddSuccessText;
			}
		}

		public override string CompletionStatus
		{
			get
			{
				return base.WorkUnits.HasFailures ? Strings.AddFailStatus : Strings.AddSuccessStatus;
			}
		}

		protected override void UpdateDataHandlers()
		{
			SetupLogger.TraceEnter(new object[0]);
			base.DataHandlers.Clear();
			this.AddConfigurationDataHandlers();
			SetupLogger.TraceExit();
		}

		private List<CultureInfo> selectedCultures;
	}
}
