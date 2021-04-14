using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using Microsoft.Exchange.Compliance.Xml;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Security.Authorization
{
	internal sealed class SerializedAccessToken : SecurityAccessToken
	{
		public SerializedAccessToken(string logonName, string authenticationType, ClientSecurityContext clientSecurityContext)
		{
			if (logonName == null)
			{
				throw new ArgumentNullException("logonName");
			}
			if (authenticationType == null)
			{
				throw new ArgumentNullException("authenticationType");
			}
			if (clientSecurityContext == null)
			{
				throw new ArgumentNullException("clientSecurityContext");
			}
			this.LogonName = logonName;
			this.AuthenticationType = authenticationType;
			clientSecurityContext.SetSecurityAccessToken(this);
		}

		public SerializedAccessToken(Stream input)
		{
			if (input == null)
			{
				throw new ArgumentNullException("input");
			}
			using (XmlTextReader xmlTextReader = SafeXmlFactory.CreateSafeXmlTextReader(input))
			{
				this.Deserialize(xmlTextReader);
			}
		}

		public static SerializedAccessToken Deserialize(string token)
		{
			if (token == null)
			{
				throw new ArgumentNullException("token");
			}
			SerializedAccessToken result;
			using (MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(token)))
			{
				result = new SerializedAccessToken(memoryStream);
			}
			return result;
		}

		public string LogonName { get; private set; }

		public string AuthenticationType { get; private set; }

		public override string ToString()
		{
			string result;
			using (StringWriter stringWriter = new StringWriter())
			{
				using (XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter))
				{
					this.Serialize(xmlTextWriter);
					result = stringWriter.ToString();
				}
			}
			return result;
		}

		public void Serialize(Stream writeStream)
		{
			using (XmlTextWriter xmlTextWriter = new XmlTextWriter(writeStream, Encoding.UTF8))
			{
				this.Serialize(xmlTextWriter);
			}
		}

		private void Serialize(XmlWriter writer)
		{
			if (string.IsNullOrEmpty(base.UserSid))
			{
				throw new InvalidOperationException();
			}
			writer.WriteStartElement("r");
			writer.WriteAttributeString("at", this.AuthenticationType);
			writer.WriteAttributeString("ln", this.LogonName);
			SerializedAccessToken.WriteSid(writer, base.UserSid, 0U, "0");
			SerializedAccessToken.WriteGroups(writer, base.GroupSids, "1");
			SerializedAccessToken.WriteGroups(writer, base.RestrictedGroupSids, "2");
			writer.WriteEndElement();
			writer.Flush();
		}

		private static void WriteSid(XmlWriter writer, string sid, uint attributes, string sidType)
		{
			writer.WriteStartElement("s");
			if (attributes != 0U)
			{
				writer.WriteAttributeString("a", attributes.ToString());
			}
			if (sidType != "0")
			{
				writer.WriteAttributeString("t", sidType);
			}
			writer.WriteString(sid);
			writer.WriteEndElement();
		}

		private static void WriteGroups(XmlWriter writer, SidStringAndAttributes[] groups, string sidType)
		{
			if (groups != null)
			{
				for (int i = 0; i < groups.Length; i++)
				{
					SerializedAccessToken.WriteSid(writer, groups[i].SecurityIdentifier, groups[i].Attributes, sidType);
				}
			}
		}

		private void Deserialize(XmlTextReader reader)
		{
			try
			{
				this.ReadRootNode(reader);
				this.ReadSidNodes(reader);
				if (base.UserSid == null)
				{
					SerializedAccessToken.ThrowParserException(reader, AuthorizationStrings.MissingUserSid);
				}
			}
			catch (XmlException innerException)
			{
				throw new SerializedAccessTokenParserException(reader.LineNumber, reader.LinePosition, AuthorizationStrings.InvalidXml, innerException);
			}
		}

		private void ReadRootNode(XmlTextReader reader)
		{
			SerializedAccessToken.ReadRootNodeElement(reader);
			this.ReadRootAttributes(reader);
			if (this.AuthenticationType == null)
			{
				SerializedAccessToken.ThrowParserException(reader, AuthorizationStrings.AuthenticationTypeIsMissing);
			}
			if (this.LogonName == null)
			{
				SerializedAccessToken.ThrowParserException(reader, AuthorizationStrings.LogonNameIsMissing);
			}
		}

		private static void ReadRootNodeElement(XmlTextReader reader)
		{
			if (!reader.Read() || XmlNodeType.Element != reader.NodeType || !StringComparer.OrdinalIgnoreCase.Equals(reader.Name, "r"))
			{
				SerializedAccessToken.ThrowParserException(reader, AuthorizationStrings.InvalidRoot);
			}
		}

		private void ReadRootAttributes(XmlTextReader reader)
		{
			if (reader.MoveToFirstAttribute())
			{
				do
				{
					if (StringComparer.OrdinalIgnoreCase.Equals(reader.Name, "at"))
					{
						this.AuthenticationType = reader.Value;
					}
					else if (StringComparer.OrdinalIgnoreCase.Equals(reader.Name, "ln"))
					{
						this.LogonName = reader.Value;
					}
					else
					{
						SerializedAccessToken.ThrowParserException(reader, AuthorizationStrings.InvalidRootAttribute(reader.Name));
					}
				}
				while (reader.MoveToNextAttribute());
			}
		}

		private void ReadSidNodes(XmlTextReader reader)
		{
			List<SidStringAndAttributes> list = new List<SidStringAndAttributes>();
			List<SidStringAndAttributes> list2 = new List<SidStringAndAttributes>();
			foreach (SerializedAccessToken.SidNode sidNode in this.EnumerateSidNodes(reader))
			{
				if (sidNode.Type == "0")
				{
					if (base.UserSid != null)
					{
						SerializedAccessToken.ThrowParserException(reader, AuthorizationStrings.MultipleUserSid);
					}
					base.UserSid = sidNode.SidStringAndAttributes.SecurityIdentifier;
				}
				else if (sidNode.Type == "1")
				{
					list.Add(sidNode.SidStringAndAttributes);
				}
				else if (sidNode.Type == "2")
				{
					list2.Add(sidNode.SidStringAndAttributes);
				}
			}
			base.GroupSids = list.ToArray();
			base.RestrictedGroupSids = list2.ToArray();
		}

		private IEnumerable<SerializedAccessToken.SidNode> EnumerateSidNodes(XmlTextReader reader)
		{
			int sidCount = 0;
			while (reader.Read() && !SerializedAccessToken.IsEndOfRootNode(reader))
			{
				yield return SerializedAccessToken.SidNode.Read(reader);
				sidCount++;
				if (sidCount > 3000)
				{
					SerializedAccessToken.ThrowParserException(reader, AuthorizationStrings.TooManySidNodes(this.LogonName, 3000));
				}
			}
			yield break;
		}

		private static bool IsEndOfRootNode(XmlTextReader reader)
		{
			return XmlNodeType.EndElement == reader.NodeType && StringComparer.OrdinalIgnoreCase.Equals(reader.Name, "r");
		}

		private static void ThrowParserException(XmlTextReader reader, LocalizedString description)
		{
			throw new SerializedAccessTokenParserException(reader.LineNumber, reader.LinePosition, description);
		}

		private const int MaximumSidsPerContext = 3000;

		private const string rootElementName = "r";

		private const string authenticationTypeAttributeName = "at";

		private const string logonNameAttributeName = "ln";

		private const string sidElementName = "s";

		private const string sidTypeAttributeName = "t";

		private const string sidAttributesAttributeName = "a";

		private struct SidType
		{
			public const string User = "0";

			public const string Group = "1";

			public const string RestrictedGroup = "2";
		}

		private struct SidNode
		{
			public SidNode(string sidType, string sidValue, uint attributes)
			{
				this.Type = sidType;
				this.SidStringAndAttributes = new SidStringAndAttributes(sidValue, attributes);
			}

			public static SerializedAccessToken.SidNode Read(XmlTextReader reader)
			{
				string text = "0";
				uint num = 0U;
				SerializedAccessToken.SidNode.CheckReaderOnSidNode(reader);
				if (reader.MoveToFirstAttribute())
				{
					do
					{
						if (StringComparer.OrdinalIgnoreCase.Equals(reader.Name, "t"))
						{
							text = SerializedAccessToken.SidNode.ParseSidType(reader);
						}
						else if (StringComparer.OrdinalIgnoreCase.Equals(reader.Name, "a"))
						{
							num = SerializedAccessToken.SidNode.ParseSidAttributes(reader);
						}
						else
						{
							SerializedAccessToken.ThrowParserException(reader, AuthorizationStrings.InvalidSidAttribute(reader.Name));
						}
					}
					while (reader.MoveToNextAttribute());
				}
				if (text == "0" && num != 0U)
				{
					SerializedAccessToken.ThrowParserException(reader, AuthorizationStrings.UserSidMustNotHaveAttributes);
				}
				string sidValue = SerializedAccessToken.SidNode.ReadSidValue(reader);
				SerializedAccessToken.SidNode.ReadEndOfSidNode(reader);
				return new SerializedAccessToken.SidNode(text, sidValue, num);
			}

			private static void CheckReaderOnSidNode(XmlTextReader reader)
			{
				if (XmlNodeType.Element != reader.NodeType || !StringComparer.OrdinalIgnoreCase.Equals(reader.Name, "s"))
				{
					SerializedAccessToken.ThrowParserException(reader, AuthorizationStrings.SidNodeExpected);
				}
			}

			private static string ParseSidType(XmlTextReader reader)
			{
				string value = reader.Value;
				if (value != "0" && value != "1" && value != "2")
				{
					SerializedAccessToken.ThrowParserException(reader, AuthorizationStrings.InvalidSidType);
				}
				return value;
			}

			private static uint ParseSidAttributes(XmlTextReader reader)
			{
				uint result;
				try
				{
					uint num = uint.Parse(reader.Value);
					if (num == 0U)
					{
						SerializedAccessToken.ThrowParserException(reader, AuthorizationStrings.InvalidAttributeValue);
					}
					result = num;
				}
				catch (FormatException innerException)
				{
					throw new SerializedAccessTokenParserException(reader.LineNumber, reader.LinePosition, AuthorizationStrings.InvalidAttributeValue, innerException);
				}
				catch (OverflowException innerException2)
				{
					throw new SerializedAccessTokenParserException(reader.LineNumber, reader.LinePosition, AuthorizationStrings.InvalidAttributeValue, innerException2);
				}
				return result;
			}

			private static string ReadSidValue(XmlTextReader reader)
			{
				if (!reader.Read() || XmlNodeType.Text != reader.NodeType || string.IsNullOrEmpty(reader.Value))
				{
					SerializedAccessToken.ThrowParserException(reader, AuthorizationStrings.ExpectingSidValue);
				}
				return reader.Value;
			}

			private static void ReadEndOfSidNode(XmlTextReader reader)
			{
				if (!reader.Read() || XmlNodeType.EndElement != reader.NodeType)
				{
					SerializedAccessToken.ThrowParserException(reader, AuthorizationStrings.ExpectingEndOfSid);
				}
			}

			public string Type;

			public SidStringAndAttributes SidStringAndAttributes;
		}
	}
}
