﻿using System;
using System.Runtime.InteropServices;

namespace System.Deployment.Internal.Isolation
{
	internal struct STORE_CATEGORY_SUBCATEGORY
	{
		[MarshalAs(UnmanagedType.LPWStr)]
		public string Subcategory;
	}
}
