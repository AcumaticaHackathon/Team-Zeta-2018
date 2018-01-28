using PX.Data;
using PX.Objects.AP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamZeta
{
    public class APInvoiceExt : PXCacheExtension<APInvoice>
    {
        #region UsrFileURL

        [PXString]
        [PXUIField(DisplayName = "FiledURL", Visibility = PXUIVisibility.Visible, Visible = false)]
        public virtual string UsrFileURL
        {
            get; set;
        }
        public abstract class usrFileURL : IBqlField { }

        #endregion
    }
}
