namespace Cafe.Domain
{
    public sealed class Employee
    {
        public Employee(string name)
        {
            Name = name;
        }

        public string Name { get; }
        public bool IsBusy { get; private set; }

        public void Assign()
        {
            IsBusy = true;
        }

        public void Release()
        {
            IsBusy = false;
        }
    }
}
