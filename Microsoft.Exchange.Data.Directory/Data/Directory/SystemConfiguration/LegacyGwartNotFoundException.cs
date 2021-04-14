using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class LegacyGwartNotFoundException : MandatoryContainerNotFoundException
	{
		public LegacyGwartNotFoundException(string gwartName, string adminGroupName) : base(DirectoryStrings.LegacyGwartNotFoundException(gwartName, adminGroupName))
		{
			this.gwartName = gwartName;
			this.adminGroupName = adminGroupName;
		}

		public LegacyGwartNotFoundException(string gwartName, string adminGroupName, Exception innerException) : base(DirectoryStrings.LegacyGwartNotFoundException(gwartName, adminGroupName), innerException)
		{
			this.gwartName = gwartName;
			this.adminGroupName = adminGroupName;
		}

		protected LegacyGwartNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.gwartName = (string)info.GetValue("gwartName", typeof(string));
			this.adminGroupName = (string)info.GetValue("adminGroupName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("gwartName", this.gwartName);
			info.AddValue("adminGroupName", this.adminGroupName);
		}

		public string GwartName
		{
			get
			{
				return this.gwartName;
			}
		}

		public string AdminGroupName
		{
			get
			{
				return this.adminGroupName;
			}
		}

		private readonly string gwartName;

		private readonly string adminGroupName;
	}
}
