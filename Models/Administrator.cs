namespace ArreglaMiCiudad.Models
{
    public class Administrator
    {
        public int UserId { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public char Gender { get; set; }

        public User User { get; set; } = null!;
    }
}
