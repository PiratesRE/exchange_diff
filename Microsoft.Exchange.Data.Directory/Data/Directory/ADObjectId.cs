using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Exchange.Conversion;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings;
using Microsoft.Exchange.Data.Globalization;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Directory;

namespace Microsoft.Exchange.Data.Directory
{
	[Serializable]
	public sealed class ADObjectId : ObjectId, IDnFormattable, ITraceable, INonOrgHierarchy
	{
		public ADObjectId() : this(string.Empty, Guid.Empty)
		{
		}

		public ADObjectId(AdName distinguishedName) : this((distinguishedName != null) ? distinguishedName.ToString() : string.Empty)
		{
		}

		public ADObjectId(string distinguishedName) : this(distinguishedName, Guid.Empty)
		{
		}

		public ADObjectId(Guid partitionGuid, Guid guid) : this(string.Empty, partitionGuid, guid)
		{
		}

		public ADObjectId(Guid guid, string partitionFQDN) : this(string.Empty, Guid.Empty, partitionFQDN, guid, false)
		{
		}

		public ADObjectId(Guid guid) : this(string.Empty, guid)
		{
		}

		public ADObjectId(string distinguishedName, Guid objectGuid) : this(distinguishedName, Guid.Empty, null, objectGuid, true)
		{
		}

		public ADObjectId(string distinguishedName, Guid partitionGuid, Guid objectGuid) : this(distinguishedName, partitionGuid, null, objectGuid, true)
		{
		}

		public ADObjectId(string distinguishedName, string partitionFQDN, Guid objectGuid) : this(distinguishedName, Guid.Empty, partitionFQDN, objectGuid, false)
		{
		}

		public ADObjectId(byte[] bytes) : this(bytes, Encoding.Unicode)
		{
		}

		public ADObjectId(byte[] bytes, Encoding encoding) : this(bytes, encoding, 0)
		{
		}

		private ADObjectId(string distinguishedName, Guid partitionGuid, string partitionFQDN, Guid objectGuid, bool validateDN)
		{
			this.depth = -1;
			base..ctor();
			if (string.IsNullOrEmpty(distinguishedName))
			{
				this.distinguishedName = string.Empty;
			}
			else if (validateDN)
			{
				this.distinguishedName = ADObjectId.FormatDN(distinguishedName, true, out this.depth);
			}
			else
			{
				this.distinguishedName = distinguishedName;
			}
			this.partitionGuid = partitionGuid;
			this.objectGuid = objectGuid;
			this.securityIdentifierString = null;
			if (!string.IsNullOrEmpty(partitionFQDN))
			{
				this.partitionFqdn = partitionFQDN;
			}
		}

		private ADObjectId(string dn, Guid partitionGuid, string partitionFQDN, Guid guid, int depth, OrganizationId orgId, string sid)
		{
			this.depth = -1;
			base..ctor();
			this.distinguishedName = dn;
			this.partitionGuid = partitionGuid;
			this.partitionFqdn = partitionFQDN;
			this.objectGuid = guid;
			this.depth = depth;
			this.executingUserOrganization = orgId;
			this.securityIdentifierString = sid;
		}

		public OrganizationId OrgHierarchyToIgnore
		{
			get
			{
				return this.orgHierarchyToIgnore;
			}
			set
			{
				this.orgHierarchyToIgnore = value;
				this.toStringVal = null;
			}
		}

		public static bool IsValidDistinguishedName(string distinguishedName)
		{
			if (distinguishedName == null)
			{
				return false;
			}
			string text = null;
			int num = 0;
			return ADObjectId.TryFormatDN(distinguishedName, true, out text, out num);
		}

		public static ADObjectId ParseDnOrGuid(string input)
		{
			ADObjectId result = null;
			if (input == null)
			{
				throw new ArgumentNullException("input");
			}
			if (!ADObjectId.TryParseDnOrGuid(input, out result))
			{
				throw new FormatException(DirectoryStrings.InvalidIdFormat(input));
			}
			return result;
		}

		public static bool Equals(ADObjectId x, ADObjectId y)
		{
			if (x == y)
			{
				return true;
			}
			if (x == null || y == null)
			{
				return false;
			}
			if (x.ObjectGuid == y.ObjectGuid)
			{
				return x.ObjectGuid != Guid.Empty || string.Equals(x.DistinguishedName, y.DistinguishedName, StringComparison.OrdinalIgnoreCase);
			}
			return (!(x.ObjectGuid != Guid.Empty) || !(y.ObjectGuid != Guid.Empty)) && !string.IsNullOrEmpty(x.DistinguishedName) && !string.IsNullOrEmpty(y.DistinguishedName) && string.Equals(x.DistinguishedName, y.DistinguishedName, StringComparison.OrdinalIgnoreCase);
		}

		public override byte[] GetBytes()
		{
			return this.GetBytes(Encoding.Unicode);
		}

		public byte[] GetBytes(Encoding encoding)
		{
			int byteCount = this.GetByteCount(encoding);
			byte[] array = new byte[byteCount];
			this.GetBytes(encoding, array, 0);
			return array;
		}

		public string ToDNString()
		{
			if (!string.IsNullOrEmpty(this.DistinguishedName))
			{
				return this.DistinguishedName;
			}
			if (this.objectGuid != Guid.Empty)
			{
				return ADObjectId.ToGuidString(this.objectGuid);
			}
			return string.Empty;
		}

		public string ToGuidOrDNString()
		{
			if (this.objectGuid != Guid.Empty)
			{
				return ADObjectId.ToGuidString(this.objectGuid);
			}
			if (!string.IsNullOrEmpty(this.DistinguishedName))
			{
				return this.DistinguishedName;
			}
			return string.Empty;
		}

