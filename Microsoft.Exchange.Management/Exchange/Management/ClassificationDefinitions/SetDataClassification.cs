using System;
using System.Globalization;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.ClassificationDefinitions
{
	[Cmdlet("Set", "DataClassification", SupportsShouldProcess = true)]
	public sealed class SetDataClassification : SetSystemConfigurationObjectTask<DataClassificationIdParameter, TransportRule>
	{
		[Parameter]
		public string Name
		{
			get
			{
				return (string)base.Fields["Name"];
			}
			set
			{
				base.Fields["Name"] = value;
			}
		}

		[Parameter]
		public string Description
		{
			get
			{
				return (string)base.Fields["Description"];
			}
			set
			{
				base.Fields["Description"] = value;
			}
		}

		[Parameter]
		[ValidateNotNull]
		public CultureInfo Locale
		{
			get
			{
				return (CultureInfo)base.Fields["Locale"];
			}
			set
			{
				base.Fields["Locale"] = value;
			}
		}

		[Parameter]
		public SwitchParameter IsDefault
		{
			get
			{
				return (SwitchParameter)(base.Fields["IsDefault"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["IsDefault"] = value;
			}
		}

		[Parameter]
		public MultiValuedProperty<Fingerprint> Fingerprints
		{
			get
			{
				return (MultiValuedProperty<Fingerprint>)base.Fields["Fingerprints"];
			}
			set
			{
				base.Fields["Fingerprints"] = value;
			}
		}

		public override object GetDynamicParameters()
		{
			return null;
		}

		protected override void InternalValidate()
		{
			if (base.OptionalIdentityData != null)
			{
				base.OptionalIdentityData.ConfigurationContainerRdn = ClassificationDefinitionConstants.ClassificationDefinitionsRdn;
			}
			if (base.UserSpecifiedParameters.Contains("Locale") && !base.UserSpecifiedParameters.Contains("Name") && !base.UserSpecifiedParameters.Contains("Description") && !base.UserSpecifiedParameters.Contains("IsDefault"))
			{
				base.WriteError(new ErrorMissingNameOrDescriptionOrIsDefaultParametersException(), ErrorCategory.InvalidOperation, null);
			}
			if (this.Locale == null)
			{
				this.Locale = CultureInfo.CurrentCulture;
			}
			this.ValidateParameterValueContraints("Name", this.Name);
			this.ValidateParameterValueContraints("Description", this.Description);
			base.InternalValidate();
		}

		protected override IConfigurable ResolveDataObject()
		{
			TaskLogger.LogEnter();
			this.implementation = new DataClassificationCmdletsImplementation(this);
			TransportRule transportRule = this.implementation.Initialize(base.DataSession, this.Identity, base.OptionalIdentityData);
			if (TaskHelper.ShouldUnderscopeDataSessionToOrganization((IDirectorySession)base.DataSession, transportRule))
			{
				base.UnderscopeDataSession(transportRule.OrganizationId);
				base.CurrentOrganizationId = transportRule.OrganizationId;
			}
			TaskLogger.LogExit();
			return transportRule;
		}

		protected override void InternalProcessRecord()
		{
			if (this.IsDefault || this.Locale.Equals(this.implementation.DataClassificationPresentationObject.DefaultCulture))
			{
				if (!base.UserSpecifiedParameters.Contains("Name") && this.implementation.DataClassificationPresentationObject.AllLocalizedNames.ContainsKey(this.Locale))
				{
					this.Name = this.implementation.DataClassificationPresentationObject.AllLocalizedNames[this.Locale];
				}
				if (!base.UserSpecifiedParameters.Contains("Description") && this.implementation.DataClassificationPresentationObject.AllLocalizedDescriptions.ContainsKey(this.Locale))
				{
					this.Description = this.implementation.DataClassificationPresentationObject.AllLocalizedDescriptions[this.Locale];
				}
				if (string.IsNullOrEmpty(this.Name) || string.IsNullOrEmpty(this.Description))
				{
					base.WriteError(new InvalidNameOrDescriptionForDefaultLocaleException(), ErrorCategory.InvalidOperation, null);
				}
				this.implementation.DataClassificationPresentationObject.SetDefaultResource(this.Locale, this.Name, this.Description);
			}
			else
			{
				if (base.Fields.IsModified("Name"))
				{
					this.implementation.DataClassificationPresentationObject.SetLocalizedName(this.Locale, this.Name);
				}
				if (base.Fields.IsModified("Description"))
				{
					this.implementation.DataClassificationPresentationObject.SetLocalizedDescription(this.Locale, this.Description);
				}
			}
			if (base.Fields.IsModified("Fingerprints"))
			{
				this.implementation.DataClassificationPresentationObject.Fingerprints = this.Fingerprints;
			}
			ValidationContext validationContext = new ValidationContext(ClassificationRuleCollectionOperationType.Update, base.CurrentOrganizationId, false, true, (IConfigurationSession)base.DataSession, this.DataObject, null, null);
			this.implementation.Save(validationContext);
			if (this.IsObjectStateChanged())
			{
				base.InternalProcessRecord();
				return;
			}
			bool flag = false;
			if (!base.TryGetVariableValue<bool>("ExchangeDisableNotChangedWarning", out flag) && !flag)
			{
				this.WriteWarning(Strings.WarningForceMessageWithId(this.Identity.ToString()));
			}
		}

		private void ValidateParameterValueContraints(string parameterName, string parameterValue)
		{
			int num = 256;
			if (!string.IsNullOrEmpty(parameterValue) && parameterValue.Length > num)
			{
				base.WriteError(new ErrorInvalidNameOrDescriptionParametersException(parameterName, parameterValue.Length, num), ErrorCategory.InvalidOperation, null);
			}
		}

		private DataClassificationCmdletsImplementation implementation;
	}
}
