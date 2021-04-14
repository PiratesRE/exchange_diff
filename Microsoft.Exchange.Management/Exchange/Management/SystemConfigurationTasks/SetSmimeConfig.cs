using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Set", "SmimeConfig", DefaultParameterSetName = "Identity", SupportsShouldProcess = true)]
	public sealed class SetSmimeConfig : SetMultitenancySingletonSystemConfigurationObjectTask<SmimeConfigurationContainer>
	{
		[Parameter(Mandatory = false)]
		public bool OWACheckCRLOnSend
		{
			get
			{
				return (bool)base.Fields["OWACheckCRLOnSend"];
			}
			set
			{
				base.Fields["OWACheckCRLOnSend"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public uint OWADLExpansionTimeout
		{
			get
			{
				return (uint)base.Fields["OWADLExpansionTimeout"];
			}
			set
			{
				base.Fields["OWADLExpansionTimeout"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool OWAUseSecondaryProxiesWhenFindingCertificates
		{
			get
			{
				return (bool)base.Fields["OWAUseSecondaryProxiesWhenFindingCertificates"];
			}
			set
			{
				base.Fields["OWAUseSecondaryProxiesWhenFindingCertificates"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public uint OWACRLConnectionTimeout
		{
			get
			{
				return (uint)base.Fields["OWACRLConnectionTimeout"];
			}
			set
			{
				base.Fields["OWACRLConnectionTimeout"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public uint OWACRLRetrievalTimeout
		{
			get
			{
				return (uint)base.Fields["OWACRLRetrievalTimeout"];
			}
			set
			{
				base.Fields["OWACRLRetrievalTimeout"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool OWADisableCRLCheck
		{
			get
			{
				return (bool)base.Fields["OWADisableCRLCheck"];
			}
			set
			{
				base.Fields["OWADisableCRLCheck"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool OWAAlwaysSign
		{
			get
			{
				return (bool)base.Fields["OWAAlwaysSign"];
			}
			set
			{
				base.Fields["OWAAlwaysSign"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool OWAAlwaysEncrypt
		{
			get
			{
				return (bool)base.Fields["OWAAlwaysEncrypt"];
			}
			set
			{
				base.Fields["OWAAlwaysEncrypt"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool OWAClearSign
		{
			get
			{
				return (bool)base.Fields["OWAClearSign"];
			}
			set
			{
				base.Fields["OWAClearSign"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool OWAIncludeCertificateChainWithoutRootCertificate
		{
			get
			{
				return (bool)base.Fields["OWAIncludeCertificateChainWithoutRootCertificate"];
			}
			set
			{
				base.Fields["OWAIncludeCertificateChainWithoutRootCertificate"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool OWAIncludeCertificateChainAndRootCertificate
		{
			get
			{
				return (bool)base.Fields["OWAIncludeCertificateChainAndRootCertificate"];
			}
			set
			{
				base.Fields["OWAIncludeCertificateChainAndRootCertificate"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool OWAEncryptTemporaryBuffers
		{
			get
			{
				return (bool)base.Fields["OWAEncryptTemporaryBuffers"];
			}
			set
			{
				base.Fields["OWAEncryptTemporaryBuffers"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool OWASignedEmailCertificateInclusion
		{
			get
			{
				return (bool)base.Fields["OWASignedEmailCertificateInclusion"];
			}
			set
			{
				base.Fields["OWASignedEmailCertificateInclusion"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public uint OWABCCEncryptedEmailForking
		{
			get
			{
				return (uint)base.Fields["OWABCCEncryptedEmailForking"];
			}
			set
			{
				base.Fields["OWABCCEncryptedEmailForking"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool OWAIncludeSMIMECapabilitiesInMessage
		{
			get
			{
				return (bool)base.Fields["OWAIncludeSMIMECapabilitiesInMessage"];
			}
			set
			{
				base.Fields["OWAIncludeSMIMECapabilitiesInMessage"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool OWACopyRecipientHeaders
		{
			get
			{
				return (bool)base.Fields["OWACopyRecipientHeaders"];
			}
			set
			{
				base.Fields["OWACopyRecipientHeaders"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool OWAOnlyUseSmartCard
		{
			get
			{
				return (bool)base.Fields["OWAOnlyUseSmartCard"];
			}
			set
			{
				base.Fields["OWAOnlyUseSmartCard"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool OWATripleWrapSignedEncryptedMail
		{
			get
			{
				return (bool)base.Fields["OWATripleWrapSignedEncryptedMail"];
			}
			set
			{
				base.Fields["OWATripleWrapSignedEncryptedMail"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool OWAUseKeyIdentifier
		{
			get
			{
				return (bool)base.Fields["OWAUseKeyIdentifier"];
			}
			set
			{
				base.Fields["OWAUseKeyIdentifier"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string OWAEncryptionAlgorithms
		{
			get
			{
				return (string)base.Fields["OWAEncryptionAlgorithms"];
			}
			set
			{
				base.Fields["OWAEncryptionAlgorithms"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string OWASigningAlgorithms
		{
			get
			{
				return (string)base.Fields["OWASigningAlgorithms"];
			}
			set
			{
				base.Fields["OWASigningAlgorithms"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool OWAForceSMIMEClientUpgrade
		{
			get
			{
				return (bool)base.Fields["OWAForceSMIMEClientUpgrade"];
			}
			set
			{
				base.Fields["OWAForceSMIMEClientUpgrade"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string OWASenderCertificateAttributesToDisplay
		{
			get
			{
				return (string)base.Fields["OWASenderCertificateAttributesToDisplay"];
			}
			set
			{
				base.Fields["OWASenderCertificateAttributesToDisplay"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool OWAAllowUserChoiceOfSigningCertificate
		{
			get
			{
				return (bool)base.Fields["OWAAllowUserChoiceOfSigningCertificate"];
			}
			set
			{
				base.Fields["OWAAllowUserChoiceOfSigningCertificate"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public byte[] SMIMECertificateIssuingCA
		{
			get
			{
				return (byte[])base.Fields["SMIMECertificateIssuingCA"];
			}
			set
			{
				base.Fields["SMIMECertificateIssuingCA"] = value;
			}
		}

		protected override ObjectId RootId
		{
			get
			{
				return SmimeConfigurationContainer.GetWellKnownParentLocation(base.CurrentOrgContainerId);
			}
		}

		protected override IConfigurable PrepareDataObject()
		{
			TaskLogger.LogEnter();
			IConfigurable configurable = base.PrepareDataObject();
			SmimeConfigurationContainer smimeConfigurationContainer = configurable as SmimeConfigurationContainer;
			if (base.Fields.Contains("SMIMECertificateIssuingCA"))
			{
				byte[] array = base.Fields["SMIMECertificateIssuingCA"] as byte[];
				if (array != null && array.Length != 0)
				{
					if (array.Length > 70000)
					{
						base.WriteError(new InvalidOperationException(string.Format(Strings.SSTFileSizeExceedLimit, 70000.ToString())), ErrorCategory.LimitsExceeded, (configurable != null) ? configurable.Identity : null);
						return smimeConfigurationContainer;
					}
					smimeConfigurationContainer.SMIMECertificateIssuingCA = array;
					X509Certificate2Collection x509Certificate2Collection = new X509Certificate2Collection();
					try
					{
						x509Certificate2Collection.Import(array);
					}
					catch (Exception e)
					{
						base.WriteError(new FormatException("SMIMECertificateIssuingCA has wrong format"), ErrorCategory.InvalidData, null);
						TaskLogger.LogError(e);
					}
					List<string> list = new List<string>();
					DateTime dateTime = DateTime.MaxValue;
					string text = string.Empty;
					for (int i = 0; i < x509Certificate2Collection.Count; i++)
					{
						DateTime dateTime2;
						if (DateTime.TryParse(x509Certificate2Collection[i].GetExpirationDateString(), out dateTime2))
						{
							if (dateTime2 < DateTime.UtcNow)
							{
								list.Add(x509Certificate2Collection[i].Thumbprint);
							}
							if (string.IsNullOrWhiteSpace(text) || dateTime2 < dateTime)
							{
								dateTime = dateTime2;
								text = x509Certificate2Collection[i].Thumbprint;
							}
						}
						else
						{
							base.WriteError(new FormatException("Certificate Expiry date has wrong format"), ErrorCategory.InvalidData, null);
						}
					}
					if (list.Count > 0)
					{
						StringBuilder stringBuilder = new StringBuilder();
						foreach (string value in list)
						{
							if (stringBuilder.Length != 0)
							{
								stringBuilder.Append(" , ");
							}
							stringBuilder.Append(value);
						}
						base.WriteError(new InvalidOperationException(string.Format(Strings.ExpiryCertMessage, stringBuilder.ToString())), ErrorCategory.LimitsExceeded, null);
						return smimeConfigurationContainer;
					}
					if (dateTime != DateTime.MaxValue)
					{
						smimeConfigurationContainer.SMIMECertificatesExpiryDate = new DateTime?(dateTime);
						smimeConfigurationContainer.SMIMEExpiredCertificateThumbprint = text;
					}
				}
				else
				{
					smimeConfigurationContainer.SMIMECertificateIssuingCA = null;
					smimeConfigurationContainer.SMIMECertificatesExpiryDate = null;
					smimeConfigurationContainer.SMIMEExpiredCertificateThumbprint = string.Empty;
				}
			}
			this.ProcessSmimeRecord(smimeConfigurationContainer);
			TaskLogger.LogExit();
			return smimeConfigurationContainer;
		}

		protected override IConfigurable ResolveDataObject()
		{
			TaskLogger.LogEnter();
			IConfigurable[] array = null;
			base.WriteVerbose(TaskVerboseStringHelper.GetFindDataObjectsVerboseString(base.DataSession, typeof(SmimeConfigurationContainer), this.InternalFilter, this.RootId, this.DeepSearch));
			try
			{
				array = base.DataSession.Find<SmimeConfigurationContainer>(this.InternalFilter, this.RootId, this.DeepSearch, null);
			}
			catch (DataSourceTransientException exception)
			{
				base.WriteError(exception, (ErrorCategory)1002, null);
			}
			finally
			{
				base.WriteVerbose(TaskVerboseStringHelper.GetSourceVerboseString(base.DataSession));
			}
			if (array == null)
			{
				array = new IConfigurable[0];
			}
			IConfigurable result = null;
			switch (array.Length)
			{
			case 0:
			{
				SmimeConfigurationContainer smimeConfigurationContainer = new SmimeConfigurationContainer();
				smimeConfigurationContainer.SetId(this.RootId as ADObjectId);
				smimeConfigurationContainer.Initialize();
				smimeConfigurationContainer.OrganizationId = base.CurrentOrganizationId;
				result = smimeConfigurationContainer;
				break;
			}
			case 1:
				result = array[0];
				break;
			default:
				TaskLogger.Log(Strings.SmimeConfigAmbiguous);
				break;
			}
			TaskLogger.LogExit();
			return result;
		}

		private void ProcessSmimeRecord(SmimeConfigurationContainer dataObject)
		{
			if (dataObject == null)
			{
				return;
			}
			if (base.Fields.Contains("OWACheckCRLOnSend"))
			{
				dataObject.OWACheckCRLOnSend = this.OWACheckCRLOnSend;
			}
			if (base.Fields.Contains("OWADLExpansionTimeout"))
			{
				dataObject.OWADLExpansionTimeout = this.OWADLExpansionTimeout;
			}
			if (base.Fields.Contains("OWAUseSecondaryProxiesWhenFindingCertificates"))
			{
				dataObject.OWAUseSecondaryProxiesWhenFindingCertificates = this.OWAUseSecondaryProxiesWhenFindingCertificates;
			}
			if (base.Fields.Contains("OWACRLConnectionTimeout"))
			{
				dataObject.OWACRLConnectionTimeout = this.OWACRLConnectionTimeout;
			}
			if (base.Fields.Contains("OWACRLRetrievalTimeout"))
			{
				dataObject.OWACRLRetrievalTimeout = this.OWACRLRetrievalTimeout;
			}
			if (base.Fields.Contains("OWADisableCRLCheck"))
			{
				dataObject.OWADisableCRLCheck = this.OWADisableCRLCheck;
			}
			if (base.Fields.Contains("OWAAlwaysEncrypt"))
			{
				dataObject.OWAAlwaysEncrypt = this.OWAAlwaysEncrypt;
			}
			if (base.Fields.Contains("OWAAlwaysSign"))
			{
				dataObject.OWAAlwaysSign = this.OWAAlwaysSign;
			}
			if (base.Fields.Contains("OWAClearSign"))
			{
				dataObject.OWAClearSign = this.OWAClearSign;
			}
			if (base.Fields.Contains("OWAIncludeCertificateChainWithoutRootCertificate"))
			{
				dataObject.OWAIncludeCertificateChainWithoutRootCertificate = this.OWAIncludeCertificateChainWithoutRootCertificate;
			}
			if (base.Fields.Contains("OWAIncludeCertificateChainAndRootCertificate"))
			{
				dataObject.OWAIncludeCertificateChainAndRootCertificate = this.OWAIncludeCertificateChainAndRootCertificate;
			}
			if (base.Fields.Contains("OWAEncryptTemporaryBuffers"))
			{
				dataObject.OWAEncryptTemporaryBuffers = this.OWAEncryptTemporaryBuffers;
			}
			if (base.Fields.Contains("OWASignedEmailCertificateInclusion"))
			{
				dataObject.OWASignedEmailCertificateInclusion = this.OWASignedEmailCertificateInclusion;
			}
			if (base.Fields.Contains("OWABCCEncryptedEmailForking"))
			{
				dataObject.OWABCCEncryptedEmailForking = this.OWABCCEncryptedEmailForking;
			}
			if (base.Fields.Contains("OWAIncludeSMIMECapabilitiesInMessage"))
			{
				dataObject.OWAIncludeSMIMECapabilitiesInMessage = this.OWAIncludeSMIMECapabilitiesInMessage;
			}
			if (base.Fields.Contains("OWACopyRecipientHeaders"))
			{
				dataObject.OWACopyRecipientHeaders = this.OWACopyRecipientHeaders;
			}
			if (base.Fields.Contains("OWAOnlyUseSmartCard"))
			{
				dataObject.OWAOnlyUseSmartCard = this.OWAOnlyUseSmartCard;
			}
			if (base.Fields.Contains("OWATripleWrapSignedEncryptedMail"))
			{
				dataObject.OWATripleWrapSignedEncryptedMail = this.OWATripleWrapSignedEncryptedMail;
			}
			if (base.Fields.Contains("OWAUseKeyIdentifier"))
			{
				dataObject.OWAUseKeyIdentifier = this.OWAUseKeyIdentifier;
			}
			if (base.Fields.Contains("OWAEncryptionAlgorithms"))
			{
				dataObject.OWAEncryptionAlgorithms = this.OWAEncryptionAlgorithms;
			}
			if (base.Fields.Contains("OWASigningAlgorithms"))
			{
				dataObject.OWASigningAlgorithms = this.OWASigningAlgorithms;
			}
			if (base.Fields.Contains("OWAForceSMIMEClientUpgrade"))
			{
				dataObject.OWAForceSMIMEClientUpgrade = this.OWAForceSMIMEClientUpgrade;
			}
			if (base.Fields.Contains("OWASenderCertificateAttributesToDisplay"))
			{
				dataObject.OWASenderCertificateAttributesToDisplay = this.OWASenderCertificateAttributesToDisplay;
			}
			if (base.Fields.Contains("OWAAllowUserChoiceOfSigningCertificate"))
			{
				dataObject.OWAAllowUserChoiceOfSigningCertificate = this.OWAAllowUserChoiceOfSigningCertificate;
			}
			dataObject.SaveSettings();
		}

		private const int SSTFileSizeLimit = 70000;
	}
}
