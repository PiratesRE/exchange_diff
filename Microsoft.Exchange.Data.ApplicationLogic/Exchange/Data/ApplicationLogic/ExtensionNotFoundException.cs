using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.ApplicationLogic.Extension;

namespace Microsoft.Exchange.Data.ApplicationLogic
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class ExtensionNotFoundException : OwaExtensionOperationException
	{
		public ExtensionNotFoundException(string extensionID) : base(Strings.ErrorExtensionNotFound(extensionID))
		{
			this.extensionID = extensionID;
		}

		public ExtensionNotFoundException(string extensionID, Exception innerException) : base(Strings.ErrorExtensionNotFound(extensionID), innerException)
		{
			this.extensionID = extensionID;
		}

		protected ExtensionNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.extensionID = (string)info.GetValue("extensionID", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("extensionID", this.extensionID);
		}

		public string ExtensionID
		{
			get
			{
				return this.extensionID;
			}
		}

		private readonly string extensionID;
	}
}
