using System;
using System.Xml.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Eas.Model.Common.WindowsLive
{
	[XmlType(Namespace = "WindowsLive", TypeName = "CategoryId")]
	[ClassAccessLevel(AccessLevel.Implementation)]
	public class CategoryId
	{
		[XmlText]
		public int Id { get; set; }
	}
}
