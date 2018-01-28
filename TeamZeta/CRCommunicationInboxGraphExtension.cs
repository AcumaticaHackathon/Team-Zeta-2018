using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using PX.Data;
using PX.Objects.AP;
using PX.Objects.CR;
using PX.SM;

namespace TeamZeta
{
    public class CRCommunicationInboxGraphExtension : PXGraphExtension<CRCommunicationInbox>
    {
        public override void Initialize()
        {
            Base.Create.AddMenuAction(CreateAPVoucher);
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


                APInvoiceEntryPXInhExt graph = PXGraph.CreateInstance<APInvoiceEntryPXInhExt>();
                graph.Clear();
                APInvoice doc = (APInvoice)graph.Document.Cache.CreateInstance();
                doc.GetExtension<APInvoiceExt>().UsrFileURL = url;
                doc.LineCntr = 0;
                doc = graph.Document.Insert(doc);
                PXNoteAttribute.CopyNoteAndFiles(Base.Emails.Cache, Base.Emails.Current, graph.Document.Cache, doc);
                PXRedirectHelper.TryRedirect(graph, PXRedirectHelper.WindowMode.NewWindow);

            }


            return adapter.Get();
        }

	    private void TryFillValuesFromPDF(PXCache cache, APInvoice invoice)
	    {

	    }


        public PXAction<CRSMEmail> CreateAPVoucher;
        [PXUIField(DisplayName = "AP Voucher", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        [PXButton()]
        public virtual IEnumerable createAPVoucher(PXAdapter adapter)
        {
            List<CRSMEmail> list = new List<CRSMEmail>();
            
            foreach(CRSMEmail email in Base.Emails.Select())
            {
                if (email.Selected.GetValueOrDefault(false))
                {
                    list.Add(email);
                }
            }

            PXLongOperation.StartOperation(this, delegate ()
            {
                createAPVoucherTemplate(Base.Caches[typeof(CRSMEmail)], list);
                //APInvoiceEntry graph = PXGraph.CreateInstance<APInvoiceEntry>();
                //foreach (CRSMEmail email in list)
                //{
                //    graph.Clear();
                //    APInvoice doc = (APInvoice)graph.Document.Cache.CreateInstance();
                //    doc.VendorID = 6995;
                //    doc.InvoiceNbr = "TBD";
                //    PXNoteAttribute.CopyNoteAndFiles(Base.Caches[typeof(CRSMEmail)], email, graph.Document.Cache, graph.Document.Current, true, true);
                //    graph.Document.Insert(doc);
                //    graph.Save.Press();
                //}
            });          
            return adapter.Get();
        }

        public static void createAPVoucherTemplate(PXCache cache, List<CRSMEmail> emailList)
        {
            APInvoiceEntry graph = PXGraph.CreateInstance<APInvoiceEntry>();
            foreach (CRSMEmail email in emailList)
            {
                graph.Clear();
                APInvoice doc = (APInvoice)graph.Document.Cache.CreateInstance();
                doc.VendorID = 6995;
                doc.InvoiceNbr = "TBD";
                graph.Document.Insert(doc);
                graph.Save.Press();
                PXNoteAttribute.CopyNoteAndFiles(cache, email, graph.Document.Cache, graph.Document.Current, true, true);              
                graph.Save.Press();
            }
        }




    }





}
