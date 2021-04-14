using System;
using System.Runtime.InteropServices;
using System.Security;

namespace System.Runtime.Serialization.Formatters
{
	[ComVisible(true)]
	public interface IFieldInfo
	{
		string[] FieldNames { [SecurityCritical] get; [SecurityCritical] set; }

		Type[] FieldTypes { [SecurityCritical] get; [SecurityCritical] set; }
	}
}
