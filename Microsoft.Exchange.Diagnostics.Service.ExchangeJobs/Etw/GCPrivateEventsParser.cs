using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Diagnostics.Service.ExchangeJobs.Etw
{
	internal class GCPrivateEventsParser : IParser
	{
		public static Guid ProviderGuid
		{
			get
			{
				return GCPrivateEventsParser.providerGuid;
			}
		}

		public IEnumerable<Guid> Guids
		{
			get
			{
				return new Guid[]
				{
					GCPrivateEventsParser.providerGuid
				};
			}
		}

		public unsafe void Parse(EtwTraceNativeComponents.EVENT_RECORD* rawData)
		{
			if (rawData->EventHeader.Opcode == 17)
			{
				GCPerHeapHistoryTraceEvent gcperHeapHistoryTraceEvent = new GCPerHeapHistoryTraceEvent(GCPrivateEventsParser.providerGuid, "Microsoft-Windows-DotNETRuntimePrivate", rawData);
				List<long[]> list;
				if (!this.processData.TryGetValue(rawData->EventHeader.ProcessId, out list))
				{
					list = new List<long[]>();
					this.processData.Add(rawData->EventHeader.ProcessId, list);
				}
				long[] array = new long[4];
				for (Gens gens = Gens.Gen0; gens <= Gens.GenLargeObj; gens++)
				{
					long fragmentation = gcperHeapHistoryTraceEvent.GenData(gens).Fragmentation;
					array[(int)gens] = fragmentation;
				}
				list.Add(array);
			}
		}

		public Dictionary<int, long[][]> GetGenData()
		{
			Dictionary<int, long[][]> dictionary = new Dictionary<int, long[][]>();
			foreach (KeyValuePair<int, List<long[]>> keyValuePair in this.processData)
			{
				int num = 1;
				long[][] array = new long[num][];
				keyValuePair.Value.CopyTo(0, array, 0, num);
				dictionary.Add(keyValuePair.Key, array);
			}
			return dictionary;
		}

		private const string ProviderName = "Microsoft-Windows-DotNETRuntimePrivate";

		private const int PrivatePerHeapDataOpcode = 17;

		private static Guid providerGuid = new Guid(1983895380, 28806, 19966, 149, 235, 192, 26, 70, 250, 244, 202);

		private Dictionary<int, List<long[]>> processData = new Dictionary<int, List<long[]>>();
	}
}
