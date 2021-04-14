using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.DxStore.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DxStoreManagerGroupNotFoundException : DxStoreManagerClientException
	{
		public DxStoreManagerGroupNotFoundException(string groupName) : base(Strings.DxStoreManagerGroupNotFoundException(groupName))
		{
			this.groupName = groupName;
		}

		public DxStoreManagerGroupNotFoundException(string groupName, Exception innerException) : base(Strings.DxStoreManagerGroupNotFoundException(groupName), innerException)
		{
			this.groupName = groupName;
		}

		protected DxStoreManagerGroupNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.groupName = (string)info.GetValue("groupName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("groupName", this.groupName);
		}

		public string GroupName
		{
			get
			{
				return this.groupName;
			}
		}

		private readonly string groupName;
	}
}
