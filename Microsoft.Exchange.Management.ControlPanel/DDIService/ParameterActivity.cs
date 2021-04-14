using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Windows.Markup;

namespace Microsoft.Exchange.Management.DDIService
{
	[ContentProperty("Parameters")]
	public abstract class ParameterActivity : Activity
	{
		public ParameterActivity() : base(string.Empty)
		{
		}

		public ParameterActivity(string name) : base(name)
		{
		}

		protected ParameterActivity(ParameterActivity activity) : base(activity)
		{
			this.Parameters = new Collection<Parameter>((from c in activity.Parameters
			select c.Clone() as Parameter).ToList<Parameter>());
		}

		[DDINoDuplication(PropertyName = "Name")]
		public Collection<Parameter> Parameters
		{
			get
			{
				return this.parameters;
			}
			set
			{
				this.parameters = value;
			}
		}

		public virtual IList<Parameter> GetEffectiveParameters(DataRow input, DataTable dataTable, DataObjectStore store)
		{
			IList<Parameter> list = new List<Parameter>();
			foreach (Parameter item in this.Parameters)
			{
				list.Add(item);
			}
			return list;
		}

		private Collection<Parameter> parameters = new Collection<Parameter>();
	}
}