		public string ToExtendedDN()
		{
			string text = string.Empty;
			if (this.objectGuid != Guid.Empty)
			{
				text = ADObjectId.ToGuidString(this.objectGuid);
			}
			if (!string.IsNullOrEmpty(this.DistinguishedName))
			{
				if (this.objectGuid != Guid.Empty)
				{
					text = text + ";" + this.DistinguishedName;
				}
				else
				{
					text = this.DistinguishedName;
				}
			}
			return text;
		}

		public void TraceTo(ITraceBuilder traceBuilder)
		{
			traceBuilder.AddArgument(string.Format("ADObjectId({0},{1})", string.IsNullOrEmpty(this.DistinguishedName) ? "<null/empty>" : this.DistinguishedName, (this.objectGuid == Guid.Empty) ? "<empty>" : this.objectGuid.ToString()));
		}

		public override string ToString()
		{
			if (this.toStringVal != null)
			{
				return this.toStringVal;
			}
			if (string.IsNullOrEmpty(this.DistinguishedName) && this.ObjectGuid != Guid.Empty)
			{
				return this.ObjectGuid.ToString();
			}
			if (string.IsNullOrEmpty(this.DistinguishedName))
			{
				return this.toStringVal;
			}
			string text = this.DistinguishedName.ToLower();
			OrganizationId organizationId = (this.OrgHierarchyToIgnore != null && (this.executingUserOrganization == null || OrganizationId.ForestWideOrgId.Equals(this.executingUserOrganization))) ? this.OrgHierarchyToIgnore : this.executingUserOrganization;
			if (!this.IsTenantId(this, organizationId))
			{
				if (!this.IsIdUnderConfigurationContainer(text))
				{
					this.toStringVal = this.ResolveDomainNCToString(this.DistinguishedName);
				}
				else
				{
					this.toStringVal = this.ResolveConfigNCToString(this.DistinguishedName);
					if (text.Contains(",cn=configurationunits,"))
					{
						string text2 = this.DistinguishedName.Substring(0, this.DistinguishedName.LastIndexOf("CN=ConfigurationUnits", StringComparison.OrdinalIgnoreCase) - 1);
						if (text2.Contains("CN=Configuration"))
						{
							if (text.IndexOf("CN=Configuration", StringComparison.OrdinalIgnoreCase) == 0)
							{
								this.toStringVal = string.Empty;
							}
							text2 = text2.Substring(text2.LastIndexOf("CN=Configuration", StringComparison.OrdinalIgnoreCase) + "CN=Configuration".Length);
							this.toStringVal = this.AppendHierarchicalIdentity(this.toStringVal, this.BuildHierarchicalIdentity(text2));
						}
					}
				}
			}
			else if (!this.IsIdUnderConfigurationContainer(text))
			{
				if (this.DistinguishedName.Equals(organizationId.OrganizationalUnit.DistinguishedName))
				{
					string text3 = organizationId.OrganizationalUnit.Rdn.ToString();
					this.toStringVal = AdName.Unescape(text3.Substring(text3.IndexOf('=') + 1));
				}
				else
				{
					string partialDn = this.DistinguishedName.Replace(organizationId.OrganizationalUnit.DistinguishedName, string.Empty);
					this.toStringVal = this.BuildHierarchicalIdentity(partialDn);
				}
			}
			else
			{
				string text4 = organizationId.ConfigurationUnit.DistinguishedName;
				int length = "CN=ConfigurationUnits".Length;
				int length2 = "CN=Configuration".Length;
				int num = text4.LastIndexOf("CN=ConfigurationUnits", StringComparison.OrdinalIgnoreCase);
				string text5 = "CN=First Organization" + text4.Substring(num + length);
				text5 = this.DistinguishedName.Replace(text4, text5);
				string identity = this.ResolveConfigNCToString(text5);
				string text6 = this.DistinguishedName.Replace(organizationId.ConfigurationUnit.Parent.DistinguishedName, string.Empty);
				text6 = text6.Substring(text6.IndexOf("CN=Configuration") + length2);
				this.toStringVal = this.AppendHierarchicalIdentity(identity, this.BuildHierarchicalIdentity(text6));
			}
			return this.toStringVal;
		}

		private static string ToGuidString(Guid guid)
		{
			if (guid != Guid.Empty)
			{
				return "<GUID=" + guid.ToString() + ">";
			}
			return string.Empty;
		}

		private static int GetAddressListContainerIndex(string[] splitDn)
		{
			for (int i = 0; i < splitDn.Length; i++)
			{
				if ("cn=address lists container".Equals(splitDn[i], StringComparison.OrdinalIgnoreCase))
				{
					return i;
				}
			}
			return -1;
		}

		private static string ResolveLanguageCode2ISOFormat(string languageCode)
		{
			if (string.IsNullOrEmpty(languageCode))
			{
				return null;
			}
			int lcid;
			if (int.TryParse(languageCode, out lcid))
			{
				try
				{
					CultureInfo cultureFromLcid = LocaleMap.GetCultureFromLcid(lcid);
					return cultureFromLcid.Name;
				}
				catch (ArgumentException)
				{
				}
			}
			return null;
		}

