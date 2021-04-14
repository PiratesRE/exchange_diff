using System;

namespace Microsoft.Exchange.VariantConfiguration
{
	public abstract class VariantConfigurationOverride
	{
		public VariantConfigurationOverride(string componentName, string sectionName, params string[] parameters)
		{
			if (string.IsNullOrEmpty(componentName))
			{
				throw new ArgumentNullException("componentName");
			}
			if (string.IsNullOrEmpty(sectionName))
			{
				throw new ArgumentNullException("sectionName");
			}
			if (parameters == null || parameters.Length == 0)
			{
				throw new ArgumentNullException("parameters");
			}
			this.ComponentName = componentName;
			this.SectionName = sectionName;
			this.Parameters = parameters;
		}

		public bool IsFlight
		{
			get
			{
				return this is VariantConfigurationFlightOverride;
			}
		}

		public string ComponentName { get; private set; }

		public abstract string FileName { get; }

		public string SectionName { get; private set; }

		public string[] Parameters { get; private set; }

		public override bool Equals(object obj)
		{
			VariantConfigurationOverride variantConfigurationOverride = obj as VariantConfigurationOverride;
			return variantConfigurationOverride != null && base.GetType() == variantConfigurationOverride.GetType() && this.IsFlight == variantConfigurationOverride.IsFlight && this.ComponentName.Equals(variantConfigurationOverride.ComponentName, StringComparison.InvariantCultureIgnoreCase) && this.SectionName.Equals(variantConfigurationOverride.SectionName, StringComparison.InvariantCultureIgnoreCase) && this.ParametersMatch(variantConfigurationOverride.Parameters);
		}

		public override int GetHashCode()
		{
			return base.GetType().GetHashCode() ^ (this.IsFlight ? 1 : 0) ^ this.ComponentName.ToLowerInvariant().GetHashCode() ^ this.SectionName.ToLowerInvariant().GetHashCode() ^ this.GetParametersHashCode();
		}

		private bool ParametersMatch(string[] parameters)
		{
			if (this.Parameters.Length != parameters.Length)
			{
				return false;
			}
			for (int i = 0; i < this.Parameters.Length; i++)
			{
				if (!this.Parameters[i].Equals(parameters[i]))
				{
					return false;
				}
			}
			return true;
		}

		private int GetParametersHashCode()
		{
			int num = 0;
			foreach (string text in this.Parameters)
			{
				num ^= text.GetHashCode();
			}
			return num;
		}
	}
}
