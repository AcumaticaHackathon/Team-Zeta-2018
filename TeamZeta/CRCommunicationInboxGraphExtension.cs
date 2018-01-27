using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PX.Data;
using PX.Objects.AP;
using PX.Objects.CR;

namespace TeamZeta
{
    public class CRCommunicationInboxGraphExtension : PXGraphExtension<CRCommunicationInbox>
    {
        public override void Initialize()
        {
            Base.Create.AddMenuAction(CreateAPVoucher);
            Base.Create.AddMenuAction(CreateAPPopup);
        }

        public PXFilter<APVendorPopup> APSettings;

        public PXAction<CRSMEmail> CreateAPPopup;
        [PXUIField(DisplayName = "AP Popup", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        [PXButton()]
        public virtual IEnumerable createAPPopup(PXAdapter adapter)
        {
            if (APSettings.AskExt() == WebDialogResult.OK)
            {
                if (APSettings.Current.VendorID != null && !String.IsNullOrEmpty(APSettings.Current.RefNbr))
                {
                    PXLongOperation.StartOperation(this, delegate ()
                    {
                        
                    });
                }
                else
                {
                    throw new PXSetPropertyException<APVendorPopup.vendorID>(
                                       "Please select a Vendor and assign RefNbr.", PXErrorLevel.Error);
                }
            }
            return adapter.Get();
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
