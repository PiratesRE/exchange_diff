using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.PolicyNudges
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class NewPolicyTipConfigInvalidLocaleException : LocalizedException
	{
		public NewPolicyTipConfigInvalidLocaleException(string locales) : base(Strings.NewPolicyTipConfigInvalidLocale(locales))
		{
			this.locales = locales;
		}

		public NewPolicyTipConfigInvalidLocaleException(string locales, Exception innerException) : base(Strings.NewPolicyTipConfigInvalidLocale(locales), innerException)
		{
			this.locales = locales;
		}

		protected NewPolicyTipConfigInvalidLocaleException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.locales = (string)info.GetValue("locales", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("locales", this.locales);
		}

		public string Locales
		{
			get
			{
				return this.locales;
			}
		}

		private readonly string locales;
	}
}
