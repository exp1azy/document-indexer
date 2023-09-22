using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentIndexer.Models;
using Nest;
using System.Net;

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

        private async Task<IEnumerable<WordDocument>> SearchAsync(IEnumerable<WordDocument> docs, CancellationToken cancellationToken)
        {
            var response = await _elasticClient.SearchAsync<WordDocument>(s => s
                .Index(_index)
                , cancellationToken
            );

            if (!response.ApiCall.Success)
            {
                throw new ApplicationException(response.ApiCall.OriginalException.Message);
            }

            return response.Documents;
        }

        public async Task<WordDocument?> GetByIdAsync(string id, CancellationToken cancellationToken)
        {
            if (id == null)
            {
                throw new ApplicationException("Id не может быть null");
            }

            var response = await _elasticClient.GetAsync(new DocumentPath<WordDocument>(id).Index(_index), ct: cancellationToken);
            if (response.IsValid)
            {
                return response.Source;
            }

            return null;
        }

        public async Task IndexAsync(WordDocument wordDocument, CancellationToken cancellationToken)
        {
            try
            {
                wordDocument.Id = Guid.NewGuid().ToString();
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
                if (docs == null || !docs.Any())
                {
                    throw new ApplicationException("Список документов на индексирование не может быть пуст");
                }

                foreach (var doc in docs)
                {
                    doc.Id = Guid.NewGuid().ToString();
                }

                await _elasticClient.BulkAsync(descr =>
                    descr.IndexMany(docs, (bid, doc) =>
                        bid.Index(_index).Id(doc.Id)), cancellationToken);                   
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Ошибка в процессе индексации нескольких документов: {ex.Message}");
            }
        }

        public async Task DeleteAsync(IEnumerable<WordDocument> docs, CancellationToken cancellationToken)
        {
            if (docs == null || !docs.Any())
            {
                throw new ApplicationException("Список документов на удаление не может быть пуст");
            }

            var caughtDocs = await SearchAsync(docs, cancellationToken);
            var response = await _elasticClient.BulkAsync(descr => caughtDocs.Aggregate(descr, (d, i) => d.Delete<WordDocument>(dd => dd.Index(_index).Id(i.Id))), cancellationToken);

            if (!response.ApiCall.Success)
            {
                throw new ApplicationException(response.ApiCall.OriginalException.Message);
            }
        }
    }
}
