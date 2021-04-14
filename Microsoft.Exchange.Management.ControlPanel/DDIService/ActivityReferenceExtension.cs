using System;
using System.Windows.Markup;
using System.Xaml;

namespace Microsoft.Exchange.Management.DDIService
{
	[ContentProperty("Name")]
	[MarkupExtensionReturnType(typeof(Activity))]
	public class ActivityReferenceExtension : MarkupExtension
	{
		public ActivityReferenceExtension()
		{
		}

		public ActivityReferenceExtension(string name)
		{
			this.Name = name;
		}

		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			IRootObjectProvider rootObjectProvider = (IRootObjectProvider)serviceProvider.GetService(typeof(IRootObjectProvider));
			object obj = ((Service)rootObjectProvider.RootObject).Resources[this.Name];
			if (obj is Activity)
			{
				return ((Activity)obj).Clone();
			}
			throw new InvalidOperationException("ActivityReference can't reference to a value which is not an Activity");
		}

		public string Name { get; set; }
	}
}
