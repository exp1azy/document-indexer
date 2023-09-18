namespace DocumentIndexer.Models
{
    public class IndexDescription
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public string Text { get; set; }

        public DateTime Date { get; set; }

        public static implicit operator IndexDescription(WordDocument document)
        {
            return new IndexDescription
            {
                Id = document.Id,
                Title = document.Title,
                Text = document.Text,
                Date = document.Date
            };
        }
    }
}
