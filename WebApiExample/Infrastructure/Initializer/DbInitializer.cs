using IdentityModel;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using IDGFAuth.Data;
using IDGFAuth.Data.Entities;

namespace IDGFAuth.Infrastructure.Initializer
{
    public class DbInitializer : IDbInitializer
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IDGFAuthDbContextSQL _dbContext;

        public DbInitializer(IDGFAuthDbContextSQL dbContext,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {

            _userManager = userManager;
            _roleManager = roleManager;
            _dbContext = dbContext;
        }
        public void Initialize()
        {
            if (_roleManager.FindByNameAsync(GlobalRoles.Admin).Result == null)
            {
                _roleManager.CreateAsync(new IdentityRole(GlobalRoles.Admin)).GetAwaiter().GetResult();
            }
            if (_userManager.FindByNameAsync("Admin")?.Result == null)
            {
                ApplicationUser adminUser = new ApplicationUser()
                {
                    UserName = "Admin",
                    Email = "admin@gmail.com",
                    EmailConfirmed = true,
                    FirstName = "کاربر",
                    LastName = "مدیر سیستم"
                };

                _userManager.CreateAsync(adminUser, "Admin123*").GetAwaiter().GetResult();
                _userManager.AddToRoleAsync(adminUser, GlobalRoles.Admin).GetAwaiter().GetResult();
                _userManager.AddClaimsAsync(adminUser, new Claim[]
                {
                new Claim(JwtClaimTypes.Subject,adminUser.Email),
                new Claim(JwtClaimTypes.Email,adminUser.Email),
                }).GetAwaiter().GetResult();
            }
            if (_userManager.FindByNameAsync("shokouhi")?.Result == null)
            {
                ApplicationUser user = new ApplicationUser()
                {
                    UserName = "shokouhi",
                    Email = "shokouhi@gmail.com",
                    EmailConfirmed = true,
                    FirstName = "شکری",
                    LastName = "shokouhi"
                };
                _userManager.CreateAsync(user, "Admin123*").GetAwaiter().GetResult();
                _userManager.AddToRoleAsync(user, "Nazer").GetAwaiter().GetResult();
        
                ApplicationUser user16 = new ApplicationUser()
                {
                    UserName = "Talebi",
                    Email = "Talebi@gmail.com",
                    EmailConfirmed = true,
                    FirstName = "طالبی",
                    LastName = "Talebi"
                };
                _userManager.CreateAsync(user16, "Admin123*").GetAwaiter().GetResult();
                _userManager.AddToRoleAsync(user16, "Nazer").GetAwaiter().GetResult();
                ApplicationUser user2 = new ApplicationUser()
                {
                    UserName = "dadgar",
                    Email = "dadgar@gmail.com",
                    EmailConfirmed = true,
                    FirstName = "دادگر",
                    LastName = "dadgar"
                };
                _userManager.CreateAsync(user2, "Admin123*").GetAwaiter().GetResult();
                _userManager.AddToRoleAsync(user2, "Nazer").GetAwaiter().GetResult();
                ApplicationUser user3 = new ApplicationUser()
                {
                    UserName = "hosseini",
                    Email = "hosseini@gmail.com",
                    EmailConfirmed = true,
                    FirstName = "حسینی",
                    LastName = "hosseini"
                };
                _userManager.CreateAsync(user3, "Admin123*").GetAwaiter().GetResult();
                _userManager.AddToRoleAsync(user3, "Nazer").GetAwaiter().GetResult();
                ApplicationUser user4 = new ApplicationUser()
                {
                    UserName = "hayati",
                    Email = "hayati@gmail.com",
                    EmailConfirmed = true,
                    FirstName = "حیاتی",
                    LastName = "hayati"
                };
                _userManager.CreateAsync(user4, "Admin123*").GetAwaiter().GetResult();
                _userManager.AddToRoleAsync(user4, "Nazer").GetAwaiter().GetResult();
                ApplicationUser user5 = new ApplicationUser()
                {
                    UserName = "m.sh",
                    Email = "m.sh@gmail.com",
                    EmailConfirmed = true,
                    FirstName = "m.sh",
                    LastName = "m.sh"
                };
                _userManager.CreateAsync(user5, "Admin123*").GetAwaiter().GetResult();
                _userManager.AddToRoleAsync(user5, "Nazer").GetAwaiter().GetResult();

                ApplicationUser user6 = new ApplicationUser()
                {
                    UserName = "khosravi",
                    Email = "khosravih@gmail.com",
                    EmailConfirmed = true,
                    FirstName = "khosravi",
                    LastName = "khosravi"
                };
                _userManager.CreateAsync(user6, "Admin123*").GetAwaiter().GetResult();
                _userManager.AddToRoleAsync(user6, "Nazer").GetAwaiter().GetResult();

                ApplicationUser user7 = new ApplicationUser()
                {
                    UserName = "mobahat",
                    Email = "mobahat@gmail.com",
                    EmailConfirmed = true,
                    FirstName = "مباهات",
                    LastName = "mobahat"
                };
                _userManager.CreateAsync(user7, "Admin123*").GetAwaiter().GetResult();
                _userManager.AddToRoleAsync(user7, "Nazer").GetAwaiter().GetResult();

                ApplicationUser user15 = new ApplicationUser()
                {
                    UserName = "a.mobahat",
                    Email = "a.mobahat@gmail.com",
                    EmailConfirmed = true,
                    FirstName = "مباهات",
                    LastName = "a.mobahat"
                };
                _userManager.CreateAsync(user15, "Admin123*").GetAwaiter().GetResult();
                _userManager.AddToRoleAsync(user15, "Nazer").GetAwaiter().GetResult();

                ApplicationUser user8 = new ApplicationUser()
                {
                    UserName = "mohagheghian",
                    Email = "mohagheghian@gmail.com",
                    EmailConfirmed = true,
                    FirstName = "محققیان",
                    LastName = "mohagheghian"
                };
                _userManager.CreateAsync(user8, "Admin123*").GetAwaiter().GetResult();
                _userManager.AddToRoleAsync(user8, "Nazer").GetAwaiter().GetResult();

                ApplicationUser user9 = new ApplicationUser()
                {
                    UserName = "s.majed",
                    Email = "s.majed@gmail.com",
                    EmailConfirmed = true,
                    FirstName = "ماجد",
                    LastName = "s.majed"
                };
                _userManager.CreateAsync(user9, "Admin123*").GetAwaiter().GetResult();
                _userManager.AddToRoleAsync(user9, "Nazer").GetAwaiter().GetResult();

                ApplicationUser user10 = new ApplicationUser()
                {
                    UserName = "a.akrami",
                    Email = "a.akrami@gmail.com",
                    EmailConfirmed = true,
                    FirstName = "اکرمی",
                    LastName = "a.akrami"
                };
                _userManager.CreateAsync(user10, "Admin123*").GetAwaiter().GetResult();
                _userManager.AddToRoleAsync(user10, "Nazer").GetAwaiter().GetResult();

                ApplicationUser user11 = new ApplicationUser()
                {
                    UserName = "m.zaafari",
                    Email = "m.zaafari@gmail.com",
                    EmailConfirmed = true,
                    FirstName = "زفری",
                    LastName = "m.zaafari"
                };
                _userManager.CreateAsync(user11, "Admin123*").GetAwaiter().GetResult();
                _userManager.AddToRoleAsync(user11, "Nazer").GetAwaiter().GetResult();

                ApplicationUser user12 = new ApplicationUser()
                {
                    UserName = "zaafari",
                    Email = "zaafari@gmail.com",
                    EmailConfirmed = true,
                    FirstName = "زفری",
                    LastName = "zaafari"
                };
                _userManager.CreateAsync(user12, "Admin123*").GetAwaiter().GetResult();
                _userManager.AddToRoleAsync(user12, "Nazer").GetAwaiter().GetResult();

                ApplicationUser user13 = new ApplicationUser()
                {
                    UserName = "m.yousefi",
                    Email = "m.yousefi@gmail.com",
                    EmailConfirmed = true,
                    FirstName = "یوسفی",
                    LastName = "m.yousefi"
                };
                _userManager.CreateAsync(user13, "Admin123*").GetAwaiter().GetResult();
                _userManager.AddToRoleAsync(user13, "Nazer").GetAwaiter().GetResult();

                ApplicationUser user14 = new ApplicationUser()
                {
                    UserName = "f.pazoki",
                    Email = "f.pazoki@gmail.com",
                    EmailConfirmed = true,
                    FirstName = "پازوکی",
                    LastName = "f.pazoki"
                };
                _userManager.CreateAsync(user14, "Admin123*").GetAwaiter().GetResult();
                _userManager.AddToRoleAsync(user14, "Nazer").GetAwaiter().GetResult();

            }
        }
    }
}
