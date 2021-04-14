using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.UM.UMCore.Exceptions;

namespace Microsoft.Exchange.UM.UMCore
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class RecognizerNotInstalledException : LocalizedException
	{
		public RecognizerNotInstalledException(string engineType, string language) : base(Strings.RecognizerNotInstalled(engineType, language))
		{
			this.engineType = engineType;
			this.language = language;
		}

		public RecognizerNotInstalledException(string engineType, string language, Exception innerException) : base(Strings.RecognizerNotInstalled(engineType, language), innerException)
		{
			this.engineType = engineType;
			this.language = language;
		}

		protected RecognizerNotInstalledException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.engineType = (string)info.GetValue("engineType", typeof(string));
			this.language = (string)info.GetValue("language", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("engineType", this.engineType);
			info.AddValue("language", this.language);
		}

		public string EngineType
		{
			get
			{
				return this.engineType;
			}
		}

		public string Language
		{
			get
			{
				return this.language;
			}
		}

		private readonly string engineType;

		private readonly string language;
	}
}
