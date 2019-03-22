namespace Net.Data.Commons.Test.Repository.Sample
{
    public class FakeEntity
    {
        public FakeEntity()
        {
        }

        public FakeEntity(int id) : this(id, null, null)
        {
        }

        public FakeEntity(int id, string name, string lastName)
        {
            Id = id;
            Name = name;
            LastName = lastName;
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public string LastName { get; set; }
    }
}