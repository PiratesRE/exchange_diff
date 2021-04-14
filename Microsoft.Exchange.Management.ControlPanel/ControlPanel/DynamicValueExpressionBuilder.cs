using System;
using System.CodeDom;
using System.Reflection;
using System.Web.Compilation;
using System.Web.UI;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public class DynamicValueExpressionBuilder : ExpressionBuilder
	{
		public override bool SupportsEvaluate
		{
			get
			{
				return true;
			}
		}

		public override object EvaluateExpression(object target, BoundPropertyEntry entry, object parsedData, ExpressionBuilderContext context)
		{
			string text = null;
			string text2 = null;
			this.ParseParameter(entry, out text, out text2);
			Module[] modules = Assembly.GetExecutingAssembly().GetModules(false);
			Module module = null;
			foreach (Module module2 in modules)
			{
				if (string.Compare(DynamicValueExpressionBuilder.SearchModuleName, module2.Name, StringComparison.OrdinalIgnoreCase) == 0)
				{
					module = module2;
					break;
				}
			}
			if (null == module)
			{
				throw new ApplicationException("Could not find module by name " + DynamicValueExpressionBuilder.SearchModuleName + ". If the module name is changed, please change DynamicValueExpressionBuilder.SearchModuleName.");
			}
			Type type = module.GetType(DynamicValueExpressionBuilder.SearchNamespaceName + text);
			if (null == type)
			{
				throw new ArgumentException(string.Concat(new string[]
				{
					"Can not find your class ",
					text,
					". Please check 1. You defined your static class in namespace ",
					DynamicValueExpressionBuilder.SearchNamespaceName,
					". 2. You spell your class name correctly. "
				}));
			}
			PropertyInfo property = type.GetProperty(text2, BindingFlags.Static);
			if (null == property)
			{
				throw new ArgumentException(string.Concat(new string[]
				{
					"Can not find your property ",
					text2,
					" on class ",
					text,
					". Please make sure you spelled your static property name correctly. "
				}));
			}
			object value = property.GetValue(null, null);
			return value.ToString();
		}

		public override CodeExpression GetCodeExpression(BoundPropertyEntry entry, object parsedData, ExpressionBuilderContext context)
		{
			string type = null;
			string propertyName = null;
			this.ParseParameter(entry, out type, out propertyName);
			CodePropertyReferenceExpression targetObject = new CodePropertyReferenceExpression(new CodeTypeReferenceExpression(type), propertyName);
			return new CodeMethodInvokeExpression(targetObject, "ToString", new CodeExpression[0]);
		}

		private void ParseParameter(BoundPropertyEntry entry, out string className, out string propertyName)
		{
			string text = entry.Expression.Trim();
			string[] array = text.Split(new char[]
			{
				'.'
			});
			if (2 != array.Length)
			{
				throw new ArgumentException("DynamicValue formated is StaticClassName.StaticPropertyName");
			}
			className = array[0];
			propertyName = array[1];
		}

		private static readonly string SearchModuleName = "Microsoft.Exchange.Management.ControlPanel.DLL";

		private static readonly string SearchNamespaceName = "Microsoft.Exchange.Management.ControlPanel.";
	}
}
