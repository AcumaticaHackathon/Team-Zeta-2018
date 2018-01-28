using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Compilation;
using PX.Data;
using PX.Data.EP;
using PX.Objects.AP;
using PX.Objects.CR;
using PX.SM;

namespace TeamZeta
{
    public class CRCommunicationInboxGraphExtension : PXGraphExtension<CRCommunicationInbox>
    {
        public override void Initialize()
        {
            Base.Create.AddMenuAction(CreateAP);
        }

        public const string HandlerURL = "Frames/GetFile.ashx?fileID=";
        public class pdfExtension : Constant<string>
        {
            public pdfExtension() : base("%.pdf%") { }
        }

        public PXFilter<APVendorPopup> APSettings;

        public PXAction<CRSMEmail> CreateAP;
		[PXUIField(DisplayName = "Create AP Bill", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXButton()]
		public virtual IEnumerable createAP(PXAdapter adapter)
		{
			UploadFileRevision file = PXSelectJoin<UploadFileRevision,
			InnerJoin<UploadFile,
				On<UploadFileRevision.fileID, Equal<UploadFile.fileID>,
					And<UploadFileRevision.fileRevisionID, Equal<UploadFile.lastRevisionID>>>,
			InnerJoin<NoteDoc,
				On<NoteDoc.fileID, Equal<UploadFile.fileID>>>>,
			Where<NoteDoc.noteID, Equal<Required<CRSMEmail.noteID>>,
				And<UploadFile.name, Like<pdfExtension>>>,
			OrderBy<Desc<UploadFileRevision.createdDateTime>>>.Select(Base, Base.Emails.Current.NoteID);

			string url = null;

			if (file != null)
			{
				string rooturl;

				if (HttpContext.Current == null)
					rooturl = string.Empty;

				var applicationpath = string.IsNullOrEmpty(HttpContext.Current.Request.ApplicationPath)
					? string.Empty
					: HttpContext.Current.Request.ApplicationPath + "/";
				rooturl = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) + applicationpath;

				url = string.Concat(rooturl != null ? rooturl : string.Empty, HandlerURL, file.FileID.GetValueOrDefault(Guid.Empty).ToString("D"));

				CRSMEmail email = Base.Emails.Current;

				APInvoiceEntryPXInhExt graph = PXGraph.CreateInstance<APInvoiceEntryPXInhExt>();
				graph.Clear();
				APInvoice doc = (APInvoice)graph.Document.Cache.CreateInstance();
				doc.GetExtension<APInvoiceExt>().UsrFileURL = url;
				doc.LineCntr = 0;
				TryFillValuesFromPDF(graph.Document.Cache, doc, Base.Emails.Cache, email);
				doc = graph.Document.Insert(doc);

				(email).Selected = false;
				CRSMEmail copy = PXCache<CRSMEmail>.CreateCopy(email);

				CRActivity newActivity = (CRActivity)graph.Caches[typeof(CRActivity)].Insert();

				copy.BAccountID = newActivity.BAccountID;
				copy.ContactID = newActivity.ContactID;
				copy.RefNoteID = newActivity.RefNoteID;
				copy.MPStatus = MailStatusListAttribute.Processed;
				copy.Exception = null;
				PXRefNoteSelectorAttribute.EnsureNotePersistence(Base, Base.entityFilter.Current.Type, Base.entityFilter.Current.RefNoteID);
				copy = Base.Emails.Update(copy);
				Base.Save.Press();

				PXNoteAttribute.CopyNoteAndFiles(Base.Emails.Cache, Base.Emails.Current, graph.Document.Cache, doc);
				PXRedirectHelper.TryRedirect(graph, PXRedirectHelper.WindowMode.NewWindow);

			}
			return adapter.Get();
		}

	    private void TryFillValuesFromPDF(PXCache invoiceCache, APInvoice invoice, PXCache emailCache, CRSMEmail email)
	    {
		    string[] content = PDFParser.ParseFirstFileAsPDF(invoiceCache, invoice, emailCache, email);
		    if (content != null)
		    {
			    PXSelectBase<Vendor> select = new PXSelect<Vendor, Where<
				    Vendor.acctCD, Equal<Required<Vendor.acctCD>>,
				    Or<Vendor.acctName, Equal<Required<Vendor.acctName>>>>>(Base);

			    int? vendorID = null;

			    foreach (string phrase in content)
			    {
				    Vendor vendor = select.SelectSingle(phrase.Trim(), phrase.Trim());

				    if (vendor != null)
				    {
					    vendorID = vendor.BAccountID;
					    break;
				    }
			    }

			    invoice.VendorID = vendorID;
		    }
	    }
    }
}
