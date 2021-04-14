using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[XmlType(TypeName = "ClientVersion")]
	[Serializable]
	public sealed class ClientVersion : XMLSerializableBase
	{
		[XmlIgnore]
		public Version Version { get; set; }

		[XmlAttribute("Version")]
		public string VersionString
		{
			get
			{
				return this.Version.ToString();
			}
			set
			{
				Version version;
				if (Version.TryParse(value, out version))
				{
					this.Version = version;
				}
			}
		}

		[XmlAttribute("ExpirationDate")]
		public DateTime ExpirationDate { get; set; }

		public static ClientVersion Parse(string clientVersionString)
		{
			if (string.IsNullOrEmpty(clientVersionString))
			{
				throw new ArgumentNullException("clientVersionString");
			}
			string[] array = clientVersionString.Split(new char[]
			{
				','
			}, StringSplitOptions.RemoveEmptyEntries);
			if (array.Length != 2)
			{
				throw new ArgumentException("clientVersionString");
			}
			ClientVersion result;
			try
			{
				Version version = Version.Parse(array[0]);
				DateTime dateTime = DateTime.Parse(array[1]);
				ClientVersion clientVersion = new ClientVersion
				{
					Version = version,
					ExpirationDate = dateTime.Date
				};
				result = clientVersion;
			}
			catch (FormatException ex)
			{
				throw new ArgumentException(string.Format("Unable to parse string {0} into a valid ClientVersion. {1}", clientVersionString, ex), ex);
			}
			return result;
		}

		public override string ToString()
		{
			return string.Format("{0},{1}", this.Version, this.ExpirationDate);
		}
	}
}
