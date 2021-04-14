using System;

namespace System.Security.Cryptography
{
	public sealed class RSASignaturePadding : IEquatable<RSASignaturePadding>
	{
		private RSASignaturePadding(RSASignaturePaddingMode mode)
		{
			this._mode = mode;
		}

		public static RSASignaturePadding Pkcs1
		{
			get
			{
				return RSASignaturePadding.s_pkcs1;
			}
		}

		public static RSASignaturePadding Pss
		{
			get
			{
				return RSASignaturePadding.s_pss;
			}
		}

		public RSASignaturePaddingMode Mode
		{
			get
			{
				return this._mode;
			}
		}

		public override int GetHashCode()
		{
			return this._mode.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return this.Equals(obj as RSASignaturePadding);
		}

		public bool Equals(RSASignaturePadding other)
		{
			return other != null && this._mode == other._mode;
		}

		public static bool operator ==(RSASignaturePadding left, RSASignaturePadding right)
		{
			if (left == null)
			{
				return right == null;
			}
			return left.Equals(right);
		}

		public static bool operator !=(RSASignaturePadding left, RSASignaturePadding right)
		{
			return !(left == right);
		}

		public override string ToString()
		{
			return this._mode.ToString();
		}

		private static readonly RSASignaturePadding s_pkcs1 = new RSASignaturePadding(RSASignaturePaddingMode.Pkcs1);

		private static readonly RSASignaturePadding s_pss = new RSASignaturePadding(RSASignaturePaddingMode.Pss);

		private readonly RSASignaturePaddingMode _mode;
	}
}
