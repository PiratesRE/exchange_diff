using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Setup.GUI
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class LanguageBundleAlreadyInstalledException : LocalizedException
	{
		public LanguageBundleAlreadyInstalledException() : base(Strings.LanguageBundleCannotRunInstall)
		{
		}

		public LanguageBundleAlreadyInstalledException(Exception innerException) : base(Strings.LanguageBundleCannotRunInstall, innerException)
		{
		}

		protected LanguageBundleAlreadyInstalledException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