		private static bool TryFormatDN(string distinguishedName, bool validate, out string formattedDN, out int depth)
		{
			formattedDN = string.Empty;
			depth = 0;
			if (!string.IsNullOrEmpty(distinguishedName))
			{
				StringBuilder stringBuilder = null;
				int num = 0;
				int i = 0;
				while (i < distinguishedName.Length)
				{
					if (validate)
					{
						string input = distinguishedName.Substring(i);
						if (!ADObjectId.keycharRdnRegex.IsMatch(input) && !ADObjectId.oidRdnRegex.IsMatch(input))
						{
							return false;
						}
					}
					int num2 = DNConvertor.IndexOfUnescapedChar(distinguishedName, i, ',');
					if (num2 == -1)
					{
						num2 = distinguishedName.Length;
					}
					int num3 = distinguishedName.IndexOf('=', i, num2 - i);
					if (num3 < i + 1 || num3 >= num2 - 1)
					{
						return false;
					}
					num3++;
					string text;
					if (!AdName.TryConvertTo(AdName.ConvertOption.Format, distinguishedName, num3, num2 - num3, out text))
					{
						return false;
					}
					if (text != null)
					{
						if (stringBuilder == null)
						{
							stringBuilder = new StringBuilder(distinguishedName, 0, num3, distinguishedName.Length);
						}
						else
						{
							stringBuilder.Append(distinguishedName, num, num3 - num);
						}
						stringBuilder.Append(text);
						num = num2;
					}
					i = num2;
					i++;
					depth++;
				}
				if (stringBuilder != null)
				{
					if (num < distinguishedName.Length)
					{
						stringBuilder.Append(distinguishedName, num, distinguishedName.Length - num);
					}
					distinguishedName = stringBuilder.ToString();
				}
				depth--;
			}
			formattedDN = distinguishedName;
			return true;
		}

		private static string FormatDN(string distinguishedName, bool validate, out int depth)
		{
			string result = null;
			if (ADObjectId.TryFormatDN(distinguishedName, validate, out result, out depth))
			{
				return result;
			}
			throw new FormatException(DirectoryStrings.InvalidDNFormat(distinguishedName));
		}

		private static bool TryParseBytes(byte[] bytes, int offset, int length, Encoding encoding, out string formattedDN, out Guid partitionGuid, out string partitionFQDN, out Guid objectGuid, out int depth)
		{
			formattedDN = string.Empty;
			objectGuid = Guid.Empty;
			partitionGuid = Guid.Empty;
			partitionFQDN = null;
			depth = -1;
			if (bytes == null)
			{
				throw new ArgumentNullException("bytes");
			}
			if (encoding == null)
			{
				throw new ArgumentNullException("encoding");
			}
			if (offset < 0)
			{
				throw new ArgumentOutOfRangeException("offset", offset, "offset cannot be negative");
			}
			if (length < 0)
			{
				throw new ArgumentOutOfRangeException("length", length, "length cannot be negative");
			}
			if (bytes.Length < offset + length)
			{
				throw new ArgumentOutOfRangeException("bytes", "not enough bytes for provided offset/length");
			}
			if (length < 16)
			{
				return false;
			}
			objectGuid = ExBitConverter.ReadGuid(bytes, offset);
			int num = 16 + offset;
			if (length == num - offset)
			{
				return true;
			}
			if (bytes[num] != 1)
			{
				int count = length - (num - offset);
				string text = string.Empty;
				try
				{
					text = encoding.GetString(bytes, num, count);
				}
				catch (DecoderFallbackException)
				{
					return false;
				}
				catch (ArgumentException)
				{
					return false;
				}
				string text2;
				int num2;
				if (!ADObjectId.TryFormatDN(text, false, out text2, out num2))
				{
					return false;
				}
				formattedDN = text2;
				depth = num2;
				return true;
			}
			num++;
			partitionFQDN = encoding.GetString(bytes, num, length - num - offset);
			return true;
		}

		private string AppendHierarchicalIdentity(string identity, string hierarchicalId)
		{
			if (string.IsNullOrEmpty(identity))
			{
				return hierarchicalId;
			}
			if (!string.IsNullOrEmpty(hierarchicalId) && !hierarchicalId.EndsWith("\\") && !identity.StartsWith("\\"))
			{
				return hierarchicalId + "\\" + identity;
			}
			if (!string.IsNullOrEmpty(hierarchicalId))
			{
				return hierarchicalId + identity;
			}
			return identity;
		}

		internal static bool TryCreateFromBytes(byte[] bytes, int offset, int length, Encoding encoding, out ADObjectId objectId)
		{
			objectId = null;
			string text;
			Guid guid;
			string partitionFQDN;
			Guid guid2;
			int num;
			if (!ADObjectId.TryParseBytes(bytes, offset, length, encoding, out text, out guid, out partitionFQDN, out guid2, out num))
			{
				return false;
			}
			objectId = new ADObjectId(text, guid, partitionFQDN, guid2, false);
			objectId.depth = num;
			return true;
		}

		internal ADObjectId(byte[] bytes, Encoding encoding, int offset)
		{
			this.depth = -1;
			base..ctor();
			if (!ADObjectId.TryParseBytes(bytes, offset, bytes.Length - offset, encoding, out this.distinguishedName, out this.partitionGuid, out this.partitionFqdn, out this.objectGuid, out this.depth))
			{
				throw new FormatException("Unable to interpret provided bytes");
			}
		}

		private string BuildHierarchicalIdentity(string partialDn)
		{
			if (string.IsNullOrEmpty(partialDn))
			{
				return partialDn;
			}
			string[] array = DNConvertor.SplitDistinguishedName(partialDn, ',');
			StringBuilder stringBuilder = new StringBuilder(partialDn.Length);
			for (int i = 0; i < array.Length; i++)
			{
				stringBuilder.Insert(0, AdName.Unescape(array[i].Substring(array[i].IndexOf('=') + 1)));
				if (i + 1 < array.Length)
				{
					stringBuilder.Insert(0, '\\');
				}
			}
			return stringBuilder.ToString();
		}

		private bool IsTenantId(ADObjectId id, OrganizationId orgId)
		{
			return id != null && !(orgId == null) && !orgId.Equals(OrganizationId.ForestWideOrgId) && (id.IsDescendantOf(orgId.OrganizationalUnit) || id.IsDescendantOf(orgId.ConfigurationUnit.Parent) || id.DistinguishedName.ToLower().Contains(",cn=configurationunits,"));
		}

