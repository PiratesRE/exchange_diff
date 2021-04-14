using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Directory.Management;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public abstract class SetGroupBase : SetObjectProperties
	{
		public override string AssociatedCmdlet
		{
			get
			{
				return "Set-Group";
			}
		}

		[DataMember]
		public string Notes
		{
			get
			{
				return (string)base[WindowsGroupSchema.Notes];
			}
			set
			{
				base[WindowsGroupSchema.Notes] = value;
			}
		}
	}
}
