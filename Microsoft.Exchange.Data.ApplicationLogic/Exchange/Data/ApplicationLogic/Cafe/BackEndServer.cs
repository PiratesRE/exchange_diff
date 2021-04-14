using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Data.ApplicationLogic.Cafe
{
	[Serializable]
	public class BackEndServer
	{
		public BackEndServer(string fqdn, int version)
		{
			if (string.IsNullOrEmpty(fqdn))
			{
				throw new ArgumentNullException("fqdn");
			}
			if (version == 0)
			{
				throw new ArgumentOutOfRangeException("version");
			}
			this.Fqdn = fqdn;
			this.Version = version;
		}

		public static BackEndServer FromString(string input)
		{
			if (string.IsNullOrEmpty(input))
			{
				throw new ArgumentNullException("input");
			}
			string[] array = input.Split(new char[]
			{
				'~'
			});
			int version;
			if (array.Length != 2 || !int.TryParse(array[1], out version) || UriHostNameType.Dns != Uri.CheckHostName(array[0]))
			{
				throw new ArgumentException("Invalid input value", "input");
			}
			return new BackEndServer(array[0], version);
		}

		public string Fqdn { get; private set; }

		public int Version { get; private set; }

		public override string ToString()
		{
			return string.Format("{0}~{1}", this.Fqdn, this.Version);
		}

		public bool IsE15OrHigher
		{
			get
			{
				return this.Version >= Server.E15MinVersion;
			}
		}
	}
}
