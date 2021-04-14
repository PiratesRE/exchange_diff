using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.Tools
{
	[Serializable]
	public class ToolInformation : ConfigurableObject
	{
		internal ToolInformation(ToolId identity, ToolVersionStatus versionStatus, Version minimumSupportedVersion, Version latestVersion, Uri updateInformationUrl) : this()
		{
			this.Identity = identity;
			this.VersionStatus = versionStatus;
			this.MinimumSupportedVersion = minimumSupportedVersion;
			this.LatestVersion = latestVersion;
			this.UpdateInformationUrl = updateInformationUrl;
		}

		internal ToolInformation() : base(new SimpleProviderPropertyBag())
		{
			this.propertyBag.SetField(this.propertyBag.ObjectVersionPropertyDefinition, ExchangeObjectVersion.Exchange2010);
		}

		internal override ObjectSchema ObjectSchema
		{
			get
			{
				return ToolInformation.schema;
			}
		}

		public new ToolId Identity
		{
			get
			{
				return (ToolId)this.propertyBag[ToolInformationSchema.Identity];
			}
			private set
			{
				this.propertyBag[ToolInformationSchema.Identity] = value;
			}
		}

		public ToolVersionStatus VersionStatus
		{
			get
			{
				return (ToolVersionStatus)this.propertyBag[ToolInformationSchema.VersionStatus];
			}
			private set
			{
				this.propertyBag[ToolInformationSchema.VersionStatus] = value;
			}
		}

		public Version MinimumSupportedVersion
		{
			get
			{
				return (Version)this.propertyBag[ToolInformationSchema.MinimumSupportedVersion];
			}
			private set
			{
				this.propertyBag[ToolInformationSchema.MinimumSupportedVersion] = value;
			}
		}

		public Version LatestVersion
		{
			get
			{
				return (Version)this.propertyBag[ToolInformationSchema.LatestVersion];
			}
			private set
			{
				this.propertyBag[ToolInformationSchema.LatestVersion] = value;
			}
		}

		public Uri UpdateInformationUrl
		{
			get
			{
				return (Uri)this.propertyBag[ToolInformationSchema.UpdateInformationUrl];
			}
			private set
			{
				this.propertyBag[ToolInformationSchema.UpdateInformationUrl] = value;
			}
		}

		private static ToolInformationSchema schema = ObjectSchema.GetInstance<ToolInformationSchema>();
	}
}
