using System;
using Microsoft.Exchange.Autodiscover.WCF;

namespace Microsoft.Exchange.Autodiscover.ConfigurationSettings
{
	public class DocumentSharingLocation
	{
		public DocumentSharingLocation(string serviceUrl, string locationUrl, string displayName, FileExtensionCollection supportedFileExtensions, bool externalAccessAllowed, bool anonymousAccessAllowed, bool canModifyPermissions, bool isDefault)
		{
			Common.ThrowIfNullOrEmpty(serviceUrl, "serviceUrl");
			Common.ThrowIfNullOrEmpty(locationUrl, "locationUrl");
			Common.ThrowIfNullOrEmpty(displayName, "displayName");
			if (supportedFileExtensions == null)
			{
				throw new ArgumentNullException("supportedFileExtensions");
			}
			if (supportedFileExtensions.Count == 0)
			{
				throw new ArgumentException("supportedFileExtensions must not be empty.");
			}
			this.serviceUrl = serviceUrl;
			this.locationUrl = locationUrl;
			this.displayName = displayName;
			this.supportedFileExtensions = supportedFileExtensions;
			this.externalAccessAllowed = externalAccessAllowed;
			this.anonymousAccessAllowed = anonymousAccessAllowed;
			this.canModifyPermissions = canModifyPermissions;
			this.isDefault = isDefault;
		}

		public string ServiceUrl
		{
			get
			{
				return this.serviceUrl;
			}
		}

		public string LocationUrl
		{
			get
			{
				return this.locationUrl;
			}
		}

		public string DisplayName
		{
			get
			{
				return this.displayName;
			}
		}

		public FileExtensionCollection SupportedFileExtensions
		{
			get
			{
				return this.supportedFileExtensions;
			}
		}

		public bool ExternalAccessAllowed
		{
			get
			{
				return this.externalAccessAllowed;
			}
		}

		public bool AnonymousAccessAllowed
		{
			get
			{
				return this.anonymousAccessAllowed;
			}
		}

		public bool CanModifyPermissions
		{
			get
			{
				return this.canModifyPermissions;
			}
		}

		public bool IsDefault
		{
			get
			{
				return this.isDefault;
			}
		}

		private string serviceUrl;

		private string locationUrl;

		private string displayName;

		private FileExtensionCollection supportedFileExtensions;

		private bool externalAccessAllowed;

		private bool anonymousAccessAllowed;

		private bool canModifyPermissions;

		private bool isDefault;
	}
}
