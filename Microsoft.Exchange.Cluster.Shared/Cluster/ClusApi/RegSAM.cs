using System;

namespace Microsoft.Exchange.Cluster.ClusApi
{
	internal enum RegSAM : uint
	{
		None,
		QueryValue,
		SetValue,
		CreateSubKey = 4U,
		EnumerateSubKeys = 8U,
		Notify = 16U,
		CreateLink = 32U,
		WOW64_32Key = 512U,
		WOW64_64Key = 256U,
		WOW64_Res = 768U,
		Read = 131097U,
		Write = 131078U,
		Execute = 131097U,
		AllAccess = 983103U
	}
}
