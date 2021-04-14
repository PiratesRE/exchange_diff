using System;
using System.Runtime.InteropServices;

namespace System.Deployment.Internal.Isolation.Manifest
{
	[StructLayout(LayoutKind.Sequential)]
	internal class SubcategoryMembershipEntry
	{
		[MarshalAs(UnmanagedType.LPWStr)]
		public string Subcategory;

		public ISection CategoryMembershipData;
	}
}
