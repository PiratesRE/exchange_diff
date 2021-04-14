using System;

namespace System.Runtime.InteropServices
{
	public struct OSPlatform : IEquatable<OSPlatform>
	{
		public static OSPlatform Linux { get; } = new OSPlatform("LINUX");

		public static OSPlatform OSX { get; } = new OSPlatform("OSX");

		public static OSPlatform Windows { get; } = new OSPlatform("WINDOWS");

		private OSPlatform(string osPlatform)
		{
			if (osPlatform == null)
			{
				throw new ArgumentNullException("osPlatform");
			}
			if (osPlatform.Length == 0)
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_EmptyValue"), "osPlatform");
			}
			this._osPlatform = osPlatform;
		}

		public static OSPlatform Create(string osPlatform)
		{
			return new OSPlatform(osPlatform);
		}

		public bool Equals(OSPlatform other)
		{
			return this.Equals(other._osPlatform);
		}

		internal bool Equals(string other)
		{
			return string.Equals(this._osPlatform, other, StringComparison.Ordinal);
		}

		public override bool Equals(object obj)
		{
			return obj is OSPlatform && this.Equals((OSPlatform)obj);
		}

		public override int GetHashCode()
		{
			if (this._osPlatform != null)
			{
				return this._osPlatform.GetHashCode();
			}
			return 0;
		}

		public override string ToString()
		{
			return this._osPlatform ?? string.Empty;
		}

		public static bool operator ==(OSPlatform left, OSPlatform right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(OSPlatform left, OSPlatform right)
		{
			return !(left == right);
		}

		private readonly string _osPlatform;
	}
}
