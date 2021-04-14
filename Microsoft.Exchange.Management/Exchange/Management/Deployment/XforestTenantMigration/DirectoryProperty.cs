using System;
using System.Collections;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.ActiveDirectory;
using System.Linq;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Deployment.XforestTenantMigration
{
	[Serializable]
	public sealed class DirectoryProperty
	{
		public string Name { get; set; }

		public List<object> Values { get; set; }

		public object Value
		{
			get
			{
				if (this.Values.Count < 1)
				{
					throw new ArgumentNullException("Values");
				}
				if (this.Values.Count > 1)
				{
					throw new ArgumentException(Strings.ErrorMoreThanOneValue(this.Name, this.Values.Count), "Values");
				}
				return this.Values[0];
			}
			set
			{
				if (this.Values.Count < 1)
				{
					throw new ArgumentNullException("Values");
				}
				if (this.Values.Count > 1)
				{
					throw new ArgumentException(Strings.ErrorMoreThanOneValue(this.Name, this.Values.Count), "Values");
				}
				this.Values[0] = value;
			}
		}

		public object this[int index]
		{
			get
			{
				return this.Values[index];
			}
			set
			{
				this.Values[index] = value;
			}
		}

		internal bool IsRequired { get; set; }

		internal bool IsSystemOnly { get; set; }

		internal bool IsBackLink { get; set; }

		internal bool IsLink { get; set; }

		internal ActiveDirectorySyntax Syntax { get; set; }

		public DirectoryProperty()
		{
		}

		public DirectoryProperty(string searchRoot, DictionaryEntry value)
		{
			this.Name = value.Key.ToString();
			this.Values = ((ResultPropertyValueCollection)value.Value).Cast<object>().ToList<object>();
		}
	}
}
