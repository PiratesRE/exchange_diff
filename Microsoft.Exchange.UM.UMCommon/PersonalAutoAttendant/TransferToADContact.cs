using System;

namespace Microsoft.Exchange.UM.PersonalAutoAttendant
{
	internal abstract class TransferToADContact : KeyMapping<string>
	{
		internal TransferToADContact(KeyMappingTypeEnum keyMappingType, int key, string context, string legacyExchangeDN) : base(keyMappingType, key, context, legacyExchangeDN)
		{
		}

		internal string LegacyExchangeDN
		{
			get
			{
				return base.Data;
			}
		}

		public override bool Validate(IDataValidator validator)
		{
			throw new NotImplementedException();
		}
	}
}
