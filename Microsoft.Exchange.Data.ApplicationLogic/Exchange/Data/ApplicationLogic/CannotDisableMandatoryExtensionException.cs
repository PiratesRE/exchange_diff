using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.ApplicationLogic.Extension;

namespace Microsoft.Exchange.Data.ApplicationLogic
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class CannotDisableMandatoryExtensionException : OwaExtensionOperationException
	{
		public CannotDisableMandatoryExtensionException() : base(Strings.ErrorCannotDisableMandatoryExtension)
		{
		}

		public CannotDisableMandatoryExtensionException(Exception innerException) : base(Strings.ErrorCannotDisableMandatoryExtension, innerException)
		{
		}

		protected CannotDisableMandatoryExtensionException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
