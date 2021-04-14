using System;
using System.Management.Automation;
using System.Security;
using Microsoft.Exchange.Extensions;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal static class PSCredentialHelper
	{
		public static PSCredential GetCredentialFromUserPass(string userName, string password, bool passwordEncryptionEnabled)
		{
			if (passwordEncryptionEnabled)
			{
				throw new NotImplementedException("Password encryption not yet implemented");
			}
			if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(password))
			{
				SecureString secureString = new SecureString();
				foreach (char c in password.ToCharArray())
				{
					secureString.AppendChar(c);
				}
				return new PSCredential(userName, secureString);
			}
			return null;
		}

		public static void GetUserPassFromCredential(PSCredential credential, out string userName, out string password, bool passwordEncryptionEnabled)
		{
			userName = null;
			password = null;
			if (passwordEncryptionEnabled)
			{
				throw new NotImplementedException("Password encryption not yet implemented");
			}
			if (credential == null)
			{
				return;
			}
			string text = string.Empty;
			if (credential.Password == null || credential.Password.Length == 0)
			{
				return;
			}
			text = credential.Password.ConvertToUnsecureString();
			userName = credential.UserName;
			password = text;
		}
	}
}
