using System;
using System.Globalization;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Management.Deployment
{
	[ClassAccessLevel(AccessLevel.Consumer)]
	[Serializable]
	public sealed class ExsetdataException : LocalizedException
	{
		public ExsetdataException(uint sc, LocalizedString englishMessage, LocalizedString localizedMessage) : base(localizedMessage)
		{
			this.sc = sc;
			this.englishMessage = englishMessage;
		}

		public override string Message
		{
			get
			{
				CultureInfo cultureInfo = (base.FormatProvider as CultureInfo) ?? CultureInfo.CurrentUICulture;
				if (cultureInfo.LCID == ExsetdataException.englishCulture.LCID)
				{
					return this.englishMessage.ToString(ExsetdataException.englishCulture);
				}
				return base.Message;
			}
		}

		public uint SC
		{
			get
			{
				return this.sc;
			}
		}

		private readonly uint sc;

		private static CultureInfo englishCulture = new CultureInfo("en-US");

		private LocalizedString englishMessage;
	}
}
