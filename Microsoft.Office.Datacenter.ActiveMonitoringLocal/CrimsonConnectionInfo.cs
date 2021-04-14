using System;
using System.Security;

namespace Microsoft.Office.Datacenter.ActiveMonitoring
{
	internal class CrimsonConnectionInfo
	{
		internal CrimsonConnectionInfo(string computerName)
		{
			this.ComputerName = computerName;
		}

		internal CrimsonConnectionInfo(string computerName, string userDomain, string userName, SecureString password)
		{
			this.ComputerName = computerName;
			this.UserDomain = userDomain;
			this.UserName = userName;
			this.Password = password;
		}

		internal string ComputerName { get; private set; }

		internal string UserDomain { get; private set; }

		internal string UserName { get; private set; }

		internal SecureString Password { get; private set; }
	}
}
