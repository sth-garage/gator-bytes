using HtmlRendererCore.PdfSharp;
using Microsoft.SemanticKernel;
using PdfSharpCore.Fonts;
using PdfSharpCore.Utils;
using System.ComponentModel;
using System.Diagnostics;

namespace ResumeJobAnalysisTool.Plugins
{
    public class FileHelperPlugin
    {
        [KernelFunction("export_text")]
        public async void ExportText(string textToExport, string filePath)
        {
            File.WriteAllText(filePath, textToExport);
        }

        [KernelFunction("open_file")]
        public async void OpenFile(string filePath)
        {

            using (Process fileopener = new Process())
            {

                fileopener.StartInfo.FileName = "explorer";
                fileopener.StartInfo.Arguments = "\"" + filePath + "\"";
                fileopener.Start();
            }
        }

        [KernelFunction("save_html_to_pdf")]
        public async void SaveHtmlToPDF(string htmlToBeSaved, string filePath)
        {


            GlobalFontSettings.FontResolver = new FontResolver();
            var document = new PdfSharpCore.Pdf.PdfDocument();
            PdfGenerator.AddPdfPages(document, htmlToBeSaved, PdfSharpCore.PageSize.A4);
            document.Save(filePath);
        }


        [KernelFunction("save_to_pdf")]
        [Description("Converts provided rich html with color into a PDF")]
        public async void SaveToPDF(string htmlToBeSaved, string filePath)
        {

            GlobalFontSettings.FontResolver = new FontResolver();
            var document = new PdfSharpCore.Pdf.PdfDocument();

            PdfGenerator.AddPdfPages(document, htmlToBeSaved, PdfSharpCore.PageSize.A4);
            document.Save(filePath);
        }
    }
}
