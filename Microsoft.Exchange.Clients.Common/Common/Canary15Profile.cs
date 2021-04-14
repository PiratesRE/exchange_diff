using System;

namespace Microsoft.Exchange.Clients.Common
{
	public class Canary15Profile
	{
		public static Canary15Profile Owa
		{
			get
			{
				return Canary15Profile.owa.Value;
			}
		}

		public string Name { get; private set; }

		public string Path { get; private set; }

		public Canary15Profile(string name, string path)
		{
			this.Name = name;
			this.Path = path;
		}

		public const string OwaCanaryName = "X-OWA-CANARY";

		private static Lazy<Canary15Profile> owa = new Lazy<Canary15Profile>(() => new Canary15Profile("X-OWA-CANARY", "/"));
	}
}
