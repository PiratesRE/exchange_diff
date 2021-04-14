using System;
using System.CodeDom.Compiler;

namespace Microsoft.Internal.ManagedWPP
{
	[GeneratedCode("ManagedWPP", "1.15.0.0")]
	public class WPP_1cd3dee55f704f6905d1e53a161baad7
	{
		private WPP_1cd3dee55f704f6905d1e53a161baad7()
		{
		}

		[GeneratedCode("ManagedWPP", "1.15.0.0")]
		public unsafe static void WPP_s(int messageGuidIndex, int messageNumber, string arg0)
		{
			int bufferSize = TraceProvider.GetBufferSize(2, 2);
			void* buffer = stackalloc byte[(UIntPtr)bufferSize];
			int num = 0;
			void* field = TraceProvider.InitializeTraceBuffer(buffer, WPP_1cd3dee55f704f6905d1e53a161baad7.messageGuids[messageGuidIndex], messageNumber, 2, ref num);
			fixed (char* charBuffer = arg0)
			{
				field = TraceProvider.InitializeTraceField(field, arg0, buffer, ref num, charBuffer);
				Tracing.tracer.TraceEvent(buffer);
			}
		}

		[GeneratedCode("ManagedWPP", "1.15.0.0")]
		public unsafe static void WPP_i(int messageGuidIndex, int messageNumber, int arg0)
		{
			int bufferSize = TraceProvider.GetBufferSize(1, 4);
			void* buffer = stackalloc byte[(UIntPtr)bufferSize];
			int num = 0;
			void* field = TraceProvider.InitializeTraceBuffer(buffer, WPP_1cd3dee55f704f6905d1e53a161baad7.messageGuids[messageGuidIndex], messageNumber, 1, ref num);
			field = TraceProvider.InitializeTraceField(field, arg0, buffer, ref num);
			Tracing.tracer.TraceEvent(buffer);
		}

		[GeneratedCode("ManagedWPP", "1.15.0.0")]
		public unsafe static void WPP_is(int messageGuidIndex, int messageNumber, int arg0, string arg1)
		{
			int bufferSize = TraceProvider.GetBufferSize(3, 6);
			void* buffer = stackalloc byte[(UIntPtr)bufferSize];
			int num = 0;
			void* field = TraceProvider.InitializeTraceBuffer(buffer, WPP_1cd3dee55f704f6905d1e53a161baad7.messageGuids[messageGuidIndex], messageNumber, 3, ref num);
			fixed (char* charBuffer = arg1)
			{
				field = TraceProvider.InitializeTraceField(field, arg0, buffer, ref num);
				field = TraceProvider.InitializeTraceField(field, arg1, buffer, ref num, charBuffer);
				Tracing.tracer.TraceEvent(buffer);
			}
		}

		[GeneratedCode("ManagedWPP", "1.15.0.0")]
		public unsafe static void WPP_iss(int messageGuidIndex, int messageNumber, int arg0, string arg1, string arg2)
		{
			int bufferSize = TraceProvider.GetBufferSize(5, 8);
			void* buffer = stackalloc byte[(UIntPtr)bufferSize];
			int num = 0;
			void* field = TraceProvider.InitializeTraceBuffer(buffer, WPP_1cd3dee55f704f6905d1e53a161baad7.messageGuids[messageGuidIndex], messageNumber, 5, ref num);
			fixed (char* charBuffer = arg1, charBuffer2 = arg2)
			{
				field = TraceProvider.InitializeTraceField(field, arg0, buffer, ref num);
				field = TraceProvider.InitializeTraceField(field, arg1, buffer, ref num, charBuffer);
				field = TraceProvider.InitializeTraceField(field, arg2, buffer, ref num, charBuffer2);
				Tracing.tracer.TraceEvent(buffer);
			}
		}

		private static Guid[] messageGuids = new Guid[]
		{
			new Guid("{44f79c5d-d950-a1cb-e41b-1c3107db2b1e}"),
			new Guid("{2a428b2b-bb10-1c28-337e-baba1409df62}"),
			new Guid("{f51b3d09-494f-6609-efb6-3c5c508587a8}"),
			new Guid("{013e9f2f-7ec0-df47-5ad0-bff73960c1ba}"),
			new Guid("{4592ee03-f1d3-3388-d511-57e6503e0375}"),
			new Guid("{f57b0ef8-fc7e-fc8f-4b81-93bfcdfda744}"),
			new Guid("{e7cd1550-59cb-8012-155e-e1dbddb74ff4}"),
			new Guid("{428165db-6e1e-0924-45e6-0521916dab5e}")
		};
	}
}
