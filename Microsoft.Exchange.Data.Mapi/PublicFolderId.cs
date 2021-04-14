using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Mapi.Common;

namespace Microsoft.Exchange.Data.Mapi
{
	[Serializable]
	public sealed class PublicFolderId : FolderId
	{
		public static PublicFolderId Parse(string input)
		{
			if (string.IsNullOrEmpty(input))
			{
				throw new FormatException(Strings.ExceptionFormatNotSupported);
			}
			try
			{
				return new PublicFolderId(MapiFolderPath.Parse(input));
			}
			catch (FormatException)
			{
			}
			try
			{
				return new PublicFolderId(MapiEntryId.Parse(input));
			}
			catch (FormatException)
			{
			}
			return new PublicFolderId(input);
		}

		public PublicFolderId()
		{
		}

		public PublicFolderId(byte[] bytes) : base(bytes)
		{
		}

		public PublicFolderId(string legacyDn) : base(new PublicStoreId(), legacyDn)
		{
		}

		public PublicFolderId(MapiEntryId entryId) : base(new PublicStoreId(), entryId)
		{
		}

		public PublicFolderId(MapiFolderPath folderPath) : base(new PublicStoreId(), folderPath)
		{
		}

		internal PublicFolderId(MapiEntryId entryId, MapiFolderPath folderPath, string legacyDn) : base(new PublicStoreId(), entryId, folderPath, legacyDn)
		{
		}

		public override string ToString()
		{
			string str = (this.organizationId == null) ? string.Empty : (this.organizationId.OrganizationalUnit.Name + '\\'.ToString());
			if (null != base.MapiFolderPath)
			{
				return str + base.MapiFolderPath.ToString();
			}
			if (null != base.MapiEntryId)
			{
				return str + base.MapiEntryId.ToString();
			}
			if (!string.IsNullOrEmpty(base.LegacyDistinguishedName))
			{
				return str + base.LegacyDistinguishedName;
			}
			return null;
		}

		internal OrganizationId OrganizationId
		{
			get
			{
				return this.organizationId;
			}
			set
			{
				this.organizationId = value;
			}
		}

		private OrganizationId organizationId;
	}
}
