using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public class ELCContentSettingsIdParameter : ADIdParameter
	{
		public ELCContentSettingsIdParameter(ADObjectId objectId) : base(objectId)
		{
		}

		public ELCContentSettingsIdParameter(string identity) : base(identity)
		{
		}

		public ELCContentSettingsIdParameter(ElcContentSettings elcContentSetting) : base(elcContentSetting.Id)
		{
		}

		public ELCContentSettingsIdParameter()
		{
		}

		public ELCContentSettingsIdParameter(INamedIdentity namedIdentity) : base(namedIdentity)
		{
		}

		public static ELCContentSettingsIdParameter Parse(string rawString)
		{
			return new ELCContentSettingsIdParameter(rawString);
		}
	}
}
