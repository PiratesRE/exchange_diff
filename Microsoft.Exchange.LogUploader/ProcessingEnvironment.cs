using System;
using System.Configuration;

namespace Microsoft.Exchange.LogUploader
{
	internal class ProcessingEnvironment : ConfigurationElement
	{
		[ConfigurationProperty("name", IsRequired = true)]
		public string Name
		{
			get
			{
				return (string)base["name"];
			}
		}

		[ConfigurationProperty("Logs", IsRequired = true)]
		public LogTypeInstanceCollection Logs
		{
			get
			{
				return (LogTypeInstanceCollection)base["Logs"];
			}
		}

		public string EnvironmentName { get; private set; }

		public string Region { get; private set; }

		protected override void PostDeserialize()
		{
			this.EnvironmentName = string.Empty;
			this.Region = string.Empty;
			string[] array = this.Name.Split(new char[]
			{
				'-'
			}, StringSplitOptions.RemoveEmptyEntries);
			string text = null;
			if (array.Length < 1)
			{
				text = "Invalid name. empty name? " + this.Name;
			}
			else
			{
				this.EnvironmentName = array[0];
				if (array.Length > 1)
				{
					this.Region = array[1];
				}
			}
			if (text != null)
			{
				throw new ArgumentException(text);
			}
		}

		public const string NameKey = "name";

		public const string LogTypeInstancesKey = "Logs";
	}
}
