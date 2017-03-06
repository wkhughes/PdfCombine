using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PdfCombine
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 3)
            {
                Console.WriteLine("Usage: PdfCombine inputFile1 inputFile2 [inputFiles...] outputFile");
                return;
            }

            var inputFilePaths = args.Take(args.Length - 1);
            var outputFilePath = args.Last();

            CombineFiles(inputFilePaths, outputFilePath);
            Console.WriteLine($"Written combined PDF to {outputFilePath}");
        }

        private static void CombineFiles(IEnumerable<String> inputFilePaths, String outputFilePath)
        {
            using (var outputFile = new FileStream(outputFilePath, FileMode.Create))
            using (var document = new Document())
            using (var writer = new PdfCopy(document, outputFile))
            {
                document.Open();

                foreach (var inputFilePath in inputFilePaths)
                {
                    AppendFilePages(writer, inputFilePath);
                }
            }
        }

        private static void AppendFilePages(PdfCopy writer, string filePath)
        {
            using (var reader = new PdfReader(filePath))
            {
                var pages = Enumerable.Range(1, reader.NumberOfPages).Select(i => writer.GetImportedPage(reader, i));

                foreach (var page in pages)
                {
                    writer.AddPage(page);
                }

                writer.FreeReader(reader);
            }
        }
    }
}
