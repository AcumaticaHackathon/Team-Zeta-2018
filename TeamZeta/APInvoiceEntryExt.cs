using PX.Data;
using PX.Objects.AP;
using PX.SM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using static TeamZeta.CRCommunicationInboxGraphExtension;

namespace TeamZeta
{
    public class APInvoiceEntryExt : PXGraphExtension<APInvoiceEntry>
    {
        public void APInvoice_RowSelecting(PXCache cache, PXRowSelectingEventArgs e)
        {
            APInvoice row = e.Row as APInvoice;

            if (row == null)
                return;

            UploadFileRevision file = PXSelectJoin<UploadFileRevision,
            InnerJoin<UploadFile,
                On<UploadFileRevision.fileID, Equal<UploadFile.fileID>,
                    And<UploadFileRevision.fileRevisionID, Equal<UploadFile.lastRevisionID>>>,
            InnerJoin<NoteDoc,
                On<NoteDoc.fileID, Equal<UploadFile.fileID>>>>,
            Where<NoteDoc.noteID, Equal<Required<APInvoice.noteID>>,
                And<UploadFile.name, Like<pdfExtension>>>,
            OrderBy<Desc<UploadFileRevision.createdDateTime>>>.Select(Base, Base.Document.Current.NoteID);

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

            }

            row.GetExtension<APInvoiceExt>().UsrFileURL = url;
        }
    }
}

