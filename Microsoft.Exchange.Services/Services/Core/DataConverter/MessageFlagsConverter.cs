using System;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal class MessageFlagsConverter : BaseConverter
	{
		public MessageFlagsConverter(MessageFlags messageFlags)
		{
			this.flag = messageFlags;
		}

		public override object ConvertToObject(string propertyString)
		{
			return (propertyString == "true" || propertyString == "1") ? this.flag : MessageFlags.None;
		}

		public override string ConvertToString(object propertyValue)
		{
			if (this.flag == ((MessageFlags)propertyValue & this.flag))
			{
				return "true";
			}
			return "false";
		}

		protected override object ConvertToServiceObjectValue(object propertyValue)
		{
			return this.flag == ((MessageFlags)propertyValue & this.flag);
		}

		private MessageFlags flag;
	}
}
