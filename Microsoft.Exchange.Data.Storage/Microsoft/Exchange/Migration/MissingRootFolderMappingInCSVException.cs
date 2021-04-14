using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class MissingRootFolderMappingInCSVException : MigrationPermanentException
	{
		public MissingRootFolderMappingInCSVException(string hierarchyMailboxName) : base(Strings.MissingRootFolderMappingInCSVError(hierarchyMailboxName))
		{
			this.hierarchyMailboxName = hierarchyMailboxName;
		}

		public MissingRootFolderMappingInCSVException(string hierarchyMailboxName, Exception innerException) : base(Strings.MissingRootFolderMappingInCSVError(hierarchyMailboxName), innerException)
		{
			this.hierarchyMailboxName = hierarchyMailboxName;
		}

		protected MissingRootFolderMappingInCSVException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.hierarchyMailboxName = (string)info.GetValue("hierarchyMailboxName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("hierarchyMailboxName", this.hierarchyMailboxName);
		}

		public string HierarchyMailboxName
		{
			get
			{
				return this.hierarchyMailboxName;
			}
		}

		private readonly string hierarchyMailboxName;
	}
}
