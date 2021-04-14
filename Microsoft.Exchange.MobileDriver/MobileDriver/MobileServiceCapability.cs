using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Microsoft.Exchange.TextMessaging.MobileDriver
{
	internal abstract class MobileServiceCapability
	{
		public static bool DoesCodingAndCapacityMatch(MobileServiceCapability a, MobileServiceCapability b)
		{
			if (a == b)
			{
				return true;
			}
			if (a == null || b == null)
			{
				return false;
			}
			if (a.CodingSupportabilities.Count != b.CodingSupportabilities.Count)
			{
				return false;
			}
			if (a.SupportedPartType != b.SupportedPartType)
			{
				return false;
			}
			bool flag = (PartType)0 != (a.SupportedPartType | PartType.Concatenated);
			if (flag && a.SegmentsPerConcatenatedPart != b.SegmentsPerConcatenatedPart)
			{
				return false;
			}
			SmtpToSmsGatewayCapability smtpToSmsGatewayCapability = a as SmtpToSmsGatewayCapability;
			SmtpToSmsGatewayCapability smtpToSmsGatewayCapability2 = b as SmtpToSmsGatewayCapability;
			bool flag2 = smtpToSmsGatewayCapability != null && smtpToSmsGatewayCapability.Parameters != null && null != smtpToSmsGatewayCapability.Parameters.MessageRendering;
			bool flag3 = smtpToSmsGatewayCapability2 != null && smtpToSmsGatewayCapability2.Parameters != null && null != smtpToSmsGatewayCapability2.Parameters.MessageRendering;
			if (flag2 != flag3)
			{
				return false;
			}
			if (smtpToSmsGatewayCapability.Parameters.MessageRendering.Container != smtpToSmsGatewayCapability2.Parameters.MessageRendering.Container)
			{
				return false;
			}
			foreach (CodingSupportability codingSupportability in a.CodingSupportabilities)
			{
				bool flag4 = false;
				foreach (CodingSupportability codingSupportability2 in b.CodingSupportabilities)
				{
					if (codingSupportability.CodingScheme == codingSupportability2.CodingScheme && codingSupportability.RadixPerPart == codingSupportability2.RadixPerPart && (!flag || codingSupportability.RadixPerSegment == codingSupportability2.RadixPerSegment))
					{
						flag4 = true;
						break;
					}
				}
				if (!flag4)
				{
					return false;
				}
			}
			return true;
		}

		internal MobileServiceCapability(PartType supportedPartType, int segmentsPerConcatenatedPart, IList<CodingSupportability> codingSupportabilities, FeatureSupportability featureSupportabilities)
		{
			this.SupportedPartType = supportedPartType;
			this.SegmentsPerConcatenatedPart = segmentsPerConcatenatedPart;
			if (!codingSupportabilities.IsReadOnly)
			{
				codingSupportabilities = new ReadOnlyCollection<CodingSupportability>(codingSupportabilities);
			}
			this.CodingSupportabilities = codingSupportabilities;
			this.FeatureSupportabilities = featureSupportabilities;
		}

		public PartType SupportedPartType { get; private set; }

		public int SegmentsPerConcatenatedPart { get; private set; }

		public IList<CodingSupportability> CodingSupportabilities { get; private set; }

		public FeatureSupportability FeatureSupportabilities { get; private set; }
	}
}
