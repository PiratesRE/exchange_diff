using System;

namespace Microsoft.Exchange.Diagnostics.Components.Entities
{
	public static class ExTraceGlobals
	{
		public static Trace CommonTracer
		{
			get
			{
				if (ExTraceGlobals.commonTracer == null)
				{
					ExTraceGlobals.commonTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.commonTracer;
			}
		}

		public static Trace ConvertersTracer
		{
			get
			{
				if (ExTraceGlobals.convertersTracer == null)
				{
					ExTraceGlobals.convertersTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.convertersTracer;
			}
		}

		public static Trace ReliableActionsTracer
		{
			get
			{
				if (ExTraceGlobals.reliableActionsTracer == null)
				{
					ExTraceGlobals.reliableActionsTracer = new Trace(ExTraceGlobals.componentGuid, 2);
				}
				return ExTraceGlobals.reliableActionsTracer;
			}
		}

		public static Trace SerializationTracer
		{
			get
			{
				if (ExTraceGlobals.serializationTracer == null)
				{
					ExTraceGlobals.serializationTracer = new Trace(ExTraceGlobals.componentGuid, 3);
				}
				return ExTraceGlobals.serializationTracer;
			}
		}

		public static Trace AttachmentDataProviderTracer
		{
			get
			{
				if (ExTraceGlobals.attachmentDataProviderTracer == null)
				{
					ExTraceGlobals.attachmentDataProviderTracer = new Trace(ExTraceGlobals.componentGuid, 4);
				}
				return ExTraceGlobals.attachmentDataProviderTracer;
			}
		}

		public static Trace CreateAttachmentTracer
		{
			get
			{
				if (ExTraceGlobals.createAttachmentTracer == null)
				{
					ExTraceGlobals.createAttachmentTracer = new Trace(ExTraceGlobals.componentGuid, 5);
				}
				return ExTraceGlobals.createAttachmentTracer;
			}
		}

		public static Trace ReadAttachmentTracer
		{
			get
			{
				if (ExTraceGlobals.readAttachmentTracer == null)
				{
					ExTraceGlobals.readAttachmentTracer = new Trace(ExTraceGlobals.componentGuid, 6);
				}
				return ExTraceGlobals.readAttachmentTracer;
			}
		}

		public static Trace UpdateAttachmentTracer
		{
			get
			{
				if (ExTraceGlobals.updateAttachmentTracer == null)
				{
					ExTraceGlobals.updateAttachmentTracer = new Trace(ExTraceGlobals.componentGuid, 7);
				}
				return ExTraceGlobals.updateAttachmentTracer;
			}
		}

		public static Trace DeleteAttachmentTracer
		{
			get
			{
				if (ExTraceGlobals.deleteAttachmentTracer == null)
				{
					ExTraceGlobals.deleteAttachmentTracer = new Trace(ExTraceGlobals.componentGuid, 8);
				}
				return ExTraceGlobals.deleteAttachmentTracer;
			}
		}

		public static Trace FindAttachmentsTracer
		{
			get
			{
				if (ExTraceGlobals.findAttachmentsTracer == null)
				{
					ExTraceGlobals.findAttachmentsTracer = new Trace(ExTraceGlobals.componentGuid, 9);
				}
				return ExTraceGlobals.findAttachmentsTracer;
			}
		}

		private static Guid componentGuid = new Guid("B3FC667F-1AD2-4377-AC4D-3AE344A739D4");

		private static Trace commonTracer = null;

		private static Trace convertersTracer = null;

		private static Trace reliableActionsTracer = null;

		private static Trace serializationTracer = null;

		private static Trace attachmentDataProviderTracer = null;

		private static Trace createAttachmentTracer = null;

		private static Trace readAttachmentTracer = null;

		private static Trace updateAttachmentTracer = null;

		private static Trace deleteAttachmentTracer = null;

		private static Trace findAttachmentsTracer = null;
	}
}
