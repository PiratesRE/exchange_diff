using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	public class X509Identifier : IEquatable<X509Identifier>
	{
		public string Issuer { get; private set; }

		public string Subject { get; private set; }

		public bool IsGenericIdentifier
		{
			get
			{
				return string.IsNullOrEmpty(this.Subject);
			}
		}

		public X509Identifier(string issuer, string subject)
		{
			if (string.IsNullOrEmpty(issuer))
			{
				throw new ArgumentNullException("issuer");
			}
			this.Issuer = issuer;
			this.Subject = subject;
		}

		public X509Identifier(string issuer) : this(issuer, null)
		{
		}

		public X509Identifier(X509Certificate certificate)
		{
			this.Subject = certificate.Subject;
			this.Issuer = certificate.Issuer;
		}

		public bool IsMatchWith(X509Identifier other)
		{
			if (other == null)
			{
				return false;
			}
			if (this.IsGenericIdentifier)
			{
				return this.Issuer.Equals(other.Issuer, StringComparison.OrdinalIgnoreCase);
			}
			return this.Equals(other);
		}

		public static X509Identifier Parse(string x509Identifier)
		{
			X509Identifier result = null;
			if (!X509Identifier.TryParse(x509Identifier, out result))
			{
				throw new FormatException(DataStrings.InvalidX509IdentifierFormat(x509Identifier));
			}
			return result;
		}

		public static bool TryParse(string x509Identifier, out X509Identifier instance)
		{
			Match match = Regex.Match(x509Identifier, "^X509:<I>([^<]+)(<S>(.+))?", RegexOptions.IgnoreCase);
			instance = null;
			bool flag = match.Success;
			if (flag)
			{
				try
				{
					instance = new X509Identifier(new X500DistinguishedName(match.Groups[1].Value, X500DistinguishedNameFlags.None).Format(false), new X500DistinguishedName(match.Groups[3].Value, X500DistinguishedNameFlags.None).Format(false));
				}
				catch (CryptographicException)
				{
					flag = false;
				}
			}
			return flag;
		}

		public override string ToString()
		{
			string result;
			if (this.IsGenericIdentifier)
			{
				result = this.ToIssuerString();
			}
			else
			{
				result = string.Format("{0}{1}{2}{3}{4}", new object[]
				{
					"X509:",
					"<I>",
					this.Issuer,
					"<S>",
					this.Subject
				});
			}
			return result;
		}

		internal string ToIssuerString()
		{
			return string.Format("{0}{1}{2}", "X509:", "<I>", this.Issuer);
		}

		public bool Equals(X509Identifier other)
		{
			return other != null && this.Subject == other.Subject && this.Issuer == other.Issuer;
		}

		public override bool Equals(object obj)
		{
			return this.Equals(obj as X509Identifier);
		}

		public static bool operator ==(X509Identifier left, X509Identifier right)
		{
			if (left != null)
			{
				return left.Equals(right);
			}
			return right == null;
		}

		public static bool operator !=(X509Identifier left, X509Identifier right)
		{
			return !(left == right);
		}

		public override int GetHashCode()
		{
			return this.ToString().GetHashCode();
		}

		internal const string Prefix = "X509:";

		private const string IssuerPrefix = "<I>";

		private const string SubjectPrefix = "<S>";

		private const string FormatRegularExpression = "^X509:<I>([^<]+)(<S>(.+))?";
	}
}
