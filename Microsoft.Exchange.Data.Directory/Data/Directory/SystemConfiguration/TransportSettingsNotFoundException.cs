using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class TransportSettingsNotFoundException : MandatoryContainerNotFoundException
	{
		public TransportSettingsNotFoundException() : base(DirectoryStrings.TransportSettingsNotFoundException)
		{
		}

		public TransportSettingsNotFoundException(Exception innerException) : base(DirectoryStrings.TransportSettingsNotFoundException, innerException)
		{
		}

		protected TransportSettingsNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
