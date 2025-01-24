// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;

namespace SalticosAdmin.Areas.Identity.Pages.Account
{
    public class ForgotPasswordModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IEmailSender _emailSender;

        public ForgotPasswordModel(UserManager<IdentityUser> userManager, IEmailSender emailSender)
        {
            _userManager = userManager;
            _emailSender = emailSender;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [EmailAddress]
            public string Email { get; set; }
        }

        //public async Task<IActionResult> OnPostAsync()
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var user = await _userManager.FindByEmailAsync(Input.Email);
        //        if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
        //        {
        //            // Don't reveal that the user does not exist or is not confirmed
        //            return RedirectToPage("./ForgotPasswordConfirmation");
        //        }

        //        // For more information on how to enable account confirmation and password reset please
        //        // visit https://go.microsoft.com/fwlink/?LinkID=532713
        //        var code = await _userManager.GeneratePasswordResetTokenAsync(user);
        //        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
        //        var callbackUrl = Url.Page(
        //            "/Account/ResetPassword",
        //            pageHandler: null,
        //            values: new { area = "Identity", code },
        //            protocol: Request.Scheme);

        //        await _emailSender.SendEmailAsync(
        //            Input.Email,
        //            "Reset Password",
        //            $"Please reset your password by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

        //        return RedirectToPage("./ForgotPasswordConfirmation");
        //    }

        //    return Page();
        //}




        //public async Task<IActionResult> OnPostAsync()
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return Page();
        //    }

        //    var user = await _userManager.FindByEmailAsync(Input.Email);
        //    //if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
        //    if (user == null)
        //    {
        //        // No revelar si el usuario no existe o no está confirmado
        //        return RedirectToPage("./ForgotPasswordConfirmation");
        //    }

        //    // Generar el token de restablecimiento de contraseña
        //    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        //    var callbackUrl = Url.Page(
        //        "/Account/ResetPassword",
        //        pageHandler: null,
        //        values: new { area = "Identity", code = token, email = Input.Email },
        //        //values: new { area = "Identity", token, email = Input.Email },
        //        protocol: Request.Scheme);

        //    // Enviar el correo
        //    await _emailSender.SendEmailAsync(
        //        Input.Email,
        //        "Restablecer contraseña",
        //        $"Por favor, restablezca su contraseña haciendo <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clic aquí</a>.");

        //    return RedirectToPage("./ForgotPasswordConfirmation");
        //}


        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.FindByEmailAsync(Input.Email);
            if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
            {
                // No revelar si el usuario no existe o no está confirmado
                return RedirectToPage("./ForgotPasswordConfirmation");
            }

            // Generar el token de restablecimiento de contraseña
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var callbackUrl = Url.Page(
                "/Account/ResetPassword",
                pageHandler: null,
                values: new { area = "Identity", code = token, email = Input.Email },
                protocol: Request.Scheme);

            // Crear contenido del correo
            var emailBody = $@"
                <div style='font-family: Arial, sans-serif; line-height: 1.6; background-color: #f9f9f9; padding: 20px; border-radius: 10px; max-width: 600px; margin: auto;'>
                    <h2 style='color: #4CAF50; text-align: center;'>Restablecimiento de Contraseña</h2>
                    <p style='font-size: 16px; color: #333;'>Hola,</p>
                    <p style='font-size: 16px; color: #333;'>
                        Hemos recibido una solicitud para restablecer la contraseña de tu cuenta. Si fuiste tú quien realizó esta solicitud, haz clic en el siguiente botón para continuar:
                    </p>
                    <div style='text-align: center; margin: 20px 0;'>
                        <a href='{HtmlEncoder.Default.Encode(callbackUrl)}' 
                           style='background-color: #007bff; color: white; text-decoration: none; padding: 10px 20px; border-radius: 5px; font-size: 16px;'>
                            Restablecer Contraseña
                        </a>
                    </div>
                    <p style='font-size: 14px; color: #555;'>
                        Si no realizaste esta solicitud, puedes ignorar este correo. Tu cuenta permanecerá segura.
                    </p>
                    <hr style='border: none; border-top: 1px solid #ccc; margin: 20px 0;' />
                    <p style='font-size: 12px; color: #999; text-align: center;'>
                        Este es un mensaje automático. Por favor, no respondas a este correo.
                    </p>
                </div>";


            // Enviar el correo
            await _emailSender.SendEmailAsync(
                Input.Email,
                "Solicitud para Restablecer Contraseña",
                emailBody);

            return RedirectToPage("./ForgotPasswordConfirmation");
        }


    }
}
