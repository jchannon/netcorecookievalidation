namespace ConsoleApplication
{
    public class UserDatabase
    {
        private static User User = new User { Name = "Jim", Age = 21 };
        public static User GetUser()
        {
            return User;
        }

        public static void ChangeUser(string name, int age)
        {
            User.Name = name;
            User.Age = age;
        }
    }
}