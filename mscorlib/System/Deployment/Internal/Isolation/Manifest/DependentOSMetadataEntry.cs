﻿using System;
using System.Runtime.InteropServices;

namespace System.Deployment.Internal.Isolation.Manifest
{
	[StructLayout(LayoutKind.Sequential)]
	internal class DependentOSMetadataEntry
	{
		[MarshalAs(UnmanagedType.LPWStr)]
		public string SupportUrl;

		[MarshalAs(UnmanagedType.LPWStr)]
		public string Description;

		public ushort MajorVersion;

		public ushort MinorVersion;

		public ushort BuildNumber;

		public byte ServicePackMajor;

		public byte ServicePackMinor;
	}
}
