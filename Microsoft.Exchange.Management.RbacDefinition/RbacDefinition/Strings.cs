using System;
using System.Reflection;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.RbacDefinition
{
	internal static class Strings
	{
		public static LocalizedString ExOrgReadAdminSGroupNotFoundException(Guid guid)
		{
			return new LocalizedString("ExOrgReadAdminSGroupNotFoundException", Strings.ResourceManager, new object[]
			{
				guid
			});
		}

		public static LocalizedString ExOrgAdminSGroupNotFoundException(Guid guid)
		{
			return new LocalizedString("ExOrgAdminSGroupNotFoundException", Strings.ResourceManager, new object[]
			{
				guid
			});
		}

		public static LocalizedString ExPublicFolderAdminSGroupNotFoundException(Guid guid)
		{
			return new LocalizedString("ExPublicFolderAdminSGroupNotFoundException", Strings.ResourceManager, new object[]
			{
				guid
			});
		}

		public static LocalizedString ExMailboxAdminSGroupNotFoundException(Guid guid)
		{
			return new LocalizedString("ExMailboxAdminSGroupNotFoundException", Strings.ResourceManager, new object[]
			{
				guid
			});
		}

		public static LocalizedString ExRbacRoleGroupNotFoundException(Guid guid, string groupName)
		{
			return new LocalizedString("ExRbacRoleGroupNotFoundException", Strings.ResourceManager, new object[]
			{
				guid,
				groupName
			});
		}

		private static ExchangeResourceManager ResourceManager = ExchangeResourceManager.GetResourceManager("Microsoft.Exchange.Management.RbacDefinition.Strings", typeof(Strings).GetTypeInfo().Assembly);

		private enum ParamIDs
		{
			ExOrgReadAdminSGroupNotFoundException,
			ExOrgAdminSGroupNotFoundException,
			ExPublicFolderAdminSGroupNotFoundException,
			ExMailboxAdminSGroupNotFoundException,
			ExRbacRoleGroupNotFoundException
		}
	}
}
