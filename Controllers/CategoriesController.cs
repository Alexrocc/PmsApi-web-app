using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using PmsApi.DataContexts;
using PmsApi.DTOs;
using PmsApi.Models;

namespace PmsApi.Controllers;

[ApiController]                 //needed to define the controller
[Route("api/categories"), Authorize(Policy = "IsAdmin")]

public class CategoriesController : ControllerBase
{
    private readonly PmsapiContext _context;
    private readonly IMapper _mapper;

    public CategoriesController(PmsapiContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoryDto>>> GetCategories()
    {

        var categories = await _context.ProjectCategories.ToListAsync();
        var categoryDtos = _mapper.Map<IEnumerable<CategoryDto>>(categories);

        return Ok(categoryDtos);
    }

    [HttpGet("{categoryId:int}")]
    public async Task<ActionResult<CategoryDto>> GetCategory([FromRoute] int categoryId)
    {
        ProjectCategory? category = await _context.ProjectCategories.FirstAsync(p => p.CategoryId == categoryId);
        if (category is null)
        {
            return NotFound();
        }
        var categoryDto = _mapper.Map<CategoryDto>(category);
        return Ok(categoryDto);
    }

    [HttpPost]
    public async Task<ActionResult> CreateCategory(CreateCategoryDto categoryDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var category = _mapper.Map<ProjectCategory>(categoryDto);

        try
        {
            _context.ProjectCategories.Add(category);
            await _context.SaveChangesAsync();
            var newCategoryDto = _mapper.Map<CategoryDto>(category);
            // api/projects/{newId} ---> CreatedAtAction() returns the new URL for the resource
            return CreatedAtAction(nameof(GetCategory), new { categoryId = category.CategoryId }, newCategoryDto);
        }
        catch (DbUpdateException ex)
        when (ex.InnerException is MySqlException mySqlException
        && mySqlException.Number == 1062)
        {
            return BadRequest("Category name already exists.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpPut("{categoryId:int}")]
    public async Task<ActionResult> UpdateCategory([FromRoute] int categoryId, [FromBody] CreateCategoryDto categoryDto) //[FromBody] is not necessary for POST and PUT calls, since it is implicitly understood by Entity
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        ProjectCategory? category = await _context.ProjectCategories.FindAsync(categoryId);

        if (category is null)
        {
            return NotFound($"The category with the ID {categoryId} could not be found.");
        }

        _mapper.Map(categoryDto, category);

        try
        {
            await _context.SaveChangesAsync();
            // api/users/{newId} ---> CreatedAtAction() returns the new URL for the resource
            return Ok();
        }
        catch (DbUpdateException ex)
        when (ex.InnerException is MySqlException mySqlException
        && mySqlException.Number == 1062)
        {
            return BadRequest("Category name already taken.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpDelete]
    public async Task<ActionResult> DeleteCategory(int categoryId)
    {
        ProjectCategory? category = await _context.ProjectCategories.FindAsync(categoryId);

        if (category == null)
        {
            return NotFound($"No category found with ID {categoryId}.");
        }
        try
        {
            _context.ProjectCategories.Remove(category);
            await _context.SaveChangesAsync();
            return NoContent();
        }
        catch (DbUpdateException ex)
        when (ex.InnerException is MySqlException)
        {
            return BadRequest("The category has other dependencies. Please delete those first.");
        }
        catch (Exception)
        {
            return StatusCode(500, "An internal error has occoured.");
        }
    }
}