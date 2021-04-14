using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Deployment
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class UnSupportedUMLanguageException : LocalizedException
	{
		public UnSupportedUMLanguageException(string language) : base(Strings.UnSupportedUMLanguageException(language))
		{
			this.language = language;
		}

		public UnSupportedUMLanguageException(string language, Exception innerException) : base(Strings.UnSupportedUMLanguageException(language), innerException)
		{
			this.language = language;
		}

		protected UnSupportedUMLanguageException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.language = (string)info.GetValue("language", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("language", this.language);
		}

		public string Language
		{
			get
			{
				return this.language;
			}
		}

		private readonly string language;
	}
}