		private string ResolveConfigNCToString(string originalDN)
		{
			StringBuilder stringBuilder = new StringBuilder(originalDN.Length);
			string[] array = DNConvertor.SplitDistinguishedName(originalDN, ',');
			string text = originalDN.ToLower();
			stringBuilder.Append((array.Length > 0) ? AdName.Unescape(array[0].Substring(array[0].IndexOf('=') + 1)) : originalDN);
			if (array.Length < 5)
			{
				return stringBuilder.ToString();
			}
			string text2 = AdName.Unescape(array[1].Substring(array[1].IndexOf('=') + 1));
			string text3 = AdName.Unescape(array[2].Substring(array[2].IndexOf('=') + 1));
			string value = AdName.Unescape(array[3].Substring(array[3].IndexOf('=') + 1));
			string value2 = AdName.Unescape(array[4].Substring(array[4].IndexOf('=') + 1));
			if (text.Contains("cn=servers,") && text.Contains("cn=administrative groups,") && !text.Contains("cn=address lists container"))
			{
				if ("informationstore".Equals(text2, StringComparison.OrdinalIgnoreCase) && "servers".Equals(value, StringComparison.OrdinalIgnoreCase))
				{
					stringBuilder.Insert(0, "\\");
					stringBuilder.Insert(0, text3);
				}
				else if ("informationstore".Equals(text3, StringComparison.OrdinalIgnoreCase) && "servers".Equals(value2, StringComparison.OrdinalIgnoreCase))
				{
					stringBuilder.Insert(0, "\\");
					stringBuilder.Insert(0, text2);
					stringBuilder.Insert(0, "\\");
					stringBuilder.Insert(0, value);
				}
				else if ("protocols".Equals(text3, StringComparison.OrdinalIgnoreCase) && "servers".Equals(value2, StringComparison.OrdinalIgnoreCase))
				{
					stringBuilder.Insert(0, "\\");
					stringBuilder.Insert(0, value);
				}
			}
			if ("databases".Equals(text3, StringComparison.OrdinalIgnoreCase))
			{
				stringBuilder.Insert(0, "\\");
				stringBuilder.Insert(0, text2);
			}
			else if (text.Contains("cn=address lists container"))
			{
				int addressListContainerIndex = ADObjectId.GetAddressListContainerIndex(array);
				if (0 < addressListContainerIndex)
				{
					StringBuilder stringBuilder2 = new StringBuilder("\\");
					for (int i = addressListContainerIndex - 2; i >= 0; i--)
					{
						stringBuilder2.Append(AdName.Unescape(array[i].Substring(array[i].IndexOf('=') + 1)));
						if (i != 0)
						{
							stringBuilder2.Append("\\");
						}
					}
					return stringBuilder2.ToString();
				}
				return originalDN;
			}
			else if ("message classifications".Equals(text3, StringComparison.OrdinalIgnoreCase) && "transport settings".Equals(value, StringComparison.OrdinalIgnoreCase))
			{
				stringBuilder.Insert(0, "\\");
				stringBuilder.Insert(0, text2);
			}
			else if ("dsn customization".Equals(value, StringComparison.OrdinalIgnoreCase) && "transport settings".Equals(value2, StringComparison.OrdinalIgnoreCase))
			{
				string value3 = ADObjectId.ResolveLanguageCode2ISOFormat(text3);
				if (string.IsNullOrEmpty(value3))
				{
					return originalDN;
				}
				stringBuilder.Insert(0, "\\");
				stringBuilder.Insert(0, text2);
				stringBuilder.Insert(0, "\\");
				stringBuilder.Insert(0, value3);
			}
			else if ("dsn customization".Equals(text3, StringComparison.OrdinalIgnoreCase) && "transport settings".Equals(value, StringComparison.OrdinalIgnoreCase))
			{
				string value4 = ADObjectId.ResolveLanguageCode2ISOFormat(text2);
				if (string.IsNullOrEmpty(value4))
				{
					return originalDN;
				}
				stringBuilder.Insert(0, "\\");
				stringBuilder.Insert(0, value4);
			}
			else if ("Display-Templates".Equals(text3, StringComparison.OrdinalIgnoreCase))
			{
				Culture culture = null;
				if (!Culture.TryGetCulture(int.Parse(text2, NumberStyles.HexNumber), out culture))
				{
					return originalDN;
				}
				stringBuilder = new StringBuilder(originalDN.Length);
				stringBuilder.Insert(0, this.InternalGetDetailsTemplateName(array[0]));
				stringBuilder.Insert(0, "\\");
				stringBuilder.Insert(0, culture.Name);
			}
			return stringBuilder.ToString();
		}

		private bool IsIdUnderConfigurationContainer(string dnLower)
		{
			return dnLower.Contains("cn=microsoft exchange,cn=services,cn=configuration,dc=") || dnLower.Contains("cn=microsoft exchange,cn=services,cn=configuration,ou=") || dnLower.Contains(",cn=configurationunits,dc=") || this.InternalIsUnderAdamConfigurationContainer(dnLower);
		}

		internal static ADObjectId ParseExtendedDN(string extendedDN)
		{
			return ADObjectId.ParseExtendedDN(extendedDN, Guid.Empty, null);
		}

		internal static ADObjectId ParseExtendedDN(string extendedDN, OrganizationId orgId)
		{
			return ADObjectId.ParseExtendedDN(extendedDN, Guid.Empty, orgId);
		}

