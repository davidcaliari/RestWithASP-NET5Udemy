using RestWithASPNETUdemy.Model;

namespace RestWithASPNETUdemy.Services.Implementations
{
    public class PersonServiceImplementation : IPersonService
    {
        private List<Person> persons;
        private Person person;
        private volatile int count;

        public Person Create(Person person)
        {
            return person;
        }

        public void Delete(long id)
        {
            
        }

        public List<Person> FindAll()
        {
            persons = new List<Person>();
            for(int i = 0; i < 8; i++)
            {
                person = MockPerson(i);
                persons.Add(person);
            }
            return persons;
        }

       public Person FindByID(long id)
        {
            return new Person
            {
                Id = IncrementAndGet(),
                FirstName = "David",
                LastName = "Ribeiro",
                Gender = "Male",
                Address = "Belo Horizonte"
            };
        }

        public Person Update(Person person)
        {
            return person;
        }

        private Person MockPerson(int i)
        {
            return new Person
            {
                Id = IncrementAndGet(),
                FirstName = "Person Name" + i,
                LastName = "Person LastNAme" + i,
                Gender = "Male" + i,
                Address = "Some Address" + i,

            };
        }

        private long IncrementAndGet()
        {
            return Interlocked.Increment(ref count);
        }
    }
}
