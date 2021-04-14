using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Transport;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal class EhloOptions : IEhloOptions
	{
		static EhloOptions()
		{
			foreach (object obj in Enum.GetValues(typeof(EhloOptionsFlags)))
			{
				EhloOptionsFlags ehloOptionsFlags = (EhloOptionsFlags)obj;
				EhloOptions.flagStrings[(int)ehloOptionsFlags] = ehloOptionsFlags.ToString();
			}
		}

		public int Flags
		{
			get
			{
				return this.flags;
			}
			set
			{
				this.flags = value;
			}
		}

		public string AdvertisedFQDN
		{
			get
			{
				return this.advertisedFQDN;
			}
			set
			{
				this.advertisedFQDN = value;
			}
		}

		public IPAddress AdvertisedIPAddress
		{
			get
			{
				return this.advertisedIPAddress;
			}
			set
			{
				this.advertisedIPAddress = value;
			}
		}

		public bool BinaryMime
		{
			get
			{
				return (this.flags & 4) != 0;
			}
			set
			{
				this.SetFlags(EhloOptionsFlags.BinaryMime, value);
			}
		}

		public bool EightBitMime
		{
			get
			{
				return (this.flags & 32) != 0;
			}
			set
			{
				this.SetFlags(EhloOptionsFlags.EightBitMime, value);
			}
		}

		public bool StartTLS
		{
			get
			{
				return (this.flags & 256) != 0;
			}
			set
			{
				this.SetFlags(EhloOptionsFlags.StartTls, value);
			}
		}

		public bool AnonymousTLS
		{
			get
			{
				return (this.flags & 1) != 0;
			}
			set
			{
				this.SetFlags(EhloOptionsFlags.AnonymousTls, value);
			}
		}

		public ICollection<string> ExchangeAuthArgs
		{
			get
			{
				return this.exchangeAuthArgs;
			}
		}

		public bool Pipelining
		{
			get
			{
				return (this.flags & 128) != 0;
			}
			set
			{
				this.SetFlags(EhloOptionsFlags.Pipelining, value);
			}
		}

		public bool EnhancedStatusCodes
		{
			get
			{
				return (this.flags & 64) != 0;
			}
			set
			{
				this.SetFlags(EhloOptionsFlags.EnhancedStatusCodes, value);
			}
		}

		public bool Dsn
		{
			get
			{
				return (this.flags & 16) != 0;
			}
			set
			{
				this.SetFlags(EhloOptionsFlags.Dsn, value);
			}
		}

		public SizeMode Size
		{
			get
			{
				return this.size;
			}
			set
			{
				this.size = value;
			}
		}

		public long MaxSize
		{
			get
			{
				return this.maxSize;
			}
			set
			{
				this.maxSize = value;
			}
		}

		public ICollection<string> AuthenticationMechanisms
		{
			get
			{
				return this.authenticationMechanisms;
			}
		}

		public void AddAuthenticationMechanism(string mechanism, bool enabled)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("mechanism", mechanism);
			if (enabled && !this.authenticationMechanisms.Contains(mechanism))
			{
				this.authenticationMechanisms.Add(mechanism);
			}
		}

		public bool Chunking
		{
			get
			{
				return (this.flags & 8) != 0;
			}
			set
			{
				this.SetFlags(EhloOptionsFlags.Chunking, value);
			}
		}

		public bool XMsgId
		{
			get
			{
				return (this.flags & 33554432) != 0;
			}
			set
			{
				this.SetFlags(EhloOptionsFlags.XMsgId, value);
			}
		}

		public bool Xexch50
		{
			get
			{
				return (this.flags & 512) != 0;
			}
			set
			{
				this.SetFlags(EhloOptionsFlags.Xexch50, value);
			}
		}

		public bool XLongAddr
		{
			get
			{
				return (this.flags & 1024) != 0;
			}
			set
			{
				this.SetFlags(EhloOptionsFlags.XlongAddr, value);
			}
		}

		public bool XOrar
		{
			get
			{
				return (this.flags & 4096) != 0;
			}
			set
			{
				this.SetFlags(EhloOptionsFlags.Xorar, value);
			}
		}

		public bool XRDst
		{
			get
			{
				return (this.flags & 16384) != 0;
			}
			set
			{
				this.SetFlags(EhloOptionsFlags.Xrdst, value);
			}
		}

		public bool XShadow
		{
			get
			{
				return (this.flags & 32768) != 0;
			}
			set
			{
				this.SetFlags(EhloOptionsFlags.Xshadow, value);
			}
		}

		public bool XShadowRequest
		{
			get
			{
				return (this.flags & 131072) != 0;
			}
			set
			{
				this.SetFlags(EhloOptionsFlags.XshadowRequest, value);
			}
		}

		public bool XOorg
		{
			get
			{
				return (this.flags & 2048) != 0;
			}
			set
			{
				this.SetFlags(EhloOptionsFlags.Xoorg, value);
			}
		}

		public bool XProxy
		{
			get
			{
				return (this.flags & 8192) != 0;
			}
			set
			{
				this.SetFlags(EhloOptionsFlags.Xproxy, value);
			}
		}

		public bool XProxyFrom
		{
			get
			{
				return (this.flags & 65536) != 0;
			}
			set
			{
				this.SetFlags(EhloOptionsFlags.XproxyFrom, value);
			}
		}

		public bool XAdrc
		{
			get
			{
				return (this.flags & 262144) != 0;
			}
			set
			{
				this.SetFlags(EhloOptionsFlags.XAdrc, value);
			}
		}

		public bool XExprops
		{
			get
			{
				return (this.flags & 524288) != 0;
			}
			set
			{
				this.SetFlags(EhloOptionsFlags.XExProps, value);
			}
		}

		public Version XExpropsVersion
		{
			get
			{
				return this.xExpropsVersion;
			}
		}

		public bool XFastIndex
		{
			get
			{
				return (this.flags & 8388608) != 0;
			}
			set
			{
				this.SetFlags(EhloOptionsFlags.XFastIndex, value);
			}
		}

		public bool XProxyTo
		{
			get
			{
				return (this.flags & 1048576) != 0;
			}
			set
			{
				this.SetFlags(EhloOptionsFlags.XproxyTo, value);
			}
		}

		public bool XRsetProxyTo
		{
			get
			{
				return (this.flags & 67108864) != 0;
			}
			set
			{
				this.SetFlags(EhloOptionsFlags.XrsetProxyTo, value);
			}
		}

		public bool XSessionMdbGuid
		{
			get
			{
				return (this.flags & 2097152) != 0;
			}
			set
			{
				this.SetFlags(EhloOptionsFlags.XSessionMdbGuid, value);
			}
		}

		public bool XAttr
		{
			get
			{
				return (this.flags & 4194304) != 0;
			}
			set
			{
				this.SetFlags(EhloOptionsFlags.XAttr, value);
			}
		}

		public bool XSysProbe
		{
			get
			{
				return (this.flags & 16777216) != 0;
			}
			set
			{
				this.SetFlags(EhloOptionsFlags.XSysProbe, value);
			}
		}

		public bool XOrigFrom
		{
			get
			{
				return (this.flags & 268435456) != 0;
			}
			set
			{
				this.SetFlags(EhloOptionsFlags.XOrigFrom, value);
			}
		}

		public bool XSessionType
		{
			get
			{
				return (this.flags & 536870912) != 0;
			}
			set
			{
				this.SetFlags(EhloOptionsFlags.XSessionType, value);
			}
		}

		public ICollection<string> ExtendedCommands
		{
			get
			{
				return this.extendedCommands;
			}
		}

		public bool SmtpUtf8
		{
			get
			{
				return (this.flags & 134217728) != 0;
			}
			set
			{
				this.SetFlags(EhloOptionsFlags.SmtpUtf8, value);
			}
		}

		public bool AreAnyAuthMechanismsSupported()
		{
			return this.AuthenticationMechanisms.Any<string>();
		}

		public void SetFlags(EhloOptionsFlags flagsToSet, bool value)
		{
			if (value)
			{
				this.flags |= (int)flagsToSet;
				return;
			}
			this.flags &= (int)(~(int)flagsToSet);
		}

		public SmtpResponse CreateSmtpResponse(AdrcSmtpMessageContextBlob adrcSmtpMessageContextBlobInstance, ExtendedPropertiesSmtpMessageContextBlob extendedPropertiesSmtpMessageContextBlobInstance, FastIndexSmtpMessageContextBlob fastIndexSmtpMessageContextBlobInstance)
		{
			StringBuilder stringBuilder = new StringBuilder(64);
			StringBuilder stringBuilder2 = new StringBuilder(64);
			bool flag = false;
			if (this.AuthenticationMechanisms.Any<string>())
			{
				if (this.AuthenticationMechanisms.Contains("AUTH GSSAPI"))
				{
					stringBuilder.Append(" GSSAPI");
				}
				if (this.AuthenticationMechanisms.Contains("AUTH NTLM"))
				{
					stringBuilder.Append(" NTLM");
				}
				if (this.AuthenticationMechanisms.Contains("AUTH LOGIN"))
				{
					stringBuilder.Append(" LOGIN");
				}
				if (this.AuthenticationMechanisms.Contains("X-EXPS EXCHANGEAUTH"))
				{
					stringBuilder2.Append(" EXCHANGEAUTH");
					flag = true;
				}
				if (this.AuthenticationMechanisms.Contains("X-EXPS GSSAPI"))
				{
					stringBuilder2.Append(" GSSAPI");
				}
				if (this.AuthenticationMechanisms.Contains("X-EXPS NTLM"))
				{
					stringBuilder2.Append(" NTLM");
				}
				if (stringBuilder.Length > 0)
				{
					stringBuilder.Insert(0, "AUTH");
				}
				if (stringBuilder2.Length > 0)
				{
					stringBuilder2.Insert(0, "X-EXPS");
				}
			}
			string text = null;
			if (this.Size == SizeMode.Enabled)
			{
				text = string.Format(CultureInfo.InvariantCulture, "SIZE {0}", new object[]
				{
					this.MaxSize
				});
			}
			else if (this.Size == SizeMode.EnabledWithoutValue)
			{
				text = "SIZE";
			}
			StringBuilder stringBuilder3 = null;
			if (this.XAdrc && adrcSmtpMessageContextBlobInstance.AcceptBlobFromSmptIn)
			{
				stringBuilder3 = new StringBuilder();
				stringBuilder3.Append(AdrcSmtpMessageContextBlob.VersionStringWithSpace);
			}
			if (this.XExprops && extendedPropertiesSmtpMessageContextBlobInstance.AcceptBlobFromSmptIn)
			{
				if (stringBuilder3 == null)
				{
					stringBuilder3 = new StringBuilder();
				}
				stringBuilder3.Append(ExtendedPropertiesSmtpMessageContextBlob.VersionStringWithSpace);
			}
			if (this.XFastIndex && fastIndexSmtpMessageContextBlobInstance.AcceptBlobFromSmptIn)
			{
				if (stringBuilder3 == null)
				{
					stringBuilder3 = new StringBuilder();
				}
				stringBuilder3.Append(FastIndexSmtpMessageContextBlob.VersionStringWithSpace);
			}
			if (stringBuilder3 != null && stringBuilder3.Length > 0)
			{
				stringBuilder3.Insert(0, "X-MESSAGECONTEXT");
			}
			StringBuilder stringBuilder4 = null;
			if (this.XSessionMdbGuid)
			{
				stringBuilder4 = new StringBuilder();
				stringBuilder4.Append("XSESSIONPARAMS MDBGUID");
			}
			if (this.XSessionType)
			{
				stringBuilder4 = (stringBuilder4 ?? new StringBuilder());
				if (stringBuilder4.Length == 0)
				{
					stringBuilder4.Append("XSESSIONPARAMS");
				}
				stringBuilder4.Append(" TYPE");
			}
			StringBuilder stringBuilder5 = null;
			if (this.XProxyTo)
			{
				stringBuilder5 = new StringBuilder();
				stringBuilder5.Append("XPROXYTO");
				if (this.XRsetProxyTo)
				{
					stringBuilder5.AppendFormat(" {0}", "XRSETPROXYTO");
				}
			}
			return new SmtpResponse("250", string.Empty, new string[]
			{
				string.Format(CultureInfo.InvariantCulture, "{0} Hello [{1}]", new object[]
				{
					this.AdvertisedFQDN,
					this.AdvertisedIPAddress
				}),
				text,
				this.Pipelining ? "PIPELINING" : null,
				this.Dsn ? "DSN" : null,
				this.EnhancedStatusCodes ? "ENHANCEDSTATUSCODES" : null,
				this.StartTLS ? "STARTTLS" : null,
				this.AnonymousTLS ? "X-ANONYMOUSTLS" : null,
				(stringBuilder.Length > 0) ? stringBuilder.ToString() : null,
				(stringBuilder2.Length > 0) ? stringBuilder2.ToString() : null,
				flag ? "X-EXCHANGEAUTH SHA256" : null,
				this.EightBitMime ? "8BITMIME" : null,
				this.BinaryMime ? "BINARYMIME" : null,
				this.Chunking ? "CHUNKING" : null,
				this.Xexch50 ? "XEXCH50" : null,
				this.SmtpUtf8 ? "SMTPUTF8" : null,
				this.XLongAddr ? "XLONGADDR" : null,
				this.XOrar ? "XORAR" : null,
				this.XRDst ? "XRDST" : null,
				this.XShadow ? "XSHADOW" : null,
				this.XShadowRequest ? "XSHADOWREQUEST" : null,
				this.XOorg ? "XOORG" : null,
				this.XProxy ? "XPROXY" : null,
				this.XProxyFrom ? "XPROXYFROM" : null,
				(stringBuilder5 != null && stringBuilder5.Length > 0) ? stringBuilder5.ToString() : null,
				(stringBuilder3 != null && stringBuilder3.Length > 0) ? stringBuilder3.ToString() : null,
				(stringBuilder4 != null && stringBuilder4.Length > 0) ? stringBuilder4.ToString() : null,
				this.XAttr ? "XATTR" : null,
				this.XSysProbe ? "XSYSPROBE" : null,
				this.XMsgId ? "XMSGID" : null,
				this.XOrigFrom ? "XORIGFROM" : null
			});
		}

		public void ParseResponse(SmtpResponse ehloResponse, IPAddress remoteIPAddress)
		{
			this.ParseResponse(ehloResponse, remoteIPAddress, 0);
		}

		public void ParseResponse(SmtpResponse ehloResponse, IPAddress remoteIPAddress, int linesToSkip)
		{
			if (ehloResponse.StatusText == null)
			{
				return;
			}
			string[] statusText = ehloResponse.StatusText;
			int num = statusText.Length;
			if (linesToSkip > num)
			{
				throw new ArgumentException(string.Format("linesToSkip ({0}) cannot be more than total number of lines ({1})", linesToSkip, num));
			}
			for (int i = linesToSkip; i < num; i++)
			{
				bool flag = true;
				string[] advertisements = EhloOptions.GetAdvertisements(statusText[i]);
				if (advertisements != null && advertisements.Length != 0 && advertisements[0].Length != 0)
				{
					char c = advertisements[0][0];
					if (c <= 'S')
					{
						if (c <= 'E')
						{
							if (c != '8')
							{
								switch (c)
								{
								case 'A':
									goto IL_13C;
								case 'B':
									goto IL_1B6;
								case 'C':
									goto IL_191;
								case 'D':
									goto IL_1DB;
								case 'E':
									goto IL_200;
								default:
									goto IL_657;
								}
							}
							else if (advertisements[0].Equals("8bitmime", StringComparison.OrdinalIgnoreCase))
							{
								this.EightBitMime = true;
							}
							else
							{
								flag = false;
							}
						}
						else
						{
							if (c == 'P')
							{
								goto IL_225;
							}
							if (c != 'S')
							{
								goto IL_657;
							}
							goto IL_24A;
						}
					}
					else
					{
						if (c <= 'e')
						{
							if (c != 'X')
							{
								switch (c)
								{
								case 'a':
									goto IL_13C;
								case 'b':
									goto IL_1B6;
								case 'c':
									goto IL_191;
								case 'd':
									goto IL_1DB;
								case 'e':
									goto IL_200;
								default:
									goto IL_657;
								}
							}
						}
						else
						{
							if (c == 'p')
							{
								goto IL_225;
							}
							if (c == 's')
							{
								goto IL_24A;
							}
							if (c != 'x')
							{
								goto IL_657;
							}
						}
						if (advertisements[0].Equals("XEXCH50", StringComparison.OrdinalIgnoreCase))
						{
							this.Xexch50 = true;
						}
						else if (advertisements[0].Equals("XLONGADDR", StringComparison.OrdinalIgnoreCase))
						{
							this.XLongAddr = true;
						}
						else if (advertisements[0].Equals("XORAR", StringComparison.OrdinalIgnoreCase))
						{
							this.XOrar = true;
						}
						else if (advertisements[0].Equals("XRDST", StringComparison.OrdinalIgnoreCase))
						{
							this.XRDst = true;
						}
						else if (advertisements[0].Equals("XSHADOW", StringComparison.OrdinalIgnoreCase))
						{
							this.XShadow = true;
						}
						else if (advertisements[0].Equals("XSHADOWREQUEST", StringComparison.OrdinalIgnoreCase))
						{
							this.XShadowRequest = true;
						}
						else if (advertisements[0].Equals("XOORG", StringComparison.OrdinalIgnoreCase))
						{
							this.XOorg = true;
						}
						else if (advertisements[0].Equals("XPROXY", StringComparison.OrdinalIgnoreCase))
						{
							this.XProxy = true;
						}
						else if (advertisements[0].Equals("XPROXYFROM", StringComparison.OrdinalIgnoreCase))
						{
							this.XProxyFrom = true;
						}
						else if (advertisements[0].Equals("XPROXYTO", StringComparison.OrdinalIgnoreCase))
						{
							this.XProxyTo = true;
							if (advertisements.Length > 1)
							{
								if (Array.Exists<string>(advertisements, (string word) => string.Equals("XRSETPROXYTO", word, StringComparison.OrdinalIgnoreCase)))
								{
									this.XRsetProxyTo = true;
								}
							}
						}
						else if (advertisements[0].Equals("X-EXPS", StringComparison.OrdinalIgnoreCase))
						{
							for (int j = 1; j < advertisements.Length; j++)
							{
								this.AuthenticationMechanisms.Add("X-EXPS " + advertisements[j].ToUpper(CultureInfo.InvariantCulture));
							}
						}
						else if (advertisements[0].Equals("X-MESSAGECONTEXT", StringComparison.OrdinalIgnoreCase))
						{
							for (int k = 1; k < advertisements.Length; k++)
							{
								if (advertisements[k].StartsWith(AdrcSmtpMessageContextBlob.ADRCBlobName, true, CultureInfo.InvariantCulture) && !this.XAdrc)
								{
									Version version;
									this.XAdrc = AdrcSmtpMessageContextBlob.IsSupportedVersion(advertisements[k], false, out version);
								}
								else if (advertisements[k].StartsWith(ExtendedPropertiesSmtpMessageContextBlob.ExtendedPropertiesBlobName, true, CultureInfo.InvariantCulture) && !this.XExprops)
								{
									this.XExprops = ExtendedPropertiesSmtpMessageContextBlob.IsSupportedVersion(advertisements[k], out this.xExpropsVersion);
								}
								else if (advertisements[k].StartsWith(FastIndexSmtpMessageContextBlob.FastIndexBlobName, true, CultureInfo.InvariantCulture) && !this.XFastIndex)
								{
									Version version;
									this.XFastIndex = FastIndexSmtpMessageContextBlob.IsSupportedVersion(advertisements[k], out version);
								}
							}
						}
						else if (advertisements[0].Equals("XSESSIONPARAMS", StringComparison.OrdinalIgnoreCase))
						{
							for (int l = 1; l < advertisements.Length; l++)
							{
								if (advertisements[l].Equals("MDBGUID", StringComparison.OrdinalIgnoreCase))
								{
									this.XSessionMdbGuid = true;
								}
								else if (advertisements[l].Equals("TYPE", StringComparison.OrdinalIgnoreCase))
								{
									this.XSessionType = true;
								}
							}
						}
						else if (advertisements[0].Equals("X-ANONYMOUSTLS", StringComparison.OrdinalIgnoreCase))
						{
							this.AnonymousTLS = true;
						}
						else if (advertisements[0].Equals("X-EXCHANGEAUTH", StringComparison.OrdinalIgnoreCase))
						{
							for (int m = 1; m < advertisements.Length; m++)
							{
								this.exchangeAuthArgs.Add(advertisements[m]);
							}
						}
						else if (advertisements[0].Equals("XATTR", StringComparison.OrdinalIgnoreCase))
						{
							this.XAttr = true;
						}
						else if (advertisements[0].Equals("XSYSPROBE", StringComparison.OrdinalIgnoreCase))
						{
							this.XSysProbe = true;
						}
						else if (advertisements[0].Equals("XMSGID", StringComparison.OrdinalIgnoreCase))
						{
							this.XMsgId = true;
						}
						else if (advertisements[0].Equals("XORIGFROM", StringComparison.OrdinalIgnoreCase))
						{
							this.XOrigFrom = true;
						}
						else
						{
							flag = false;
						}
					}
					IL_65A:
					if (i == linesToSkip && !flag && !this.ParseDomain(advertisements))
					{
						this.AdvertisedFQDN = string.Format(CultureInfo.InvariantCulture, "[{0}]", new object[]
						{
							remoteIPAddress
						});
						goto IL_690;
					}
					goto IL_690;
					IL_13C:
					if (advertisements[0].Equals("AUTH", StringComparison.OrdinalIgnoreCase))
					{
						for (int n = 1; n < advertisements.Length; n++)
						{
							this.AuthenticationMechanisms.Add("AUTH " + advertisements[n].ToUpper(CultureInfo.InvariantCulture));
						}
						goto IL_65A;
					}
					flag = false;
					goto IL_65A;
					IL_191:
					if (advertisements[0].Equals("CHUNKING", StringComparison.OrdinalIgnoreCase))
					{
						this.Chunking = true;
						goto IL_65A;
					}
					flag = false;
					goto IL_65A;
					IL_1B6:
					if (advertisements[0].Equals("BINARYMIME", StringComparison.OrdinalIgnoreCase))
					{
						this.BinaryMime = true;
						goto IL_65A;
					}
					flag = false;
					goto IL_65A;
					IL_1DB:
					if (advertisements[0].Equals("DSN", StringComparison.OrdinalIgnoreCase))
					{
						this.Dsn = true;
						goto IL_65A;
					}
					flag = false;
					goto IL_65A;
					IL_200:
					if (advertisements[0].Equals("ENHANCEDSTATUSCODES", StringComparison.OrdinalIgnoreCase))
					{
						this.EnhancedStatusCodes = true;
						goto IL_65A;
					}
					flag = false;
					goto IL_65A;
					IL_225:
					if (advertisements[0].Equals("PIPELINING", StringComparison.OrdinalIgnoreCase))
					{
						this.Pipelining = true;
						goto IL_65A;
					}
					flag = false;
					goto IL_65A;
					IL_24A:
					if (advertisements[0].Equals("SIZE", StringComparison.OrdinalIgnoreCase))
					{
						this.Size = SizeMode.Enabled;
						if (advertisements.Length >= 2)
						{
							long.TryParse(advertisements[1], out this.maxSize);
							goto IL_65A;
						}
						goto IL_65A;
					}
					else
					{
						if (advertisements[0].Equals("SMTPUTF8", StringComparison.OrdinalIgnoreCase))
						{
							this.SmtpUtf8 = true;
							goto IL_65A;
						}
						if (advertisements[0].Equals("STARTTLS", StringComparison.OrdinalIgnoreCase))
						{
							this.StartTLS = true;
							goto IL_65A;
						}
						flag = false;
						goto IL_65A;
					}
					IL_657:
					flag = false;
					goto IL_65A;
				}
				IL_690:;
			}
		}

		public void ParseHeloResponse(SmtpResponse heloResponse)
		{
			if (heloResponse.StatusText != null)
			{
				int num = heloResponse.StatusText.Length;
				if (num > 0)
				{
					string[] advertisements = EhloOptions.GetAdvertisements(heloResponse.StatusText[0]);
					if (advertisements != null)
					{
						this.ParseDomain(advertisements);
					}
				}
			}
		}

		public EhloOptions Clone()
		{
			return new EhloOptions
			{
				advertisedFQDN = this.advertisedFQDN,
				advertisedIPAddress = this.advertisedIPAddress,
				authenticationMechanisms = new List<string>(this.authenticationMechanisms),
				maxSize = this.maxSize,
				size = this.size,
				flags = this.flags,
				extendedCommands = new List<string>(this.extendedCommands),
				exchangeAuthArgs = new List<string>(this.exchangeAuthArgs)
			};
		}

		public bool MatchForInboundProxySession(IEhloOptions other, bool proxyingBdat, out string nonMatchingOptions, out string criticalNonMatchingOptions)
		{
			StringBuilder stringBuilder = new StringBuilder();
			int num = ~other.Flags;
			int num2 = this.flags & -896772994;
			int num3 = num2 & num;
			if (num3 != 0)
			{
				stringBuilder.Append(EhloOptions.GetFlagsString(num3));
			}
			if (this.maxSize > other.MaxSize && other.MaxSize != 0L)
			{
				stringBuilder.Append("maxSize, ");
			}
			nonMatchingOptions = stringBuilder.ToString();
			int num4 = 21512;
			if (proxyingBdat)
			{
				num4 |= 8;
			}
			int num5 = this.flags & num4;
			int num6 = num5 & num;
			criticalNonMatchingOptions = EhloOptions.GetFlagsString(num6);
			return num6 == 0;
		}

		public bool MatchForClientProxySession(IEhloOptions other, out string nonCriticalNonMatchingOptions, out string criticalNonMatchingOptions)
		{
			int num = ~other.Flags;
			int num2 = this.flags & -830434050 & -5257;
			int num3 = num2 & num;
			int num4 = this.flags & 5256;
			int num5 = num4 & num;
			bool result = num5 == 0;
			StringBuilder stringBuilder = new StringBuilder(EhloOptions.GetFlagsString(num5));
			if (!other.AuthenticationMechanisms.Contains("AUTH LOGIN"))
			{
				result = false;
				EhloOptions.AppendFlagString("AUTH LOGIN", stringBuilder);
			}
			if (this.maxSize > other.MaxSize && other.MaxSize != 0L)
			{
				EhloOptions.AppendFlagString("maxSize", stringBuilder);
			}
			if (this.size != other.Size && (this.size == SizeMode.Enabled || this.size == SizeMode.EnabledWithoutValue) && other.Size == SizeMode.Disabled)
			{
				EhloOptions.AppendFlagString("size", stringBuilder);
			}
			nonCriticalNonMatchingOptions = EhloOptions.GetFlagsString(num3);
			criticalNonMatchingOptions = stringBuilder.ToString();
			return result;
		}

		private static string GetFlagsString(int flags)
		{
			StringBuilder stringBuilder = new StringBuilder();
			int num = 1;
			while (flags != 0)
			{
				string arg;
				if ((num & flags) == num && EhloOptions.flagStrings.TryGetValue(num, out arg))
				{
					stringBuilder.AppendFormat("{0}{1}", arg, ", ");
				}
				flags &= ~num;
				num <<= 1;
			}
			int length = ", ".Length;
			if (stringBuilder.Length > length)
			{
				stringBuilder.Remove(stringBuilder.Length - length, length);
			}
			return stringBuilder.ToString();
		}

		private static void AppendFlagString(string stringToAppend, StringBuilder sb)
		{
			if (sb.Length > 0)
			{
				sb.Append(", ");
			}
			sb.Append(stringToAppend);
		}

		private static string[] GetAdvertisements(string responseLine)
		{
			if (responseLine.Length < 1)
			{
				return null;
			}
			return responseLine.Split(EhloOptions.delimiters, StringSplitOptions.RemoveEmptyEntries);
		}

		private bool ParseDomain(string[] domainLineWords)
		{
			if (domainLineWords.Length < 1)
			{
				ExTraceGlobals.SmtpSendTracer.TraceError((long)this.GetHashCode(), "First line of EHLO response must be at least 5 characters long in order to contain the remote server FQDN");
				return false;
			}
			if (!RoutingAddress.IsValidDomain(domainLineWords[0]) && !RoutingAddress.IsDomainIPLiteral(domainLineWords[0]))
			{
				return false;
			}
			this.AdvertisedFQDN = domainLineWords[0];
			return true;
		}

		public const string AuthKeyword = "AUTH";

		public const string LoginKeyword = "LOGIN";

		public const string GssApiKeyword = "GSSAPI";

		public const string StartTlsKeyword = "STARTTLS";

		public const string AnonymousTlsKeyword = "X-ANONYMOUSTLS";

		public const string SessionParamsKeyword = "XSESSIONPARAMS";

		public const string XLongAddrKeyword = "XLONGADDR";

		public const string XOrarKeyword = "XORAR";

		public const string XRDstKeyword = "XRDST";

		public const string XShadowKeyword = "XSHADOW";

		public const string XShadowRequestKeyword = "XSHADOWREQUEST";

		public const string XOorgKeyword = "XOORG";

		public const string XProxyKeyword = "XPROXY";

		public const string XProxyFromKeyword = "XPROXYFROM";

		public const string XExchangeContextKeyword = "X-MESSAGECONTEXT";

		public const string XProxyToKeyword = "XPROXYTO";

		public const string XAttrKeyword = "XATTR";

		public const string XSysProbeKeyword = "XSYSPROBE";

		public const string XMsgIdKeyword = "XMSGID";

		public const string XRsetProxyToKeyword = "XRSETPROXYTO";

		public const string XOrigFromKeyword = "XORIGFROM";

		private const string FlagsStringSeparator = ", ";

		private const EhloOptionsFlags clientProxyFlagsToNotCompare = EhloOptionsFlags.AnonymousTls | EhloOptionsFlags.StartTls | EhloOptionsFlags.Xexch50 | EhloOptionsFlags.Xoorg | EhloOptionsFlags.Xproxy | EhloOptionsFlags.Xrdst | EhloOptionsFlags.XproxyFrom | EhloOptionsFlags.XshadowRequest | EhloOptionsFlags.XAdrc | EhloOptionsFlags.XExProps | EhloOptionsFlags.XproxyTo | EhloOptionsFlags.XSessionMdbGuid | EhloOptionsFlags.XAttr | EhloOptionsFlags.XSysProbe | EhloOptionsFlags.XOrigFrom | EhloOptionsFlags.XSessionType;

		private const EhloOptionsFlags clientProxyCriticalFlagsToCompare = EhloOptionsFlags.Chunking | EhloOptionsFlags.Pipelining | EhloOptionsFlags.XlongAddr | EhloOptionsFlags.Xorar;

		private const EhloOptionsFlags inboundProxyFlagsToNotCompare = EhloOptionsFlags.AnonymousTls | EhloOptionsFlags.Pipelining | EhloOptionsFlags.StartTls | EhloOptionsFlags.Xexch50 | EhloOptionsFlags.Xoorg | EhloOptionsFlags.Xproxy | EhloOptionsFlags.Xshadow | EhloOptionsFlags.XproxyFrom | EhloOptionsFlags.XshadowRequest | EhloOptionsFlags.XproxyTo | EhloOptionsFlags.XSessionMdbGuid | EhloOptionsFlags.XAttr | EhloOptionsFlags.XSysProbe | EhloOptionsFlags.XrsetProxyTo | EhloOptionsFlags.XOrigFrom | EhloOptionsFlags.XSessionType;

		private const EhloOptionsFlags inboundProxyCriticalFlagsToCompare = EhloOptionsFlags.Chunking | EhloOptionsFlags.XlongAddr | EhloOptionsFlags.Xorar | EhloOptionsFlags.Xrdst;

		private static readonly char[] delimiters = new char[]
		{
			' '
		};

		private static readonly Dictionary<int, string> flagStrings = new Dictionary<int, string>();

		private string advertisedFQDN;

		private IPAddress advertisedIPAddress;

		private SizeMode size;

		private long maxSize;

		private List<string> authenticationMechanisms = new List<string>();

		private int flags;

		private List<string> extendedCommands = new List<string>();

		private List<string> exchangeAuthArgs = new List<string>();

		private Version xExpropsVersion;
	}
}
