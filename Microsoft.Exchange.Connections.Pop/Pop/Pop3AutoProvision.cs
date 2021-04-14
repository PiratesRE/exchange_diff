using System;
using System.Security;
using Microsoft.Exchange.Connections.Common;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Pop
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class Pop3AutoProvision : IAutoProvision
	{
		public Pop3AutoProvision(SmtpAddress emailAddress, SecureString password, string userLegacyDN) : this(emailAddress, password, userLegacyDN, new AutoProvisionOverrideProvider())
		{
		}

		internal Pop3AutoProvision(SmtpAddress emailAddress, SecureString password, string userLegacyDN, IAutoProvisionOverrideProvider overrideProvider)
		{
			Pop3AuthenticationMechanism[] array = new Pop3AuthenticationMechanism[2];
			array[0] = Pop3AuthenticationMechanism.Spa;
			this.authMechanisms = array;
			base..ctor();
			if (emailAddress == SmtpAddress.Empty)
			{
				throw new ArgumentException("cannot be an empty address", "emailAddress");
			}
			string domain = emailAddress.Domain;
			this.userNames = new string[]
			{
				emailAddress.ToString(),
				emailAddress.Local
			};
			bool flag;
			if (!overrideProvider.TryGetOverrides(domain, ConnectionType.Pop, out this.hostNames, out flag))
			{
				this.hostNames = new string[]
				{
					"pop." + domain,
					"mail." + domain,
					"pop3." + domain,
					domain
				};
			}
		}

		public string[] Hostnames
		{
			get
			{
				return this.hostNames;
			}
		}

		public int[] ConnectivePorts
		{
			get
			{
				return this.connectivePorts;
			}
		}

		private int[] SecurePorts
		{
			get
			{
				return this.securePorts;
			}
		}

		private int[] InsecurePorts
		{
			get
			{
				return this.insecurePorts;
			}
		}

		private string[] UserNames
		{
			get
			{
				return this.userNames;
			}
		}

		private Pop3AuthenticationMechanism[] AuthMechanisms
		{
			get
			{
				return this.authMechanisms;
			}
		}

		public static bool ValidatePopSettings(bool leaveOnServer, bool mirrored, string host, int port, string username, SecureString password, Pop3AuthenticationMechanism authentication, Pop3SecurityMechanism security, string userLegacyDN, ILog log, out LocalizedException validationException)
		{
			validationException = null;
			bool result2;
			using (IPop3Connection pop3Connection = Pop3Connection.CreateInstance(null))
			{
				Pop3ResultData result = pop3Connection.VerifyAccount();
				if (mirrored && !Pop3AutoProvision.SupportsMirroredSubscription(result))
				{
					validationException = new Pop3MirroredAccountNotPossibleException();
					result2 = false;
				}
				else if (leaveOnServer && !Pop3AutoProvision.SupportsLeaveOnServer(result))
				{
					validationException = new Pop3LeaveOnServerNotPossibleException();
					result2 = false;
				}
				else
				{
					result2 = true;
				}
			}
			return result2;
		}

		private static bool SupportsLeaveOnServer(Pop3ResultData result)
		{
			return result.UidlCommandSupported && (result.RetentionDays == null || result.RetentionDays.Value > 0);
		}

		private static bool SupportsMirroredSubscription(Pop3ResultData result)
		{
			return result.RetentionDays == null || !(result.RetentionDays != int.MaxValue);
		}

		internal static readonly int ConnectionTimeout = 20000;

		private readonly int[] connectivePorts = new int[]
		{
			995,
			110
		};

		private readonly int[] securePorts = new int[]
		{
			995,
			110
		};

		private readonly int[] insecurePorts = new int[]
		{
			110
		};

		private readonly Pop3AuthenticationMechanism[] authMechanisms;

		private readonly string[] hostNames;

		private readonly string[] userNames;
	}
}
