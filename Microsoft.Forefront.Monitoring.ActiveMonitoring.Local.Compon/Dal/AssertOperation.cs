using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.Dal
{
	public class AssertOperation : DalProbeOperation
	{
		[XmlAttribute]
		public string ActualValue { get; set; }

		[XmlAttribute]
		public string ExpectedValue { get; set; }

		[XmlAttribute]
		public AssertOperator Operator { get; set; }

		public override void Execute(IDictionary<string, object> variables)
		{
			object value = DalProbeOperation.GetValue(this.ActualValue, variables);
			object obj = DalProbeOperation.GetValue(this.ExpectedValue, variables);
			if (value != null && obj != null && obj.GetType() != value.GetType())
			{
				obj = Convert.ChangeType(obj, value.GetType());
			}
			if ((this.Operator == AssertOperator.EqualTo || this.Operator == AssertOperator.LessThanOrEqualTo || this.Operator == AssertOperator.GreaterThanOrEqualTo) && object.Equals(value, obj))
			{
				return;
			}
			if (this.Operator == AssertOperator.NotEqualTo && !object.Equals(value, obj))
			{
				return;
			}
			if (this.Operator == AssertOperator.LessThan || this.Operator == AssertOperator.LessThanOrEqualTo)
			{
				IComparable comparable = value as IComparable;
				if (comparable != null)
				{
					if (comparable.CompareTo(obj) < 0)
					{
						return;
					}
				}
				else
				{
					comparable = (obj as IComparable);
					if (comparable == null)
					{
						throw new ArgumentException(string.Format("Either {0} or {1} must be IComparable", value, obj));
					}
					if (comparable.CompareTo(value) > 0)
					{
						return;
					}
				}
			}
			if (this.Operator == AssertOperator.GreaterThan || this.Operator == AssertOperator.GreaterThanOrEqualTo)
			{
				IComparable comparable2 = value as IComparable;
				if (comparable2 != null)
				{
					if (comparable2.CompareTo(obj) > 0)
					{
						return;
					}
				}
				else
				{
					comparable2 = (obj as IComparable);
					if (comparable2 == null)
					{
						throw new ArgumentException(string.Format("Either {0} or {1} must be IComparable", value, obj));
					}
					if (comparable2.CompareTo(value) < 0)
					{
						return;
					}
				}
			}
			if (this.Operator == AssertOperator.RegexMatch)
			{
				Regex regex = new Regex(obj.ToString());
				if (regex.IsMatch(value.ToString()))
				{
					return;
				}
			}
			throw new ExAssertException(string.Format("Assertion of {0} {1} {2} Failed", value, this.Operator, obj));
		}
	}
}
