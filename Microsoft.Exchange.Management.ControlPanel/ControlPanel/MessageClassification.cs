using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class MessageClassification : EnumValue
	{
		public MessageClassification(MessageClassification classification) : base(classification.DisplayName, classification.Guid.ToString())
		{
			this.PermissionMenuVisible = classification.PermissionMenuVisible;
		}

		internal bool PermissionMenuVisible { get; private set; }
	}
}
