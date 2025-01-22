using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models.Movies;

[Route("api/companies")]
[ApiController]
public class CompaniesApiController : ControllerBase
{
    private readonly MoviesContext _context;

    public CompaniesApiController(MoviesContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult GetFiltered(string filter)
    {
        var companies = _context.ProductionCompanies
            .Where(o => o.CompanyName.ToLower().Contains(filter.ToLower()))
            .OrderBy(o => o.CompanyName)
            .AsNoTracking()
            .AsEnumerable()
            .Select(o => new
            {
                companyId = o.CompanyId, 
                companyName = o.CompanyName
            });
            

        return Ok(companies);
    }
}