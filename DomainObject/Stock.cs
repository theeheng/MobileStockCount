using DomainInterface;
using SQLite;

namespace DomainObject
{
    public class Stock : IStock
    {
        [PrimaryKey, AutoIncrement, Column("_id")]
        public int Id { get; set; }

        [MaxLength(8)]
        public string Symbol { get; set; }

        public string Name { get; set; }

        public long TestLongCol { get; set; }
    }
}