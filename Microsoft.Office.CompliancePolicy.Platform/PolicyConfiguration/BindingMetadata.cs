using System;

namespace Microsoft.Office.CompliancePolicy.PolicyConfiguration
{
	[Serializable]
	public sealed class BindingMetadata
	{
		public BindingMetadata()
		{
		}

		public BindingMetadata(string displayName, string name, string immutableIdentity, PolicyBindingTypes type = PolicyBindingTypes.IndividualResource)
		{
			this.DisplayName = displayName;
			this.Name = name;
			this.ImmutableIdentity = immutableIdentity;
			this.Type = type;
			this.SchemaVersion = BindingMetadata.latestVersion;
		}

		public string DisplayName { get; set; }

		public string Name { get; set; }

		public string ImmutableIdentity { get; set; }

		public PolicyBindingTypes Type { get; set; }

		public Workload Workload { get; set; }

		public int SchemaVersion { get; set; }

		public static BindingMetadata FromStorage(string storageValue)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("storageValue", storageValue);
			return CommonUtility.StringToObject<BindingMetadata>(storageValue);
		}

		public static string ToStorage(BindingMetadata binding)
		{
			ArgumentValidator.ThrowIfNull("binding", binding);
			return CommonUtility.ObjectToString(binding);
		}

		public override string ToString()
		{
			return this.DisplayName;
		}

		public override bool Equals(object other)
		{
			bool result = false;
			if (other != null)
			{
				if (this == other)
				{
					result = true;
				}
				else
				{
					BindingMetadata bindingMetadata = other as BindingMetadata;
					if (bindingMetadata != null)
					{
						result = (string.Equals(bindingMetadata.DisplayName, this.DisplayName) && string.Equals(bindingMetadata.Name, this.Name, StringComparison.OrdinalIgnoreCase) && string.Equals(bindingMetadata.ImmutableIdentity, this.ImmutableIdentity, StringComparison.OrdinalIgnoreCase) && bindingMetadata.SchemaVersion == this.SchemaVersion && bindingMetadata.Type == this.Type && bindingMetadata.Workload == this.Workload);
					}
				}
			}
			return result;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		private static readonly int latestVersion = 2;
	}
}