		internal static ADObjectId ParseExtendedDN(string extendedDN, Guid partitionGuid, OrganizationId orgId)
		{
			string sid = null;
			Guid guid = Guid.Empty;
			int num = -1;
			if (string.IsNullOrEmpty(extendedDN) || !extendedDN.StartsWith("<GUID=", StringComparison.Ordinal))
			{
				guid = Guid.Empty;
			}
			else
			{
				guid = GuidFactory.Parse(extendedDN, "<GUID=".Length);
				int num2 = "<GUID=".Length + "098f2470-bae0-11cd-b579-08002b30bfeb".Length + 2;
				if (extendedDN.Length > num2)
				{
					int num3 = num2;
					if (extendedDN[num2] == '<' && extendedDN.Substring(num2, "<SID=".Length) == "<SID=")
					{
						int num4 = num2 + "<SID=".Length;
						int num5 = extendedDN.IndexOf(';', num2) - 2;
						sid = extendedDN.Substring(num4, num5 - num4 + 1);
						num3 = extendedDN.IndexOf(';', num2) + 1;
					}
					if (extendedDN.Length > num3)
					{
						extendedDN = extendedDN.Substring(num3);
					}
				}
				else
				{
					extendedDN = string.Empty;
				}
			}
			string dn;
			if (string.IsNullOrEmpty(extendedDN))
			{
				dn = string.Empty;
			}
			else
			{
				dn = ADObjectId.FormatDN(extendedDN, false, out num);
			}
			ExTraceGlobals.ADObjectTracer.TraceDebug<string>(0L, "ADObjectId.ParseExtendedDN - Initialized using DN {0}", extendedDN);
			return new ADObjectId(dn, partitionGuid, null, guid, num, orgId, sid);
		}

		internal static SecurityIdentifier GetSecurityIdentifier(ADObjectId instance)
		{
			if (instance.securityIdentifierString == null)
			{
				return null;
			}
			return new SecurityIdentifier(instance.securityIdentifierString);
		}

		internal static bool TryParseDnOrGuid(string input, out ADObjectId instance)
		{
			instance = null;
			if (input == null || input.Length < 3)
			{
				return false;
			}
			string text = null;
			int num = 0;
			if (ADObjectId.TryFormatDN(input, true, out text, out num))
			{
				instance = new ADObjectId(text, Guid.Empty, null, Guid.Empty, false);
				return true;
			}
			Guid guid;
			if (GuidHelper.TryParseGuid(input, out guid))
			{
				instance = new ADObjectId(string.Empty, Guid.Empty, null, guid, false);
				return true;
			}
			return false;
		}

		public ADObjectId GetChildId(string unescapedCommonName)
		{
			return this.GetChildId("CN", unescapedCommonName);
		}

		public ADObjectId GetChildId(string prefix, string unescapedCommonName)
		{
			if (string.IsNullOrEmpty(unescapedCommonName))
			{
				throw new ArgumentNullException(DirectoryStrings.CannotGetChild(" unescapedCommonName is null/empty").ToString());
			}
			if (string.IsNullOrEmpty(prefix))
			{
				throw new ArgumentNullException(DirectoryStrings.CannotGetChild("prefix is null/empty").ToString());
			}
			if (this.Rdn == null)
			{
				throw new ArgumentNullException(DirectoryStrings.CannotGetChild("this is a GUID based ADObjectId").ToString());
			}
			AdName adName = new AdName(prefix, unescapedCommonName);
			return new ADObjectId(adName.ToString() + "," + this.DistinguishedName, Guid.Empty, this.partitionFqdn, Guid.Empty, false)
			{
				currentRdn = adName,
				parent = this,
				domainInfo = this.domainInfo
			};
		}

		public ADObjectId GetDescendantId(string unescapedChildName, string unescapedGrandChildName, params string[] unescapedDescendants)
		{
			ADObjectId childId = this.GetChildId(unescapedChildName).GetChildId(unescapedGrandChildName);
			foreach (string unescapedCommonName in unescapedDescendants)
			{
				childId = childId.GetChildId(unescapedCommonName);
			}
			return childId;
		}

		public ADObjectId GetDescendantId(ADObjectId relativePath)
		{
			if (string.IsNullOrEmpty(this.DistinguishedName))
			{
				return null;
			}
			if (relativePath == null)
			{
				return this;
			}
			if (string.IsNullOrEmpty(relativePath.DistinguishedName))
			{
				return null;
			}
			return new ADObjectId(relativePath.DistinguishedName + "," + this.DistinguishedName, Guid.Empty, this.partitionFqdn, Guid.Empty, false)
			{
				domainInfo = this.domainInfo
			};
		}

		public AdName GetAdNameAtDepth(int depth)
		{
			if (string.IsNullOrEmpty(this.DistinguishedName))
			{
				throw new InvalidOperationException(DirectoryStrings.CannotGetDnFromGuid(this.objectGuid));
			}
			if (depth < 0 || depth > this.Depth)
			{
				throw new InvalidOperationException(DirectoryStrings.CannotGetDnAtDepth(this.DistinguishedName, depth));
			}
			ADObjectId adobjectId = this;
			for (int i = 0; i < this.Depth - depth; i++)
			{
				adobjectId = adobjectId.Parent;
			}
			return adobjectId.Rdn;
		}

		public ADObjectId DescendantDN(int depth)
		{
			if (depth < 0)
			{
				throw new InvalidOperationException(DirectoryStrings.CannotGetDnAtDepth(this.DistinguishedName, depth));
			}
			int num;
			if (this.DomainId == null)
			{
				num = this.Depth;
			}
			else
			{
				num = this.Depth - this.DomainId.Depth;
			}
			num -= depth;
			if (num > this.Depth || num < 0)
			{
				throw new InvalidOperationException(DirectoryStrings.CannotGetDnAtDepth(this.DistinguishedName, num));
			}
			ADObjectId adobjectId = this;
			for (int i = 0; i < num; i++)
			{
				adobjectId = adobjectId.Parent;
			}
			return adobjectId;
		}

		public ADObjectId AncestorDN(int generation)
		{
			ADObjectId adobjectId = this;
			int num = generation;
			while (adobjectId != null && num > 0)
			{
				num--;
				adobjectId = adobjectId.Parent;
			}
			if (adobjectId == null || num != 0)
			{
				throw new InvalidOperationException(DirectoryStrings.CannotGetDnAtDepth(this.DistinguishedName, generation));
			}
			return adobjectId;
		}

