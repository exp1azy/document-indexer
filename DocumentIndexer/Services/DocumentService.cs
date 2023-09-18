using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentIndexer.Models;

namespace DocumentIndexer.Services
{
    public class DocumentService
    {
        private readonly string _folderPath;

        public DocumentService(IConfiguration configuration)
        {
            _folderPath = configuration["FolderPath"]!;
        }

        public WordDocument ReadDocument(string file)
        {
            if (!File.Exists($"{_folderPath}\\{file}.docx"))
                throw new ApplicationException("Такого файла с расширением .docx не существует");

            string documentText = "";
            string title = "";
            var creationDate = DateTime.MinValue;

            try
            {
                using (var doc = WordprocessingDocument.Open($"{_folderPath}\\{file}.docx", false))
                {
                    MainDocumentPart mainPart = doc.MainDocumentPart!;
                    Body body = mainPart.Document.Body!;

                    foreach (Paragraph paragraph in body.Elements<Paragraph>())
                    {
                        foreach (var run in paragraph.Elements<Run>())
                        {
                            foreach (var text in run.Elements<Text>())
                            {
                                documentText += text.Text;
                            }
                        }
                    }
                }

                var fileInfo = new FileInfo($"{_folderPath}\\{file}.docx");

                if (fileInfo.Exists)
                {
                    creationDate = fileInfo.CreationTimeUtc;
                    title = Path.GetFileNameWithoutExtension(fileInfo.Name);
                }

                return new WordDocument
                {
                    Id = Guid.NewGuid().ToString(),
                    Title = title,
                    Text = documentText,
                    Date = creationDate
                };
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Произошла ошибка в процессе чтения документа: {ex.Message}");
            }            
        }

        public List<WordDocument> ReadAllDocuments()
        {           
            var wordDocuments = new List<WordDocument>();
          
            try
            {
                var fileNames = Directory.GetFiles(_folderPath, "*.docx").Select(Path.GetFileName).ToList()!;
                foreach (var file in fileNames)
                {
                    string documentText = "";
                    string title = "";
                    var creationDate = DateTime.MinValue;

                    using (var doc = WordprocessingDocument.Open($"{_folderPath}\\{file}", false))
                    {
                        MainDocumentPart mainPart = doc.MainDocumentPart!;
                        Body body = mainPart.Document.Body!;

                        foreach (Paragraph paragraph in body.Elements<Paragraph>())
                        {
                            foreach (var run in paragraph.Elements<Run>())
                            {
                                foreach (var text in run.Elements<Text>())
                                {
                                    documentText += text.Text;
                                }
                            }
                        }
                    }

                    var fileInfo = new FileInfo($"{_folderPath}\\{file}");

                    if (fileInfo.Exists)
                    {
                        creationDate = fileInfo.CreationTimeUtc;
                        title = Path.GetFileNameWithoutExtension(fileInfo.Name);
                    }

                    wordDocuments.Add(new WordDocument
                    {
                        Id = Guid.NewGuid().ToString(),
                        Title = title,
                        Text = documentText,
                        Date = creationDate
                    });
                }             
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Произошла ошибка в процессе чтения документа: {ex.Message}");
            }

            return wordDocuments;
        }
    }
}
