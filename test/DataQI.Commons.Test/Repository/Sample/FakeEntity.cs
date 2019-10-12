namespace DataQI.Commons.Test.Repository.Sample
{
    public class FakeEntity
    {
        public FakeEntity()
        {
        }

        public FakeEntity(int id) : this(id, null)
        {
        }

        public FakeEntity(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public int Id { get; set; }

        public string Name { get; set; }
    }
}