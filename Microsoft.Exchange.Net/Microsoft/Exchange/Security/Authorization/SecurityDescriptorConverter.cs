using System;
using System.DirectoryServices;
using System.Security.AccessControl;

namespace Microsoft.Exchange.Security.Authorization
{
	public static class SecurityDescriptorConverter
	{
		public static ActiveDirectorySecurity ConvertToActiveDirectorySecurity(RawSecurityDescriptor rawSd)
		{
			if (rawSd == null)
			{
				throw new ArgumentNullException("RawSecurityDescriptor");
			}
			ActiveDirectorySecurity activeDirectorySecurity = new ActiveDirectorySecurity();
			byte[] array = new byte[rawSd.BinaryLength];
			rawSd.GetBinaryForm(array, 0);
			activeDirectorySecurity.SetSecurityDescriptorBinaryForm(array);
			return activeDirectorySecurity;
		}

		public static RawSecurityDescriptor ConvertToRawSecurityDescriptor(ActiveDirectorySecurity activeDirectorySecurity)
		{
			if (activeDirectorySecurity == null)
			{
				throw new ArgumentNullException("ActiveDirectorySecurity");
			}
			return new RawSecurityDescriptor(activeDirectorySecurity.GetSecurityDescriptorBinaryForm(), 0);
		}
	}
}
