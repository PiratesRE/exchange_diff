using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ELCFolderAlreadyUpgraded : LocalizedException
	{
		public ELCFolderAlreadyUpgraded(string folderName, string rptName) : base(Strings.ELCFolderAlreadyUpgraded(folderName, rptName))
		{
			this.folderName = folderName;
			this.rptName = rptName;
		}

		public ELCFolderAlreadyUpgraded(string folderName, string rptName, Exception innerException) : base(Strings.ELCFolderAlreadyUpgraded(folderName, rptName), innerException)
		{
			this.folderName = folderName;
			this.rptName = rptName;
		}

		protected ELCFolderAlreadyUpgraded(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.folderName = (string)info.GetValue("folderName", typeof(string));
			this.rptName = (string)info.GetValue("rptName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("folderName", this.folderName);
			info.AddValue("rptName", this.rptName);
		}

		public string FolderName
		{
			get
			{
				return this.folderName;
			}
		}

		public string RptName
		{
			get
			{
				return this.rptName;
			}
		}

		private readonly string folderName;

		private readonly string rptName;
	}
}
