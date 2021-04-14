using System;

namespace Microsoft.Exchange.UM.UMCore.OCS
{
	internal delegate bool TryParseTargetUserDelegate(string target, out string userPart, out string hostPart, out bool isPhoneNumber);
}
