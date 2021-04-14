using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	internal class FolderParameter : FormletParameter
	{
		public FolderParameter(string name, LocalizedString dialogTitle, LocalizedString dialogLabel) : base(name, dialogTitle, dialogLabel)
		{
		}
	}
}
