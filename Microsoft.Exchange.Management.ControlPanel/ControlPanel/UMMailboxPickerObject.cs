using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Directory.Management;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class UMMailboxPickerObject : RecipientPickerObject
	{
		public UMMailboxPickerObject(ReducedRecipient recipient) : base(recipient)
		{
		}
	}
}
