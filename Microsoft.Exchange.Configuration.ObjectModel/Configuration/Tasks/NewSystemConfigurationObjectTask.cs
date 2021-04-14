using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Configuration.Tasks
{
	public abstract class NewSystemConfigurationObjectTask<TDataObject> : NewFixedNameSystemConfigurationObjectTask<TDataObject> where TDataObject : ADObject, new()
	{
		[Parameter(Mandatory = true, Position = 0)]
		public string Name
		{
			get
			{
				return this.DataObject.Name;
			}
			set
			{
				this.DataObject.Name = value;
			}
		}
	}
}
