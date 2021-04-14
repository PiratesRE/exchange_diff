using System;

namespace Microsoft.Exchange.UM.PersonalAutoAttendant
{
	internal class TransferToVoicemail : KeyMappingBase
	{
		internal TransferToVoicemail(string context) : base(KeyMappingTypeEnum.TransferToVoicemail, 10, context)
		{
		}

		public override bool Validate(IDataValidator validator)
		{
			return true;
		}
	}
}
