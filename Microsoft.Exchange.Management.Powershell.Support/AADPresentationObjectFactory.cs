using System;
using Microsoft.WindowsAzure.ActiveDirectory;

namespace Microsoft.Exchange.Management.Powershell.Support
{
	[Serializable]
	public sealed class AADPresentationObjectFactory
	{
		public static AADDirectoryObjectPresentationObject Create(DirectoryObject directoryObject)
		{
			Group group = directoryObject as Group;
			if (group != null)
			{
				return new AADGroupPresentationObject(group);
			}
			User user = directoryObject as User;
			if (user != null)
			{
				return new AADUserPresentationObject(user);
			}
			return new AADDirectoryObjectPresentationObject(directoryObject);
		}
	}
}
