using System;
using System.Diagnostics;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ObjectScope(new ConfigScopes[]
	{
		ConfigScopes.TenantLocal,
		ConfigScopes.TenantSubTree
	})]
	[Serializable]
	public class RbacContainer : Container
	{
		internal void StampExchangeObjectVersion(FileVersionInfo managementDllVersion)
		{
			ExchangeObjectVersion version = new ExchangeObjectVersion(base.ExchangeVersion.Major, base.ExchangeVersion.Minor, (byte)managementDllVersion.FileMajorPart, (byte)managementDllVersion.FileMinorPart, (ushort)managementDllVersion.FileBuildPart, (ushort)managementDllVersion.FilePrivatePart);
			this.StampExchangeObjectVersion(version);
		}

		internal void StampExchangeObjectVersion(ExchangeObjectVersion version)
		{
			base.SetExchangeVersion(version);
			this.MinAdminVersion = new int?(base.ExchangeVersion.ExchangeBuild.ToExchange2003FormatInt32());
		}

		internal const string RdnString = "CN=RBAC";

		internal static readonly ExchangeBuild InitialRBACBuild = ExchangeObjectVersion.Exchange2010.ExchangeBuild;

		internal static readonly ExchangeBuild E14RTMBuild = new ExchangeBuild(14, 0, 639, 20);

		internal static readonly ExchangeBuild FirstRGRABuild = new ExchangeBuild(14, 0, 582, 0);
	}
}
