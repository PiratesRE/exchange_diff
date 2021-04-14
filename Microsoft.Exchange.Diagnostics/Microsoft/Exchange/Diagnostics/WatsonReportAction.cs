using System;
using System.Xml;

namespace Microsoft.Exchange.Diagnostics
{
	internal abstract class WatsonReportAction
	{
		internal WatsonReportAction(string expression, bool caseSensitive)
		{
			this.expression = (expression ?? string.Empty);
			this.caseSensitive = caseSensitive;
		}

		public abstract string ActionName { get; }

		protected string Expression
		{
			get
			{
				return this.expression;
			}
		}

		public abstract string Evaluate(WatsonReport watsonReport);

		public override bool Equals(object obj)
		{
			return !(obj.GetType() != base.GetType()) && this.Expression.Equals(((WatsonReportAction)obj).Expression, this.caseSensitive ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
		}

		public override int GetHashCode()
		{
			return this.Expression.GetHashCode();
		}

		internal void WriteResult(WatsonReport watsonReport, XmlWriter writer)
		{
			using (SafeXmlTag safeXmlTag = new SafeXmlTag(writer, "action").WithAttribute("type", this.ActionName))
			{
				try
				{
					safeXmlTag.SetContent(this.Evaluate(watsonReport));
				}
				catch (Exception ex)
				{
					watsonReport.RecordExceptionWhileCreatingReport(ex);
					safeXmlTag.SetContent(string.Concat(new string[]
					{
						"Exception thrown while evaluating action \"",
						this.ActionName,
						"\" (expression: ",
						this.Expression,
						"):\r\n",
						ex.ToString()
					}));
				}
			}
		}

		private string expression;

		private bool caseSensitive;
	}
}
