using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Management.Extension;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class ExtensionRow : BaseRow
	{
		public ExtensionRow(App extension) : base(((AppId)extension.Identity).ToIdentity(), extension)
		{
			this.Name = extension.DisplayName;
			this.Enabled = extension.Enabled;
			this.Type = extension.Type.ToString();
			this.IsRemovable = (extension.Type == ExtensionType.Private);
			this.Version = extension.AppVersion;
			this.ExtensionId = extension.AppId;
		}

		[DataMember]
		public string Name { get; protected set; }

		[DataMember]
		public string Type { get; protected set; }

		[DataMember]
		public bool Enabled { get; protected set; }

		[DataMember]
		public bool IsRemovable { get; protected set; }

		[DataMember]
		public string Version { get; protected set; }

		[DataMember]
		public string ExtensionId { get; protected set; }
	}
}
