using System;
using System.Globalization;
using System.Runtime.InteropServices;

namespace Microsoft.Isam.Esent.Interop
{
	public class LOGSHIP_INFO
	{
		public LogshipType ulType { get; set; }

		public string wszName { get; set; }

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "LOGSHIP_INFO({0})", new object[]
			{
				this.wszName
			});
		}

		internal void SetFromNative(ref NATIVE_LOGSHIP_INFO native)
		{
			this.ulType = (LogshipType)native.ulType;
			this.wszName = Marshal.PtrToStringUni(native.wszName);
		}

		internal NATIVE_LOGSHIP_INFO GetNativeLogshipInfo()
		{
			return new NATIVE_LOGSHIP_INFO
			{
				ulType = (uint)this.ulType,
				cchName = (uint)(this.wszName.Length + 1),
				wszName = Marshal.StringToHGlobalUni(this.wszName)
			};
		}
	}
}
