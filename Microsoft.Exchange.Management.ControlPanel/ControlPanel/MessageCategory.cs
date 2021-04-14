using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Storage.Management;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class MessageCategory : EnumValue
	{
		public MessageCategory(MessageCategory category) : base(category.Name, category.Name)
		{
		}
	}
}
