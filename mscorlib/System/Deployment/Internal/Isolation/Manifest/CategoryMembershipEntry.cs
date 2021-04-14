using System;
using System.Runtime.InteropServices;

namespace System.Deployment.Internal.Isolation.Manifest
{
	[StructLayout(LayoutKind.Sequential)]
	internal class CategoryMembershipEntry
	{
		public IDefinitionIdentity Identity;

		public ISection SubcategoryMembership;
	}
}
