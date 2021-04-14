using System;
using System.IO;

namespace Microsoft.Exchange.AirSync.SchemaConverter.Common
{
	internal interface IBodyProperty : IMIMERelatedProperty, IProperty
	{
		Stream RtfData { get; }

		bool RtfPresent { get; }

		int RtfSize { get; }

		Stream TextData { get; }

		bool TextPresent { get; }

		int TextSize { get; }

		Stream GetTextData(int length);
	}
}
