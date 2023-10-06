using MongoDB.Driver;
using Invoicing.Data;
using Invoicing.Data.Entities;

namespace Invoicing.Infrastructure.Data.Repositories
{
    public class FileRepository : BaseMongoDbRepository<File>
    {
        private const string CollectionName = "Files";
        public FileRepository(MongoDbContext context) : base(context)
        {
        }
        protected override IMongoCollection<File> Collection => _context.MongoDatabase.GetCollection<File>(CollectionName);
    }
}