		public bool IsDescendantOf(ADObjectId rootId)
		{
			return rootId != null && !string.IsNullOrEmpty(rootId.DistinguishedName) && !string.IsNullOrEmpty(this.DistinguishedName) && this.DistinguishedName.EndsWith(rootId.DistinguishedName, StringComparison.OrdinalIgnoreCase);
		}

		public ADObjectId GetFirstGenerationDecendantOf(ADObjectId rootId)
		{
			if (rootId == null)
			{
				throw new ArgumentNullException("rootId");
			}
			ADObjectId adobjectId = this;
			int i = 0;
			while (i < 256)
			{
				ADObjectId adobjectId2 = adobjectId.Parent;
				i++;
				if (adobjectId2 == null)
				{
					break;
				}
				if (adobjectId2.DistinguishedName.Equals(rootId.DistinguishedName, StringComparison.OrdinalIgnoreCase))
				{
					return adobjectId;
				}
				adobjectId = adobjectId2;
			}
			throw new InvalidOperationException(DirectoryStrings.CannotGetDnAtDepth(this.DistinguishedName, i));
		}

		public bool IsDeleted
		{
			get
			{
				AdName rdn = this.Rdn;
				return !(rdn == null) && rdn.UnescapedName.Contains("\nDEL:");
			}
		}

		public AdName Rdn
		{
			get
			{
				if (null == this.currentRdn && !string.IsNullOrEmpty(this.DistinguishedName))
				{
					int num = DNConvertor.IndexOfUnescapedChar(this.DistinguishedName, 0, ',');
					if (num == -1)
					{
						this.currentRdn = AdName.ParseRdn(this.DistinguishedName, 0, this.DistinguishedName.Length, true);
					}
					else
					{
						this.currentRdn = AdName.ParseRdn(this.DistinguishedName, 0, num, true);
					}
				}
				return this.currentRdn;
			}
		}

		public ADObjectId Parent
		{
			get
			{
				if (this.parent == null && this.Depth > 0)
				{
					int num = DNConvertor.IndexOfUnescapedChar(this.DistinguishedName, 0, ',');
					this.parent = new ADObjectId(this.DistinguishedName.Substring(num + 1), Guid.Empty, null, Guid.Empty, false);
					if (this.depth != -1)
					{
						this.parent.depth = this.depth - 1;
					}
				}
				return this.parent;
			}
		}

		public int Depth
		{
			get
			{
				if (this.depth == -1)
				{
					int num = 0;
					if (!string.IsNullOrEmpty(this.DistinguishedName))
					{
						if (this.parent != null)
						{
							num = this.parent.Depth + 1;
						}
						else
						{
							int num2 = -1;
							while ((num2 = DNConvertor.IndexOfUnescapedChar(this.DistinguishedName, num2 + 1, ',')) != -1)
							{
								num++;
							}
						}
					}
					this.depth = num;
				}
				return this.depth;
			}
		}

		public string DistinguishedName
		{
			get
			{
				return this.distinguishedName;
			}
		}

		private bool InitializeDomainInfo()
		{
			if (this.domainInfo != null)
			{
				return !this.domainInfo.IsRelativeDn;
			}
			if (string.IsNullOrEmpty(this.DistinguishedName))
			{
				this.domainInfo = ADObjectId.DomainInfo.NullDomainFalseRelative;
				return true;
			}
			ADObjectId adobjectId = this;
			while (adobjectId.parent != null)
			{
				if (adobjectId.DistinguishedName.StartsWith("DC=", StringComparison.OrdinalIgnoreCase))
				{
					this.domainInfo = ADObjectId.DomainInfo.GetDomainInfo(adobjectId, false);
					return true;
				}
				adobjectId = adobjectId.parent;
			}
			int num = adobjectId.DistinguishedName.IndexOf("DC=", 0, adobjectId.DistinguishedName.Length, StringComparison.OrdinalIgnoreCase);
			if (num == -1)
			{
				num = adobjectId.DistinguishedName.LastIndexOf("CN={", StringComparison.OrdinalIgnoreCase);
				if (num == -1)
				{
					num = adobjectId.DistinguishedName.LastIndexOf("OU=MSExchangeGateway", StringComparison.OrdinalIgnoreCase);
					if (num == -1)
					{
						this.domainInfo = ADObjectId.DomainInfo.NullDomainTrueRelative;
						return false;
					}
				}
			}
			this.domainInfo = ADObjectId.DomainInfo.GetDomainInfo(new ADObjectId(adobjectId.DistinguishedName.Substring(num), Guid.Empty, null, Guid.Empty, false), false);
			return true;
		}

		public bool IsRelativeDn
		{
			get
			{
				if (this.domainInfo == null)
				{
					this.InitializeDomainInfo();
				}
				return this.domainInfo.IsRelativeDn;
			}
		}

		public ADObjectId DomainId
		{
			get
			{
				if (this.domainInfo == null && !this.InitializeDomainInfo())
				{
					throw new InvalidOperationException(DirectoryStrings.CannotGetDomainFromDN(this.DistinguishedName));
				}
				return this.domainInfo.DomainId;
			}
		}

		public Guid PartitionGuid
		{
			get
			{
				return this.partitionGuid;
			}
		}

		public string PartitionFQDN
		{
			get
			{
				if (this.partitionFqdn == null)
				{
					if (this.domainInfo == null)
					{
						this.InitializeDomainInfo();
					}
					this.partitionFqdn = ((this.domainInfo.DomainId != null) ? this.domainInfo.DomainId.ToString() : string.Empty);
				}
				return this.partitionFqdn;
			}
		}

		public Guid ObjectGuid
		{
			get
			{
				return this.objectGuid;
			}
		}

		public string Name
		{
			get
			{
				if (!(this.Rdn != null))
				{
					return string.Empty;
				}
				return this.Rdn.UnescapedName;
			}
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			ADObjectId adobjectId = obj as ADObjectId;
			if (adobjectId != null)
			{
				return this.Equals(adobjectId);
			}
			string text = obj as string;
			return text != null && this.Equals(text);
		}

