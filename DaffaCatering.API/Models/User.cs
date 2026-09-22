using System;
using System.Collections.Generic;

namespace DaffaCatering.API.Models;

public partial class User
{
    public string IdUser { get; set; } = null!;

    public string IdRole { get; set; } = null!;

    public string NamaUser { get; set; } = null!;

    public string Password { get; set; } = null!;

    public bool Status { get; set; }
}