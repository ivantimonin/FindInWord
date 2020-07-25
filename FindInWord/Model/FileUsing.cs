using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Application = Microsoft.Office.Interop.Word.Application;
using Document = Microsoft.Office.Interop.Word.Document;

namespace FindInWord.Model
{
    class FileUsing
    {
        public static void Openfile(string FileName)
        {
            Application app = new Application();
            Document doc = app.Documents.Open(FileName);
            try
            {
                app.Documents.Open(FileName);
            }
            catch (Exception ex)
            {
                doc.Close();
                app.Quit();
                MessageBox.Show(ex.Message);
            }
        }

        public static string OpenWordprocessingDocumentReadonly(string filepath)
        {
            try
            {
                // Open a WordprocessingDocument based on a filepath.
                using (WordprocessingDocument wordDocument =
                    WordprocessingDocument.Open(filepath, false))
                {
                    // Assign a reference to the existing document body.  
                    Body body = wordDocument.MainDocumentPart.Document.Body;
                    //text of Docx file 
                    return body.InnerText.ToString();
                }

            }
            catch (Exception ex)
            {
                //MessageBox.Show($"{ex.Message} Файл{filepath}");
                return "-1";
            }
        }
    }
}
