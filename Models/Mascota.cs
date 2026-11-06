using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace VeterinariaAPI.Models;

[Index("PropietarioId", "Nombre", Name = "UQ_Mascotas_Propietario_Nombre", IsUnique = true)]
public partial class Mascota
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 50 caracteres.")]
    public string Nombre { get; set; } = null!;

    [Required(ErrorMessage = "La especie es obligatoria.")]
    [StringLength(30, MinimumLength = 3, ErrorMessage = "La especie debe tener entre 3 y 30 caracteres.")]
    public string Especie { get; set; } = null!;

    [StringLength(50)]
    public string? Raza { get; set; }

    [Range(0, 40, ErrorMessage = "La edad debe estar entre 0 y 40 años.")]
    public int Edad { get; set; }

    [Range(typeof(decimal), "0.1", "200", ErrorMessage = "El peso debe estar entre 0.1 y 200 kg.")]
    [Column(TypeName = "decimal(5, 2)")]
    public decimal Peso { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Debe indicar un propietario válido.")]
    public int PropietarioId { get; set; }

    // Admite nulo para que el POST/PUT solo envíe PropietarioId.
    [ForeignKey("PropietarioId")]
    [InverseProperty("Mascotas")]
    public virtual Propietario? Propietario { get; set; }
}
