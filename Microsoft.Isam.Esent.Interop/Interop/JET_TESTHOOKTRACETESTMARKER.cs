using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using Microsoft.Isam.Esent.Interop.Implementation;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public class JET_TESTHOOKTRACETESTMARKER : IContentEquatable<JET_TESTHOOKTRACETESTMARKER>, IDeepCloneable<JET_TESTHOOKTRACETESTMARKER>
	{
		public ulong qwMarkerID
		{
			[DebuggerStepThrough]
			get
			{
				return this.markerId;
			}
			set
			{
				this.markerId = value;
			}
		}

		public string szAnnotation
		{
			[DebuggerStepThrough]
			get
			{
				return this.annotation;
			}
			set
			{
				this.annotation = value;
			}
		}

		public bool ContentEquals(JET_TESTHOOKTRACETESTMARKER other)
		{
			return other != null && this.qwMarkerID == other.qwMarkerID && string.Equals(this.szAnnotation, other.szAnnotation);
		}

		public JET_TESTHOOKTRACETESTMARKER DeepClone()
		{
			return (JET_TESTHOOKTRACETESTMARKER)base.MemberwiseClone();
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "JET_TESTHOOKTRACETESTMARKER({0}:{1})", new object[]
			{
				this.qwMarkerID,
				this.szAnnotation
			});
		}

		internal NATIVE_TESTHOOKTRACETESTMARKER GetNativeTraceTestMarker(ref GCHandleCollection handles)
		{
			return new NATIVE_TESTHOOKTRACETESTMARKER
			{
				cbStruct = checked((uint)Marshal.SizeOf(typeof(NATIVE_TESTHOOKTRACETESTMARKER))),
				qwMarkerID = this.qwMarkerID,
				szAnnotation = handles.Add(Util.ConvertToNullTerminatedAsciiByteArray(this.szAnnotation))
			};
		}

		private ulong markerId;

		private string annotation;
	}
}
