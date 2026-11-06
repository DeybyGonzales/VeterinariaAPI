using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VeterinariaAPI.Models;

namespace VeterinariaAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PropietariosController : ControllerBase
{
    private readonly VeterinariaContext _context;

    public PropietariosController(VeterinariaContext context) => _context = context;

    // GET api/propietarios?buscar=lopez   (busca por nombres o DNI)
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Propietario>>> Get(string? buscar)
    {
        var consulta = _context.Propietarios.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(buscar))
            // Collate "AI" = insensible a tildes (lopez encuentra López)
            consulta = consulta.Where(p => EF.Functions.Collate(p.Nombres, "Latin1_General_CI_AI").Contains(buscar)
                                        || p.Dni.Contains(buscar));

        var propietarios = await consulta.OrderBy(p => p.Nombres).ToListAsync();
        return Ok(propietarios);
    }

    // GET api/propietarios/1
    [HttpGet("{id}")]
    public async Task<ActionResult<Propietario>> Get(int id)
    {
        var propietario = await _context.Propietarios.FindAsync(id);
        if (propietario == null) return NotFound();
        return Ok(propietario);
    }

    // GET api/propietarios/1/mascotas   (ruta anidada)
    [HttpGet("{id}/mascotas")]
    public async Task<ActionResult<IEnumerable<Mascota>>> GetMascotas(int id)
    {
        if (!await _context.Propietarios.AnyAsync(p => p.Id == id)) return NotFound();

        var mascotas = await _context.Mascotas
            .AsNoTracking()
            .Where(m => m.PropietarioId == id)
            .OrderBy(m => m.Nombre)
            .ToListAsync();
        return Ok(mascotas);
    }

    // POST api/propietarios
    [HttpPost]
    public async Task<ActionResult<Propietario>> Post(Propietario propietario)
    {
        // Regla de negocio 1: el DNI no puede repetirse
        if (await _context.Propietarios.AnyAsync(p => p.Dni == propietario.Dni))
            return Conflict(new { mensaje = $"Ya existe un propietario con el DNI {propietario.Dni}." });

        propietario.Id = 0;
        propietario.Mascotas = new List<Mascota>();
        _context.Propietarios.Add(propietario);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = propietario.Id }, propietario);
    }

    // PUT api/propietarios/1
    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, Propietario propietario)
    {
        if (id != propietario.Id) return BadRequest();

        var actual = await _context.Propietarios.FindAsync(id);
        if (actual == null) return NotFound();

        // Regla de negocio 1: el DNI no puede repetirse (excluyendo al propio registro)
        if (await _context.Propietarios.AnyAsync(p => p.Dni == propietario.Dni && p.Id != id))
            return Conflict(new { mensaje = $"Ya existe otro propietario con el DNI {propietario.Dni}." });

        actual.Nombres = propietario.Nombres;
        actual.Dni = propietario.Dni;
        actual.Telefono = propietario.Telefono;
        actual.Email = propietario.Email;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    // DELETE api/propietarios/1
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var propietario = await _context.Propietarios.FindAsync(id);
        if (propietario == null) return NotFound();

        // Regla de negocio 2: no se elimina un propietario que aún tiene mascotas registradas
        if (await _context.Mascotas.AnyAsync(m => m.PropietarioId == id))
            return Conflict(new { mensaje = "No se puede eliminar el propietario porque tiene mascotas registradas." });

        _context.Propietarios.Remove(propietario);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
