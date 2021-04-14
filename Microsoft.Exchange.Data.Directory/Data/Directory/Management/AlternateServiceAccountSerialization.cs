using System;
using System.Linq;
using System.Management.Automation;
using System.Security;

namespace Microsoft.Exchange.Data.Directory.Management
{
	public static class AlternateServiceAccountSerialization
	{
		public static SecureString[] GetSerializationData_AlternateServiceAccountConfiguration_AllCredentials_Password(PSObject clientAccessServerObject)
		{
			ClientAccessServer clientAccessServer = clientAccessServerObject.BaseObject as ClientAccessServer;
			if (clientAccessServer == null || clientAccessServer.AlternateServiceAccountConfiguration == null)
			{
				return null;
			}
			return (from credential in clientAccessServer.AlternateServiceAccountConfiguration.AllCredentials
			select credential.Password).ToArray<SecureString>();
		}
	}
}
