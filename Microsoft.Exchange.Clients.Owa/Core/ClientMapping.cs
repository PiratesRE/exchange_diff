using System;
using System.Globalization;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	internal sealed class ClientMapping
	{
		internal string Application
		{
			get
			{
				return this.application;
			}
			set
			{
				this.application = value;
			}
		}

		internal UserAgentParser.UserAgentVersion MinimumVersion
		{
			get
			{
				return this.minimumVersion;
			}
			set
			{
				this.minimumVersion = value;
			}
		}

		internal string Platform
		{
			get
			{
				return this.platform;
			}
			set
			{
				this.platform = value;
			}
		}

		internal ClientControl Control
		{
			get
			{
				return this.control;
			}
			set
			{
				this.control = value;
			}
		}

		internal Experience Experience
		{
			get
			{
				return this.experience;
			}
			set
			{
				this.experience = value;
			}
		}

		internal ClientMapping()
		{
		}

		internal ClientMapping(string application, UserAgentParser.UserAgentVersion minimumVersion, string platform, ClientControl control, Experience experience)
		{
			this.application = application;
			this.minimumVersion = minimumVersion;
			this.platform = platform;
			this.control = control;
			this.experience = experience;
		}

		internal static ClientMapping Copy(ClientMapping clientMapping)
		{
			return new ClientMapping
			{
				application = string.Copy(clientMapping.Application),
				minimumVersion = clientMapping.MinimumVersion,
				platform = clientMapping.Platform,
				control = clientMapping.Control,
				experience = Experience.Copy(clientMapping.Experience)
			};
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "Experience = ({0}), Application = {1}, Platform = {2}, Control = {3}, Minimum Version = {4}", new object[]
			{
				this.experience,
				this.application,
				this.platform,
				this.control,
				this.minimumVersion
			});
		}

		private string application = string.Empty;

		private UserAgentParser.UserAgentVersion minimumVersion;

		private string platform = string.Empty;

		private ClientControl control;

		private Experience experience;
	}
}
