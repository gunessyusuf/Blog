﻿    #nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
    using DataAccess;
using Business.Services;
using Business.Models;
using System.Net;
using Microsoft.AspNetCore.Authorization;

//Generated by ScaffoldApp.
namespace WebApi.Controllers
{
    //[Route("api/Tagler")]
    [Route("api/[controller]")] // Tags
    [ApiController]
    [Authorize]
    public class TagsController : ControllerBase
    {
        // Add service injections here
        private readonly ITagService _tagService;

        public TagsController(ITagService tagService)
        {
            _tagService = tagService;
        }

        // GET: api/Tags/List
        [HttpGet("List")]
        public IActionResult GetList()
        {
            List<TagModel> tagList = _tagService.GetList();
            return Ok(tagList);
        }

        // GET: api/Tags/Details/5
        [HttpGet("Details/{id}")]
        public IActionResult GetDetails(int id)
        {
            TagModel tag = _tagService.GetItem(id);
			if (tag == null)
            {
                return NotFound();
            }
			return Ok(tag);
        }

		// POST: api/Tags/Create
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("Create")]
        public IActionResult CreateTag(TagModel tag)
        {
            if (ModelState.IsValid)
            {
                var result = _tagService.Add(tag);
                if (result.IsSuccessful)
                    return CreatedAtAction("GetDetails", new { id = tag.Id }, tag);
                ModelState.AddModelError("Message", result.Message);
            }
            return BadRequest(ModelState);
        }

        // PUT: api/Tags
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("Update")]
        public IActionResult UpdateTag(TagModel tag)
        {
            if (ModelState.IsValid)
            {
                var result = _tagService.Update(tag);
                if (result.IsSuccessful)
                {
                    //return NoContent();
                    return StatusCode(204, "Update successful.");
                }
                ModelState.AddModelError("Message", result.Message);
            }
            //return BadRequest(ModelState);
            return StatusCode(400, ModelState);
        }

        // DELETE: api/Tags/5
        [HttpDelete("Remove/{id}")]
        public IActionResult RemoveTag(int id)
        {
            _tagService.Delete(id);
            return NoContent();
        }
	}
}
