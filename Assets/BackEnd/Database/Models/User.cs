public enum Role
{
    User = 1,
    Manager = 2,
    Admin = 3
}

[System.Serializable]
public class User
{
    public int id;
    public string name;
    public string email;
    public string password;
    public string enterprise;
    public Role role;
}
