using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.ManagementGUI.Resources
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ObjectPickerException : LocalizedException
	{
		public ObjectPickerException(string backgroundThreadMessage) : base(Strings.ObjectPickerError(backgroundThreadMessage))
		{
			this.backgroundThreadMessage = backgroundThreadMessage;
		}

		public ObjectPickerException(string backgroundThreadMessage, Exception innerException) : base(Strings.ObjectPickerError(backgroundThreadMessage), innerException)
		{
			this.backgroundThreadMessage = backgroundThreadMessage;
		}

		protected ObjectPickerException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.backgroundThreadMessage = (string)info.GetValue("backgroundThreadMessage", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("backgroundThreadMessage", this.backgroundThreadMessage);
		}

		public string BackgroundThreadMessage
		{
			get
			{
				return this.backgroundThreadMessage;
			}
		}

		private readonly string backgroundThreadMessage;
	}
}
