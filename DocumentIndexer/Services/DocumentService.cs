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

        private string GetText(WordprocessingDocument doc)
        {
            string documentText = string.Empty;
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

            return documentText;
        }

        public WordDocument ReadDocument(string file)
        {
            if (!File.Exists($"{_folderPath}\\{file}.docx"))
                throw new ApplicationException("Такого файла с расширением .docx не существует");

            string documentText = string.Empty;
            string title = string.Empty;
            var creationDate = DateTime.MinValue;

            try
            {
                using (var doc = WordprocessingDocument.Open($"{_folderPath}\\{file}.docx", false))
                {
                    documentText = GetText(doc);
                }
                
                var fileInfo = new FileInfo($"{_folderPath}\\{file}.docx");

                if (fileInfo.Exists)
                {
                    creationDate = fileInfo.CreationTimeUtc;
                    title = Path.GetFileNameWithoutExtension(fileInfo.Name);
                }

                return new WordDocument
                {
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

        public List<WordDocument> ReadRequiredDocuments(List<string> files)
        {
            if (files == null || !files.Any())
            {
                throw new ApplicationException("Список имён файлов не может быть пустым");
            }

            var wordDocuments = new List<WordDocument>();

            try
            {
                foreach (var file in files)
                {
                    string documentText = string.Empty;
                    string title = string.Empty;
                    var creationDate = DateTime.MinValue;

                    using (var doc = WordprocessingDocument.Open($"{_folderPath}\\{file}.docx", false))
                    {
                        documentText = GetText(doc);
                    }

                    var fileInfo = new FileInfo($"{_folderPath}\\{file}.docx");

                    if (fileInfo.Exists)
                    {
                        creationDate = fileInfo.CreationTimeUtc;
                        title = Path.GetFileNameWithoutExtension(fileInfo.Name);
                    }

                    wordDocuments.Add(new WordDocument
                    {
                        Title = title,
                        Text = documentText,
                        Date = creationDate
                    });
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Произошла ошибка в процессе чтения документов: {ex.Message}");
            }

            return wordDocuments;
        }

        public List<WordDocument> ReadAllDocuments()
        {           
            var wordDocuments = new List<WordDocument>();
          
            try
            {
                var fileNames = Directory.GetFiles(_folderPath, "*.docx").Select(Path.GetFileName).ToList()!;
                foreach (var file in fileNames)
                {
                    string documentText = string.Empty;
                    string title = string.Empty;
                    var creationDate = DateTime.MinValue;

                    using (var doc = WordprocessingDocument.Open($"{_folderPath}\\{file}", false))
                    {
                        documentText = GetText(doc);
                    }

                    var fileInfo = new FileInfo($"{_folderPath}\\{file}");

                    if (fileInfo.Exists)
                    {
                        creationDate = fileInfo.CreationTimeUtc;
                        title = Path.GetFileNameWithoutExtension(fileInfo.Name);
                    }

                    wordDocuments.Add(new WordDocument
                    {
                        Title = title,
                        Text = documentText,
                        Date = creationDate
                    });
                }             
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Произошла ошибка в процессе чтения документов: {ex.Message}");
            }

            return wordDocuments;
        }
    }
}
