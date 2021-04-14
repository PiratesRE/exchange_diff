using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Autodiscover.WCF
{
	[DataContract(Name = "DocumentSharingLocation", Namespace = "http://schemas.microsoft.com/exchange/2010/Autodiscover")]
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

		[DataMember(Name = "ServiceUrl", IsRequired = true, Order = 1)]
		public string ServiceUrl
		{
			get
			{
				return this.serviceUrl;
			}
			set
			{
				this.serviceUrl = value;
			}
		}

		[DataMember(Name = "LocationUrl", IsRequired = true, Order = 2)]
		public string LocationUrl
		{
			get
			{
				return this.locationUrl;
			}
			set
			{
				this.locationUrl = value;
			}
		}

		[DataMember(Name = "DisplayName", IsRequired = true, Order = 3)]
		public string DisplayName
		{
			get
			{
				return this.displayName;
			}
			set
			{
				this.displayName = value;
			}
		}

		[DataMember(Name = "SupportedFileExtensions", IsRequired = true, Order = 4)]
		public FileExtensionCollection SupportedFileExtensions
		{
			get
			{
				return this.supportedFileExtensions;
			}
			set
			{
				this.supportedFileExtensions = value;
			}
		}

		[DataMember(Name = "ExternalAccessAllowed", IsRequired = true, Order = 5)]
		public bool ExternalAccessAllowed
		{
			get
			{
				return this.externalAccessAllowed;
			}
			set
			{
				this.externalAccessAllowed = value;
			}
		}

		[DataMember(Name = "AnonymousAccessAllowed", IsRequired = true, Order = 6)]
		public bool AnonymousAccessAllowed
		{
			get
			{
				return this.anonymousAccessAllowed;
			}
			set
			{
				this.anonymousAccessAllowed = value;
			}
		}

		[DataMember(Name = "CanModifyPermissions", IsRequired = true, Order = 7)]
		public bool CanModifyPermissions
		{
			get
			{
				return this.canModifyPermissions;
			}
			set
			{
				this.canModifyPermissions = value;
			}
		}

		[DataMember(Name = "IsDefault", IsRequired = true, Order = 8)]
		public bool IsDefault
		{
			get
			{
				return this.isDefault;
			}
			set
			{
				this.isDefault = value;
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
