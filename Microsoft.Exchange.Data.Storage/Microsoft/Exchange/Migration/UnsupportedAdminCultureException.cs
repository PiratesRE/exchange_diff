using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class UnsupportedAdminCultureException : MigrationPermanentException
	{
		public UnsupportedAdminCultureException(string culture) : base(Strings.UnsupportedAdminCulture(culture))
		{
			this.culture = culture;
		}

		public UnsupportedAdminCultureException(string culture, Exception innerException) : base(Strings.UnsupportedAdminCulture(culture), innerException)
		{
			this.culture = culture;
		}

		protected UnsupportedAdminCultureException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.culture = (string)info.GetValue("culture", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("culture", this.culture);
		}

		public string Culture
		{
			get
			{
				return this.culture;
			}
		}

		private readonly string culture;
	}
}
