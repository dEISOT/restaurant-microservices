using IdentityModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Restaurant.Services.Identity.Models;
using System.Security.Claims;

namespace Restaurant.Services.Identity.Pages.Create;

[SecurityHeaders]
[AllowAnonymous]
public class Index : PageModel
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public Index(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        RoleManager<IdentityRole> roleManager
        )
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
    }

    [BindProperty]
    public InputModel Input { get; set; }

    public async Task<IActionResult> OnGet(string returnUrl)
    {
        

        Input = new InputModel
        {
            ReturnUrl = returnUrl
        };

        return Page();
    }

    public async Task<IActionResult> OnPost(string returnUrl)
    {
        if (ModelState.IsValid)
        {
            var user = new ApplicationUser
            {
                UserName = Input.Username,
                Email = Input.Email,
                EmailConfirmed = true,
                FirstName = Input.FirstName,
                LastName = Input.LastName
            };

            var result = await _userManager.CreateAsync(user, Input.Password);

            if (result.Succeeded)
            {
                if (!_roleManager.RoleExistsAsync(SD.Customer).GetAwaiter().GetResult())
                {
                    var userRole = new IdentityRole
                    {
                        Name = SD.Customer,
                        NormalizedName = SD.Customer,

                    };
                    await _roleManager.CreateAsync(userRole);
                }

                await _userManager.AddToRoleAsync(user, SD.Customer);

                await _userManager.AddClaimsAsync(user, new Claim[]{
                        new Claim(JwtClaimTypes.Name, Input.Username),
                        new Claim(JwtClaimTypes.Email, Input.Email),
                        new Claim(JwtClaimTypes.Role, SD.Customer) });

                var loginresult = await _signInManager.PasswordSignInAsync(Input.Username, Input.Password, false, lockoutOnFailure: true);
                if (loginresult.Succeeded)
                {

                    if (Url.IsLocalUrl(Input.ReturnUrl))
                    {
                        return Redirect(Input.ReturnUrl);
                    }
                    else if (string.IsNullOrEmpty(Input.ReturnUrl))
                    {
                        return Redirect("~/");
                    }
                    else
                    {
                        throw new Exception("Invalid return url");
                    }
                }

            }
        }

        return Page();
    }
}
