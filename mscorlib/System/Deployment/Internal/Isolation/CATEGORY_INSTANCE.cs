using System;
using System.Runtime.InteropServices;

namespace System.Deployment.Internal.Isolation
{
	internal struct CATEGORY_INSTANCE
	{
		public IDefinitionAppId DefinitionAppId_Application;

		[MarshalAs(UnmanagedType.LPWStr)]
		public string XMLSnippet;
	}
}
