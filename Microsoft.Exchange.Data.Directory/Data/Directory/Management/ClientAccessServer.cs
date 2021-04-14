using System;
using System.Collections;
using System.Linq;
using System.Management.Automation;
using System.Runtime.Serialization;
using System.Security;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Data.Directory.Management
{
	[Serializable]
	public class ClientAccessServer : ADPresentationObject, IDeserializationCallback
	{
		internal override ADPresentationSchema PresentationSchema
		{
			get
			{
				return ClientAccessServer.schema;
			}
		}

		public ClientAccessServer()
		{
		}

		public ClientAccessServer(Server dataObject) : base(dataObject)
		{
		}

		public new string Name
		{
			get
			{
				return (string)this[ADObjectSchema.Name];
			}
		}

		public string Fqdn
		{
			get
			{
				return (string)this[ServerSchema.Fqdn];
			}
		}

		public ADObjectId ClientAccessArray
		{
			get
			{
				return (ADObjectId)this[ClientAccessServerSchema.ClientAccessArray];
			}
			internal set
			{
				this[ClientAccessServerSchema.ClientAccessArray] = value;
			}
		}

		public bool? OutlookAnywhereEnabled
		{
			get
			{
				return this.isRpcHttpEnabled;
			}
			internal set
			{
				this.isRpcHttpEnabled = value;
			}
		}

		public Fqdn AutoDiscoverServiceCN
		{
			get
			{
				return this.scpAutoDiscoverServiceCN;
			}
			internal set
			{
				this.scpAutoDiscoverServiceCN = value;
			}
		}

		public string AutoDiscoverServiceClassName
		{
			get
			{
				return this.scpAutoDiscoverServiceClassName;
			}
			internal set
			{
				this.scpAutoDiscoverServiceClassName = value;
			}
		}

		public Uri AutoDiscoverServiceInternalUri
		{
			get
			{
				return this.scpAutoDiscoverServiceInternalUri;
			}
			set
			{
				this.scpAutoDiscoverServiceInternalUri = value;
			}
		}

		public Guid? AutoDiscoverServiceGuid
		{
			get
			{
				return this.scpAutoDiscoverServiceGuid;
			}
			internal set
			{
				this.scpAutoDiscoverServiceGuid = value;
			}
		}

		public MultiValuedProperty<string> AutoDiscoverSiteScope
		{
			get
			{
				return this.scpAutoDiscoverSiteScope;
			}
			set
			{
				this.scpAutoDiscoverSiteScope = value;
			}
		}

		public AlternateServiceAccountConfiguration AlternateServiceAccountConfiguration { get; internal set; }

		[Parameter(Mandatory = false)]
		public bool IsOutOfService
		{
			get
			{
				return (bool)this[ActiveDirectoryServerSchema.IsOutOfService];
			}
			set
			{
				this[ActiveDirectoryServerSchema.IsOutOfService] = value;
			}
		}

		void IDeserializationCallback.OnDeserialization(object sender)
		{
			PSObject psobject = sender as PSObject;
			if (psobject != null && this.AlternateServiceAccountConfiguration != null)
			{
				PSObject psobject2 = (PSObject)psobject.Properties["_AlternateServiceAccountConfiguration_AllCredentials_Password"].Value;
				if (psobject2 != null)
				{
					this.AlternateServiceAccountConfiguration.ApplyPasswords(((IEnumerable)psobject2.BaseObject).Cast<SecureString>().ToArray<SecureString>());
				}
			}
		}

		private static ClientAccessServerSchema schema = ObjectSchema.GetInstance<ClientAccessServerSchema>();

		private bool? isRpcHttpEnabled;

		private Fqdn scpAutoDiscoverServiceCN;

		private string scpAutoDiscoverServiceClassName;

		private Uri scpAutoDiscoverServiceInternalUri;

		private Guid? scpAutoDiscoverServiceGuid;

		private MultiValuedProperty<string> scpAutoDiscoverSiteScope;
	}
}
