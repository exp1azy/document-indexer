using DocumentIndexer.Models;
using Nest;

namespace DocumentIndexer.Services
{
    public class ElasticService
    {
        private readonly ElasticClient _elasticClient;
        private readonly string _index;

        public ElasticService(ElasticClient elasticClient, IConfiguration config)
        {
            _elasticClient = elasticClient;
            _index = config.GetSection("ElasticSearch")["Index"]!;
        }

        public async Task IndexAsync(WordDocument wordDocument, CancellationToken cancellationToken)
        {
            try
            {
                await _elasticClient.IndexAsync(wordDocument, idx => idx.Index(_index));
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Ошибка в процессе индексации документа: {ex.Message}");
            }
        }

        public async Task IndexManyAsync(IEnumerable<WordDocument> docs, CancellationToken cancellationToken)
        {
            try
            {
                await _elasticClient.BulkAsync(descr =>
                    descr.IndexMany(docs, (bid, doc) =>
                        bid.Index(_index).Id(doc.Id)), cancellationToken);                   
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Ошибка в процессе индексации нескольких документов: {ex.Message}");
            }
        }
    }
}
