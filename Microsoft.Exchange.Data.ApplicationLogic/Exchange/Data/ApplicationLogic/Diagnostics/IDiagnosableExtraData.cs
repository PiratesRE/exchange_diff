using System;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.ApplicationLogic.Diagnostics
{
	internal interface IDiagnosableExtraData : IDiagnosable
	{
		void SetData(XElement data);

		void OnStop();
	}
}
