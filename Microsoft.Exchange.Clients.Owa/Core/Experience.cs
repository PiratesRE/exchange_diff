using System;
using System.Globalization;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	public sealed class Experience
	{
		internal string Name
		{
			get
			{
				return this.name;
			}
		}

		internal FormsRegistry FormsRegistry
		{
			get
			{
				return this.formsRegistry;
			}
			set
			{
				this.formsRegistry = value;
			}
		}

		internal Experience(string name, FormsRegistry formsRegistry)
		{
			this.name = name;
			this.formsRegistry = formsRegistry;
		}

		public new static bool Equals(object a, object b)
		{
			Experience experience = a as Experience;
			Experience experience2 = b as Experience;
			return experience != null && experience2 != null && experience.Name == experience2.Name && experience.FormsRegistry == experience2.FormsRegistry;
		}

		public override int GetHashCode()
		{
			return this.name.GetHashCode() ^ this.formsRegistry.GetHashCode();
		}

		public override bool Equals(object value)
		{
			return Experience.Equals(value, this);
		}

		internal static Experience Copy(Experience experience)
		{
			return new Experience(string.Copy(experience.Name), experience.FormsRegistry);
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "Name = {0}, FormsRegistry = {1}", new object[]
			{
				this.name,
				this.formsRegistry.Name
			});
		}

		private string name;

		private FormsRegistry formsRegistry;
	}
}
