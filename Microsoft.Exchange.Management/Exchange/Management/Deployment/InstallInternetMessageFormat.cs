using System;
using System.Management.Automation;
using System.Threading;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Globalization;

namespace Microsoft.Exchange.Management.Deployment
{
	[Cmdlet("Install", "InternetMessageFormat")]
	public sealed class InstallInternetMessageFormat : NewMultitenancyFixedNameSystemConfigurationObjectTask<ContentConfigContainer>
	{
		protected override IConfigurable PrepareDataObject()
		{
			QueryFilter filter = new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Name, "Internet Message Formats");
			IConfigurationSession configurationSession = (IConfigurationSession)base.DataSession;
			configurationSession.SessionSettings.IsSharedConfigChecked = true;
			ADObjectId childId = configurationSession.GetOrgContainerId().GetChildId("Global Settings");
			ContentConfigContainer[] array = configurationSession.Find<ContentConfigContainer>(childId, QueryScope.OneLevel, filter, null, 0);
			ContentConfigContainer contentConfigContainer;
			if (this.isContainerExisted = (array != null && array.Length > 0))
			{
				contentConfigContainer = array[0];
			}
			else
			{
				contentConfigContainer = (ContentConfigContainer)base.PrepareDataObject();
				contentConfigContainer.SetId(childId.GetChildId("Internet Message Formats"));
				MultiValuedProperty<string> mimeTypes = contentConfigContainer.MimeTypes;
				foreach (string item in InstallInternetMessageFormat.newTypes)
				{
					if (!mimeTypes.Contains(item))
					{
						mimeTypes.Add(item);
					}
				}
				contentConfigContainer.MimeTypes = mimeTypes;
			}
			return contentConfigContainer;
		}

		protected override void InternalProcessRecord()
		{
			if (!this.isContainerExisted)
			{
				base.InternalProcessRecord();
			}
			IConfigurationSession configurationSession = (IConfigurationSession)base.DataSession;
			DomainContentConfig[] array = configurationSession.Find<DomainContentConfig>(this.DataObject.Id, QueryScope.OneLevel, null, null, 1);
			if (array == null || array.Length == 0)
			{
				DomainContentConfig domainContentConfig = new DomainContentConfig();
				domainContentConfig.SetId(this.DataObject.Id.GetChildId("Default"));
				domainContentConfig.DomainName = new SmtpDomainWithSubdomains("*");
				domainContentConfig.ContentType = ContentType.MimeHtmlText;
				domainContentConfig.DisplaySenderName = true;
				Culture culture = Culture.GetCulture(Thread.CurrentThread.CurrentCulture.LCID);
				domainContentConfig.CharacterSet = culture.MimeCharset.Name;
				domainContentConfig.NonMimeCharacterSet = culture.MimeCharset.Name;
				domainContentConfig.AllowedOOFType = AllowedOOFType.External;
				domainContentConfig.AutoForwardEnabled = false;
				domainContentConfig.AutoReplyEnabled = false;
				domainContentConfig.DeliveryReportEnabled = true;
				domainContentConfig.NDREnabled = true;
				domainContentConfig.OrganizationId = this.DataObject.OrganizationId;
				domainContentConfig.NDRDiagnosticInfoEnabled = true;
				configurationSession.Save(domainContentConfig);
			}
		}

		private static readonly string[] newTypes = new string[]
		{
			"text/html;html",
			"text/plain;txt",
			"text/css;css",
			"text/iuls;uls",
			"text/scriptlet;wsc",
			"text/webviewhtml;htt",
			"text/x-component;htc",
			"text/x-vcard;vcf",
			"text/xml;xml",
			"image/gif;gif",
			"image/jpeg;jpg",
			"image/x-xbitmap;xbm",
			"image/bmp;bmp",
			"image/pjpeg;jpg",
			"image/png;png",
			"image/tiff;tif",
			"image/tiff;tiff",
			"image/x-icon;ico",
			"image/x-png;png",
			"image/xbm;xbm",
			"audio/aiff;aiff",
			"audio/x-aiff;aiff",
			"audio/basic;au",
			"audio/wav;wav",
			"audio/x-wav;wav",
			"audio/x-pn-realaudio;ra",
			"audio/mid;mid",
			"audio/midi;mid",
			"audio/x-midi;mid",
			"audio/mpeg;mp3",
			"audio/x-mpegurl;m3u",
			"video/avi;avi",
			"video/mpeg;mpeg",
			"video/x-mpeg;mp2",
			"video/x-mpeg2a;mp2",
			"video/quicktime;mov",
			"video/msvideo;avi",
			"video/x-msvideo;avi",
			"video/x-ivf;ivf",
			"video/x-la-asf;lsx",
			"video/x-ms-asf;asx",
			"application/rtf;rtf",
			"application/msword;doc",
			"application/msaccess;mdb",
			"application/postscript;ps",
			"application/vnd.ms-excel;xls",
			"application/x-msexcel;xls",
			"application/vnd.ms-powerpoint;ppt",
			"application/x-mspowerpoint;ppt",
			"application/vnd.ms-project;mpp",
			"application/cdf;cdf",
			"application/x-cdf;cdf",
			"application/fractals;fif",
			"application/futuresplash;spl",
			"application/hta;hta",
			"application/mac-binhex40;hqx",
			"application/pkcs10;p10",
			"application/pkcs7-mime;p7m",
			"application/pkcs7-signature;p7s",
			"application/pkix-cert;cer",
			"application/pkix-crl;crl",
			"application/vnd.ms-pki.certstore;sst",
			"application/vnd.ms-pki.pko;pko",
			"application/vnd.ms-pki.seccat;cat",
			"application/vnd.ms-pki.stl;stl",
			"application/x-compress;z",
			"application/x-compressed;tgz",
			"application/x-director;dir",
			"application/x-gzip;gz",
			"application/x-internet-signup;ins",
			"application/x-iphone;iii",
			"application/x-latex;latex",
			"application/x-mix-transfer;nix",
			"application/x-mplayer2;asx",
			"application/x-pkcs12;p12",
			"application/x-pkcs7-certificates;p7b",
			"application/x-pkcs7-certreqresp;p7r",
			"application/x-shockwave-flash;swf",
			"application/x-stuffit;sit",
			"application/x-tar;tar",
			"application/x-troff-man;man",
			"application/x-x509-ca-cert;cer",
			"application/x-zip-compressed;zip",
			"application/xml;xml",
			"drawing/x-dwf;dwf",
			"model/vnd.dwf;dwf"
		};

		private bool isContainerExisted;
	}
}
