using System;

namespace Microsoft.Office.Datacenter.WorkerTaskFramework
{
	[Serializable]
	public class PropertyInformation
	{
		public PropertyInformation(string name, string description, bool isMandatory = false)
		{
			this.name = name;
			this.Description = description;
			this.IsMandatory = isMandatory;
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		public string Description { get; set; }

		public bool IsMandatory { get; set; }

		private readonly string name;
	}
}
