using System;
using System.Net;
using System.Text;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Security.Cryptography;

namespace Microsoft.Exchange.MailboxReplicationService
{
	public sealed class NetworkCredentialXML : XMLSerializableBase
	{
		[XmlElement]
		public string Username { get; set; }

		[XmlElement]
		public byte[] EncryptedPassword { get; set; }

		[XmlElement]
		public string Domain { get; set; }

		internal static NetworkCredentialXML Get(NetworkCredential nc)
		{
			if (nc == null)
			{
				return null;
			}
			NetworkCredentialXML networkCredentialXML = new NetworkCredentialXML();
			networkCredentialXML.Username = nc.UserName;
			networkCredentialXML.Domain = nc.Domain;
			if (string.IsNullOrEmpty(nc.Password))
			{
				networkCredentialXML.EncryptedPassword = null;
			}
			else
			{
				networkCredentialXML.EncryptedPassword = CryptoTools.Encrypt(Encoding.Unicode.GetBytes(nc.Password), NetworkCredentialXML.PwdEncryptionKey);
			}
			return networkCredentialXML;
		}

		internal static NetworkCredential Get(NetworkCredentialXML value)
		{
			if (value == null)
			{
				return null;
			}
			string password;
			if (value.EncryptedPassword != null)
			{
				byte[] bytes = CryptoTools.Decrypt(value.EncryptedPassword, NetworkCredentialXML.PwdEncryptionKey);
				password = Encoding.Unicode.GetString(bytes);
			}
			else
			{
				password = null;
			}
			return new NetworkCredential(value.Username, password, value.Domain);
		}

		private static readonly byte[] PwdEncryptionKey = new Guid("2bb81a8d-f5d1-48c9-99c1-bb2dda57f66d").ToByteArray();
	}
}