		public bool Equals(string objString)
		{
			if (string.IsNullOrEmpty(objString))
			{
				return false;
			}
			ADObjectId y = null;
			if (ADObjectId.TryParseDnOrGuid(objString, out y))
			{
				return ADObjectId.Equals(this, y);
			}
			return this.ToString().Equals(objString);
		}

		public bool Equals(ADObjectId id)
		{
			if (id == null)
			{
				return false;
			}
			bool result;
			if (!this.ObjectGuid.Equals(Guid.Empty) && !id.ObjectGuid.Equals(Guid.Empty))
			{
				result = this.ObjectGuid.Equals(id.ObjectGuid);
			}
			else if (this.DistinguishedName == null)
			{
				result = (id.DistinguishedName == null);
			}
			else
			{
				result = this.DistinguishedName.Equals(id.DistinguishedName, StringComparison.OrdinalIgnoreCase);
			}
			return result;
		}

		public override int GetHashCode()
		{
			int hashCode = this.ObjectGuid.GetHashCode();
			if (this.ObjectGuid.Equals(Guid.Empty) && !string.IsNullOrEmpty(this.DistinguishedName))
			{
				hashCode = this.DistinguishedName.ToLowerInvariant().GetHashCode();
			}
			return hashCode;
		}

		internal static bool IsNullOrEmpty(ADObjectId adobjectId)
		{
			return adobjectId == null || (string.IsNullOrEmpty(adobjectId.DistinguishedName) && adobjectId.ObjectGuid == Guid.Empty);
		}

		internal PartitionId GetPartitionId()
		{
			return new PartitionId(this);
		}

		public string ToCanonicalName()
		{
			if (!string.IsNullOrEmpty(this.DistinguishedName))
			{
				return NativeHelpers.CanonicalNameFromDistinguishedName(this.DistinguishedName);
			}
			return string.Empty;
		}

		public int GetByteCount(Encoding encoding)
		{
			if (encoding == null)
			{
				throw new ArgumentNullException("encoding");
			}
			int num = 16;
			if (!string.IsNullOrEmpty(this.DistinguishedName))
			{
				num += encoding.GetByteCount(this.DistinguishedName);
			}
			else if (!string.IsNullOrEmpty(this.PartitionFQDN) && ConfigBase<AdDriverConfigSchema>.GetConfig<int>("SoftLinkFormatVersion") == 2)
			{
				num += encoding.GetByteCount(this.PartitionFQDN) + 1;
			}
			return num;
		}

		internal void GetBytes(Encoding encoding, byte[] byteArray, int offset)
		{
			offset += ExBitConverter.Write(this.objectGuid, byteArray, offset);
			if (!string.IsNullOrEmpty(this.DistinguishedName))
			{
				encoding.GetBytes(this.DistinguishedName, 0, this.DistinguishedName.Length, byteArray, offset);
				return;
			}
			if (!string.IsNullOrEmpty(this.PartitionFQDN) && ConfigBase<AdDriverConfigSchema>.GetConfig<int>("SoftLinkFormatVersion") == 2)
			{
				byteArray[offset++] = 1;
				encoding.GetBytes(this.PartitionFQDN, 0, this.PartitionFQDN.Length, byteArray, offset);
			}
		}

		public byte[] ToSoftLinkValue()
		{
			byte b = (byte)ConfigBase<AdDriverConfigSchema>.GetConfig<int>("SoftLinkFormatVersion");
			if (this.objectGuid == Guid.Empty || (b == 2 && string.IsNullOrEmpty(this.PartitionFQDN)))
			{
				throw new FormatException(DirectoryStrings.InvalidIdFormat(this.ToExtendedDN()));
			}
			if (b == 1)
			{
				Guid resourcePartitionGuid = this.partitionGuid;
				if (this.partitionGuid == Guid.Empty)
				{
					resourcePartitionGuid = ADObjectId.ResourcePartitionGuid;
				}
				byte[] array = new byte[33];
				array[0] = b;
				byte[] sourceArray = this.objectGuid.ToByteArray();
				Array.Copy(sourceArray, 0, array, 1, 16);
				sourceArray = resourcePartitionGuid.ToByteArray();
				Array.Copy(sourceArray, 0, array, 17, 16);
				return array;
			}
			if (b == 2)
			{
				int num = 16 + Encoding.UTF8.GetByteCount(this.PartitionFQDN) + 1;
				byte[] array2 = new byte[num];
				array2[0] = b;
				byte[] array3 = this.objectGuid.ToByteArray();
				Array.Copy(array3, 0, array2, 1, 16);
				array3 = Encoding.UTF8.GetBytes(this.PartitionFQDN);
				Array.Copy(array3, 0, array2, 17, array3.Length);
				return array2;
			}
			throw new FormatException(string.Format("Invalid soft link format version: {0}", b));
		}

		internal byte[] ToSoftLinkLdapQueryValue(byte prefix)
		{
			if (this.objectGuid == Guid.Empty)
			{
				throw new FormatException(DirectoryStrings.InvalidIdFormat(this.ToExtendedDN()));
			}
			if (prefix != 1 && prefix != 2)
			{
				throw new ArgumentException(string.Format("Invalid soft link format version: {0}", prefix), "prefix");
			}
			byte[] array = new byte[17];
			array[0] = prefix;
			byte[] sourceArray = this.objectGuid.ToByteArray();
			Array.Copy(sourceArray, 0, array, 1, 16);
			return array;
		}

