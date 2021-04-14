using System;
using System.Globalization;

namespace System.Reflection
{
	internal class MetadataException : Exception
	{
		internal MetadataException(int hr)
		{
			this.m_hr = hr;
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.CurrentCulture, "MetadataException HResult = {0:x}.", this.m_hr);
		}

		private int m_hr;
	}
}
