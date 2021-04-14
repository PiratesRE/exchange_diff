using System;

namespace Microsoft.Office.Datacenter.WorkerTaskFramework
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
	public class PropertyInformationAttribute : Attribute
	{
		public PropertyInformationAttribute(string propertyDescription, bool isMandatory)
		{
			if (string.IsNullOrEmpty(propertyDescription))
			{
				throw new ArgumentException("Parameter is either null or empty", "propertyDescription");
			}
			this.description = propertyDescription;
			this.isMandatory = isMandatory;
		}

		public string Description
		{
			get
			{
				return this.description;
			}
		}

		public bool IsMandatory
		{
			get
			{
				return this.isMandatory;
			}
		}

		private readonly string description;

		private readonly bool isMandatory;
	}
}
