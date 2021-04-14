using System;
using System.DirectoryServices;
using System.DirectoryServices.ActiveDirectory;
using System.Net;

namespace Microsoft.Exchange.Management.Deployment.XforestTenantMigration
{
	internal sealed class DirectoryBindingInfo
	{
		public string SchemaNamingContextDN
		{
			get
			{
				return this.ldapPathToSchema;
			}
		}

		public NetworkCredential Credential { get; set; }

		public string DefaultNamingContextDN
		{
			get
			{
				return this.ldapPathToDefault;
			}
		}

		public string ConfigurationNamingContextDN
		{
			get
			{
				return this.ldapPathToConfiguration;
			}
		}

		public string LdapBasePath
		{
			get
			{
				return "LDAP://";
			}
		}

		public DirectoryBindingInfo()
		{
			this.ldapPathToSchema = this.GetRootDseProperty("schemaNamingContext");
			this.ldapPathToConfiguration = this.GetRootDseProperty("configurationNamingContext");
			this.ldapPathToDefault = this.GetRootDseProperty("defaultNamingContext");
		}

		public DirectoryBindingInfo(NetworkCredential credential)
		{
			this.Credential = credential;
			this.ldapPathToSchema = this.GetRootDseProperty("schemaNamingContext");
			this.ldapPathToConfiguration = this.GetRootDseProperty("configurationNamingContext");
			this.ldapPathToDefault = this.GetRootDseProperty("defaultNamingContext");
		}

		private string GetRootDseProperty(string name)
		{
			string result;
			using (DirectoryEntry rootDse = this.GetRootDse())
			{
				result = rootDse.Properties[name][0].ToString();
			}
			return result;
		}

		private DirectoryEntry GetRootDse()
		{
			string ldapPath;
			if (this.Credential != null && this.Credential.Domain != null)
			{
				ldapPath = string.Format("{0}{1}/RootDSE", this.LdapBasePath, this.Credential.Domain);
			}
			else
			{
				ldapPath = string.Format("{0}RootDSE", this.LdapBasePath);
			}
			return this.GetDirectoryEntry(ldapPath);
		}

		public DirectoryEntry GetDirectoryEntry(string ldapPath)
		{
			if (this.Credential != null && this.Credential.UserName != null && this.Credential.Password != null)
			{
				return new DirectoryEntry(ldapPath, (this.Credential.Domain == null) ? this.Credential.UserName : (this.Credential.Domain + "\\" + this.Credential.UserName), this.Credential.Password);
			}
			return new DirectoryEntry(ldapPath);
		}

		public DirectoryContext GetDirectoryContext(DirectoryContextType contextType)
		{
			if (this.Credential != null && this.Credential.UserName != null && this.Credential.Password != null)
			{
				return new DirectoryContext(contextType, this.Credential.Domain, this.Credential.UserName, this.Credential.Password);
			}
			return new DirectoryContext(contextType);
		}

		private readonly string ldapPathToSchema;

		private readonly string ldapPathToDefault;

		private readonly string ldapPathToConfiguration;
	}
}
