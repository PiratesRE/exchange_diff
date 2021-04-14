using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Management.Automation;
using System.Web.Security.AntiXss;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.E4E
{
	[Cmdlet("Set", "OMEConfiguration", DefaultParameterSetName = "Identity")]
	public sealed class SetOMEConfiguration : SetTenantADTaskBase<OMEConfigurationIdParameter, EncryptionConfiguration, EncryptionConfiguration>
	{
		[Parameter(Mandatory = false, ValueFromPipeline = true)]
		public OrganizationIdParameter Organization
		{
			get
			{
				return (OrganizationIdParameter)base.Fields["Organization"];
			}
			set
			{
				base.Fields["Organization"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public byte[] Image
		{
			get
			{
				return (byte[])base.Fields["Image"];
			}
			set
			{
				base.Fields["Image"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string EmailText
		{
			get
			{
				return (string)base.Fields["EmailText"];
			}
			set
			{
				base.Fields["EmailText"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string PortalText
		{
			get
			{
				return (string)base.Fields["PortalText"];
			}
			set
			{
				base.Fields["PortalText"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string DisclaimerText
		{
			get
			{
				return (string)base.Fields["DisclaimerText"];
			}
			set
			{
				base.Fields["DisclaimerText"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool OTPEnabled
		{
			get
			{
				return (bool)base.Fields["OTPEnabled"];
			}
			set
			{
				base.Fields["OTPEnabled"] = value;
			}
		}

		protected override IConfigDataProvider CreateSession()
		{
			if (this.Organization != null)
			{
				this.SetCurrentOrganizationId();
			}
			if (base.CurrentOrganizationId == null || base.CurrentOrganizationId.OrganizationalUnit == null || string.IsNullOrWhiteSpace(base.CurrentOrganizationId.OrganizationalUnit.Name))
			{
				base.WriteError(new LocalizedException(Strings.ErrorParameterRequired("Organization")), ErrorCategory.InvalidArgument, null);
			}
			string organizationRawIdentity;
			if (this.Organization == null)
			{
				organizationRawIdentity = base.CurrentOrganizationId.OrganizationalUnit.Name;
			}
			else
			{
				organizationRawIdentity = this.Organization.RawIdentity;
			}
			return new EncryptionConfigurationDataProvider(organizationRawIdentity);
		}

		protected override IConfigurable PrepareDataObject()
		{
			EncryptionConfiguration encryptionConfiguration = (EncryptionConfiguration)base.PrepareDataObject();
			if (base.Fields.IsModified("Image"))
			{
				encryptionConfiguration.Image = this.Image;
				if (this.Image != null && this.Image.Length > 0)
				{
					if (this.Image.Length > 40960)
					{
						base.WriteError(new LocalizedException(Strings.ErrorImageFileBig(40)), ErrorCategory.InvalidArgument, null);
					}
					try
					{
						using (MemoryStream memoryStream = new MemoryStream(this.Image))
						{
							Bitmap bitmap = new Bitmap(memoryStream);
							if (bitmap.Height > 70 || bitmap.Width > 170)
							{
								int height;
								int width;
								if (bitmap.Height * 170 > bitmap.Width * 70)
								{
									height = 70;
									width = bitmap.Width * 70 / bitmap.Height;
								}
								else
								{
									width = 170;
									height = bitmap.Height * 170 / bitmap.Width;
								}
								bitmap = new Bitmap(bitmap, width, height);
							}
							using (MemoryStream memoryStream2 = new MemoryStream())
							{
								bitmap.Save(memoryStream2, ImageFormat.Png);
								encryptionConfiguration.Image = memoryStream2.ToArray();
							}
						}
					}
					catch (Exception)
					{
						base.WriteError(new LocalizedException(Strings.ErrorImageImport), ErrorCategory.InvalidArgument, null);
					}
				}
			}
			if (base.Fields.IsModified("EmailText"))
			{
				encryptionConfiguration.EmailText = AntiXssEncoder.HtmlEncode(this.EmailText, false);
			}
			if (base.Fields.IsModified("PortalText"))
			{
				encryptionConfiguration.PortalText = AntiXssEncoder.HtmlEncode(this.PortalText, false);
			}
			if (base.Fields.IsModified("DisclaimerText"))
			{
				encryptionConfiguration.DisclaimerText = AntiXssEncoder.HtmlEncode(this.DisclaimerText, false);
			}
			if (base.Fields.IsModified("OTPEnabled"))
			{
				encryptionConfiguration.OTPEnabled = this.OTPEnabled;
			}
			return encryptionConfiguration;
		}

		protected override void InternalValidate()
		{
			base.InternalValidate();
			if (base.Fields.IsModified("EmailText") && !string.IsNullOrWhiteSpace(this.EmailText) && this.EmailText.Length > 1024)
			{
				base.WriteError(new LocalizedException(Strings.ErrorEmailTextBig(1024)), ErrorCategory.InvalidArgument, null);
			}
			if (base.Fields.IsModified("PortalText") && !string.IsNullOrWhiteSpace(this.PortalText) && this.PortalText.Length > 128)
			{
				base.WriteError(new LocalizedException(Strings.ErrorPortalTextBig(128)), ErrorCategory.InvalidArgument, null);
			}
			if (base.Fields.IsModified("DisclaimerText") && !string.IsNullOrWhiteSpace(this.DisclaimerText) && this.DisclaimerText.Length > 1024)
			{
				base.WriteError(new LocalizedException(Strings.ErrorDisclaimerTextBig(1024)), ErrorCategory.InvalidArgument, null);
			}
		}

		private void SetCurrentOrganizationId()
		{
			IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(base.DomainController, true, ConsistencyMode.PartiallyConsistent, null, base.SessionSettings, 285, "SetCurrentOrganizationId", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\E4E\\SetEncryptionConfiguration.cs");
			tenantOrTopologyConfigurationSession.UseConfigNC = false;
			ADOrganizationalUnit adorganizationalUnit = (ADOrganizationalUnit)base.GetDataObject<ADOrganizationalUnit>(this.Organization, tenantOrTopologyConfigurationSession, null, null, new LocalizedString?(Strings.ErrorOrganizationNotFound(this.Organization.ToString())), new LocalizedString?(Strings.ErrorOrganizationNotUnique(this.Organization.ToString())), ExchangeErrorCategory.Client);
			base.CurrentOrganizationId = adorganizationalUnit.OrganizationId;
		}

		private const int maxWidth = 170;

		private const int maxHeight = 70;

		private const int maxImageSize = 40960;

		private const int maxEmailTextSize = 1024;

		private const int maxPortalTextSize = 128;

		private const int maxDisclaimerTextSize = 1024;
	}
}
