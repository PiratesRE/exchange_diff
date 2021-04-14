using System;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;
using Microsoft.Exchange.Management.SystemConfigurationTasks;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class OptionalRetentionPolicyTagRow : RetentionPolicyTagBaseRow
	{
		public OptionalRetentionPolicyTagRow(PresentationRetentionPolicyTag rpt) : base(rpt)
		{
		}

		[DataMember]
		public string Description
		{
			get
			{
				return base.RetentionPolicyTag.GetLocalizedFolderComment(new CultureInfo[]
				{
					Thread.CurrentThread.CurrentUICulture
				}.AsEnumerable<CultureInfo>());
			}
			private set
			{
				throw new NotSupportedException();
			}
		}
	}
}
