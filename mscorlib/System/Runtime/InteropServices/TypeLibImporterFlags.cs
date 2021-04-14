using System;

namespace System.Runtime.InteropServices
{
	[Flags]
	[ComVisible(true)]
	[Serializable]
	public enum TypeLibImporterFlags
	{
		None = 0,
		PrimaryInteropAssembly = 1,
		UnsafeInterfaces = 2,
		SafeArrayAsSystemArray = 4,
		TransformDispRetVals = 8,
		PreventClassMembers = 16,
		SerializableValueClasses = 32,
		ImportAsX86 = 256,
		ImportAsX64 = 512,
		ImportAsItanium = 1024,
		ImportAsAgnostic = 2048,
		ReflectionOnlyLoading = 4096,
		NoDefineVersionResource = 8192,
		ImportAsArm = 16384
	}
}
