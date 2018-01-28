using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using PX.Data;

namespace TeamZeta
{
	public static class PDFParser
	{
		public static string[] ExtractTextFromPdf(byte[] pdfFile)
		{
			using (PdfReader reader = new PdfReader(pdfFile))
			{
				StringBuilder text = new StringBuilder();

				for (int i = 1; i <= reader.NumberOfPages; i++)
				{
					text.Append(PdfTextExtractor.GetTextFromPage(reader, i));
				}

				return text.ToString().Split('\n');
			}
		}

		public static byte[] GetFirstFile(PXCache cache, object row)
		{
			Guid[] guids = PXNoteAttribute.GetFileNotes(cache, row);
			if (guids.Length > 0)
			{
				var fm = new PX.SM.UploadFileMaintenance();
				PX.SM.FileInfo fi = fm.GetFile(guids[0]);
				return fi.BinData;
			}
			else
			{
				return null;
			}
		}

		public static string[] ParseFirstFileAsPDF(PXCache cache, object row, PXCache emailCache, object email)
		{
			byte[] File = GetFirstFile(emailCache, email);
			if (File != null)
			{
				return ExtractTextFromPdf(File);
			}

			return null;
		}
	}
}
