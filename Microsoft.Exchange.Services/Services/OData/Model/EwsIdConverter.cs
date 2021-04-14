using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal static class EwsIdConverter
	{
		public static string EwsIdToODataId(string ewsId)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("ewsId", ewsId);
			return ewsId.Replace('/', '-').Replace('+', '_');
		}

		public static string ODataIdToEwsId(string odataId)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("odataId", odataId);
			return odataId.Replace('-', '/').Replace('_', '+');
		}

		public static BaseFolderId CreateFolderIdFromEwsId(string id)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("id", id);
			DistinguishedFolderIdName id2;
			BaseFolderId result;
			if (Enum.TryParse<DistinguishedFolderIdName>(id, true, out id2))
			{
				result = new DistinguishedFolderId
				{
					Id = id2
				};
			}
			else if (string.Equals(EwsIdConverter.RootFolderName, id, StringComparison.OrdinalIgnoreCase))
			{
				result = new DistinguishedFolderId
				{
					Id = DistinguishedFolderIdName.msgfolderroot
				};
			}
			else
			{
				result = new FolderId
				{
					Id = id
				};
			}
			return result;
		}

		public static string RootFolderName = "RootFolder";
	}
}
