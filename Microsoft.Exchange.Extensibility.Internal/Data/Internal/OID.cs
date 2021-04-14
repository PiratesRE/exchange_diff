using System;
using System.Text;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Internal
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal struct OID : IComparable, IComparable<OID>
	{
		public OID(int t1, int t2)
		{
			OID.CheckT1T2(t1, t2);
			this.bytes = new byte[1];
			this.bytes[0] = (byte)(t1 * 40 + t2);
		}

		public OID(int t1, int t2, int t3)
		{
			OID.CheckT1T2(t1, t2);
			int num = 1 + OID.GetTLength(t3);
			this.bytes = new byte[num];
			this.bytes[0] = (byte)(t1 * 40 + t2);
			int num2 = 1;
			num2 += OID.EncodeT(t3, this.bytes, num2);
		}

		public OID(int t1, int t2, int t3, int t4)
		{
			OID.CheckT1T2(t1, t2);
			int num = 1 + OID.GetTLength(t3) + OID.GetTLength(t4);
			this.bytes = new byte[num];
			this.bytes[0] = (byte)(t1 * 40 + t2);
			int num2 = 1;
			num2 += OID.EncodeT(t3, this.bytes, num2);
			num2 += OID.EncodeT(t4, this.bytes, num2);
		}

		public OID(int t1, int t2, int t3, int t4, int t5)
		{
			OID.CheckT1T2(t1, t2);
			int num = 1 + OID.GetTLength(t3) + OID.GetTLength(t4) + OID.GetTLength(t5);
			this.bytes = new byte[num];
			this.bytes[0] = (byte)(t1 * 40 + t2);
			int num2 = 1;
			num2 += OID.EncodeT(t3, this.bytes, num2);
			num2 += OID.EncodeT(t4, this.bytes, num2);
			num2 += OID.EncodeT(t5, this.bytes, num2);
		}

		public OID(int t1, int t2, int t3, int t4, int t5, int t6)
		{
			OID.CheckT1T2(t1, t2);
			int num = 1 + OID.GetTLength(t3) + OID.GetTLength(t4) + OID.GetTLength(t5) + OID.GetTLength(t6);
			this.bytes = new byte[num];
			this.bytes[0] = (byte)(t1 * 40 + t2);
			int num2 = 1;
			num2 += OID.EncodeT(t3, this.bytes, num2);
			num2 += OID.EncodeT(t4, this.bytes, num2);
			num2 += OID.EncodeT(t5, this.bytes, num2);
			num2 += OID.EncodeT(t6, this.bytes, num2);
		}

		public OID(int t1, int t2, int t3, int t4, int t5, int t6, int t7)
		{
			OID.CheckT1T2(t1, t2);
			int num = 1 + OID.GetTLength(t3) + OID.GetTLength(t4) + OID.GetTLength(t5) + OID.GetTLength(t6) + OID.GetTLength(t7);
			this.bytes = new byte[num];
			this.bytes[0] = (byte)(t1 * 40 + t2);
			int num2 = 1;
			num2 += OID.EncodeT(t3, this.bytes, num2);
			num2 += OID.EncodeT(t4, this.bytes, num2);
			num2 += OID.EncodeT(t5, this.bytes, num2);
			num2 += OID.EncodeT(t6, this.bytes, num2);
			num2 += OID.EncodeT(t7, this.bytes, num2);
		}

		public OID(int t1, int t2, int t3, int t4, int t5, int t6, int t7, int t8)
		{
			OID.CheckT1T2(t1, t2);
			int num = 1 + OID.GetTLength(t3) + OID.GetTLength(t4) + OID.GetTLength(t5) + OID.GetTLength(t6) + OID.GetTLength(t7) + OID.GetTLength(t8);
			this.bytes = new byte[num];
			this.bytes[0] = (byte)(t1 * 40 + t2);
			int num2 = 1;
			num2 += OID.EncodeT(t3, this.bytes, num2);
			num2 += OID.EncodeT(t4, this.bytes, num2);
			num2 += OID.EncodeT(t5, this.bytes, num2);
			num2 += OID.EncodeT(t6, this.bytes, num2);
			num2 += OID.EncodeT(t7, this.bytes, num2);
			num2 += OID.EncodeT(t8, this.bytes, num2);
		}

		public OID(int t1, int t2, int t3, int t4, int t5, int t6, int t7, int t8, int t9)
		{
			OID.CheckT1T2(t1, t2);
			int num = 1 + OID.GetTLength(t3) + OID.GetTLength(t4) + OID.GetTLength(t5) + OID.GetTLength(t6) + OID.GetTLength(t7) + OID.GetTLength(t8) + OID.GetTLength(t9);
			this.bytes = new byte[num];
			this.bytes[0] = (byte)(t1 * 40 + t2);
			int num2 = 1;
			num2 += OID.EncodeT(t3, this.bytes, num2);
			num2 += OID.EncodeT(t4, this.bytes, num2);
			num2 += OID.EncodeT(t5, this.bytes, num2);
			num2 += OID.EncodeT(t6, this.bytes, num2);
			num2 += OID.EncodeT(t7, this.bytes, num2);
			num2 += OID.EncodeT(t8, this.bytes, num2);
			num2 += OID.EncodeT(t9, this.bytes, num2);
		}

		public OID(int t1, int t2, int t3, int t4, int t5, int t6, int t7, int t8, int t9, int t10)
		{
			OID.CheckT1T2(t1, t2);
			int num = 1 + OID.GetTLength(t3) + OID.GetTLength(t4) + OID.GetTLength(t5) + OID.GetTLength(t6) + OID.GetTLength(t7) + OID.GetTLength(t8) + OID.GetTLength(t9) + OID.GetTLength(t10);
			this.bytes = new byte[num];
			this.bytes[0] = (byte)(t1 * 40 + t2);
			int num2 = 1;
			num2 += OID.EncodeT(t3, this.bytes, num2);
			num2 += OID.EncodeT(t4, this.bytes, num2);
			num2 += OID.EncodeT(t5, this.bytes, num2);
			num2 += OID.EncodeT(t6, this.bytes, num2);
			num2 += OID.EncodeT(t7, this.bytes, num2);
			num2 += OID.EncodeT(t8, this.bytes, num2);
			num2 += OID.EncodeT(t9, this.bytes, num2);
			num2 += OID.EncodeT(t10, this.bytes, num2);
		}

		public OID(params int[] t)
		{
			if (t.Length < 2)
			{
				throw new InvalidOperationException("invalid OID value - should have at least 2 components");
			}
			OID.CheckT1T2(t[0], t[1]);
			int num = 1;
			for (int i = 2; i < t.Length; i++)
			{
				num += OID.GetTLength(t[i]);
			}
			this.bytes = new byte[num];
			this.bytes[0] = (byte)(t[0] * 40 + t[1]);
			int num2 = 1;
			for (int j = 2; j < t.Length; j++)
			{
				num2 += OID.EncodeT(t[j], this.bytes, num2);
			}
		}

		internal OID(byte[] buffer, int offset, int length)
		{
			this.bytes = new byte[length];
			Buffer.BlockCopy(buffer, offset, this.bytes, 0, length);
		}

		private OID(int t1)
		{
			this.bytes = null;
		}

		public int Length
		{
			get
			{
				if (this.bytes != null)
				{
					return this.bytes.Length;
				}
				return 0;
			}
		}

		public static bool operator ==(OID x, OID y)
		{
			return 0 == OID.CompareBytes(x.bytes, y.bytes);
		}

		public static bool operator !=(OID x, OID y)
		{
			return !(x == y);
		}

		public override bool Equals(object obj)
		{
			return obj is OID && this == (OID)obj;
		}

		public override int GetHashCode()
		{
			if (this.bytes == null)
			{
				return 0;
			}
			int num = 0;
			for (int i = 0; i < this.bytes.Length; i++)
			{
				num = (num << 8 ^ (int)this.bytes[i] ^ num >> 24);
			}
			return num;
		}

		public int CompareTo(object obj)
		{
			if (obj == null)
			{
				throw new ArgumentNullException("obj");
			}
			if (!(obj is OID))
			{
				throw new ArgumentException("obj");
			}
			return this.CompareTo((OID)obj);
		}

		public int CompareTo(OID other)
		{
			return OID.CompareBytes(this.bytes, other.bytes);
		}

		public override string ToString()
		{
			if (this.bytes == null)
			{
				return string.Empty;
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(((int)(this.bytes[0] / 40)).ToString());
			stringBuilder.Append(".");
			stringBuilder.Append(((int)(this.bytes[0] % 40)).ToString());
			int num = 1;
			while (num != this.bytes.Length)
			{
				int num2 = 0;
				byte b;
				do
				{
					b = this.bytes[num++];
					num2 = (num2 << 7) + (int)(b & 127);
				}
				while ((b & 128) != 0 && num < this.bytes.Length);
				stringBuilder.Append(".");
				stringBuilder.Append(num2.ToString());
			}
			return stringBuilder.ToString();
		}

		private static int CompareBytes(byte[] x, byte[] y)
		{
			if (x == y)
			{
				return 0;
			}
			if (y == null)
			{
				return 1;
			}
			if (x == null)
			{
				return -1;
			}
			for (int i = 0; i < x.Length; i++)
			{
				if (i >= y.Length)
				{
					return 1;
				}
				if (x[i] > y[i])
				{
					return -1;
				}
				if (x[i] < y[i])
				{
					return -1;
				}
			}
			if (x.Length < y.Length)
			{
				return -1;
			}
			return 0;
		}

		private static void CheckT1T2(int t1, int t2)
		{
			if (t1 >= 3 || t2 >= 40)
			{
				throw new InvalidOperationException("invalid OID value - first two elements");
			}
		}

		private static int GetTLength(int t)
		{
			if (t >= 0)
			{
				if (t <= 127)
				{
					return 1;
				}
				if (t <= 16383)
				{
					return 2;
				}
				if (t <= 2097151)
				{
					return 3;
				}
				if (t <= 268435455)
				{
					return 4;
				}
			}
			throw new InvalidOperationException("invalid OID value - element is negative or too large");
		}

		private static int EncodeT(int t, byte[] buffer, int offset)
		{
			int tlength = OID.GetTLength(t);
			int num = OID.Masks[tlength - 1];
			for (int num2 = OID.Shifts[tlength - 1]; num2 != 0; num2 -= 7)
			{
				buffer[offset++] = (byte)((t & num) >> num2 | 128);
				num >>= 7;
			}
			buffer[offset] = (byte)(t & 127);
			return tlength;
		}

		private static readonly int[] Masks = new int[]
		{
			127,
			16256,
			2080768,
			266338304
		};

		private static readonly int[] Shifts = new int[]
		{
			0,
			7,
			14,
			21
		};

		public static readonly OID DS = new OID(2, 5);

		public static readonly OID DSALG = new OID(2, 5, 8);

		public static readonly OID DSALGCRPT = new OID(2, 5, 8, 1);

		public static readonly OID DSALGHash = new OID(2, 5, 8, 2);

		public static readonly OID DSALGSign = new OID(2, 5, 8, 3);

		public static readonly OID DSALGRSA = new OID(2, 5, 8, 1, 1);

		public static readonly OID AuthorityKeyIdentifier = new OID(2, 5, 29, 1);

		public static readonly OID KeyUsageRestriction = new OID(2, 5, 29, 4);

		public static readonly OID LegacyPolicyMappings = new OID(2, 5, 29, 5);

		public static readonly OID SubjectALTName = new OID(2, 5, 29, 7);

		public static readonly OID IssuerALTName = new OID(2, 5, 29, 8);

		public static readonly OID BasicConstraints = new OID(2, 5, 29, 10);

		public static readonly OID SubjectKeyIdentifier = new OID(2, 5, 29, 14);

		public static readonly OID KeyUsage = new OID(2, 5, 29, 15);

		public static readonly OID SubjectALTName2 = new OID(2, 5, 29, 17);

		public static readonly OID IssuerALTName2 = new OID(2, 5, 29, 18);

		public static readonly OID BasicConstraints2 = new OID(2, 5, 29, 19);

		public static readonly OID CRLNumber = new OID(2, 5, 29, 20);

		public static readonly OID CRLReasonCode = new OID(2, 5, 29, 21);

		public static readonly OID ReasonCodeHold = new OID(2, 5, 29, 23);

		public static readonly OID DeltaCRLIndicator = new OID(2, 5, 29, 27);

		public static readonly OID IssuingDISTPoint = new OID(2, 5, 29, 28);

		public static readonly OID NameConstraints = new OID(2, 5, 29, 30);

		public static readonly OID CRLDISTPoints = new OID(2, 5, 29, 31);

		public static readonly OID CertPolicies = new OID(2, 5, 29, 32);

		public static readonly OID PolicyMappings = new OID(2, 5, 29, 33);

		public static readonly OID AuthorityKeyIdentifier2 = new OID(2, 5, 29, 35);

		public static readonly OID PolicyConstraints = new OID(2, 5, 29, 36);

		public static readonly OID EnhancedKeyUsage = new OID(2, 5, 29, 37);

		public static readonly OID FreshestCRL = new OID(2, 5, 29, 46);

		public static readonly OID MicrosoftEncryptionKeyPreference = new OID(1, 3, 6, 1, 4, 1, 311, 16, 4);

		public static readonly OID EnrollCerttypeExtension = new OID(1, 3, 6, 1, 4, 1, 311, 20, 2);

		public static readonly OID NTPrincipalName = new OID(1, 3, 6, 1, 4, 1, 311, 20, 2, 3);

		public static readonly OID CertificateTemplate = new OID(1, 3, 6, 1, 4, 1, 311, 21, 7);

		public static readonly OID RDNDummySigner = new OID(1, 3, 6, 1, 4, 1, 311, 21, 9);

		public static readonly OID AuthorityInfoAccess = new OID(1, 3, 6, 1, 5, 5, 7, 1, 1);

		public static readonly OID RSA = new OID(1, 2, 840, 113549);

		public static readonly OID PKCS = new OID(1, 2, 840, 113549, 1);

		public static readonly OID RSAHASH = new OID(1, 2, 840, 113549, 2);

		public static readonly OID RSAENCRYPT = new OID(1, 2, 840, 113549, 3);

		public static readonly OID PKCS1 = new OID(1, 2, 840, 113549, 1, 1);

		public static readonly OID PKCS2 = new OID(1, 2, 840, 113549, 1, 2);

		public static readonly OID PKCS3 = new OID(1, 2, 840, 113549, 1, 3);

		public static readonly OID PKCS4 = new OID(1, 2, 840, 113549, 1, 4);

		public static readonly OID PKCS5 = new OID(1, 2, 840, 113549, 1, 5);

		public static readonly OID PKCS6 = new OID(1, 2, 840, 113549, 1, 6);

		public static readonly OID PKCS7 = new OID(1, 2, 840, 113549, 1, 7);

		public static readonly OID PKCS8 = new OID(1, 2, 840, 113549, 1, 8);

		public static readonly OID PKCS9 = new OID(1, 2, 840, 113549, 1, 9);

		public static readonly OID PKCS10 = new OID(1, 2, 840, 113549, 1, 10);

		public static readonly OID PKCS12 = new OID(1, 2, 840, 113549, 1, 12);

		public static readonly OID RSARSA = new OID(1, 2, 840, 113549, 1, 1, 1);

		public static readonly OID RSAMD2RSA = new OID(1, 2, 840, 113549, 1, 1, 2);

		public static readonly OID RSAMD4RSA = new OID(1, 2, 840, 113549, 1, 1, 3);

		public static readonly OID RSAMD5RSA = new OID(1, 2, 840, 113549, 1, 1, 4);

		public static readonly OID RSASHA1RSA = new OID(1, 2, 840, 113549, 1, 1, 5);

		public static readonly OID RSASETOAEPRSA = new OID(1, 2, 840, 113549, 1, 1, 6);

		public static readonly OID RSADH = new OID(1, 2, 840, 113549, 1, 3, 1);

		public static readonly OID RSAData = new OID(1, 2, 840, 113549, 1, 7, 1);

		public static readonly OID RSASignedData = new OID(1, 2, 840, 113549, 1, 7, 2);

		public static readonly OID RSAEnvelopedData = new OID(1, 2, 840, 113549, 1, 7, 3);

		public static readonly OID RSASignEnvData = new OID(1, 2, 840, 113549, 1, 7, 4);

		public static readonly OID RSADigestedData = new OID(1, 2, 840, 113549, 1, 7, 5);

		public static readonly OID RSAHashedData = new OID(1, 2, 840, 113549, 1, 7, 5);

		public static readonly OID RSAEncryptedData = new OID(1, 2, 840, 113549, 1, 7, 6);

		public static readonly OID RSAEmailAddr = new OID(1, 2, 840, 113549, 1, 9, 1);

		public static readonly OID RSAUnstructName = new OID(1, 2, 840, 113549, 1, 9, 2);

		public static readonly OID RSAContentType = new OID(1, 2, 840, 113549, 1, 9, 3);

		public static readonly OID RSAMessageDigest = new OID(1, 2, 840, 113549, 1, 9, 4);

		public static readonly OID RSASigningTime = new OID(1, 2, 840, 113549, 1, 9, 5);

		public static readonly OID RSACounterSign = new OID(1, 2, 840, 113549, 1, 9, 6);

		public static readonly OID RSAChallengePwd = new OID(1, 2, 840, 113549, 1, 9, 7);

		public static readonly OID RSAUnstructAddr = new OID(1, 2, 840, 113549, 1, 9, 8);

		public static readonly OID RSAExtCertAttrs = new OID(1, 2, 840, 113549, 1, 9, 9);

		public static readonly OID RSACertExtensions = new OID(1, 2, 840, 113549, 1, 9, 14);

		public static readonly OID RSASMIMECapabilities = new OID(1, 2, 840, 113549, 1, 9, 15);

		public static readonly OID RSAPreferSignedData = new OID(1, 2, 840, 113549, 1, 9, 15, 1);

		public static readonly OID OIWSESha1 = new OID(1, 3, 14, 3, 2, 26);

		public static readonly OID RSAMD2 = new OID(1, 2, 840, 113549, 2, 2);

		public static readonly OID RSAMD4 = new OID(1, 2, 840, 113549, 2, 4);

		public static readonly OID RSAMD5 = new OID(1, 2, 840, 113549, 2, 5);

		public static readonly OID OIWSECSHA256 = new OID(2, 16, 840, 1, 101, 3, 4, 1);

		public static readonly OID OIWSECSHA384 = new OID(2, 16, 840, 1, 101, 3, 4, 2);

		public static readonly OID OIWSECSHA512 = new OID(2, 16, 840, 1, 101, 3, 4, 3);

		public static readonly OID RSARC2CBC = new OID(1, 2, 840, 113549, 3, 2);

		public static readonly OID RSARC4 = new OID(1, 2, 840, 113549, 3, 4);

		public static readonly OID RSADESEDE3CBC = new OID(1, 2, 840, 113549, 3, 7);

		public static readonly OID RSARC5CBCPad = new OID(1, 2, 840, 113549, 3, 9);

		public static readonly OID OIWSECDesCBC = new OID(1, 3, 14, 3, 2, 7);

		public static readonly OID RSASMIMEalg = new OID(1, 2, 840, 113549, 1, 9, 16, 3);

		public static readonly OID RSASMIMEalgESDH = new OID(1, 2, 840, 113549, 1, 9, 16, 3, 5);

		public static readonly OID RSASMIMEalgCMS3DESwrap = new OID(1, 2, 840, 113549, 1, 9, 16, 3, 6);

		public static readonly OID RSASMIMEalgCMSRC2wrap = new OID(1, 2, 840, 113549, 1, 9, 16, 3, 7);

		public static readonly OID OIWSECSha1RSASign = new OID(1, 3, 14, 3, 2, 29);

		public static readonly OID AnsiX942 = new OID(1, 2, 840, 10046);

		public static readonly OID AnsiX942DH = new OID(1, 2, 840, 10046, 2, 1);

		public static readonly OID X957 = new OID(1, 2, 840, 10040);

		public static readonly OID X957DSA = new OID(1, 2, 840, 10040, 4, 1);

		public static readonly OID X957SHA1DSA = new OID(1, 2, 840, 10040, 4, 3);

		public static readonly OID CommonName = new OID(2, 5, 4, 3);

		public static readonly OID SurName = new OID(2, 5, 4, 4);

		public static readonly OID DeviceSerialNumber = new OID(2, 5, 4, 5);

		public static readonly OID CountryName = new OID(2, 5, 4, 6);

		public static readonly OID LocalityName = new OID(2, 5, 4, 7);

		public static readonly OID StateOrProvinceName = new OID(2, 5, 4, 8);

		public static readonly OID StreetAddress = new OID(2, 5, 4, 9);

		public static readonly OID OrganizationName = new OID(2, 5, 4, 10);

		public static readonly OID OrganizationalUnitName = new OID(2, 5, 4, 11);

		public static readonly OID TITLE = new OID(2, 5, 4, 12);

		public static readonly OID DESCRIPTION = new OID(2, 5, 4, 13);

		public static readonly OID SearchGuide = new OID(2, 5, 4, 14);

		public static readonly OID BusinessCategory = new OID(2, 5, 4, 15);

		public static readonly OID PostalAddress = new OID(2, 5, 4, 16);

		public static readonly OID PostalCode = new OID(2, 5, 4, 17);

		public static readonly OID PostOfficeBox = new OID(2, 5, 4, 18);

		public static readonly OID PhysicalDeliveryOfficeName = new OID(2, 5, 4, 19);

		public static readonly OID TelephoneNumber = new OID(2, 5, 4, 20);

		public static readonly OID TelexNumber = new OID(2, 5, 4, 21);

		public static readonly OID TeletextTerminalIdentifier = new OID(2, 5, 4, 22);

		public static readonly OID FacsimileTelephoneNumber = new OID(2, 5, 4, 23);

		public static readonly OID X21Address = new OID(2, 5, 4, 24);

		public static readonly OID InternationalISDNNumber = new OID(2, 5, 4, 25);

		public static readonly OID RegisteredAddress = new OID(2, 5, 4, 26);

		public static readonly OID DestinationIndicator = new OID(2, 5, 4, 27);

		public static readonly OID PreferredDeliveryMethod = new OID(2, 5, 4, 28);

		public static readonly OID PresentationAddress = new OID(2, 5, 4, 29);

		public static readonly OID SupportedApplicationContext = new OID(2, 5, 4, 30);

		public static readonly OID MEMBER = new OID(2, 5, 4, 31);

		public static readonly OID OWNER = new OID(2, 5, 4, 32);

		public static readonly OID RoleOccupant = new OID(2, 5, 4, 33);

		public static readonly OID SeeAlso = new OID(2, 5, 4, 34);

		public static readonly OID UserPassword = new OID(2, 5, 4, 35);

		public static readonly OID UserCertificate = new OID(2, 5, 4, 36);

		public static readonly OID CACertificate = new OID(2, 5, 4, 37);

		public static readonly OID AuthorityRevocationList = new OID(2, 5, 4, 38);

		public static readonly OID CertificateRevocationList = new OID(2, 5, 4, 39);

		public static readonly OID CrossCertificatePair = new OID(2, 5, 4, 40);

		public static readonly OID GivenName = new OID(2, 5, 4, 42);

		public static readonly OID INITIALS = new OID(2, 5, 4, 43);

		public static readonly OID DnQualifier = new OID(2, 5, 4, 46);

		public static readonly OID DomainComponent = new OID(0, 9, 2342, 19200300, 100, 1, 25);

		public static readonly OID Pkcs12FriendlyNameAttr = new OID(1, 2, 840, 113549, 1, 9, 20);

		public static readonly OID Pkcs12LocalKeyID = new OID(1, 2, 840, 113549, 1, 9, 21);

		public static readonly OID Pkcs12KeyProviderNameAttr = new OID(1, 3, 6, 1, 4, 1, 311, 17, 1);

		public static readonly OID LocalMachineKeyset = new OID(1, 3, 6, 1, 4, 1, 311, 17, 2);

		public static readonly OID KeyIdRDN = new OID(1, 3, 6, 1, 4, 1, 311, 10, 7, 1);

		private byte[] bytes;
	}
}
