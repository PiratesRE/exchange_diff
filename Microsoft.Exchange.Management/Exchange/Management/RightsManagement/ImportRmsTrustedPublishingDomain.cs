using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management.Automation;
using System.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage.OfflineRms;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.Common;
using Microsoft.Exchange.Management.EventMessages;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Security.RightsManagement;
using Microsoft.RightsManagementServices.Core;
using Microsoft.RightsManagementServices.Online.Partner;
using Microsoft.RightsManagementServices.Provider;

namespace Microsoft.Exchange.Management.RightsManagement
{
	[Cmdlet("Import", "RMSTrustedPublishingDomain", SupportsShouldProcess = true, DefaultParameterSetName = "IntranetLicensingUrl")]
	public sealed class ImportRmsTrustedPublishingDomain : NewMultitenancySystemConfigurationObjectTask<RMSTrustedPublishingDomain>
	{
		[Parameter(Mandatory = true, ParameterSetName = "RefreshTemplates")]
		[Parameter(Mandatory = true, ParameterSetName = "ImportFromFile")]
		[Parameter(Mandatory = true, ParameterSetName = "IntranetLicensingUrl")]
		public SecureString Password
		{
			get
			{
				return (SecureString)base.Fields["Password"];
			}
			set
			{
				base.Fields["Password"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "ImportFromFile")]
		[Parameter(Mandatory = true, ParameterSetName = "IntranetLicensingUrl")]
		[Parameter(Mandatory = true, ParameterSetName = "RefreshTemplates")]
		public byte[] FileData
		{
			get
			{
				return (byte[])base.Fields["FileData"];
			}
			set
			{
				base.Fields["FileData"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "IntranetLicensingUrl")]
		[Parameter(Mandatory = true, ParameterSetName = "ImportFromFile")]
		public Uri IntranetLicensingUrl
		{
			get
			{
				return (Uri)base.Fields["IntranetLicensingUrl"];
			}
			set
			{
				base.Fields["IntranetLicensingUrl"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "IntranetLicensingUrl")]
		[Parameter(Mandatory = true, ParameterSetName = "ImportFromFile")]
		public Uri ExtranetLicensingUrl
		{
			get
			{
				return (Uri)base.Fields["ExtranetLicensingUrl"];
			}
			set
			{
				base.Fields["ExtranetLicensingUrl"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "ImportFromFile")]
		public Uri IntranetCertificationUrl
		{
			get
			{
				return (Uri)base.Fields["IntranetCertificationUrl"];
			}
			set
			{
				base.Fields["IntranetCertificationUrl"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "ImportFromFile")]
		public Uri ExtranetCertificationUrl
		{
			get
			{
				return (Uri)base.Fields["ExtranetCertificationUrl"];
			}
			set
			{
				base.Fields["ExtranetCertificationUrl"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter Default
		{
			get
			{
				return (SwitchParameter)(base.Fields["Default"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["Default"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "RMSOnline")]
		[Parameter(Mandatory = false, ParameterSetName = "RefreshTemplates")]
		public SwitchParameter RefreshTemplates
		{
			get
			{
				return (SwitchParameter)(base.Fields["RefreshTemplates"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["RefreshTemplates"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "RMSOnline")]
		public SwitchParameter RMSOnline
		{
			get
			{
				return (SwitchParameter)(base.Fields["RMSOnline"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["RMSOnline"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "RMSOnline")]
		public Guid RMSOnlineOrgOverride
		{
			get
			{
				return (Guid)(base.Fields["RMSOnlineOrgOverride"] ?? Guid.Empty);
			}
			set
			{
				base.Fields["RMSOnlineOrgOverride"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "RMSOnline")]
		public string RMSOnlineAuthCertSubjectNameOverride
		{
			get
			{
				return (string)base.Fields["RMSOnlineAuthCertSubjectNameOverride"];
			}
			set
			{
				base.Fields["RMSOnlineAuthCertSubjectNameOverride"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "RMSOnline2")]
		public byte[] RMSOnlineConfig
		{
			get
			{
				return (byte[])base.Fields["RMSOnlineConfig"];
			}
			set
			{
				base.Fields["RMSOnlineConfig"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "RMSOnline2")]
		public Hashtable RMSOnlineKeys
		{
			get
			{
				return (Hashtable)base.Fields["RMSOnlineKeys"];
			}
			set
			{
				base.Fields["RMSOnlineKeys"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "RMSOnline2")]
		public Hashtable RMSOnlineAuthorTest
		{
			get
			{
				Hashtable hashtable = new Hashtable
				{
					{
						"TrustedRootHierarchy",
						0
					},
					{
						"KeyProtectionCertificate",
						null
					},
					{
						"KeyProtectionCertificatePassword",
						null
					}
				};
				foreach (object obj in ((Hashtable)(base.Fields["RMSOnlineAuthorTest"] ?? new Hashtable())))
				{
					DictionaryEntry dictionaryEntry = (DictionaryEntry)obj;
					hashtable[dictionaryEntry.Key] = dictionaryEntry.Value;
				}
				return hashtable;
			}
			set
			{
				base.Fields["RMSOnlineAuthorTest"] = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageImportRMSTPD(base.Name);
			}
		}

		protected override bool IsKnownException(Exception exception)
		{
			return base.IsKnownException(exception) || RmsUtil.IsKnownException(exception) || typeof(InvalidRpmsgFormatException).IsInstanceOfType(exception);
		}

		private void ThrowIfExistingTpdNotFound(RMSTrustedPublishingDomain[] existingTpd)
		{
			if (existingTpd == null || existingTpd.Length == 0)
			{
				base.WriteError(new FailedToFindTPDForRefreshException(base.Name), ExchangeErrorCategory.Client, base.Name);
			}
		}

		private void ThrowIfRmsOnlinePreRequisitesNotMet(IRMConfiguration irmConfiguration)
		{
			if (!RmsUtil.AreRmsOnlinePreRequisitesMet(irmConfiguration))
			{
				base.WriteError(new RmsOnlineUrlsNotPresentException(), ExchangeErrorCategory.Client, this.RMSOnline);
			}
			if (!string.IsNullOrEmpty(irmConfiguration.RMSOnlineVersion))
			{
				base.WriteError(new OldRmsOnlineImportAfterRmsOnlineForwardSync(), ExchangeErrorCategory.Client, this.RMSOnline);
			}
		}

		private void ThrowIfOrganizationNotSpecified()
		{
			if (OrganizationId.ForestWideOrgId == base.CurrentOrganizationId)
			{
				base.WriteError(new ArgumentException(Strings.ErrorOrganizationParameterRequired), ErrorCategory.InvalidOperation, null);
			}
		}

		private void UpdateTpdNameForRmsOnline()
		{
			RMSTrustedPublishingDomain[] array = ((IConfigurationSession)base.DataSession).Find<RMSTrustedPublishingDomain>(null, QueryScope.SubTree, null, null, 0);
			string existingDefaultTpdName = null;
			foreach (RMSTrustedPublishingDomain rmstrustedPublishingDomain in array)
			{
				if (rmstrustedPublishingDomain.Default)
				{
					existingDefaultTpdName = rmstrustedPublishingDomain.Name;
					break;
				}
			}
			base.Name = RmsUtil.GenerateRmsOnlineTpdName(existingDefaultTpdName, base.Name);
		}

		internal static void ChangeDefaultTPDAndUpdateIrmConfigData(IConfigurationSession session, IRMConfiguration irmConfiguration, RMSTrustedPublishingDomain trustedPublishingDomain, out RMSTrustedPublishingDomain oldDefaultTPD)
		{
			ADPagedReader<RMSTrustedPublishingDomain> source = session.FindPaged<RMSTrustedPublishingDomain>(trustedPublishingDomain.Id.Parent, QueryScope.OneLevel, null, null, 0);
			oldDefaultTPD = source.FirstOrDefault((RMSTrustedPublishingDomain tpd) => tpd.Default);
			ImportRmsTrustedPublishingDomain.ChangeDefaultTPDAndUpdateIrmConfigData(irmConfiguration, trustedPublishingDomain, oldDefaultTPD);
		}

		internal static void ChangeDefaultTPDAndUpdateIrmConfigData(IRMConfiguration irmConfiguration, RMSTrustedPublishingDomain trustedPublishingDomain, RMSTrustedPublishingDomain oldDefaultTPD)
		{
			if (oldDefaultTPD != null)
			{
				oldDefaultTPD.Default = false;
			}
			irmConfiguration.ServiceLocation = trustedPublishingDomain.ExtranetCertificationUrl;
			irmConfiguration.PublishingLocation = new Uri(RMUtil.ConvertUriToPublishUrl(trustedPublishingDomain.ExtranetCertificationUrl), UriKind.Absolute);
			if (!string.IsNullOrEmpty(trustedPublishingDomain.PrivateKey))
			{
				SharedServerBoxRacIdentityGenerator sharedServerBoxRacIdentityGenerator = new SharedServerBoxRacIdentityGenerator(trustedPublishingDomain.SLCCertChain, oldDefaultTPD, irmConfiguration.SharedServerBoxRacIdentity);
				irmConfiguration.SharedServerBoxRacIdentity = sharedServerBoxRacIdentityGenerator.GenerateSharedKey();
				return;
			}
			irmConfiguration.SharedServerBoxRacIdentity = null;
		}

		private MultiValuedProperty<string> CompressAndUpdateTemplates(RMSTrustedPublishingDomain dataObject, string[] templatesFromTpd, bool refreshTemplates, RmsTemplateType templateType, out ImportRmsTrustedPublishingDomainResult rmsTpdResult)
		{
			MultiValuedProperty<string> multiValuedProperty = new MultiValuedProperty<string>();
			rmsTpdResult = new ImportRmsTrustedPublishingDomainResult(dataObject);
			if (refreshTemplates)
			{
				Dictionary<Guid, RmsTemplate> existingTemplateEntries = ImportRmsTrustedPublishingDomain.GetExistingTemplateEntries(dataObject.RMSTemplates);
				if (templatesFromTpd != null)
				{
					for (int i = 0; i < templatesFromTpd.Length; i++)
					{
						RmsTemplate rmsTemplate = RmsTemplate.CreateServerTemplateFromTemplateDefinition(templatesFromTpd[i], templateType);
						if (existingTemplateEntries.ContainsKey(rmsTemplate.Id))
						{
							templateType = existingTemplateEntries[rmsTemplate.Id].Type;
							existingTemplateEntries.Remove(rmsTemplate.Id);
							rmsTpdResult.UpdatedTemplates.Add(rmsTemplate.Name);
						}
						else
						{
							rmsTpdResult.AddedTemplates.Add(rmsTemplate.Name);
						}
						multiValuedProperty.Add(RMUtil.CompressTemplate(templatesFromTpd[i], templateType));
					}
				}
				foreach (KeyValuePair<Guid, RmsTemplate> keyValuePair in existingTemplateEntries)
				{
					rmsTpdResult.RemovedTemplates.Add(keyValuePair.Value.Name);
				}
				if (dataObject.Default && rmsTpdResult.RemovedTemplates.Count > 0)
				{
					this.WriteWarning(Strings.WarningDeleteTemplate);
				}
			}
			else if (templatesFromTpd != null)
			{
				for (int j = 0; j < templatesFromTpd.Length; j++)
				{
					RmsTemplate rmsTemplate2 = RmsTemplate.CreateServerTemplateFromTemplateDefinition(templatesFromTpd[j], templateType);
					rmsTpdResult.AddedTemplates.Add(rmsTemplate2.Name);
					multiValuedProperty.Add(RMUtil.CompressTemplate(templatesFromTpd[j], templateType));
				}
			}
			if (rmsTpdResult.AddedTemplates.Count > 0 && dataObject.Default && templateType == RmsTemplateType.Archived)
			{
				this.WriteWarning(Strings.WarningMarkNewTemplatesAsDistributedForCreatingProtectionRules(dataObject.Name));
			}
			if (multiValuedProperty.Count != 0)
			{
				return multiValuedProperty;
			}
			return null;
		}

		private static Dictionary<Guid, RmsTemplate> GetExistingTemplateEntries(MultiValuedProperty<string> existingCompressedTemplates)
		{
			if (MultiValuedPropertyBase.IsNullOrEmpty(existingCompressedTemplates))
			{
				return new Dictionary<Guid, RmsTemplate>(0);
			}
			Dictionary<Guid, RmsTemplate> dictionary = new Dictionary<Guid, RmsTemplate>(existingCompressedTemplates.Count);
			foreach (string encodedTemplate in existingCompressedTemplates)
			{
				RmsTemplateType type;
				string templateXrml = RMUtil.DecompressTemplate(encodedTemplate, out type);
				RmsTemplate rmsTemplate = RmsTemplate.CreateServerTemplateFromTemplateDefinition(templateXrml, type);
				dictionary[rmsTemplate.Id] = rmsTemplate;
			}
			return dictionary;
		}

		private IRMConfiguration ReadIrmConfiguration()
		{
			return IRMConfiguration.Read((IConfigurationSession)base.DataSession);
		}

		private XrmlCertificateChain ReenrollSlcCertificateChain(XrmlCertificateChain slcCertificateChain, object errorTarget)
		{
			XrmlCertificateChain result;
			try
			{
				result = Enroller.Reenroll(slcCertificateChain);
			}
			catch (ArgumentException e)
			{
				ImportRmsTrustedPublishingDomain.WriteFailureToEventLog(e);
				base.WriteError(new FailedToReEnrollTPDException(e), ExchangeErrorCategory.Client, errorTarget);
				result = null;
			}
			return result;
		}

		private TrustedDocDomain ParseTpdFileData(SecureString password, byte[] fileData)
		{
			TrustedDocDomain result;
			try
			{
				result = TrustedPublishingDomainParser.Parse(password, fileData);
			}
			catch (TrustedPublishingDomainParser.ParseFailedException ex)
			{
				ImportRmsTrustedPublishingDomain.WriteFailureToEventLog(ex);
				base.WriteError(new FailedToDeSerializeImportedTrustedPublishingDomainException(ex), ExchangeErrorCategory.Client, base.Name);
				result = null;
			}
			return result;
		}

		private TrustedDocDomain GetTpdFromRmsOnline(Uri rmsOnlineKeySharingLocation)
		{
			RmsUtil.ThrowIfParameterNull(rmsOnlineKeySharingLocation, "rmsOnlineKeySharingLocation");
			this.ThrowIfOrganizationNotSpecified();
			RmsOnlineTpdImporter rmsOnlineTpdImporter = new RmsOnlineTpdImporter(rmsOnlineKeySharingLocation, this.RMSOnlineAuthCertSubjectNameOverride ?? RmsOnlineConstants.AuthenticationCertificateSubjectDistinguishedName);
			TrustedDocDomain result;
			try
			{
				Guid guid = this.RMSOnlineOrgOverride;
				if (Guid.Empty == guid)
				{
					guid = RmsUtil.GetExternalDirectoryOrgIdThrowOnFailure(this.ConfigurationSession, base.CurrentOrganizationId);
				}
				TrustedDocDomain trustedDocDomain = rmsOnlineTpdImporter.Import(guid);
				if (!this.RefreshTemplates)
				{
					this.IntranetLicensingUrl = rmsOnlineTpdImporter.IntranetLicensingUrl;
					this.ExtranetLicensingUrl = rmsOnlineTpdImporter.ExtranetLicensingUrl;
					this.IntranetCertificationUrl = rmsOnlineTpdImporter.IntranetCertificationUrl;
					this.ExtranetCertificationUrl = rmsOnlineTpdImporter.ExtranetCertificationUrl;
				}
				result = trustedDocDomain;
			}
			catch (ImportTpdException ex)
			{
				ImportRmsTrustedPublishingDomain.WriteFailureToEventLog(ex);
				base.WriteError(new FailedToGetTrustedPublishingDomainFromRmsOnlineException(ex, ex.InnerException), ExchangeErrorCategory.Client, base.Name);
				result = null;
			}
			return result;
		}

		private static void WriteFailureToEventLog(Exception e)
		{
			ExManagementApplicationLogger.LogEvent(ManagementEventLogConstants.Tuple_ImportTpdFailure, new string[]
			{
				e.ToString()
			});
		}

		public ImportRmsTrustedPublishingDomain()
		{
			this.impl = new Lazy<ImportRmsTrustedPublishingDomain.Impl>(() => ImportRmsTrustedPublishingDomain.CreateImpl(this));
		}

		protected override IConfigurable PrepareDataObject()
		{
			return this.impl.Value.PrepareDataObject(() => this.<>n__FabricatedMethod6());
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			this.impl.Value.InternalValidate(delegate
			{
				this.<>n__FabricatedMethod8();
			});
			TaskLogger.LogExit();
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			this.impl.Value.InternalProcessRecord(delegate
			{
				this.<>n__FabricatedMethoda();
			});
			TaskLogger.LogExit();
		}

		protected override void WriteResult(IConfigurable dataObject)
		{
			TaskLogger.LogEnter();
			this.impl.Value.WriteResut(dataObject, delegate(IConfigurable da)
			{
				this.<>n__FabricatedMethodc(da);
			});
			TaskLogger.LogExit();
		}

		private static ImportRmsTrustedPublishingDomain.Impl CreateImpl(ImportRmsTrustedPublishingDomain cmdlet)
		{
			if (cmdlet.RMSOnlineConfig == null)
			{
				return new ImportRmsTrustedPublishingDomain.OnPremImpl
				{
					cmdlet = cmdlet
				};
			}
			ExAssert.RetailAssert(!cmdlet.RefreshTemplates, "When importing from RMS Online, RefreshTemplates should always be false");
			return new ImportRmsTrustedPublishingDomain.RMSOnlineImpl
			{
				cmdlet = cmdlet
			};
		}

		private const string ImportFromFileParameterSetName = "ImportFromFile";

		private const string IntranetLicensingUrlParameterSetName = "IntranetLicensingUrl";

		private const string RefreshTemplatesParameterSetName = "RefreshTemplates";

		private const string ImportFromRMSOnlineParameterSetName = "RMSOnline";

		private const string ImportFromRMSOnline2ParameterSetName = "RMSOnline2";

		internal const string RMSOnlinePsuedoDataObjectName = "RMS Online";

		internal const string RMSOnlineAuthorTest_TrustedRootHierarchy = "TrustedRootHierarchy";

		internal const string RMSOnlineAuthorTest_KeyProtectionCertificate = "KeyProtectionCertificate";

		internal const string RMSOnlineAuthorTest_KeyProtectionCertificatePassword = "KeyProtectionCertificatePassword";

		private Lazy<ImportRmsTrustedPublishingDomain.Impl> impl;

		private abstract class Impl
		{
			internal ImportRmsTrustedPublishingDomain cmdlet { get; set; }

			internal abstract IConfigurable PrepareDataObject(Func<IConfigurable> basePrepareDataObject);

			internal abstract void InternalValidate(Action baseInternalValidate);

			internal abstract void InternalProcessRecord(Action baseInternalProcessRecord);

			internal abstract void WriteResut(IConfigurable dataObject, Action<IConfigurable> baseWriteResult);
		}

		private class OnPremImpl : ImportRmsTrustedPublishingDomain.Impl
		{
			internal override IConfigurable PrepareDataObject(Func<IConfigurable> basePrepareDataObject)
			{
				RMSTrustedPublishingDomain rmstrustedPublishingDomain;
				if (base.cmdlet.RefreshTemplates)
				{
					RMSTrustedPublishingDomain[] array = ((IConfigurationSession)base.cmdlet.DataSession).Find<RMSTrustedPublishingDomain>(null, QueryScope.SubTree, new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Name, base.cmdlet.Name), null, 1);
					base.cmdlet.ThrowIfExistingTpdNotFound(array);
					rmstrustedPublishingDomain = array[0];
				}
				else
				{
					if (base.cmdlet.RMSOnline)
					{
						base.cmdlet.UpdateTpdNameForRmsOnline();
					}
					rmstrustedPublishingDomain = (RMSTrustedPublishingDomain)basePrepareDataObject();
					rmstrustedPublishingDomain.SetId((IConfigurationSession)base.cmdlet.DataSession, base.cmdlet.Name);
				}
				return rmstrustedPublishingDomain;
			}

			internal override void InternalValidate(Action baseInternalValidate)
			{
				baseInternalValidate();
				if (base.cmdlet.HasErrors)
				{
					return;
				}
				IRMConfiguration irmconfiguration = base.cmdlet.ReadIrmConfiguration();
				if (base.cmdlet.RMSOnline)
				{
					base.cmdlet.ThrowIfRmsOnlinePreRequisitesNotMet(irmconfiguration);
					this.trustedDoc = base.cmdlet.GetTpdFromRmsOnline(irmconfiguration.RMSOnlineKeySharingLocation);
				}
				else
				{
					this.trustedDoc = base.cmdlet.ParseTpdFileData(base.cmdlet.Password, base.cmdlet.FileData);
				}
				object obj = null;
				try
				{
					TpdValidator tpdValidator = new TpdValidator(irmconfiguration.InternalLicensingEnabled, base.cmdlet.IntranetLicensingUrl, base.cmdlet.ExtranetLicensingUrl, base.cmdlet.IntranetCertificationUrl, base.cmdlet.ExtranetCertificationUrl, base.cmdlet.RMSOnline, base.cmdlet.Default, base.cmdlet.RefreshTemplates);
					this.dkmEncryptedPrivateKey = tpdValidator.ValidateTpdSuitableForImport(this.trustedDoc, base.cmdlet.Name, out obj, base.cmdlet.ConfigurationSession, base.cmdlet.DataObject.KeyId, base.cmdlet.DataObject.KeyIdType, base.cmdlet.DataObject.IntranetLicensingUrl, base.cmdlet.DataObject.ExtranetLicensingUrl, base.cmdlet.Password);
					if (base.cmdlet.RMSOnline && !base.cmdlet.RefreshTemplates && RmsUtil.TPDExists(base.cmdlet.ConfigurationSession, this.trustedDoc.m_ttdki.strID, this.trustedDoc.m_ttdki.strIDType))
					{
						base.cmdlet.WriteWarning(Strings.TpdAlreadyImported);
						this.SkipImport = true;
					}
				}
				catch (LocalizedException ex)
				{
					ImportRmsTrustedPublishingDomain.WriteFailureToEventLog(ex);
					base.cmdlet.WriteError(ex, ExchangeErrorCategory.Client, obj ?? base.cmdlet.Name);
				}
			}

			internal override void InternalProcessRecord(Action baseInternalProcessRecord)
			{
				if (!this.SkipImport)
				{
					RMSTrustedPublishingDomain dataObject = base.cmdlet.DataObject;
					IRMConfiguration irmconfiguration = null;
					RMSTrustedPublishingDomain rmstrustedPublishingDomain = null;
					if (!base.cmdlet.RefreshTemplates)
					{
						if (base.cmdlet.Default)
						{
							dataObject.Default = true;
						}
						else
						{
							dataObject.Default = !RmsUtil.TPDExists(base.cmdlet.ConfigurationSession, null);
						}
						irmconfiguration = base.cmdlet.ReadIrmConfiguration();
						dataObject.CSPType = this.trustedDoc.m_ttdki.nCSPType;
						dataObject.CSPName = this.trustedDoc.m_ttdki.strCSPName;
						dataObject.KeyContainerName = this.trustedDoc.m_ttdki.strKeyContainerName;
						dataObject.KeyNumber = this.trustedDoc.m_ttdki.nKeyNumber;
						dataObject.KeyId = this.trustedDoc.m_ttdki.strID;
						dataObject.KeyIdType = this.trustedDoc.m_ttdki.strIDType;
						if (!string.IsNullOrEmpty(this.dkmEncryptedPrivateKey))
						{
							dataObject.PrivateKey = this.dkmEncryptedPrivateKey;
						}
						XrmlCertificateChain xrmlCertificateChain = base.cmdlet.ReenrollSlcCertificateChain(new XrmlCertificateChain(this.trustedDoc.m_strLicensorCertChain), base.cmdlet.Name);
						dataObject.SLCCertChain = RMUtil.CompressSLCCertificateChain(xrmlCertificateChain.ToStringArray());
						dataObject.IntranetLicensingUrl = RMUtil.ConvertUriToLicenseLocationDistributionPoint(base.cmdlet.IntranetLicensingUrl);
						dataObject.ExtranetLicensingUrl = RMUtil.ConvertUriToLicenseLocationDistributionPoint(base.cmdlet.ExtranetLicensingUrl);
						dataObject.IntranetCertificationUrl = RMUtil.ConvertUriToCertificateLocationDistributionPoint(base.cmdlet.IntranetCertificationUrl ?? base.cmdlet.IntranetLicensingUrl);
						dataObject.ExtranetCertificationUrl = RMUtil.ConvertUriToCertificateLocationDistributionPoint(base.cmdlet.ExtranetCertificationUrl ?? base.cmdlet.ExtranetLicensingUrl);
						if (dataObject.Default)
						{
							try
							{
								ImportRmsTrustedPublishingDomain.ChangeDefaultTPDAndUpdateIrmConfigData((IConfigurationSession)base.cmdlet.DataSession, irmconfiguration, dataObject, out rmstrustedPublishingDomain);
								irmconfiguration.ServerCertificatesVersion++;
							}
							catch (RightsManagementServerException ex)
							{
								ImportRmsTrustedPublishingDomain.WriteFailureToEventLog(ex);
								base.cmdlet.WriteError(new FailedToGenerateSharedKeyException(ex), ExchangeErrorCategory.Client, base.cmdlet.Name);
							}
						}
						if (!irmconfiguration.LicensingLocation.Contains(dataObject.ExtranetLicensingUrl))
						{
							irmconfiguration.LicensingLocation.Add(dataObject.ExtranetLicensingUrl);
						}
						if (!irmconfiguration.LicensingLocation.Contains(dataObject.IntranetLicensingUrl))
						{
							irmconfiguration.LicensingLocation.Add(dataObject.IntranetLicensingUrl);
						}
					}
					dataObject.RMSTemplates = base.cmdlet.CompressAndUpdateTemplates(dataObject, this.trustedDoc.m_astrRightsTemplates, base.cmdlet.RefreshTemplates, base.cmdlet.RMSOnline ? RmsTemplateType.Distributed : RmsTemplateType.Archived, out this.rmsTpdResult);
					if (rmstrustedPublishingDomain != null)
					{
						base.cmdlet.WriteWarning(Strings.WarningChangeDefaultTPD(rmstrustedPublishingDomain.Name, dataObject.Name));
						base.cmdlet.DataSession.Save(rmstrustedPublishingDomain);
					}
					if (irmconfiguration != null)
					{
						base.cmdlet.DataSession.Save(irmconfiguration);
					}
					baseInternalProcessRecord();
				}
			}

			internal override void WriteResut(IConfigurable dataObject, Action<IConfigurable> baseWriteResult)
			{
				baseWriteResult(this.rmsTpdResult);
			}

			private TrustedDocDomain trustedDoc;

			private string dkmEncryptedPrivateKey;

			private bool SkipImport;

			private ImportRmsTrustedPublishingDomainResult rmsTpdResult;
		}

		private class RMSOnlineImpl : ImportRmsTrustedPublishingDomain.Impl
		{
			internal override IConfigurable PrepareDataObject(Func<IConfigurable> basePrepareDataObject)
			{
				RMSTrustedPublishingDomain rmstrustedPublishingDomain = (RMSTrustedPublishingDomain)basePrepareDataObject();
				rmstrustedPublishingDomain.SetId((IConfigurationSession)base.cmdlet.DataSession, "RMS Online");
				return rmstrustedPublishingDomain;
			}

			internal override void InternalValidate(Action baseInternalValidate)
			{
				baseInternalValidate();
				if (base.cmdlet.HasErrors)
				{
					return;
				}
				IRMConfiguration irmconfiguration = base.cmdlet.ReadIrmConfiguration();
				Guid parsedResultNotUsed;
				if (base.cmdlet.RMSOnlineKeys.Keys.Cast<object>().All((object key) => key != null && Guid.TryParse(key.ToString(), out parsedResultNotUsed)))
				{
					if (base.cmdlet.RMSOnlineKeys.Values.Cast<object>().All((object value) => value is byte[] && value != null))
					{
						goto IL_A9;
					}
				}
				base.cmdlet.WriteError(new RmsOnlineFailedToValidateKeys(), ExchangeErrorCategory.Client, base.cmdlet.RMSOnlineKeys);
				IL_A9:
				ImportRmsTrustedPublishingDomain.RMSOnlineImpl.RMSOCompanyConfigurationKeyData trustedDocDomain = new ImportRmsTrustedPublishingDomain.RMSOnlineImpl.RMSOCompanyConfigurationKeyData(RmsUtil.GetExternalDirectoryOrgIdThrowOnFailure(base.cmdlet.ConfigurationSession, base.cmdlet.CurrentOrganizationId), from DictionaryEntry de in base.cmdlet.RMSOnlineKeys
				select new ImportRmsTrustedPublishingDomain.RMSOnlineImpl.RMSOTenantKey(Guid.Parse((string)de.Key), (byte[])de.Value), base.cmdlet.RMSOnlineConfig);
				this.trustedDocs = new List<ImportRmsTrustedPublishingDomain.RMSOnlineImpl.IntermediateTrustedDocDomain>(from trustedDoc in this.GetTpdsFromRmsOnline(trustedDocDomain)
				select new ImportRmsTrustedPublishingDomain.RMSOnlineImpl.IntermediateTrustedDocDomain
				{
					trustedDocDomain = trustedDoc
				});
				object obj = null;
				try
				{
					foreach (ImportRmsTrustedPublishingDomain.RMSOnlineImpl.IntermediateTrustedDocDomain intermediateTrustedDocDomain in this.trustedDocs)
					{
						TpdValidator tpdValidator = new ImportRmsTrustedPublishingDomain.RMSOnlineImpl.RMSOnlineTpdValidator(irmconfiguration.InternalLicensingEnabled, this.configurationInfo, (TrustedRootHierarchy)base.cmdlet.RMSOnlineAuthorTest["TrustedRootHierarchy"]);
						intermediateTrustedDocDomain.dkmEncryptedPrivateKey = tpdValidator.ValidateTpdSuitableForImport(intermediateTrustedDocDomain.trustedDocDomain, "RMS Online", out obj, base.cmdlet.ConfigurationSession, null, null, null, null, null);
					}
				}
				catch (LocalizedException ex)
				{
					ImportRmsTrustedPublishingDomain.WriteFailureToEventLog(ex);
					base.cmdlet.WriteError(ex, ExchangeErrorCategory.Client, obj ?? "RMS Online");
				}
			}

			internal override void InternalProcessRecord(Action baseInternalProcessRecord)
			{
				if (this.configurationInfo == null)
				{
					ExManagementApplicationLogger.LogEvent(ManagementEventLogConstants.Tuple_TenantConfigurationIsNull, new string[0]);
					base.cmdlet.WriteWarning(Strings.TenantConfigurationInfoIsNull);
					return;
				}
				bool flag;
				IRMConfiguration irmconfiguration = IRMConfiguration.Read((IConfigurationSession)base.cmdlet.DataSession, out flag);
				RMSTrustedPublishingDomain[] array = ((IConfigurationSession)base.cmdlet.DataSession).Find<RMSTrustedPublishingDomain>(null, QueryScope.SubTree, null, null, 0);
				RMSTrustedPublishingDomain defaultTrustedPublishingDomain = array.FirstOrDefault((RMSTrustedPublishingDomain tpd) => tpd.Default);
				bool flag2 = defaultTrustedPublishingDomain != null && defaultTrustedPublishingDomain.IsRMSOnline;
				LinkedList<RMSTrustedPublishingDomain> linkedList = new LinkedList<RMSTrustedPublishingDomain>(from tpd in array
				where tpd.IsRMSOnline && tpd != defaultTrustedPublishingDomain
				select tpd);
				LinkedList<ImportRmsTrustedPublishingDomain.RMSOnlineImpl.RMSTrustedPublishingDomainAndResult> linkedList2 = new LinkedList<ImportRmsTrustedPublishingDomain.RMSOnlineImpl.RMSTrustedPublishingDomainAndResult>();
				ImportRmsTrustedPublishingDomain.RMSOnlineImpl.RMSOnlineUniqueADNamesEnumerator rmsOnlineUniqueDataObjectNames = new ImportRmsTrustedPublishingDomain.RMSOnlineImpl.RMSOnlineUniqueADNamesEnumerator(array);
				foreach (ImportRmsTrustedPublishingDomain.RMSOnlineImpl.IntermediateTrustedDocDomain intermediateTrustedDoc in this.trustedDocs)
				{
					this.InternalProcessRecord_ProcessIntermediateTrustedDoc(irmconfiguration, defaultTrustedPublishingDomain, flag2, linkedList, linkedList2, rmsOnlineUniqueDataObjectNames, intermediateTrustedDoc);
				}
				if (this.configurationInfo.FunctionalState == 1 || defaultTrustedPublishingDomain == null)
				{
					irmconfiguration.InternalLicensingEnabled = (irmconfiguration.ClientAccessServerEnabled = (this.configurationInfo.FunctionalState == 1));
				}
				if (flag && linkedList2.Any<ImportRmsTrustedPublishingDomain.RMSOnlineImpl.RMSTrustedPublishingDomainAndResult>())
				{
					base.cmdlet.DataSession.Save(irmconfiguration);
				}
				linkedList.All(delegate(RMSTrustedPublishingDomain tpd)
				{
					base.cmdlet.DataSession.Delete(tpd);
					return true;
				});
				linkedList2.All(delegate(ImportRmsTrustedPublishingDomain.RMSOnlineImpl.RMSTrustedPublishingDomainAndResult tpd)
				{
					tpd.domain.OrganizationId = base.cmdlet.CurrentOrganizationId;
					base.cmdlet.DataSession.Save(tpd.domain);
					base.cmdlet.WriteResult(tpd.result);
					return true;
				});
				if (flag2 && !this.trustedDocs.Any<ImportRmsTrustedPublishingDomain.RMSOnlineImpl.IntermediateTrustedDocDomain>())
				{
					irmconfiguration.InternalLicensingEnabled = false;
					irmconfiguration.SharedServerBoxRacIdentity = null;
					irmconfiguration.PublishingLocation = null;
					irmconfiguration.ServiceLocation = null;
					irmconfiguration.LicensingLocation = null;
					base.cmdlet.DataSession.Delete(defaultTrustedPublishingDomain);
				}
				irmconfiguration.RMSOnlineVersion = (this.trustedDocs.Any<ImportRmsTrustedPublishingDomain.RMSOnlineImpl.IntermediateTrustedDocDomain>() ? this.configurationInfo.DataVersions.Item2 : string.Empty);
				base.cmdlet.DataSession.Save(irmconfiguration);
			}

			private void InternalProcessRecord_ProcessIntermediateTrustedDoc(IRMConfiguration irmConfiguration, RMSTrustedPublishingDomain defaultTrustedPublishingDomain, bool defaultTrustedPublishingDomainIsRMSOnline, LinkedList<RMSTrustedPublishingDomain> existingRMSOTrustedPublishingDomainsToReuse, LinkedList<ImportRmsTrustedPublishingDomain.RMSOnlineImpl.RMSTrustedPublishingDomainAndResult> updatedTrustedPublishingDomains, ImportRmsTrustedPublishingDomain.RMSOnlineImpl.RMSOnlineUniqueADNamesEnumerator rmsOnlineUniqueDataObjectNames, ImportRmsTrustedPublishingDomain.RMSOnlineImpl.IntermediateTrustedDocDomain intermediateTrustedDoc)
			{
				RMSTrustedPublishingDomain rmstrustedPublishingDomain = this.InternalProcessRecord_GetRMSTrustedPublishingDomainToUse(existingRMSOTrustedPublishingDomainsToReuse, rmsOnlineUniqueDataObjectNames);
				rmstrustedPublishingDomain.Default = (this.configurationInfo.FunctionalState == 1 && intermediateTrustedDoc == this.trustedDocs.First<ImportRmsTrustedPublishingDomain.RMSOnlineImpl.IntermediateTrustedDocDomain>());
				this.InternalProcessRecord_InitializeTrustedPublishingDomainGenericProperties(intermediateTrustedDoc, rmstrustedPublishingDomain);
				if (rmstrustedPublishingDomain.Default)
				{
					this.InternalProcessRecord_ChangeDefaultTPDAndUpdateIrmConfigData(irmConfiguration, rmstrustedPublishingDomain, defaultTrustedPublishingDomain);
				}
				ImportRmsTrustedPublishingDomain.RMSOnlineImpl.AddIfNotContains<Uri>(irmConfiguration.LicensingLocation, rmstrustedPublishingDomain.ExtranetLicensingUrl);
				ImportRmsTrustedPublishingDomain.RMSOnlineImpl.AddIfNotContains<Uri>(irmConfiguration.LicensingLocation, rmstrustedPublishingDomain.IntranetLicensingUrl);
				ImportRmsTrustedPublishingDomainResult result;
				rmstrustedPublishingDomain.RMSTemplates = base.cmdlet.CompressAndUpdateTemplates(rmstrustedPublishingDomain, intermediateTrustedDoc.trustedDocDomain.m_astrRightsTemplates, false, RmsTemplateType.Distributed, out result);
				if (rmstrustedPublishingDomain.Default)
				{
					if (defaultTrustedPublishingDomainIsRMSOnline)
					{
						base.cmdlet.DataSession.Delete(defaultTrustedPublishingDomain);
					}
					else if (defaultTrustedPublishingDomain != null)
					{
						base.cmdlet.DataSession.Save(defaultTrustedPublishingDomain);
					}
				}
				updatedTrustedPublishingDomains.AddLast(new ImportRmsTrustedPublishingDomain.RMSOnlineImpl.RMSTrustedPublishingDomainAndResult
				{
					domain = rmstrustedPublishingDomain,
					result = result
				});
			}

			private RMSTrustedPublishingDomain InternalProcessRecord_GetRMSTrustedPublishingDomainToUse(LinkedList<RMSTrustedPublishingDomain> existingRMSOTrustedPublishingDomainsToReuse, ImportRmsTrustedPublishingDomain.RMSOnlineImpl.RMSOnlineUniqueADNamesEnumerator rmsOnlineUniqueDataObjectNames)
			{
				RMSTrustedPublishingDomain rmstrustedPublishingDomain;
				if (existingRMSOTrustedPublishingDomainsToReuse.Any<RMSTrustedPublishingDomain>())
				{
					rmstrustedPublishingDomain = existingRMSOTrustedPublishingDomainsToReuse.First<RMSTrustedPublishingDomain>();
					existingRMSOTrustedPublishingDomainsToReuse.RemoveFirst();
				}
				else
				{
					rmstrustedPublishingDomain = new RMSTrustedPublishingDomain();
					rmstrustedPublishingDomain.SetId((IConfigurationSession)base.cmdlet.DataSession, rmsOnlineUniqueDataObjectNames.GetNext());
					rmstrustedPublishingDomain.Name = rmstrustedPublishingDomain.Id.Name;
				}
				return rmstrustedPublishingDomain;
			}

			private void InternalProcessRecord_InitializeTrustedPublishingDomainGenericProperties(ImportRmsTrustedPublishingDomain.RMSOnlineImpl.IntermediateTrustedDocDomain intermediateTrustedDoc, RMSTrustedPublishingDomain trustedPublishingDomain)
			{
				trustedPublishingDomain.CSPType = intermediateTrustedDoc.trustedDocDomain.m_ttdki.nCSPType;
				trustedPublishingDomain.CSPName = intermediateTrustedDoc.trustedDocDomain.m_ttdki.strCSPName;
				trustedPublishingDomain.KeyContainerName = intermediateTrustedDoc.trustedDocDomain.m_ttdki.strKeyContainerName;
				trustedPublishingDomain.KeyNumber = intermediateTrustedDoc.trustedDocDomain.m_ttdki.nKeyNumber;
				trustedPublishingDomain.KeyId = intermediateTrustedDoc.trustedDocDomain.m_ttdki.strID;
				trustedPublishingDomain.KeyIdType = intermediateTrustedDoc.trustedDocDomain.m_ttdki.strIDType;
				if (!string.IsNullOrEmpty(intermediateTrustedDoc.dkmEncryptedPrivateKey))
				{
					trustedPublishingDomain.PrivateKey = intermediateTrustedDoc.dkmEncryptedPrivateKey;
				}
				trustedPublishingDomain.SLCCertChain = RMUtil.CompressSLCCertificateChain(new XrmlCertificateChain(intermediateTrustedDoc.trustedDocDomain.m_strLicensorCertChain).ToStringArray());
				trustedPublishingDomain.IntranetLicensingUrl = RMUtil.ConvertUriToLicenseLocationDistributionPoint(this.configurationInfo.LicensingIntranetDistributionPointUrl);
				trustedPublishingDomain.ExtranetLicensingUrl = RMUtil.ConvertUriToLicenseLocationDistributionPoint(this.configurationInfo.LicensingExtranetDistributionPointUrl);
				trustedPublishingDomain.IntranetCertificationUrl = RMUtil.ConvertUriToCertificateLocationDistributionPoint(this.configurationInfo.CertificationIntranetDistributionPointUrl ?? this.configurationInfo.LicensingIntranetDistributionPointUrl);
				trustedPublishingDomain.ExtranetCertificationUrl = RMUtil.ConvertUriToCertificateLocationDistributionPoint(this.configurationInfo.CertificationExtranetDistributionPointUrl ?? this.configurationInfo.LicensingExtranetDistributionPointUrl);
			}

			private void InternalProcessRecord_ChangeDefaultTPDAndUpdateIrmConfigData(IRMConfiguration irmConfiguration, RMSTrustedPublishingDomain trustedPublishingDomain, RMSTrustedPublishingDomain oldDefaultTPD)
			{
				try
				{
					ImportRmsTrustedPublishingDomain.ChangeDefaultTPDAndUpdateIrmConfigData(irmConfiguration, trustedPublishingDomain, oldDefaultTPD);
					irmConfiguration.ServerCertificatesVersion++;
				}
				catch (RightsManagementServerException ex)
				{
					ImportRmsTrustedPublishingDomain.WriteFailureToEventLog(ex);
					base.cmdlet.WriteError(new FailedToGenerateSharedKeyException(ex), ExchangeErrorCategory.Client, "RMS Online");
				}
			}

			internal override void WriteResut(IConfigurable dataObject, Action<IConfigurable> baseWriteResult)
			{
				baseWriteResult(dataObject);
			}

			private IEnumerable<TrustedDocDomain> GetTpdsFromRmsOnline(ICompanyConfigurationKeyData trustedDocDomain)
			{
				ITenantTpdAndConfigurationAuthor author = (base.cmdlet.RMSOnlineAuthorTest["KeyProtectionCertificate"] != null) ? TenantTpdAndConfigurationAuthorFactory.CreateTenantTpdAndConfigurationAuthor(new X509Certificate2(Convert.FromBase64String((string)base.cmdlet.RMSOnlineAuthorTest["KeyProtectionCertificate"]), (string)base.cmdlet.RMSOnlineAuthorTest["KeyProtectionCertificatePassword"])) : TenantTpdAndConfigurationAuthorFactory.CreateTenantTpdAndConfigurationAuthor();
				try
				{
					this.configurationInfo = author.GetTrustedPublishingDomain(trustedDocDomain);
				}
				catch (HardwareSecurityModuleEncryptedKeyException ex)
				{
					this.configurationInfo = null;
					ExManagementApplicationLogger.LogEvent(ManagementEventLogConstants.Tuple_SkipHSMEncryptedTpd, new string[]
					{
						ex.ToString()
					});
					base.cmdlet.WriteWarning(Strings.TpdIsHSMEncrypted);
					yield break;
				}
				if (this.configurationInfo.ActivePublishingDomain != null)
				{
					yield return ImportRmsTrustedPublishingDomain.RMSOnlineImpl.ConvertFromRmsOnlineTrustedDocDomain(this.configurationInfo.ActivePublishingDomain);
					if (this.configurationInfo.ArchivedPublishingDomains != null)
					{
						foreach (ITrustedDocDomain rmsoTpd in this.configurationInfo.ArchivedPublishingDomains)
						{
							yield return ImportRmsTrustedPublishingDomain.RMSOnlineImpl.ConvertFromRmsOnlineTrustedDocDomain(rmsoTpd);
						}
					}
				}
				yield break;
			}

			private static TrustedDocDomain ConvertFromRmsOnlineTrustedDocDomain(ITrustedDocDomain rmsoTPD)
			{
				RmsUtil.ThrowIfParameterNull(rmsoTPD, "rmsoTPD");
				return new TrustedDocDomain
				{
					m_ttdki = ImportRmsTrustedPublishingDomain.RMSOnlineImpl.ConvertFromRmsOnlineKeyInformation(rmsoTPD.KeyInfo),
					m_strLicensorCertChain = rmsoTPD.LicensorCertChain,
					m_astrRightsTemplates = rmsoTPD.RightsTemplates
				};
			}

			private static KeyInformation ConvertFromRmsOnlineKeyInformation(IKeyInformation rmsoKeyInfo)
			{
				RmsUtil.ThrowIfParameterNull(rmsoKeyInfo, "rmsoKeyInfo");
				return new KeyInformation
				{
					strID = rmsoKeyInfo.ID,
					strIDType = rmsoKeyInfo.IdType,
					nCSPType = rmsoKeyInfo.CSPType,
					strCSPName = rmsoKeyInfo.CSPName,
					strKeyContainerName = rmsoKeyInfo.KeyContainerName,
					nKeyNumber = rmsoKeyInfo.KeyNumber,
					strEncryptedPrivateKey = Convert.ToBase64String(rmsoKeyInfo.PrivateKey)
				};
			}

			private static void AddIfNotContains<T>(ICollection<T> collection, T item)
			{
				if (!collection.Contains(item))
				{
					collection.Add(item);
				}
			}

			private List<ImportRmsTrustedPublishingDomain.RMSOnlineImpl.IntermediateTrustedDocDomain> trustedDocs;

			private ITenantTpdAndConfigurationInfo configurationInfo;

			private static readonly Regex reRMSOnlineUniqueADName = new Regex("^RMS Online - (\\d{1,2})$", RegexOptions.Compiled);

			private class IntermediateTrustedDocDomain
			{
				internal TrustedDocDomain trustedDocDomain;

				internal string dkmEncryptedPrivateKey;
			}

			private class RMSOTenantKey : ITenantKey
			{
				internal RMSOTenantKey(Guid keyIdentifier, byte[] key)
				{
					this.KeyIdentifier = keyIdentifier;
					this.Key = key;
				}

				public Guid KeyIdentifier { get; private set; }

				public byte[] Key { get; private set; }
			}

			private class RMSOCompanyConfigurationKeyData : ICompanyConfigurationKeyData
			{
				internal RMSOCompanyConfigurationKeyData(Guid orgId, IEnumerable<ITenantKey> keys, byte[] configuration)
				{
					this.TenantMsodsId = orgId;
					this.TenantKeys = keys.ToArray<ITenantKey>();
					this.TenantConfiguration = configuration;
				}

				public Guid TenantMsodsId { get; private set; }

				public ITenantKey[] TenantKeys { get; private set; }

				public byte[] TenantConfiguration { get; private set; }
			}

			private class RMSOnlineTpdValidator : TpdValidator
			{
				public RMSOnlineTpdValidator(bool internalLicensingEnabled, ITenantTpdAndConfigurationInfo configurationInfo, TrustedRootHierarchy trustedRootHierarchy) : base(internalLicensingEnabled, configurationInfo.LicensingIntranetDistributionPointUrl, configurationInfo.LicensingExtranetDistributionPointUrl, configurationInfo.CertificationIntranetDistributionPointUrl, configurationInfo.CertificationExtranetDistributionPointUrl, new SwitchParameter(true), new SwitchParameter(true), new SwitchParameter(false))
				{
					this.trustedRootHierarchy = trustedRootHierarchy;
				}

				protected override byte[] DecryptPrivateKey(KeyInformation keyInfo, SecureString tpdFilePassword)
				{
					return Convert.FromBase64String(keyInfo.strEncryptedPrivateKey);
				}

				protected override TrustedPublishingDomainImportUtilities CreateTpdImportUtilities(TrustedDocDomain tpd, TrustedPublishingDomainPrivateKeyProvider privateKeyProvider)
				{
					return new TrustedPublishingDomainImportUtilities(this.trustedRootHierarchy, new XrmlCertificateChain(tpd.m_strLicensorCertChain), privateKeyProvider, TraceLevel.Error, int.MaxValue);
				}

				private TrustedRootHierarchy trustedRootHierarchy;
			}

			private class RMSTrustedPublishingDomainAndResult
			{
				internal RMSTrustedPublishingDomain domain;

				internal ImportRmsTrustedPublishingDomainResult result;
			}

			private class RMSOnlineUniqueADNamesEnumerator
			{
				internal RMSOnlineUniqueADNamesEnumerator(RMSTrustedPublishingDomain[] existingTrustedPublishingDomains)
				{
					this.existingIds = new Lazy<HashSet<int>>(() => new HashSet<int>(from tpd in existingTrustedPublishingDomains
					let m = ImportRmsTrustedPublishingDomain.RMSOnlineImpl.reRMSOnlineUniqueADName.Match(tpd.Name)
					where m.Success
					let num = int.Parse(m.Groups[1].Value)
					select num));
				}

				internal string GetNext()
				{
					while (this.existingIds.Value.Contains(this.index))
					{
						this.index++;
					}
					return "RMS Online - " + this.index++.ToString();
				}

				private Lazy<HashSet<int>> existingIds;

				private int index = 1;
			}
		}
	}
}
