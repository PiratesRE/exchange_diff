using System;
using System.Management.Automation;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.SystemManager
{
	public static class PSCredentialExtension
	{
		public static bool IsLiveId(this PSCredential cred)
		{
			if (cred == null)
			{
				throw new ArgumentNullException("cred");
			}
			return SmtpAddress.IsValidSmtpAddress(cred.UserName);
		}
	}
}
