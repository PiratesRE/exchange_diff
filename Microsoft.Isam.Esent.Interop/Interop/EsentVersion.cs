using System;
using Microsoft.Isam.Esent.Interop.Implementation;

namespace Microsoft.Isam.Esent.Interop
{
	public static class EsentVersion
	{
		public static bool SupportsServer2003Features
		{
			get
			{
				return EsentVersion.Capabilities.SupportsServer2003Features;
			}
		}

		public static bool SupportsVistaFeatures
		{
			get
			{
				return EsentVersion.Capabilities.SupportsVistaFeatures;
			}
		}

		public static bool SupportsWindows7Features
		{
			get
			{
				return EsentVersion.Capabilities.SupportsWindows7Features;
			}
		}

		public static bool SupportsWindows8Features
		{
			get
			{
				return EsentVersion.Capabilities.SupportsWindows8Features;
			}
		}

		public static bool SupportsWindows81Features
		{
			get
			{
				return EsentVersion.Capabilities.SupportsWindows81Features;
			}
		}

		public static bool SupportsUnicodePaths
		{
			get
			{
				return EsentVersion.Capabilities.SupportsUnicodePaths;
			}
		}

		public static bool SupportsLargeKeys
		{
			get
			{
				return EsentVersion.Capabilities.SupportsLargeKeys;
			}
		}

		private static JetCapabilities Capabilities
		{
			get
			{
				return Api.Impl.Capabilities;
			}
		}
	}
}
