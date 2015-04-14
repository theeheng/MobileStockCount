namespace DomainInterface
{
    public interface IStock
    {
        //[PrimaryKey, AutoIncrement, Column("_id")]
        int Id { get; set; }

        //[MaxLength(8)]
        string Symbol { get; set; }

        string Name { get; set; }
    }
}