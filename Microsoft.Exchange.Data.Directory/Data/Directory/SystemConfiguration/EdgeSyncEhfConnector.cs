using System;
using System.Management.Automation;
using System.Security;
using Microsoft.Exchange.Extensions;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Serializable]
	public class EdgeSyncEhfConnector : EdgeSyncConnector
	{
		[Parameter(Mandatory = false)]
		public Uri ProvisioningUrl
		{
			get
			{
				return (Uri)this.propertyBag[EdgeSyncEhfConnector.EdgeSyncEhfConnectorSchema.ProvisioningUrl];
			}
			set
			{
				this.propertyBag[EdgeSyncEhfConnector.EdgeSyncEhfConnectorSchema.ProvisioningUrl] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string PrimaryLeaseLocation
		{
			get
			{
				return (string)this.propertyBag[EdgeSyncEhfConnector.EdgeSyncEhfConnectorSchema.PrimaryLeaseLocation];
			}
			set
			{
				this.propertyBag[EdgeSyncEhfConnector.EdgeSyncEhfConnectorSchema.PrimaryLeaseLocation] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string BackupLeaseLocation
		{
			get
			{
				return (string)this.propertyBag[EdgeSyncEhfConnector.EdgeSyncEhfConnectorSchema.BackupLeaseLocation];
			}
			set
			{
				this.propertyBag[EdgeSyncEhfConnector.EdgeSyncEhfConnectorSchema.BackupLeaseLocation] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public PSCredential AuthenticationCredential
		{
			get
			{
				return (PSCredential)this.propertyBag[EdgeSyncEhfConnector.EdgeSyncEhfConnectorSchema.AuthenticationCredential];
			}
			set
			{
				this.propertyBag[EdgeSyncEhfConnector.EdgeSyncEhfConnectorSchema.AuthenticationCredential] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string ResellerId
		{
			get
			{
				return (string)this.propertyBag[EdgeSyncEhfConnector.EdgeSyncEhfConnectorSchema.ResellerId];
			}
			set
			{
				this.propertyBag[EdgeSyncEhfConnector.EdgeSyncEhfConnectorSchema.ResellerId] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool AdminSyncEnabled
		{
			get
			{
				return (bool)this[EdgeSyncEhfConnector.EdgeSyncEhfConnectorSchema.AdminSyncEnabled];
			}
			set
			{
				this[EdgeSyncEhfConnector.EdgeSyncEhfConnectorSchema.AdminSyncEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int Version
		{
			get
			{
				return (int)this[EdgeSyncEhfConnector.EdgeSyncEhfConnectorSchema.Version];
			}
			set
			{
				this[EdgeSyncEhfConnector.EdgeSyncEhfConnectorSchema.Version] = value;
			}
		}

		internal override ADObjectSchema Schema
		{
			get
			{
				return EdgeSyncEhfConnector.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return "msExchEdgeSyncEhfConnector";
			}
		}

		internal override QueryFilter ImplicitFilter
		{
			get
			{
				return new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.ObjectCategory, this.MostDerivedObjectClass);
			}
		}

		internal static object AuthenticationCredentialGetter(IPropertyBag propertyBag)
		{
			string text = (string)propertyBag[EdgeSyncEhfConnector.EdgeSyncEhfConnectorSchema.UserName];
			string text2 = (string)propertyBag[EdgeSyncEhfConnector.EdgeSyncEhfConnectorSchema.Password];
			if (!string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(text2))
			{
				SecureString password = text2.ConvertToSecureString();
				return new PSCredential(text, password);
			}
			return null;
		}

		internal static void AuthenticationCredentialSetter(object value, IPropertyBag propertyBag)
		{
			PSCredential pscredential = value as PSCredential;
			if (pscredential == null)
			{
				propertyBag[EdgeSyncEhfConnector.EdgeSyncEhfConnectorSchema.UserName] = null;
				propertyBag[EdgeSyncEhfConnector.EdgeSyncEhfConnectorSchema.Password] = null;
				return;
			}
			string empty = string.Empty;
			if (pscredential.Password == null || pscredential.Password.Length == 0)
			{
				return;
			}
			string value2 = pscredential.Password.ConvertToUnsecureString();
			propertyBag[EdgeSyncEhfConnector.EdgeSyncEhfConnectorSchema.UserName] = pscredential.UserName;
			propertyBag[EdgeSyncEhfConnector.EdgeSyncEhfConnectorSchema.Password] = value2;
		}

		internal void SetIdentity(ObjectId id)
		{
			this[this.propertyBag.ObjectIdentityPropertyDefinition] = id;
		}

		internal const string MostDerivedClass = "msExchEdgeSyncEhfConnector";

		private static EdgeSyncEhfConnector.EdgeSyncEhfConnectorSchema schema = ObjectSchema.GetInstance<EdgeSyncEhfConnector.EdgeSyncEhfConnectorSchema>();

		[Flags]
		internal enum EdgeSyncEhfConnectorFlags
		{
			None = 0,
			AdminSyncEnabled = 1,
			All = 1
		}

		internal class EdgeSyncEhfConnectorSchema : EdgeSyncConnectorSchema
		{
			public static readonly ADPropertyDefinition ProvisioningUrl = new ADPropertyDefinition("ProvisioningUrl", ExchangeObjectVersion.Exchange2007, typeof(Uri), "msExchEdgeSyncEHFProvisioningUrl", ADPropertyDefinitionFlags.None, null, new PropertyDefinitionConstraint[]
			{
				new UriKindConstraint(UriKind.Absolute)
			}, new PropertyDefinitionConstraint[]
			{
				new UriKindConstraint(UriKind.Absolute)
			}, null, null);

			public static readonly ADPropertyDefinition PrimaryLeaseLocation = new ADPropertyDefinition("PrimaryLeaseLocation", ExchangeObjectVersion.Exchange2007, typeof(string), "msExchEdgeSyncEHFPrimaryLeaseLocation", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

			public static readonly ADPropertyDefinition BackupLeaseLocation = new ADPropertyDefinition("BackupLeaseLocation", ExchangeObjectVersion.Exchange2007, typeof(string), "msExchEdgeSyncEHFBackupLeaseLocation", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

			public static readonly ADPropertyDefinition UserName = new ADPropertyDefinition("UserName", ExchangeObjectVersion.Exchange2007, typeof(string), "msExchEdgeSyncEHFUserName", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

			public static readonly ADPropertyDefinition Password = new ADPropertyDefinition("Password", ExchangeObjectVersion.Exchange2007, typeof(string), "msExchEdgeSyncEHFPassword", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

			public static readonly ADPropertyDefinition ResellerId = new ADPropertyDefinition("ResellerId", ExchangeObjectVersion.Exchange2007, typeof(string), "msExchEdgeSyncEHFResellerID", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
			{
				new Int32ParsableStringConstraint()
			}, null, null);

			public static readonly ADPropertyDefinition AuthenticationCredential = new ADPropertyDefinition("AuthenticationCredential", ExchangeObjectVersion.Exchange2007, typeof(PSCredential), null, ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
			{
				EdgeSyncEhfConnector.EdgeSyncEhfConnectorSchema.UserName,
				EdgeSyncEhfConnector.EdgeSyncEhfConnectorSchema.Password
			}, null, new GetterDelegate(EdgeSyncEhfConnector.AuthenticationCredentialGetter), new SetterDelegate(EdgeSyncEhfConnector.AuthenticationCredentialSetter), null, null);

			public static readonly ADPropertyDefinition Flags = new ADPropertyDefinition("EdgeSyncEhfConnectorFlags", ExchangeObjectVersion.Exchange2007, typeof(int), "msExchEdgeSyncEHFFlags", ADPropertyDefinitionFlags.PersistDefaultValue, 0, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

			public static readonly ADPropertyDefinition AdminSyncEnabled = new ADPropertyDefinition("AdminSyncEnabled", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
			{
				EdgeSyncEhfConnector.EdgeSyncEhfConnectorSchema.Flags
			}, null, ADObject.FlagGetterDelegate(EdgeSyncEhfConnector.EdgeSyncEhfConnectorSchema.Flags, 1), ADObject.FlagSetterDelegate(EdgeSyncEhfConnector.EdgeSyncEhfConnectorSchema.Flags, 1), null, null);

			public static readonly ADPropertyDefinition Version = new ADPropertyDefinition("Version", ExchangeObjectVersion.Exchange2007, typeof(int), "msExchEdgeSyncConnectorVersion", ADPropertyDefinitionFlags.PersistDefaultValue, 1, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);
		}
	}
}
