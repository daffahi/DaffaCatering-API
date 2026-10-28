using System.ComponentModel.DataAnnotations;

namespace DaffaCatering.Blazor.Models.MasterUser
{
    public class UserModels
    {
        [Required(ErrorMessage = "ID pengguna wajib diisi")]
        [StringLength(10)]
        public string IdUser { get; set; } = "";

        [Required(ErrorMessage = "Role wajib dipilih")]
        [StringLength(10)]
        public string IdRole { get; set; } = "";

        public string NamaRole { get; set; } = "";

        [Required(ErrorMessage = "Nama pengguna wajib diisi")]
        [StringLength(50)]
        public string NamaUser { get; set; } = "";

        // Hanya diisi saat membuat user baru
        [StringLength(255, MinimumLength = 6, ErrorMessage = "Password minimal 6 karakter")]
        public string? Password { get; set; }

        public bool Status { get; set; } = true;
    }

    public class RoleModels
    {
        public string IdRole { get; set; } = "";
        public string NamaRole { get; set; } = "";
    }

    public class ResetPasswordModel
    {
        [Required(ErrorMessage = "Password baru wajib diisi")]
        [StringLength(255, MinimumLength = 6, ErrorMessage = "Password minimal 6 karakter")]
        public string PasswordBaru { get; set; } = "";

        [Required(ErrorMessage = "Konfirmasi password wajib diisi")]
        [Compare(nameof(PasswordBaru), ErrorMessage = "Konfirmasi password tidak sama")]
        public string KonfirmasiPassword { get; set; } = "";
    }
}