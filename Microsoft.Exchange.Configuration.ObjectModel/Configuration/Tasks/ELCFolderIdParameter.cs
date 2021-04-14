using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public class ELCFolderIdParameter : ADIdParameter
	{
		public ELCFolderIdParameter(ADObjectId objectId) : base(objectId)
		{
		}

		public ELCFolderIdParameter(string identity) : base(identity)
		{
		}

		public ELCFolderIdParameter(ELCFolder elcFolder) : base(elcFolder.Id)
		{
		}

		public ELCFolderIdParameter()
		{
		}

		public ELCFolderIdParameter(INamedIdentity namedIdentity) : base(namedIdentity)
		{
		}

		public static ELCFolderIdParameter Parse(string rawString)
		{
			return new ELCFolderIdParameter(rawString);
		}
	}
}
