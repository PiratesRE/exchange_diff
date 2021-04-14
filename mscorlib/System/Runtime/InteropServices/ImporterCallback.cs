using System;
using System.Reflection;
using System.Runtime.InteropServices.ComTypes;
using System.Security;

namespace System.Runtime.InteropServices
{
	internal class ImporterCallback : ITypeLibImporterNotifySink
	{
		public void ReportEvent(ImporterEventKind EventKind, int EventCode, string EventMsg)
		{
		}

		[SecuritySafeCritical]
		public Assembly ResolveRef(object TypeLib)
		{
			Assembly result;
			try
			{
				ITypeLibConverter typeLibConverter = new TypeLibConverter();
				result = typeLibConverter.ConvertTypeLibToAssembly(TypeLib, Marshal.GetTypeLibName((ITypeLib)TypeLib) + ".dll", TypeLibImporterFlags.None, new ImporterCallback(), null, null, null, null);
			}
			catch (Exception)
			{
				result = null;
			}
			return result;
		}
	}
}
