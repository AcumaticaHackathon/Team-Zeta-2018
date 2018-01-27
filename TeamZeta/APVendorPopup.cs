using PX.Data;
using System;
using PX.Objects.AP;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamZeta
{
   
    [System.SerializableAttribute()]
    public partial class APVendorPopup : IBqlTable
    {
        #region DocumentDate

        public abstract class documentDate : PX.Data.IBqlField
        {
        }
        [PXDefault(typeof(AccessInfo.businessDate))]
        [PXDate()]
        [PXUIField(DisplayName = "Document Date")]
        public virtual DateTime? DocumentDate { get; set; }

        #endregion

        #region VendorID
        public abstract class vendorID : PX.Data.IBqlField { }
        [PXString(15, IsUnicode = true)]
        [PXUIField(DisplayName = "Vendor", Required = true)]
        [PXSelector(typeof(Vendor.bAccountID),
            SubstituteKey = typeof(Vendor.acctCD),
            DescriptionField = typeof(Vendor.acctName))]
        
        public virtual string VendorID { get; set; }
        #endregion
 
        #region RefNbr
        public abstract class refNbr : PX.Data.IBqlField
        {
        }
        [PXString(200, IsUnicode = true)]
        [PXUIField(DisplayName = "RefNbr")]
        public virtual string RefNbr { get; set; }

        #endregion

    }
}
