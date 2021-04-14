using System;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	public class ProtocolConnectionSettings
	{
		public ProtocolConnectionSettings(string settings)
		{
			this.ParseAndValidate(settings);
		}

		public ProtocolConnectionSettings(Hostname hostname, int port, EncryptionType? encryptionType)
		{
			this.hostname = hostname;
			this.port = port;
			this.encryptionType = encryptionType;
		}

		public Hostname Hostname
		{
			get
			{
				return this.hostname;
			}
		}

		public int Port
		{
			get
			{
				return this.port;
			}
		}

		public EncryptionType? EncryptionType
		{
			get
			{
				return this.encryptionType;
			}
		}

		public static ProtocolConnectionSettings Parse(string settings)
		{
			return new ProtocolConnectionSettings(settings);
		}

		public override bool Equals(object obj)
		{
			ProtocolConnectionSettings protocolConnectionSettings = obj as ProtocolConnectionSettings;
			return protocolConnectionSettings != null && this.Equals(protocolConnectionSettings);
		}

		public bool Equals(ProtocolConnectionSettings settings)
		{
			return settings != null && (this.hostname.Equals(settings.hostname) && this.port == settings.port) && this.encryptionType == settings.encryptionType;
		}

		public override int GetHashCode()
		{
			if (this.hostname == null)
			{
				return 0;
			}
			return this.hostname.GetHashCode() ^ this.port ^ ((this.encryptionType != null) ? this.encryptionType.GetHashCode() : 0);
		}

		public override string ToString()
		{
			if (this.encryptionType != null)
			{
				return string.Format("{0}:{1}:{2}", this.hostname, this.port, this.encryptionType);
			}
			return string.Format("{0}:{1}", this.hostname, this.port);
		}

		private void ParseAndValidate(string settings)
		{
			EncryptionType? encryptionType = null;
			string[] array = settings.Split(new char[]
			{
				':'
			});
			if (array.Length < 2 || array.Length > 3)
			{
				throw new FormatException(DataStrings.ExceptionProtocolConnectionSettingsInvalidFormat(settings));
			}
			Hostname hostname;
			if (!Hostname.TryParse(array[0], out hostname))
			{
				throw new FormatException(DataStrings.ExceptionProtocolConnectionSettingsInvalidHostname(settings));
			}
			int num;
			if (!int.TryParse(array[1], out num) || num < 0 || num > 65535)
			{
				throw new FormatException(DataStrings.ExceptionProtocolConnectionSettingsInvalidPort(settings, 0, 65535));
			}
			if (array.Length > 2)
			{
				try
				{
					encryptionType = new EncryptionType?((EncryptionType)Enum.Parse(typeof(EncryptionType), array[2], true));
				}
				catch (ArgumentException)
				{
					throw new FormatException(DataStrings.ExceptionProtocolConnectionSettingsInvalidEncryptionType(settings));
				}
			}
			this.hostname = hostname;
			this.port = num;
			this.encryptionType = encryptionType;
		}

		private Hostname hostname;

		private int port;

		private EncryptionType? encryptionType;
	}
}
