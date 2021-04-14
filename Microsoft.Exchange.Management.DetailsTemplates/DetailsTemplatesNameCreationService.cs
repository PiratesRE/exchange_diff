using System;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;

namespace Microsoft.Exchange.Management.DetailsTemplates
{
	internal class DetailsTemplatesNameCreationService : INameCreationService
	{
		public string CreateName(IContainer container, Type dataType)
		{
			if (dataType == null)
			{
				throw new ArgumentNullException("dataType");
			}
			string text = dataType.Name;
			if (container != null)
			{
				int num = 0;
				string text2;
				do
				{
					num++;
					text2 = string.Format(CultureInfo.CurrentCulture, "{0}{1}", new object[]
					{
						text,
						num.ToString(CultureInfo.InvariantCulture)
					});
				}
				while (container.Components[text2] != null);
				text = text2;
			}
			return text;
		}

		public bool IsValidName(string name)
		{
			return true;
		}

		public void ValidateName(string name)
		{
		}
	}
}
