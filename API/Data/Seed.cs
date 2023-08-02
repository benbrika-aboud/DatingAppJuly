using API.DTOs;
using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace API.Data
{
    public class Seed
    {
        public static async Task SeedUsers(DataContext context)
        {
            //vérifier s'il y a des utilisateurs ne rien faire dutout
            if (await context.Users.AnyAsync()) return;

            // recuperer les donnees depuis le fichier json
            var userData = await File.ReadAllTextAsync("Data/UserSeedData.json");
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            // recuperer les donnees deserialisee dans une variable user
            var users = JsonSerializer.Deserialize<List<AppUser>>(userData);

            // generer des mots de passe haches pour les utilisateurs

            foreach(var user in users)
            {
                using var hmac = new HMACSHA512();
                user.UserName = user.UserName.ToLower();

                user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("123"));
                user.PasswordSalt = hmac.Key;

                context.Users.Add(user);

            }

            await context.SaveChangesAsync();
        }
    }
}
