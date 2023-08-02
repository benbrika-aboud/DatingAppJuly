using API.Extensions;

namespace API.Entities
{
    public class AppUser
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public string KnownAs { get; set; }
        public DateTime Created { get; set; } = DateTime.UtcNow;
        public DateTime LastActive { get; set; } = DateTime.UtcNow;
        public string Gender { get; set; }
        public string Introduction { get; set; }
        public string LookingFor { get; set; }
        public string Interests { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public List<Photo> Photos { get; set; } = new List<Photo>();

        //public int GetAge()
        //{
        //    return DateOfBirth.CalculateAge();
        //}

        
        //on aurait pu faire comme ca pour PhotoUrl property in MemberDto mais on a prefere utiliser AutoMapper
        //public string GetPhotoUrl()
        //{
        //    var photo = Photos.FirstOrDefault(p => p.IsMain);
        //    if (photo != null)
        //        return photo.Url;

        //    return "";
        //}


    }
}
