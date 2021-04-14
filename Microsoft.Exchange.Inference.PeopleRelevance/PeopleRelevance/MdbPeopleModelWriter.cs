using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Diagnostics.Components.Inference;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Inference.Mdb;
using Microsoft.Exchange.Inference.MdbCommon;
using Microsoft.Exchange.Search.Core.Abstraction;

namespace Microsoft.Exchange.Inference.PeopleRelevance
{
	internal sealed class MdbPeopleModelWriter : InferenceBaseModelWriter<MdbPeopleModelDataBinder, PeopleModelItem>
	{
		internal MdbPeopleModelWriter(IPipelineComponentConfig config, IPipelineContext context) : base(config, context)
		{
			this.DiagnosticsSession.ComponentName = "MdbPeopleModelWriter";
			this.DiagnosticsSession.Tracer = ExTraceGlobals.MdbInferenceModelWriterTracer;
		}

		public override string Description
		{
			get
			{
				return "MdbPeopleModelWriter is responsible for persisting the people inference model item to the Mdb.";
			}
		}

		public override string Name
		{
			get
			{
				return "MdbPeopleModelWriter";
			}
		}

		protected override void PrepareModelItem(IDocument document, PeopleModelItem modelItem)
		{
			object obj;
			if (document.TryGetProperty(PeopleRelevanceSchema.ContactList, out obj))
			{
				List<IInferenceRecipient> list = (obj as IDictionary<string, IInferenceRecipient>).Values.ToList<IInferenceRecipient>();
				modelItem.ContactList = list;
				if (list.Count<IInferenceRecipient>() > 0)
				{
					DateTime dateTime = list.Max((IInferenceRecipient x) => x.LastSentTime);
					if (dateTime > modelItem.LastProcessedMessageSentTime)
					{
						modelItem.LastProcessedMessageSentTime = dateTime;
					}
				}
			}
			if (document.TryGetProperty(PeopleRelevanceSchema.CurrentTimeWindowStartTime, out obj))
			{
				modelItem.CurrentTimeWindowStartTime = ((ExDateTime)obj).UniversalTime;
				modelItem.CurrentTimeWindowNumber = document.GetProperty<long>(PeopleRelevanceSchema.CurrentTimeWindowNumber);
			}
			if (document.TryGetProperty(PeopleRelevanceSchema.LastRecipientCacheValidationTime, out obj))
			{
				modelItem.LastRecipientCacheValidationTime = ((ExDateTime)obj).UniversalTime;
			}
			document.SetProperty(PeopleRelevanceSchema.PeopleModelVersion, modelItem.Version);
			modelItem.IsDefaultModel = false;
		}

		protected override MdbPeopleModelDataBinder GetModelDataBinder(object context)
		{
			DocumentProcessingContext documentProcessingContext = context as DocumentProcessingContext;
			if (documentProcessingContext != null)
			{
				return MdbPeopleModelDataBinderFactory.Current.CreateInstance(documentProcessingContext.Session);
			}
			throw new NullDocumentProcessingContextException();
		}

		protected override void WriteModelItem(MdbPeopleModelDataBinder dataBinder, PeopleModelItem modelItem)
		{
			MdbModelUtils.WriteModelItem<PeopleModelItem, MdbPeopleModelDataBinder>(dataBinder, modelItem);
		}

		protected override PeopleModelItem GetModelItem(MdbPeopleModelDataBinder dataBinder)
		{
			return MdbModelUtils.GetModelItem<PeopleModelItem, MdbPeopleModelDataBinder>(dataBinder);
		}

		private const string ComponentName = "MdbPeopleModelWriter";

		private const string ComponentDescription = "MdbPeopleModelWriter is responsible for persisting the people inference model item to the Mdb.";
	}
}
