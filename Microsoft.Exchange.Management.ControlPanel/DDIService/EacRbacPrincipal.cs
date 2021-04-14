using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Management.ControlPanel;
using Microsoft.Exchange.PowerShell.RbacHostingTools;

namespace Microsoft.Exchange.Management.DDIService
{
	public class EacRbacPrincipal : IEacRbacPrincipal
	{
		private EacRbacPrincipal()
		{
		}

		public static IEacRbacPrincipal Instance { get; internal set; } = new EacRbacPrincipal();

		public ADObjectId ExecutingUserId
		{
			get
			{
				return RbacPrincipal.Current.ExecutingUserId;
			}
		}

		public string Name
		{
			get
			{
				return RbacPrincipal.Current.Name;
			}
		}

		public SmtpAddress ExecutingUserPrimarySmtpAddress
		{
			get
			{
				return LocalSession.Current.ExecutingUserPrimarySmtpAddress;
			}
		}

		public ExTimeZone UserTimeZone
		{
			get
			{
				return RbacPrincipal.Current.UserTimeZone;
			}
		}

		public string DateFormat
		{
			get
			{
				return RbacPrincipal.Current.DateFormat;
			}
		}

		public string TimeFormat
		{
			get
			{
				return RbacPrincipal.Current.TimeFormat;
			}
		}
	}
}
