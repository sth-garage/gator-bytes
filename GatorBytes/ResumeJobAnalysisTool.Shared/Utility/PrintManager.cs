using System;
using System.IO;
using System.Drawing;
using System.Drawing.Printing;

namespace ResumeJobAnalysisTool.Shared.Utility
{

    public class PrintManager
    {
        private Font printFont;
        private StreamReader streamToPrint;

        public PrintManager()
        {
        }

        // The PrintPage event is raised for each page to be printed.
        private void pd_PrintPage(object sender, PrintPageEventArgs ev)
        {
            float linesPerPage = 0;
            float yPos = 0;
            int count = 0;
            float leftMargin = ev.MarginBounds.Left;
            float topMargin = ev.MarginBounds.Top;
            String line = null;

            // Calculate the number of lines per page.
            linesPerPage = ev.MarginBounds.Height /
               printFont.GetHeight(ev.Graphics);

            // Iterate over the file, printing each line.
            while (count < linesPerPage &&
               ((line = streamToPrint.ReadLine()) != null))
            {
                yPos = topMargin + (count * printFont.GetHeight(ev.Graphics));
                ev.Graphics.DrawString(line, printFont, Brushes.Black,
                   leftMargin, yPos, new StringFormat());
                count++;
            }

            // If more lines exist, print another page.
            if (line != null)
                ev.HasMorePages = true;
            else
                ev.HasMorePages = false;
        }

        // Print the file.
        public void Print(string filePath, string outputPath, int delay = 500)
        {
            try
            {
                streamToPrint = new StreamReader(filePath);
                try
                {
                    printFont = new Font("Arial", 10);
                    PrinterSettings settings = new PrinterSettings()
                    {
                        // set the printer to 'Microsoft Print to PDF'
                        PrinterName = "Microsoft Print to PDF",

                        // tell the object this document will print to file
                        PrintToFile = true,

                        // set the filename to whatever you like (full path)
                        PrintFileName = outputPath,
                    };
                    PrintDocument pd = new PrintDocument();
                    pd.PrinterSettings = settings;
                    pd.PrintPage += new PrintPageEventHandler(pd_PrintPage);


                    // Print the document.
                    pd.Print();
                }
                finally
                {
                    streamToPrint.Close();
                }

                Thread.Sleep(delay);
            }
            catch (Exception ex)
            {

            }
        }
    }
}