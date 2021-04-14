using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Management.FederationProvisioning;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Set", "AuthConfig", DefaultParameterSetName = "AuthConfigSettings", SupportsShouldProcess = true)]
	public sealed class SetAuthConfig : SetSingletonSystemConfigurationObjectTask<AuthConfig>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageSetAuthConfig;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "AuthConfigSettings")]
		public string ServiceName
		{
			get
			{
				return (string)base.Fields[AuthConfigSchema.ServiceName];
			}
			set
			{
				base.Fields[AuthConfigSchema.ServiceName] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "AuthConfigSettings")]
		public string Realm
		{
			get
			{
				return (string)base.Fields[AuthConfigSchema.Realm];
			}
			set
			{
				base.Fields[AuthConfigSchema.Realm] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "CurrentCertificateParameter")]
		public string CertificateThumbprint
		{
			get
			{
				return (string)base.Fields[AuthConfigSchema.CurrentCertificateThumbprint];
			}
			set
			{
				base.Fields[AuthConfigSchema.CurrentCertificateThumbprint] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "NewCertificateParameter")]
		public string NewCertificateThumbprint
		{
			get
			{
				return (string)base.Fields[AuthConfigSchema.NextCertificateThumbprint];
			}
			set
			{
				base.Fields[AuthConfigSchema.NextCertificateThumbprint] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "NewCertificateParameter")]
		public DateTime? NewCertificateEffectiveDate
		{
			get
			{
				return (DateTime?)base.Fields[AuthConfigSchema.NextCertificateEffectiveDate];
			}
			set
			{
				base.Fields[AuthConfigSchema.NextCertificateEffectiveDate] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "NewCertificateParameter")]
		[Parameter(Mandatory = false, ParameterSetName = "CurrentCertificateParameter")]
		public SwitchParameter SkipImmediateCertificateDeployment
		{
			get
			{
				return (SwitchParameter)(base.Fields["SkipImmediateCertificateDeployment"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["SkipImmediateCertificateDeployment"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "CurrentCertificateParameter")]
		[Parameter(Mandatory = false, ParameterSetName = "NewCertificateParameter")]
		public ServerIdParameter Server
		{
			get
			{
				return (ServerIdParameter)base.Fields["Server"];
			}
			set
			{
				base.Fields["Server"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "PublishAuthCertificateParameter")]
		[Parameter(Mandatory = false, ParameterSetName = "CurrentCertificateParameter")]
		[Parameter(Mandatory = false, ParameterSetName = "NewCertificateParameter")]
		public SwitchParameter Force
		{
			get
			{
				return (SwitchParameter)(base.Fields["Force"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["Force"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "PublishAuthCertificateParameter")]
		public SwitchParameter PublishCertificate
		{
			get
			{
				return (SwitchParameter)(base.Fields["PublishCertificate"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["PublishCertificate"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "PublishAuthCertificateParameter")]
		public SwitchParameter ClearPreviousCertificate
		{
			get
			{
				return (SwitchParameter)(base.Fields["ClearPreviousCertificate"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["ClearPreviousCertificate"] = value;
			}
		}

		protected override void InternalValidate()
		{
			base.InternalValidate();
			if (base.HasErrors)
			{
				return;
			}
			this.ValidateAndSetServerContext();
			string parameterSetName;
			if ((parameterSetName = base.ParameterSetName) != null)
			{
				if (parameterSetName == "NewCertificateParameter")
				{
					this.ValidateNewCertificateParameters();
					return;
				}
				if (parameterSetName == "AuthConfigSettings")
				{
					this.ValidateAuthConfigSettingsParameters();
					return;
				}
				if (parameterSetName == "CurrentCertificateParameter")
				{
					this.ValidateCurrentCertificateParameters();
					return;
				}
				if (parameterSetName == "PublishAuthCertificateParameter")
				{
					return;
				}
			}
			throw new NotSupportedException(base.ParameterSetName + "is not a supported parameter set");
		}

		private void ValidateAndSetServerContext()
		{
			if (this.Server == null)
			{
				return;
			}
			this.serverObject = (Server)base.GetDataObject<Server>(this.Server, base.DataSession, null, new LocalizedString?(Strings.ErrorServerNotFound((string)this.Server)), new LocalizedString?(Strings.ErrorServerNotUnique((string)this.Server)));
			if (!this.serverObject.IsE14OrLater)
			{
				base.WriteError(new ArgumentException(Strings.RemoteExchangeVersionNotSupported), ErrorCategory.InvalidArgument, null);
			}
			base.VerifyIsWithinScopes(DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromCustomScopeSet(base.ScopeSet, ADSystemConfigurationSession.GetRootOrgContainerId(base.DomainController, null), base.CurrentOrganizationId, base.ExecutingUserOrganizationId, true), 283, "ValidateAndSetServerContext", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\OAuth\\SetAuthConfig.cs"), this.serverObject, false, new DataAccessTask<AuthConfig>.ADObjectOutOfScopeString(Strings.ErrorServerOutOfScope));
		}

		protected override void InternalProcessRecord()
		{
			string parameterSetName;
			if ((parameterSetName = base.ParameterSetName) != null)
			{
				bool flag;
				if (!(parameterSetName == "NewCertificateParameter"))
				{
					if (!(parameterSetName == "AuthConfigSettings"))
					{
						if (!(parameterSetName == "CurrentCertificateParameter"))
						{
							if (!(parameterSetName == "PublishAuthCertificateParameter"))
							{
								goto IL_66;
							}
							flag = this.ProcessCertificatePublish();
						}
						else
						{
							flag = this.ProcessEmergencyCertificateUpdate();
						}
					}
					else
					{
						flag = this.ProcessAuthConfigSettings();
					}
				}
				else
				{
					flag = this.ProcessNormalCertificateUpdate();
				}
				if (flag)
				{
					base.InternalProcessRecord();
				}
				return;
			}
			IL_66:
			throw new NotSupportedException(base.ParameterSetName + "is not a supported parameter set");
		}

		private void ValidateAuthConfigSettingsParameters()
		{
			if (base.Fields.IsModified(AuthConfigSchema.ServiceName) && string.IsNullOrEmpty(this.ServiceName))
			{
				base.WriteError(new TaskException(Strings.ErrorAuthServiceNameNotBlank), ErrorCategory.InvalidArgument, null);
			}
		}

		private void ValidateNewCertificateParameters()
		{
			if (string.IsNullOrEmpty(this.NewCertificateThumbprint))
			{
				if (this.NewCertificateEffectiveDate != null && this.NewCertificateEffectiveDate != null)
				{
					if (base.Fields.IsModified(AuthConfigSchema.NextCertificateThumbprint) || string.IsNullOrEmpty(this.DataObject.NextCertificateThumbprint))
					{
						base.WriteError(new TaskException(Strings.ErrorAuthNewCertificateNeeded), ErrorCategory.InvalidArgument, null);
					}
					this.ValidateNewEffectiveDate();
					return;
				}
			}
			else
			{
				this.ValidateNewEffectiveDate();
				this.NewCertificateThumbprint = FederationCertificate.UnifyThumbprintFormat(this.NewCertificateThumbprint);
				this.ValidateCertificate(this.NewCertificateThumbprint, this.NewCertificateEffectiveDate);
				if (!string.IsNullOrEmpty(this.DataObject.CurrentCertificateThumbprint) && string.Compare(this.NewCertificateThumbprint, this.DataObject.CurrentCertificateThumbprint, StringComparison.OrdinalIgnoreCase) == 0)
				{
					base.WriteError(new TaskException(Strings.ErrorAuthSameAsCurrent), ErrorCategory.InvalidArgument, null);
				}
				if (!string.IsNullOrEmpty(this.DataObject.PreviousCertificateThumbprint) && string.Compare(this.NewCertificateThumbprint, this.DataObject.PreviousCertificateThumbprint, StringComparison.OrdinalIgnoreCase) == 0)
				{
					this.WriteWarning(Strings.WarningCertificateSameAsPrevious(this.NewCertificateThumbprint));
				}
			}
		}

		private void ValidateNewEffectiveDate()
		{
			if (this.NewCertificateEffectiveDate == null || this.NewCertificateEffectiveDate == null)
			{
				base.WriteError(new TaskException(Strings.ErrorAuthSetNewEffectDate), ErrorCategory.InvalidArgument, null);
			}
			if (((ExDateTime)this.NewCertificateEffectiveDate.Value).ToUtc() < ExDateTime.UtcNow.AddMinutes(-5.0))
			{
				base.WriteError(new TaskException(Strings.ErrorAuthInvalidNewEffectiveDate), ErrorCategory.InvalidArgument, null);
			}
		}

		private void ValidateCurrentCertificateParameters()
		{
			if (!string.IsNullOrEmpty(this.CertificateThumbprint))
			{
				this.CertificateThumbprint = FederationCertificate.UnifyThumbprintFormat(this.CertificateThumbprint);
				this.ValidateCertificate(this.CertificateThumbprint, null);
				if (!string.IsNullOrEmpty(this.DataObject.PreviousCertificateThumbprint) && string.Compare(this.CertificateThumbprint, this.DataObject.PreviousCertificateThumbprint, StringComparison.OrdinalIgnoreCase) == 0)
				{
					this.WriteWarning(Strings.WarningCertificateSameAsPrevious(this.CertificateThumbprint));
					return;
				}
			}
			else
			{
				base.WriteError(new TaskException(Strings.ErrorAuthCannotDeleteCurrent), ErrorCategory.InvalidArgument, null);
			}
		}

		private void ValidateCertificate(string thumbprint, DateTime? futurePublishDate)
		{
			if (this.DataObject.OrganizationId.OrganizationalUnit != null)
			{
				base.WriteError(new TaskException(Strings.ErrorSettingCertOnTenant), ErrorCategory.InvalidArgument, null);
			}
			bool skipAutomatedDeploymentChecks = OAuthTaskHelper.IsDatacenter() || this.Force;
			if (this.Server == null)
			{
				OAuthTaskHelper.ValidateLocalCertificate(thumbprint, futurePublishDate, skipAutomatedDeploymentChecks, new Task.TaskErrorLoggingDelegate(base.WriteError));
				return;
			}
			OAuthTaskHelper.ValidateRemoteCertificate((string)this.Server, thumbprint, futurePublishDate, skipAutomatedDeploymentChecks, new Task.TaskErrorLoggingDelegate(base.WriteError));
		}

		private bool ProcessAuthConfigSettings()
		{
			if (base.Fields.IsModified(AuthConfigSchema.ServiceName))
			{
				this.DataObject.ServiceName = this.ServiceName;
			}
			if (base.Fields.IsModified(AuthConfigSchema.Realm))
			{
				this.DataObject.Realm = this.Realm;
			}
			return true;
		}

		private bool ProcessEmergencyCertificateUpdate()
		{
			if (!string.IsNullOrEmpty(this.DataObject.CurrentCertificateThumbprint) && string.Compare(this.DataObject.CurrentCertificateThumbprint, this.CertificateThumbprint, StringComparison.OrdinalIgnoreCase) == 0)
			{
				this.DataObject.CurrentCertificateThumbprint = this.CertificateThumbprint;
				return true;
			}
			if (!string.IsNullOrEmpty(this.DataObject.NextCertificateThumbprint) && string.Compare(this.DataObject.NextCertificateThumbprint, this.CertificateThumbprint, StringComparison.OrdinalIgnoreCase) == 0)
			{
				base.WriteError(new TaskException(Strings.ErrorCertificateAlreadyinPublish(this.CertificateThumbprint)), ErrorCategory.InvalidArgument, null);
			}
			if (!OAuthTaskHelper.IsDatacenter() && !this.Force && !base.ShouldContinue(Strings.ConfirmationMessageAuthSettingOutage))
			{
				return false;
			}
			this.DataObject.PreviousCertificateThumbprint = this.DataObject.CurrentCertificateThumbprint;
			this.DataObject.CurrentCertificateThumbprint = this.CertificateThumbprint;
			this.TryPushCertificateInSameSite(this.CertificateThumbprint);
			return true;
		}

		private bool ProcessNormalCertificateUpdate()
		{
			if (base.Fields.IsModified(AuthConfigSchema.NextCertificateThumbprint) && string.IsNullOrEmpty(this.NewCertificateThumbprint))
			{
				if (string.IsNullOrEmpty(this.DataObject.NextCertificateThumbprint))
				{
					return false;
				}
				if (!this.Force && !base.ShouldContinue(Strings.ConfirmationMessageAuthNewInProgress))
				{
					return false;
				}
				this.DataObject.NextCertificateThumbprint = null;
				this.DataObject.NextCertificateEffectiveDate = null;
				return true;
			}
			else
			{
				if (((ExDateTime)this.NewCertificateEffectiveDate.Value).ToUtc() < ExDateTime.UtcNow.AddHours(48.0) && !this.Force && !base.ShouldContinue(Strings.ConfirmationMessageAuthNewDateTooShort(48)))
				{
					return false;
				}
				if (string.IsNullOrEmpty(this.NewCertificateThumbprint))
				{
					this.DataObject.NextCertificateEffectiveDate = new DateTime?(this.NewCertificateEffectiveDate.Value.ToUniversalTime());
					return true;
				}
				if (!string.IsNullOrEmpty(this.DataObject.NextCertificateThumbprint))
				{
					if (string.Compare(this.DataObject.NextCertificateThumbprint, this.NewCertificateThumbprint, StringComparison.OrdinalIgnoreCase) == 0)
					{
						this.DataObject.NextCertificateEffectiveDate = new DateTime?(this.NewCertificateEffectiveDate.Value.ToUniversalTime());
						return true;
					}
					if (!this.Force && !base.ShouldContinue(Strings.ConfirmationMessageAuthNewInProgressReplace(this.DataObject.NextCertificateThumbprint, this.NewCertificateThumbprint)))
					{
						return false;
					}
				}
				this.DataObject.NextCertificateThumbprint = this.NewCertificateThumbprint;
				this.DataObject.NextCertificateEffectiveDate = new DateTime?(this.NewCertificateEffectiveDate.Value.ToUniversalTime());
				this.TryPushCertificateInSameSite(this.NewCertificateThumbprint);
				return true;
			}
		}

		private bool ProcessCertificatePublish()
		{
			if (!this.PublishCertificate)
			{
				if (!this.ClearPreviousCertificate)
				{
					return false;
				}
				this.DataObject.PreviousCertificateThumbprint = string.Empty;
				return true;
			}
			else
			{
				if (string.IsNullOrEmpty(this.DataObject.NextCertificateThumbprint))
				{
					base.WriteError(new NoNextCertificateException(), ErrorCategory.InvalidArgument, null);
				}
				if (this.DataObject.NextCertificateEffectiveDate != null && this.DataObject.NextCertificateEffectiveDate != null && ((ExDateTime)this.DataObject.NextCertificateEffectiveDate.Value).ToUtc() > ExDateTime.UtcNow && !this.Force && !base.ShouldContinue(Strings.ConfirmationMessageAuthEffectiveDateNotReached(this.DataObject.NextCertificateThumbprint)))
				{
					return false;
				}
				this.DataObject.PreviousCertificateThumbprint = (this.ClearPreviousCertificate ? string.Empty : this.DataObject.CurrentCertificateThumbprint);
				this.DataObject.CurrentCertificateThumbprint = this.DataObject.NextCertificateThumbprint;
				this.DataObject.NextCertificateThumbprint = null;
				this.DataObject.NextCertificateEffectiveDate = null;
				return true;
			}
		}

		private void TryPushCertificateInSameSite(string thumbprint)
		{
			if (this.SkipImmediateCertificateDeployment)
			{
				return;
			}
			if (!OAuthTaskHelper.IsDatacenter())
			{
				try
				{
					if (this.serverObject != null)
					{
						FederationCertificate.PushCertificate(this.serverObject, new Task.TaskProgressLoggingDelegate(base.WriteProgress), new Task.TaskWarningLoggingDelegate(this.WriteWarning), thumbprint);
					}
					else
					{
						FederationCertificate.PushCertificate(new Task.TaskProgressLoggingDelegate(base.WriteProgress), new Task.TaskWarningLoggingDelegate(this.WriteWarning), thumbprint);
					}
				}
				catch (InvalidOperationException exception)
				{
					base.WriteError(exception, ErrorCategory.InvalidArgument, null);
				}
				catch (LocalizedException exception2)
				{
					base.WriteError(exception2, ErrorCategory.InvalidArgument, null);
				}
			}
		}

		private Server serverObject;
	}
}
