using System.ComponentModel.DataAnnotations;

namespace DaffaCatering.API.DTOs.MasterUser
{
    // untuk membuat user baru
    public class UserDto
    {
        [Required(ErrorMessage = "ID User wajib diisi")]
        [StringLength(10, ErrorMessage = "ID User maksimal 10 karakter")]
        public string IdUser { get; set; } = null!;

        [Required(ErrorMessage = "ID Role wajib diisi")]
        [StringLength(10, ErrorMessage = "ID Role maksimal 10 karakter")]
        public string IdRole { get; set; } = null!;

        [Required(ErrorMessage = "Nama User wajib diisi")]
        [StringLength(50, ErrorMessage = "Nama User maksimal 50 karakter")]
        public string NamaUser { get; set; } = null!;

        [Required(ErrorMessage = "Password wajib diisi")]
        [StringLength(255, MinimumLength = 6, ErrorMessage = "Password minimal 6 karakter")]
        public string Password { get; set; } = null!;

        public bool Status { get; set; }
    }

    // untuk mengubah data user (tanpa password)
    public class UserUpdateDto
    {
        [Required(ErrorMessage = "ID Role wajib diisi")]
        [StringLength(10, ErrorMessage = "ID Role maksimal 10 karakter")]
        public string IdRole { get; set; } = null!;

        [Required(ErrorMessage = "Nama User wajib diisi")]
        [StringLength(50, ErrorMessage = "Nama User maksimal 50 karakter")]
        public string NamaUser { get; set; } = null!;

        public bool Status { get; set; }
    }

    // untuk mengganti password user
    public class ResetPasswordDto
    {
        [Required(ErrorMessage = "Password baru wajib diisi")]
        [StringLength(255, MinimumLength = 6, ErrorMessage = "Password minimal 6 karakter")]
        public string PasswordBaru { get; set; } = null!;
    }

    // Data yang dikirim ke client (tanpa password)
    public class UserResponseDto
    {
        public string IdUser { get; set; } = null!;
        public string IdRole { get; set; } = null!;
        public string NamaRole { get; set; } = null!;
        public string NamaUser { get; set; } = null!;
        public bool Status { get; set; }
    }
}