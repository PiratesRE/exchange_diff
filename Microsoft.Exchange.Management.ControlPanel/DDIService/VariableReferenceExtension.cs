using System;
using System.Windows.Markup;

namespace Microsoft.Exchange.Management.DDIService
{
	public class VariableReferenceExtension : MarkupExtension
	{
		public VariableReferenceExtension()
		{
		}

		public VariableReferenceExtension(string variable, bool useInput)
		{
			this.Variable = variable;
			this.UseInput = useInput;
		}

		public string Variable { get; set; }

		public bool UseInput { get; set; }

		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			return new VariableReference
			{
				Variable = this.Variable,
				UseInput = this.UseInput
			};
		}
	}
}
