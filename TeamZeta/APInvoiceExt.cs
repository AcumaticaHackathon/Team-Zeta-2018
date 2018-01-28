using PX.Data;
using PX.Objects.AP;
using PX.SM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace TeamZeta
{
    public class APInvoiceExt : PXCacheExtension<PX.Objects.AP.APInvoice>
    {
        public const string HandlerURL = "Frames/GetFile.ashx?fileID=";
        public class pdfExtension : Constant<string>
        {
            public pdfExtension() : base("%.pdf%") { }
        }


        #region UsrFiledID
        [PXGuid]
        [PXUIField(DisplayName = "FiledID")]
        [PXUnboundDefault(typeof(Search2<UploadFileRevision.fileID,
            InnerJoin<UploadFile,
                On<UploadFileRevision.fileID, Equal<UploadFile.fileID>,
                    And<UploadFileRevision.fileRevisionID, Equal<UploadFile.lastRevisionID>>>,
            InnerJoin<NoteDoc,
                On<NoteDoc.fileID, Equal<UploadFile.fileID>>>>,
            Where<NoteDoc.noteID, Equal<Current<APRegister.noteID>>,
                And<UploadFile.name, Like<pdfExtension>>>,
            OrderBy<Desc<UploadFileRevision.createdDateTime>>>), PersistingCheck = PXPersistingCheck.Nothing)]
        [PXDBScalar(typeof(Search2<UploadFileRevision.fileID,
            InnerJoin<UploadFile,
                On<UploadFileRevision.fileID, Equal<UploadFile.fileID>,
                    And<UploadFileRevision.fileRevisionID, Equal<UploadFile.lastRevisionID>>>,
            InnerJoin<NoteDoc,
                On<NoteDoc.fileID, Equal<UploadFile.fileID>>>>,
            Where<NoteDoc.noteID, Equal<APRegister.noteID>,
                And<UploadFile.name, Like<pdfExtension>>>,
            OrderBy<Desc<UploadFileRevision.createdDateTime>>>))]
        public virtual Guid? UsrFiledID { get; set; }
        public abstract class usrFiledID : IBqlField { }

        #endregion

        #region RootURL

        [PXString]
        [PXUIField(DisplayName = "RootURL")]
        public virtual string RootURL
        {
            get
            {
                if (HttpContext.Current == null)
                    return string.Empty;

                var applicationpath = string.IsNullOrEmpty(HttpContext.Current.Request.ApplicationPath)
                    ? string.Empty
                    : HttpContext.Current.Request.ApplicationPath + "/";
                return HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) + applicationpath;
            }
        }
        public abstract class rootURL : IBqlField { }

        #endregion

        #region UsrFileURL
        private string _UsrFileURL;

        [PXString]
        [PXUIField(DisplayName = "FiledURL", Visibility = PXUIVisibility.Visible, Visible = false)]
        public virtual string UsrFileURL
        {
            get
            {
                return _UsrFileURL ?? string.Concat(RootURL != null ? RootURL : string.Empty, HandlerURL, UsrFiledID.GetValueOrDefault(Guid.Empty).ToString("D"));
            }
            set
            {
                _UsrFileURL = value;
            }
        }
        public abstract class usrFileURL : IBqlField { }

        #endregion


        #region UsrContainerSize

        [PXString]
        [PXUIField(DisplayName = "Container", Visibility = PXUIVisibility.Visible, Visible = false)]
        public virtual string UsrContainerSize { get; set; }
        public abstract class usrContainerSize : IBqlField { }

        #endregion

    }
}
