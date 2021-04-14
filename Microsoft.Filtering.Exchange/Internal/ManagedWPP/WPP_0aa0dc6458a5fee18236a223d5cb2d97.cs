using System;
using System.CodeDom.Compiler;

namespace Microsoft.Internal.ManagedWPP
{
	[GeneratedCode("ManagedWPP", "1.15.0.0")]
	internal class WPP_0aa0dc6458a5fee18236a223d5cb2d97
	{
		private WPP_0aa0dc6458a5fee18236a223d5cb2d97()
		{
		}

		[GeneratedCode("ManagedWPP", "1.15.0.0")]
		internal unsafe static void WPP_isss(int messageGuidIndex, int messageNumber, int arg0, string arg1, string arg2, string arg3)
		{
			int bufferSize = TraceProvider.GetBufferSize(7, 10);
			void* ptr = stackalloc byte[(UIntPtr)bufferSize];
			int num = 0;
			void* ptr2 = TraceProvider.InitializeTraceBuffer(ptr, WPP_0aa0dc6458a5fee18236a223d5cb2d97.messageGuids[messageGuidIndex], messageNumber, 7, ref num);
			fixed (char* ptr3 = arg1, ptr4 = arg2, ptr5 = arg3)
			{
				ptr2 = TraceProvider.InitializeTraceField(ptr2, arg0, ptr, ref num);
				ptr2 = TraceProvider.InitializeTraceField(ptr2, arg1, ptr, ref num, ptr3);
				ptr2 = TraceProvider.InitializeTraceField(ptr2, arg2, ptr, ref num, ptr4);
				ptr2 = TraceProvider.InitializeTraceField(ptr2, arg3, ptr, ref num, ptr5);
				Tracing.tracer.TraceEvent(ptr);
			}
		}

		private static Guid[] messageGuids = new Guid[]
		{
			new Guid("{400c448c-9e5e-c70d-3892-10d63e28a5fa}"),
			new Guid("{1c88631e-cb07-11e4-b7f6-b62420f3ffe2}"),
			new Guid("{b67d6479-8a29-abbd-de11-6ae86ac8e1e9}"),
			new Guid("{8674f07a-eca7-6946-925c-aaa7104d8cce}"),
			new Guid("{75df96ff-0ea0-7bdc-3f65-8b2ee96ea9a0}"),
			new Guid("{0d2aca11-937f-7bc8-8afc-5a19ce614549}"),
			new Guid("{f8f4835e-5761-64ee-04cd-0cfbbbdfe1ad}"),
			new Guid("{c177f5ef-d3e7-4780-c411-95aa8076c51d}"),
			new Guid("{1a1a724b-5c63-752f-b556-9b0c7ff2de3b}"),
			new Guid("{a404c313-daa6-5c1b-8946-deeef36e89d2}"),
			new Guid("{7c483023-637b-a518-450a-4e83f4c879bc}"),
			new Guid("{b031813e-aaa3-eda7-4798-5c749c03f7a9}"),
			new Guid("{d656dbf6-ec36-cd7c-703d-2c5bd556d77b}"),
			new Guid("{077772cd-f261-fe5d-5609-74d7e4a9c568}"),
			new Guid("{9b2e374f-6057-79a4-ae43-6ab63623c552}"),
			new Guid("{ca67721c-6c08-72ca-f880-87c3d7938fd3}"),
			new Guid("{4ea9194f-0e57-cd5b-a82d-e14fda9e44ad}"),
			new Guid("{aed21ff6-11cf-8188-f7fd-a90d401a138f}"),
			new Guid("{14bd62fd-2788-6879-3bdf-f093b43bbcb3}"),
			new Guid("{967fbe75-3588-919f-7530-e43f49ceb16a}"),
			new Guid("{914ac831-4ee3-56a5-b930-4ec999e84e6b}")
		};
	}
}