		internal static ADObjectId FromSoftLinkValue(byte[] input, ADObjectId objectId, OrganizationId executingUserOrgId)
		{
			Guid empty = Guid.Empty;
			Guid empty2 = Guid.Empty;
			byte b = input[0];
			if ((b != 1 && b != 2) || (b == 1 && input.Length != 33) || (b == 2 && input.Length < 38))
			{
				throw new FormatException(DirectoryStrings.InvalidIdFormat(input.ToString()));
			}
			byte[] array = new byte[16];
			Array.Copy(input, 1, array, 0, 16);
			empty = new Guid(array);
			string partitionFQDN;
			if (b == 1)
			{
				Array.Copy(input, 17, array, 0, 16);
				empty2 = new Guid(array);
				if (objectId != null)
				{
					partitionFQDN = ADResourceForestLocator.InferResourceForestFromAccountForestIdentity(objectId);
				}
				else
				{
					partitionFQDN = PartitionId.LocalForest.ForestFQDN;
				}
			}
			else
			{
				partitionFQDN = Encoding.UTF8.GetString(input, 17, input.Length - 17);
			}
			return new ADObjectId(string.Empty, empty2, partitionFQDN, empty, 0, executingUserOrgId, null);
		}

		private string ResolveDomainNCToString(string originalDn)
		{
			try
			{
				return NativeHelpers.CanonicalNameFromDistinguishedName(originalDn);
			}
			catch (NameConversionException)
			{
			}
			return originalDn;
		}

		private string InternalGetDetailsTemplateName(string detailsTemplateId)
		{
			return DetailsTemplate.TranslateTemplateIDToName(AdName.Unescape(detailsTemplateId.Substring(detailsTemplateId.IndexOf('=') + 1)));
		}

		private bool InternalIsUnderAdamConfigurationContainer(string dnLower)
		{
			bool result = false;
			if (ADSession.IsBoundToAdam && dnLower.LastIndexOf("{") > 0)
			{
				int num = dnLower.LastIndexOf("{") - "cn=microsoft exchange,cn=services,cn=configuration,cn=".Length;
				if (num > 0)
				{
					result = dnLower.Substring(num).StartsWith("cn=microsoft exchange,cn=services,cn=configuration,cn={");
				}
			}
			return result;
		}

		internal const int MaxRdnLength = 64;

		private const string GuidDNPrefix = "<GUID=";

		private const string SIDPrefix = "<SID=";

		private const string GuidStringRepresentation = "098f2470-bae0-11cd-b579-08002b30bfeb";

		private const int BytesForGuid = 16;

		private const byte SoftLinkValueLengthV1 = 33;

		private const byte MinSoftLinkValueLengthV2 = 38;

		private readonly Guid partitionGuid;

		private string partitionFqdn;

		private Guid objectGuid;

		private string distinguishedName;

		private string securityIdentifierString;

		private int depth;

		private string toStringVal;

		private OrganizationId executingUserOrganization;

		[NonSerialized]
		private ADObjectId parent;

		[NonSerialized]
		private ADObjectId.DomainInfo domainInfo;

		[NonSerialized]
		private AdName currentRdn;

		[NonSerialized]
		private OrganizationId orgHierarchyToIgnore;

		public static Guid ResourcePartitionGuid = new Guid("59ce2f71-eaa2-4ddf-a4fa-f25069d0b324");

		private static readonly Regex keycharRdnRegex = new Regex("^[a-zA-Z][a-zA-Z0-9\\-]*=", RegexOptions.Compiled | RegexOptions.CultureInvariant);

		private static readonly Regex oidRdnRegex = new Regex("^[0-9]+(\\.[0-9]+)*=", RegexOptions.Compiled | RegexOptions.CultureInvariant);

		private class DomainInfo
		{
			private DomainInfo(ADObjectId domainId, bool isRelativeDn)
			{
				this.domainId = domainId;
				this.isRelativeDn = isRelativeDn;
			}

			public ADObjectId DomainId
			{
				get
				{
					return this.domainId;
				}
			}

			public bool IsRelativeDn
			{
				get
				{
					return this.isRelativeDn;
				}
			}

			public static ADObjectId.DomainInfo GetDomainInfo(ADObjectId domainId, bool isRelativeDn)
			{
				ADObjectId.DomainInfo domainInfo;
				if (ADObjectId.DomainInfo.domains != null)
				{
					domainInfo = ADObjectId.DomainInfo.GetDomainInfoFromList(domainId, isRelativeDn);
					if (domainInfo != null)
					{
						return domainInfo;
					}
					lock (ADObjectId.DomainInfo.domainLock)
					{
						domainInfo = ADObjectId.DomainInfo.GetDomainInfoFromList(domainId, isRelativeDn);
						if (domainInfo == null)
						{
							domainInfo = new ADObjectId.DomainInfo(domainId, isRelativeDn);
							ADObjectId.DomainInfo.domains = new List<ADObjectId.DomainInfo>(ADObjectId.DomainInfo.domains)
							{
								domainInfo
							};
						}
						return domainInfo;
					}
				}
				domainInfo = new ADObjectId.DomainInfo(domainId, isRelativeDn);
				return domainInfo;
			}

			private static ADObjectId.DomainInfo GetDomainInfoFromList(ADObjectId domainId, bool isRelativeDn)
			{
				foreach (ADObjectId.DomainInfo domainInfo in ADObjectId.DomainInfo.domains)
				{
					if (domainInfo.isRelativeDn.Equals(isRelativeDn) && ADObjectIdEqualityComparer.Instance.Equals(domainInfo.domainId, domainId))
					{
						return domainInfo;
					}
				}
				return null;
			}

			static DomainInfo()
			{
				if (Globals.IsDatacenter)
				{
					ADObjectId.DomainInfo.domains = new List<ADObjectId.DomainInfo>();
				}
			}

			private static List<ADObjectId.DomainInfo> domains;

			private static object domainLock = new object();

			public static ADObjectId.DomainInfo NullDomainTrueRelative = new ADObjectId.DomainInfo(null, true);

			public static ADObjectId.DomainInfo NullDomainFalseRelative = new ADObjectId.DomainInfo(null, false);

			private ADObjectId domainId;

			private bool isRelativeDn;
		}
	}
}
