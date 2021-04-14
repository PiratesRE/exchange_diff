using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Setup.GUI
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class UMLanguagePackAlreadyInstalledException : LocalizedException
	{
		public UMLanguagePackAlreadyInstalledException(string culture) : base(Strings.UnifiedMessagingCannotRunInstall(culture))
		{
			this.culture = culture;
		}

		public UMLanguagePackAlreadyInstalledException(string culture, Exception innerException) : base(Strings.UnifiedMessagingCannotRunInstall(culture), innerException)
		{
			this.culture = culture;
		}

		protected UMLanguagePackAlreadyInstalledException(SerializationInfo info, StreamingContext context) : base(info, context)
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
