using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class OrgUploadExtensionParameter : UploadExtensionParameter
	{
		public override string RbacScope
		{
			get
			{
				return "@W:Organization";
			}
		}
	}
}
