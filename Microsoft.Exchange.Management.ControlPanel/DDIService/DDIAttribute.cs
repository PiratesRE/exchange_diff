using System;

namespace Microsoft.Exchange.Management.DDIService
{
	public abstract class DDIAttribute : Attribute
	{
		public string Description { get; set; }

		public DDIAttribute(string description)
		{
			this.Description = description;
		}
	}
}
