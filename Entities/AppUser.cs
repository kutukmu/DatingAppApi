namespace API.Entities
{
    public class AppUser
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public byte[] UserPassword { get; set; }
        public byte[] UserHash { get; set; }


    }
}