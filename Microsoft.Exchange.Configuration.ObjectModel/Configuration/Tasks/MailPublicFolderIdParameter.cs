using System;
using System.Collections.Generic;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage.Management;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public class MailPublicFolderIdParameter : RecipientIdParameter
	{
		internal OrganizationIdParameter Organization { get; private set; }

		public MailPublicFolderIdParameter()
		{
		}

		public MailPublicFolderIdParameter(string identity) : base(identity)
		{
			PublicFolderIdParameter publicFolderIdParameter = new PublicFolderIdParameter(identity, false);
			this.folderId = publicFolderIdParameter.PublicFolderId;
			if (publicFolderIdParameter.Organization != null && this.folderId != null)
			{
				this.Organization = publicFolderIdParameter.Organization;
			}
		}

		public MailPublicFolderIdParameter(ADObjectId adObjectId) : base(adObjectId)
		{
		}

		public MailPublicFolderIdParameter(ADObject adObject) : this(adObject.Id)
		{
			if (adObject.OrganizationId != null && adObject.OrganizationId.ConfigurationUnit != null)
			{
				this.Organization = new OrganizationIdParameter(adObject.OrganizationId.OrganizationalUnit);
			}
		}

		public MailPublicFolderIdParameter(PublicFolderId folderId)
		{
			this.folderId = folderId;
			if (folderId.OrganizationId != null)
			{
				this.Organization = new OrganizationIdParameter(folderId.OrganizationId.OrganizationalUnit);
			}
		}

		public MailPublicFolderIdParameter(PublicFolder folder) : this((PublicFolderId)folder.Identity)
		{
		}

		public MailPublicFolderIdParameter(INamedIdentity namedIdentity) : base(namedIdentity)
		{
		}

		internal override RecipientType[] RecipientTypes
		{
			get
			{
				return MailPublicFolderIdParameter.AllowedRecipientTypes;
			}
		}

		public new static MailPublicFolderIdParameter Parse(string identity)
		{
			if (identity == null)
			{
				throw new ArgumentNullException("identity");
			}
			if (identity.Length == 0)
			{
				throw new ArgumentException(Strings.ErrorEmptyParameter(typeof(MailPublicFolderIdParameter).ToString()), "identity");
			}
			return new MailPublicFolderIdParameter(identity);
		}

		public override string ToString()
		{
			if (this.folderId != null)
			{
				return this.folderId.ToString();
			}
			return base.ToString();
		}

		internal sealed override IEnumerable<T> GetObjects<T>(ADObjectId rootId, IDirectorySession session, IDirectorySession subTreeSession, OptionalIdentityData optionalData, out LocalizedString? notFoundReason)
		{
			notFoundReason = null;
			bool flag = false;
			if (base.InternalADObjectId == null && this.folderId != null)
			{
				IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(session.ConsistencyMode, session.SessionSettings, 205, "GetObjects", "f:\\15.00.1497\\sources\\dev\\Configuration\\src\\ObjectModel\\IdentityParameter\\RecipientParameters\\MailPublicFolderIdParameter.cs");
				using (PublicFolderDataProvider publicFolderDataProvider = new PublicFolderDataProvider(tenantOrTopologyConfigurationSession, "resolve-MailPublicFolderIdParameter", Guid.Empty))
				{
					PublicFolder publicFolder = (PublicFolder)publicFolderDataProvider.Read<PublicFolder>(this.folderId);
					if (publicFolder == null)
					{
						return new List<T>();
					}
					flag = true;
					if (!publicFolder.MailEnabled)
					{
						notFoundReason = new LocalizedString?(Strings.ErrorPublicFolderMailDisabled(this.folderId.ToString()));
						return new List<T>();
					}
					if (publicFolder.ProxyGuid == null)
					{
						notFoundReason = new LocalizedString?(Strings.ErrorPublicFolderGeneratingProxy(this.folderId.ToString()));
						return new List<T>();
					}
					this.Initialize(new ADObjectId(publicFolder.ProxyGuid));
				}
			}
			IEnumerable<T> objects = base.GetObjects<T>(rootId, session, subTreeSession, optionalData, out notFoundReason);
			EnumerableWrapper<T> wrapper = EnumerableWrapper<T>.GetWrapper(objects);
			if (!wrapper.HasElements() && flag)
			{
				notFoundReason = new LocalizedString?(Strings.ErrorPublicFolderMailDisabled(this.folderId.ToString()));
			}
			return wrapper;
		}

		protected override LocalizedString GetErrorMessageForWrongType(string id)
		{
			return Strings.WrongTypeMailPublicFolder(id);
		}

		internal new static readonly RecipientType[] AllowedRecipientTypes = new RecipientType[]
		{
			RecipientType.PublicFolder
		};

		private PublicFolderId folderId;
	}
}
