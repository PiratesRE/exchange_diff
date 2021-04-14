using System;
using System.Collections.Generic;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public class PublicFolderIdParameter : IIdentityParameter
	{
		internal PublicFolderId PublicFolderId { get; set; }

		internal OrganizationIdParameter Organization { get; private set; }

		internal PublicFolderIdParameter(string publicFolderIdString, bool isEntryId)
		{
			if (string.IsNullOrEmpty(publicFolderIdString))
			{
				throw new ArgumentNullException("publicFolderIdString");
			}
			this.rawIdentity = publicFolderIdString;
			PublicFolderId publicFolderId = null;
			try
			{
				int num = publicFolderIdString.IndexOf('\\');
				if (num < 0)
				{
					if (isEntryId)
					{
						publicFolderId = new PublicFolderId(StoreObjectId.FromHexEntryId(publicFolderIdString));
					}
				}
				else if (num == 0)
				{
					if (publicFolderIdString.Length > 1 && publicFolderIdString[num + 1] == '\\')
					{
						throw new FormatException(Strings.ErrorIncompletePublicFolderIdParameter(publicFolderIdString));
					}
					publicFolderId = new PublicFolderId(MapiFolderPath.Parse(publicFolderIdString));
				}
				else
				{
					if (!MapiTaskHelper.IsDatacenter)
					{
						throw new FormatException(Strings.ErrorIncompletePublicFolderIdParameter(publicFolderIdString));
					}
					if (num == publicFolderIdString.Length - 1)
					{
						throw new FormatException(Strings.ErrorIncompleteDCPublicFolderIdParameter(publicFolderIdString));
					}
					this.Organization = new OrganizationIdParameter(publicFolderIdString.Substring(0, num));
					if (publicFolderIdString[num + 1] == '\\')
					{
						publicFolderId = new PublicFolderId(MapiFolderPath.Parse(publicFolderIdString.Substring(num + 1)));
					}
					else if (isEntryId)
					{
						publicFolderId = new PublicFolderId(StoreObjectId.FromHexEntryId(publicFolderIdString.Substring(num + 1)));
					}
				}
			}
			catch (FormatException innerException)
			{
				throw new FormatException(MapiTaskHelper.IsDatacenter ? Strings.ErrorIncompleteDCPublicFolderIdParameter(this.rawIdentity) : Strings.ErrorIncompletePublicFolderIdParameter(this.rawIdentity), innerException);
			}
			catch (CorruptDataException innerException2)
			{
				throw new FormatException(MapiTaskHelper.IsDatacenter ? Strings.ErrorIncompleteDCPublicFolderIdParameter(this.rawIdentity) : Strings.ErrorIncompletePublicFolderIdParameter(this.rawIdentity), innerException2);
			}
			if (publicFolderId != null)
			{
				((IIdentityParameter)this).Initialize(publicFolderId);
			}
		}

		public PublicFolderIdParameter()
		{
		}

		public PublicFolderIdParameter(PublicFolder publicFolder)
		{
			if (publicFolder == null)
			{
				throw new ArgumentNullException("publicFolder");
			}
			if (publicFolder.Identity == null)
			{
				throw new ArgumentNullException("publicFolder.Identity");
			}
			this.rawIdentity = publicFolder.Identity.ToString();
			((IIdentityParameter)this).Initialize(publicFolder.Identity);
			if (publicFolder.OrganizationId != null && publicFolder.OrganizationId.OrganizationalUnit != null)
			{
				this.Organization = new OrganizationIdParameter(publicFolder.OrganizationId.OrganizationalUnit);
			}
		}

		public PublicFolderIdParameter(PublicFolderId publicFolderId)
		{
			if (publicFolderId == null)
			{
				throw new ArgumentNullException("publicFolderId");
			}
			this.rawIdentity = publicFolderId.ToString();
			((IIdentityParameter)this).Initialize(publicFolderId);
		}

		public PublicFolderIdParameter(string publicFolderIdString) : this(publicFolderIdString, true)
		{
		}

		public PublicFolderIdParameter(INamedIdentity namedIdentity) : this(namedIdentity.Identity)
		{
			this.rawIdentity = namedIdentity.DisplayName;
		}

		IEnumerable<T> IIdentityParameter.GetObjects<T>(ObjectId rootId, IConfigDataProvider session)
		{
			LocalizedString? localizedString;
			return ((IIdentityParameter)this).GetObjects<T>(rootId, session, null, out localizedString);
		}

		IEnumerable<T> IIdentityParameter.GetObjects<T>(ObjectId rootId, IConfigDataProvider session, OptionalIdentityData optionalData, out LocalizedString? notFoundReason)
		{
			if (session == null)
			{
				throw new ArgumentNullException("session");
			}
			if (this.PublicFolderId == null)
			{
				throw new InvalidOperationException(Strings.ErrorOperationOnInvalidObject);
			}
			if (!(session is PublicFolderDataProvider) && !(session is PublicFolderStatisticsDataProvider))
			{
				throw new NotSupportedException("session: " + session.GetType().FullName);
			}
			if (optionalData != null && optionalData.AdditionalFilter != null)
			{
				throw new NotSupportedException("Supplying Additional Filters is not currently supported by this IdParameter.");
			}
			T t = (T)((object)session.Read<T>(this.PublicFolderId));
			if (t == null)
			{
				notFoundReason = new LocalizedString?(Strings.ErrorManagementObjectNotFound(this.ToString()));
				return new T[0];
			}
			notFoundReason = null;
			return new T[]
			{
				t
			};
		}

		public void Initialize(ObjectId objectId)
		{
			if (objectId == null)
			{
				throw new ArgumentNullException("objectId");
			}
			if (!(objectId is PublicFolderId))
			{
				throw new NotSupportedException("objectId: " + objectId.GetType().FullName);
			}
			this.PublicFolderId = (PublicFolderId)objectId;
			if (this.PublicFolderId.OrganizationId != null)
			{
				this.Organization = new OrganizationIdParameter(this.PublicFolderId.OrganizationId.OrganizationalUnit);
			}
		}

		string IIdentityParameter.RawIdentity
		{
			get
			{
				return this.rawIdentity;
			}
		}

		public override string ToString()
		{
			return this.rawIdentity;
		}

		private readonly string rawIdentity;
	}
}
